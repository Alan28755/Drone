using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Button returnToMenu;

    public TMP_Dropdown renderModeDropdown;


    // Start is called before the first frame update
    void Start()
    {
        renderModeDropdown.onValueChanged.AddListener(OnRenderModedChanged);
        returnToMenu.onClick.AddListener(ReturnToMenu);


        renderModeDropdown.value = GameObject.FindWithTag("UserConfig").GetComponent<UserConfig>().IsInfrared ? 1 : 0;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnRenderModedChanged(int id)
    {

        GameObject.FindWithTag("UserConfig").GetComponent<UserConfig>().ChangeIsInfrared(id==1);
        print(GameObject.FindWithTag("UserConfig").GetComponent<UserConfig>().IsInfrared);

    }
    private void ReturnToMenu()
    {
        SceneManager.LoadScene("StartScene");
    }
}
