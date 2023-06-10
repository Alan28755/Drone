using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserConfig : MonoBehaviour
{
    public static UserConfig instance = null;
    private bool _isExist = false;
    private bool isInfrared = false;
    private int _droneID;


    public int DroneID
    {
        get
        {
            return _droneID;
        }
        set
        {
            _droneID = value;
        }
    }

    public bool IsInfrared
    {
        get
        {
            return isInfrared;
        }
        set
        {
            isInfrared = value;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Load();
    }

    public void Save()
    {
        int t = isInfrared ? 1 : 0;
        PlayerPrefs.SetInt("isInfrared",t);
    }

    public void ChangeIsInfrared(bool b)
    {
        isInfrared = b;
        Save();
    }
    public void Load()
    {
        int t = PlayerPrefs.GetInt("isInfrared",0);
        isInfrared = t == 1 ? true : false;

    }
    // Update is called once per frame
    void Update()
    {

    }
}
