namespace DerelictComputer.BTree
{
    /// <summary>
    /// Repeats the child until interrupted
    /// </summary>
    public class Repeater : Decorator
    {
        public Repeater(Node childNode) : base(childNode)
        {
        }

        protected override Result OnTick(float deltaTime)
        {
            ChildNode.Tick(deltaTime);
            return Result.Running;
        }
    }
}
