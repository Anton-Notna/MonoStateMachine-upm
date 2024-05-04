using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MonoStateMachine.Editor
{
    public class StateNode : NamedNode
    {
        private static readonly Vector2 _defaultScale = new Vector2(125, 100);

        private StateData _data;

        public StateData Data => new StateData(_data, Name, GUID, Position);

        public override Color Color => new Color32(186, 155, 89, 255);


        public StateNode(StateData data, GraphView graph, Action<string> changed) : base(data, _defaultScale, graph, changed)
        {
            _data = data;
        }

        protected override void DrawContent()
        {
            GUILayout.Label("State", GUIStyles.Header);

            base.DrawContent();

            if (_data.State == null)
                GUILayout.Label("Select state", GUIStyles.Warning);
            else
                GUILayout.Label(_data.State.ToString().Replace('/',' '), GUIStyles.Wrap);
        }

        protected override void FillContextMenu(GenericMenu menu)
        {
            foreach (State state in _data.GetPossibleStates())
                menu.AddItem(new GUIContent("Select State/" + state.ToString()), _data.State == state, () => SelectState(state));

            if (menu.GetItemCount() == 0)
                menu.AddDisabledItem(new GUIContent("No states in gameObject"));

            menu.AddSeparator("");
            var connected = Graph.ComputeConnected(this);

            foreach (var condition in Graph.Conditions)
                menu.AddItem(new GUIContent("Connect/" + condition.Name), connected.Any(unit => unit == condition), () => Graph.Connect(this, condition));
            
            base.FillContextMenu(menu);
        }

        private void SelectState(State state)
        {
            _data = _data.SelectState(state);
            Save("State selected");
        }
    }
}