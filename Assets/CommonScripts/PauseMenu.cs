using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
   
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public LevelManager manager;


   private static PauseMenu _instance;

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

    private void Update()
    {
        if(Input.GetButtonDown("Pause") && SceneManager.GetActiveScene().name != "EndScene")
        {
            if(GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        GameIsPaused = false;

    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        GameIsPaused = true;
        manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<LevelManager>();

    }
    public void LoadMenu()
    {
        //manager.LevelEnd();
        GameSettings.usedPoints.Clear();
        GameSettings.enemies.Clear();
        Destroy(manager.gameObject);
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
        
    }
    public void QuitGame()
    {

    }
}
