using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class Repeater : Decorator
    {
        public Repeater(Node childNode) : base(childNode)
        {
        }

        protected override Result OnTick(double tickDspTime)
        {
            ChildNode.Tick(tickDspTime);
            return Result.Running;
        }
    }
}
