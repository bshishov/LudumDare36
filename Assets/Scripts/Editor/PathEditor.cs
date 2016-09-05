using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode]
[CustomEditor(typeof(PathMover))]
public class PathEditor : Editor
{
    void OnSceneGUI()
    {
        var t = target as PathMover;

        // Set the colour of the next handle to be drawn
        Handles.color = Color.magenta;

        for (var i = 0; i < t.Waypoints.Length; i++)
        {
            EditorGUI.BeginChangeCheck();
            
            var position = t.Waypoints[i].Position;
            var handledPosition = Handles.PositionHandle(position, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed waypoint Target");
                t.Waypoints[i].Position = handledPosition;
            }
        }
    }
}
