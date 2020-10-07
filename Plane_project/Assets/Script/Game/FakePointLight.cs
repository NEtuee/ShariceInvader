using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePointLight : Collisionable
{
    public Shader shader;
    private List<Collisionable> _coll = new List<Collisionable>();

    private SortedDictionary<float,Vector3> _angVert = new SortedDictionary<float, Vector3>();
    private List<Vector3> _vert = new List<Vector3>();
    private List<Vector2> _uv = new List<Vector2>();
    private List<int> _indi = new List<int>();

    private MeshFilter _mesh;
    private MeshRenderer _meshRenderer;

    private Mesh _mainMesh;


    private Define.SimpleRect _rect = new Define.SimpleRect(1f,1f);

    private int rayCount = 6;

    private float _timer = 0f;
    private float _radius = 0f;

    private bool _collision = false;

    private float updateTime = 0.01f;

    private Color _mainColor = Color.white;
    private Color _lerpColor = Color.white;

    private ObjectBase _target;

    public override void firstSetting()
    {
        SetCollider(new Define.SimpleCircleCollider(10f,10f,_position));

        _mesh = obj.AddComponent<MeshFilter>();
        _meshRenderer = obj.AddComponent<MeshRenderer>();

        _mainMesh = new Mesh();

        _mesh.mesh = _mainMesh;
        _meshRenderer.material = new Material(shader);
        _meshRenderer.sortingOrder = - 8;
        //_meshRenderer.color = new Color(1f,1f,1f,0.3f);

        type = Define.ObjectType.objects;

        allowMultiCollision = true;

    }

    public override void initialize()
    {

    }

    public FakePointLight Active(Vector3 pos, float radius, float time, Color color)
    {
        RegisteCollisionList();
        _coll.Clear();

        _radius = radius;
        _collider.bound.SetRect(radius,radius);
        _timer = updateTime = time;

        _position = pos;

        _mainColor = _lerpColor = color;
        _lerpColor.a = 0f;

        _target = null;

        UpdateTransform();

        SetActive(true);
        Revive();

        _coll.Clear();
        _angVert.Clear();
        _vert.Clear();
        _indi.Clear();
        _mainMesh.Clear();
        _collisions.Clear();

        _meshRenderer.material.SetColor("_Color",_mainColor);

        return this;
    }

    public FakePointLight SetTarget(ObjectBase obj) {_target = obj; return this;}

    public override void progress(float deltaTime)
    {
        _meshRenderer.material.SetColor("_Color",Color.Lerp(_mainColor,_lerpColor,(updateTime - _timer) / _timer));

        _timer -= deltaTime;
        if(_timer <= 0f)
        {
            Delete();
            SetActive(false);
        }

        if(_target != null)
		{
			if(_target.deleted)
			{
				SetActive(false);

			}
			_position = _target.position;
		}

        // updateTime -= deltaTime;
        // if(_collision)
        // {
        //     _collision = false;
        //     updateTime = 0.01f;
        // }
        // else if(updateTime <= 0f)
        // {
        //     _collision = true;
        //     return;
        // }
        // else
        //     return;

        _angVert.Clear();
        _vert.Clear();
        _uv.Clear();
        _indi.Clear();

        foreach(var col in _coll)
        {
            var dir = (col.position - _position).normalized;
            var colRadius = col.coll.bound.box.x;

            var left = (Vector3.Cross(dir,new Vector3(0f,0f,1f)).normalized * colRadius);// + col.position;
            var right = -left;

            left += col.position;
            right += col.position;

            for(int i = 1; i < rayCount; ++i)
            {
                var ray = Vector2.Lerp(left,right,(float)i / (float)rayCount);
                var near = ray * 1000f;

                dir = ((Vector3)ray - _position).normalized;
                Vector2 intersection = Vector2.zero;

                foreach(var target in _coll)
                {
                    var end = ray + (Vector2)dir * 100f;

                    if(Define.SimpleCollider.CircleRaycast(target.position,target.coll.bound.box.x,_position,end,out intersection) != 0)
                    {
                        if(Vector2.Distance(_position,near) > 
                            Vector2.Distance(_position,intersection))
                            near = intersection;
                        
                    }
                }

                float ang = MathEx.directionToAngle(dir);

                if(!_angVert.ContainsKey(ang))
                    _angVert.Add(ang,(Vector3)near - _position);
            }

            for(int i = 0; i < 2; ++i)
            {
                var ray = Vector2.Lerp(left,right,(float)i);

                dir = ((Vector3)ray - _position).normalized;
                Vector2 intersection = Vector2.zero;


                var near = _position + dir * _radius;


                foreach(var target in _coll)
                {
                    if(col == target)
                        continue;
                        
                    if(Define.SimpleCollider.CircleRaycast(target.position,target.coll.bound.box.x,_position, ray + (Vector2)dir * 100f,out intersection) != 0)
                    {
                        if(Vector2.Distance(_position,near) > 
                            Vector2.Distance(_position,intersection))
                            near = intersection;
                    }
                }

                float ang = MathEx.directionToAngle(dir);
                if(!_angVert.ContainsKey(ang))
                    _angVert.Add(ang,(Vector3)near - _position);
                //GizmoHelperEx.instance.DrawLine(_position,near,Color.red);
            }

        }


        for(int i = 1; i <= 36; ++i)
        {
            float ang = (float)i * 10f;
            var dir = MathEx.angleToDirection(ang * Mathf.Deg2Rad);

            Vector2 intersection = Vector2.zero;


            var near = _position + dir * _radius;


            foreach(var target in _coll)
            {          
                if(Define.SimpleCollider.CircleRaycast(target.position,target.coll.bound.box.x,
                                                    _position, _position + dir * _radius,out intersection) != 0)
                {
                    if(Vector2.Distance(_position,near) > 
                        Vector2.Distance(_position,intersection))
                        near = intersection;
                }
            }

            if(!_angVert.ContainsKey(ang))
                _angVert.Add(ang,(Vector3)near - _position);

        }


        _vert.Add(Vector3.zero);
        _uv.Add(Vector2.zero);

        bool prog = false;
        int count = 1;

        foreach(var item in _angVert)
        {
            _vert.Add(item.Value);

            _uv.Add(new Vector2(item.Value.magnitude / _radius,1f));

            if(prog)
            {
                _indi.Add(0);
                _indi.Add(count++);
                _indi.Add(count);
            }

            prog = true;
        }

        _indi.Add(0);
        _indi.Add(count++);
        _indi.Add(1);

        CreateMesh();

        _coll.Clear();
    }

    public void CreateMesh()
    {
        _mainMesh.Clear();
        _mainMesh.SetVertices(_vert);
        _mainMesh.SetUVs(0,_uv);
        _mainMesh.SetIndices(_indi.ToArray(),MeshTopology.Triangles,0);
    }

    public override void CollisionProgress(Define.ObjectType type, Collisionable target)
    {
        //if(_collision)
            _coll.Add(target);
    }
}
