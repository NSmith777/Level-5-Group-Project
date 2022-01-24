using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Bezier))]
public class BezierEditor : Editor
{
    private void OnSceneGUI()
    {
        Bezier be = target as Bezier;

        for(int i = 0; i < be.points.Length - 3; i += 3)
        {
            Handles.Label(be.points[i], "[" + i + "]");
            Handles.Label(be.points[i + 1], "[" + (i + 1) + "]");
            Handles.Label(be.points[i + 2], "[" + (i + 2) + "]");
            Handles.Label(be.points[i + 3], "[" + (i + 3) + "]");

            be.points[i] = Handles.PositionHandle(be.points[i], Quaternion.identity);
            be.points[i+1] = Handles.PositionHandle(be.points[i+1], Quaternion.identity);
            be.points[i+2] = Handles.PositionHandle(be.points[i+2], Quaternion.identity);
            be.points[i+3] = Handles.PositionHandle(be.points[i+3], Quaternion.identity);

            Handles.DrawBezier(be.points[i], be.points[i + 3], be.points[i + 1], be.points[i + 2], Color.red, null, 5f);
        }
    }
}
#endif
