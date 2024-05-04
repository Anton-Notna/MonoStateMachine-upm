using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MonoStateMachine.Editor
{
    public class EntryNode : BaseNode
    {
        private static readonly Vector2 _defaultScale = new Vector2(100, 75);

        public EntryPoint Data => new EntryPoint(GUID, Position);

        public override string Name => "Entry point";

        public override Color Color => new Color32(113, 189, 132, 255);

        public EntryNode(Vector2 position, GraphView graph, Action<string> changed) : base(position, _defaultScale, graph, changed) { }

        public EntryNode(EntryPoint data, GraphView graph, Action<string> changed) : base(data, _defaultScale, graph, changed) { }

        protected override void DrawContent()
        {
            GUILayout.Box("Entry point", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            base.DrawContent();
        }

        protected override void FillContextMenu(GenericMenu menu)
        {
            if (Graph.ComputeConnected(this).Count() > 0)
            {
                menu.AddDisabledItem(new GUIContent("Already connected"));
            }
            else
            {
                foreach (var state in Graph.States)
                    menu.AddItem(new GUIContent("Connect/" + state.Name), false, () => Graph.Connect(this, state));
            }

            base.FillContextMenu(menu);
        }
    }
}