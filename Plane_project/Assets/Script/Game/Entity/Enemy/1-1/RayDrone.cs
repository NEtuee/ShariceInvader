using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayDrone : PlaneBase
{

    bool act = false;
    bool shot = false;
    bool deact = false;
    bool flare = false;

    int shotCount = 0;
    int _flareCount = 0;
    int _flareShotCount = 2;

    Vector3 targetDirection;
    float _randomHeight = 3f;
    float _randomDir = 1f;
    float _shotTimer = 0f;
    float _stayTimer = 0f;

    float _flareTimer = 0f;

    Transform[] _flarePos = new Transform[2];
    Transform _shotPos;


	public override void firstSetting()
	{
		base.firstSetting();
		SetSpriteSet("RayDrone/Base",AnimationType.Horizontal);
		SetCollider(new Define.SimpleCircleCollider(.11f,.11f,_position));

		_maxSpeed = Random.Range(2.8f,3f);
		_speed = .4f;
        _gravityScale = 0.3f;
        _mass = 2f;

        _shotPos = new GameObject("shotPos").transform;
        _shotPos.position = new Vector3(0.06f,0f);

        _flarePos[0] = new GameObject("flarePos0").transform;
        _flarePos[1] = new GameObject("flarePos1").transform;

        _flarePos[0].position = new Vector3(-0.07f,-0.04f);
        _flarePos[1].position = new Vector3(0.07f,-0.04f);

        _shotPos.SetParent(tp);
        _flarePos[0].SetParent(tp);
        _flarePos[1].SetParent(tp);


        //_trail.gameObject.SetActive(false);
	}

	public override void initialize()
	{
		BasicInitialize();
		
		SetNoClip(false);

        flare = false;

        _eulerAngle = 0f;

        _randomDir = 1f;
        _randomHeight = 3f;

        RegisteCollisionList();
	}

	public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(1);
	}

	public override void progress(float deltaTime)
	{

        Vector3 pos = Player.instance.position;
        float dist = Vector3.Distance(pos,_position);

        if(shot)
        {
            if(_shotTimer <= 0f)
            {
                Vector3 rand = (_direction + new Vector3(Random.Range(-0.1f,0.1f),Random.Range(-0.1f,0.1f))).normalized;
                BulletManager.GetInstance().Active(BulletType.enemy,_position,rand,_velocity.magnitude + 4f,1,3f)
                                                .NoneDelete()
                                                .SetAngle(MathEx.directionToAngle(rand));

                EffectManager.GetInstance().AddEffect(_shotPos.localPosition,"RayDrone/Fire",false,this,2)
                                            .SetAddPoint(new Vector3(0.06f,0f))
                                            .SetAngle(_eulerAngle);

                if(--shotCount <= 0)
                {
                    shotCount = 0;
                    _shotTimer = 0f;
                    shot = false;
                    deact = true;

                    _flareShotCount = 2;
                }
                else
                {
                    _shotTimer = 0.08f;
                }
            }
            else
            {
                _shotTimer -= deltaTime;
            }
        }
        else
        {
            if(!act)
            {
                Vector3 dirPos = new Vector3(_position.x + _randomDir * dist, _randomHeight);
                targetDirection = (dirPos - _position).normalized;

                _stayTimer += deltaTime;

                if(dist > 6f)
                {
                    act = true;
                    _maxSpeed = Random.Range(3f,4f);
                }
                else if(dist < 3f)
                {
                    if(_stayTimer >= 2.5f)
                    {
                        float dot = Mathf.Cos(Mathf.Deg2Rad * 60f);

                        Vector3 dir = (pos - _position).normalized;
                        if(Vector3.Dot(dir,MathEx.angleToDirection(Mathf.Deg2Rad * angle)) > dot)
                        {
                            shotCount = 5;
                            shot = true;
                        }
                        else if(--_flareShotCount >= 0)
                        {
                            deact = true;
                            flare = true;
                            _flareCount = 6;
                            _flareTimer = 0f;
                            SetAdditionalSpeed(1.2f,1f,true);
                        }
                        else if(_flareShotCount < 0)
                        {
                            deact = true;
                        }
                        
                    }
                }
                else if(_stayTimer >= 4f)
                {
                    act = true;
                    _maxSpeed = Random.Range(3f,4f);
                    SetAdditionalSpeed(3f,0.3f,true);
                }
            }
            else
            {
                Vector3 dir = (pos - _position).normalized;
                Vector3 heightLine = new Vector3(_position.x + dir.x,pos.y < 2f ? 2f : pos.y);
                targetDirection = (heightLine - _position).normalized;

                if(dist < 2.5f)
                {
                    float dot = Mathf.Cos(Mathf.Deg2Rad * 25f);


                    if(Vector3.Dot(dir,MathEx.angleToDirection(Mathf.Deg2Rad * angle)) > dot)
                    {
                        shotCount = 5;
                        shot = true;
                    }
                    else
                    {
                        deact = true;
                        
                    }
                }
            }
        }

        if(deact)
        {
            _randomHeight = Random.Range(2f,ObjectManager.GetInstance()._place._mapHeight);
            _randomDir = Random.Range(0,2) == 0 ? -1f : 1f;
            _stayTimer = 0f;
            act = false;
            deact = false;

            _maxSpeed = Random.Range(2.5f,2.8f);
        }

        if(flare)
        {
            if(_flareTimer <= 0f)
            {
                for(int i = 0; i < 2; ++i)
                {
                    Vector3 dir = Vector3.Cross(_direction,new Vector3(0f,0f,-1f)) * (i % 2 == 0 ? 1f : -1f);

                    var obj = ObjectManager.GetInstance().AddObject<Flare>(Define.ObjectType.enemy,"Flare");
                    obj.target = this;
                    obj.right = (i % 2 == 0);
                    obj.SetPosition(_flarePos[i].position);
                    obj.SetMaxSpeed(_velocity.magnitude - 0.8f);
                    obj.SetAbsoluteForce(_direction * 1000f);
                    obj.SetDirection(_direction);
                    obj.SetAngle(_eulerAngle);
                    obj.AddForce(dir);
                }
                
                if(--_flareCount <= 0)
                {
                    _flareCount = 0;
                    _flareTimer = 0f;
                    flare = false;
                }
                else
                {
                    _flareTimer = 0.1f;
                }
            }
            else
            {
                _flareTimer -= deltaTime;
            }
        }


        _direction = Vector3.Lerp(_direction,targetDirection,2f * deltaTime);
        
        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
        
		BasicUpdate(deltaTime);
	}
}
