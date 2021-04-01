using System;
using System.Collections.Generic;

public class SequencerNode : CompositeNode
{
    private int currentNode = 0;

    public SequencerNode(BehaviourTree T, BTNode[] children) : base(T, children)
    {

    }

    public override Result Execute()
    {
        

        if(currentNode < Children.Count)
        {
            Result result = Children[currentNode].Execute();

            if (result == Result.Running) 
            {
                return Result.Running;
            }
            else if(result == Result.Failure)
            {
                currentNode = 0;
                return Result.Failure;
            }
            else
            {
                currentNode++;
                if (currentNode < Children.Count)
                    return Result.Running;
                else
                {
                    currentNode = 0;
                    return Result.Success;
                }

            }
        }

        return Result.Success;

    }


}

