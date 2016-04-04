using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class Repeater : Decorator
    {
        public Repeater(SerializableNode serialized, Node childNode) : base(serialized, childNode)
        {
        }

        protected override Result OnTick(double tickDspTime)
        {
            ChildNode.Tick(tickDspTime);
            return Result.Running;
        }
    }
}
