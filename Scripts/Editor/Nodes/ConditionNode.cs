using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MonoStateMachine.Editor
{
    public class ConditionNode : NamedNode
    {
        private static readonly Vector2 _defaultScale = new Vector2(125, 100);

        private ConditionData _data;

        public ConditionData Data => new ConditionData(_data, Name, GUID, Position);

        public override Color Color => new Color32(83, 183, 219, 255);

        public ConditionNode(ConditionData data, GraphView graph, Action<string> changed) : base(data, _defaultScale, graph, changed)
        {
            _data = data;
        }

        protected override void DrawContent()
        {
            GUILayout.Label("Condition", GUIStyles.Header);

            base.DrawContent();

            if (_data.Condition == null)
                GUILayout.Label("Select condition", GUIStyles.Warning);
            else
                GUILayout.Label(_data.Condition.ToString().Replace('/', ' '), GUIStyles.Wrap);
        }

        protected override void FillContextMenu(GenericMenu menu)
        {
            foreach (Condition condition in _data.GetPossibleConditions())
                menu.AddItem(new GUIContent("Select condition/" + condition.ToString()), condition.Equals(_data.Condition), () => SelectCondition(condition));

            menu.AddSeparator("");

            if (Graph.OutConnections(this) > 0)
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

        private void SelectCondition(Condition condition)
        {
            _data = _data.SelectCondition(condition);
            Save("Condition selected");
        }
    }
}