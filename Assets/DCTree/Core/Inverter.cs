using System;
using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class Inverter : Decorator
    {
        public Inverter(Node childNode) : base(childNode)
        {
        }

        protected override Result OnTick(double tickDspTime)
        {
            var result = ChildNode.Tick(tickDspTime);

            switch (result)
            {
                case Result.Success:
                    return Result.Failure;
                case Result.Failure:
                    return Result.Success;
                default:
                    return result;
            }
        }
    }
}
