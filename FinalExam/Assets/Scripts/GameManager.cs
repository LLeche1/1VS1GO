using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public Text timeText;
    private float time = 300;
    public GameObject[] players;
    Wagon wagon;
    PhotonView PV;
    GameManager gamemanager;
    IsClass isClass;

    void Awake()
    {
        wagon = GameObject.Find("Wagon").GetComponent<Wagon>();
        PV = GetComponent<PhotonView>();
        isClass = GameObject.Find("IsClass").GetComponent<IsClass>();
    }

    void Start()
    {
        Generate();
    }

    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        SetTeam();
        PV.RPC("Recall", RpcTarget.All);
        //PV.RPC("Hp", RpcTarget.All);
        if (PV.IsMine)
        {
            PV.RPC("limitTime", RpcTarget.All);
        }
    }

    void Generate()
    {
        GameObject player = PhotonNetwork.Instantiate(isClass.classType, Vector3.zero, Quaternion.identity);
        player.name = PhotonNetwork.LocalPlayer.NickName;
    }

    void SetTeam()
    {
        foreach (var player in players)
        {
            if (PhotonNetwork.IsMasterClient == player)
            {
                player.GetComponent<Player>().myTeam = "a";
            }
            else
            {
                player.GetComponent<Player>().myTeam = "b";
            }
        }
    }

    [PunRPC]
    void Recall()
    {
        foreach (var player in players)
        {
            if (player.GetComponent<Player>().isDead == true)
            {
                Recall(player);                
            }
        }
    }

    void Recall(GameObject player)
    {
        player.active = false;
        StartCoroutine(RecallCoolTime(player));
    }
    IEnumerator RecallCoolTime(GameObject player)
    {
        yield return new WaitForSeconds(3.0f);
        player.transform.position = new Vector3(Random.Range(-4f, 4f), player.transform.position.y, wagon.transform.position.z - 35f);
        player.GetComponent<Player>().playerHp = 100;
        player.GetComponent<Player>().isDead = false;
        player.SetActive(true);
        
    }

    [PunRPC]
    void limitTime()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            timeText.text = TimeSpan.FromSeconds(time).ToString(@"m\:ss");
        }

        if(time <= 0)
        {
            SceneManager.LoadScene("Lobby");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(time);
        }
        else
        {
            time = (float)stream.ReceiveNext(); ;
        }
    }
}
