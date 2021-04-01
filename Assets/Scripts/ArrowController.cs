using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{

    [SerializeField]
    private float forceAmount;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.up * forceAmount, ForceMode2D.Impulse);
        // GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        
        if(other.collider.CompareTag("Player"))
        {
            transform.parent.GetComponent<BasicEnemyController>().sendArrowDamageDetails();            
        }
        Destroy(gameObject);
    }

}
