using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

// 마스터(매치 메이킹) 서버와 룸 접속을 담당
public class LobbyManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public InputField inputID; // 닉네임 입력받는 곳.
    public InputField inputPW;
    public Text connectInfo; // 네트워크 정보를 표시할 텍스트
    public Text currentPlayerCount; // 현재 서버에 접속된 인원
    public Text user1;
    public Text user2;
    public Button joinBtn; // 룸 접속 버튼
    public Button outBtn;
    public Button startBtn;
    public GameObject sign;
    public GameObject room;
    public GameObject set;
    public Toggle remember;
    private PhotonView PV;
    private string gameVersion = "1"; // 게임 버전
    private string lastCanvas;

    void Awake()
    {
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
            PhotonNetwork.LocalPlayer.NickName = nick;
            // 중복 접속 시도를 막기 위해, 접속 버튼 잠시 비활성화
            joinBtn.interactable = false;

            // 마스터 서버에 접속중이라면
            if (PhotonNetwork.IsConnected)
            {
                // 룸 접속 실행
                connectInfo.text = "매칭 중...";
                PhotonNetwork.JoinRandomRoom();
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
        // 접속 상태 표시
        connectInfo.text = "방 생성 중...";
        // 최대 4명을 수용 가능한 빈방을 생성
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        sign.SetActive(false);
        room.SetActive(true);
        Participate();
        UpdatePlayerCounts();
        // 접속 상태 표시
        connectInfo.text = "방 참가 성공";
        outBtn.interactable = true;
        if (photonView.IsMine)
        {
            startBtn.interactable = true;
        }
    }

    private void UpdatePlayerCounts()
    {
        if(connectInfo.text != "방 참가 성공")
        {
            currentPlayerCount.text = "";
        }
        else
        {
            currentPlayerCount.text = $"현재 인원 / 최대 인원 \n{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
        }
    }

    public void Out()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby");
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

    public void Back()
    {
        if (lastCanvas == "sign")
        {
            set.SetActive(false);
            sign.SetActive(true);
            connectInfo.enabled = true;
        }
        else if (lastCanvas == "room")
        {
            set.SetActive(false);
            room.SetActive(true);
            connectInfo.enabled = true;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    void Load(Toggle toggle)
    {
        inputID.text = PlayerPrefs.GetString("ID");
        inputPW.text = PlayerPrefs.GetString("PW");
        if(PlayerPrefs.GetInt("IsOn") == 1)
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
            if (sign.activeSelf == true)
            {
                sign.SetActive(false);
                lastCanvas = "sign";
            }
            else if (room.activeSelf == true)
            {
                room.SetActive(false);
                lastCanvas = "room";
            }
            set.SetActive(true);
            connectInfo.enabled = false;
        }
    }

    void Update()
    {
        UpdatePlayerCounts();
        Set();
        Debug.Log(user1.text);
        Debug.Log(user2.text);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(user1.text);
            stream.SendNext(user2.text);
        }
        else
        {
            user1.text = (string)stream.ReceiveNext();
            user2.text = (string)stream.ReceiveNext();
        }
    }

    void Participate()
    {
        if (user1.text == "")
        {
            user1.text = PhotonNetwork.LocalPlayer.NickName;
        }
        else if (user2.text == "")
        {
            user2.text = PhotonNetwork.LocalPlayer.NickName;
        }
    }
}