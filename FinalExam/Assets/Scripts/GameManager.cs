using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
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
    public AudioMixer audioMixer;
    public Slider set_Fx;
    public Slider set_Music;
    public GameObject set_Vibration_Yes;
    public GameObject set_Vibration_No;
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

    void Update()
    {
        if(PhotonNetwork.IsMasterClient && isRandom == false)
        {
            int random = Random.Range(0, 2);
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
                cannonGame.transform.GetComponent<CannonGame>().randGenTrigger = true;
                cannonGame.transform.GetComponent<CannonGame>().lineGenTrigger = true;
            }
            cannonGame.SetActive(true);
        }
        isRandom = true;
    }

    void Generate()
    {
        Vector3 position = new Vector3(Random.Range(5f, 10f), 0, 4f);
        GameObject player = PhotonNetwork.Instantiate("C01", position, Quaternion.identity);
        player.name = lobbyManager.nickName;
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
        set_Fx.value = lobbyManager.fxValue;
        set_Music.value = lobbyManager.musicValue;
        if (lobbyManager.isVibration == 0)
        {
            set_Vibration_Yes.SetActive(false);
            set_Vibration_No.SetActive(true);
        }
        else if (lobbyManager.isVibration == 1)
        {
            set_Vibration_Yes.SetActive(true);
            set_Vibration_No.SetActive(false);
        }
        set.SetActive(true);
    }

    public void Set_Fx()
    {
        audioMixer.SetFloat("FX", Mathf.Log10(set_Fx.value) * 20);
        PlayerPrefs.SetFloat("fxValue", set_Fx.value);
        lobbyManager.fxValue = PlayerPrefs.GetFloat("fxValue");
    }

    public void Set_Music()
    {
        audioMixer.SetFloat("Music", Mathf.Log10(set_Music.value) * 20);
        PlayerPrefs.SetFloat("musicValue", set_Music.value);
        lobbyManager.musicValue = PlayerPrefs.GetFloat("musicValue");
    }

    public void Set_Vibration()
    {
        if (lobbyManager.isVibration == 0)
        {
            set_Vibration_Yes.SetActive(true);
            set_Vibration_No.SetActive(false);
            lobbyManager.isVibration = 1;
            PlayerPrefs.SetInt("isVibration", lobbyManager.isVibration);
            lobbyManager.isVibration = PlayerPrefs.GetInt("isVibration");
        }
        else if (lobbyManager.isVibration == 1)
        {
            set_Vibration_Yes.SetActive(false);
            set_Vibration_No.SetActive(true);
            lobbyManager.isVibration = 0;
            PlayerPrefs.SetInt("isVibration", lobbyManager.isVibration);
            lobbyManager.isVibration = PlayerPrefs.GetInt("isVibration");
        }
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

    public void GiveUp()
    {
        Reset();
        PhotonNetwork.LeaveRoom();
    }

    void Reset()
    {
        var child = cannonGame.transform.GetChild(1).GetComponentsInChildren<Transform>();

        foreach (var item in child)
        {
            Debug.Log(item);
            if (item.name != "Cannons")
            {
                Destroy(item.gameObject);
            }
        }
        pause.SetActive(false);
        set.SetActive(false);
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
            GiveUp();
        }

        foreach(GameObject player in players)
        {
            if(player.transform.position.y < -30)
            {
                GiveUp();
            }
        }
    }
}
