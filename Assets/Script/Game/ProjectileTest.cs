using System.Collections;
using UnityEngine;

public class ProjectileTest : MonoBehaviour
{
    public float speed = 10f;
    public float aliveTime = 4f;

    IEnumerator kill()
    {
        yield return new WaitForSeconds(aliveTime);
        Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(kill());
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
