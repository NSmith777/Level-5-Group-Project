// Unity namespaces
using UnityEngine;

public class GlobalSettingsMgr
{
    public static float m_AnalogSensitive;
    public static float m_GyroSensitive;

    // Load existing settings data from disk on startup
    static GlobalSettingsMgr()
    {
        m_AnalogSensitive = PlayerPrefs.GetFloat("AnalogSensitivity", 120f);
        m_GyroSensitive = PlayerPrefs.GetFloat("GyroSensitivity", 4000f);
    }
}

public class SettingsManager : MonoBehaviour
{
    public void AnalogValChanged(float val)
    {
        GlobalSettingsMgr.m_AnalogSensitive = val * 10;
        PlayerPrefs.SetFloat("AnalogSensitivity", GlobalSettingsMgr.m_AnalogSensitive);
    }

    public void GyroValChanged(float val)
    {
        GlobalSettingsMgr.m_GyroSensitive = val * 1000;
        PlayerPrefs.SetFloat("GyroSensitivity", GlobalSettingsMgr.m_GyroSensitive);
    }
}
