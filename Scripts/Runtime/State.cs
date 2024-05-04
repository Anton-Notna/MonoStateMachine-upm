using System.Text;
using UnityEngine;

namespace MonoStateMachine
{
    public abstract class State : MonoBehaviour 
    {
        [SerializeField]
        private string _subName;

        public virtual void Enter() { }

        public virtual void Execute() { }

        public virtual void Exit() { }

        public override string ToString()
        {
            return GetFullPath();
        }

        public string GetFullPath()
        {
            StringBuilder path = new StringBuilder();
            path.FillByHierarchy(transform);
            path.Append(GetType().Name);

            if(string.IsNullOrEmpty(_subName) == false)
                path.Append($" ({_subName})");

            return path.ToString();
        }
    }
}