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

        EventSystem.current.SetSelectedGameObject(m_Pause_First);

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        GlobalPauseMgr.m_IsPaused = false;
        m_PausePanel.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);

        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        ResumeGame();
        SceneManager.LoadScene("MainMenu");
    }
}
