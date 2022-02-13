// System namespaces
using System;
using System.Collections;
// Unity namespaces
using UnityEngine;
using UnityEngine.UI;
// Package namespaces
using BezierSolution;

public class Enemy : MonoBehaviour
{
    public AudioSource m_enemyHit;
    public AudioSource m_enemyDestroy;

    // Amount this enemy will add to score when destroyed
    public int m_ScoreAdd = 100;
    // Enemy destroyed effect
    public GameObject m_DestroyEffect;

    [Header("Movement")]
    // Enemy movement speed across the path
    public float m_Speed = 3f;
    // User defined points to stop and shoot the player at
    public StopPoint[] m_StopPoints;

    [Header("Health")]
    // Maximum enemy health
    public float m_MaxHealth = 2f;
    // Current enemy health
    public float m_Health = 2f;
    // Canvas containing health bar UI
    public Canvas m_HealthCanvas;
    // Health bar UI
    public Image m_HealthBar;

    [Header("Shooting")]
    public GameObject m_Projectile;
    public float m_ShootFrequency;

    // Cached vars
    private BezierWalkerWithSpeed m_BezierWalker;
    private Transform m_Transform;
    private GameObject m_Player;

    // Index indicating the current stop to target next
    private int m_CurrentStop = 0;
    // Is this enemy currently moving?
    private bool m_IsMoving = true;

    private bool m_IsShooting = true;

    [Serializable]
    public struct StopPoint
    {
        public float m_StopAt;  // Location on path the enemy should stop moving at (normalized time)
        public float m_Time;    // How long the enemy should stop at this location for
    }

    void Start()
    {
        m_BezierWalker = GetComponent<BezierWalkerWithSpeed>();
        m_BezierWalker.speed = m_Speed;

        m_Transform = transform;
        m_Player = GameObject.FindGameObjectWithTag("Player");
    }

    IEnumerator MovementTimer()
    {
        // Stop moving this enemy
        m_IsMoving = false;
        m_BezierWalker.speed = 0;

        yield return new WaitForSeconds(m_StopPoints[m_CurrentStop].m_Time);

        // Resume moving along the path
        m_BezierWalker.speed = m_Speed;
        m_IsMoving = true;
        m_CurrentStop++;
    }

    IEnumerator ShootTimer()
    {
        m_IsShooting = false;

        yield return new WaitForSeconds(m_ShootFrequency);

        Instantiate(m_Projectile, m_Transform.position, m_Transform.rotation);

        m_IsShooting = true;
    }

    void Update()
    {
        // Keep checking for when the enemy reaches the next stop point
        if(m_IsMoving && m_CurrentStop < m_StopPoints.Length)
        {
            if (m_BezierWalker.NormalizedT >= m_StopPoints[m_CurrentStop].m_StopAt)
                StartCoroutine(MovementTimer());
        }

        // Periodically shoot assigned projectile towards player
        if (m_IsShooting && m_Projectile != null)
            StartCoroutine(ShootTimer());

        // Always face towards the player
        m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.LookRotation(m_Player.transform.position - m_Transform.position), 0.05f);

        // Destroy this enemy and add to players' score
        if(m_Health <= 0)
        {
            m_Player.GetComponent<Player>().m_Score += m_ScoreAdd;

            if (m_DestroyEffect != null)
                Instantiate(m_DestroyEffect, m_Transform.position, Quaternion.Euler(-90, 0, 0));

            Destroy(gameObject);
            m_enemyDestroy.Play();
        }

        // Always billboard the health bar UI
        m_HealthCanvas.transform.rotation = m_Player.transform.rotation;

        // Update health bar UI
        m_HealthBar.fillAmount = m_Health / m_MaxHealth;
    }

    /*
     * Invoked upon reaching the end of the path.
     */
    public void PathCompletedFunc()
    {
        Destroy(gameObject);
    }
}
