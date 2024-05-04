using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonoStateMachine
{
    public class StateMachine : MonoBehaviour
    {
        [SerializeField]
        private GraphData _data;
        [SerializeField, HideInInspector]
        private int _dataHashCode;

        private Dictionary<State, List<(Condition condition, State next)>> _graph;
        private State _current;

        public GraphData Load() => _data;

        public int DataHashCode => _dataHashCode;

        public void Save(GraphData data)
        {
            _data = data;
            _dataHashCode = _data.GetHashCode();
            if (Application.isPlaying)
                Init();
        }

        public bool IsCurrentState(State state) => _current == state;

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            Clear();
        }

        private void Update()
        {
            foreach ((Condition condition, State next) condition in _graph[_current])
            {
                if (condition.condition.Check())
                {
                    ChangeState(condition.next);
                    break;
                }
            }

            _current.Execute();
        }

        private void Init()
        {
            Clear();

            if (_data == null)
                throw new NullReferenceException("Empty GraphData");

            if (_data.EntryPoint == null)
                throw new NullReferenceException("Empty EntryPoint in GraphData");

            StateData entryStateData = _data.FindNextState(_data.EntryPoint);
            if (entryStateData == null)
                throw new NullReferenceException("EntryPoint don't connected to any State in GraphData");

            State entry = entryStateData.State;
            _graph = new Dictionary<State, List<(Condition condition, State next)>>();

            foreach (StateData stateData in _data.States)
            {
                State state = stateData.State;
                if (state == null)
                    throw new NullReferenceException($"Empty state selected in \"{stateData.Name}\" State");

                List<(Condition condition, State next)> conditions = new List<(Condition condition, State next)>();

                foreach (ConditionData conditionData in _data.FindNextConditions(stateData))
                {
                    Condition condition = conditionData.Condition;
                    if (condition == null)
                        throw new NullReferenceException($"Empty condition selected in \"{conditionData.Name}\" Condition");

                    if (condition.Deserialize() == false)
                        throw new InvalidOperationException($"Failed to deserialize \"{conditionData.Name}\" Condition");

                    StateData next = _data.FindNextState(conditionData);
                    if (next == null)
                        throw new NullReferenceException($"Condition \"{conditionData.Name}\" doesnt connected to any State");

                    if(next.State == null)
                        throw new NullReferenceException($"Empty state selected in \"{next.Name}\" State");

                    conditions.Add((condition, next.State));
                }

                _graph.Add(state, conditions);
            }

            ChangeState(entry);
        }

        private void ChangeState(State entry)
        {
            if (_current != null)
                _current.Exit();

            _current = entry;
            _current.Enter();
        }

        private void Clear()
        {
            if (_current != null)
                _current.Exit();

            _graph = null;
            _current = null;
        }
    }
}