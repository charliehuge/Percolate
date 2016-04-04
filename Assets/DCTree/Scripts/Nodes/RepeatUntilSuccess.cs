using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class RepeatUntilSuccess : Repeater
    {
        public RepeatUntilSuccess(SerializableNode serialized, Node childNode) : base(serialized, childNode)
        {
        }

        protected override Result OnTick(double tickDspTime)
        {
            var result = ChildNode.Tick(tickDspTime);

            if (result == Result.Success)
            {
                return Result.Success;
            }

            return Result.Running;
        }
    }
}
