using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDrone : PlaneBase
{
    Vector2 _targetMapPos;
    Vector3 _attackDir;

    float _updateTimer = 0.1f;
    float _chargeTimer = 0f;
    float _blinkTimer = 0f;
    float _attackTimer = 0f;
    float _attackCooldown = 5f;

    float _verticalAngle = 0f;
    float _spinAccel = 0f;

    float explosiveTimer = 0f;
    

    bool _move = false;
    bool _act = false;
    bool _charging = false;

    AnimationControllEx _back;
    AnimationControllEx _front;


    public override void firstSetting()
    {
        base.firstSetting();

        //SetSpriteSet("Planes/LaserDrone/",AnimationType.None);
        _aniType = AnimationType.None;
        _dirSprites = ResourceManager.GetInstance().GetSpriteSet("LaserDrone/Center",2);
		SetCollider(new Define.SimpleCircleCollider(.22f,.22f,_position));

        miniMapIcon.gameObject.GetComponent<SpriteRenderer>().sprite = ResourceManager.GetInstance().GetSprite("UI/map_eliteicon");

        AddDeco();
    }

    public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(1);
	}

    public override void initialize()
    {
        BasicInitialize();

        maxHp = _hp = 50;

        _direction = Vector3.up;
        _speed = 0f;
        _maxSpeed = 3.2f; 
        _gravityScale = 0f;
        _updateTimer = 0.1f;
        _frictionFactor = 0.1f;
        _attackCooldown = 3f;

        _rotateLock = true;
        _velocityFlip = false;

        _move = false;

        RegisteCollisionList();
    }

    public override void progress(float deltaTime)
    {
        Vector3 pos = Player.instance.position;//ObjectManager.GetInstance()._place.MapPosToWorldPos(_targetMapPos);
        float dist = Vector3.Distance(pos,_position);

        if(_move && _updateTimer <= 0f)
        {
            Vector3 targetPos = ObjectManager.GetInstance()._place.MapPosToWorldPos(_targetMapPos);
            float targetDist = Vector3.Distance(targetPos,_position);
            _direction = (targetPos - _position).normalized;

            if(targetDist <= 0.2f)
            {
                _speed = 0f;
                _move = false;
            }

            _updateTimer = 0.1f;
        }
        else
        {
            _updateTimer -= deltaTime;
        }

        if(_act)
        {
            if(_attackCooldown != 0f)
            {
                _attackCooldown -= deltaTime;
                if(_attackCooldown <= 0f)
                {
                    _attackCooldown = 0f;
                    SetMovePoint(Player.instance.position);

                    _charging = true;
                    _chargeTimer = 3f;
                    _blinkTimer = 1f;
                }

                if(dist <= 4f)
                {
                    _act = false;
                }
            }

            if(_charging)
            {
                _blinkTimer -= deltaTime;
                _chargeTimer -= deltaTime;

                if(_blinkTimer <= 0f)
                {
                    _sprRenderer.color = _sprRenderer.color == Color.white ? Color.red : Color.white;
                    _blinkTimer = _chargeTimer / 3f;
                }

                if(_chargeTimer <= 0f)
                {
                    _attackTimer = 2f;

                    _charging = false;
                    _sprRenderer.color = Color.white;

                    _attackDir = (Player.instance.position - _position).normalized;
                    _spinAccel = 0f;

                    // for(int i = 0; i < 20; ++i)
                    // {
                    //     EffectManager.GetInstance().AddEffect(_position + (_attackDir * 0.1f) + (_attackDir * (0.745f * (float)i)),"Planes/LaserDrone/Aim/Apear",false,null,0)
                    //             .PassiveDeactive()
                    //             .DelayApear(0.08f * (float)i)
                    //             .SetTimer(2f)
                    //             .SetAngle(MathEx.directionToAngle(_attackDir) - 20f);
                    // }
                }
            }

            if(_attackTimer != 0f)
            {
                _attackTimer -= deltaTime;

                _verticalAngle = Mathf.LerpAngle(_verticalAngle, 0f,0.05f);
                _eulerAngle = Mathf.LerpAngle(_eulerAngle, MathEx.directionToAngle(_attackDir),0.05f);

                if(_attackTimer <= 0f)
                {                       
                    // for(int i = 0; i < 20; ++i)
                    // {
                    //     Vector3 exp = _position + (_attackDir * 0.1f) + (_attackDir * (0.745f * (float)i));
                    //     EffectManager.GetInstance().AddEffect(exp,"Planes/LaserDrone/Aim/Disapear",false,null,0)
                    //                                 .DelayApear(0.08f * (float)i)
                    //                                 .SetAngle(MathEx.directionToAngle(_attackDir) - 20f);
                    // }

                    for(int i = 0; i < 120; ++i)
                    {
                        Vector3 exp = _position + (_attackDir * 0.1f) + ((_attackDir * 14.9f) * ((float)i / 120f));
                        exp += MathEx.RandomVector3(-0.2f,0.2f,-0.2f,0.2f);

                        ExplosionSprkle(3,0.01f * (float)i,exp);
                    }

#region oldStuff
                    // for(int i = 0; i < 20; ++i)
                    // {
                    //     Vector3 exp = _position + (_attackDir * 0.1f) + (_attackDir * (0.745f * (float)i));
                    //     EffectManager.GetInstance().AddEffect(exp,"Explosion")
                    //             .DelayApear(0.04f * (float)i)
                    //             .SetApearEvent(()=>{
                    //                 //EffectManager.GetInstance().Explosion(exp,8);

                    //                 EffectManager.GetInstance().AddEffect(exp,"Planes/LaserDrone/Explosion",false,null,0);
                    //                 EffectManager.GetInstance().AddEffect(exp,"Planes/LaserDrone/Aim/Disapear",false,null,0)
                    //                                         .SetAngle(MathEx.directionToAngle(_attackDir) - 20f);

                    //                 var list = CollisionManager.GetInstance().GetCollisionList(Define.ObjectType.player);

                    //                 if(list != null)
                    //                 {
                    //                     int count = list.Count;

                    //                     for(int j = 0; j < count; ++j)
                    //                     {
                    //                         var col = new Define.SimpleCircleCollider(0.3f,0.3f,exp);
                    //                         list[j].UpdateCollider();
                    //                         if(list[j].coll.CollisionCheck(col))
                    //                         {
                    //                             EffectManager.GetInstance().AddEffect(list[j].position,"AttackHit_0").SetAngle(Random.Range(0f,360f));
		    		//                             EffectManager.GetInstance().AddEffect(list[j].position,"AttackHit_1").SetAngle(Random.Range(0f,360f));

                    //                             ((PlaneBase)list[j]).Hit(5,this);
                    //                         }
                    //                     }
                    //                 }
                    //             })
                    //             .SetAngle(Random.Range(0f,360f));
                    // }
#endregion
                    _attackTimer = 0f;
                    _attackCooldown = Random.Range(4f,5f);

                    _act = false;
                }
            }
        }
        else
        {
            if(dist <= 4f)
            {
                _act = true;
            }
        }

        if(_attackTimer == 0f)
        {
            _spinAccel = _spinAccel < 180f ? _spinAccel + 100 * deltaTime : 180f;
            _verticalAngle += _spinAccel * deltaTime;
            _eulerAngle -= _spinAccel / 2f * deltaTime;

            _eulerAngle = MathEx.clamp360Degree(_eulerAngle);
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            //SetMovePoint(Player.instance.position);
            _attackCooldown = 0f;

            _charging = true;
            _chargeTimer = 3f;
            _blinkTimer = 1f;
        }

        SpinProgress();

        if(_hp < 15)
		{
			explosiveTimer -= deltaTime;

			if(explosiveTimer <= 0f)
			{
				explosiveTimer = Random.Range(0.3f,0.8f);

				Vector3 randPos = MathEx.RandomVector3(-coll.bound.box.x,coll.bound.box.x);

				EffectManager.GetInstance().Explosion(_position + randPos,5,0.2f,0.2f,0.3f);
				EffectManager.GetInstance().AddEffect(_position + randPos,"Explosion")
											.SetTarget(this)
											.SetAddPoint(randPos)
											.SetSortingOrder(2).SetAngle(Random.Range(0f,360f));
                
				EffectManager.GetInstance().EmitParticles("ExplosionSmoke",_position + randPos,4);
				//EffectManager.GetInstance().ExplosionSmoke(_position + randPos,_position + randPos + new Vector3(Random.Range(-0.2f,0.2f),Random.Range(-0.2f,0.2f)),0.15f,0.01f,4);
			}
		}

        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
		BasicUpdate(deltaTime);
    }

    public void SetMovePoint(Vector2 worldPos)
    {
        _targetMapPos = ObjectManager.GetInstance()._place.WorldPosToMapPos(worldPos);
        _speed = 0.2f;

        _move = true;
    }

    public void ExplosionSprkle(int count,float delay,Vector3 pos)
    {
        int rand = MathEx.RandomInt(0,2);
        EffectManager.GetInstance().AddEffect(pos,"Planes/LaserDrone/Sparkle/" + rand,false,null,0)
                                            .SetDisableEvent(()=> {

                                                DelayActManager.GetInstance().RequestAction(()=>{
                                                    EffectManager.GetInstance().Explosion(pos,count,0.07f,0.15f,0.24f);
                                                },1.1f);
                                                // float range = UnityEngine.Random.Range(0.6f,1.5f);
                                                // if(MathEx.RandomInt(0,5) == 0)
                                                //     EffectManager.GetInstance().ExplosionSmoke(pos,pos + MathEx.RandomVector3(-1f,1f,-1f,1f).normalized * range,0.13f,0.04f,22);
                                                //EffectManager.GetInstance().AddEffect(pos,"Explosion").SetSortingOrder(2).SetAngle(UnityEngine.Random.Range(0f,360f));
                                            })
                                            .DelayApear(delay)
											.SetSortingOrder(2);
                                            //.SetAngle(Random.Range(0f,360f));
    }

    public void SpinProgress()
    {
        _verticalAngle = MathEx.clamp360Degree(_verticalAngle);
        int point = (int)(_verticalAngle / _spriteAngle);

        _back.SetAnimationSprite(point);
        _front.SetAnimationSprite(point);

        SetSprite(_dirSprites[point]);

        Vector2 dir = MathEx.angleToDirection(_verticalAngle * Mathf.Deg2Rad);

        _back._sprRenderer.transform.localPosition = new Vector2(-0.21f,0f) * dir;
        _front._sprRenderer.transform.localPosition = new Vector2(0.2f,0f) * dir;

        if(_verticalAngle > 180f)
        {
            _back._sprRenderer.sortingOrder = 1;
            _front._sprRenderer.sortingOrder = -1;
        }
        else
        {
            _back._sprRenderer.sortingOrder = -1;
            _front._sprRenderer.sortingOrder = 1;
        }
    }

    public void AddDeco()
    {
        _deco.aniProgress = false;

        _back = _deco.AddDeco(new Vector2(-0.21f,0f));
        _front = _deco.AddDeco(new Vector2(0.2f,0f));

        _back.AddAnimation("progress","Planes/LaserDrone/Back");
        _front.AddAnimation("progress","Planes/LaserDrone/Front");

        _back.ChangeAni("progress",false);
        _front.ChangeAni("progress",false);

        _back._sprRenderer.sortingOrder = -1;
        _front._sprRenderer.sortingOrder = 1;

        _spriteAngle = 360f / 64f;
    }
}
