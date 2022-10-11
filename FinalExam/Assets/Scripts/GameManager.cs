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
    public GameObject runningGame;
    public GameObject cannonGame;
    public GameObject[] players;
    public GameObject joystick;
    public GameObject pause;
    public GameObject set;
    public TMP_Text timeText;
    private float limitTime = 300;
    private bool isGenerate = false;
    private bool isRandom = false;
    private string lastCanvas;
    PhotonView PV;
    LobbyManager lobbyManager;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
    }

    void Start()
    {

    }
    
    void Update()
    {
        if(PhotonNetwork.IsMasterClient && isRandom == false)
        {
            int random = 0;
            PV.RPC("RandomMap", RpcTarget.All, random);
        }

        if(isGenerate == false)
        {
            Generate();
        }

        players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            player.transform.GetComponent<PlayerController>().enabled = true;
        }
        LimitTime();
        LastCanvas();
        Statue();
    }

    [PunRPC]
    void RandomMap(int random)
    {
        if (random == 0)
        {
            runningGame.SetActive(true);
        }
        else if (random == 1)
        {
            if(cannonGame.activeSelf == false)
            {
                cannonGame.transform.GetComponent<CannonGame>().genAble = true;
            }
            cannonGame.SetActive(true);
        }
        isRandom = true;
    }

    void Generate()
    {
        Vector3 position = new Vector3(Random.Range(5f, 10f), 0, 4f);
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
        isRandom = false;
        runningGame.SetActive(false);
        cannonGame.SetActive(false);
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
        if(limitTime <= 0)
        {
            GivpUp();
        }
    }
}
