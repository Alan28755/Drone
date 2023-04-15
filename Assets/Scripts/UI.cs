using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public void ExitButton()
    {
        Application.Quit();
    }

    public void StartButton()
    {
        SceneManager.LoadScene("Test");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("StartScene");
    }
    public void SettingsButton()
    {
        SceneManager.LoadScene("SettingsScene");
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
