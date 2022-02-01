// System namespaces
using System.IO;
// Unity namespaces
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/*
 * Singleton class in which all scripts derive DualShock 4 (DS4) gamepad input from.
 * (Note: This class uses the new Input System, don't add code that uses the legacy input API here. ~ Nathan)
 */
public class DS4
{
    // Gyroscope data
    private static ButtonControl m_GyroX, m_GyroY, m_GyroZ;
    // Touchpad state data
    private static ButtonControl m_Touch1X, m_Touch1Split, m_Touch1Y, m_Touch1Down;

    /*
     * Class constructor
     * Loads our custom DS4 gamepad layout into Input System.
     */
    static DS4() {
        // Read custom DS4 layout from JSON file
        string layout = File.ReadAllText(Application.streamingAssetsPath + "/Input/customLayout.json");

        // Overwrite the default Gamepad layout
        InputSystem.RegisterLayoutOverride(layout, "DualShock4GamepadHID");

        BindControls(Gamepad.current);
    }

    /*
     * Binds the custom gyro/touchpad entires to our local vars to be read by the game.
     */
    private static void BindControls(Gamepad ds4) {
        // Bind raw gyroscope readings
        m_GyroX = ds4.GetChildControl<ButtonControl>("gyro X 14");
        m_GyroY = ds4.GetChildControl<ButtonControl>("gyro Y 16");
        m_GyroZ = ds4.GetChildControl<ButtonControl>("gyro Z 18");

        // Bind raw touchpad readings
        m_Touch1X = ds4.GetChildControl<ButtonControl>("touch 1 36");
        m_Touch1Split = ds4.GetChildControl<ButtonControl>("touch 1 37");
        m_Touch1Y = ds4.GetChildControl<ButtonControl>("touch 1 38");
        m_Touch1Down = ds4.GetChildControl<DiscreteButtonControl>("touch 1 down");
    }

    /*
     * Returns the active gamepad currently bound to Input System.
     */
    public static Gamepad GetController() {
        return Gamepad.current;
    }

    /*
     * Returns touch coordinates from the DS4 touchpad.
     */
    public static Vector2Int GetTouch() {
        // Method to decode touchpad data adapted from info at:
        // https://www.psdevwiki.com/ps4/DS4-USB#Data_Format

        // Convert raw readings from float to byte
        byte x = (byte)(m_Touch1X.ReadValue() * 255);
        byte split = (byte)(m_Touch1Split.ReadValue() * 255);
        byte y = (byte)(m_Touch1Y.ReadValue() * 255);

        // To decode, each coordinate (x & y) is using 12 bits ... mask/split and swap the middle byte
        int touch_x = x | ((split & 0x0f) << 8);
        int touch_y = (y << 4) | ((split & 0xf0) >> 4);

        return new Vector2Int(touch_x, touch_y);
    }

    /*
     * Is the touchpad currently being touched?
     */
    public static bool IsTouchHeld() {
        return m_Touch1Down.ReadValue() == 1f;
    }

    /*
     * Creates a rotation quaternion based on gyroscope data.
     * Can pass an optional value to scale the returned rotation.
     */
    public static Quaternion GetRotation(float scale = 1) {
        float x = ProcessRawData(m_GyroX.ReadValue()) * scale;
        float y = ProcessRawData(m_GyroY.ReadValue()) * scale;
        float z = -ProcessRawData(m_GyroZ.ReadValue()) * scale;
        return Quaternion.Euler(x, y, z);
    }

    /*
     * Helper function to normalize gyroscope readings.
     */
    private static float ProcessRawData(float data) {
        return data > 0.5 ? 1 - data : -data;
    }
}