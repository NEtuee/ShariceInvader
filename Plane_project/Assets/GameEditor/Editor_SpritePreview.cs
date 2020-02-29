using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Editor_SpritePreview : MonoBehaviour
{
    public Vector2 imageSize;

    public Image image;
    public Text text;
    public RectTransform imageRect;
    private Sprite _sprite;

    private Sprite [] _aniSprite;
    private bool _aniPlay = true;
    private float _timer = 0f;
    private int _aniPos = 0;

    public void Start()
    {
        Editor_FolderItemBase.selectEvent += PathSelectEvent;
    }

    public void Update()
    {
        if(_aniPlay && _aniSprite != null)
        {
            _timer += Time.deltaTime;
            if(_timer >= 1f / 12f)
            {
                _timer = 0f;
                _aniPos = _aniPos == _aniSprite.Length - 1 ? 0 : _aniPos + 1;

                image.sprite = _aniSprite[_aniPos];
            }
        }
    }

    public void PathSelectEvent(Editor_FolderItemBase item)
    {
        LoadSprite(item.filePath);

        _timer = 0f;
        _aniPos = 0;
    }

    public void LoadSprite(string path)
    {
        string [] p = Directory.GetDirectories(path);
        if(p.Length != 0)
        {
            text.text = "This is a classified folder";
            return;
        }

        _aniSprite = ResourceManager.GetInstance().GetSpriteAll(path);

        if(_aniSprite != null)
        {
            text.text = "Total " + _aniSprite.Length;
            _sprite = _aniSprite[0];
            SetSpriteToImage();
        }
        else
        {
            text.text = "File does not exist";
        }
    }

    public void SetSpriteToImage()
    {
        Vector2 spriteSize = new Vector2(_sprite.rect.width,_sprite.rect.height);

        float ratio = 0f;

        if(spriteSize.x > spriteSize.y)
        {
            ratio = imageSize.x / spriteSize.x;
        }
        else
        {
            ratio = imageSize.y / spriteSize.y;
        }

        imageRect.sizeDelta = spriteSize * ratio;
        image.sprite = _sprite;
    }
}
