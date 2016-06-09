using UnityEngine;
using System.Collections;
using DerelictComputer.DCTree;

public class BlackboardUI : MonoBehaviour
{
    private static BlackboardUI _instance;

    [SerializeField] private Blackboard _blackboard;

    public static BlackboardUI Instance
    {
        get { return _instance; }
    }

    public void UpdateBlackboard(string paramName, float floatParam)
    {
        var param = _blackboard.GetParam(paramName);

        if (param != null)
        {
            param.FloatValue = floatParam;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
}
