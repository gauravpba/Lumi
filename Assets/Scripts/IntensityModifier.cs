using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntensityModifier : MonoBehaviour
{

    Material material;
    public float intensity;
    public float reductionSpeed;
    private bool playerInteracted = false;
    Color color;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
        color = material.GetColor("_Color");

        material.SetColor("_Color", color * intensity);
 
    }


    private void Update()
    {
        if (intensity > 1 && playerInteracted)
        {
            intensity -= Time.deltaTime * reductionSpeed;

            material.SetColor("_Color", color * intensity);
        }
        else if(!playerInteracted && intensity < 5)
        {
            intensity += Time.deltaTime * reductionSpeed/2;
            material.SetColor("_Color", color * intensity);
        }

        if (intensity < 1 && playerInteracted)
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else if(intensity >= 5)
        {
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerInteracted = true;
            other.SendMessage("FlowerInteractionBegan");
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInteracted = false;
            GetComponent<BoxCollider2D>().enabled = true;
            other.SendMessage("FlowerInteractionEnded");
        }
    }

}
