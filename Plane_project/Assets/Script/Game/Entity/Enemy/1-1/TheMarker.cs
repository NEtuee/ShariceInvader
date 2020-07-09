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
    private float _spreadTimer = 0f;
    private float _sparkleTimer = 0f;

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





    private bool _act = false;
    private bool _shot = false;
    private bool _piercerShot = false;
    private float _shotTimer = 2f;
    private float _actTimer = 0f;
    private float _explosiveTimer = 0f;

    private Vector3 _targetDirection;


    private SoundOption _attackSound;
    private SoundOption _loopSound;




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

        maxHp = _hp = 350;
    }

    public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(this);
        
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
        
        // _loopSound.mainAudioItem.Stop();
        // _loopSound.mainAudioItem.gameObject.SetActive(false);
	}

    public override void initialize()
    {
        BasicInitialize();

        _directionAngle = false;
        _velocityFlip = false;
        _rotateLock = true;

        _direction = Vector3.left;
        _speed = .35f;
        _maxSpeed = 4.3f; 
        _gravityScale = 0f;
        _frictionFactor = 0.03f;

        RegisteCollisionList();

        float rat = Vector2.Distance(_position,CameraControll.instance.position);
		rat = rat < 2f ? 1f : rat;
		if(rat >= 2f) 
		{
			rat = 1f - (MathEx.abs((2f - rat)) * .1f);
		}
        //_loopSound = SoundManager.instance.Play("SE/Marker/loopSound",true,-1,rat);
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
        float rat = Vector2.Distance(_position,CameraControll.instance.position);
		rat = rat < 2f ? 1f : rat;
		if(rat >= 2f) 
		{
			rat = 1f - (MathEx.abs((2f - rat)) * .1f);
		}

        //_loopSound.volRatio = rat;

        MarkerProgress(deltaTime);
        WandsProgress(deltaTime);
        
        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
		BasicUpdate(deltaTime);

        _immortal = _defenders.Count != 0;


        if(!_act)
        {
            var dist = Vector3.Distance(Player.instance.position, position);
            if(dist <= 3f)
            {
                _act = true;
            }

            var angle = MathEx.directionToAngle((Player.instance.position - position).normalized);
            _eulerAngle = Mathf.LerpAngle(_eulerAngle,angle,0.05f);

            _direction = MathEx.angleToDirection(_eulerAngle * Mathf.Deg2Rad);
        }
        else
        {
            if(_shot)
            {
                _shotTimer -= deltaTime;
                _sparkleTimer -= deltaTime;

                if(_sparkleTimer <= 0f)
                {
                    EffectManager.GetInstance().AddEffect(_position + MathEx.RandomCircle(0.08f),"SpriteSet/Effects/Sparkle_small").SetSortingOrder(3);
                    SoundManager.instance.Play("SE/Marker/Beep",false);
                    _sparkleTimer = _shotTimer * 0.2f;
                }

                if(_shotTimer <= 0f)
                {
                    _speed = .35f;

                    _shot = false;
                    _shotTimer = Random.Range(3.5f,4.5f);

                    _attackSound.mainAudioItem.Stop();
                    _attackSound = null;

                    var playerPos = Player.instance.position;
                    var randFactor = 7f * (1f - (_hp / maxHp)) + 4f * (1f - (_defenders.Count / 4f));
                    for(int i = 4; i >= _defenders.Count; --i)
                    {
                        var randomPos = MathEx.RandomVector3(-randFactor,randFactor,0f,0f,0f,0f);
                        EnemyCreator.LaserIndicator(Player.instance.position + randomPos,3f,Random.Range(0f,1f * (1.5f - (_defenders.Count / 4f))));
                    }

                    EffectManager.GetInstance().AddEffect(_position,"SpriteSet/Effects/Sparkle_big").SetSortingOrder(3);
                    SoundManager.instance.Play("SE/Marker/Beep",false);

                    Fold();
                    RandomSpread();
                }

                var angle = MathEx.directionToAngle((Player.instance.position - position).normalized);
                _eulerAngle = Mathf.LerpAngle(_eulerAngle,angle,0.2f);
            }
            else if(_actTimer != 0f)
            {
                _actTimer -= deltaTime;
                if(_actTimer <= 0f)
                {
                    _actTimer = 0f;

                    _act = false;
                }

                float dirangle = Mathf.LerpAngle(_eulerAngle,MathEx.directionToAngle(_targetDirection.normalized),3f * deltaTime);
                _eulerAngle = dirangle;

                _direction = MathEx.angleToDirection(_eulerAngle * Mathf.Deg2Rad);
            }
            else
            {
                _shotTimer -= deltaTime;
                if(_shotTimer <= 0f)
                {
                    _shot = true;
                    _shotTimer = Random.Range(1.5f,2.5f);

                    _sparkleTimer = 0f;
                    _speed = 0f;

                    if(_position.y <= 2.5f)
                    {
                        ChangeDirection(Vector3.up);
                    }

                    _attackSound = SoundManager.instance.Play("SE/Marker/Attack",false);
                    
                    Spread();

                    if(!_piercerShot)
                    {
                        ShotPiercer();
                        SoundManager.instance.Play("SE/Marker/PiercerOn",false);
                    }
                }

                var dist = Vector3.Distance(Player.instance.position, position);

                if(_position.y <= 2f)
                {
                    _targetDirection.y += 1f;
                    _targetDirection = _targetDirection.normalized;

                    float dirangle = Mathf.LerpAngle(_eulerAngle,MathEx.directionToAngle(_targetDirection.normalized),3f * deltaTime);
                    _eulerAngle = dirangle;

                    _direction = MathEx.angleToDirection(_eulerAngle * Mathf.Deg2Rad);
                }
                // else if(dist <= 1.5f)
                // {
                //     _targetDirection = (_position - Player.instance.position).normalized;

                //     float dirangle = Mathf.LerpAngle(_eulerAngle,MathEx.directionToAngle(_targetDirection.normalized),3f * deltaTime);

                //     _direction = MathEx.angleToDirection(dirangle * Mathf.Deg2Rad);
                // }
                else if(dist > 1f)
                {
                    _targetDirection = (Player.instance.position - _position).normalized;

                    float dirangle = Mathf.LerpAngle(_eulerAngle,MathEx.directionToAngle(_targetDirection.normalized),3f * deltaTime);
                    _eulerAngle = dirangle;

                    _direction = MathEx.angleToDirection(_eulerAngle * Mathf.Deg2Rad);
                }

                
            }
        }

        if(_hp < 15)
		{
			_explosiveTimer -= deltaTime;

			if(_explosiveTimer <= 0f)
			{
				_explosiveTimer = Random.Range(0.1f,0.5f);

				Vector3 randPos = new Vector3(Random.Range(-.05f,.05f),Random.Range(-.05f,.05f));

				EffectManager.GetInstance().Explosion(_position + randPos,5,0.2f,0.2f,0.3f);
				EffectManager.GetInstance().AddEffect(_position + randPos,"SpriteSet/Effects/Explosion")
											.SetTarget(this)
											.SetAddPoint(randPos)
											.SetSortingOrder(2).SetAngle(Random.Range(0f,360f));
			
				EffectManager.GetInstance().EmitParticles("ExplosionSmoke",_position + randPos,4);
				//EffectManager.GetInstance().ExplosionSmoke(_position + randPos,_position + randPos + new Vector3(Random.Range(-0.2f,0.2f),Random.Range(-0.2f,0.2f)),0.15f,0.01f,4);
			}
		}

    }


    public void RandomSpread()
    {
        _targetDirection = new Vector3(Random.Range(0,2) == 0 ? -1f : 1f, Random.Range(-0.5f,0.5f));
        if(position.y <= 2f)
        {
            _targetDirection.y += 1f;
        }

        _targetDirection = _targetDirection.normalized;

        _actTimer = Random.Range(2f,5f);

        //BurstActive();
        //SetAbsoluteForce(_direction * 10f);
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

        _piercerShot = true;
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
                list[i].SetSortingGroupOrder(hide ? 1 : -1);

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

        if(Wand_Defender.guardTimer != 0f)
        {
            Wand_Defender.guardTimer -= deltaTime;
            if(Wand_Defender.guardTimer <= 0f)
            {
                Wand_Defender.guardTimer = 0f;
                Wand_Defender.guardFactor = 0;
            }
        }
        
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
            _spreadTimer -= deltaTime;
            if(_spreadTimer <= 0f)
            {
                _spreading = false;
            }
        }

    }

    public void Spread()
    {
        MarkerSpread(45f,800f);

        _defenderPosX = 0f;
    }

    public void Fold()
    {
        MarkerSpread(-5f,180f);
    }

    public void MarkerSpread(float angle, float speed)
    {
        _spinSpeedTarget = speed;
        _markerSpreadTarget = angle;
        _defenderPosX = -0.15f;

        _spreading = true;

        _spreadTimer = 2f;
    }
}
