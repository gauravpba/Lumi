using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClose : MonoBehaviour
{

    private LevelManager manager;
    private bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        isOpen = true;
        GetComponent<Animator>().SetFloat("Status", -1);
        manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<LevelManager>();
    }

    private void DisableAnimator()
    {
        GetComponent<Animator>().enabled = false;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && other.GetType() == typeof(EdgeCollider2D))
        {
            Invoke("CloseDoor", 0.1f);            
        }
    }

    private void CloseDoor()
    {
        isOpen = false;
        GetComponent<Animator>().SetFloat("Status", 1);
    }
}
