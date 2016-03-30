namespace DerelictComputer.BTree
{
    /// <summary>
    /// Ticks the child and always returns Success.
    /// </summary>
    public class Succeeder : Decorator
    {
        public Succeeder(Node childNode) : base(childNode)
        {
        }

        protected override Result OnTick(float deltaTime)
        {
            ChildNode.Tick(deltaTime);
            return Result.Success;
        }
    }
}
