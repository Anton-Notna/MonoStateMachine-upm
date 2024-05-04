using System;
using UnityEngine;

namespace MonoStateMachine.Editor
{
    public abstract class NamedNode : BaseNode
    {
        private string _name;

        public override string Name => _name;

        public NamedNode(NamedData data, Vector2 scale, GraphView graph, Action<string> changed) : base(data, scale, graph, changed)
        {
            _name = data.Name;
        }

        protected override void DrawContent()
        {
            base.DrawContent();
            string newName = GUILayout.TextField(_name);

            if (newName == _name)
                return;

            _name = newName;
            Save("Name changed");
        }
    }
}