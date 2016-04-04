using System;
using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class FiniteRepeater : Repeater
    {
        [NodeParam(1, 9999)] protected readonly int Repeats;

        protected uint RepeatCount;

        public FiniteRepeater(SerializableNode serialized, Node childNode) : base(serialized, childNode)
        {
            for (int i = 0; i < serialized.Params.Length; i++)
            {
                if (serialized.Params[i].FieldName == "Repeats")
                {
                    Repeats = serialized.Params[i].IntValue;
                }
            }
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
