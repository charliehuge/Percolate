namespace DerelictComputer.DCTree
{
    public abstract class Decorator : Node
    {
        [NodeChild] protected readonly Node ChildNode;

        protected Decorator(Node childNode)
        {
            ChildNode = childNode;
        }
    }
}
