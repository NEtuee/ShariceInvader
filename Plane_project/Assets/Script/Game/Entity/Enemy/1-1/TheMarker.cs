using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheMarker : PlaneBase
{
    public class MarkerHead
    {
        public float d3Angle = 0f;
        public SpriteRenderer spriteRenderer;

        private Sprite[] _sprites;
        private float _dirAngle;

        public void Init(Sprite[] sp,Transform tp)
        {
            _sprites = sp;

            spriteRenderer = new GameObject("Marker").AddComponent<SpriteRenderer>();

            _dirAngle = 360f / (sp.Length - 1);
            spriteRenderer.sprite = sp[0];

            spriteRenderer.transform.SetParent(tp);
        }

        public void UpdateSprite()
        {
            d3Angle = MathEx.clamp360Degree(d3Angle);
            int pos = (int)(d3Angle / _dirAngle);

            spriteRenderer.sortingOrder = d3Angle >= 180f ? 1 : -1;
            spriteRenderer.sprite = _sprites[pos];
        }

        public void SetPosition(Vector3 pos,Vector3 angle)
        {
            spriteRenderer.transform.localPosition = pos;
            spriteRenderer.transform.eulerAngles = angle;
        }
    }

    public float spinSpeed = 180f;


    private List<MarkerHead> _heads = new List<MarkerHead>();
    private float _spinSpeed = 180f;
    private float _markerAngle = 0f;
    private float _markerSpread = 1f;

    private float _spinSpeedTarget = 180f;
    private float _markerSpreadTarget = -5f;
    private float _defenderPosX = -0.15f;
    private float _piercerPosX = 0.65f;
    private float _defenderlerpPosX = 0f;

    private bool _spreading = false;

    private List<WandsBase> _defenders = new List<WandsBase>();
    private List<WandsBase> _piercers = new List<WandsBase>();

    private List<Transform> _headPoints = new List<Transform>();
    private List<Transform> _defenderPoints = new List<Transform>();
    private List<Transform> _piercerPoints = new List<Transform>();

    private Transform _archerPos;
    private WandsBase _archer;

    public override void firstSetting()
    {
        base.firstSetting();


        SetSpriteSet("SpriteSet/Planes/StarFish/starfish_axis",AnimationType.None);
		SetCollider(new Define.SimpleCircleCollider(.22f,.22f,_position));

        for(int i = 0; i < 3; ++i)
        {
            var marker = new MarkerHead();
            marker.Init(ResourceManager.GetInstance().GetSpriteSet("SpriteSet/Planes/StarFish/Marker"),tp);
            marker.SetPosition(_position,new Vector3(Mathf.Sin(_markerAngle * Mathf.Deg2Rad),0f, Mathf.Cos(_markerAngle * Mathf.Deg2Rad)) * _markerSpread);

            _heads.Add(marker);

            var t = new GameObject("headpoint_" + i).transform;
            t.position = _position;
            t.SetParent(tp);

            _headPoints.Add(t);
        }

        _mass = 5f;

        maxHp = _hp = 30;
    }

    public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(1);
        
        for(int i = 0; i < _defenders.Count; ++i)
        {
            _defenders[i].Delete();
        }

        for(int i = 0; i < _piercers.Count; ++i)
        {
            _piercers[i].Delete();
        }

        if(!_archer.deleted)
            _archer.Delete();
	}

    public override void initialize()
    {
        BasicInitialize();

        _directionAngle = false;
        _velocityFlip = false;

        _direction = Vector3.left;
        _speed = 0.1f;
        _maxSpeed = 1f; 
        _gravityScale = 0f;

        RegisteCollisionList();
    }

    public override void BeforeCreated()
    {
        for(int i = 0; i < 4; ++i)
        {
            var wand = ObjectManager.GetInstance().AddObject<Wand_Defender>(Define.ObjectType.enemy,"Defender");
            wand.tp.position = _position;
            wand.SetPositionEm(_position);
            wand.targetPos = _position;
            _defenders.Add(wand);

            var t = new GameObject("headpoint_" + i).transform;
            t.position = _position;
            t.SetParent(tp);

            _defenderPoints.Add(t);
        }

        for(int i = 0; i < 4; ++i)
        {
            var wand = ObjectManager.GetInstance().AddObject<Wand_Piercer>(Define.ObjectType.enemy,"Piercer");
            wand.tp.position = _position;
            wand.SetPositionEm(_position);
            wand.targetPos = _position;
            _piercers.Add(wand);

            var t = new GameObject("headpoint_" + i).transform;
            t.position = _position;
            t.SetParent(tp);

            _piercerPoints.Add(t);
        }

        _archerPos = new GameObject("archerPoint").transform;
        _archerPos.SetParent(tp);
        _archerPos.localPosition = new Vector2(-0.5f,0f);

        var archer = ObjectManager.GetInstance().AddObject<Wand_Archer>(Define.ObjectType.enemy,"Wands");
        archer.SetPositionEm(_archerPos.position);
        archer.targetPos = _archerPos.position;
        _archer = archer;
        _archer.UpdateTransform();

        UpdateTransform();
    }

    public override void progress(float deltaTime)
    {
        MarkerProgress(deltaTime);
        WandsProgress(deltaTime);
        
        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
		BasicUpdate(deltaTime);

        _immortal = _defenders.Count != 0;

        if(Input.GetKeyDown(KeyCode.Z))
        {
            Spread();
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            Fold();
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            ShotPiercer();
        }

    }

    public void ShotPiercer()
    {
        for(int i = 0; i < _piercers.Count; ++i)
        {
            _piercers[i].act = true;
            _piercers[i].transform.SetParent(null);
            _piercers[i].SetDirection(_direction);
            _piercers[i].AddForce(MathEx.RandomCircle(1f).normalized);
            //_piercers[i].SetAbsoluteForce(direction * 100f);
        }
    }

    public void WandProgress(ref List<WandsBase> list, ref List<Transform> points, float x, float dist, float spinDir)
    {
        for(int i = 0; i < list.Count;)
        {
            if(list[i].deleted)
            {
                list.RemoveAt(i);
            }
            else if(!list[i].act)
            {
                float angle = MathEx.clamp360Degree(((_markerAngle * spinDir) + ((360f / list.Count) * i)));
                points[i].localPosition = new Vector3(x,Mathf.Cos(angle * Mathf.Deg2Rad) * dist,0f);//,new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad),0f, Mathf.Cos(angle * Mathf.Deg2Rad)) * _markerSpread;
                list[i].targetPos = points[i].position;
                list[i].mainAngle = MathEx.clamp360Degree(-angle);

                list[i].SetAngle(_eulerAngle);

                var hide = angle >= 180f;
                list[i].SetSortingGroupOrder(hide ? -1 : 1);

                ++i;
            }
            else
            {
                ++i;
            }
        }
    }

    public void WandsProgress(float deltaTime)
    {
        _defenderlerpPosX = Mathf.Lerp(_defenderlerpPosX,_defenderPosX,0.1f);

        WandProgress(ref _defenders,ref _defenderPoints, _defenderlerpPosX, 0.4f, 1f);
        WandProgress(ref _piercers,ref _piercerPoints, _piercerPosX, 0.4f, -1f);


        _archer.targetPos = _archerPos.position;
        _archer.SetAngle(MathEx.directionToAngle((_position - _archer.position).normalized));
        _archer.mainAngle = MathEx.clamp360Degree(_markerAngle);
    }

    public void MarkerProgress(float deltaTime)
    {
        _markerAngle += _spinSpeed * deltaTime;
        _markerAngle = MathEx.clamp360Degree(_markerAngle);

        for(int i = 0; i < _heads.Count; ++i)
        {
            float angle = MathEx.clamp360Degree((_markerAngle + ((360f / _heads.Count) * i)));

            _heads[i].UpdateSprite();
            _headPoints[i].localPosition = new Vector3(0.1f,Mathf.Cos(angle * Mathf.Deg2Rad) * 0.15f,0f);


            _heads[i].SetPosition(_headPoints[i].localPosition,new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad),0f,Mathf.Cos(angle * Mathf.Deg2Rad) * _markerSpread + _eulerAngle));
            _heads[i].d3Angle = angle;
        }

        _spinSpeed = Mathf.Lerp(_spinSpeed,_spinSpeedTarget,0.03f);
        _markerSpread = Mathf.Lerp(_markerSpread,_markerSpreadTarget,0.03f);

        if(_spreading)
        {
            if(_spinSpeed >= (1440f - spinSpeed) / 2f)
            {
                _spinSpeedTarget = spinSpeed;
                _spreading = false;
            }
        }

    }

    public void Spread()
    {
        MarkerSpread(45f);

        _defenderPosX = 0f;
    }

    public void Fold()
    {
        MarkerSpread(-5f);
    }

    public void MarkerSpread(float angle)
    {
        _spinSpeedTarget = 1440f;
        _markerSpreadTarget = angle;
        _defenderPosX = -0.15f;

        _spreading = true;
    }
}
