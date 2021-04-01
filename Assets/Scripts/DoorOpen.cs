using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{

    private LevelManager manager;
    private bool isOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().SetFloat("Status", 1);
        manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<LevelManager>();
    }

    private void DisableAnimator()
    {
        GetComponent<Animator>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && other.GetType() == typeof(EdgeCollider2D) && isOpen)
        {           
            manager.LevelEnd();
        }
    }

    public void OpenDoor()
    {
        isOpen = true;
        GetComponent<Animator>().SetFloat("Status", -1);        
    }
}
