using UnityEngine;

namespace DerelictComputer.BTree
{
    /// <summary>
    /// Randomly succeeds or fails on start
    /// </summary>
    public class RandomSucceeder : Decorator
    {
        private readonly float _chance;

        public RandomSucceeder(Node childNode, float chance) : base(childNode)
        {
            _chance = chance;
        }

        protected override Result OnStart()
        {
            return Random.value < _chance ? Result.Success : Result.Failure;
        }

        protected override Result OnTick(float deltaTime)
        {
            return ChildNode.Tick(deltaTime);
        }
    }
}
