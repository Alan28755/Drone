using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown droneChoice;
    [SerializeField] private TMP_Dropdown sceneChoice;
    [SerializeField] private Image droneImage;
    [SerializeField] private Image sceneImage;

    [SerializeField] private Sprite[] drones;
    [SerializeField] private Sprite[] scenes;
    public void ExitButton()
    {
        Application.Quit();
    }

    public void StartButton()
    {
        GameObject.FindWithTag("UserConfig").GetComponent<UserConfig>().DroneID = droneChoice.value;
        switch (sceneChoice.value)
        {
            case 0:
                SceneManager.LoadScene("scene01");
                break;
            case 1:
                SceneManager.LoadScene("Test");
                break;
            case 2:
                SceneManager.LoadScene("InfraredDemoScene");
                break;

        }
    }


    public void SettingsButton()
    {
        SceneManager.LoadScene("SettingsScene");
    }
    // Start is called before the first frame update
    void Start()
    {
        droneImage.sprite = drones[droneChoice.value];
        sceneImage.sprite = scenes[sceneChoice.value];
        droneChoice.onValueChanged.AddListener(droneChoiceOnValueChanged);
        sceneChoice.onValueChanged.AddListener(sceneChoiceOnValueChanged);



    }


    // Update is called once per frame
    void Update()
    {


    }

    private void droneChoiceOnValueChanged(int value)
    {
        droneImage.sprite = drones[value];
    }

    private void sceneChoiceOnValueChanged(int value)
    {
        sceneImage.sprite = scenes[value];
    }

}
