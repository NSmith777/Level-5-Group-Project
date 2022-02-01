// System namespaces
using System.Collections;
// Unity namespaces
using UnityEngine;

/*
 * Simple dummy projectile prefab script.
 */
public class ProjectileTest : MonoBehaviour
{
    // Firing speed
    public float m_Speed = 10f;
    // How long will this projectile stay alive for?
    public float m_AliveTime = 4f;

    /*
     * Waits for X seconds, then destroys this projectile.
     */
    IEnumerator kill()
    {
        yield return new WaitForSeconds(m_AliveTime);
        Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(kill());
    }

    void Update()
    {
        transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);
    }
}
