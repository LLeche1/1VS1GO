using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField loginID;
    public InputField loginPW;
    public Text loginInfo;
    public Button loginBtn;
    public Toggle loginMemory;
    public Button lobbyPreviousBtn;
    public Button lobbyNextBtn;
    public Button[] lobbyRoomListBtn;
    public Text roomTitle;
    public Text roomCurrentPlayer;
    public Text roomCountdown;
    public GameObject roomStartBtn;
    public GameObject roomReadyBtn;
    public GameObject roomOutBtn;
    public GameObject roomShadow;
    public Image[] roomPlayerList;
    public Button roomClassSelectBtn;
    public Button[] classListBtn;
    public InputField createRoomNum;
    public Button setBtn;
    public Text[] roomChatText;
    public InputField roomChatInput;
    public GraphicRaycaster graphicRaycaster;
    public GameObject login;
    public GameObject error;
    public GameObject lobby;
    public GameObject room;
    public GameObject Class;
    public GameObject createRoom;
    public GameObject set;
    public GameObject result;
    public Texture2D cursor;
    public string classType;
    private string gameVersion = "1";
    private string lastCanvas;
    private string errorType;
    private int currentPage = 1;
    private int maxPage;
    private int multiple;
    private bool isReady = false;
    private bool isReadyRpc = false;
    private bool isClassSelect = false;
    List<RoomInfo> roomList = new List<RoomInfo>();
    GameManager gameManager;
    PhotonView PV;
    Data data;

    void Awake()
    {
        Application.targetFrameRate = 144;
        Screen.SetResolution(1600, 900, false);
        PhotonNetwork.AutomaticallySyncScene = true;
        PV = GetComponent<PhotonView>();
        data = GameObject.Find("Data").GetComponent<Data>();
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Start()
    {
        LoginLoad(loginMemory);
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
            error.SetActive(true);
            errorType = "login";
            error.transform.GetChild(1).GetComponent<Text>().text = "아이디 또는 비밀번호를 잘못 입력했습니다";
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
        roomChatInput.ActivateInputField();
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
        error.SetActive(true);
        errorType = "createRoom";
        error.transform.GetChild(1).GetComponent<Text>().text = "현재 방번호와 같은 방이 존재합니다.";
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        RoomRenewal();
        PV.RPC("RoomChatRpc", RpcTarget.All, newPlayer.NickName + "님이 참가하셨습니다");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        RoomRenewal();
        roomStartBtn.GetComponent<Button>().interactable = false;
        roomReadyBtn.GetComponent<Button>().interactable = false;
        isReady = false;
        PV.RPC("ReadyRpc", RpcTarget.All, isReady);
        PV.RPC("ClassSelectBtnRpc", RpcTarget.All);
        PV.RPC("RoomChatRpc", RpcTarget.All, otherPlayer.NickName + "님이 퇴장하셨습니다");
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
            roomStartBtn.GetComponent<Button>().interactable = false;
            roomReadyBtn.GetComponent<Button>().interactable = false;
            roomClassSelectBtn.GetComponent<Button>().interactable = true;
            isReady = false;
            PV.RPC("ReadyRpc", RpcTarget.All, isReady);
            PV.RPC("ClassSelectBtnRpc", RpcTarget.All);
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
        Regex regex = new Regex(@"^[0-9]*$");
        if (regex.IsMatch(createRoomNum.text))
        {
            PhotonNetwork.CreateRoom(createRoomNum.text == "" ? Random.Range(0, 100) + "번" : createRoomNum.text + "번", new RoomOptions { MaxPlayers = 2 });
        }
        else if(!regex.IsMatch(createRoomNum.text))
        {
            createRoom.SetActive(false);
            error.SetActive(true);
            errorType = "createRoom";
            error.transform.GetChild(1).GetComponent<Text>().text = "숫자 이외에 다른 문자가 포함되어있습니다.\n다시 입력해주세요.";
        }
    }

    public void LobbyFastJoinBtn()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void RoomStartBtn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(Delay());
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    public void RoomReadyBtn()
    {
        if(isReady == false)
        {
            isReady = true;
            roomReadyBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
            roomClassSelectBtn.interactable = false;
        }
        else if (isReady == true)
        {
            isReady = false;
            roomReadyBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            roomClassSelectBtn.interactable = true;
        }
        PV.RPC("RoomReadyBtnRpc", RpcTarget.All, isReady);
    }

    [PunRPC]
    public void RoomReadyBtnRpc(bool isReady)
    {
        isReadyRpc = isReady;

        if (isReadyRpc == true)
        {
            if(PhotonNetwork.IsMasterClient && isClassSelect == true)
            {
                roomStartBtn.GetComponent<Button>().interactable = true;
            }
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
            roomStartBtn.GetComponent<Button>().interactable = false;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.MasterClient.NickName != PhotonNetwork.PlayerList[i].NickName)
                {
                    roomPlayerList[i].transform.GetChild(2).GetComponent<Text>().text = "";
                }
            }
        }
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

    public void RoomOut()
    {
        for (int i = 0; i < roomChatText.Length; i++)
        {
            roomChatText[i].text = "";
        }
        roomStartBtn.GetComponent<Button>().interactable = false;
        roomReadyBtn.GetComponent<Button>().interactable = false;
        roomStartBtn.SetActive(false);
        roomReadyBtn.SetActive(false);
        isReady = false;
        isClassSelect = false;
        roomClassSelectBtn.GetComponent<Button>().interactable = true;
        classListBtn[0].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        classListBtn[1].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        roomReadyBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        PV.RPC("ReadyRpc", RpcTarget.All, isReady);
        PV.RPC("ClassSelectBtnRpc", RpcTarget.All);
        PhotonNetwork.LeaveRoom(); 
        room.SetActive(false);
        lobby.SetActive(true);
    }

    public void Back()
    {
        if(lastCanvas == "error")
        {
            if(errorType == "login")
            {
                error.SetActive(false);
                login.SetActive(true);
            }
            else if(errorType == "createRoom")
            {
                error.SetActive(false);
                createRoom.SetActive(true);
            }
        }
        else if (lastCanvas == "createRoom")
        {
            createRoom.SetActive(false);
            lobby.SetActive(true);
        }
        else if (lastCanvas == "result")
        {
            result.SetActive(false);
            lobby.SetActive(true);
        }
    }

    public void SetBack()
    {
        if (lastCanvas == "login")
        {
            set.SetActive(false);
            login.SetActive(true);
        }
        else if (lastCanvas == "error")
        {
            set.SetActive(false);
            error.SetActive(true);
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
        else if(lastCanvas == "room")
        {
            set.SetActive(false);
            room.SetActive(true);
        }
        else if (lastCanvas == "Class")
        {
            set.SetActive(false);
            Class.SetActive(true);
        }
        else if (lastCanvas == "result")
        {
            set.SetActive(false);
            result.SetActive(true);
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
            PV.RPC("RoomCountdownRpc", RpcTarget.All, time);
        }
        PhotonNetwork.LoadLevel("InGame");
        yield return null;
    }

    [PunRPC]
    public void RoomCountdownRpc(float time)
    {
        roomCountdown.text = time.ToString("0");
        if(time > 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                roomStartBtn.GetComponent<Button>().interactable = false;

            }
            roomOutBtn.GetComponent<Button>().interactable = false;
            roomClassSelectBtn.GetComponent<Button>().interactable = false;
            roomShadow.gameObject.SetActive(true);

        }
        else if(time <= 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                roomStartBtn.GetComponent<Button>().interactable = true;

            }
            roomOutBtn.GetComponent<Button>().interactable = true;
            roomClassSelectBtn.GetComponent<Button>().interactable = true;
            roomShadow.gameObject.SetActive(false);
        }
    }

    public void RoomClassSelectBtn()
    {
        if (lastCanvas == "room")
        {
            room.SetActive(false);
            Class.SetActive(true);
        }

    }

    public void ClassSelectBtn()
    {
        if (lastCanvas == "Class")
        {
            Class.SetActive(false);
            room.SetActive(true);
        }

        if(isClassSelect == true)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                roomReadyBtn.GetComponent<Button>().interactable = true;
            }
            else if (PhotonNetwork.IsMasterClient && isReadyRpc == true)
            {
                roomStartBtn.GetComponent<Button>().interactable = true;
            }
        }
        else if(isClassSelect == false)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                roomReadyBtn.GetComponent<Button>().interactable = false;
            }
            else if (PhotonNetwork.IsMasterClient)
            {
                roomStartBtn.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void ClassListBtn(GameObject gameObject)
    {
        if(gameObject.name == classListBtn[0].name)
        {
            if(isClassSelect == false)
            {
                data.classType = "Warrior";
                gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
                isClassSelect = true;
            }
            else if(isClassSelect == true && gameObject.GetComponent<Image>().color == new Color32(255, 255, 255, 100))
            {
                data.classType = "";
                gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                isClassSelect = false;
            }
            else if (isClassSelect == true && classListBtn[1].GetComponent<Image>().color == new Color32(255, 255, 255, 100))
            {
                data.classType = "Warrior";
                gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
                classListBtn[1].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }

        }
        else if(gameObject.name == classListBtn[1].name)
        {
            if (isClassSelect == false)
            {
                data.classType = "Archor";
                gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
                isClassSelect = true;
            }
            else if (isClassSelect == true && gameObject.GetComponent<Image>().color == new Color32(255, 255, 255, 100))
            {
                data.classType = "";
                gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                isClassSelect = false;
            }
            else if (isClassSelect == true && classListBtn[0].GetComponent<Image>().color == new Color32(255, 255, 255, 100))
            {
                data.classType = "Archor";
                gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
                classListBtn[0].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }
    }

    void roomListRenewal()
    {
        maxPage = (roomList.Count % lobbyRoomListBtn.Length == 0) ? roomList.Count / lobbyRoomListBtn.Length : roomList.Count / lobbyRoomListBtn.Length + 1;

        lobbyPreviousBtn.interactable = (currentPage <= 1) ? false : true;
        lobbyNextBtn.interactable = (currentPage >= maxPage) ? false : true;

        multiple = (currentPage - 1) * lobbyRoomListBtn.Length;
        for (int i = 0; i < lobbyRoomListBtn.Length; i++)
        {
            lobbyRoomListBtn[i].interactable = (multiple + i < roomList.Count) ? true : false;
            lobbyRoomListBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < roomList.Count) ? roomList[multiple + i].Name : "";
            lobbyRoomListBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < roomList.Count) ? roomList[multiple + i].PlayerCount + "/" + roomList[multiple + i].MaxPlayers : "";
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
            else if (lastCanvas == "error")
            {
                error.SetActive(false);
            }
            else if (lastCanvas == "lobby")
            {
                lobby.SetActive(false);
            }
            else if (lastCanvas == "createRoom")
            {
                createRoom.SetActive(false);
            }
            else if (lastCanvas == "room")
            {
                room.SetActive(false);
            }
            else if (lastCanvas == "Class")
            {
                Class.SetActive(false);
            }
            else if (lastCanvas == "result")
            {
                result.SetActive(false);
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
        else if (error.activeSelf == true)
        {
            lastCanvas = "error";
        }
        else if (lobby.activeSelf == true)
        {
            lastCanvas = "lobby";
        }
        else if (createRoom.activeSelf == true)
        {
            lastCanvas = "createRoom";
        }
        else if (room.activeSelf == true)
        {
            lastCanvas = "room";
        }
        else if (Class.activeSelf == true)
        {
            lastCanvas = "Class";
        }
        else if (result.activeSelf == true)
        {
            lastCanvas = "result";
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

    public void RoomChatInput()
    {
        if(roomChatInput.text != "")
        {
            PV.RPC("RoomChatRpc", RpcTarget.All, PhotonNetwork.NickName + " : " + roomChatInput.text);
        }
        roomChatInput.text = "";
        roomChatInput.ActivateInputField();
    }

    [PunRPC]
    void RoomChatRpc(string text)
    {
        bool isInput = false;
        for (int i = 0; i < roomChatText.Length; i++)
        {
            if (roomChatText[i].text == "")
            {
                isInput = true;
                roomChatText[i].text = text;
                break;
            }
        }
        if (!isInput)
        {
            for (int i = 1; i < roomChatText.Length; i++) roomChatText[i - 1].text = roomChatText[i].text;
            roomChatText[roomChatText.Length - 1].text = text;
        }
    }

    void RoomPlayerListSet()
    {
        var ray = new PointerEventData(null);
        ray.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(ray, results);
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.name == "Cell (1)" && results[i].gameObject.transform.GetChild(0).transform.GetComponent<Text>().text != "")
            {
                if (PhotonNetwork.IsMasterClient && Input.GetMouseButtonDown(1))
                {
                    roomPlayerList[1].transform.GetChild(3).gameObject.SetActive(true);
                }
            }
        }

        if (roomPlayerList[1].transform.GetChild(3).gameObject.activeSelf == true && PhotonNetwork.IsMasterClient && Input.GetMouseButtonUp(0))
        {
            roomPlayerList[1].transform.GetChild(3).gameObject.SetActive(false);
        }
    }

    void RoomChat()
    {
        if (room.activeSelf == true && Input.GetKeyDown(KeyCode.Return))
        {
            RoomChatInput();
        }
    }

    void Update()
    {
        Set();
        LastCanvas();
        RoomPlayerListSet();
        RoomChat();
    }
}