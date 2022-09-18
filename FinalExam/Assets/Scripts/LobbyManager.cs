using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// 마스터(매치 메이킹) 서버와 룸 접속을 담당
public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField inputID;
    public InputField inputPW;
    public InputField inputRoom;
    public Text connectInfo;
    public Text lobbyInfo;
    public Text roomInfo;
    public Text currentPlayerCount;
    public Text playerList;
    public Button joinBtn; // 룸 접속 버튼
    public Button startBtn;
    public Button outBtn;
    public GameObject sign;
    public GameObject lobby;
    public GameObject room;
    public GameObject roomNum;
    public GameObject roomNumError;
    public GameObject set;
    public Toggle remember;
    private PhotonView PV;
    private string gameVersion = "1";
    private string lastCanvas;
    List<RoomInfo> roomList = new List<RoomInfo>();

    void Awake()
    {
        Application.targetFrameRate = 144;
        Screen.SetResolution(1600, 900, false);
        PhotonNetwork.AutomaticallySyncScene = true;
        PV = GetComponent<PhotonView>();
    }

    // 게임 실행과 동시에 마스터 서버 접속 시도
    void Start()
    {
        Load(remember);
        // 접속에 필요한 정보(게임 버전) 설정
        PhotonNetwork.GameVersion = gameVersion;
        // 설정한 정보를 가지고 마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();
        // 접속을 시도 중임을 텍스트로 표시
        connectInfo.text = "서버에 접속중...";
    }

    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster()
    {
        // 룸 접속 버튼을 활성화
        joinBtn.interactable = true;
        // 접속 정보 표시
        connectInfo.text = "온라인";
    }

    // 마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        // 접속 정보 표시
        connectInfo.text = "오프라인 : 서버와 연결되지 않음\n접속 재시도 중...";

        // 마스터 서버로의 재접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 룸 접속 시도
    public void Connect()
    {
        string nick = inputID.text;
        if (nick != "")
        {
            // 중복 접속 시도를 막기 위해, 접속 버튼 잠시 비활성화
            joinBtn.interactable = false;

            // 마스터 서버에 접속중이라면
            if (PhotonNetwork.IsConnected)
            {
                // 룸 접속 실행
                connectInfo.text = "매칭 중...";
                PhotonNetwork.JoinLobby();
                sign.SetActive(false);
                lobby.SetActive(true);
                PhotonNetwork.LocalPlayer.NickName = nick;
                //PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // 마스터 서버에 접속중이 아니라면, 마스터 서버에 접속 시도
                connectInfo.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
                // 마스터 서버로의 재접속 시도
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }

    // (빈 방이 없어)랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        lobby.SetActive(false);
        roomNum.SetActive(false);
        room.SetActive(true);
        RoomRenewal();
        outBtn.interactable = true;
        if (photonView.IsMine)
        {
            startBtn.interactable = true;
        }
    }

    // 룸 생성 실패시 자동 실행
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
    }

    public void DisconnectBtn()
    {
        PhotonNetwork.Disconnect();
        lobby.SetActive(false);
        sign.SetActive(true);
    }

    public void RoomListBtn()
    {

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

    public void GameStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 목표 인원 수 채웠으면, 맵 이동을 한다. 권한은 마스터 클라이언트만.
            // PhotonNetwork.AutomaticallySyncScene = true; 를 해줬어야 방에 접속한 인원이 모두 이동함.
            /*if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                PhotonNetwork.LoadLevel("InGame 1");
            }
            */
            PhotonNetwork.LoadLevel("InGame");
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
        PhotonNetwork.LeaveRoom();
        room.SetActive(false);
        lobby.SetActive(true);
    }

    public void Back()
    {
        if (lastCanvas == "roomNum")
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
        if (lastCanvas == "sign")
        {
            set.SetActive(false);
            sign.SetActive(true);
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
            if (lastCanvas == "sign")
            {
                sign.SetActive(false);
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
        if (sign.activeSelf == true)
        {
            lastCanvas = "sign";
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
        playerList.text = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerList.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : "\n\n ");
        }
        roomInfo.text = "방 번호 : " + PhotonNetwork.CurrentRoom.Name;
        //" 현재인원 : " + PhotonNetwork.CurrentRoom.PlayerCount + " 최대인원 : " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    void Update()
    {
        Set();
        LastCanvas();
        //lobbyInfo = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";
    }
}