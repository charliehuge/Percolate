using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class NullNode : Leaf
    {
        public NullNode(SerializableNode serialized) : base(serialized)
        {
        }

        protected override Result OnTick(double tickDspTime)
        {
            return Result.Success;
        }
    }
}
