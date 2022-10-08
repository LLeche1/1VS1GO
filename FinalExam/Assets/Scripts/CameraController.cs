using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float distance = 5;
    public float height = 7;
    GameManager gameManager;
    Vector3 cameraPos;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (player.transform.localScale != Vector3.zero)
        {
            cameraPos = player.transform.position - (Vector3.forward * distance) + (Vector3.up * height);
        }
        else
        {
            foreach (var player in gameManager.players)
            {
                if (player.GetComponent<PlayerController>().isDead != true)
                    cameraPos = player.transform.position - (Vector3.forward * distance) + (Vector3.up * height);
            }
        }

        gameObject.transform.position = cameraPos;
    }
}
