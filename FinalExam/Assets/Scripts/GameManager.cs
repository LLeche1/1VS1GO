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
    public GameObject[] tabPlayers;
    public GameObject recall;
    public GameObject myHp;
    public GameObject otherHp;
    public GameObject myWagonHp;
    public GameObject otherWagonHp;
    public GameObject tab;
    public Sprite warriorSprite;
    public Sprite archerSprite;
    public Texture2D cursor;
    private float time = 300;
    Wagon wagon;
    PhotonView PV;
    IsClass isClass;

    private string myTeam;

    void Awake()
    {
        wagon = GameObject.Find("Wagon").GetComponent<Wagon>();
        PV = GetComponent<PhotonView>();
        isClass = GameObject.Find("IsClass").GetComponent<IsClass>();
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Start()
    {
        Generate();
    }

    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        PV.RPC("Hp", RpcTarget.All);
        PV.RPC("Recall", RpcTarget.All);
        if (PV.IsMine)
        {
            PV.RPC("limitTime", RpcTarget.All);
        }
        Tab();
    }

    void Generate()
    {
        Vector3 position = new Vector3(Random.Range(-4f, 4f), 0, 0);
        GameObject player = PhotonNetwork.Instantiate(isClass.classType, position, Quaternion.identity);
        player.name = PhotonNetwork.LocalPlayer.NickName;
        player.GetComponent<Player>().classType = isClass.classType;
        if (PhotonNetwork.IsMasterClient)
        {
            player.GetComponent<Player>().myTeam = "a";
            myTeam = "a";
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            player.GetComponent<Player>().myTeam = "b";
            myTeam = "b";
        }
    }

    [PunRPC]
    void Hp()
    {
        foreach (var player in players)
        {
            for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (player.name == PhotonNetwork.PlayerList[i].NickName && PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.LocalPlayer.NickName)
                {
                    myHp.transform.GetChild(0).GetComponent<Image>().fillAmount = player.GetComponent<Player>().playerHp / 100;
                    myHp.transform.GetChild(1).GetComponent<Text>().text = "HP " + player.GetComponent<Player>().playerHp + " / 100";
                    if (player.GetComponent<Player>().myTeam == "a")
                    {
                        myWagonHp.transform.GetChild(0).GetComponent<Image>().fillAmount = (float)wagon.aTeamHp / 100;
                        otherWagonHp.transform.GetChild(0).GetComponent<Image>().fillAmount = (float)wagon.bTeamHp / 100;
                    }
                    else if (player.GetComponent<Player>().myTeam == "b")
                    {
                        myWagonHp.transform.GetChild(0).GetComponent<Image>().fillAmount = (float)wagon.bTeamHp / 100;
                        otherWagonHp.transform.GetChild(0).GetComponent<Image>().fillAmount = (float)wagon.aTeamHp / 100;
                    }
                }
                else if (player.name != PhotonNetwork.LocalPlayer.NickName && PhotonNetwork.PlayerList[i].NickName != PhotonNetwork.LocalPlayer.NickName)
                {
                    otherHp.transform.GetChild(0).GetComponent<Image>().fillAmount = player.GetComponent<Player>().playerHp / 100;
                    otherHp.transform.GetChild(1).GetComponent<Text>().text = PhotonNetwork.PlayerList[i].NickName;
                }
            }
        }
    }

    void Tab()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tab.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            tab.SetActive(false);
        }

        foreach (var player in players)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (player.name == PhotonNetwork.PlayerList[i].NickName && PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.LocalPlayer.NickName)
                {
                    if (player.layer == 6)
                    {
                        tabPlayers[0].transform.GetChild(0).GetComponent<Image>().sprite = warriorSprite;
                    }
                    else if (player.layer == 7)
                    {
                        tabPlayers[0].transform.GetChild(0).GetComponent<Image>().sprite = archerSprite;
                    }
                    tabPlayers[0].transform.GetChild(1).GetComponent<Text>().text = PhotonNetwork.PlayerList[i].NickName;
                }
                else if (player.name != PhotonNetwork.LocalPlayer.NickName && PhotonNetwork.PlayerList[i].NickName != PhotonNetwork.LocalPlayer.NickName)
                {
                    if (player.layer == 6)
                    {
                        tabPlayers[1].transform.GetChild(0).GetComponent<Image>().sprite = warriorSprite;
                    }
                    else if (player.layer == 7)
                    {
                        tabPlayers[1].transform.GetChild(0).GetComponent<Image>().sprite = archerSprite;
                    }
                    tabPlayers[1].transform.GetChild(1).GetComponent<Text>().text = PhotonNetwork.PlayerList[i].NickName;
                }
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
