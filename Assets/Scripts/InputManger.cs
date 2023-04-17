using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using Evereal.VideoCapture;

public class InputManger : MonoBehaviour
{
    //按下ESC键呼出的菜单
    public GameObject menu;
    private bool _menuActive = false;//菜单是否被呼出


    public GameObject editWeather;
    public GameObject editWayPoints;
    public GameObject editCameras;
    private GameObject _editMenu;


    private GameObject _mainCamera;
    public GameObject captureVideo;


    //获取录制相机
    [SerializeField] private Camera captureCamera;



    //获取videoCapture
    [SerializeField] private VideoCapture videoCapture;
    [SerializeField] private TMPro.TMP_Text capture;

    //界面上方选项菜单的枚举
    enum MenuButtons
    {
        None,
        Weather,
        WayPoints,
        Cameras
    }

    private MenuButtons _menuButtons;


    private void Awake()
    {
        Application.runInBackground = true;
    }

    void Start()
    {
        menu.SetActive(false);
        editWeather.SetActive(false);
        editCameras.SetActive(false);
        editWayPoints.SetActive(false);


        _editMenu = editWeather;
        _editMenu.SetActive(true);

        _mainCamera = GameObject.FindWithTag("MainCamera");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _menuActive = !_menuActive;
            menu.SetActive(_menuActive);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            captureCamera.transform.position = _mainCamera.transform.position;
            captureCamera.transform.rotation = _mainCamera.transform.rotation;
            // captureVideo.transform.position = _mainCamera.transform.position;
            // captureVideo.transform.rotation = _mainCamera.transform.rotation;
        }

        if (videoCapture.status == CaptureStatus.PENDING)
        {
            capture.SetText("混合中");
        }
        else if (videoCapture.status == CaptureStatus.STOPPED)
        {
            capture.SetText("编码中");
        }
        else if (videoCapture.status == CaptureStatus.READY)
        {
            capture.SetText("录制");
        }
        else if (videoCapture.status == CaptureStatus.STARTED)
        {
            capture.SetText("停止");
        }
    }
    public void ClickWeatherButton()
    {
        _menuButtons = MenuButtons.Weather;
        _editMenu.SetActive(false);
        _editMenu = editWeather;
        _editMenu.SetActive(true);
    }

    public void ClickWayPointsButton()
    {
        _menuButtons = MenuButtons.WayPoints;
        _editMenu.SetActive(false);
        _editMenu = editWayPoints;
        _editMenu.SetActive(true);
    }

    public void ClickCamerasButton()
    {
        _menuButtons = MenuButtons.Cameras;
        _editMenu.SetActive(false);
        _editMenu = editCameras;
        _editMenu.SetActive(true);
    }

    //录制按钮
    public void CaptureButtonClick()
    {
        if (videoCapture.status == CaptureStatus.READY)
        {
            //开始录制
            videoCapture.StartCapture();
            //无人机开始飞行
            GameObject drone = GameObject.FindWithTag("Drone");
            drone.GetComponent<FlyingDroneScript>().enabled=true;
        }
        else if(videoCapture.status==CaptureStatus.STARTED)
        {
            //停止录制
            videoCapture.StopCapture();
            //无人机停止飞行
            GameObject drone = GameObject.FindWithTag("Drone");
            drone.GetComponent<FlyingDroneScript>().enabled=false;
        }

    }

    public void BrowserButtonClick()
    {
        Utils.BrowseFolder(videoCapture.saveFolder);
    }
}
