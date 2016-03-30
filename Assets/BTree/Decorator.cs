namespace DerelictComputer.BTree
{
    public abstract class Decorator : Node
    {
        protected readonly Node ChildNode;

        protected Decorator(Node childNode)
        {
            ChildNode = childNode;
        }
    }
}