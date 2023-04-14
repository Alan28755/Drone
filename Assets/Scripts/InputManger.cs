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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _menuActive = !_menuActive;
            menu.SetActive(_menuActive);
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
