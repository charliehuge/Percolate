namespace DerelictComputer.BTree
{
    /// <summary>
    /// Inverts the result of the child node. Useful for inverting test actions,
    /// so we don't have to write the inverse by hand.
    /// </summary>
    public class Inverter : Decorator
    {
        public Inverter(Node childNode) : base(childNode)
        {
        }

        protected override Result OnTick(float deltaTime)
        {
            var result = ChildNode.Tick(deltaTime);

            if (result == Result.Failure)
            {
                return Result.Success;
            }

            return Result.Failure;
        }
    }
}
