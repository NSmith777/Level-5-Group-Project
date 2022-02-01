// System namespaces
using System;
// Unity namespaces
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Rotates the DS4 GUI model to provide visualisation of gyroscope data.
 */
public class ControllerGUI : MonoBehaviour
{
    // Cached vars
    private Gamepad m_Gamepad = null;
    private Transform m_Transform;

    void Start()
    {
        m_Gamepad = DS4.GetController();
        m_Transform = transform;
    }

    void Update()
    {
        // Make sure a gamepad is plugged in
        if (m_Gamepad == null) {
            try {
                m_Gamepad = DS4.GetController();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        else
        {
            // Press circle button to reset rotation
            if (m_Gamepad.buttonEast.isPressed)
                m_Transform.rotation = Quaternion.identity;

            // Apply processed rotation to the gamepad mesh
            m_Transform.rotation *= DS4.GetRotation(Mathf.Rad2Deg * 80f * Time.deltaTime);
        }
    }
}
