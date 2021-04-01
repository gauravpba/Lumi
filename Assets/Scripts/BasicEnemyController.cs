using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BasicEnemyController : MonoBehaviour
{
    private enum State
    {
        Moving,
        Knockback,
        Charge,
        Attack,
        HopBack,
        Dead
    }

    private State currentState;

    public int 
        facingDirection, 
        damageDirection;

    public Vector2 movement;

    [SerializeField]
    public Vector2 knockBackSpeed, 
        knockBackSpeedAttack1, 
        knockBackSpeedAttack2, 
        knockBackSpeedAttack3,
        touchDamageBotLeft,
        touchDamageTopRight,
        hopBackSpeed;

    [SerializeField]
    public bool isRanged;

    public bool 
        groundDetected, 
        wallDetected,
        playerDetected,
        isCharging,
        playerInRange,
        isMoving,
        isHurt,
        isAttacking,
        isDead;

    [SerializeField]
    public float
        groundCheckDistance,
        wallCheckDistance,
        movementSpeed,
        knockBackDuration,
        hopBackDuration,
        maxHealth,
        touchDamageCoolDown,
        touchDamage,
        touchDamageWidth,
        touchDamageHeight,
        enemyVisionDistance,        
        attackRange,
        attackDamage,
        attackRadius,
        attackRate,
        hopBackRate,
        hopBackRange;

    public float
        lastTouchDamageTime,
        lastAttackDamageTime,
        knockBackStartTime,
        hopBackStartTime,
        hopBackCoolDown,
        attackCoolDown = 0;

    public float[] attackDetails = new float[2];

    [SerializeField]
    public Transform 
        groundCheck, 
        wallCheckBot,
        wallCheckTop,
        touchDamageCheck,
        attackPoint;

    [SerializeField]
    public LayerMask 
        whatIsGround,
        whatIsPlayer;

    [SerializeField]
    public GameObject hitParticle, arrowPrefab, playerObject;

    public Rigidbody2D rb;

    public Animator enemyAnim;

    public CameraShaker camShaker;

    public LevelManager manager;
    public EnemySpawner spawner;

    public float currentHealth;
    
    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<LevelManager>();
        spawner = GameObject.FindGameObjectWithTag("EnemySpawner").GetComponent<EnemySpawner>();
        camShaker = Camera.main.GetComponent<CameraShaker>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();

        if (transform.eulerAngles.y == 0)
        {
            facingDirection = 1;
        }
        else if (transform.eulerAngles.y == 180)
        {
            facingDirection = -1;
        }

        //SwitchState(State.Moving);
        isMoving = true;
        currentHealth = maxHealth;
        
       
    }
    

    private void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.Knockback:
                UpdateKnockBackState();
                break;
            case State.Charge:
                UpdateChargeState();
                break;
            case State.Attack:
                UpdateAttackState();
                break;
            case State.HopBack:
                UpdateHopState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }

        UpdateAnimations();
    }
    
    // Moving State
    private void EnterMovingState()
    {
        isMoving = true;        
    }
    private void UpdateMovingState()
    {
        isMoving = true;
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        bool wallDetectedBot = Physics2D.Raycast(wallCheckBot.position, transform.right, wallCheckDistance, whatIsGround);
        bool wallDetectedTop = Physics2D.Raycast(wallCheckTop.position, transform.right, wallCheckDistance, whatIsGround);
        wallDetected = wallDetectedBot || wallDetectedTop;

        bool playerDetectedBot = Physics2D.Raycast(wallCheckBot.position, transform.right, enemyVisionDistance, whatIsPlayer);
        bool playerDetectedTop = Physics2D.Raycast(wallCheckTop.position, transform.right, enemyVisionDistance, whatIsPlayer);
       // playerDetected = Physics2D.Raycast(touchDamageCheck.position, transform.right, enemyVisionDistance, whatIsPlayer);

        if (playerDetectedTop || playerDetectedBot)
            playerDetected = true;
        else
            playerDetected = false;
        if ( (!groundDetected || wallDetected) && !playerDetected)
        {
            Flip();
        }
        else
        {
            //move
            movement.Set(movementSpeed * facingDirection, rb.velocity.y);
            rb.velocity = movement;
        }

        if(playerDetected)
        {
            SwitchState(State.Charge);
        }
        
        if(groundDetected)
        {
            groundCheckDistance = 0.2f;
        }

        CheckTouchDamage();

    }
    private void ExitMovingState()
    {
        isMoving = false;
    }

    // KnockBack State
    private void EnterKnockBackState()
    {
        isHurt = true;
        knockBackStartTime = Time.time; 
        groundCheckDistance = 2;
        movement.Set(knockBackSpeed.x * damageDirection, knockBackSpeed.y);
        rb.velocity = movement;
        enemyAnim.SetTrigger("isHurt");
       
        StartCoroutine(camShaker.Shake(0.2f, 0.75f));
    }
    private void UpdateKnockBackState()
    {
        playerDetected = Physics2D.Raycast(touchDamageCheck.position, transform.right, enemyVisionDistance, whatIsPlayer);

        if (Time.time >= knockBackStartTime + knockBackDuration)
        {
            SwitchState(State.Moving);
        }

        if (currentHealth <= 0.0f)
        {
            SwitchState(State.Dead);
        }
    }
    private void ExitKnockBackState()
    {
        isHurt = false;
        groundCheckDistance = 0.2f;
        movement.Set(movementSpeed * facingDirection, rb.velocity.y);
        rb.velocity = movement;
    }

    //Charge State
    private void EnterChargeState()
    {
        isCharging = true;
        isMoving = true;
    }
    private void UpdateChargeState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        bool wallDetectedBot = Physics2D.Raycast(wallCheckBot.position, transform.right, wallCheckDistance, whatIsGround);
        bool wallDetectedTop = Physics2D.Raycast(wallCheckTop.position, transform.right, wallCheckDistance, whatIsGround);
        wallDetected = wallDetectedBot || wallDetectedTop;
        playerDetected = Physics2D.Raycast(touchDamageCheck.position, transform.right, enemyVisionDistance, whatIsPlayer);
        if(!playerDetected)
            SwitchState(State.Moving);
        movement.Set(movementSpeed * 3.5f * facingDirection, rb.velocity.y);
        rb.velocity = movement;


        playerInRange = Physics2D.Raycast(touchDamageCheck.position, transform.right, attackRange, whatIsPlayer);

        if(playerInRange)
        {
             SwitchState(State.Attack);
        }      
    }
    private void ExitChargeState()
    {
        isCharging = false;
        isMoving = false;
    }


    // Attack State
    private void EnterAttackState()
    {        
        rb.velocity = Vector2.zero;
        //attackCoolDown = Time.time;
    }
    private void UpdateAttackState()
    {
        if(Time.time >= attackCoolDown + attackRate)
        {
            // Attack
            isAttacking = true;
            attackCoolDown = Time.time;
             
        }
        else
        {
            isAttacking = false;
        }


        playerDetected = Physics2D.Raycast(touchDamageCheck.position, transform.right, attackRange, whatIsPlayer);

        if(!playerDetected)
        {
             SwitchState(State.Moving);
        }

        if (isRanged && Physics2D.Raycast(touchDamageCheck.position, transform.right, hopBackRange, whatIsPlayer) && hopBackCoolDown <= 0)
        {
            SwitchState(State.HopBack);
            hopBackCoolDown = hopBackRate;
        }

        if (hopBackCoolDown > 0)
            hopBackCoolDown -= Time.deltaTime;

    }
    private void ExitAttackState()
    {
        isAttacking = false;
    }

    // Dead State
    private void EnterDeadState()
    {
        // Spawn Blood? 

        isDead = true;
        gameObject.layer = 11;
        rb.velocity = Vector2.zero;
        spawner.removeEnemy(this.gameObject);
        manager.enemyKilled();
        Invoke("ClearBody", 5.0f);
        
    }
    private void UpdateDeadState()
    {
       
    }
    private void ExitDeadState()
    {
        isDead = false;
        enemyAnim.enabled = true;
        enemyAnim.SetBool("isDead", false);
        gameObject.layer = 10;
        currentHealth = maxHealth;
    }

    private void EnterHopState()
    {
        isMoving = false;
        int hopDirection;
        if(playerObject.transform.position.x < transform.position.x)
        {
            hopDirection = 1;
        }
        else
        {
            hopDirection = -1;
        }
        //knockBackStartTime = Time.time;
        groundCheckDistance = 10;
        enemyAnim.SetBool("isJumping", true);
    }
    private void UpdateHopState()
    {

    }
    private void ExitHopState()
    {
        
    }

    private void SetToMovingAfterHop()
    {
        enemyAnim.SetBool("isJumping", false);
        SwitchState(State.Moving);
    }

    //- Remaining Functions // 

    private void SwitchState(State state)
    {
        switch(currentState)
        {
            case State.Moving:
                ExitMovingState();                
                break;

            case State.Knockback:
                ExitKnockBackState();
                break;

            case State.Charge:
                ExitChargeState();
                break;

            case State.Attack:
                ExitAttackState();
                break;

            case State.HopBack:
                ExitHopState();
                break;

            case State.Dead:
                ExitDeadState();
                break;
        }
        switch (state)
        {
            case State.Moving:
                EnterMovingState();
                break;

            case State.Knockback:
                EnterKnockBackState();
                break;

            case State.Charge:
                EnterChargeState();
                break;

            case State.Attack:
                EnterAttackState();
                break;
            case State.HopBack:
                EnterHopState();
                break;

            case State.Dead:
                EnterDeadState();
                break;
        }
        currentState = state;
    }

    
    public void Damage(float[] attackDetails)
    {
      
        currentHealth -= attackDetails[0];
        if (attackDetails[1] > transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        Instantiate(hitParticle, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));

        SwitchState(State.Knockback);
             
    }

    public void CheckTouchDamage()
    {
        if(Time.time >= lastTouchDamageTime + touchDamageCoolDown && currentState != State.Dead)
        {
            touchDamageBotLeft.Set(touchDamageCheck.position.x - touchDamageWidth / 2, touchDamageCheck.position.y - touchDamageHeight / 2);
            touchDamageTopRight.Set(touchDamageCheck.position.x + touchDamageWidth / 2, touchDamageCheck.position.y + touchDamageHeight / 2);

            Collider2D hit = Physics2D.OverlapArea(touchDamageBotLeft, touchDamageTopRight, whatIsPlayer);

            if(hit != null)
            {
                lastTouchDamageTime = Time.time;
                attackDetails[0] = touchDamage;
                attackDetails[1] = transform.position.x;
                hit.SendMessage("Damage", attackDetails);
            }
        }
    }

    public void SendAttackDetailsAtEndOfAnim()
    {        
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, whatIsPlayer);
        if (hit != null)
        {
            attackCoolDown = Time.time;
            attackDetails[0] = attackDamage;
            attackDetails[1] = transform.position.x;
            isAttacking = false;
            hit.SendMessage("Damage", attackDetails);
        }
    }


    public void InstantiateArrow()
    {
        GameObject arrowClone = Instantiate(arrowPrefab, attackPoint.position, Quaternion.Euler(new Vector3(0,transform.eulerAngles.y , -90)));
        arrowClone.transform.SetParent(transform);
        attackCoolDown = Time.time;
        isAttacking = false;
    }
    public void sendArrowDamageDetails()
    {
        attackDetails[0] = attackDamage;
        attackDetails[1] = transform.position.x;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController_Hero>().ArrowDamage(attackDetails);
    }

    public void SetKnockBackValues(int attackType)
    {
        switch(attackType)
        {
            case 0:
                knockBackSpeed = new Vector2(knockBackSpeedAttack1.x,knockBackSpeedAttack1.y);
                break;
            case 1:
                knockBackSpeed = new Vector2(knockBackSpeedAttack2.x, knockBackSpeedAttack2.y);
                break;
            case 2:
                knockBackSpeed = new Vector2(knockBackSpeedAttack3.x, knockBackSpeedAttack3.y);
                break;

        }

    }


    public void UpdateAnimations()
    {
        enemyAnim.SetBool("isWalking", isMoving);
        enemyAnim.SetBool("isAttacking", isAttacking);
        enemyAnim.SetBool("isDead", isDead);
    }

    public void DisableAnimator()
    {
        enemyAnim.enabled = false;
    }

    public void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(new Vector3(0, 180.0f, 0));
    }

    public void ClearBody()
    {
        Destroy(gameObject);
        
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheckBot.position, new Vector2(wallCheckBot.position.x + wallCheckDistance, wallCheckBot.position.y));
        Gizmos.DrawLine(wallCheckTop.position, new Vector2(wallCheckTop.position.x + wallCheckDistance, wallCheckTop.position.y));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + enemyVisionDistance, transform.position.y));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + attackRange, transform.position.y));
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);




        Vector2 botLeft = new Vector2(touchDamageCheck.position.x - touchDamageWidth / 2, touchDamageCheck.position.y - touchDamageHeight / 2);
        Vector2 botRight = new Vector2(touchDamageCheck.position.x + touchDamageWidth / 2, touchDamageCheck.position.y - touchDamageHeight / 2);
        Vector2 topLeft = new Vector2(touchDamageCheck.position.x - touchDamageWidth / 2, touchDamageCheck.position.y + touchDamageHeight / 2);
        Vector2 topRight = new Vector2(touchDamageCheck.position.x + touchDamageWidth / 2, touchDamageCheck.position.y + touchDamageHeight / 2);
        Gizmos.DrawLine(botLeft, botRight);
        Gizmos.DrawLine(botRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, botLeft);
    }
    
}
