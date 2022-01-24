using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class DS4
{
    private static ButtonControl gyroX, gyroY, gyroZ; // Gyroscope
    private static ButtonControl touch1X, touch1Split, touch1Y, touch1down; // Touchpad

    static DS4()
    {
        // Read layout from JSON file
        string layout = File.ReadAllText(Application.streamingAssetsPath + "/Input/customLayout.json");

        // Overwrite the default layout
        InputSystem.RegisterLayoutOverride(layout, "DualShock4GamepadHID");

        bindControls(Gamepad.current);
    }

    public static Gamepad getController()
    {
        return Gamepad.current;
    }

    private static void bindControls(Gamepad ds4)
    {
        gyroX = ds4.GetChildControl<ButtonControl>("gyro X 14");
        gyroY = ds4.GetChildControl<ButtonControl>("gyro Y 16");
        gyroZ = ds4.GetChildControl<ButtonControl>("gyro Z 18");

        touch1X = ds4.GetChildControl<ButtonControl>("touch 1 36");
        touch1Split = ds4.GetChildControl<ButtonControl>("touch 1 37");
        touch1Y = ds4.GetChildControl<ButtonControl>("touch 1 38");
        touch1down = ds4.GetChildControl<DiscreteButtonControl>("touch 1 down");
    }

    public static Vector2Int getTouch()
    {
        byte x = (byte)(touch1X.ReadValue() * 255);
        byte split = (byte)(touch1Split.ReadValue() * 255);
        byte y = (byte)(touch1Y.ReadValue() * 255);

        int touch_x = x | ((split & 0x0f) << 8);
        int touch_y = (y << 4) | ((split & 0xf0) >> 4);

        return new Vector2Int(touch_x, touch_y);
    }

    public static bool isTouchHeld()
    {
        return touch1down.ReadValue() == 1f;
    }

    public static Quaternion getRotation(float scale = 1)
    {
        float x = processRawData(gyroX.ReadValue()) * scale;
        float y = processRawData(gyroY.ReadValue()) * scale;
        float z = -processRawData(gyroZ.ReadValue()) * scale;
        return Quaternion.Euler(x, y, z);
    }

    private static float processRawData(float data)
    {
        return data > 0.5 ? 1 - data : -data;
    }
}