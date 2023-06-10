using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LocalConfig : MonoBehaviour
{
    [SerializeField] private GameObject[] drones;
    // Start is called before the first frame update
    void Start()
    {
        int i = GameObject.FindWithTag("UserConfig").GetComponent<UserConfig>().DroneID;
        Transform waypoints = GameObject.FindWithTag("WayPoints").transform;
        Vector3 pos = waypoints.GetChild(0).transform.position;
        GameObject drone=GameObject.Instantiate(drones[i],pos,Quaternion.identity);
        drone.GetComponent<FlyingDroneScript>().nextWaypoint = waypoints.GetChild(1).gameObject;


        Volume volume = GameObject.Find("InfraredVolume").GetComponent<Volume>();
        if (!volume.profile.TryGet<Infrared>(out var infrared))
        {
            infrared = volume.profile.Add<Infrared>(false);

        }

        GameObject userConfig = GameObject.FindWithTag("UserConfig");

        if (userConfig.GetComponent<UserConfig>().IsInfrared)
        {
            infrared.active = true;
            print("hi");
        }
        else
        {
            infrared.active = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
