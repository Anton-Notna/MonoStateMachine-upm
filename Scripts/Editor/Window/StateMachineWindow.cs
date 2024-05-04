using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MonoStateMachine.Editor
{
    public class StateMachineWindow : EditorWindow
    {
        private StateMachine _current;
        private GraphView _view;

        private bool Compiling => EditorApplication.isUpdating || EditorApplication.isCompiling || AssetDatabase.IsAssetImportWorkerProcess();

        private StateMachine Selected =>
            Selection.gameObjects
            .Where(obj => obj.TryGetComponent<StateMachine>(out _))
            .Select(obj => obj.GetComponent<StateMachine>())
            .FirstOrDefault();

        [MenuItem("Window/MonoStateMachine")]
        public static void OpenWindow()
        {
            StateMachineWindow window = GetWindow<StateMachineWindow>();
            window.titleContent = new GUIContent(nameof(StateMachineWindow));
        }

        private void OnEnable()
        {
            //EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Undo.undoRedoPerformed += OnUndo;
        }

        private void OnDisable()
        {
            //EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            Undo.undoRedoPerformed -= OnUndo;
            Clear();
        }

        private void OnLostFocus()
        {
            CheckSave("Graph changed");
        }

        private void OnDestroy()
        {
            CheckSave("Graph changed");
        }

        private void OnGUI()
        {
            if (Compiling)
            {
                GUILayout.Label("Compiling..");

                Clear();
                return;
            }

            if (Selected == null)
            {
                GUILayout.Label("Select StateMachine");
                Clear();
                return;
            }

            if (Selected != _current)
                InitGraph(Selected);

            _view.OnGUI();
        }

        private void InitGraph(StateMachine selected)
        {
            Clear();

            _current = selected;
            GraphData data = _current.Load();
            if (data == null)
                data = new GraphData();

            _view = new GraphView(data, _current, ShowToolbar, OnChanged, ComputeCenter, Repaint);
        }

        private void OnUndo()
        {
            if (_current == null)
                return;

            if (_view == null)
                return;

            if (_current.DataHashCode == _view.CollectData().GetHashCode())
                return;

            Load();
            Repaint();
        }

        private void OnChanged(string recordMessage)
        {
            bool changed = CheckSave(recordMessage);
            Debug.Log($"{recordMessage}, changed: {changed}");
            Repaint();
        }

        private bool CheckSave(string recordMessage)
        {
            if (_view == null || _current == null)
                return false;

            GraphData data = _view.CollectData();
            bool changed = data.GetHashCode() != _current.DataHashCode;
            if (changed)
                Save(data, recordMessage);

            return changed;

            /*bool save = EditorUtility.DisplayDialog("You have unsaved changes", "Would you like to do?", "Save", "Continue without saving");
            if (save)*/
        }

        private void Clear()
        {
            _current = null;
            _view = null;
        }

        private void Save() => Save(_view.CollectData(), "Graph saved");

        private void Save(GraphData data, string recordMessage)
        {
            Undo.RecordObject(_current, recordMessage);
            _current.Save(data);
            EditorUtility.SetDirty(_current);
        }

        private void Load()
        {
            if (Selected != null)
                InitGraph(Selected);
        }

        private void ShowToolbar()
        {
            GenericMenu menu = new GenericMenu();

            if (_view.EntryExists == false)
            {
                menu.AddItem(new GUIContent("New Entry point"), false, _view.CreateEntryPoint);
                menu.AddSeparator("");
            }

            menu.AddItem(new GUIContent("New State"), false, _view.CreateState);
            menu.AddItem(new GUIContent("New Condition"), false, _view.CreateCondition);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Move all in bounds"), false, _view.MoveAllInBounds);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Load"), false, Load);
            menu.AddItem(new GUIContent("Save"), false, Save);

            menu.ShowAsContext();
        }

        private Vector2 ComputeCenter() => new Vector2(position.width, position.height) * 0.5f;
    }
}