using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] players;
    public GameObject joystick;
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        Generate();
    }

    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            if (player.name == PhotonNetwork.LocalPlayer.NickName)
            {
                joystick.GetComponent<JoyStick>().player = player;
            }
        }
    }

    void Generate()
    {
        Vector3 position = new Vector3(Random.Range(-4f, 4f), 0, 0);
        GameObject player = PhotonNetwork.Instantiate("C01", position, Quaternion.identity);
        player.name = PhotonNetwork.LocalPlayer.NickName;
    }
}
