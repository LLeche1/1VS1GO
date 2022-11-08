using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpeedGame : MonoBehaviourPunCallbacks
{
    public float speed = 0;
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
        }
        else if(gameManager.isStart == false)
        {
            speed = 0;
        }
    }
}
