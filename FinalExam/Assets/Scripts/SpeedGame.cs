using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpeedGame : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    GameManager gameManager;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && gameManager.isStart == true)
        {
        }
    }
}
