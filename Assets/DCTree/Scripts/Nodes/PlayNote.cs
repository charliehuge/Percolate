using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class PlayNote : Leaf
    {
        protected readonly Instrument TargetInstrument;
        [NodeParam(0, 127)] protected readonly int Note;

        public PlayNote(SerializableNode serialized, Instrument targetInstrument) : base(serialized)
        {
            TargetInstrument = targetInstrument;

            for (int i = 0; i < serialized.Params.Length; i++)
            {
                if (serialized.Params[i].FieldName == "Note")
                {
                    Note = serialized.Params[i].IntValue;
                }
            }
        }

        protected override Result OnTick(double tickDspTime)
        {
            TargetInstrument.PlayNote(tickDspTime, Note);
            return Result.Success;
        }
    }
}
