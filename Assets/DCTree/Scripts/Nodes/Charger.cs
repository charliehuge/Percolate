using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class Charger : Decorator
    {
        [NodeParam(1, 9999)] protected readonly int ChargesToExecuteChild;

        private uint _charges;

        public Charger(SerializableNode serialized, Node childNode) : base(serialized, childNode)
        {
            for (int i = 0; i < serialized.Params.Length; i++)
            {
                if (serialized.Params[i].FieldName == "ChargesToExecuteChild")
                {
                    ChargesToExecuteChild = serialized.Params[i].IntValue;
                }
            }
        }

        protected override Result OnStart()
        {
            _charges = 0;
            return Result.Success;
        }

        protected override Result OnTick(double tickDspTime)
        {
            if (++_charges >= ChargesToExecuteChild)
            {
                return ChildNode.Tick(tickDspTime);
            }

            return Result.Running;
        }
    }
}
