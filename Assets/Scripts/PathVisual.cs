using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrawXXL;
using Unity.Mathematics;

public class PathVisual : MonoBehaviour
{
    void Start()
    {


        StartCoroutine(SetLineLayer());

    }

    // Update is called once per frame
    void Update()
    {
        int count = transform.childCount;
        for (int i = 1; i < count-1; i++)
        {
            DrawBasics.MovingArrowsLine(transform.GetChild(i).position,transform.GetChild(i+1).position);
            // DrawShapes.Sphere(transform.GetChild(i).position, 0.1f,Color.yellow,
            //     quaternion.identity,0.1f, i.ToString());
        }
        DrawBasics.MovingArrowsLine(transform.GetChild(count-1).position,transform.GetChild(1).position);
        // DrawShapes.Sphere(transform.GetChild(count-1).position, 0.1f,Color.yellow,
        //     quaternion.identity,0.1f, (count-1).ToString());



    }

    //设置xxl绘制对象对录屏摄像机不可见
    IEnumerator SetLineLayer()
    {
        yield return null;
        DrawXXL.DrawXXL_LinesManager.instance.gameObject.layer=LayerMask.NameToLayer("PathVisual");

    }
}
