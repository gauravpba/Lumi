using UnityEngine;
using System.Collections;

public class RepeaterNode : DecoratorNode
{


    public RepeaterNode(BehaviourTree T, BTNode child) : base(T, child)
    {
    }

    public override Result Execute()
    {
        Debug.Log("Child returned: " + child.Execute());
        return Result.Running;
    }
}

