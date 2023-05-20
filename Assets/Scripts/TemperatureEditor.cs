using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TemperatureEditor : MonoBehaviour
{
    public float minTemperature = 0;

    public float maxTemperature = 375;

    public float[] temperatures;

    void Start()
    {
        temperatures = new float[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Renderer>().material.SetFloat("minTemperatureK",minTemperature);
            transform.GetChild(i).GetComponent<Renderer>().material.SetFloat("maxTemperatureK",maxTemperature);
            transform.GetChild(i).GetComponent<Renderer>().material.SetFloat("editTemperature",temperatures[i]);
        }
        print(transform.childCount);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
