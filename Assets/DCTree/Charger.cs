using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class Charger : Decorator
    {
        protected uint ChargesToExecuteChild;

        private uint _charges;

        public Charger(Node childNode, uint chargesToExecuteChild) : base(childNode)
        {
            ChargesToExecuteChild = chargesToExecuteChild;
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
