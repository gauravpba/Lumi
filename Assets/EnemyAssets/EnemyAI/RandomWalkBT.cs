using UnityEngine;
using System.Collections;

public class RandomWalkBT : BTNode
{

    protected Vector3 NextDestination { get; set; }

    public float moveSpeed = 7.5f;

    public RandomWalkBT(BehaviourTree T) : base(T)
    {
        NextDestination = Vector3.zero;
        FindNextDestination();
    }



    public bool FindNextDestination()
    {
        object o;
        bool found = false;

        found = Tree.Blackboard.TryGetValue("WorldBounds", out o);
        if(found)
        {
            Rect bounds = (Rect)o;

            float x = bounds.width * Random.Range(0.0f,2.0f) - bounds.width/2;
            float y = bounds.height * Random.Range(0.0f, 2.0f) - bounds.height/2;

            NextDestination = new Vector3(x, y, NextDestination.z);
        }


        return found;
    }

    public override Result Execute()
    {
        if(Tree.gameObject.transform.position == NextDestination)
        {
            if(!FindNextDestination())
            {
                return Result.Failure;
            }

            return Result.Success;
        }
        else
        {
            if(NextDestination.x < Tree.gameObject.transform.position.x && !Tree.gameObject.GetComponent<SpriteRenderer>().flipX)
            {
                Tree.gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            else if(NextDestination.x > Tree.gameObject.transform.position.x && Tree.gameObject.GetComponent<SpriteRenderer>().flipX)
            {
                Tree.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }

            Tree.gameObject.transform.position = Vector3.MoveTowards(Tree.gameObject.transform.position, NextDestination, Time.deltaTime * moveSpeed);
            return Result.Running;
        }

    }
}

