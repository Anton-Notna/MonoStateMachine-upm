using System;
using UnityEditor;
using UnityEngine;

namespace MonoStateMachine.Editor
{
    public abstract class BaseNode
    {
        private const float _activeWidth = 5f;

        public string GUID { get; private set; }

        public Vector2 Position { get; private set; }

        public GraphView Graph { get; private set; }

        public Vector2 Center => Rect.center;

        public abstract string Name { get; }

        public abstract Color Color { get; }

        private Rect Rect => new Rect(Position, _scale);

        private readonly Vector2 _scale;
        private readonly Action<string> _changed;

        public BaseNode(Vector2 position, Vector2 scale, GraphView graph, Action<string> changed)
        { 
            GUID = Guid.NewGuid().ToString();
            Position = position;
            _scale = scale;
            Graph = graph;
            _changed = changed;
        }

        public BaseNode(BaseData data, Vector2 scale, GraphView graph, Action<string> changed)
        {
            GUID = data.GUID;
            Position = data.NodePosition;
            _scale = scale;
            Graph = graph;
            _changed = changed;
        }

        public void Draw(Vector2 canvasOffset, bool active)
        {
            Color defaultColor = GUI.backgroundColor;

            Rect rect = Rect;
            rect.position += canvasOffset;
            if (active)
            {
                Rect expandedRect = rect;
                expandedRect.position -= Vector2.one * _activeWidth;
                expandedRect.size += Vector2.one * _activeWidth * 2;

                GUILayout.BeginArea(expandedRect);
                GUI.backgroundColor = (Color.yellow + Color.green) * 0.5f;
                GUILayout.Box(string.Empty, GUI.skin.button, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                GUILayout.EndArea();
            }

            GUILayout.BeginArea(rect);
            GUI.backgroundColor = Color;
            GUILayout.Box(string.Empty, GUI.skin.button, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.EndArea();

            GUI.backgroundColor = defaultColor;

            GUILayout.BeginArea(rect);
            DrawContent();
            GUILayout.EndArea();
        }

        public void Move(Vector2 delta) => Position += delta;

        internal void SetPosition(Vector2 position) => Position = position;

        public bool ContainsCursor(Vector2 cursor) => Rect.Contains(cursor);

        public void ShowContextMenu()
        {
            GenericMenu menu = new GenericMenu();
            FillContextMenu(menu);
            menu.ShowAsContext();
        }

        protected void Save(string recordMessage) => _changed.Invoke(recordMessage);

        protected virtual void FillContextMenu(GenericMenu menu)
        {
            menu.AddSeparator("");

            foreach (var connected in Graph.ComputeConnected(this))
                menu.AddItem(new GUIContent("Disconnect/" + connected.Name), false, () => Graph.Disconnect(connected, this));

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Remove"), false, () => Graph.Remove(this));
        }

        protected virtual void DrawContent()
        {

        }

    }
}