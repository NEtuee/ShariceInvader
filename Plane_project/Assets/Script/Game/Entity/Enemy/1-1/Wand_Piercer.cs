using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand_Piercer : WandsBase
{
    private bool _act = false;
    private bool _canShot = false;
    private bool _shotReady = false;
    private bool _shot = false;
    private float _actTimer = 0f;

    private Vector3 _targetDirection;

    private LineEffectBase _line;

    public override void firstSetting()
    {
        base.firstSetting();

        //SetSpriteSet("StarFish/Marker",AnimationType.None);
		SetCollider(new Define.SimpleCircleCollider(.22f,.22f,_position));
        LoadPlaneData("StarFish/Piercer");

        _aniType = AnimationType.None;

        _dirSprites = ResourceManager.GetInstance().GetSpriteSet("StarFish/Piercer",2);
        _spriteAngle = 360f / _dirSprites.Length;

        UpdateSprite();

        var deco = _deco.AddDeco(new Vector2(0.1f,0f));
        deco._sprRenderer.sprite = ResourceManager.GetInstance().GetSprite("SpriteSet/Planes/StarFish/starfish_piercer");

        // _dirSprites = ResourceManager.GetInstance().GetSpriteSet("StarFish/Defender",2);
        // _spriteAngle = 360f / _dirSprites.Length;
    }

    public override void deleteEvent()
    {
        base.deleteEvent();
        if(_line != null)
        {
            _line.SetActive(false);
        }
    }

    public override void initialize()
    {
        base.initialize();

        _canBurst = false;

        _mass = 5f;
        _gravityScale =.0f;
        _maxSpeed = 8f;
        _speed = 0f;

        maxHp = _hp = 15;

        _actTimer = Random.Range(0.8f,1.2f);
    }

    public override void progress(float deltaTime)
    {
        base.progress(deltaTime);
        mainAngle += 120f * deltaTime;
        mainAngle = MathEx.clamp360Degree(mainAngle);
        _sprRenderer.sprite = _dirSprites[(int)(mainAngle / _spriteAngle)];

        if(!_act)
        {
            if(act)
            {
                _actTimer -= deltaTime;
                if(_actTimer <= 0f)
                {
                    _actTimer = 0f;
                    _gravityScale = 0f;
                    _frictionFactor = 0.1f;
                    _speed = 0.2f;
                    _act = true;

                    AddForce(_direction * 5);

                    RandomSpread();
                }
            }

            return;
        }


        _actTimer -= deltaTime;
        if(_actTimer <= 0f && _canShot && !_shotReady)
        {
            _speed = 0f;
            _line = EffectManager.GetInstance().AddLineEffect(_position,(Player.instance.position - _position).normalized * 100f,0f,5f)
                                        .SetLerpWidth(0.2f,.2f)
                                        .SetTiling(ResourceManager.GetInstance().GetSprite("SpriteSet/Planes/StarFish/starfish_piercerline"),1f)
                                        .SetOffsetScrolling(.5f);
            _actTimer = 4f;
            _shotReady = true;
        }

        if(_shotReady && !_shot)
        {
            var pos = Player.instance.position;
            _targetDirection = pos - _position;
            _line.SetPosition(_position,0);
            _line.SetPosition(_targetDirection * 100f,1);

            if(_actTimer <= 0f)
            {
                _line.SetActive(false);
                _line = null;
                _shot = true;
                _actTimer = 3f;
                _speed = 0.2f;
                _direction = _targetDirection;

                _bodyAttack = 4;

                SetImmortal(true);

                BurstActive();
                SetAdditionalSpeed(8f,3f);
            }
        }
        else if(_shot)
        {
            if(_actTimer <= 0f)
            {
                _shot = false;
                _shotReady = false;

                _bodyAttack = 1;
                SetImmortal(false);

                RandomSpread();
            }
        }

        _trailEmmit = _shot;

        TopDownEdgeCheck();

        float dirangle = Mathf.LerpAngle(_eulerAngle,MathEx.directionToAngle(_targetDirection.normalized),.2f);
        _eulerAngle = dirangle;

        _direction = Vector3.Lerp(_direction,_targetDirection,13f * deltaTime);
        
        var dist = Vector3.Distance(_position,Player.instance.position);
        if(dist < 5f)
        {
            _canShot = false;
        }
    }

    public void TopDownEdgeCheck()
    {
        if(position.y > ObjectManager.GetInstance()._place._mapHeight - 3f || position.y < 3f)
        {
            _canShot = false;
        }
        else
        {
            _canShot = true;
        }

        if(position.y < 3f && _targetDirection.y < 0)
        {
            _targetDirection.y *= -1f;
        }
        else if(position.y > ObjectManager.GetInstance()._place._mapHeight - 3f && _targetDirection.y > 0)
        {
            _targetDirection.y *= -1f;
        }
    }

    public void RandomSpread()
    {
        _targetDirection = new Vector3(Random.Range(0,2) == 0 ? -1f : 1f, Random.Range(-0.5f,0.5f)).normalized;

        _actTimer = Random.Range(2f,5f);

        //BurstActive();
        //SetAbsoluteForce(_direction * 10f);
    }
}
