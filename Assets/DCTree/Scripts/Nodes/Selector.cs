using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class Selector : Composite
    {
        protected int SelectedChildIndex;

        public Selector(SerializableNode serialized, Node[] childNodes) : base(serialized, childNodes)
        {
        }

        protected override Result OnStart()
        {
            SelectedChildIndex = -1;
            return Result.Success;
        }

        protected override Result OnTick(double tickDspTime)
        {
            if (SelectedChildIndex >= 0)
            {
                return ChildNodes[SelectedChildIndex].Tick(tickDspTime);
            }

            for (int i = 0; i < ChildNodes.Length; i++)
            {
                var result = ChildNodes[i].Tick(tickDspTime);

                if (result == Result.Failure)
                {
                    continue;
                }

                SelectedChildIndex = i;
                return result;
            }

            return Result.Failure;
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
