// System namespaces
using System;
using System.Collections;
// Unity namespaces
using UnityEngine;
// Package namespaces
using BezierSolution;

public class Enemy : MonoBehaviour
{
    // Maximum enemy health
    public float m_MaxHealth = 2f;
    // Current enemy health
    public float m_Health = 2f;
    // Amount this enemy will add to score when destroyed
    public int m_ScoreAdd = 100;
    // Enemy movement speed across the path
    public float m_Speed = 3f;
    // User defined points to stop and shoot the player at
    public StopPoint[] m_StopPoints;

    // Cached vars
    private BezierWalkerWithSpeed m_BezierWalker;
    private Transform m_Transform;
    private GameObject m_Player;

    // Index indicating the current stop to target next
    private int m_CurrentStop = 0;
    // Is this enemy currently moving?
    private bool m_IsMoving = true;

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

    void Update()
    {
        // Keep checking for when the enemy reaches the next stop point
        if(m_IsMoving && m_CurrentStop < m_StopPoints.Length)
        {
            if (m_BezierWalker.NormalizedT >= m_StopPoints[m_CurrentStop].m_StopAt)
                StartCoroutine(MovementTimer());
        }

        // Always face towards the player
        m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.LookRotation(m_Player.transform.position - m_Transform.position), 0.05f);

        // Destroy this enemy and add to players' score
        if(m_Health <= 0)
        {
            m_Player.GetComponent<Player>().m_Score += m_ScoreAdd;

            Destroy(gameObject);
        }
    }

    /*
     * Invoked upon reaching the end of the path.
     */
    public void PathCompletedFunc()
    {
        Destroy(gameObject);
    }
}
