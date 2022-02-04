// System namespaces
using System;
using System.Collections;
// Unity namespaces
using UnityEngine;
// Package namespaces
using BezierSolution;

public class EnemyFactory : MonoBehaviour
{
    public BezierSpline m_SplineToAssign;
    public EnemyEntry[] m_EnemySpawnList;

    private int m_CurrentEnemy = 0;
    private int m_CurrentRepeat = 0;

    private bool m_CanSpawnNext = true;

    [Serializable]
    public struct EnemyEntry
    {
        public GameObject m_EnemyObj;   // Enemy GameObject/Prefab to spawn
        public float m_NextSpawnTime;   // How long until the next enemy in the list will be spawned
        public int m_Count;             // How many times the same enemy should spawn repeatedly
    }

    void Start()
    {
        // Start deactivated
        gameObject.SetActive(false);
    }

    IEnumerator SpawnTimer()
    {
        m_CanSpawnNext = false;

        GameObject EnemyGO = Instantiate(m_EnemySpawnList[m_CurrentEnemy].m_EnemyObj, m_SplineToAssign.GetPoint(0.0f), Quaternion.identity);
        EnemyGO.GetComponent<BezierWalkerWithSpeed>().spline = m_SplineToAssign;

        yield return new WaitForSeconds(m_EnemySpawnList[m_CurrentEnemy].m_NextSpawnTime);

        m_CanSpawnNext = true;

        if(m_CurrentRepeat < m_EnemySpawnList[m_CurrentEnemy].m_Count - 1)
        {
            m_CurrentRepeat++;
        }
        else
        {
            m_CurrentEnemy++;
            m_CurrentRepeat = 0;
        }
    }

    void Update()
    {
        if(m_CanSpawnNext && m_CurrentEnemy < m_EnemySpawnList.Length)
        {
            StartCoroutine(SpawnTimer());
        }
    }
}
