using UnityEngine;

namespace DerelictComputer.BTree
{
    public class DebugPrintAction : Action
    {
        private readonly string _stringToPrint;

        public DebugPrintAction(string stringToPrint)
        {
            _stringToPrint = stringToPrint;
        }

        protected override Result OnTick(float deltaTime)
        {
            Debug.Log(_stringToPrint);
            return Result.Running;
        }
    }
}
