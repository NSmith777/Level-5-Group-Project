using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerGUI : MonoBehaviour
{
    private Gamepad controller = null;
    private Transform m_transform;

    void Start()
    {
        controller = DS4.getController();
        m_transform = transform;
    }

    void Update()
    {
        if (controller == null) {
            try {
                controller = DS4.getController();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        else
        {
            // Press circle button to reset rotation
            if (controller.buttonEast.isPressed)
                m_transform.rotation = Quaternion.identity;

            m_transform.rotation *= DS4.getRotation(Mathf.Rad2Deg * 80f * Time.deltaTime);
        }
    }
}
