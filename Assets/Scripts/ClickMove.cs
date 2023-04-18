using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMove : MonoBehaviour
{
    private Vector3 _mOffset;//鼠标位置距离物体中心的偏移
    private float _mZCoord;//物体在摄像坐标系下的深度

    //每次鼠标在物体按下时获取物体深度和偏移
    void OnMouseDown()
    {
        _mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        // Store offset = gameobject world pos - mouse world pos
        _mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }
    //获取当前鼠标在世界空间下的位置(摄像坐标系下的鼠标深度与物体相同)
    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = _mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    //鼠标在物体上拖得时更新物体世界坐标
    void OnMouseDrag()
    {
        transform.position = GetMouseAsWorldPoint() + _mOffset;
    }

}
