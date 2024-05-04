using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonoStateMachine
{
    [Serializable]
    public class GraphData
    {
        public EntryPoint EntryPoint => _entryPoint == null || _entryPoint.Count == 0 ? null : _entryPoint[0];

        public IReadOnlyList<StateData> States => _states;

        public IReadOnlyList<ConditionData> Conditions => _conditions;

        public IReadOnlyList<Link> Links => _links;

        public Vector2 CanvasOffset => _canvasOffset;

        [SerializeField]
        private List<EntryPoint> _entryPoint;
        [SerializeField]
        private List<StateData> _states;
        [SerializeField]
        private List<ConditionData> _conditions;
        [SerializeField]
        private List<Link> _links;
        [SerializeField]
        private Vector2 _canvasOffset;

        public GraphData()
        {
            _entryPoint = new List<EntryPoint>();
            _states = new List<StateData>();
            _conditions = new List<ConditionData>();
            _links = new List<Link>();
        }

        public GraphData(EntryPoint entryPoint, IEnumerable<StateData> states, IEnumerable<ConditionData> conditions, IEnumerable<Link> links, Vector2 canvasOffset)
        {
            _entryPoint = new List<EntryPoint>() { entryPoint };
            _states = new List<StateData>(states);
            _conditions = new List<ConditionData>(conditions);
            _links = new List<Link>(links);
            _canvasOffset = canvasOffset;
        }

        public StateData FindNextState(BaseData previousNode)
        {
            Link link = _links.Find(link => link.BaseGuid == previousNode.GUID);
            if(link == null)
                return null;

            return _states.Find(state => state.GUID == link.TargetGuid);
        }

        public List<ConditionData> FindNextConditions(StateData state)
        {
            return _links
                .Where(link => link.BaseGuid == state.GUID)
                .Select(link => _conditions
                    .First(condition => condition.GUID == link.TargetGuid))
                .ToList();
        }

        public override int GetHashCode()
        {
            return GetHashCode(_states) ^ GetHashCode(_conditions) ^ GetHashCode(_links) ^ (_entryPoint?.GetHashCode()).GetValueOrDefault() ^ _canvasOffset.GetHashCode();
        }

        private int GetHashCode<T>(List<T> list)
        {
            int res = 0x2D2816FE;
            foreach (var item in list)
                res = res * 31 + (item == null ? 0 : item.GetHashCode());
            
            return res;
        }
    }
}