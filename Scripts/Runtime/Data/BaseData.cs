using System;
using UnityEngine;

namespace MonoStateMachine
{
    [Serializable]
    public abstract class BaseData
    {
        [SerializeField]
        private string _guid;
        [SerializeField]
        private Vector2 _nodePosition;

        public string GUID => _guid;

        public Vector2 NodePosition => _nodePosition;

        public BaseData(string guid, Vector2 nodePosition)
        {
            _guid = guid;
            _nodePosition = nodePosition;
        }

        public override int GetHashCode()
        {
            return _guid.GetHashCode() ^ _nodePosition.GetHashCode();
        }
    }
}