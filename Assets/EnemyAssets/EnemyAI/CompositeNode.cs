using System;
using System.Collections.Generic;

public class CompositeNode : BTNode
{

    public List<BTNode> Children { get; set; }

    public CompositeNode(BehaviourTree T, BTNode[] nodes) : base(T)
    {
        Children = new List<BTNode>(nodes);
    }
}
