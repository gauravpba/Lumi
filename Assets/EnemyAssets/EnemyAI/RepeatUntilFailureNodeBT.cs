using UnityEngine;
using System.Collections;

class RepeatUntilFailureNodeBT : DecoratorNode
{
    public RepeatUntilFailureNodeBT(BehaviourTree T, BTNode c) : base(T, c)
    {
    }

    public override Result Execute()
    {
        Result result = child.Execute();
        Debug.Log("Child returned: " + result);
        if (result == Result.Failure)
            return Result.Failure;
        else
            return Result.Running ;
    }
}

