using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    public static int difficultyLevel;
    public static float volumeLevel = 1;
    public static bool musicMuted = false;

    public static float playerMaxHealth;
    private static float playerHealthEasy = 20;
    private static float playerHealthNormal = 16;
    private static float playerHealthHard = 12;
    public static int EnemyCount = 0;
    public static int darkEnemyCount = 0;
    public static List<int> enemies = new List<int>();
    public static List<Vector3> usedPoints = new List<Vector3>();


    public static void GetSettings()
    {
        switch (difficultyLevel)
        {
            case 0:
                playerMaxHealth = playerHealthEasy;
                EnemyCount = 3;
                break;
            case 1:
                playerMaxHealth = playerHealthNormal;
                EnemyCount = 5;
                break;
            case 2:
                playerMaxHealth = playerHealthHard;
                EnemyCount = 8;
                break;
            default:
                playerMaxHealth = playerHealthHard;
                EnemyCount = 8;
                break;
        }

    }

    public static void GetLumiSettings()
    {
        switch (difficultyLevel)
        {
            case 0:
                darkEnemyCount = 3;
                break;
            case 1:
                darkEnemyCount = 5;
                break;
            case 2:
                darkEnemyCount = 8;
                break;
            default:
                darkEnemyCount = 8;
                break;
        }
    }

}
