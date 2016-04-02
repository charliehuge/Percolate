namespace DerelictComputer.DCTree
{
    public abstract class Composite : Node
    {
        [NodeChild] protected readonly Node[] ChildNodes;

        protected Composite(Node[] childNodes)
        {
            ChildNodes = childNodes;
        }
    }
}
