using UnityEngine;

namespace MonoStateMachine
{
    public class DebugLogState : State
    {
        public override void Enter()
        {
            Debug.Log($"Enter {ToString()}");
        }

        public override void Execute()
        {
            Debug.Log($"Execute {ToString()}");
        }

        public override void Exit()
        {
            Debug.Log($"Exit {ToString()}");
        }
    }
}