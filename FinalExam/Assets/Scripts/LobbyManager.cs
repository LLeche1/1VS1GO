using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    public InputField loginID;
    public InputField loginPW;
    public InputField createRoomNum;
    public Text loginInfo;
    public Text roomTitle;
    public Text roomCurrentPlayer;
    public Text roomCountdown;
    public Button setBtn;
    public Button loginBtn;
    public GameObject roomStartBtn;
    public GameObject roomReadyBtn;
    public Button roomStartBtn2;
    public Button roomReadyBtn2;
    public Button lobbyPreviousBtn;
    public Button lobbyNextBtn;
    public Button[] LobbyRoomListBtn;
    public Button[] classListBtn;
    public Image[] roomPlayerList;
    public GameObject login;
    public GameObject loginError;
    public GameObject lobby;
    public GameObject room;
    public GameObject Class;
    public GameObject createRoom;
    public GameObject createRoomError;
    public GameObject set;
    public Toggle loginMemory;
    private PhotonView PV;
    private string gameVersion = "1";
    private string lastCanvas;
    private string[] characterType;
    private int currentPage = 1;
    private int maxPage;
    private int multiple;
    private float delay = 10;
    private bool isReady = false;
    private bool isReadyRpc = false;
    private bool isStart = false;
    List<RoomInfo> roomList = new List<RoomInfo>();
    GameManager gameManager;

    void Awake()
    {
        Application.targetFrameRate = 144;
        Screen.SetResolution(1600, 900, false);
        PhotonNetwork.AutomaticallySyncScene = true;
        PV = GetComponent<PhotonView>();
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Load(loginMemory);
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        loginInfo.text = "서버에 접속중...";
    }

    public override void OnConnectedToMaster()
    {
        loginBtn.interactable = true;
        loginInfo.text = "온라인";
        if(lobby.activeSelf == true)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        loginInfo.text = "오프라인 : 서버와 연결되지 않음\n접속 재시도 중...";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        string id = loginID.text;
        string pw = loginPW.text;
        if (id != "" && pw != "")
        {
            loginBtn.interactable = false;

            if (PhotonNetwork.IsConnected)
            {
                loginInfo.text = "매칭 중...";
                PhotonNetwork.JoinLobby();
                PhotonNetwork.LocalPlayer.NickName = id;
            }
            else
            {
                loginInfo.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            login.SetActive(false);
            loginError.SetActive(true);
        }
    }

    public override void OnJoinedLobby()
    {
        login.SetActive(false);
        lobby.SetActive(true);
        roomList.Clear();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoomCreateBtn();
    }

    public override void OnJoinedRoom()
    {
        lobby.SetActive(false);
        createRoom.SetActive(false);
        room.SetActive(true);
        RoomRenewal();
        if (photonView.IsMine)
        {
            roomStartBtn.SetActive(true);
        }
        else
        {
            roomReadyBtn.SetActive(true);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        createRoom.SetActive(false);
        createRoomError.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        RoomRenewal();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        RoomRenewal();
        ResetReady();
    }

    public override void OnRoomListUpdate(List<RoomInfo> room)
    {
        int roomCount = room.Count;
        for(int i = 0; i < roomCount; i++)
        {
            if (!room[i].RemovedFromList)
            {
                if (!roomList.Contains(room[i]))
                {
                    roomList.Add(room[i]);
                }
                else
                {
                    roomList[roomList.IndexOf(room[i])] = room[i];
                }
            }
            else if (roomList.IndexOf(room[i]) != -1)
            {
                roomList.RemoveAt(roomList.IndexOf(room[i]));
            }
        }
        roomListRenewal();
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {
            if (isReady == true)
            {
                ResetReady();
            }
            roomStartBtn.SetActive(true);
            roomReadyBtn.SetActive(false);
        }
    }

    public void LobbyOutBtn()
    {
        PhotonNetwork.Disconnect();
        lobby.SetActive(false);
        login.SetActive(true);
    }

    public void RoomListBtn(int num)
    {
        if (num == -2)
        {
            --currentPage;
        }
        else if(num == -1)
        {
            ++currentPage;
        }
        else
        {
            PhotonNetwork.JoinRoom(roomList[multiple + num].Name);
        }
        roomListRenewal();
    }

    public void LobbyCreateBtn()
    {
        lobby.SetActive(false);
        createRoom.SetActive(true);
    }

    public void CreateRoomCreateBtn()
    {
        PhotonNetwork.CreateRoom(createRoomNum.text == "" ? Random.Range(0, 100) + "번" : createRoomNum.text + "번", new RoomOptions { MaxPlayers = 2 });
    }

    public void LobbyFastJoinBtn()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void StartBtn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            /*if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                PhotonNetwork.LoadLevel("InGame 1");
            }
            */
            StartCoroutine(Delay());
            PhotonNetwork.CurrentRoom.IsVisible = false;
            //PhotonNetwork.LoadLevel("InGame");
        }
    }

    public void ReadyBtn()
    {
        if(isReady == false)
        {
            isReady = true;
            roomReadyBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        }
        else if (isReady == true)
        {
            isReady = false;
            roomReadyBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        PV.RPC("ReadyRpc", RpcTarget.All, isReady);
    }

    [PunRPC]
    public void ReadyRpc(bool isReady)
    {
        isReadyRpc = isReady;

        if (isReadyRpc == true)
        {
            roomStartBtn2.interactable = true;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.MasterClient.NickName != PhotonNetwork.PlayerList[i].NickName)
                {
                    roomPlayerList[i].transform.GetChild(2).GetComponent<Text>().text = "Ready";
                }
            }
        }
        else if (isReadyRpc == false)
        {
            roomStartBtn2.interactable = false;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.MasterClient.NickName != PhotonNetwork.PlayerList[i].NickName)
                {
                    roomPlayerList[i].transform.GetChild(2).GetComponent<Text>().text = "";
                }
            }
        }
    }

    public void Memory(Toggle toggle)
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

    public void RoomOut()
    {
        ResetReady();
        roomStartBtn2.interactable = false;
        if (roomStartBtn.activeSelf == true)
        {
            roomStartBtn.SetActive(false);
        }
        else if(roomReadyBtn.activeSelf == true)
        {
            roomReadyBtn.SetActive(false);
        }
        PhotonNetwork.LeaveRoom();
        room.SetActive(false);
        lobby.SetActive(true);
    }

    public void Back()
    {
        if(lastCanvas == "loginError")
        {
            loginError.SetActive(false);
            login.SetActive(true);
        }
        else if (lastCanvas == "createRoom")
        {
            createRoom.SetActive(false);
            lobby.SetActive(true);
        }
        else if (lastCanvas == "createRoomError")
        {
            createRoomError.SetActive(false);
            createRoom.SetActive(true);
        }
    }

    public void SetBack()
    {
        if (lastCanvas == "login")
        {
            set.SetActive(false);
            login.SetActive(true);
        }
        else if (lastCanvas == "loginError")
        {
            set.SetActive(false);
            loginError.SetActive(true);
        }
        else if (lastCanvas == "lobby")
        {
            set.SetActive(false);
            lobby.SetActive(true);
        }
        else if (lastCanvas == "createRoom")
        {
            set.SetActive(false);
            createRoom.SetActive(true);
        }
        else if (lastCanvas == "createRoomError")
        {
            set.SetActive(false);
            createRoomError.SetActive(true);
        }
        else if(lastCanvas == "room")
        {
            set.SetActive(false);
            room.SetActive(true);
        }
    }

    public void SetExit()
    {
        Application.Quit();
    }

    IEnumerator Delay()
    {
        float time = 3;
        while (0.0f < time)
        {
            yield return new WaitForEndOfFrame();
            time -= 1.0f * Time.deltaTime;
            PV.RPC("CountdownRpc", RpcTarget.All, time);
        }
        PhotonNetwork.LoadLevel("InGame");
        yield return null;
    }

    [PunRPC]
    public void CountdownRpc(float time)
    {
        roomCountdown.text = time.ToString("0");
    }

    public void RoomClassSelectBtn()
    {
        if (lastCanvas == "room")
        {
            room.SetActive(false);
            Class.SetActive(true);
        }
        else if (lastCanvas == "Class")
        {
            Class.SetActive(false);
            room.SetActive(true);
        }

    }

    public void ClassSelectBtn()
    {
        if (gameObject.name == classListBtn[0].name)
        {
            Debug.Log(gameObject.name);
            /*for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {

            }
            characterType[i] == "Warrior";
            */
        }
    }

    void ResetReady()
    {
        isReady = false;
        PV.RPC("ReadyRpc", RpcTarget.All, isReady);
        roomReadyBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    void roomListRenewal()
    {
        maxPage = (roomList.Count % LobbyRoomListBtn.Length == 0) ? roomList.Count / LobbyRoomListBtn.Length : roomList.Count / LobbyRoomListBtn.Length + 1;

        lobbyPreviousBtn.interactable = (currentPage <= 1) ? false : true;
        lobbyNextBtn.interactable = (currentPage >= maxPage) ? false : true;

        multiple = (currentPage - 1) * LobbyRoomListBtn.Length;
        for (int i = 0; i < LobbyRoomListBtn.Length; i++)
        {
            LobbyRoomListBtn[i].interactable = (multiple + i < roomList.Count) ? true : false;
            LobbyRoomListBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < roomList.Count) ? roomList[multiple + i].Name : "";
            LobbyRoomListBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < roomList.Count) ? roomList[multiple + i].PlayerCount + "/" + roomList[multiple + i].MaxPlayers : "";
        }
    }

    void Load(Toggle toggle)
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

    public void Set()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Set2();
        }

        setBtn.onClick.AddListener(Set2);

        void Set2()
        {
            if (lastCanvas == "login")
            {
                login.SetActive(false);
            }
            else if (lastCanvas == "loginError")
            {
                loginError.SetActive(false);
            }
            else if (lastCanvas == "lobby")
            {
                lobby.SetActive(false);
            }
            else if (lastCanvas == "createRoom")
            {
                createRoom.SetActive(false);
            }
            else if (lastCanvas == "createRoomError")
            {
                createRoomError.SetActive(false);
            }
            else if (lastCanvas == "room")
            {
                room.SetActive(false);
            }
            else if (lastCanvas == "Class")
            {
                Class.SetActive(false);
            }
            set.SetActive(true);
        }
    }

    void LastCanvas()
    {
        if (login.activeSelf == true)
        {
            lastCanvas = "login";
        }
        else if (loginError.activeSelf == true)
        {
            lastCanvas = "loginError";
        }
        else if (lobby.activeSelf == true)
        {
            lastCanvas = "lobby";
        }
        else if (createRoom.activeSelf == true)
        {
            lastCanvas = "createRoom";
        }
        else if (createRoomError.activeSelf == true)
        {
            lastCanvas = "createRoomError";
        }
        else if (room.activeSelf == true)
        {
            lastCanvas = "room";
        }
        else if (Class.activeSelf == true)
        {
            lastCanvas = "Class";
        }
    }

    void RoomRenewal()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            roomPlayerList[i].transform.GetChild(0).GetComponent<Text>().text = PhotonNetwork.PlayerList[i].NickName;
            if (PhotonNetwork.MasterClient.NickName == PhotonNetwork.PlayerList[i].NickName)
            {
                roomPlayerList[i].transform.GetChild(1).gameObject.SetActive(true);
            }

            if(i == 0)
            {
                roomPlayerList[i+1].transform.GetChild(0).GetComponent<Text>().text = "";
            }
        }

        roomTitle.text = "방 : " + PhotonNetwork.CurrentRoom.Name;
        roomCurrentPlayer.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    void Update()
    {
        Set();
        LastCanvas();
    }
}