using System;

public class DecoratorNode : BTNode
{
    public BTNode child { get; set; }

    public DecoratorNode(BehaviourTree T, BTNode c) : base(T)
    {
        child = c;
    }
}

