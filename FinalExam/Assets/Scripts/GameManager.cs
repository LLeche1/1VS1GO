using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

enum TeamType
{
    oneTeam,
    twoTeam
}
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager Instance = null;
    private float time = 300;
    public Image oneTeamHp;
    public Image twoTeamHp;

    public Text timeText;
    private Wagon wagon;
    private PhotonView PV;
    private string[] playerType = { "Archer", "Warriou" };

    private int randNum;
    private GameObject[] players;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
        wagon = GameObject.Find("Wagon").GetComponent<Wagon>();

        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        randNum = UnityEngine.Random.Range(0, playerType.Length);
        Generate();
    }

    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        if (PV.IsMine)
        {
            PV.RPC("limitTime", RpcTarget.All);
        }
        SetTeam(); //룸에서 팀을 정하고 인게임으로 들어가야함
        AdjustHpUI(); //방장이 나가면 마스터클라이언트가 바뀌어서 다시 위치가 바뀌는걸 방지해야함 (아마 룸에서 팀을 정하고 시작하면 해결될듯)

        PV.RPC("Hp", RpcTarget.All);
        Recall();

    }

    void Generate()
    {
        GameObject player = PhotonNetwork.Instantiate(playerType[1], Vector3.zero, Quaternion.identity);
        player.name = PhotonNetwork.LocalPlayer.NickName;
    }
    public void Recall(GameObject player)
    {
        string characterType = player.transform.GetComponent<Player>().characterType;
        GameObject p = PhotonNetwork.Instantiate(characterType, Vector3.zero, Quaternion.identity);
        p.name = player.name;
    }

    void Recall()
    {

        
        foreach (var player in players)
        {
            if (player.GetComponent<Player>().isDead == true)
            {
                Recall(player);
                PhotonNetwork.Destroy(player);
            }
        }
    }
    void SetTeam()
    {
        foreach (var player in players)
        {
            if (PhotonNetwork.IsMasterClient == player)
            {
                player.GetComponent<Player>().myTeam = 0;
            }
            else
            {
                player.GetComponent<Player>().myTeam = 1;
            }
        }
    }

    void AdjustHpUI()
    {
        foreach (var player in players)
        {
            if (player.GetComponent<Player>().myTeam == (int)TeamType.oneTeam)
            {
                oneTeamHp.rectTransform.anchorMax = new Vector2(0, 1);
                oneTeamHp.rectTransform.anchorMin = new Vector2(0, 1);
                oneTeamHp.rectTransform.pivot = new Vector2(0, 1);

                twoTeamHp.rectTransform.anchorMax = new Vector2(1, 1);
                twoTeamHp.rectTransform.anchorMin = new Vector2(1, 1);
                twoTeamHp.rectTransform.pivot = new Vector2(1, 1);

                oneTeamHp.rectTransform.anchoredPosition = new Vector3(75, -5, 0);
                twoTeamHp.rectTransform.anchoredPosition = new Vector3(-75, -5, 0);
                Debug.Log("Yes");
            }
            else if (player.GetComponent<Player>().myTeam == (int)TeamType.twoTeam)
            {
                Debug.Log("NO");
                oneTeamHp.rectTransform.anchorMax = new Vector2(1, 1);
                oneTeamHp.rectTransform.anchorMin = new Vector2(1, 1);
                oneTeamHp.rectTransform.pivot = new Vector2(1, 1);


                twoTeamHp.rectTransform.anchorMax = new Vector2(0, 1);
                twoTeamHp.rectTransform.anchorMin = new Vector2(0, 1);
                twoTeamHp.rectTransform.pivot = new Vector2(0, 1);

                oneTeamHp.rectTransform.anchoredPosition = new Vector3(-275, -5, 0);
                twoTeamHp.rectTransform.anchoredPosition = new Vector3(75, -5, 0);
            }
        }
    }
    [PunRPC]
    void Hp()
    {
        oneTeamHp.fillAmount = wagon.oneTeamHP * 0.01f;
        twoTeamHp.fillAmount = wagon.twoTeamHP * 0.01f;
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
