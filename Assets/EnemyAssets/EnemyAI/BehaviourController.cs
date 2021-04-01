using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourController : MonoBehaviour
{
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
        enemyVisionDistance;

    public float
        lastTouchDamageTime,
        lastAttackDamageTime,
        knockBackStartTime;

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


    // Start is called before the first frame update
    void Start()
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

    // Update is called once per frame
    void Update()
    {
        UpdateAnimations();
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

        isHurt = true;
        enemyAnim.SetTrigger("isHurt");
        isDead = true;
        gameObject.layer = 11;
        rb.velocity = Vector2.zero;
        spawner.removeEnemy(this.gameObject);
        manager.enemyKilled();
        Invoke("ClearBody", 2.0f);
    }

    public void CheckTouchDamage()
    {
        if (Time.time >= lastTouchDamageTime + touchDamageCoolDown)
        {
            touchDamageBotLeft.Set(touchDamageCheck.position.x - touchDamageWidth / 2, touchDamageCheck.position.y - touchDamageHeight / 2);
            touchDamageTopRight.Set(touchDamageCheck.position.x + touchDamageWidth / 2, touchDamageCheck.position.y + touchDamageHeight / 2);

            Collider2D hit = Physics2D.OverlapArea(touchDamageBotLeft, touchDamageTopRight, whatIsPlayer);

            if (hit != null)
            {
                lastTouchDamageTime = Time.time;
                attackDetails[0] = touchDamage;
                attackDetails[1] = transform.position.x;
                hit.SendMessage("Damage", attackDetails);
            }
        }
    }


    public void SetKnockBackValues(int attackType)
    {
        switch (attackType)
        {
            case 0:
                knockBackSpeed = new Vector2(knockBackSpeedAttack1.x, knockBackSpeedAttack1.y);
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
