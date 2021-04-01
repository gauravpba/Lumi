using System;





public class FailNodeBT : BTNode
{
    public FailNodeBT(BehaviourTree T) : base(T)
    {
    }

    public override Result Execute()
    {
        return Result.Failure;
    }
}

