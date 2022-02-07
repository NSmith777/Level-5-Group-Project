﻿// System namespaces
using System;
// Unity namespaces
using UnityEngine;
using UnityEngine.InputSystem.DualShock;

/*
 * Rotates the DS4 GUI model to provide visualisation of gyroscope data.
 */
public class ControllerGUI : MonoBehaviour
{
    // Cached vars
    private Transform m_Transform;

    void Start()
    {
        m_Transform = transform;
    }

    void Update()
    {
        // Make sure a gamepad is plugged in
        if (DS4.m_IsConnected)
        {
            // Press circle button to reset rotation
            if (DS4.GetController().buttonEast.isPressed)
                m_Transform.rotation = Quaternion.identity;

            // Apply processed rotation to the gamepad mesh
            m_Transform.rotation *= DS4.GetRotation(Mathf.Rad2Deg * 80f * Time.deltaTime);
        }
    }
}
