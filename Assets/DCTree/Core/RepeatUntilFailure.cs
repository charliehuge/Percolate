using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class RepeatUntilFailure : Repeater
    {
        public RepeatUntilFailure(Node childNode) : base(childNode)
        {
        }

        protected override Result OnTick(double tickDspTime)
        {
            var result = ChildNode.Tick(tickDspTime);

            if (result == Result.Failure)
            {
                return Result.Success;
            }

            return Result.Running;
        }
    }
}
