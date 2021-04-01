using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulFragmentCollectible : MonoBehaviour
{
    private LevelManager manager;

    private FragmentSpawner fragmentSpawner;

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<LevelManager>();
        fragmentSpawner = GameObject.Find("FragmentSpawner").GetComponent<FragmentSpawner>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            other.SendMessage("collectedFragment");
            manager.fragmentCollected();
            fragmentSpawner.fragmentCollected();
            Destroy(gameObject);
        }
    }
}
