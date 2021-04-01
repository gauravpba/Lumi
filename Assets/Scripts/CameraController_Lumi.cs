using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController_Lumi : MonoBehaviour
{
    public float followSpeed = 2f;
    public float yOffSet = 2.5f;
    public float xOffSet = 0.5f;
    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = player.position;
        newPosition.x += xOffSet * player.GetComponent<PlayerController_Lumi>().GetMovementDirectionOfPlayer();
        newPosition.y += yOffSet;
        newPosition.z = -10;
        transform.position = Vector3.Slerp(transform.position, newPosition, followSpeed * Time.deltaTime);
    }
}
