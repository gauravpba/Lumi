using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EnemySpawner : MonoBehaviour
{

    public int enemyCount;

    public List<GameObject> enemies = new List<GameObject>();
   
    public List<Transform> spawnPoints = new List<Transform>();

    public List<Vector3> usedSpawnPoints = new List<Vector3>();
    public List<GameObject> clonedEnemies = new List<GameObject>();
    public List<int> enemiesCloned = new List<int>();

    [SerializeField]
    private GameObject spawnPointsholder;

    private LevelManager manager;

    // Start is called before the first frame update
    public void SetupEnemies()
    {
        spawnPointsholder = GameObject.Find("EnemySpawnPoints");
        manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<LevelManager>();

        enemyCount = GameSettings.EnemyCount;

        createEnemyList();
        GatherSpawnPoints();
        SpawnEnemies();
    }
    private void createEnemyList()
    {
        if (enemiesCloned.Count == 0)
        {
            if (SceneManager.GetActiveScene().name == "Tutorial")
            {
                enemies.Add(manager.enemyTypes[4]);
                enemiesCloned.Add(0);
                enemyCount = 1;
            }
            else
            {
                for (int i = 0; i < enemyCount; i++)
                {
                    int index; 
                    if(GameSettings.difficultyLevel == 0)
                        index = Random.Range(0, manager.enemyTypes.Length - 1);
                    else
                        index = Random.Range(0, manager.enemyTypes.Length);
                    enemies.Add(manager.enemyTypes[index]);
                    enemiesCloned.Add(index);
                }
            }
        }
        else
        {
            for (int i = 0; i < enemiesCloned.Count; i++)
            {
                enemies.Add(manager.enemyTypes[enemiesCloned[i]]);
            }
        }
        
    }

    private void GatherSpawnPoints()
    {
        spawnPoints = spawnPointsholder.GetComponentsInChildren<Transform>().ToList<Transform>();
        spawnPoints.RemoveAt(0);
    }
    private void SpawnEnemies()
    {
        if (usedSpawnPoints.Count == 0)
        {
            usedSpawnPoints.Add(spawnPoints[Random.Range(0, spawnPoints.Count)].position);
            for (int i = 0; i < enemyCount - 1; i++)
            {
                int j = 0;
                while (usedSpawnPoints.Contains(spawnPoints[j].position))
                {
                    j = Random.Range(0, spawnPoints.Count);
                }
                usedSpawnPoints.Add(spawnPoints[j].position);
            }
        }

        for (int i = 0; i < usedSpawnPoints.Count; i++)
        {
            GameObject enemyClone = Instantiate(enemies[i], usedSpawnPoints[i], Quaternion.identity);
            enemyClone.transform.SetParent(this.transform);
            clonedEnemies.Add(enemyClone);
        }
        

    }

    public void removeEnemy(GameObject enemy)
    {
        int index = clonedEnemies.IndexOf(enemy);
        clonedEnemies.Remove(enemy);
        usedSpawnPoints.RemoveAt(index);
        enemiesCloned.RemoveAt(index);
        //enemies.Remove(enemy);
    }

    public List<int> GetRemainingEnemies()
    {
        return enemiesCloned;
    }

    public List<Vector3> GetSpawnPoints()
    {
        return usedSpawnPoints;
    }
}
