using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTNode
{

    public enum Result {Running, Success,Failure};


    public BehaviourTree Tree { get; set; }

    public BTNode(BehaviourTree T)
    {
        Tree = T;
    }

    public virtual Result Execute()
    {
        return Result.Failure;
    }
}
