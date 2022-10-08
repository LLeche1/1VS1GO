using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] players;
    public GameObject joystick;
    public GameObject pause;
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        GameObject.Find("Main Camera").transform.GetComponent<CameraController>().enabled = true;
    }

    void Start()
    {
        Generate();
    }

    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            player.transform.GetComponent<PlayerController>().enabled = true;
        }
    }

    void Generate()
    {
        Vector3 position = new Vector3(Random.Range(-4f, 4f), 0, 0);
        GameObject player = PhotonNetwork.Instantiate("C01", position, Quaternion.identity);
        player.name = PhotonNetwork.LocalPlayer.NickName;
        player.transform.parent = GameObject.Find("InGame").transform;
    }

    public void Pause()
    {
        pause.SetActive(true);
    }

    public void Back()
    {
        pause.SetActive(false);
    }

    public void GivpUp()
    {
        
    }
}
