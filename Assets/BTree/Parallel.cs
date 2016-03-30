using System.Collections.Generic;

namespace DerelictComputer.BTree
{
    /// <summary>
    /// Executes all children in order each tick until one of them completes
    /// </summary>
    public class Parallel : Composite
    {
        public Parallel(Node[] childNodes) : base(childNodes)
        {
        }

        protected override Result OnTick(float deltaTime)
        {
            for (int i = 0; i < ChildNodes.Length; i++)
            {
                var result = ChildNodes[i].Tick(deltaTime);
                
                if (result != Result.Running)
                {
                    return result;
                }
            }

            return Result.Running;
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
