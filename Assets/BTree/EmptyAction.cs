namespace DerelictComputer.BTree
{
    public class EmptyAction : Action
    {
        protected override Result OnTick(float deltaTime)
        {
            return Result.Success;
        }
    }
}
