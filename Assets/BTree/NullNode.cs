namespace DerelictComputer.BTree
{
    /// <summary>
    /// For when you want a node that doesn't do anything.
    /// </summary>
    public class NullNode : Node
    {
        private readonly Result _result;

        public NullNode(Result result)
        {
            _result = result;
        }

        protected override Result OnTick(float deltaTime)
        {
            return _result;
        }
    }
}
