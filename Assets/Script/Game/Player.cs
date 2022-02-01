// System namespaces
using System;
// Unity namespaces
using UnityEngine;
using UnityEngine.InputSystem;
// Package namespaces
using BezierSolution;

/*
 * Provides interaction and functionality to the in-game Player prefab.
 */
public class Player : MonoBehaviour
{
    // Stage's main rail path
    public BezierWalkerWithSpeed m_BezierWalker;
    // Projectile prefab to shoot
    public GameObject m_Projectile;

    // Cached vars
    private Gamepad m_Gamepad = null;
    private Transform m_Transform;

    // Keep track of local gyroscope rotation
    Quaternion m_GyroRotation = Quaternion.identity;

    // Touchpad state vars
    Vector2Int m_LastTouchPos;
    bool m_IsNotTouched = false;
    bool m_HasShotProjectile = false;

    void Start() {
        m_Gamepad = DS4.GetController();
        m_Transform = transform;
    }

    void Update() {
        // Make sure a gamepad is plugged in
        if (m_Gamepad == null) {
            try {
                m_Gamepad = DS4.GetController();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        else {
            // Press circle button to reset rotation (TODO: Smooth rotation transition)
            if (m_Gamepad.buttonEast.isPressed)
                m_GyroRotation = Quaternion.identity;

            // (Debug) Use the d-pad to move across the bezier line
            if (m_Gamepad.dpad.up.isPressed)
                m_BezierWalker.speed = 3;
            else if (m_Gamepad.dpad.down.isPressed)
                m_BezierWalker.speed = -3;
            else
                m_BezierWalker.speed = 0;

            // Poll gyroscope rotation data
            m_GyroRotation *= DS4.GetRotation(4000 * Time.deltaTime);

            // Poll touch data to shoot the assigned projectile
            if (DS4.IsTouchHeld()) {
                Vector2Int tpad1 = DS4.GetTouch();

                // If we've just (re-)touched down on the touchpad, then store the current touch position
                if (m_IsNotTouched) {
                    m_LastTouchPos = tpad1;
                    m_IsNotTouched = false;
                }

                // Calculate distance vector in which our finger swipes across the touchpad
                Vector3 tpad_delta = new Vector3(
                    tpad1.x - m_LastTouchPos.x,
                    0,
                    -(tpad1.y - m_LastTouchPos.y)
                    );

                Debug.DrawRay(m_Transform.position, m_Transform.forward * 300.0f, Color.green);

                // Here, two checks are performed in order to shoot a projectile.
                // First, we see if the player has swiped across the touchpad fast enough, as per our swipe sensitivity setting.
                // (TODO: Add configurable finger swipe sensitivity setting and add a UI option for it!)
                // Second, we make sure we haven't already fired a projectile while our finger has currently been held on the touchpad.
                if(tpad_delta.magnitude > 50f && !m_HasShotProjectile) {
                    // Fire the projectile from the centre-bottom of our first-person view
                    Vector3 instantiate_pos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.0f, Camera.main.nearClipPlane));

                    // Calculate a rotation offset based on which direction we swiped across the touchpad
                    // (TODO: Account for human error in finger swipe direction; it's too difficult to keep firing at the same spot!)
                    Quaternion projectile_dir = Quaternion.LookRotation(tpad_delta);

                    // Fire the projectile directly towards the hit point, otherwise just blindly fire forwards
                    if (Physics.Raycast(m_Transform.position, m_Transform.forward, out RaycastHit hit, 300.0f))
                        Instantiate(m_Projectile, instantiate_pos, Quaternion.LookRotation(hit.point - instantiate_pos) * projectile_dir);
                    else
                        Instantiate(m_Projectile, instantiate_pos, m_Transform.rotation * projectile_dir);

                    // Make sure we don't run this again until we release our finger off the touchpad
                    m_HasShotProjectile = true;
                }

                // Store this frame's touch position to use in the next frame
                m_LastTouchPos = tpad1;
            }
            else {
                // Reset the touchpad state
                m_IsNotTouched = true;
                m_HasShotProjectile = false;
            }

            // Apply processed rotation to this player.
            // We need our orientation relative to the forward dir against the spline path (tangent).
            m_Transform.rotation = Quaternion.LookRotation(m_BezierWalker.Spline.GetTangent(m_BezierWalker.NormalizedT));
            m_Transform.rotation *= m_GyroRotation;
        }
    }
}
