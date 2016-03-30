namespace DerelictComputer.BTree
{
    /// <summary>
    /// Ticks each child node in sequence and returns its result. Resets on any failure.
    /// </summary>
    public class Sequence : Composite
    {
        protected int CurrentChildIndex;

        public Sequence(Node[] childNodes) : base(childNodes)
        {
        }

        protected override Result OnStart()
        {
            CurrentChildIndex = 0;
            return Result.Success;
        }

        protected override Result OnTick(float deltaTime)
        {
            var result = ChildNodes[CurrentChildIndex].Tick(deltaTime);

            if (result != Result.Success)
            {
                return result;
            }

            CurrentChildIndex++;

            if (CurrentChildIndex < ChildNodes.Length)
            {
                return Result.Running;
            }

            return Result.Success;
        }

        protected override void OnStop()
        {
            if (CurrentChildIndex >= 0 && CurrentChildIndex < ChildNodes.Length)
            {
                ChildNodes[CurrentChildIndex].Reset();
            }
        }
    }
}