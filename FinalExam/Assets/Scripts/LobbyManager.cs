using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    public InputField inputID;
    public InputField inputPW;
    public InputField inputRoom;
    public Text connectInfo;
    public Text roomInfo;
    public Text roomInfo2;
    public Button joinBtn;
    public GameObject startBtn;
    public Button startBtn2;
    public GameObject readyBtn;
    public Button readyBtn2;
    public Button outBtn;
    public Button previousBtn;
    public Button nextBtn;
    public Button[] roomCellBtn;
    public Image[] playerCell;
    public GameObject login;
    public GameObject loginError;
    public GameObject lobby;
    public GameObject room;
    public GameObject roomNum;
    public GameObject roomNumError;
    public GameObject set;
    public Toggle remember;
    private PhotonView PV;
    private string gameVersion = "1";
    private string lastCanvas;
    private int currentPage = 1;
    private int maxPage;
    private int multiple;
    private float delay = 10;
    private bool isReady = false;
    private bool isReadyRpc = false;
    private bool isStart = false;
    List<RoomInfo> roomList = new List<RoomInfo>();

    void Awake()
    {
        Application.targetFrameRate = 144;
        Screen.SetResolution(1600, 900, false);
        PhotonNetwork.AutomaticallySyncScene = true;
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        Load(remember);
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        connectInfo.text = "서버에 접속중...";
    }

    public override void OnConnectedToMaster()
    {
        joinBtn.interactable = true;
        connectInfo.text = "온라인";
        if(lobby.activeSelf == true)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        connectInfo.text = "오프라인 : 서버와 연결되지 않음\n접속 재시도 중...";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        string id = inputID.text;
        string pw = inputPW.text;
        if (id != "" && pw != "")
        {
            joinBtn.interactable = false;

            if (PhotonNetwork.IsConnected)
            {
                connectInfo.text = "매칭 중...";
                PhotonNetwork.JoinLobby();
                PhotonNetwork.LocalPlayer.NickName = id;
            }
            else
            {
                connectInfo.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
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
        CreateRoom();
    }

    public override void OnJoinedRoom()
    {
        lobby.SetActive(false);
        roomNum.SetActive(false);
        room.SetActive(true);
        RoomRenewal();
        if (photonView.IsMine)
        {
            startBtn.SetActive(true);
        }
        else
        {
            readyBtn.SetActive(true);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        roomNum.SetActive(false);
        roomNumError.SetActive(true);
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
            startBtn.SetActive(true);
            readyBtn.SetActive(false);
        }
    }

    public void DisconnectBtn()
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

    public void RoomNum()
    {
        lobby.SetActive(false);
        roomNum.SetActive(true);
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(inputRoom.text == "" ? Random.Range(0, 100) + "번" : inputRoom.text + "번", new RoomOptions { MaxPlayers = 2 });
    }

    public void JoinRandomRoom()
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
            PhotonNetwork.LoadLevel("InGame");
        }
    }

    public void ReadyBtn()
    {
        if(isReady == false)
        {
            isReady = true;
            readyBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        }
        else if (isReady == true)
        {
            isReady = false;
            readyBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        PV.RPC("ReadyRpc", RpcTarget.All, isReady);
    }

    [PunRPC]
    public void ReadyRpc(bool isReady)
    {
        isReadyRpc = isReady;

        if (isReadyRpc == true)
        {
            startBtn2.interactable = true;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.MasterClient.NickName != PhotonNetwork.PlayerList[i].NickName)
                {
                    playerCell[i].transform.GetChild(2).GetComponent<Text>().text = "준비완료";
                }
            }
        }
        else if (isReadyRpc == false)
        {
            startBtn2.interactable = false;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.MasterClient.NickName != PhotonNetwork.PlayerList[i].NickName)
                {
                    playerCell[i].transform.GetChild(2).GetComponent<Text>().text = "";
                }
            }
        }
    }

    public void Remember(Toggle toggle)
    {
        if (toggle.isOn)
        {
            PlayerPrefs.SetString("ID", inputID.text);
            PlayerPrefs.SetString("PW", inputPW.text);
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
        startBtn2.interactable = false;
        if (startBtn.activeSelf == true)
        {
            startBtn.SetActive(false);
        }
        else if(readyBtn.activeSelf == true)
        {
            readyBtn.SetActive(false);
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
        else if (lastCanvas == "roomNum")
        {
            roomNum.SetActive(false);
            lobby.SetActive(true);
        }
        else if (lastCanvas == "roomNumError")
        {
            roomNumError.SetActive(false);
            roomNum.SetActive(true);
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
        else if (lastCanvas == "roomNum")
        {
            set.SetActive(false);
            roomNum.SetActive(true);
        }
        else if (lastCanvas == "roomNumError")
        {
            set.SetActive(false);
            roomNumError.SetActive(true);
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

    void ResetReady()
    {
        isReady = false;
        PV.RPC("ReadyRpc", RpcTarget.All, isReady);
        readyBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    void roomListRenewal()
    {
        maxPage = (roomList.Count % roomCellBtn.Length == 0) ? roomList.Count / roomCellBtn.Length : roomList.Count / roomCellBtn.Length + 1;

        previousBtn.interactable = (currentPage <= 1) ? false : true;
        nextBtn.interactable = (currentPage >= maxPage) ? false : true;

        multiple = (currentPage - 1) * roomCellBtn.Length;
        for (int i = 0; i < roomCellBtn.Length; i++)
        {
            roomCellBtn[i].interactable = (multiple + i < roomList.Count) ? true : false;
            roomCellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < roomList.Count) ? roomList[multiple + i].Name : "";
            roomCellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < roomList.Count) ? roomList[multiple + i].PlayerCount + "/" + roomList[multiple + i].MaxPlayers : "";
        }
    }

    void Load(Toggle toggle)
    {
        inputID.text = PlayerPrefs.GetString("ID");
        inputPW.text = PlayerPrefs.GetString("PW");
        if (PlayerPrefs.GetInt("IsOn") == 1)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }
    }

    void Set()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
            else if (lastCanvas == "roomNum")
            {
                roomNum.SetActive(false);
            }
            else if (lastCanvas == "roomNumError")
            {
                roomNumError.SetActive(false);
            }
            else if (lastCanvas == "room")
            {
                room.SetActive(false);
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
        else if (roomNum.activeSelf == true)
        {
            lastCanvas = "roomNum";
        }
        else if (roomNumError.activeSelf == true)
        {
            lastCanvas = "roomNumError";
        }
        else if (room.activeSelf == true)
        {
            lastCanvas = "room";
        }
    }

    void RoomRenewal()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerCell[i].transform.GetChild(0).GetComponent<Text>().text = PhotonNetwork.PlayerList[i].NickName;
            if (PhotonNetwork.MasterClient.NickName == PhotonNetwork.PlayerList[i].NickName)
            {
                playerCell[i].transform.GetChild(1).GetComponent<Text>().text = "방장";
            }

            if(i == 0)
            {
                playerCell[i+1].transform.GetChild(0).GetComponent<Text>().text = "";
            }
        }

        if(PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        else if(PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }

        roomInfo.text = "방 : " + PhotonNetwork.CurrentRoom.Name;
        roomInfo2.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    void Update()
    {
        Set();
        LastCanvas();
    }
}