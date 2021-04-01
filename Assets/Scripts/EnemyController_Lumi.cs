using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController_Lumi : MonoBehaviour
{

    private PlayerController_Lumi pc;

    [SerializeField]
    private Transform attackPoint;

    [SerializeField]
    private LayerMask whatIsPlayer;

    [SerializeField]
    private float attackRadius, attackRate;

    private float attackCoolDown;

    private bool canAttack = false;

    private Animator anim;

    private bool facingRight = false;
    private bool facingPlayer = false;

   
    private bool isPlayerLeft;

    // Update is called once per frame
    private void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController_Lumi>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Time.time >= attackCoolDown + attackRate)
            canAttack = true;
        else
            canAttack = false;

        if (pc.transform.position.x < transform.position.x)
        {
            isPlayerLeft = true;
        }
        else
            isPlayerLeft = false;


        if(isPlayerLeft)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

    }

    public void DamagePlayer()
    {
        attackCoolDown = Time.time;
        if (Physics2D.OverlapCircle(attackPoint.position, attackRadius, whatIsPlayer))
        {
            float[] attackDetails = new float[2];
            attackDetails[0] = 0.5f;
            attackDetails[1] = transform.position.x;
            pc.Damage(attackDetails);            
        }
        GetComponent<CircleCollider2D>().enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && canAttack)
        {            
            anim.SetTrigger("Attack");
            canAttack = false;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canAttack)
        {
            anim.SetTrigger("Attack");
            canAttack = false;            
        }
    }

}
