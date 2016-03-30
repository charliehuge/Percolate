namespace DerelictComputer.BTree
{
    /// <summary>
    /// The base class for a behavior tree. Every kind of node inherits from this.
    /// </summary>
    public abstract class Node
    {
        private bool _started;

        /// <summary>
        /// Ticks the node.
        /// </summary>
        /// <param name="deltaTime">Time since last tree tick</param>
        /// <returns>Tick (or Start) result</returns>
        public Result Tick(float deltaTime)
        {
            var startResult = Start();

            if (startResult == Result.Failure)
            {
                Stop();
                return Result.Failure;
            }


            var tickResult = OnTick(deltaTime);

            //Log.AI("Ticking " + this.GetType() + ", " + tickResult);

            if (tickResult != Result.Running)
            {
                Stop();
            }

            return tickResult;
        }

        /// <summary>
        /// Resets the node (and any children under it)
        /// </summary>
        public void Reset()
        {
            Stop();
        }

        private Result Start()
        {
            if (_started)
            {
                return Result.Success;
            }

            _started = true;

            //Log.AI("Starting " + this.GetType().ToString(), LogLevel.Warning);

            return OnStart();
        }

        private void Stop()
        {
            _started = false;

            //Log.AI("Stopping " + this.GetType().ToString(), LogLevel.Warning);

            OnStop();
        }

        /// <summary>
        /// Override to do something on tick.
        /// </summary>
        /// <param name="deltaTime">Time since last tree tick</param>
        /// <returns>Tick result (default: Failure)</returns>
        protected virtual Result OnTick(float deltaTime)
        {
            return Result.Failure;
        }

        /// <summary>
        /// Override to check if this action can tick, and do any first-time setup.
        /// </summary>
        /// <returns>Start result (default: Success)</returns>
        protected virtual Result OnStart()
        {
            return Result.Success;
        }

        /// <summary>
        /// Override to clean up state after stopping the node.
        /// </summary>
        protected virtual void OnStop()
        {
            // empty
        }
    }
}
