using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float distance;
    public float height;
    GameManager gameManager;
    Vector3 cameraPos;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if(gameManager.isStart == true)
        {
            if(gameManager.random == 4)
            {
                gameObject.transform.rotation = Quaternion.Euler(12,0,0);
            }
            else
            {
                gameObject.transform.rotation = Quaternion.Euler(30,0,0);
            }
            
            gameObject.transform.position = player.transform.position - (Vector3.forward * distance) + (Vector3.up * height);
        }

    }
}
