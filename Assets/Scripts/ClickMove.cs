using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMove : MonoBehaviour
{
    private Transform _myTransform;
    private Vector3 _selfScenePosition;
 
    void Start()
    {
        _myTransform = transform;
        _selfScenePosition = Camera.main.WorldToScreenPoint(_myTransform.position); 
    }
 
    void OnMouseDrag()
    {
        //获取拖拽点鼠标坐标
        print(Input.mousePosition.x + "     y  " + Input.mousePosition.y + "     z  " + Input.mousePosition.z);
        //新的屏幕点坐标
        Vector3 currentScenePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _selfScenePosition.z);
        //将屏幕坐标转换为世界坐标
        Vector3 crrrentWorldPosition = Camera.main.ScreenToWorldPoint(currentScenePosition); 
        //设置对象位置为鼠标的世界位置
        _myTransform.position = crrrentWorldPosition;
        //更新对象屏幕坐标
        _selfScenePosition = Camera.main.WorldToScreenPoint(_myTransform.position); 

    }

}
