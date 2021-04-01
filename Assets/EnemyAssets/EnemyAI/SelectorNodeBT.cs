using System;
using System.Collections.Generic;


public class SelectorNodeBT : CompositeNode
{

    private int currentNode = 0;
    public SelectorNodeBT(BehaviourTree T, BTNode[] children) : base(T, children)
    {

    }

    public override Result Execute()
    {
        if (currentNode < Children.Count)
        {
            Result result = Children[currentNode].Execute();

            if (result == Result.Running)
            {
                return Result.Running;
            }
            else if (result == Result.Success)
            {
                currentNode = 0;
                return Result.Success;
            }
            else
            {
                currentNode++;
                if (currentNode < Children.Count)
                    return Result.Running;
                else
                {
                    currentNode = 0;
                    return Result.Failure;
                }

            }
        }

        return Result.Failure;

    }
}

