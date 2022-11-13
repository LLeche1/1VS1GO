using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpeedGame : MonoBehaviourPunCallbacks
{
    public float speed = 0;
    public GameObject[] players;
    GameManager gameManager;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if(gameManager.isStart == true)
        {
            if(speed > 0)
            {
                speed -= Time.deltaTime * 5;
            }
            else if(speed <= 0)
            {
                speed = 0;
            }

            players = GameObject.FindGameObjectsWithTag("Player");

            foreach(GameObject player in players)
            {
                if(player.transform.position.z > 460)
                {
                    player.transform.position = new Vector3(player.transform.position.x, 0, 460);
                }
            }
        }
        else if(gameManager.isStart == false)
        {
            speed = 0;
        }
    }
}
