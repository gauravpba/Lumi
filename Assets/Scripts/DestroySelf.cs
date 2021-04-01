using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public void InvokeDestroy()
    {
        Invoke("destroySelf", 5);
    }

    private void destroySelf()
    {
        Destroy(gameObject);
    }
}