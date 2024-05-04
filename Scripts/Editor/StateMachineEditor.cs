using UnityEditor;
using UnityEngine;

namespace MonoStateMachine.Editor
{
    [CustomEditor(typeof(StateMachine))]
    public class StateMachineEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            if (GUILayout.Button("Open window"))
                StateMachineWindow.OpenWindow();
        }
    }
}