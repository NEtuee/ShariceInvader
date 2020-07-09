using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserIndicator : PlaneBase
{
    private AnimationControllEx _ani;
    private Player _player;
    private CameraControll _cam;

    private bool _isColl = false;
    private bool _act = false;
    private float _timer = 3f;
    public float apearTimer = 0f;

    public override void firstSetting()
    {
        base.firstSetting();

        _ani = new AnimationControllEx(_sprRenderer);
        _player = Player.instance;
        _cam = CameraControll.instance;

        _ani.AddAnimation("Launch_0","SpriteSet/Effects/LaserIndicator/0/Launch");
        _ani.AddAnimation("Launch_1","SpriteSet/Effects/LaserIndicator/1/Launch");
        _ani.AddAnimation("FireStand_0","SpriteSet/Effects/LaserIndicator/0/FireStand");
        _ani.AddAnimation("FireStand_1","SpriteSet/Effects/LaserIndicator/1/FireStand");

        SetCollider(new Define.SimpleBoxCollider(.4f,10f,_position));
    }

    public override void initialize()
    {
        _speed = 0.1f;
        _maxSpeed = 5f;
        _ani.ChangeAni("Launch_0",false);

        noneTarget = true;

        miniMapIcon.gameObject.SetActive(true);
        SoundManager.instance.PlayRequest("SE/Marker/laserindicator");
    }

    public override void progress(float deltaTime)
    {
        if(apearTimer != 0f)
        {
            apearTimer -= deltaTime;
            _sprRenderer.enabled = false;

            if(apearTimer <= 0f)
            {
                apearTimer = 0f;
                _sprRenderer.enabled = true;
            }

            return;
        }
        _ani.AnimationProgress(deltaTime);
        Move(deltaTime);

        _position.y = _cam.position.y;
        _player.coll.UpdateBound(_player.position);
        _collider.UpdateBound(_position);
        CollCheck(_player.coll.CollisionCheck(_collider));

        _direction = _player.position.x > _position.x ? Vector3.right : Vector3.left;
        _speed += 0.05f * (Vector2.Distance(_player.position,position)) * deltaTime;
        _speed =  _speed >= _maxSpeed ? maxSpeed : _speed;




        if(_timer != 0)
        {
            _timer -= deltaTime;

            if(_timer <= 0f)
            {

                if(_act)
                {
                    EffectManager.GetInstance().AddLineEffect(_position + new Vector3(0f,-10f,0f),_position + new Vector3(0f,10f,0f),.4f,.8f)
                                        .SetLerpWidth(0.001f,.1f);
                    
                    SoundManager.instance.PlayRequest("SE/Marker/LaserShot");

                    if(_isColl)
                    {
                        _player.Hit(75,null);
                    }
                    Delete();
                }
                else
                {
                    if(_isColl)
                    {
                        _ani.ChangeAni("FireStand_1",false);
                    }
                    else
                    {
                        _ani.ChangeAni("FireStand_0",false);
                    }

                    _timer = .7f;
                    _act = true;
                }
            }

        }

        UpdateMiniMapIcon();
    }

    public override void deleteEvent()
    {
        //Debug.Log("delete");
        BasicDeleteEvents();
    }

    public void CollCheck(bool b)
    {
        if(_isColl != b)
        {
            if(b)
            {
                CollEnter();
            }
            else
            {
                CollExit();
            }

            _isColl = b;
        }
    }

    public void CollEnter()
    {
        if(_ani.currAni == "Launch_0")
        {
            _ani.ChangeAniSync("Launch_1",false);
        }
        else
        {
            _ani.ChangeAniSync("FireStand_1",false);
        }
    }

    public void CollExit()
    {
        if(_ani.currAni == "Launch_1")
        {
            _ani.ChangeAniSync("Launch_0",false);
        }
        else
        {
            _ani.ChangeAniSync("FireStand_0",false);
        }
    }
}
