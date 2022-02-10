// Unity namespaces
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [Header("Root Menu")]
    public GameObject m_RootMenuPanel;
    public GameObject m_RootMenu_First;
    [Header("Level Select")]
    public GameObject m_LevelSelectPanel;
    public GameObject m_LvlSel_First;
    [Header("Controls")]
    public GameObject m_ControlsPanel;
    public GameObject m_Controls_First;
    
    bool m_IsLvlSelOpen;
    bool m_IsControlsOpen;

    private void Update()
    {
        InputSystemUIInputModule UI_module = (InputSystemUIInputModule)EventSystem.current.currentInputModule;
        InputAction cancel = UI_module.cancel.action;

        if (cancel.triggered)
        {
            if (m_IsLvlSelOpen)
                CloseLevelSelect();
            else if (m_IsControlsOpen)
                CloseControls();
        }
    }

    public void GoToNewGame()
    {
        SceneManager.LoadScene("MainStage");
    }

    public void OpenLevelSelect()
    {
        m_RootMenuPanel.SetActive(false);
        m_LevelSelectPanel.SetActive(true);
        m_IsLvlSelOpen = true;

        EventSystem.current.SetSelectedGameObject(m_LvlSel_First);
    }

    public void CloseLevelSelect()
    {
        m_LevelSelectPanel.SetActive(false);
        m_RootMenuPanel.SetActive(true);
        m_IsLvlSelOpen = false;

        EventSystem.current.SetSelectedGameObject(m_RootMenu_First);
    }

    public void OpenControls()
    {
        m_RootMenuPanel.SetActive(false);
        m_ControlsPanel.SetActive(true);
        m_IsControlsOpen = true;

        EventSystem.current.SetSelectedGameObject(m_Controls_First);
    }

    public void CloseControls()
    {
        m_ControlsPanel.SetActive(false);
        m_RootMenuPanel.SetActive(true);
        m_IsControlsOpen = false;

        EventSystem.current.SetSelectedGameObject(m_RootMenu_First);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
