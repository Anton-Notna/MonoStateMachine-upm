using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace MonoStateMachine.EditorDeprecated
{
    public static class NodeExtentions
    {
        public static void AddPort(this Node node, string name, Color color, Direction direction, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = GeneratePort(node, direction, capacity);
            port.portName = name;
            port.portColor = color;

            if (direction == Direction.Output)
                node.outputContainer.Add(port);
            else
                node.inputContainer.Add(port);

            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        public static Port GeneratePort(this Node node, Direction direction, Port.Capacity capacity)
        {
            return node.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(float));
        }
    }
}