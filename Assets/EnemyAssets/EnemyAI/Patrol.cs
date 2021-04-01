using UnityEngine;

class Patrol : BTNode
{

    private BehaviourController controller;
    
    public Patrol(BehaviourTree T) : base(T)
    {
        controller = Tree.gameObject.GetComponent<BehaviourController>();

    }


    public override Result Execute()
    {
        if (!controller.isHurt || !controller.isDead)
        {
            controller.isMoving = true;
            controller.groundDetected = Physics2D.Raycast(controller.groundCheck.position, Vector2.down, controller.groundCheckDistance, controller.whatIsGround);
            bool wallDetectedBot = Physics2D.Raycast(controller.wallCheckBot.position, Tree.gameObject.transform.right, controller.wallCheckDistance, controller.whatIsGround);
            bool wallDetectedTop = Physics2D.Raycast(controller.wallCheckTop.position, Tree.gameObject.transform.right, controller.wallCheckDistance, controller.whatIsGround);
            controller.wallDetected = wallDetectedBot || wallDetectedTop;


            if ((!controller.groundDetected || controller.wallDetected) && !controller.playerDetected)
            {
                controller.Flip();
            }
            else
            {
                //move
                controller.movement.Set(controller.movementSpeed * controller.facingDirection, controller.rb.velocity.y);
                controller.rb.velocity = controller.movement;
            }

            if (controller.groundDetected)
            {
                controller.groundCheckDistance = 0.2f;
            }

            controller.CheckTouchDamage();

            controller.isMoving = true;

            return Result.Running;
        }
        else
        {
            return Result.Failure;
        }
    }

  
}

