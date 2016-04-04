using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class BlackboardDecorator : Decorator
    {
        protected readonly Blackboard Blackboard;

        public BlackboardDecorator(SerializableNode serialized, Node childNode, Blackboard blackboard) : base(serialized, childNode)
        {
            Blackboard = blackboard;
        }
    }
}
