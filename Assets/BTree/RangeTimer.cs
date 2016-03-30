using UnityEngine;

namespace DerelictComputer.BTree
{
    /// <summary>
    /// Executes child node until timer runs out, then returns the specified end result
    /// </summary>
    public class RangeTimer : Decorator
    {
        private readonly float _minTime;
        private readonly float _maxTime;
        private readonly Result _endResult;

        private float _timeToWait;
        private float _timeElapsed;

        public RangeTimer(Node childNode, float minTime, float maxTime, Result endResult) : base(childNode)
        {
            _minTime = minTime;
            _maxTime = maxTime;
            _endResult = endResult;
        }

        protected override Result OnStart()
        {
            _timeToWait = Random.Range(_minTime, _maxTime);
            _timeElapsed = 0f;
            return Result.Success;
        }

        protected override Result OnTick(float deltaTime)
        {
            if (_timeElapsed >= _timeToWait)
            {
                return _endResult;
            }

            _timeElapsed += deltaTime;
            return ChildNode.Tick(deltaTime);
        }
    }
}
