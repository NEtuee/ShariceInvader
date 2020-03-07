using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Editor_TrailRenderer : MonoBehaviour
{
    public Vector3 direction;

    public TrailRenderer trail;

    public Editor_PlaneInfoBase info;

    private Vector3 _velocity = Vector3.zero;
    private float dist;

    private Vector3 _prevPos;

    public Vector3 targetPos;


    public void Init(Editor_PlaneInfoBase i)
    {
        info = i;

        trail = gameObject.AddComponent<TrailRenderer>();
        TrailDataUpdate();
        direction = Vector2.left;
    }

    public void UpdateDirection(Vector2 pos)
    {
        direction = pos;
    }

    void Update()
    {
        _velocity += direction * info.speed;
        if(_velocity.magnitude >= info.maxSpeed)
        {
            float val = info.maxSpeed / _velocity.magnitude;
			_velocity = _velocity * val;
        }
        
        for(int i = 0; i < trail.positionCount; ++i)
        {
            trail.SetPosition(i,(_velocity * Time.deltaTime) + trail.GetPosition(i));
        }

        if(_prevPos == transform.position)
            trail.AddPosition(gameObject.transform.position);
        else
            _prevPos = transform.position;

        transform.position = Vector3.Lerp(transform.position,targetPos,0.2f);
    }

    public void TrailDataUpdate()
    {
		trail.material = ResourceManager.GetInstance().GetMaterial(info.trailInfo.trailMaterial);
		trail.time = info.trailInfo.time;
		trail.startWidth = info.trailInfo.startWidth;
		trail.endWidth = info.trailInfo.endWidth;
    }
}
