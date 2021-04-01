using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree : MonoBehaviour
{

    private BTNode mRoot;
    private bool startedBehaviour;
    
    private Coroutine behaviour;

    public BTNode Root { get { return mRoot; } }

    public Dictionary<string, object> Blackboard { get; set; }

    public bool isSequencerObject;


    // Start is called before the first frame update
    void Start()
    {
        Blackboard = new Dictionary<string, object>();
        

        Blackboard.Add("WorldBounds", new Rect(40,12,54,18));
        
        startedBehaviour = false;
        if (isSequencerObject)
        {
            mRoot = new RepeaterNode(this,
             new SequencerNode(this,
             new BTNode[] { new Patrol(this) }));
        }
        else
        {
            mRoot = new RepeaterNode(this,
             new SequencerNode(this,
             new BTNode[] { new RandomWalkBT(this)}));
        }

        /*if(isSequencerObject)
            mRoot = new RepeatUntilFailureNodeBT(this, new SequencerNode(this, new BTNode[] { new RandomWalkBT(this), new RandomWalkBT(this), new FailNodeBT(this) }));
        else
            mRoot = new SelectorNodeBT(this, new BTNode[] { new RandomWalkBT(this), new FailNodeBT(this) , new RandomWalkBT(this), new RandomWalkBT(this), new FailNodeBT(this) });
            */
    }

    // Update is called once per frame
    void Update()
    {

        if (!startedBehaviour)
        {
            behaviour = StartCoroutine(RunBehaviour());
            startedBehaviour = true;
        }

        
    }

    IEnumerator RunBehaviour()
    {
        BTNode.Result result = Root.Execute();

        while(result == BTNode.Result.Running)
        {
            yield return null;
            result = Root.Execute();
        }

    }
}
