using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class BlackboardFloatThreshold : BlackboardDecorator
    {
        [NodeParam] protected readonly string ParamName;
        [NodeParam] protected readonly float Threshold;

        public BlackboardFloatThreshold(SerializableNode serialized, Node childNode, Blackboard blackboard) : base(serialized, childNode, blackboard)
        {
            foreach (var param in serialized.Params)
            {
                switch (param.FieldName)
                {
                    case "ParamName":
                        ParamName = param.StringValue;
                        break;
                    case "Threshold":
                        Threshold = param.FloatValue;
                        break;
                }
            }
        }

        protected override Result OnTick(double tickDspTime)
        {
            if (Blackboard == null)
            {
                return Result.Failure;
            }

            var param = Blackboard.GetParam(ParamName);

            if (param == null || param.FloatValue < Threshold)
            {
                return Result.Failure;
            }

            return ChildNode.Tick(tickDspTime);
        }
    }
}
