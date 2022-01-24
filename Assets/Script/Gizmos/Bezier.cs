using UnityEngine;

public class Bezier : MonoBehaviour
{
    public Vector3[] points;

    public Vector3 GetPoint(float time)
    {
        float t = time % 1f;
        int idx = (int)time * 3 + 1;

        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * oneMinusT * points[idx - 1] +
            3f * oneMinusT * oneMinusT * t * points[idx] +
            3f * oneMinusT * t * t * points[idx + 1] +
            t * t * t * points[idx + 2];
    }

    public Vector3 GetDerivative(float time)
    {
        float t = time % 1f;
        int idx = (int)time * 3 + 1;

        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (points[idx] - points[idx - 1]) +
            6f * oneMinusT * t * (points[idx + 1] - points[idx]) +
            3f * t * t * (points[idx + 2] - points[idx + 1]);
    }
}
