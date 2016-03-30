namespace DerelictComputer.BTree
{
    /// <summary>
    /// Ticks child nodes until one returns success or running, then returns the result.
    /// If all child nodes return failure, returns failure.
    /// </summary>
    public class Selector : Composite
    {
        protected int SelectedChildIndex;

        public Selector(Node[] childNodes) : base(childNodes)
        {
        }

        protected override Result OnStart()
        {
            SelectedChildIndex = -1;
            return Result.Success;
        }

        protected override Result OnTick(float deltaTime)
        {
            if (SelectedChildIndex >= 0)
            {
                return ChildNodes[SelectedChildIndex].Tick(deltaTime);
            }

            for (int i = 0; i < ChildNodes.Length; i++)
            {
                var result = ChildNodes[i].Tick(deltaTime);

                if (result == Result.Failure)
                {
                    continue;
                }

                SelectedChildIndex = i;
                return result;
            }

            return Result.Failure;
        }

        protected override void OnStop()
        {
            if (SelectedChildIndex >= 0 && SelectedChildIndex < ChildNodes.Length)
            {
                ChildNodes[SelectedChildIndex].Reset();
            }
        }
    }
}