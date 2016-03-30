namespace DerelictComputer.BTree
{
    /// <summary>
    /// Executes child node until timer runs out, then returns the specified end result
    /// </summary>
    public class Timer : Decorator
    {
        private readonly float _timeToEnd;
        private readonly Result _endResult;

        private float _timeElapsed;

        public Timer(Node childNode, float timeToEnd, Result endResult) 
            : base(childNode)
        {
            _timeToEnd = timeToEnd;
            _endResult = endResult;
        }

        protected override Result OnStart()
        {
            _timeElapsed = 0f;
            return Result.Success;
        }

        protected override Result OnTick(float deltaTime)
        {
            if (_timeElapsed >= _timeToEnd)
            {
                return _endResult;
            }

            _timeElapsed += deltaTime;
            return ChildNode.Tick(deltaTime);
        }
    }
}
