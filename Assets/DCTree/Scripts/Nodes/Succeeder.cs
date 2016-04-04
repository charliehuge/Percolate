using System;
using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class Succeeder : Decorator
    {
        public Succeeder(SerializableNode serialized, Node childNode) : base(serialized, childNode)
        {
        }

        protected override Result OnTick(double tickDspTime)
        {
            var result = ChildNode.Tick(tickDspTime);

            switch (result)
            {
                case Result.Success:
                case Result.Failure:
                    return Result.Success;
                default:
                    return result;
            }
        }
    }
}
