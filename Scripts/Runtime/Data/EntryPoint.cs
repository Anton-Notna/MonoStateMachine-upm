using System;
using UnityEngine;

namespace MonoStateMachine
{
    [Serializable]
    public class EntryPoint : BaseData
    {
        public EntryPoint(string guid, Vector2 nodePosition) : base(guid, nodePosition)
        {

        }
    }
}