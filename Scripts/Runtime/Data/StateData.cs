using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonoStateMachine
{
    [Serializable]
    public class StateData : NamedData
    {
        [SerializeField]
        private State _state;
        [SerializeField]
        private GameObject _root;

        public State State => _state;

        public StateData(StateData previous, string name, string guid, Vector2 nodePosition) : base(name, guid, nodePosition)
        {
            _root = previous._root;
            _state = previous._state;
        }

        public StateData(GameObject root, string name, string guid, Vector2 nodePosition) : base(name, guid, nodePosition) 
        {
            _root = root;
        }

        public IReadOnlyList<State> GetPossibleStates()
        {
            return _root.GetComponentsInChildren<State>();
        }

        public StateData SelectState(State state) 
        {
            return new StateData(_root, Name, GUID, NodePosition)
            {
                _state = state,
            };
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (_state?.GetHashCode()).GetValueOrDefault() ^ (_root?.GetHashCode()).GetValueOrDefault();
        }
    }
}