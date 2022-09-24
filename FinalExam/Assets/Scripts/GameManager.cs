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
    public GameObject[] players;
    public GameObject recall;
    public GameObject myHp;
    public GameObject otherHp;
    public GameObject myWagonHp;
    public GameObject otherWagonHp;
    private float time = 300;
    Wagon wagon;
    PhotonView PV;
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
        Set();
        players = GameObject.FindGameObjectsWithTag("Player");
        PV.RPC("Hp", RpcTarget.All);
        PV.RPC("Recall", RpcTarget.All);
        if (PV.IsMine)
        {
            PV.RPC("limitTime", RpcTarget.All);
        }
    }

    void Generate()
    {
        Vector3 position = new Vector3(Random.Range(-4f, 4f), 0, 0);
        GameObject player = PhotonNetwork.Instantiate(isClass.classType, position, Quaternion.identity);
        player.name = PhotonNetwork.LocalPlayer.NickName;
    }

    void Set()
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
    void Hp()
    {
        foreach (var player in players)
        {
            if (player.name == PhotonNetwork.LocalPlayer.NickName)
            {
                myHp.transform.GetChild(0).GetComponent<Image>().fillAmount = player.GetComponent<Player>().playerHp / 100;
                myHp.transform.GetChild(1).GetComponent<Text>().text = "HP " + player.GetComponent<Player>().playerHp + " / 100";
                if(player.GetComponent<Player>().myTeam == "a")
                {
                    myWagonHp.transform.GetChild(0).GetComponent<Image>().fillAmount = wagon.GetComponent<Wagon>().aTeamHp / 100;
                    otherWagonHp.transform.GetChild(0).GetComponent<Image>().fillAmount = wagon.GetComponent<Wagon>().bTeamHp / 100;
                }
                else if (player.GetComponent<Player>().myTeam == "b")
                {
                    myWagonHp.transform.GetChild(0).GetComponent<Image>().fillAmount = wagon.GetComponent<Wagon>().bTeamHp / 100;
                    otherWagonHp.transform.GetChild(0).GetComponent<Image>().fillAmount = wagon.GetComponent<Wagon>().aTeamHp / 100;
                }
            }
            else if (player.name != PhotonNetwork.LocalPlayer.NickName)
            {
                otherHp.transform.GetChild(0).GetComponent<Image>().fillAmount = player.GetComponent<Player>().playerHp / 100;
                otherHp.transform.GetChild(1).GetComponent<Text>().text = player.GetComponent<Player>().name;
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
                StartCoroutine(RecallDelay(player));
            }
        }
    }

    IEnumerator RecallDelay(GameObject player)
    {
        float delay = 5;
        player.SetActive(false);
        if(player.name == PhotonNetwork.LocalPlayer.NickName)
        {
            recall.SetActive(true);
        }
        while (delay > 0)
        {
            yield return new WaitForEndOfFrame();
            delay -= 1.0f * Time.deltaTime;
            recall.transform.GetChild(1).GetComponent<Text>().text = delay.ToString("0");
        }
        player.transform.position = new Vector3(Random.Range(-4f, 4f), player.transform.position.y, wagon.transform.position.z - 20f);
        player.GetComponent<Player>().playerHp = 100;
        player.GetComponent<Player>().isDead = false;
        player.GetComponent<Player>().isAttack = false;
        if (player.name == PhotonNetwork.LocalPlayer.NickName)
        {
            recall.SetActive(false);
        }
        player.SetActive(true);
        yield return null;
    }

    [PunRPC]
    void limitTime()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            timeText.text = TimeSpan.FromSeconds(time).ToString(@"m\:ss");
        }

        if (time <= 0)
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
