using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAttackController : MonoBehaviour
{
    
    private bool comboAttackBegin = false;
    private bool isAttacking = false;
    private bool canAttack = false;
    private bool isAttackingInAir = false;
    private bool isAttackControllerSetup = false;
    public bool playerCanAttack = false;

    private float attackingCoolDown;
    private float[] attackDetails = new float[2];

    public float attackRate = 0.5f;
    public float attackComboTimer;
    public float attackComboTimerSet;
    public float baseDamage; 

    private int attackType = -1;
    private int attackComboCount;

    
    [SerializeField]
    private float 
        maxHealth,
        currentHealth,
        attackDamage,
        attackRadius;

    public Animator playerAnim;

    private LevelManager manager;
    
    private PlayerController_Hero controller;

    private Rigidbody2D rb;

    [SerializeField]
    private Transform attackPoint;

    [SerializeField]
    private LayerMask whatIsDamageable;

    [SerializeField]
    private GameObject hitParticle;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetupPlayer()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();
        manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<LevelManager>();
        controller = transform.parent.GetComponent<PlayerController_Hero>();
        playerAnim.enabled = true;
        controller.transform.gameObject.layer = 12;
        isAttackControllerSetup = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttackControllerSetup)
        {
            if (manager.isLevelActive && controller.isPlayerSetup)
            {
                CheckCombatInput();
                checkIfCanAttack();
                UpdateAnimations();

                if (controller.isGrounded)
                {
                    isAttackingInAir = false;
                }
            }
        }
    }

    private void CheckCombatInput()
    {       
        if (Input.GetButtonDown("Fire"))
        {
            Attack();
        }
    }

    private void checkIfCanAttack()
    {
        if (attackingCoolDown <= 0)
        {
            canAttack = true;            
            isAttacking = false;
        }
        else
        {
            attackingCoolDown -= Time.deltaTime;
        }

        if (comboAttackBegin && attackComboTimer <= 0)
        {
            comboAttackBegin = false;
            attackComboCount = 0;
        }
        else if (attackComboTimer > 0)
        {
            attackComboTimer -= Time.deltaTime;
        }

        attackType = attackComboCount % 3;

    }

    private void Attack()
    {
        if (canAttack)
        {
            if (controller.isGrounded)
            {
                rb.velocity = Vector2.zero;
                controller.movementSpeed = 0;
                isAttacking = true;
            }
            else
            {
                isAttackingInAir = true;             
                //controller.usedUpJump();
                controller.canjump = false;
            }            
            attackingCoolDown = attackRate;
            attackComboTimer = attackComboTimerSet;
            canAttack = false;
            comboAttackBegin = true;

           
            attackDamage = baseDamage + 10 * (attackComboCount % 3);
            attackComboCount++;
            FindObjectOfType<AudioManager>().Play("Attack");
        }
    }

    private void CheckAttackHitBox()
    {

        // Event Called in the animation window for all three attacks
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackPoint.position,attackRadius,whatIsDamageable);

        foreach(Collider2D collider in detectedObjects)
        {            
            attackDetails[0] = attackDamage;
            attackDetails[1] = transform.position.x;
            collider.transform.SendMessage("SetKnockBackValues", (attackComboCount - 1) % 3);
            collider.transform.SendMessage("Damage", attackDetails);
            GameObject comboTextObj = Instantiate(manager.comboText, collider.transform.position, Quaternion.identity);
            comboTextObj.GetComponent<TextMeshPro>().text = "x" + attackComboCount.ToString();
            FindObjectOfType<AudioManager>().Play("AttackHit");
        }
        if (SceneManager.GetActiveScene().name != "Tutorial")
            controller.SetAllBasicMovement();
        else
        {
            controller.canMove = true;
            controller.canFlip = true;
        }
        
        //isAttackingInAir = false;
    }
    private void setMovementBack()
    {
        if (SceneManager.GetActiveScene().name != "Tutorial")
            controller.StopAllBasicMovement();
        else
        {
            controller.canMove = false;
            controller.canFlip = false;
        }

    }

    public void Damage(float[] attackDetails)
    {
        if (!controller.GetDashStatus())
        {
            int direction;
            int visualDamage = (int)attackDetails[0];
            HeartsHealthVisual.healthSystem.Damage(visualDamage);

            if(SceneManager.GetActiveScene().name != "Tutorial")
                controller.SetAllBasicMovement();
            else
            {
                controller.canMove = true;
                controller.canjump = true;                
            }
            
            currentHealth -= visualDamage;
           
            if (attackDetails[1] < transform.position.x)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
            playerAnim.SetBool("isHurt", true);
            //playerAnim.SetTrigger("isHurt");
            Instantiate(hitParticle, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            controller.KnockBack(direction);
            isAttacking = false;
            isAttackingInAir = false;
            manager.SetPlayerHealth(currentHealth);
            if (currentHealth <= 0)
            {
                controller.StopAllAction();
                FindObjectOfType<AudioManager>().Play("PlayerDeath");
                playerDied();
                return;
            }
            FindObjectOfType<AudioManager>().Play("Hurt");

        }
    }    

    private void playerDied()
    {
        playerAnim.SetBool("isDead", true);
        controller.isDead = true;
        controller.transform.gameObject.layer = 11;
        manager.playerKilled();        
    }

    private void DestroyPlayer()
    {
        Destroy(transform.parent.gameObject);
    }

    private void EndDoubleJump()
    {
        controller.isDoubleJumping = false;
       
    }

    private void LevelEnd()
    {
        StartCoroutine(manager.BeginSceneTransition());
    }

    public void SetupHealth(float health)
    {
        currentHealth = health;
    }

    private void UpdateAnimations()
    {
        playerAnim.SetBool("isAttacking", isAttacking);
        playerAnim.SetFloat("attackType", attackType);
        playerAnim.SetBool("isAttackingInAir", isAttackingInAir);
    }

    private void DisableAnimator()
    {
        playerAnim.SetBool("isDead", false);
        playerAnim.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

}
