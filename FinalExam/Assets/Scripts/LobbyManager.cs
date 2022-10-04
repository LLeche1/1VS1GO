using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject title;
    public GameObject titleLoading;
    public GameObject login;
    public TMP_InputField loginID;
    public TMP_InputField loginPW;
    public Button loginBtn;
    public Toggle loginRememberMe;
    public GameObject lobby;
    public TMP_Text lobbyLevel;
    public TMP_Text lobbyName;
    public GameObject lobbySet;
    public GameObject lobbyChat;
    public TMP_Text[] lobbyChatText;
    public TMP_InputField lobbyChatInput;
    public GameObject error;
    public TMP_Text errorInfo;
    public GameObject errorNetwork;
    public GameObject room;
    public TMP_Text roomPlayer;
    public GameObject roomCount;
    public TMP_Text roomCountText;

    public Button lobbyPreviousBtn;
    public Button lobbyNextBtn;
    public Button[] lobbyRoomListBtn;
    public Text roomTitle;

    public GameObject roomStartBtn;
    public GameObject roomReadyBtn;
    public GameObject roomOutBtn;
    public GameObject roomShadow;
    public Image[] roomPlayerList;
    public Button roomClassSelectBtn;
    public Button[] classListBtn;
    public InputField createRoomNum;
    public Button setBtn;
    public GraphicRaycaster graphicRaycaster;



    public GameObject Class;
    public GameObject createRoom;
    public GameObject set;
    public GameObject result;
    public Texture2D cursor;
    public string classType;
    private string gameVersion = "1";
    private string lastCanvas;
    private string errorType;
    private int multiple;
    private bool isReady = false;
    private bool isReadyRpc = false;
    List<RoomInfo> roomList = new List<RoomInfo>();
    PhotonView PV;

    void Awake()
    {
        Application.targetFrameRate = 144;
        PhotonNetwork.AutomaticallySyncScene = true;
        PV = GetComponent<PhotonView>();
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Start()
    {
        LoginLoad(loginRememberMe);
        PhotonNetwork.GameVersion = gameVersion;
    }

    public void Title_Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        titleLoading.SetActive(true);
        titleLoading.GetComponent<Slider>().value = 0.5f;
        titleLoading.transform.GetChild(1).GetComponent<TMP_Text>().text = "50%";
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("온라인");
        titleLoading.GetComponent<Slider>().value = 1f;
        titleLoading.transform.GetChild(1).GetComponent<TMP_Text>().text = "100%";
        titleLoading.SetActive(false);
        if(lastCanvas != "lobby")
        {
            login.SetActive(true);
        }
        loginBtn.interactable = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("오프라인");
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Login()
    {
        string id = loginID.text;
        string pw = loginPW.text;
        if (id != "" && pw != "")
        {
            loginBtn.interactable = false;

            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("로비");
                PhotonNetwork.JoinLobby();
                PhotonNetwork.LocalPlayer.NickName = id;
            }
            else
            {
                Debug.Log("오프라인");
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            login.SetActive(false);
            error.SetActive(true);
            errorInfo.text = "You have entered an incorrect username or password. ";
            errorType = "login";
        }
    }

    public override void OnJoinedLobby()
    {
        title.SetActive(false);
        login.SetActive(false);
        lobby.SetActive(true);
        lobbyLevel.text = 1.ToString();
        lobbyName.text = PhotonNetwork.LocalPlayer.NickName;
    }

    public void LobbySet()
    {
        lobbySet.SetActive(true);
    }

    public void LobbyChat()
    {
        lobbyChat.SetActive(true);
    }

    public void LobbyStart()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        room.SetActive(true);
        RoomRenewal();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        RoomRenewal();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        RoomRenewal();
    }

    void RoomRenewal()
    {
        roomPlayer.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers && PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(Delay());
            PhotonNetwork.LoadLevel("InGame");
        }
    }

    IEnumerator Delay()
    {
        float time = 3;
        while (0.0f < time)
        {
            yield return new WaitForEndOfFrame();
            time -= 1.0f * Time.deltaTime;
            PV.RPC("RoomCountdownRpc", RpcTarget.All, time);
        }
        yield return null;
    }

    [PunRPC]
    public void RoomCountdownRpc(float time)
    {
        roomCount.SetActive(true);
        roomCountText.text = time.ToString("0");
    }

    public void RoomOut()
    {
        PhotonNetwork.LeaveRoom();
        room.SetActive(false);
    }

    public void LoginMemory(Toggle toggle)
    {
        if (toggle.isOn)
        {
            PlayerPrefs.SetString("ID", loginID.text);
            PlayerPrefs.SetString("PW", loginPW.text);
            PlayerPrefs.SetInt("IsOn", 1);
        }
        else
        {
            PlayerPrefs.DeleteAll();
        }

    }

    public void Back()
    {
        if(lastCanvas == "title")
        {
            login.SetActive(false);
            title.SetActive(true);
        }
        else if(lastCanvas == "error")
        {
            if(errorType == "login")
            {
                error.SetActive(false);
                login.SetActive(true);
            }
        }
        else if (lastCanvas == "lobbySet")
        {
            lobbySet.SetActive(false);
        }
        else if (lastCanvas == "lobbyChat")
        {
            lobbyChat.SetActive(false);
        }
        Debug.Log(lastCanvas);
    }

    void LastCanvas()
    {
        if(title.activeSelf == true && error.activeSelf == false)
        {
            lastCanvas = "title";
        }
        else if (login.activeSelf == true)
        {
            lastCanvas = "login";
        }
        else if (title.activeSelf == true && error.activeSelf == true)
        {
            lastCanvas = "error";
        }
        else if (lobby.activeSelf == true && lobbySet.activeSelf == false && lobbyChat.activeSelf == false && room.activeSelf == false)
        {
            lastCanvas = "lobby";
        }
        else if (lobby.activeSelf == true && lobbySet.activeSelf == true)
        {
            lastCanvas = "lobbySet";
        }
        else if (lobby.activeSelf == true && lobbyChat.activeSelf == true)
        {
            lastCanvas = "lobbyChat";
        }
    }

    void LoginLoad(Toggle toggle)
    {
        loginID.text = PlayerPrefs.GetString("ID");
        loginPW.text = PlayerPrefs.GetString("PW");
        if (PlayerPrefs.GetInt("IsOn") == 1)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }
    }

    public void LobbyChatInput()
    {
        if(lobbyChatInput.text != "")
        {
            PV.RPC("LobbyChatRpc", RpcTarget.All, lobbyChatInput.text);
        }
        lobbyChatInput.text = "";
        lobbyChatInput.ActivateInputField();
    }

    [PunRPC]
    void LobbyChatRpc(string text)
    {
        bool isInput = false;
        for (int i = 0; i < lobbyChatText.Length; i++)
        {
            if (lobbyChatText[i].text == "")
            {
                isInput = true;
                lobbyChatText[i].text = text;
                break;
            }
        }
        if (!isInput)
        {
            for (int i = 1; i < lobbyChatText.Length; i++) lobbyChatText[i - 1].text = lobbyChatText[i].text;
            lobbyChatText[lobbyChatText.Length - 1].text = text;
        }
    }

    void LobbyChatEnter()
    {
        if (lobbyChat.activeSelf == true && Input.GetKeyDown(KeyCode.Return))
        {
            LobbyChatInput();
        }
    }

    void Update()
    {
        LastCanvas();
        LobbyChatEnter();
    }
}