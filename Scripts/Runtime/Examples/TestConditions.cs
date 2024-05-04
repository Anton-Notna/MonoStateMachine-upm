using UnityEngine;

namespace MonoStateMachine
{
    public class TestConditions : MonoBehaviour
    {
        [SerializeField]
        private bool _bool;
        [SerializeField]
        private float _float;

        [Condition]
        public bool IsBool() => _bool;

        [Condition]
        public bool IsFloatPositiove() => _float > 0f;
    }
}