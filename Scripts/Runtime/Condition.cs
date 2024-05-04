using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MonoStateMachine
{
    [Serializable]
    public class Condition
    {
        [SerializeField]
        private MonoBehaviour _conditionHandler;
        [SerializeField]
        private string _methodName;

        private Func<bool> _method;

        private Condition(MonoBehaviour handler, string method)
        {
            _conditionHandler = handler;
            _methodName = method;
        }

        public static List<Condition> New(GameObject root)
        {
            return FindMethods(root).Select(pair => new Condition(pair.Item1, pair.Item2.Name)).ToList();
        }

        public bool Deserialize()
        {
            if (_conditionHandler == null)
                return false;

            MethodInfo method = GetConditions(_conditionHandler).Where(method => method.Name == _methodName).FirstOrDefault();
            if (method == null)
                return false;

            _method = (Func<bool>)method.CreateDelegate(typeof(Func<bool>), _conditionHandler);
            return (_method != null);
        }

        public bool Check()
        {
            if (_method == null)
                throw new InvalidOperationException("Cannot check before Deserialize()");

            return _method.Invoke();
        }

        public override string ToString()
        {
            if (_conditionHandler == null)
                return "Lost handler";

            if (string.IsNullOrEmpty(_methodName))
                return "Empty methodName";

            StringBuilder path = new StringBuilder();
            path.FillByHierarchy(_conditionHandler.transform);
            path.Append($"{_conditionHandler.GetType().Name}/{_methodName}");

            return path.ToString();
        }

        public override bool Equals(object other)
        {
            if(other == null)
                return false;

            Condition condition = other as Condition;
            if (condition == null)
                return false;

            return condition._conditionHandler == _conditionHandler && condition._methodName == _methodName;
        }

        public override int GetHashCode()
        {
            return ( _methodName == null ? 1 : _methodName.GetHashCode()) ^ (_conditionHandler?.GetHashCode()).GetValueOrDefault();
        }

        private static List<(MonoBehaviour, MethodInfo)> FindMethods(GameObject gameObject)
        {
            List<(MonoBehaviour, MethodInfo)> result = new List<(MonoBehaviour, MethodInfo)>();

            MonoBehaviour[] components = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
            for (int i = 0; i < components.Length; i++)
            {
                MonoBehaviour component = components[i];
                foreach (MethodInfo method in GetConditions(component))
                    result.Add((component, method));
            }
            return result;
        }

        private static IEnumerable<MethodInfo> GetConditions(MonoBehaviour component)
        {
            Type componentType = component.GetType();
            return componentType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m =>
                        m.GetCustomAttributes(typeof(ConditionAttribute), true).Length > 0 &&
                        m.GetParameters().Length == 0 &&
                        m.ReturnType == typeof(bool));
        }
    }
}