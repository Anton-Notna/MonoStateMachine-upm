using System;
using UnityEngine;

namespace MonoStateMachine
{
    [Serializable]
    public class Link
    {
        public string BaseGuid => _baseGuid;

        public string TargetGuid => _targetGuid;

        [SerializeField]
        private string _baseGuid;
        [SerializeField]
        private string _targetGuid;

        public Link(string baseGuid, string targetGuid)
        {
            _baseGuid = baseGuid;
            _targetGuid = targetGuid;
        }

        public bool Contains(string guid) => _baseGuid.Equals(guid) || _targetGuid.Equals(guid);

        public override int GetHashCode() => _baseGuid.GetHashCode() ^ _targetGuid.GetHashCode();
    }
}