using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class ReleaseNote : Leaf
    {
        protected readonly Instrument TargetInstrument;

        public ReleaseNote(SerializableNode serialized, Instrument targetInstrument) : base(serialized)
        {
            TargetInstrument = targetInstrument;
        }

        protected override Result OnTick(double tickDspTime)
        {
            TargetInstrument.ReleaseNote(tickDspTime);
            return Result.Success;
        }
    }
}
