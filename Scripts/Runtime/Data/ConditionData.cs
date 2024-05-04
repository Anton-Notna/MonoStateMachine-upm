using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonoStateMachine
{
    [Serializable]
    public class ConditionData : NamedData
    {
        [SerializeField]
        private Condition _condition;
        [SerializeField]
        private GameObject _root;

        public Condition Condition => _condition;

        public ConditionData(ConditionData previous, string name, string guid, Vector2 nodePosition) : base(name, guid, nodePosition)
        {
            _condition = previous._condition;
            _root = previous._root;
        }

        public ConditionData(GameObject root, string name, string guid, Vector2 nodePosition) : base(name, guid, nodePosition)
        {
            _root = root;
        }

        public IReadOnlyList<Condition> GetPossibleConditions() => Condition.New(_root);

        public ConditionData SelectCondition(Condition condition)
        {
            return new ConditionData(this, Name, GUID, NodePosition)
            {
                _condition = condition,
            };
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (_condition?.GetHashCode()).GetValueOrDefault() ^ (_root?.GetHashCode()).GetValueOrDefault();
        }
    }
}