using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SpriteFontTextMesh : MonoBehaviour
{
    public enum SpriteTextAlignment
    {
        LeftTop,
        LeftCenter,
        LeftBottom,
        CenterTop,
        Center,
        CenterBottom,
        RightTop,
        RightCenter,
        RightBottom
    };

    public string fontPath;
    public string text;
    
    public SpriteTextAlignment alignment;
    public Color textColor;

    public float latterSpace = 0.01f;
    public float spaceDist = 0.1f;

    public Texture2D sprite;

    private string _prevText = "";

    private Sprite[] _charTextures;
    private Mesh _textMesh;
    private Material _textMat;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;

    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _indices = new List<int>();
    private List<Vector2> _uvs = new List<Vector2>();

    private Dictionary<int, int> _charInfos = new Dictionary<int, int>();

    public void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
        _textMat = new Material(Shader.Find("Custom/SpriteTextMesh"));
        _textMat.SetTexture("_MainTex",sprite);
        _textMat.SetFloat("PixelSnap",1f);

        SetCharacterInfo();

        _charTextures = ResourceManager.GetInstance().LoadAll(fontPath,typeof(Sprite)) as Sprite[];
        UnityEngine.Object[] obj = ResourceManager.GetInstance().LoadAll(fontPath, typeof(Sprite));
		if(obj.Length == 0)
		{
			Debug.Log("Font Load Error");
		}

		_charTextures = new Sprite[obj.Length];
		for(int i = 0; i < obj.Length; ++i)
		{
			_charTextures[i] = obj[i] as Sprite;
		}

        _meshRenderer.material = _textMat;
        _textMesh = new Mesh();
        _meshFilter.mesh = _textMesh;

        Convert();
    }

    public void SetText(string s)
    {
        text = s;
        Convert();
    }

    public void Convert()
    {
        if(_meshRenderer == null)
        {
            Initialize();
        }
        if(text == _prevText)
        {
            return;
        }
        int pos = StringCompare(text,_prevText) + 1;

        if(pos != 0)
            MeshCut(pos - 1);
        else
        {
            _vertices.Clear();
            _indices.Clear();
            _uvs.Clear();
        }

        for(int i = pos; i < text.Length; ++i)
        {
            int ascii = (int)text[i];
            if(_charInfos.ContainsKey(ascii))
            {
                AddText(_charInfos[ascii],SpaceCalc(i - 1));
            }
        }

        UpdateMesh();

        _prevText = text;
    }

    public void MeshCut(int pos)
    {
        int p = pos * 4 + 4;
        int c = _vertices.Count;
        _vertices.RemoveRange(p, c - p);
        _uvs.RemoveRange(p, c - p);
        
        p = pos * 6 + 6;
        c = _indices.Count;
        _indices.RemoveRange(p, c - p);
    }

    public int StringCompare(string one, string two)
    {
        if(one == "" || two == "")
            return -1;

        int pos = -1;
        int len = one.Length < two.Length ? one.Length : two.Length;
        for(int i = 0; i < len; ++i)
        {
            if(one[i] != two[i])
            {
                break;
            }

            pos = i;
        }

        return pos;
    }

    public void UpdateMesh()
    {
        if(text.Length == 0)
            _meshRenderer.enabled = false;
        else
        {
            _meshRenderer.enabled = true;
            _textMesh.Clear();
            _textMesh.SetVertices(_vertices);
            _textMesh.SetIndices(_indices.ToArray(),MeshTopology.Triangles,0);
            _textMesh.SetUVs(0,_uvs);

            SetTextAlignment();

            UpdateColor();
        }

    }

    public void UpdateColor()
    {
        _textMat.SetColor("_MainColor",textColor);
    }

    public void SetTextAlignment()
    {
        float x = 0f;
        float y = 0f;

        float boundX = _textMesh.bounds.extents.x;
        float boundY = _textMesh.bounds.extents.y;

        switch(alignment)
        {
            case SpriteTextAlignment.Center:
            x = -boundX;
            y = -boundY;
            break;
            case SpriteTextAlignment.CenterTop:
            x = -boundX;
            y = -boundY * 2f;
            break;
            case SpriteTextAlignment.CenterBottom:
            x = -boundX;
            break;
            case SpriteTextAlignment.LeftTop:
            y = -boundY * 2f;
            break;
            case SpriteTextAlignment.LeftCenter:
            y = -boundY;
            break;
            case SpriteTextAlignment.LeftBottom:
            break;
            case SpriteTextAlignment.RightTop:
            x = -boundX * 2f;
            y = -boundY * 2f;
            break;
            case SpriteTextAlignment.RightCenter:
            x = -boundX * 2f;
            y = -boundY;
            break;
            case SpriteTextAlignment.RightBottom:
            x = -boundX * 2f;
            break;
        }

        _textMat.SetFloat("_AlignmentX",x);
        _textMat.SetFloat("_AlignmentY",y);
    }

    public float SpaceCalc(int pos)
    {
        float count = 0f;
        for(int i = pos; i >= 0; --i)
        {
            if(text[i] != ' ')
                break;
            
            count += spaceDist;
        }

        return count;
    }

    public void AddText(int textPos, float space)
    {
        Vector3 point;
        float width = (float)(_charTextures[textPos].rect.xMax - _charTextures[textPos].rect.xMin) * 0.01f;
        float height = (float)(_charTextures[textPos].rect.yMax - _charTextures[textPos].rect.yMin) * 0.01f;
        if(_vertices.Count == 0)
        {
            point = new Vector3(0f,0f,0f);
        }
        else
        {
            point = _vertices[_vertices.Count - 1];
            point.x += latterSpace;
        }

        point.x += space;

        _vertices.Add(point + new Vector3(0,height));
        _vertices.Add(point + new Vector3(width,height));
        _vertices.Add(point + new Vector3(0f,0f));
        _vertices.Add(point + new Vector3(width,0f));

        int indi = _vertices.Count - 4;
        _indices.Add(indi);
        _indices.Add(indi + 1);
        _indices.Add(indi + 2);
        _indices.Add(indi + 3);
        _indices.Add(indi + 2);
        _indices.Add(indi + 1);

        _uvs.Add(_charTextures[textPos].uv[0]);
        _uvs.Add(_charTextures[textPos].uv[1]);
        _uvs.Add(_charTextures[textPos].uv[2]);
        _uvs.Add(_charTextures[textPos].uv[3]);
    }

    public void SetCharacterInfo()
    {
        _charInfos.Add((int)'0',0);
        _charInfos.Add((int)'1',1);
        _charInfos.Add((int)'2',2);
        _charInfos.Add((int)'3',3);
        _charInfos.Add((int)'4',4);
        _charInfos.Add((int)'5',5);
        _charInfos.Add((int)'6',6);
        _charInfos.Add((int)'7',7);
        _charInfos.Add((int)'8',8);
        _charInfos.Add((int)'9',9);
        _charInfos.Add((int)'A',10);
        _charInfos.Add((int)'B',11);
        _charInfos.Add((int)'C',12);
        _charInfos.Add((int)'D',13);
        _charInfos.Add((int)'E',14);
        _charInfos.Add((int)'F',15);
        _charInfos.Add((int)'G',16);
        _charInfos.Add((int)'H',17);
        _charInfos.Add((int)'I',18);
        _charInfos.Add((int)'J',19);
        _charInfos.Add((int)'K',20);
        _charInfos.Add((int)'L',21);
        _charInfos.Add((int)'M',22);
        _charInfos.Add((int)'N',23);
        _charInfos.Add((int)'O',24);
        _charInfos.Add((int)'P',25);
        _charInfos.Add((int)'Q',26);
        _charInfos.Add((int)'R',27);
        _charInfos.Add((int)'S',28);
        _charInfos.Add((int)'T',29);
        _charInfos.Add((int)'U',30);
        _charInfos.Add((int)'V',31);
        _charInfos.Add((int)'W',32);
        _charInfos.Add((int)'X',33);
        _charInfos.Add((int)'Y',34);
        _charInfos.Add((int)'Z',35);
        _charInfos.Add((int)'.',36);
        _charInfos.Add((int)',',37);
        _charInfos.Add((int)'(',38);
        _charInfos.Add((int)')',39);
        _charInfos.Add((int)'?',40);
        _charInfos.Add((int)'!',41);
        _charInfos.Add((int)'\'',42);
        _charInfos.Add((int)'\"',43);
        _charInfos.Add((int)':',44);
        _charInfos.Add((int)'-',45);
        _charInfos.Add((int)'#',46);
        _charInfos.Add((int)'%',47);

        _charInfos.Add((int)'a',10);
        _charInfos.Add((int)'b',11);
        _charInfos.Add((int)'c',12);
        _charInfos.Add((int)'d',13);
        _charInfos.Add((int)'e',14);
        _charInfos.Add((int)'f',15);
        _charInfos.Add((int)'g',16);
        _charInfos.Add((int)'h',17);
        _charInfos.Add((int)'i',18);
        _charInfos.Add((int)'j',19);
        _charInfos.Add((int)'k',20);
        _charInfos.Add((int)'l',21);
        _charInfos.Add((int)'m',22);
        _charInfos.Add((int)'n',23);
        _charInfos.Add((int)'o',24);
        _charInfos.Add((int)'p',25);
        _charInfos.Add((int)'q',26);
        _charInfos.Add((int)'r',27);
        _charInfos.Add((int)'s',28);
        _charInfos.Add((int)'t',29);
        _charInfos.Add((int)'u',30);
        _charInfos.Add((int)'v',31);
        _charInfos.Add((int)'w',32);
        _charInfos.Add((int)'x',33);
        _charInfos.Add((int)'y',34);
        _charInfos.Add((int)'z',35);
    }

    public void SpriteSetToFontInfo(string path, int width, int height)
    {
        // Sprite[] sprites = ResourceManager.GetInstance().GetSpriteAll(path);
        // List<string> st = new List<string>();

        // for(int i = 0; i < sprites.Length; ++i)
        // {
        //     sprites[i].uv
        //     var w = (float)sprites[i].texture.width / (float)width;
        //     var h = (float)sprites[i].texture.height / (float)height;
        //     st.Add(sprites[i].texture.w)
        // }
    }
}
