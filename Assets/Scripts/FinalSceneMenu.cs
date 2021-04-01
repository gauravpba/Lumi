using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalSceneMenu : MonoBehaviour
{
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
        GameSettings.enemies.Clear();
        GameSettings.usedPoints.Clear();
        Destroy(GameObject.FindGameObjectWithTag("Game Manager"));
        //Destroy(this.gameObject);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}
