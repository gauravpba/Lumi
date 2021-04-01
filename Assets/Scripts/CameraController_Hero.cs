using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController_Hero : MonoBehaviour
{
    public float followSpeed = 2f;
    public float yOffSet = 2.5f;
    public float xOffSet = 0.5f;

    [SerializeField]
    private float
        maxY,
        maxX,
        minY,
        minX;

    public Transform target;

    [SerializeField]
    private GameObject border;

    // Start is called before the first frame update



    void Start()
    {
       
    }

    public void SetupCamera()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        minX = border.transform.position.x - (border.GetComponent<BoxCollider2D>().bounds.extents.x);
        maxX = border.transform.position.x + (border.GetComponent<BoxCollider2D>().bounds.extents.x);
        minY = border.transform.position.y - (border.GetComponent<BoxCollider2D>().bounds.extents.y);
        maxY = border.transform.position.y + (border.GetComponent<BoxCollider2D>().bounds.extents.y);
    }


    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 newPosition = target.position;
            newPosition.x += xOffSet * target.GetComponent<PlayerController_Hero>().GetMovementDirectionOfPlayer();
            newPosition.y += yOffSet;
            newPosition.z = -10;
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
            transform.position = Vector3.Slerp(transform.position, newPosition, followSpeed * Time.deltaTime);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(border.transform.position, border.GetComponent<BoxCollider2D>().bounds.size);
    }


}
