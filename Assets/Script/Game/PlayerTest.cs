using System;
using UnityEngine;
using UnityEngine.InputSystem;
using BezierSolution;

public class PlayerTest : MonoBehaviour
{
    private Gamepad controller = null;
    private Transform m_transform;

    public BezierWalkerWithSpeed testBezier;

    Quaternion gyroRotation = Quaternion.identity;

    Vector2Int lastTouchPos;
    bool is_not_touched = false;

    public GameObject projectileObj;
    bool has_shot_projectile = false;

    void Start() {
        controller = DS4.getController();
        m_transform = transform;
    }

    void Update() {
        if (controller == null) {
            try {
                controller = DS4.getController();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        else {
            // Press circle button to reset rotation
            if (controller.buttonEast.isPressed)
                gyroRotation = Quaternion.identity;

            // Move across the bezier line
            if (controller.dpad.up.isPressed)
                testBezier.speed = 3;
            else if (controller.dpad.down.isPressed)
                testBezier.speed = -3;
            else
                testBezier.speed = 0;

            gyroRotation *= DS4.getRotation(4000 * Time.deltaTime);

            if (DS4.isTouchHeld()) {
                Vector2Int tpad1 = DS4.getTouch();

                if (is_not_touched)
                {
                    lastTouchPos = tpad1;
                    is_not_touched = false;
                }

                Vector3 tpad_delta = new Vector3(
                    tpad1.x - lastTouchPos.x,
                    0,
                    -(tpad1.y - lastTouchPos.y)
                    );

                Debug.DrawRay(m_transform.position, m_transform.forward * 300.0f, Color.green);

                if(tpad_delta.magnitude > 50f && !has_shot_projectile)
                {
                    Vector3 instantiate_pos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.0f, Camera.main.nearClipPlane));
                    Quaternion projectile_dir = Quaternion.LookRotation(tpad_delta);

                    if (Physics.Raycast(m_transform.position, m_transform.forward, out RaycastHit hit, 300.0f))
                        Instantiate(projectileObj, instantiate_pos, Quaternion.LookRotation(hit.point - instantiate_pos) * projectile_dir);
                    else
                        Instantiate(projectileObj, instantiate_pos, m_transform.rotation * projectile_dir);

                    has_shot_projectile = true;
                }

                lastTouchPos = tpad1;
            }
            else {
                is_not_touched = true;
                has_shot_projectile = false;
            }

            m_transform.rotation = Quaternion.LookRotation(testBezier.Spline.GetTangent(testBezier.NormalizedT));
            m_transform.rotation *= gyroRotation;
        }
    }
}
