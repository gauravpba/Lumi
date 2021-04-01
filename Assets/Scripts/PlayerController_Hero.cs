using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class PlayerController_Hero : MonoBehaviour
{

    private bool
        isFacingRight = true,
        isWalking,
        isTouchingWall,
        isTouchingSlope,
        isWallSliding,
        isTouchingLedge,
        canClimbLedge = false,
        ledgeDetected,
        isDashing,
        knockback,
        smokeSpawn = false;

    //[HideInInspector]
    public bool
        canMove = true,
        canFlip = true,
        isGrounded,
        canjump,
        isDead = false,
        isDoubleJumping = false,
        canDash = false,
        isPlayerSetup = false;
        

    [SerializeField]
    private float
        maxSpeed,
        accelerationFactor,
        jumpForce,
        groundCheckRadius,
        wallCheckDistance,
        slopeCheckDistance,
        wallSlidingSpeed,
        movementForceInAir,
        airDragMultiplier,
        variableJumpHeightMultipler,
        ledgeClimbxOffset1,        
        ledgeClimbyOffset1,
        ledgeClimbxOffset2,
        ledgeClimbyOffset2,
        wallHopForce,
        wallJumpForce,
        dashTime,
        dashSpeed,
        distanceBetweenImages,
        dashCooldown,
        smokeSpawnTimeSet,
        knockBackDuration,
        trapTouchDamageCoolDown;
    
    [HideInInspector]
    public float movementSpeed;

    private float
        knockBackStartTime,
        movementInputDirection,
        dashTimeLeft,
        lastImageXPos,
        smokeSpawnTime,
        lastDash = -100,
        lastTouchDamageTime;

    private int 
        amountOfJumps,
        amountOfJumpsLeft,
        facingDirection;

    public LayerMask whatIsGround;

    [SerializeField]
    private Transform 
        groundCheck,
        wallCheck,
        ledgeCheck,
        slopeCheck,
        smokeSpawnPoint;
   

    private Vector2 ledgePositionBot;


    [SerializeField]
    private Vector2 knockBackSpeed;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;


    public Vector3
        wallHopDirection,
        wallJumpDirection;       
    
    //RigidBody
    private Rigidbody2D rb;

    //Animator
    [SerializeField]
    private Animator playerAnim;

    private LevelManager manager;

    private Camera cam;
    [SerializeField]
    private CameraShaker cameraShaker;

    [SerializeField]
    private PlayerAttackController attackController;

    [SerializeField]
    private Transform respawnPoint;

    [SerializeField]
    private GameObject smokeParticlePrefab;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void SetupPlayer()
    {
        manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<LevelManager>();
        rb = GetComponent<Rigidbody2D>();
        facingDirection = 1;
        movementSpeed = 0;
        ledgePositionBot = Vector2.zero;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
        gameObject.layer = 12;
        isDead = false;
        canMove = true;
        canFlip = true;
        canDash = false;
        canjump = false;        
        amountOfJumps = manager.NumberOfJumps;
        amountOfJumpsLeft = amountOfJumps;
        cameraShaker = Camera.main.GetComponent<CameraShaker>();
        cam = Camera.main;
        isPlayerSetup = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerSetup)
        {
            if (manager.isLevelActive && !isDead)
            {
                CheckInput();
                checkMovementDirection();
                UpdateAnimations();
                CheckSurroundings();
                checkIfCanJump();
                checkIfWallSliding();
                CheckLedgeClimb();
                CheckDash();
                CheckKnockBack();
            }
        }
       
       
    }

    private void FixedUpdate()
    {
        if (isPlayerSetup)
        {
            if (manager.isLevelActive && !isDead && isPlayerSetup)
            {
                Applymovement();
            }
        }
    }

    private void CheckInput()
    {

        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if(Input.GetButtonUp("Jump") && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultipler);
        }


        if (Input.GetButtonDown("Dash") && canDash)
        {
            if (Time.time >= lastDash + dashCooldown)
            {
                AttemptToDash();
            }
        }
        
        
    }

    private void Applymovement()
    {
        if (isGrounded && canMove && !isDashing && !knockback)
        {
            if (movementInputDirection == 0)
                movementSpeed = 0;
            else
            {
                if(Time.time >= smokeSpawnTime + smokeSpawnTimeSet)
                {
                    Instantiate(smokeParticlePrefab, smokeSpawnPoint.position, Quaternion.identity);
                    smokeSpawnTime = Time.time;
                }
                else
                {
                    smokeSpawnTime -= Time.deltaTime;
                }
            }
            movementSpeed += accelerationFactor * 5 * Time.fixedDeltaTime;
            movementSpeed = Mathf.Clamp(movementSpeed, 0, maxSpeed);
            rb.velocity = new Vector2(movementInputDirection * movementSpeed, rb.velocity.y);
        }
        else if (!isGrounded && !isWallSliding && movementInputDirection != 0 && canMove && !knockback && !isDashing)
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection, 0);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);

            if (Mathf.Abs(rb.velocity.x) > movementSpeed)
                rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);

        }
        else if (!isGrounded && !isWallSliding && movementInputDirection == 0 && canMove && !knockback && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlidingSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed);
                canjump = true;
            }
        }
    }

    private void Jump()
    {
        if (canjump)
        {
            if (!isWallSliding && !isDashing && rb.velocity.y >= -5)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                amountOfJumpsLeft--;
                if (amountOfJumpsLeft == 0 && amountOfJumps > 1)
                {
                    playerAnim.SetBool("isDoubleJumping", true);
                    isDoubleJumping = true;
                    StartCoroutine(cameraShaker.Shake(0.1f, 0.3f));

                }
            }
            else if (isWallSliding && movementInputDirection == 0)
            {
                movementSpeed = 6;
                isWallSliding = false;
                amountOfJumpsLeft--;                 
                Vector2 forceToadd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallJumpForce * wallHopDirection.y);
                rb.AddForce(forceToadd, ForceMode2D.Impulse);
            }
            else if ((isWallSliding || isTouchingWall) && ((movementInputDirection < 0 && facingDirection > 0) || (movementInputDirection > 0 && facingDirection < 0)))
            {
                movementSpeed = 6;
                isWallSliding = false;
                amountOfJumpsLeft--;
                Vector2 forceToadd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
                rb.AddForce(forceToadd, ForceMode2D.Impulse);
            }
            FindObjectOfType<AudioManager>().Play("Jump");
        }
    }   

    private void checkIfCanJump()
    {
        if ((isGrounded && rb.velocity.y <= 0) || isWallSliding)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if (amountOfJumpsLeft <= 0 )
        {
            canjump = false;
        }
        else
        {
            canjump = true;
        }
    }

    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        FindObjectOfType<AudioManager>().Play("Dash");
        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXPos = transform.position.x;
    }

    private void CheckDash()
    {
        if(isDashing)
        {
            if (dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                //canjump = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, rb.velocity.y);
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXPos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXPos = transform.position.x;
                }
            }

            if(dashTimeLeft <= 0 || isTouchingWall || isTouchingSlope)
            {
                canMove = true;
                canFlip = true;                
                isDashing = false;
            }
        }
    }

    public bool GetDashStatus()
    {
        return isDashing;
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        if(!isGrounded && !smokeSpawn)
        {
            smokeSpawn = true;
            Instantiate(smokeParticlePrefab, smokeSpawnPoint.position, Quaternion.identity);
        }
        else if(isGrounded)
        {
            smokeSpawn = false;
            isDoubleJumping = false;
        }

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);

        isTouchingSlope = Physics2D.Raycast(slopeCheck.position, transform.right, slopeCheckDistance, whatIsGround) && !isTouchingWall;

       
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, whatIsGround);

        if (isTouchingWall && !isTouchingLedge && !ledgeDetected && !isGrounded)
        {
            ledgeDetected = true;
            ledgePositionBot = wallCheck.position;
            canMove = false;
            canFlip = false;
            canjump = false;
            canDash = false;
            isDoubleJumping = false;
        }   
       
    }

    private void CheckLedgeClimb()
    {
        if (ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;

            if (isFacingRight)
            {
                ledgePos1 = new Vector2(Mathf.Floor(ledgePositionBot.x + wallCheckDistance) - ledgeClimbxOffset1, Mathf.Floor(ledgePositionBot.y) + ledgeClimbyOffset1);
                ledgePos2 = new Vector2(Mathf.Floor(ledgePositionBot.x + wallCheckDistance) + ledgeClimbxOffset2, Mathf.Floor(ledgePositionBot.y) + ledgeClimbyOffset2);
            }   
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePositionBot.x - wallCheckDistance - 0.2f) + ledgeClimbxOffset1, Mathf.Floor(ledgePositionBot.y) + ledgeClimbyOffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePositionBot.x - wallCheckDistance) - ledgeClimbxOffset2, Mathf.Floor(ledgePositionBot.y) + ledgeClimbyOffset2);
            }

           

            playerAnim.SetBool("canClimbLedge", canClimbLedge);
            FindObjectOfType<AudioManager>().Play("LedgeClimb");            
            StartCoroutine(FinishLedgeClimb());
           
            
        }

        if(canClimbLedge)
        {
            transform.position = ledgePos1;
            StopAllBasicMovement();
            isDoubleJumping = false;
        }
    } 

    IEnumerator FinishLedgeClimb()
    {
        yield return new WaitForSeconds(1f);
        canClimbLedge = false;
        //spriteObject.transform.localPosition = Vector3.zero;
        transform.position = ledgePos2;
        SetAllBasicMovement();
        ledgeDetected = false;
        playerAnim.SetBool("canClimbLedge", canClimbLedge);        

    }
  
    private void checkIfWallSliding()
    {
        if(isTouchingWall && !isGrounded && rb.velocity.y <=0 && !canClimbLedge)
        {
            isWallSliding = true;
            isDoubleJumping = false;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void Damage(float[] attackDetails)
    {
        attackController.Damage(attackDetails);
    }
    public void ArrowDamage(float[] attackDetails)
    {
        attackController.Damage(attackDetails);
    }
    public void KnockBack(int direction)
    {
        knockback = true;
        knockBackStartTime = Time.time;
        StartCoroutine(cameraShaker.Shake(0.15f, 0.6f));
        rb.velocity = new Vector2(knockBackSpeed.x * direction, knockBackSpeed.y);
    }
    
    private void CheckKnockBack()
    {
        if(Time.time >= knockBackStartTime + knockBackDuration && knockback)
        {
            playerAnim.SetBool("isHurt", false);
            knockback = false;
            SetAllBasicMovement();
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
    }

    private void CollectDiamond()
    {
        manager.playerCollectedDiamond();
    }

    private void checkMovementDirection()
    {
        if(isFacingRight && movementInputDirection < 0)
        {
            Flip();          
        }
        else if(!isFacingRight && movementInputDirection > 0)
        {
            Flip();            
        }

        if(Mathf.Abs(rb.velocity.x) >= 0.01f && isGrounded)
        {
            isWalking = true;

            Sound[] sounds = FindObjectOfType<AudioManager>().sounds;
            Sound s = Array.Find(sounds, sound => sound.name == "Run");
            if(s.source.isPlaying == false)
                FindObjectOfType<AudioManager>().Play("Run");
        }
        else
        {
            //FindObjectOfType<AudioManager>().Stop("Run");
            isWalking = false;
        }
    }

    private void Flip()
    {
        if (!isWallSliding && canFlip && !knockback)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0, 180, 0f, 0);
            facingDirection *= -1;
        }
    }

    private void UpdateAnimations()
    {
        playerAnim.SetBool("isWalking", isWalking);
        playerAnim.SetBool("isGrounded", isGrounded);
        playerAnim.SetBool("isWallSliding", isWallSliding);
        playerAnim.SetFloat("yVelocity", rb.velocity.y);
        playerAnim.SetBool("isDoubleJumping", isDoubleJumping);
    }
   
    public int GetMovementDirectionOfPlayer()
    {
        return facingDirection;
    }

    public void Celebrate()
    {
        StopAllAction();
        playerAnim.SetTrigger("Celebrate");
    }

    public void StopAllAction()
    {
        rb.velocity = Vector2.zero;
        StopAllBasicMovement();

        isWallSliding = false;
        isWalking = false;
        isGrounded = true;
        isDoubleJumping = false;

        playerAnim.SetBool("isWalking", isWalking);
        playerAnim.SetBool("isGrounded", isGrounded);
        playerAnim.SetBool("isWallSliding", isWallSliding);
        playerAnim.SetFloat("yVelocity", rb.velocity.y);
        playerAnim.SetBool("isDoubleJumping", isDoubleJumping);        
    }

    public void StopAllBasicMovement()
    {
        canMove = false;
        canjump = false;
        canDash = false;
        canFlip = false;
    }

    public void SetAllBasicMovement()
    {
        canMove = true;
        canDash = true;
        canjump = true;
        canFlip = true;
    }

    public void usedUpJump()
    {
        amountOfJumpsLeft--;
    }

    public void UpdateJump()
    {
        amountOfJumps = manager.NumberOfJumps;        
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null && wallCheck != null)
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
            Gizmos.DrawLine(ledgeCheck.position, new Vector2(ledgeCheck.position.x + wallCheckDistance, ledgeCheck.position.y));
            Gizmos.DrawLine(slopeCheck.position, new Vector2(slopeCheck.position.x + slopeCheckDistance, slopeCheck.position.y));
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.CompareTag("LevelEnd"))
        {
            manager.LevelEnd();
        }
        if(other.collider.CompareTag("Traps"))
        {

            float[] attackDetails = new float[2];
            attackDetails[0] = manager.TrapTouchDamage;
            attackDetails[1] = other.collider.transform.position.x;
            if (Time.time >= lastTouchDamageTime + trapTouchDamageCoolDown)
            {
                Damage(attackDetails);
                lastTouchDamageTime = Time.time;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.CompareTag("Traps"))
        {

            float[] attackDetails = new float[2];
            attackDetails[0] = manager.TrapTouchDamage;
            attackDetails[1] = other.collider.transform.position.x;
            if (Time.time >= lastTouchDamageTime + trapTouchDamageCoolDown)
            {
                Damage(attackDetails);
                lastTouchDamageTime = Time.time;
            }
        }
    }
}
