namespace DerelictComputer.DCTree
{
    public abstract class Composite : Node
    {
        [NodeChild] protected readonly Node[] ChildNodes;

        protected Composite(SerializableNode serialized, Node[] childNodes) : base(serialized)
        {
            ChildNodes = childNodes;
        }
    }
}
