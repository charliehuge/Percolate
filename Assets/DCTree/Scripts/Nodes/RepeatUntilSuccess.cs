using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class RepeatUntilSuccess : Repeater
    {
        public RepeatUntilSuccess(Node childNode) : base(childNode)
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
