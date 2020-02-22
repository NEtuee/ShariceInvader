using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleTest : MonoBehaviour
{
    ParticleSystem _particle;
    ParticleSystem.EmitParams param = new ParticleSystem.EmitParams();

    public void Start()
    {
        _particle = GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        if(Input.GetMouseButton(0))
        {
            param.position = CameraControll.instance.ScreenToWorldMouse();

            _particle.Emit(param,1);
        }
    }
}
