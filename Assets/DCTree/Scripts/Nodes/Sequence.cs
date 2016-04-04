using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class Sequence : Composite
    {
        protected int CurrentChildIndex;

        public Sequence(SerializableNode serialized, Node[] childNodes) : base(serialized, childNodes)
        {
        }

        protected override Result OnStart()
        {
            CurrentChildIndex = 0;
            return Result.Success;
        }

        protected override Result OnTick(double tickDspTime)
        {
            var result = ChildNodes[CurrentChildIndex].Tick(tickDspTime);

            if (result != Result.Success)
            {
                return result;
            }

            if (++CurrentChildIndex < ChildNodes.Length)
            {
                return Result.Running;
            }

            return Result.Success;
        }

        protected override void OnStop()
        {
            for (int i = 0; i < ChildNodes.Length; i++)
            {
                ChildNodes[i].Reset();
            }
        }
    }
}
