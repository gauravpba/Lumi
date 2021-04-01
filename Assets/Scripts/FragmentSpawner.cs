using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FragmentSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject fragmentPrefab;

    private List<Transform> spawnPoints;

    private int fragmentsInPlay = 0;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = GameObject.Find("FragmentSpawnerPoints").GetComponentsInChildren<Transform>().ToList();
        spawnPoints.RemoveAt(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(fragmentsInPlay < 1)
        {
            Instantiate(fragmentPrefab, spawnPoints[Random.Range(0, spawnPoints.Count)].position, Quaternion.identity);
            fragmentsInPlay++;
        }
    }

    public void fragmentCollected()
    {
        fragmentsInPlay--;
    }
}
