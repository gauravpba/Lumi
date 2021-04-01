using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class LevelManager : MonoBehaviour
{

    public GameObject[] enemyTypes;

    public GameObject LevelEndImage;
    
    public bool isLevelActive = true;
    public bool isPlayerDead;
    public Transform RespawnPoint;

    [SerializeField]
    private GameObject 
        playerPrefab;
 
    private bool respawn;

    private CameraController_Hero cameraController;

    [SerializeField]
    private PlayerAttackController attackController;
    [SerializeField]
    private PlayerController_Hero playerController;

    [SerializeField]
    private int 
        maxPlayerHealthEasy,
        maxPlayerHealthNormal,
        maxPlayerHealthHard,         
        currentLevel;

    private int
        enemiesInLevel,
        enemiesKilled,
        diamondsCollected;

    public int NumberOfJumps, soulFragmentsCollected;

    public float 
        maxHealth, 
        RespawnTime,
        TrapTouchDamage;
   
    private float 
        respawnTimeStart,
        currentPlayerHealth;

    [SerializeField]
    TMP_Text diamondCount;

    private Camera mainCamera;

    [SerializeField]
    private GameObject 
        FinalTutorialObject;
    
    private EnemySpawner enemySpawner; 

    private DoorOpen exitDoor;

    public GameObject comboText;

    public Button skipTutorialButton;


    private static LevelManager _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartLevel());
    }
   
    IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(1f);
        //restartButton.SetActive(false);
       

        mainCamera = Camera.main;

        currentLevel = SceneManager.GetActiveScene().buildIndex;


        if (SceneManager.GetActiveScene().name != "Level_Soul")
        {
            LevelEndImage = GameObject.Find("SceneTransition");
            LevelEndImage.SetActive(true);

            GameSettings.GetSettings();

            if (SceneManager.GetActiveScene().name == "Tutorial")
            {
                maxHealth = GameSettings.playerMaxHealth;
                currentPlayerHealth = maxHealth;
                NumberOfJumps = 1;                
            }
            else
            {
                
                
                    RespawnPoint = GameObject.FindGameObjectWithTag("RespawnPoint").transform;
                    RespawnPoint.position = PlayerStats.currentRespawnPoint;
                    if (RespawnPoint.position == Vector3.zero)
                        RespawnPoint.position = GameObject.Find("PlayerFirstSpawnPoint").transform.position;
                        
                    Instantiate(playerPrefab, RespawnPoint.position, Quaternion.identity);

                    NumberOfJumps = 2;
                    if (SceneManager.GetActiveScene().name != "EndScene")
                    {
                        currentPlayerHealth = PlayerStats.playerHealth;
                        maxHealth = PlayerStats.playerMaxHealth;
                        diamondsCollected = PlayerStats.playerDiamondsCollected;
                        enemiesKilled = PlayerStats.enemiesKilled;
                        diamondCount = GameObject.Find("DiamondCount").GetComponent<TMP_Text>();
                        diamondCount.text = diamondsCollected.ToString();
                    }
            }

            cameraController = mainCamera.GetComponentInParent<CameraController_Hero>();
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController_Hero>();
            attackController = GameObject.FindGameObjectWithTag("PlayerSpriteObject").GetComponent<PlayerAttackController>();

            if (SceneManager.GetActiveScene().name != "Tutorial" && SceneManager.GetActiveScene().name != "EndScene")
                GetPlayerStats();


            playerController.SetupPlayer();
            attackController.SetupPlayer();
            cameraController.SetupCamera();

            if (SceneManager.GetActiveScene().name != "EndScene")
            {
                HeartsHealthVisual.instance.SetHealthBar(maxHealth / 4);
                HeartsHealthVisual.healthSystem.Damage((int)(maxHealth - currentPlayerHealth));
                attackController.SetupHealth(currentPlayerHealth);

                enemySpawner = GameObject.FindGameObjectWithTag("EnemySpawner").GetComponent<EnemySpawner>();

                enemySpawner.enemiesCloned = GameSettings.enemies;
                enemySpawner.usedSpawnPoints = GameSettings.usedPoints;

                enemySpawner.SetupEnemies();
            }
            if (GameObject.FindGameObjectWithTag("ExitDoor") != null)
                exitDoor = GameObject.FindGameObjectWithTag("ExitDoor").GetComponent<DoorOpen>();

            if (SceneManager.GetActiveScene().name != "Tutorial")
            {
                enableDash();
                enableDoubleJump();
                enableJump();
                enemiesInLevel = GameSettings.EnemyCount;
            }
            else if (SceneManager.GetActiveScene().name == "Tutorial")
            {
                enemiesInLevel = 1;
                skipTutorialButton = GameObject.Find("SkipTut").GetComponent<Button>();
                skipTutorialButton.interactable = true;
            }            

            isLevelActive = true;
           

        }

    }


    private void Update()
    {
        if(isPlayerDead)
            CheckRespawn();   
     
        /*
        if(Input.GetKeyDown(KeyCode.Return))
        {           
            LevelEnd();
        }*/
    }

    public void SetPlayerHealth(float health)
    {
        currentPlayerHealth = health;
    }

    public void playerCollectedDiamond()
    {
        diamondsCollected++;
        diamondCount.text = diamondsCollected.ToString();

        if(diamondsCollected >= 3)
        {
            diamondsCollected = 0;
            HeartsHealthVisual.healthSystem.Heal(1);
            diamondCount.text = diamondsCollected.ToString();
        }
    }

    public void enemyKilled()
    {
        enemiesKilled++;
        if(enemiesKilled == enemiesInLevel)
        {
            exitDoor.OpenDoor();
        }

       PlayerStats.currentRespawnPoint = playerController.transform.position;

    }

    public void playerKilled()
    {
        isPlayerDead = true;
        UpdatePlayerStats();
       
        GameSettings.enemies = enemySpawner.GetRemainingEnemies();
        GameSettings.usedPoints = enemySpawner.GetSpawnPoints();

        //Respawn();

        FindObjectOfType<AudioManager>().Stop("Theme");
        FindObjectOfType<AudioManager>().Play("Theme_Lumi");
        soulFragmentsCollected = 0;
        StartCoroutine("ChangeLevel", SceneManager.sceneCountInBuildSettings - 1);

    }

    public void enableJump()
    {
        playerController.canjump = true;
    }
    public void enableDash()
    {
        playerController.canDash = true;
    }

    public void enableDoubleJump()
    {
        NumberOfJumps = 2;
        playerController.UpdateJump();
    }
  
    public void LevelEnd()
    {
        GameSettings.usedPoints = new List<Vector3>();
        
        playerController.StopAllAction();
        enemiesKilled = 0;

        UpdatePlayerStats();
        PlayerStats.currentRespawnPoint = Vector3.zero;
        
        GameSettings.enemies = new List<int>();

        isLevelActive = false;
        
        StartCoroutine(BeginSceneTransition());
    }

    public void UpdatePlayerStats()
    {
        PlayerStats.playerHealth = currentPlayerHealth;
        PlayerStats.playerMaxHealth = maxHealth;
        PlayerStats.playerDiamondsCollected = diamondsCollected;
        PlayerStats.enemiesKilled = enemiesKilled;
        PlayerStats.currentLevel = currentLevel;
        //PlayerStats.currentRespawnPoint = RespawnPoint.position;
    }

    private void GetPlayerStats()
    {
        currentPlayerHealth = PlayerStats.playerHealth;
        maxHealth = PlayerStats.playerMaxHealth;
        diamondsCollected = PlayerStats.playerDiamondsCollected;
        enemiesKilled = PlayerStats.enemiesKilled;
        //currentLevel = PlayerStats.currentLevel ;



        if (PlayerStats.currentRespawnPoint == Vector3.zero)
        {
            UpdateRespawnPoint(playerController.transform.position);
        }
        else
        {
            UpdateRespawnPoint(PlayerStats.currentRespawnPoint);
        }
    }
    public IEnumerator BeginSceneTransition()
    {
        yield return new WaitForSeconds(0.1f);
        LevelEndImage.GetComponent<Animator>().SetTrigger("LevelEnd");
        //restartButton.SetActive(true);
        StartCoroutine(ChangeLevel((SceneManager.GetActiveScene().buildIndex + 1)));
    }
    IEnumerator ChangeLevel(int index)
    {
        yield return new WaitForSeconds(1);
        
        SceneManager.LoadScene(index);
        StartCoroutine(StartLevel());
    }
    public void restartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void UpdateRespawnPoint(Vector3 position)
    {
        RespawnPoint.position = position;
    }
    public void Respawn()
    {        
        SceneManager.LoadScene(PlayerStats.currentLevel);
        if (soulFragmentsCollected < 4)
        {
            GameOver();
        }
        else
            StartCoroutine("StartLevel");

        FindObjectOfType<AudioManager>().Stop("Theme_Lumi");
        FindObjectOfType<AudioManager>().Play("Theme");
        respawnTimeStart = Time.time;
        respawn = true; 
    }

    private void CheckRespawn()
    {
        if(Time.time >= respawnTimeStart + RespawnTime && respawn)
        {
            isPlayerDead = false;

            float respawnHealth = soulFragmentsCollected/4;

           
            HeartsHealthVisual.instance.SetHealthBar(respawnHealth);
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController_Hero>();
            attackController = GameObject.FindGameObjectWithTag("PlayerSpriteObject").GetComponent<PlayerAttackController>();
            attackController.SetupHealth(soulFragmentsCollected);
            
        }
    }
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    public void TutorialEnd()
    {
        Destroy(FinalTutorialObject);
    }


    public void fragmentCollected()
    {
        soulFragmentsCollected++;
        
        if (soulFragmentsCollected >= GameSettings.playerMaxHealth)
        {
            Respawn();
        }
    }

    private void GameOver()
    {
        GameSettings.usedPoints = new List<Vector3>();

        //playerController.StopAllAction();

        UpdatePlayerStats();
        PlayerStats.currentRespawnPoint = Vector3.zero;

        GameSettings.enemies = new List<int>();

        isLevelActive = false;
        
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 2);
        StartCoroutine(StartLevel());

    }


    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
        GameSettings.enemies.Clear();
        GameSettings.usedPoints.Clear();
        Destroy(this.gameObject);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}
