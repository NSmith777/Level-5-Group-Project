// Unity namespaces
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GlobalPauseMgr
{
    public static bool m_IsPaused;
}

public class PauseManager : MonoBehaviour
{
    public GameObject m_PausePanel;
    public GameObject m_Pause_First;

    public GameObject m_SettingsPanel;
    public GameObject m_Settings_First;
    public GameObject m_HUDPanel;

    bool m_IsInRootMenu = true;

    void Update()
    {
        if(DS4.m_IsConnected)
        {
            if(DS4.GetController().optionsButton.wasPressedThisFrame)
            {
                if (!GlobalPauseMgr.m_IsPaused)
                    PauseGame();
                else if(GlobalPauseMgr.m_IsPaused && m_IsInRootMenu)
                    ResumeGame();
            }
        }
        else if(XInput.m_IsConnected)
        {
            if(XInput.GetController().startButton.wasPressedThisFrame)
            {
                if (!GlobalPauseMgr.m_IsPaused)
                    PauseGame();
                else if (GlobalPauseMgr.m_IsPaused && m_IsInRootMenu)
                    ResumeGame();
            }
        }
    }

    public void PauseGame()
    {
        GlobalPauseMgr.m_IsPaused = true;
        m_PausePanel.SetActive(true);
        m_HUDPanel.SetActive(false);

        EventSystem.current.SetSelectedGameObject(m_Pause_First);

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        GlobalPauseMgr.m_IsPaused = false;
        m_PausePanel.SetActive(false);
        m_HUDPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);

        Time.timeScale = 1;
    }

    public void OpenSettings()
    {
        m_IsInRootMenu = false;
        m_SettingsPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(m_Settings_First);
    }

    public void CloseSettings()
    {
        m_IsInRootMenu = true;
        m_SettingsPanel.SetActive(false);

        EventSystem.current.SetSelectedGameObject(m_Pause_First);

        PlayerPrefs.Save();
    }

    public void ExitGame()
    {
        ResumeGame();
        SceneManager.LoadScene("MainMenu");
    }
}
