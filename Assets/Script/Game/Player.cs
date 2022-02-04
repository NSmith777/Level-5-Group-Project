// System namespaces
using System;
using System.Collections;
// Unity namespaces
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
// Package namespaces
using BezierSolution;

/*
 * Provides interaction and functionality to the in-game Player prefab.
 */
public class Player : MonoBehaviour
{
    [Header("Movement")]
    // User defined points to stop and shoot on the path
    public StopPoint[] m_StopPoints;
    // Movement speed along the rail path
    public float m_RailSpeed = 3f;
    // Walk bobbing speed
    public float m_BobAnimSpeed = 0.02f;
    // Walk bobbing strength
    public float m_BobAnimStrength = 0.15f;

    [Serializable]
    public struct StopPoint
    {
        public GameObject[] m_EnableEnmRails;   // Enemy rails to enable at this stop point
        public float m_StopAt;                  // Location on path the enemy should stop moving at (normalized time)
        public float m_Time;                    // How long the enemy should stop at this location for
    }

    [Header("Rotation")]
    // Gyro motion sensitivity multiplier
    public float m_GyroSensitivity = 4000f;

    [Header("Weapons")]
    // Projectile prefab to shoot
    public GameObject m_Projectile;

    [Header("HUD")]
    public Image ProgressBar;
    public Text ProgressPercent;

    // Cached vars
    private BezierWalkerWithSpeed m_BezierWalker;
    private Gamepad m_Gamepad = null;
    private Transform m_Transform;

    // Keep track of local gyroscope rotation
    Quaternion m_GyroRotation = Quaternion.identity;

    // Index indicating the current stop to target next
    private int m_CurrentStop = 0;
    // Is player currently moving?
    private bool m_IsMoving = true;
    // Keep track of animated walk sine value
    private float m_WalkAnimSine = 0f;

    // Touchpad state vars
    Vector2Int m_LastTouchPos;
    bool m_IsNotTouched = false;
    bool m_HasShotProjectile = false;

    void Start() {
        m_BezierWalker = GetComponent<BezierWalkerWithSpeed>();
        m_BezierWalker.speed = m_RailSpeed;

        m_Gamepad = DS4.GetController();
        m_Transform = transform;
    }

    IEnumerator MovementTimer()
    {
        // Stop moving this enemy
        m_IsMoving = false;
        m_BezierWalker.speed = 0;

        // Activate all enemy rails for this stop
        foreach(GameObject EnemyRailGO in m_StopPoints[m_CurrentStop].m_EnableEnmRails)
            EnemyRailGO.SetActive(true);

        yield return new WaitForSeconds(m_StopPoints[m_CurrentStop].m_Time);

        // Resume moving along the path
        m_BezierWalker.speed = m_RailSpeed;
        m_IsMoving = true;
        m_CurrentStop++;
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

            // Produce a walking animation by bobbing up/down using sine math
            if (m_BezierWalker.speed != 0) {
                transform.position += new Vector3(0, m_BobAnimStrength * Mathf.Sin(m_WalkAnimSine), 0);
                m_WalkAnimSine += m_BobAnimSpeed * m_RailSpeed;
            }

            // Poll gyroscope rotation data
            m_GyroRotation *= DS4.GetRotation(m_GyroSensitivity * Time.deltaTime);

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

                // Calculate touchpad swipe angle
                float shoot_angle = Mathf.Abs(Vector3.Angle(tpad_delta, Vector3.forward));

                // Here, three checks are performed in order to shoot a projectile.
                // First, we see if the player has swiped across the touchpad fast enough, as per our swipe sensitivity setting.
                // (TODO: Add configurable finger swipe sensitivity setting and add a UI option for it!)
                // Second, we make sure we haven't already fired a projectile while our finger has currently been held on the touchpad.
                // Lastly, check that the finger swipe motion is upwards.
                if (tpad_delta.magnitude > 50f && !m_HasShotProjectile && shoot_angle < 45f) {
                    // Fire the projectile from the centre-bottom of our first-person view
                    Vector3 instantiate_pos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.0f, Camera.main.nearClipPlane));

                    // Fire the projectile directly towards the hit point, otherwise just blindly fire forwards
                    if (Physics.Raycast(m_Transform.position, m_Transform.forward, out RaycastHit hit, 300.0f))
                        Instantiate(m_Projectile, instantiate_pos, Quaternion.LookRotation(hit.point - instantiate_pos));
                    else
                        Instantiate(m_Projectile, instantiate_pos, m_Transform.rotation);

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

        if(m_IsMoving && m_CurrentStop < m_StopPoints.Length)
        {
            if (m_BezierWalker.NormalizedT >= m_StopPoints[m_CurrentStop].m_StopAt)
                StartCoroutine(MovementTimer());
        }

        // Update progression HUD elements as per current position in the stage
        ProgressBar.fillAmount = m_BezierWalker.NormalizedT;
        ProgressPercent.text = Mathf.CeilToInt(m_BezierWalker.NormalizedT * 100) + "%";
    }
}
