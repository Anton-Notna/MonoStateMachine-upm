using UnityEditor;
using UnityEngine;

namespace MonoStateMachine.Editor
{
    public static class EditorWindowExtensions
    {
        public static void DrawArrow(Vector2 start, Vector2 end, Color color)
        {
            Handles.color = color;

            Vector2 center = (start + end) * 0.5f;
            Vector2 direction = (end - start).normalized;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            Vector2 arrowPoint1 = center + (-direction + perpendicular) * 10f;
            Vector2 arrowPoint2 = center + (-direction - perpendicular) * 10f;

            Handles.DrawAAPolyLine(5f, center, arrowPoint1);
            Handles.DrawAAPolyLine(5f, center, arrowPoint2);
            Handles.DrawAAPolyLine(5f, start, center);

            color.a *= 0.5f;
            Handles.color = color;

            Handles.DrawAAPolyLine(3.5f, center, end);

        }
    }
}