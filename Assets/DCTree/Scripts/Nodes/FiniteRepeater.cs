using System;
using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class FiniteRepeater : Repeater
    {
        [NodeParam(1, 9999)] protected readonly uint Repeats;

        protected uint RepeatCount;

        public FiniteRepeater(Node childNode, uint repeats) : base(childNode)
        {
            Repeats = repeats;
        }

        protected override Result OnStart()
        {
            RepeatCount = 0;
            return Result.Success;
        }

        protected override Result OnTick(double tickDspTime)
        {
            var result = ChildNode.Tick(tickDspTime);

            switch (result)
            {
                case Result.Success:
                case Result.Failure:
                    if (++RepeatCount >= Repeats)
                    {
                        return result;
                    }
                    break;
            }

            return Result.Running;
        }
    }
}
