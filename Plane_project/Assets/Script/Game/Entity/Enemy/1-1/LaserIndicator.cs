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
    private float _timer = 4f;

    public override void firstSetting()
    {
        base.firstSetting();

        _ani = new AnimationControllEx(_sprRenderer);
        _player = Player.instance;
        _cam = CameraControll.instance;

        _speed = 0.1f;

        _ani.AddAnimation("Launch_0","LaserIndicator/0/Launch",1);
        _ani.AddAnimation("Launch_1","LaserIndicator/1/Launch",1);
        _ani.AddAnimation("FireStand_0","LaserIndicator/0/FireStand",1);
        _ani.AddAnimation("FireStand_1","LaserIndicator/1/FireStand",1);

        SetCollider(new Define.SimpleBoxCollider(.4f,10f,_position));
    }

    public override void initialize()
    {
        _ani.ChangeAni("Launch_0",false);
    }

    public override void progress(float deltaTime)
    {
        _direction = _cam.position.x > _position.x ? Vector3.right : Vector3.left;
        
        _ani.AnimationProgress(deltaTime);
        Move(deltaTime);

        _position.y = _cam.position.y;
        _player.coll.UpdateBound(_player.position);
        _collider.UpdateBound(_position);
        CollCheck(_player.coll.CollisionCheck(_collider));

        if(_timer != 0)
        {
            _timer -= deltaTime;

            if(_timer <= 0f)
            {

                if(_act)
                {
                    EffectManager.GetInstance().AddLineEffect(_position + new Vector3(0f,-10f,0f),_position + new Vector3(0f,10f,0f),.4f,.8f)
                                        .SetLerpWidth(0.001f,.1f);

                    if(_isColl)
                    {
                        _player.Hit(5,null);
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

                    _timer = 1f;
                    _act = true;
                }
            }

        }
    }

    public override void deleteEvent()
    {
        //Debug.Log("delete");
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
