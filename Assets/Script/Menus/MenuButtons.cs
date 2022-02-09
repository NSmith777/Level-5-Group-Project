using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject _menuPanel; //The UI panel containing the menu options and all its components.
    public GameObject _LevelSelectPanel; //The UI panel containing the Level selections and all its components.
    public GameObject _optionsPanel; //The UI panel containing the settings and all its components.
    
    bool _settingsBool;
    bool _levelBool;


    private void Update()//updates every frame
    {
        if (Input.GetKeyDown("escape")&& _levelBool==true)//If Esc key is pressed and level bool true, close level panel.
        {
            Invoke("CloseLevelSelect", 0f);
        }

        else if (Input.GetKeyDown("escape") && _settingsBool == true)//If Esc key is pressed and settings bool true, close settings panel
        {
            Invoke("CloseSettings", 0f);
        }
    }

    public void GoToNewGame()
    {
        SceneManager.LoadScene("MainStage");
    }

    public void OpenLevelSelect()
    {
        _menuPanel.SetActive(false);
        _LevelSelectPanel.SetActive(true);
        _levelBool = true;
    }

    public void CloseLevelSelect()
    {
        _LevelSelectPanel.SetActive(false);
        _menuPanel.SetActive(true);
        _levelBool = false;
    }

    public void OpenSettings()
    {
        _menuPanel.SetActive(false);
        _optionsPanel.SetActive(true);
        _settingsBool = true;
    }

    public void CloseSettings()
    {
        _optionsPanel.SetActive(false);
        _menuPanel.SetActive(true);
        _settingsBool = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
