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
    public int aKill;
    public int bKill;
    public int aDeath;
    public int bDeath;
    public string myTeam = "";
    public Text timeText;
    public GameObject[] players;
    public GameObject[] tabPlayers;
    public GameObject[] killLogs;
    public GameObject recall;
    public GameObject myHp;
    public GameObject otherHp;
    public GameObject myWagonHp;
    public GameObject otherWagonHp;
    public GameObject tab;
    public GameObject gameOver;
    public Sprite warriorSprite;
    public Sprite archerSprite;
    public Texture2D cursor;
    private float time = 300;
    private bool recallTrigger = true;
    Wagon wagon;
    PhotonView PV;
    Data data;

    public GameObject myCircle;
    public GameObject otherCircle;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        wagon = GameObject.Find("Wagon").GetComponent<Wagon>();
        PV = GetComponent<PhotonView>();
        data = GameObject.Find("Data").GetComponent<Data>();
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined;
        PhotonNetwork.ConnectUsingSettings();
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
        Set();
        KillLog();
        State();
        Circle();
    }

    void State()
    {
        if(wagon.aTeamHp <= 0)
        {
            gameOver.SetActive(true);
            if (myTeam == "a")
            {
                gameOver.transform.GetChild(2).gameObject.SetActive(true);
            }
            else if (myTeam == "b")
            {
                gameOver.transform.GetChild(1).gameObject.SetActive(true);
            }
            StartCoroutine("StateDelay");
        }
        else if (wagon.bTeamHp <= 0)
        {
            gameOver.SetActive(true);
            if (myTeam == "a")
            {
                gameOver.transform.GetChild(1).gameObject.SetActive(true);
            }
            else if (myTeam == "b")
            {
                gameOver.transform.GetChild(2).gameObject.SetActive(true);
            }
            StartCoroutine("StateDelay");
        }
        else if(time <= 0)
        {
            gameOver.SetActive(true);
            if(wagon.aTeamHp > wagon.bTeamHp)
            {
                if(myTeam == "a")
                {
                    gameOver.transform.GetChild(1).gameObject.SetActive(true);
                }
                else if (myTeam == "b")
                {
                    gameOver.transform.GetChild(2).gameObject.SetActive(true);
                }
            }
            else if (wagon.aTeamHp < wagon.bTeamHp)
            {
                if (myTeam == "a")
                {
                    gameOver.transform.GetChild(2).gameObject.SetActive(true);
                }
                else if (myTeam == "b")
                {
                    gameOver.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            else if (wagon.aTeamHp == wagon.bTeamHp)
            {
                gameOver.transform.GetChild(3).gameObject.SetActive(true);
            }
            StartCoroutine("StateDelay");
        }
    }

    IEnumerator StateDelay()
    {
        float delay = 5;
        while (delay > 0)
        {
            yield return new WaitForEndOfFrame();
            delay -= 1.0f * Time.deltaTime;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
        data.lastScene = "InGame";
        yield return null;
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

    void KillLog()
    {
        GameObject kill = null;
        GameObject death = null;

        foreach (var player in players)
        {
            if (player.GetComponent<Player>().isDead == true)
            {
                foreach (var player2 in players)
                {
                    if (player2.GetComponent<Player>().isDead == true)
                    {
                        death = player2;
                    }
                    else if (player2.GetComponent<Player>().isDead != true)
                    {
                        kill = player2;
                    }
                }

                killLogs[0].SetActive(true);

                if (player.GetComponent<Player>().myTeam == myTeam)
                {
                    killLogs[0].transform.GetChild(0).GetComponent<Image>().color = Color.red;
                }
                else if (player.GetComponent<Player>().myTeam != myTeam)
                {
                    killLogs[0].transform.GetChild(0).GetComponent<Image>().color = Color.blue;
                }

                if (kill.GetComponent<Player>().classType == "Warrior")
                {
                    killLogs[0].transform.GetChild(1).GetComponent<Image>().sprite = warriorSprite;
                }
                else if (kill.GetComponent<Player>().classType == "Archer")
                {
                    killLogs[0].transform.GetChild(1).GetComponent<Image>().sprite = archerSprite;
                }

                if (death.GetComponent<Player>().classType == "Warrior")
                {
                    killLogs[0].transform.GetChild(3).GetComponent<Image>().sprite = warriorSprite;
                }
                else if (death.GetComponent<Player>().classType == "Archer")
                {
                    killLogs[0].transform.GetChild(3).GetComponent<Image>().sprite = archerSprite;
                }
                StartCoroutine("KillLogDelay");
            }
        }
    }

    IEnumerator KillLogDelay()
    {
        float delay = 5;
        while (delay > 0)
        {
            yield return new WaitForEndOfFrame();
            delay -= 1.0f * Time.deltaTime;
        }
        killLogs[0].SetActive(false);
        yield return null;
    }

    void Set()
    {
        foreach (var player in players)
        {
            if (player.name == PhotonNetwork.LocalPlayer.NickName)
            {
                if (player.GetComponent<Player>().myTeam == "a")
                {
                    myTeam = "a";
                }
                else if (player.GetComponent<Player>().myTeam == "b")
                {
                    myTeam = "b";
                }
            }
        }

        foreach (var player in players)
        {
            if (player.name != PhotonNetwork.LocalPlayer.NickName)
            {
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    if (PhotonNetwork.PlayerList[i].NickName != PhotonNetwork.LocalPlayer.NickName)
                    {
                        player.name = PhotonNetwork.PlayerList[i].NickName;
                    }
                }

                if(player.layer == 6)
                {
                    player.GetComponent<Player>().classType = "Warrior";
                }
                else if (player.layer == 7)
                {
                    player.GetComponent<Player>().classType = "Archer";
                }

                if(myTeam == "a")
                {
                    player.GetComponent<Player>().myTeam = "b";
                }
                else if (myTeam == "b")
                {
                    player.GetComponent<Player>().myTeam = "a";
                }

            }
        }
    }

    void Generate()
    {
        Vector3 position = new Vector3(Random.Range(-4f, 4f), 0, 0);
        GameObject player = PhotonNetwork.Instantiate(data.classType, position, Quaternion.identity);
        player.name = PhotonNetwork.LocalPlayer.NickName;
        player.GetComponent<Player>().classType = data.classType;
        if (PhotonNetwork.IsMasterClient)
        {
            player.GetComponent<Player>().myTeam = "a";
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            player.GetComponent<Player>().myTeam = "b";
        }
    }

    void Circle()
    {
        foreach (var player in players)
        {
            if (player.name == PhotonNetwork.LocalPlayer.NickName)
            {
                myCircle.transform.position = new Vector3(player.transform.position.x, 0.6f, player.transform.position.z);
                if (player.GetComponent<Player>().isDead == false)
                {
                    myCircle.SetActive(true);
                }
                else if (player.GetComponent<Player>().isDead == true)
                {
                    myCircle.SetActive(false);
                }
            }
            else if (player.name != PhotonNetwork.LocalPlayer.NickName)
            {
                otherCircle.transform.position = new Vector3(player.transform.position.x, 0.6f, player.transform.position.z);
                otherCircle.GetComponent<SpriteRenderer>().color = Color.red;
                if (player.GetComponent<Player>().isDead == false)
                {
                    otherCircle.SetActive(true);
                }
                else if (player.GetComponent<Player>().isDead == true)
                {
                    otherCircle.SetActive(false);
                }
            }
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

                    if(player.GetComponent<Player>().myTeam == "a")
                    {
                        tabPlayers[0].transform.GetChild(2).GetComponent<Text>().text = aKill.ToString() + " / " + aDeath.ToString();
                    }
                    else if (player.GetComponent<Player>().myTeam == "b")
                    {
                        tabPlayers[0].transform.GetChild(2).GetComponent<Text>().text = bKill.ToString() + " / " + bDeath.ToString();
                    }
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

                    if (player.GetComponent<Player>().myTeam == "a")
                    {
                        tabPlayers[1].transform.GetChild(2).GetComponent<Text>().text = aKill.ToString() + " / " + aDeath.ToString();
                    }
                    else if (player.GetComponent<Player>().myTeam == "b")
                    {
                        tabPlayers[1].transform.GetChild(2).GetComponent<Text>().text = bKill.ToString() + " / " + bDeath.ToString();
                    }
                }
            }
        }
    }

    [PunRPC]
    void Recall()
    {
        foreach (var player in players)
        {
            if (player.GetComponent<Player>().isDead == true && recallTrigger)
            {
                recallTrigger = false;
                StartCoroutine(RecallDelay(player));
            }
        }
    }

    IEnumerator RecallDelay(GameObject player)
    {
        float delay = 5;
        player.transform.localScale = Vector3.zero;
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
        player.transform.position = new Vector3(0, player.transform.position.y, wagon.transform.position.z - 20f);
        player.GetComponent<Player>().playerHp = 100;
        player.GetComponent<Player>().isDead = false;
        player.GetComponent<Player>().isAttack = false;
        if (player.name == PhotonNetwork.LocalPlayer.NickName)
        {
            recall.SetActive(false);
        }
        recallTrigger = true;
        player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        player.transform.GetChild(5).gameObject.GetComponentInChildren<ParticleSystem>().Play();
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
