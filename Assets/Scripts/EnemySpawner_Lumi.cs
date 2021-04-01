using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner_Lumi : MonoBehaviour
{
    
    private List<Transform> spawnPoints = new List<Transform>();

    [SerializeField]
    private GameObject enemyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = GameObject.Find("EnemySpawnPoints").GetComponentsInChildren<Transform>().ToList();
        spawnPoints.RemoveAt(0);

        GameSettings.GetLumiSettings();

        for(int i = 0; i < GameSettings.darkEnemyCount; i++)
        {
            GameObject enemyClone = Instantiate(enemyPrefab, spawnPoints[Random.Range(0, spawnPoints.Count)].position, Quaternion.identity);
            enemyClone.transform.SetParent(transform);
        }

    }

}
