namespace DerelictComputer.DCTree
{
    public abstract class Decorator : Node
    {
        [NodeChild] protected readonly Node ChildNode;

        protected Decorator(SerializableNode serialized, Node childNode) : base(serialized)
        {
            ChildNode = childNode;
        }
    }
}
