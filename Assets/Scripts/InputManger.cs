using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class InputManger : MonoBehaviour
{
    //按下ESC键呼出的菜单
    public GameObject menu;
    private bool _menuActive = false;//菜单是否被呼出


    public GameObject editWeather;
    public GameObject editWayPoints;
    public GameObject editCameras;
    private GameObject _editMenu;


    public GameObject captureCamera;
    private GameObject _mainCamera;
    public GameObject captureVideo;

    //界面上方选项菜单的枚举
    enum MenuButtons
    {
        None,
        Weather,
        WayPoints,
        Cameras
    }

    private MenuButtons _menuButtons;


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
            captureVideo.transform.position = _mainCamera.transform.position;
            captureVideo.transform.rotation = _mainCamera.transform.rotation;
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
}
