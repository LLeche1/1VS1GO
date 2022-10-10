using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] players;
    public GameObject joystick;
    public GameObject pause;
    public GameObject set;
    public TMP_Text timeText;
    private float limitTime = 300;
    private bool isGenerate = false;
    private string lastCanvas;
    PhotonView PV;
    LobbyManager lobbyManager;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
    }

    void Update()
    {
        if(isGenerate == false)
        {
            Generate();
        }
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            player.transform.GetComponent<PlayerController>().enabled = true;
        }
        LimitTime();
        LastCanvas();
        Statue();
    }

    void Generate()
    {
        Vector3 position = new Vector3(Random.Range(-4f, 4f), 0, 0);
        GameObject player = PhotonNetwork.Instantiate("C01", position, Quaternion.identity);
        player.name = PhotonNetwork.LocalPlayer.NickName;
        player.transform.parent = GameObject.Find("InGame").transform;
        GameObject.Find("Main Camera").transform.GetComponent<CameraController>().enabled = true;
        isGenerate = true;
    }

    void LimitTime()
    {
        if (limitTime > 0)
        {
            limitTime -= Time.deltaTime;
            timeText.text = TimeSpan.FromSeconds(limitTime).ToString(@"m\:ss");
        }
    }

    public void Pause()
    {
        pause.SetActive(true);
    }

    public void Set()
    {
        set.SetActive(true);
    }

    public void Back()
    {
        if (lastCanvas == "Pause")
        {
            pause.SetActive(false);
        }
        else if(lastCanvas == "Set")
        {
            set.SetActive(false);
        }
    }

    public void GivpUp()
    {
        Reset();
        PhotonNetwork.LeaveRoom();
    }

    void Reset()
    {
        pause.SetActive(false);
        lobbyManager.inGame.SetActive(false);
        lobbyManager.main.SetActive(true);
        gameObject.SetActive(false);
        GameObject.Find("Main Camera").transform.GetComponent<CameraController>().enabled = false;
        limitTime = 300;
        isGenerate = false;
    }

    void LastCanvas()
    {
        if (pause.activeSelf == true)
        {
            if (set.activeSelf == true)
            {
                lastCanvas = "Set";
            }
            else
            {
                lastCanvas = "Pause";
            }
        }
    }

    void Statue()
    {
        if(limitTime <= 0 || PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            GivpUp();
        }
    }
}
