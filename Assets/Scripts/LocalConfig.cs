using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LocalConfig : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
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
