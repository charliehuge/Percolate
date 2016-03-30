
namespace DerelictComputer.BTree
{
    public abstract class Interrupt : Decorator
    {
        protected Interrupt(Node childNode) : base(childNode)
        {
        }

        protected abstract bool ShouldInterrupt();

        protected override Result OnTick(float deltaTime)
        {
            if (ShouldInterrupt())
            {
                ChildNode.Reset();
                return Result.Failure;
            }

            return ChildNode.Tick(deltaTime);
        }
    }
}
