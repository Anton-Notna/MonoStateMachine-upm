using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace MonoStateMachine.Editor
{
    public class GraphView
    {
        private static readonly Vector2 _offset = new Vector2(10, 40);
        private static readonly int _positionEpsilon = 10;

        private readonly Action _onNullGraphSelected;
        private readonly Action<string> _onChanged;
        private readonly Action _repaint;

        private readonly Func<Vector2> _computeCenter;
        private readonly StateMachine _stateMachine;
        private readonly GameObject _root;
        private readonly List<BaseNode> _nodes = new List<BaseNode>();
        private readonly List<Link> _links = new List<Link>();

        private BaseNode _selected;
        private Vector2 _canvasOffset;

        public bool EntryExists => EntryNode != null;

        public IEnumerable<StateNode> States => GetNodes<StateNode>();

        public IEnumerable<ConditionNode> Conditions => GetNodes<ConditionNode>();

        private EntryNode EntryNode => GetNodes<EntryNode>().FirstOrDefault();

        private IEnumerable<BaseNode> Nodes => GetNodes<BaseNode>();

        public GraphView(GraphData data, StateMachine stateMachine, Action onNullGraphSelected, Action<string> onChanged, Func<Vector2> computeCenter, Action repaint)
        {
            _onNullGraphSelected = onNullGraphSelected;
            _onChanged = onChanged;
            _computeCenter = computeCenter;
            _stateMachine = stateMachine;
            _root = stateMachine.gameObject;

            SpawnNodes(data);
            ConnectNodes(data.Links);
            _canvasOffset = data.CanvasOffset;
            _repaint = repaint;
        }

        private IEnumerable<T> GetNodes<T>() where T : BaseNode => _nodes.Where(node => node is T).Select(node => node as T);

        internal void CreateEntryPoint()
        {
            AddNode(new EntryNode(new EntryPoint(Guid.NewGuid().ToString(), ComputeFreePoint()), this, _onChanged));
            _onChanged.Invoke("EntryPoint added");
        }

        internal void CreateState()
        {
            AddNode(new StateNode(new StateData(_root, "New State", Guid.NewGuid().ToString(), ComputeFreePoint()), this, _onChanged));
            _onChanged.Invoke("State added");
        }

        internal void CreateCondition()
        {
            AddNode(new ConditionNode(new ConditionData(_root, "New Condition", Guid.NewGuid().ToString(), ComputeFreePoint()), this, _onChanged));
            _onChanged.Invoke("Condition Node added");
        }

        public void Connect(EntryNode from, StateNode to)
        {
            if (OutConnections(from) > 0)
                throw new InvalidOperationException("EntryNode can have only one connection");

            ConnectBases(from, to);
            _onChanged.Invoke("Nodes connected");
        }

        public void Connect(StateNode from, ConditionNode to)
        {
            ConnectBases(from, to);
            _onChanged.Invoke("Nodes connected");
        }

        public void Connect(ConditionNode from, StateNode to)
        {
            if (OutConnections(from) > 0)
                throw new InvalidOperationException("ConditionNode can have only one connection");

            ConnectBases(from, to);
            _onChanged.Invoke("Nodes connected");
        }

        public IEnumerable<BaseNode> ComputeConnected(BaseNode node)
        {
            return _links
                .Where(link => link.BaseGuid == node.GUID)
                .Select(link => _nodes.Find(node => node.GUID == link.TargetGuid))
                .Concat(_links
                    .Where(link => link.TargetGuid == node.GUID)
                    .Select(link => _nodes.Find(node => node.GUID == link.BaseGuid)));
        }

        public void Remove(BaseNode node)
        {
            foreach (var link in _links.Where(link => link.Contains(node.GUID)))
                _links.Remove(link);

            _nodes.Remove(node);
            _onChanged.Invoke("Node removed");
        }

        public void Disconnect(BaseNode node1, BaseNode node2)
        {
            var link = _links.FirstOrDefault(link => link.Contains(node1.GUID) && link.Contains(node2.GUID));
            if (link != null)
                _links.Remove(link);

            _onChanged.Invoke("Node disconnected");
        }

        public int OutConnections(BaseNode from) => _links.Where(link => link.BaseGuid == from.GUID).Count();

        public void MoveAllInBounds()
        {
            foreach (var node in _nodes)
            {
                node.SetPosition(ComputeFreePoint());
            }

            _onChanged.Invoke("Nodes moved in bounds");
        }

        public GraphData CollectData()
        {
            return new GraphData(
                    EntryNode?.Data,
                    States.Select(state => state.Data),
                    Conditions.Select(condition => condition.Data),
                    _links.ToList(),
                    _canvasOffset);
        }

        public void OnGUI()
        {
            DrawConnections();
            DrawNodes();
            ProcessEvents(Event.current);
        }

        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                        OnLeftMouseDown(e.mousePosition);
                    else if (e.button == 1)
                        OnRightMouseDown(e.mousePosition);
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0)
                        OnLeftMouseDrag(e.delta, true);
                    else if (e.button == 2)
                        OnLeftMouseDrag(e.delta, false);
                    break;
                case EventType.MouseUp:
                    _onChanged.Invoke("Node moved");
                    break;
            }
        }

        private void OnRightMouseDown(Vector2 mousePosition)
        {
            if (FindNode(mousePosition, out BaseNode node))
                node.ShowContextMenu();
            else
                _onNullGraphSelected.Invoke();
        }

        private void OnLeftMouseDown(Vector2 mousePosition)
        {
            GUI.FocusControl(null);
            FindNode(mousePosition, out _selected);
        }

        private bool FindNode(Vector2 mousePosition, out BaseNode result)
        {
            foreach (var node in _nodes)
            {
                if (node.ContainsCursor(mousePosition - _canvasOffset))
                {
                    result = node;
                    return true;
                }
            }

            result = null;
            return false;
        }

        private void OnLeftMouseDrag(Vector2 delta, bool moveSelected)
        {
            if (moveSelected && _selected != null)
                MoveSelected(delta);
            else
                MoveCanvas(delta);

            _repaint.Invoke();
        }

        private void MoveSelected(Vector2 delta) => _selected.Move(delta);

        private void MoveCanvas(Vector2 delta) => _canvasOffset += delta;

        private void DrawConnections()
        {
            foreach (var link in _links)
            {
                var from = _nodes.Find(node => node.GUID == link.BaseGuid);
                var to = _nodes.Find(node => node.GUID == link.TargetGuid);

                EditorWindowExtensions.DrawArrow(
                    from.Center + _canvasOffset,
                    to.Center + _canvasOffset,
                    Color.white);
            }
        }

        private void DrawNodes()
        {
            HashSet<BaseNode> activeNodes = new HashSet<BaseNode>();
            foreach (var state in States)
            {
                if (_stateMachine.IsCurrentState(state.Data.State))
                {
                    activeNodes.Add(state);

                    var next = _links
                        .Where(link => link.BaseGuid == state.GUID)
                        .Select(link => _nodes.Find(node => node.GUID == link.TargetGuid));

                    foreach (var connected in next)
                        activeNodes.Add(connected);

                    break;
                }
            }

            foreach (var node in _nodes)
                node.Draw(_canvasOffset, activeNodes.Contains(node));
        }

        private void SpawnNodes(GraphData data)
        {
            if (data.EntryPoint != null)
                AddNode(new EntryNode(data.EntryPoint, this, _onChanged));

            foreach (StateData state in data.States)
                AddNode(new StateNode(state, this, _onChanged));

            foreach (ConditionData condition in data.Conditions)
                AddNode(new ConditionNode(condition, this, _onChanged));
        }

        private void ConnectNodes(IReadOnlyList<Link> links)
        {
            IEnumerable<BaseNode> nodes = Nodes;
            foreach (Link link in links)
            {
                ConnectBases(
                    nodes.FirstOrDefault(node => node.GUID == link.BaseGuid),
                    nodes.FirstOrDefault(node => node.GUID == link.TargetGuid));
            }
        }

        private void AddNode(BaseNode node)
        {
            _nodes.Add(node);
        }

        private void ConnectBases(BaseNode baseNode, BaseNode targetNode)
        {
            if (baseNode == null)
                return;

            if (targetNode == null)
                return;

            if (baseNode == targetNode || baseNode.GUID == targetNode.GUID)
                throw new InvalidOperationException($"Cannot connect to self: {baseNode.GUID}");

            foreach (Link old in _links)
            {
                if (old.BaseGuid == baseNode.GUID && old.TargetGuid == targetNode.GUID)
                    throw new InvalidOperationException($"Cannot connect alreadyConnected nodes: {baseNode.GUID} {targetNode.GUID}");
            }

            Link link = new Link(baseNode.GUID, targetNode.GUID);
            _links.Add(link);
        }

        private Vector2 ComputeFreePoint()
        {
            Vector2 position = _computeCenter.Invoke() - _canvasOffset;

            bool busy = true;
            while (busy)
            {
                busy = false;

                foreach (BaseNode node in Nodes)
                {
                    bool overlap = Vector2.SqrMagnitude(node.Position - position) < _positionEpsilon;
                    if (overlap)
                    {
                        position += _offset;
                        busy = true;
                        break;
                    }
                }
            }

            return position;
        }
    }
}