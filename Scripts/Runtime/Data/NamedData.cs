using System;
using UnityEngine;

namespace MonoStateMachine
{
    [Serializable]
    public abstract class NamedData : BaseData
    {
        [SerializeField]
        private string _name;

        public string Name => _name;

        public NamedData(string name, string guid, Vector2 nodePosition) : base(guid, nodePosition)
        {
            _name = name;
        }
    }
}