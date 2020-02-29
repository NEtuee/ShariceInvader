using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Editor_CameraControllBase : MonoBehaviour
{
    public Camera mainCamera;

    private Vector2 _cursorStart;
    private Vector2 _mainCenter;

    public void SetSize(float size) {mainCamera.orthographicSize = size;}
    public void AddSize(float size) 
    {
        mainCamera.orthographicSize = mainCamera.orthographicSize + size < 0.3f ? 0.3f : mainCamera.orthographicSize + size;
    }
    public void MovePosStart(Vector2 startPos)
    {
        _cursorStart = startPos;
        _mainCenter = mainCamera.transform.position;
    }

    public void MovePosCenterBase(Vector2 movePos)
    {
        Vector2 val = (movePos - _cursorStart) * mainCamera.orthographicSize * 0.0019f;
        MovePos(_mainCenter - val);
    }

    public void MovePos(Vector3 pos)
    {
        pos.z = -10f;
        mainCamera.transform.position = pos;
    }
}
