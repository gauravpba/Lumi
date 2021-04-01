using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerController_Lumi : MonoBehaviour
{
    // Movement Variables
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isJumping;
    private bool isGrounded;
    private bool canjump;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool knockBack;

    [SerializeField]
    private float knockBackDuration = 0.01f;

    private float knockBackStartTime;

    private float movementInputDirection;
    public float movementSpeed = 10.0f;
    public float jumpForce = 16.0f;
    public float movementForceInAir;
    public float airDragMultiplier;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlidingSpeed;   
    private int amountOfJumpsLeft;
    public int amountOfJumps = 1;
    public float wallHopForce;
    public float wallJumpForce;
    private int facingDirection;
    public int fragmentsCollected;

    [SerializeField]
    private Vector2 knockBackSpeed;

    // Wall Jump
    public Vector3 wallHopDirection;
    public Vector3 wallJumpDirection;


    //Collision Detection variables
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public Transform wallCheck;

    //RigidBody
    private Rigidbody2D rb;

    //Sprite
    public GameObject spriteObject;

    //Animator
    public Animator playerAnim;

    //Lumi Intensity modifiers
    public float lightReductionSpeed;
    public float lightIncreaseSpeed;
    public Light2D lumiLight;
    public Slider intensitySlider;
    private float intensityFactor;

    private bool interactedFlower = false;

    private LevelManager manager;

    private List<Transform> spawnPoints = new List<Transform>();

    private CameraShaker cameraShaker;

    public TMP_Text fragmentText;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        amountOfJumpsLeft = amountOfJumps;
        facingDirection = 1;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
        intensitySlider.value = 1;
        intensityFactor = lumiLight.intensity;
        spawnPoints = GameObject.Find("LumiSpawnPoints").GetComponentsInChildren<Transform>().ToList();
        spawnPoints.RemoveAt(0);
        transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        cameraShaker = Camera.main.GetComponent<CameraShaker>();
        manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(intensitySlider.gameObject.activeInHierarchy == false)
            intensitySlider.gameObject.SetActive(true);
        CheckInput();
        checkMovementDirection();
        UpdateAnimations();
        CheckSurroundings();
        checkIfCanJump();
        checkIfWallSliding();
        ReduceIntensity();
        IncreaseIntensity();
        CheckKnockBack();


    }
    private void FixedUpdate()
    {
        
            Applymovement();
        
    }
    private void CheckInput()
    {

        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }
    private void Applymovement()
    {
        if (isGrounded)
            rb.velocity = new Vector2(movementInputDirection * movementSpeed, rb.velocity.y);
        else if (!isGrounded && !isWallSliding && movementInputDirection != 0)
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection, 0);
            rb.AddForce(forceToAdd);

            if (Mathf.Abs(rb.velocity.x) > movementSpeed)
                rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);

        }
        else if (!isGrounded && !isWallSliding && movementInputDirection == 0)
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
            if (!isWallSliding)
            {

                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                amountOfJumpsLeft--;
            }
            else if (isWallSliding && movementInputDirection == 0)
            {
                isWallSliding = false;
                amountOfJumpsLeft--;
                Vector2 forceToadd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);
                rb.AddForce(forceToadd, ForceMode2D.Impulse);
            }
            else if ((isWallSliding || isTouchingWall) && movementInputDirection != 0)
            {
                isWallSliding = false;
                amountOfJumpsLeft--;
                Vector2 forceToadd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
                rb.AddForce(forceToadd, ForceMode2D.Impulse);
            }
            FindObjectOfType<AudioManager>().Play("Jump_Lumi");
        }
    }
    private void checkIfCanJump()
    {
        if ((isGrounded && rb.velocity.y <= 0) || isWallSliding)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if (amountOfJumpsLeft <= 0)
        {
            canjump = false;
        }
        else
        {
            canjump = true;
        }
    }
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }
  
    private void checkIfWallSliding()
    {
        if(isTouchingWall & !isGrounded && rb.velocity.y <=0)
        {
            isWallSliding = true;
            spriteObject.transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 90);
        }
        else
        {
            isWallSliding = false;
            spriteObject.transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
        }
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

        if(rb.velocity.x != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }
    private void Flip()
    {
        if (!isWallSliding)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0, 180, 0f, 0);
            facingDirection *= -1;
        }
    }
    public int GetMovementDirectionOfPlayer()
    {
        return facingDirection;
    }
    private void UpdateAnimations()
    {
        playerAnim.SetBool("isWalking", isWalking);
        playerAnim.SetBool("isGrounded", isGrounded);
        //playerAnim.SetBool("isWallSliding", isWallSliding);
    }
   
    private void ReduceIntensity()
    {
        if (!interactedFlower)
        {
            lumiLight.intensity -= lightReductionSpeed * Time.deltaTime;

            intensitySlider.value = lumiLight.intensity  / intensityFactor;

            if(intensitySlider.value <= 0)
            {               
                manager.Respawn();
            }
        }
    }


    public void Damage(float[] attackDetails)
    {
        int direction; 

        lumiLight.intensity -= attackDetails[0];

        intensitySlider.value = lumiLight.intensity / intensityFactor;

        if (intensitySlider.value <= 0)
        {           
            manager.Respawn();
        }

        if (attackDetails[1] < transform.position.x)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }

        KnockBack(direction);

    }
    public void KnockBack(int direction)
    {
        knockBack = true;
        knockBackStartTime = Time.time;
        StartCoroutine(cameraShaker.Shake(0.15f, 0.6f));
        rb.velocity = new Vector2(knockBackSpeed.x * direction, knockBackSpeed.y);
    }
    private void CheckKnockBack()
    {
        if (Time.time >= knockBackStartTime + knockBackDuration && knockBack)
        {
            knockBack = false;            
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
    }

    private void IncreaseIntensity()
    {
        if (interactedFlower && lumiLight.intensity < intensityFactor)
        {
            lumiLight.intensity += lightIncreaseSpeed * Time.deltaTime;
            intensitySlider.value = lumiLight.intensity * 1 / intensityFactor;
        }
    }

    public void FlowerInteractionBegan()
    {
        interactedFlower = true;
    }
    public void FlowerInteractionEnded()
    {
        interactedFlower = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }

    public void collectedFragment()
    {
        fragmentsCollected++;
        fragmentText.text = fragmentsCollected.ToString();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.CompareTag("LevelEnd"))
        {
            manager.LevelEnd();
        }
    }

}
