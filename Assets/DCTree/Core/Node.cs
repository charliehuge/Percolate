namespace DerelictComputer.DCTree
{
    public abstract class Node
    {
        private bool _started;

        public Result Tick(double tickDspTime)
        {
            var startResult = Start();

            if (startResult == Result.Failure)
            {
                Stop();
                return Result.Failure;
            }

            var tickResult = OnTick(tickDspTime);

            if (tickResult != Result.Running)
            {
                Stop();
            }

            return tickResult;
        }

        public void Reset()
        {
            Stop();
        }

        protected virtual Result OnStart()
        {
            return Result.Success;
        }

        protected virtual Result OnTick(double tickDspTime)
        {
            return Result.Failure;
        }

        protected virtual void OnStop()
        {
        }

        private Result Start()
        {
            if (_started)
            {
                return Result.Success;
            }

            _started = true;

            return OnStart();
        }

        private void Stop()
        {
            _started = false;

            OnStop();
        }
    }
}
