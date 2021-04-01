using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTutorial : MonoBehaviour
{
    [SerializeField]
    private GameObject tutorialObject;
    
    private PlayerController_Hero pc;
    private LevelManager manager;

    private void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController_Hero>();
        manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<LevelManager>();       
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && other.GetType() == typeof(EdgeCollider2D))
        {
            if (tutorialObject.name == "Tutorial_Move_Text")
            {
                manager.enableJump();
            }
               
            else if (tutorialObject.name == "Tutorial_Jump_Text_WallJump")
            {
                manager.enableDoubleJump();
               
            }
            else if (tutorialObject.name == "Tutorial_Jump_Text_DoubleJump")
            {
                manager.enableDash();                
            }
            else if(tutorialObject.name == "Tutorial_Jump_Text_Dash")
            {
                GameObject.Find("FinalTutorial").GetComponent<DestroySelf>().InvokeDestroy();
            }
            Destroy(tutorialObject);
            Destroy(gameObject);
        }
    }

}
