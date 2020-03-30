using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScreenCam : MonoBehaviour
{
    public Transform screenPlane;

    public void Awake()
    {
    //    UpdateScreenSize();
    }

    public void UpdateScreenSize()
    {
        screenPlane.localScale = new Vector3((float)Screen.width / 1000f, 1f, (float)Screen.height / 1000f);
    }
}
