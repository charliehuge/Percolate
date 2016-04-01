using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class PlayNote : Leaf
    {
        protected readonly Instrument TargetInstrument;
        protected readonly int Note;

        public PlayNote(Instrument targetInstrument, int note)
        {
            TargetInstrument = targetInstrument;
            Note = note;
        }

        protected override Result OnTick(double tickDspTime)
        {
            TargetInstrument.PlayNote(tickDspTime, Note);
            return Result.Success;
        }
    }
}
