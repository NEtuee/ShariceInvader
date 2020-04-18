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

        _direction = Vector3.up;
        _speed = 0f;
        _maxSpeed = 3.2f; 
        _gravityScale = 0f;
        _updateTimer = 0.1f;
        _frictionFactor = 0.1f;
        _attackCooldown = 5f;

        _rotateLock = true;
        _velocityFlip = false;

        _move = false;

        RegisteCollisionList();
    }

    public override void progress(float deltaTime)
    {
        Vector3 pos = GameManager.instance.player.position;//ObjectManager.GetInstance()._place.MapPosToWorldPos(_targetMapPos);
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

                    _charging = true;
                    _chargeTimer = 3f;
                    _blinkTimer = 1f;
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

                    _attackDir = (GameManager.instance.player.position - _position).normalized;
                    _spinAccel = 0f;

                    for(int i = 0; i < 20; ++i)
                    {
                        EffectManager.GetInstance().AddEffect(_position + (_attackDir * 0.1f) + (_attackDir * (0.745f * (float)i)),"Planes/LaserDrone/Aim",false,null,0)
                                .PassiveDeactive()
                                .DelayApear(0.1f * (float)i)
                                .SetTimer(2f - 0.1f * (float)i)
                                .SetAngle(MathEx.directionToAngle(_attackDir) - 20f);
                    }
                }
            }

            if(_attackTimer != 0f)
            {
                _attackTimer -= deltaTime;

                _verticalAngle = Mathf.LerpAngle(_verticalAngle, 0f,0.05f);
                _eulerAngle = Mathf.LerpAngle(_eulerAngle, MathEx.directionToAngle(_attackDir),0.05f);

                if(_attackTimer <= 0f)
                {
                    Debug.Log("laser shot");
                    EffectManager.GetInstance().AddEffect(_position + (_attackDir * 0.01f),"Planes/LaserDrone/Laser",false,null,0)
                                                .SetAngle(MathEx.directionToAngle(_attackDir));

                    EffectManager.GetInstance().AddLineEffect(_position,_position + _attackDir * 20f,0.13f,1f)
                                                .SetColor(Color.yellow)
                                                .SetLerpWidth(0.001f,0.15f);

                    for(int i = 0; i < 20; ++i)
                    {
                        Vector3 exp = _position + (_attackDir * 0.1f) + (_attackDir * (0.745f * (float)i));
                        EffectManager.GetInstance().AddEffect(exp,"Explosion")
                                .DelayApear(0.04f * (float)i)
                                .SetApearEvent(()=>{EffectManager.GetInstance().Explosion(exp,8);})
                                .SetAngle(Random.Range(0f,360f));
                    }

                    _attackTimer = 0f;
                    _attackCooldown = Random.Range(5f,8f);
                }
            }
        }
        else
        {
            if(dist <= 2f)
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
            SetMovePoint(GameManager.instance.player.position);

        }

        SpinProgress();

        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
		BasicUpdate(deltaTime);
    }

    public void SetMovePoint(Vector2 worldPos)
    {
        _targetMapPos = ObjectManager.GetInstance()._place.WorldPosToMapPos(worldPos);
        _speed = 0.2f;

        _move = true;
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
