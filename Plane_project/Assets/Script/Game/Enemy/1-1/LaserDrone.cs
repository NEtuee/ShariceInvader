using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDrone : PlaneBase
{
    Vector2 _targetMapPos;

    float _updateTimer = 0.1f;
    float _chargeTimer = 0f;
    float _blinkTimer = 0f;
    float _attackTimer = 0f;
    float _alertTimer = 0f;
    float _attackCooldown = 5f;

    bool _move = false;
    bool _act = false;
    bool _charging = false;

    int _alertCount = 0;


    public override void firstSetting()
    {
        base.firstSetting();

        SetSpriteSet("BoomDrone",AnimationType.None);
		SetCollider(new Define.SimpleCircleCollider(.22f,.22f,_position));
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
        Vector3 pos = ObjectManager.GetInstance()._place.MapPosToWorldPos(_targetMapPos);
        float dist = Vector3.Distance(pos,_position);

        if(_move && _updateTimer <= 0f)
        {
            _direction = (pos - _position).normalized;

            if(dist <= 0.1f)
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
                    _attackTimer = 1.5f;
                    _alertCount = 0;

                    _alertTimer = 0.2f;
                    _charging = false;
                    _sprRenderer.color = Color.white;

                    Vector3 dir = (GameManager.instance.player.position - _position).normalized;
                    for(int i = 0; i < 20; ++i)
                    {
                        EffectManager.GetInstance().AddEffect(_position + (dir * 0.05f) + (dir * (0.35f * (float)i)),"PhantomString_Aim/Appear",false)
                                .PassiveDeactive()
                                .DelayApear(0.05f * (float)i)
                                .SetTimer(1.5f - 0.05f * (float)i);
                    }
                }
            }

            if(_attackTimer != 0f)
            {
                _attackTimer -= deltaTime;

                if(_attackTimer <= 0f)
                {
                    Debug.Log("laser shot");
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

        if(Input.GetKeyDown(KeyCode.I))
        {
            SetMovePoint(GameManager.instance.player.position);
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
}
