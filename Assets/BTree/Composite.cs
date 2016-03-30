namespace DerelictComputer.BTree
{
    public abstract class Composite : Node
    {
        protected readonly Node[] ChildNodes;

        protected Composite(Node[] childNodes)
        {
            ChildNodes = childNodes;
        }
    }
}
