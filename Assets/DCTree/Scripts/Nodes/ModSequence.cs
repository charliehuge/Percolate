using System;
using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class ModSequence : Composite
    {
        [NodeParam(0, 32)] protected readonly int ModTicks;

        protected int _currentTick;

        public ModSequence(SerializableNode serialized, Node[] childNodes) : base(serialized, childNodes)
        {
            for (int i = 0; i < serialized.Params.Length; i++)
            {
                if (serialized.Params[i].FieldName == "ModTicks")
                {
                    ModTicks = serialized.Params[i].IntValue;
                }
            }
        }

        protected override Result OnStart()
        {
            _currentTick = 0;
            return Result.Success;
        }

        protected override Result OnTick(double tickDspTime)
        {
            Result result;

            if (_currentTick < ChildNodes.Length)
            {
                result = ChildNodes[_currentTick].Tick(tickDspTime);
            }
            else
            {
                result = Result.Failure;
            }

            _currentTick = (_currentTick + 1)%ModTicks;

            return result != Result.Failure ? Result.Running : Result.Success;
        }
    }
}
