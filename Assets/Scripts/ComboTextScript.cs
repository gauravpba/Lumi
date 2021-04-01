using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboTextScript : MonoBehaviour
{
    [SerializeField]
    private float floatSpeed;

    TextMeshPro textComponent;

    private void Start()
    {
        textComponent = GetComponent<TextMeshPro>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        Color textColor = textComponent.color;


        textColor.a -= Time.deltaTime;


        textComponent.color = textColor;
        if(textColor.a <0.1f)
        {
            Destroy(gameObject);
        }

    }
}
