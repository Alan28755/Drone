using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Vector3 GetWayPoint(int i)
    {
        return transform.GetChild(i).position;
    }

    public int GetNextIndex(int i)
    {
        return (i + 1) % transform.childCount;
    }
}
