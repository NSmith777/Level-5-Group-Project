// Unity namespaces
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

/*
 * Singleton class in which all scripts derive Xbox 360/One (XInput) gamepad input from.
 * (Note: This class uses the new Input System, don't add code that uses the legacy input API here. ~ Nathan)
 */
public class XInput
{
    public static bool m_IsConnected = false;

    static XInput()
    {
        // InputSystem.onDeviceChange += onInputDeviceChange;

        if (Gamepad.current != null && Gamepad.current is XInputController)
        {
            Debug.Log("XInput connected...");

            m_IsConnected = true;
        }
    }

    /*
     * Returns the active gamepad currently bound to Input System.
     */
    public static Gamepad GetController()
    {
        return Gamepad.current;
    }
}