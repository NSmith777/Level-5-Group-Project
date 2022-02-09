// Unity namespaces
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public class GlobalInput
{
    // For some weird reason, we have to access a class member to coax the constructor to run...
    public static bool m_Init = false;

    static GlobalInput() {
        Debug.Log("Init GlobalInput...");

        InputSystem.onDeviceChange += onInputDeviceChange;
    }

    /*
     * Manages DualShock connection/disconnection events.
     */
    static void onInputDeviceChange(InputDevice device, InputDeviceChange change)
    {
        Debug.Log("onInputDeviceChange");
        switch (change)
        {
            case InputDeviceChange.Added:
                Debug.Log("Device added: " + device);

                if (device is DualShockGamepad)
                {
                    // Re-bind extra DS4 controls
                    DS4.BindControls(DualShockGamepad.current);

                    device.MakeCurrent();

                    DS4.m_IsConnected = true;
                }
                else if (device is XInputController)
                {
                    device.MakeCurrent();

                    XInput.m_IsConnected = true;
                }

                break;
            case InputDeviceChange.Disconnected:
                Debug.Log("Device removed: " + device);

                if (device is DualShockGamepad)
                {
                    DS4.m_IsConnected = false;
                }
                else if (device is XInputController)
                {
                    XInput.m_IsConnected = false;
                }

                break;
        }
    }
}
