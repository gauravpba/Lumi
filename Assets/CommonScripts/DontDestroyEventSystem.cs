using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyEventSystem : MonoBehaviour
{
    private static DontDestroyEventSystem _instance;

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

}
