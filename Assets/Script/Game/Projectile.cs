﻿// System namespaces
using System.Collections;
// Unity namespaces
using UnityEngine;

/*
 * Simple dummy projectile prefab script.
 */
public class Projectile : MonoBehaviour
{
    // How much health this projectile removes from an enemy
    public int m_Strength = 1;
    // Firing speed
    public float m_Speed = 10f;
    // How long will this projectile stay alive for?
    public float m_AliveTime = 4f;
    // Does this projectile damage the player?
    public bool m_CanHarmPlayer = false;

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
        float next_step = m_Speed * Time.deltaTime;

        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, next_step))
        {
            if(m_CanHarmPlayer)
            {
                // Deal damage to the player
                if (hit.collider.CompareTag("Player"))
                {
                    hit.collider.GetComponent<Player>().m_Health -= m_Strength;
                    Destroy(gameObject);
                }
            }
            else
            {
                // Deal damage to collided enemies
                if (hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.GetComponent<Enemy>().m_Health -= m_Strength;
                    Destroy(gameObject);
                }
                // Destroy incoming collided enemy projectiles 
                if(hit.collider.CompareTag("Projectile"))
                {
                    Destroy(hit.collider.gameObject);
                    Destroy(gameObject);
                }
            }
        }

        transform.position += transform.forward * next_step;
    }
}
