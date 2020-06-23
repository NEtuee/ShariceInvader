using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundAlert : MonoBehaviour
{
    
    void Update()
    {
        transform.position = new Vector3(CameraControll.instance.mainCam.transform.position.x,0f);
    }
}
