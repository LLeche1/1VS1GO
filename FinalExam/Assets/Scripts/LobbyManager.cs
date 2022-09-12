using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 마스터(매치 메이킹) 서버와 룸 접속을 담당
public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1"; // 게임 버전
    public InputField inputNick; // 닉네임 입력받는 곳.
    public Text connectionInfoText; // 네트워크 정보를 표시할 텍스트
    public Text currentPlayerCount; // 현재 서버에 접속된 인원
    public Button joinBtn; // 룸 접속 버튼
    public Button outBtn;
    public Button startBtn;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // 게임 실행과 동시에 마스터 서버 접속 시도
    void Start()
    {
        // 접속에 필요한 정보(게임 버전) 설정
        PhotonNetwork.GameVersion = gameVersion;
        // 설정한 정보를 가지고 마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();
        // 접속을 시도 중임을 텍스트로 표시
        connectionInfoText.text = "서버에 접속중...";
    }

    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster()
    {
        // 룸 접속 버튼을 활성화
        joinBtn.interactable = true;
        // 접속 정보 표시
        connectionInfoText.text = "온라인";
    }

    // 마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        // 룸 접속 버튼을 비활성화
        joinBtn.interactable = false;
        // 접속 정보 표시
        connectionInfoText.text = "오프라인 : 서버와 연결되지 않음\n접속 재시도 중...";

        // 마스터 서버로의 재접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 룸 접속 시도
    public void Connect()
    {
        string nick = inputNick.text;
        if (nick != "")
        {
            PhotonNetwork.LocalPlayer.NickName = nick;
            // 중복 접속 시도를 막기 위해, 접속 버튼 잠시 비활성화
            joinBtn.interactable = false;

            // 마스터 서버에 접속중이라면
            if (PhotonNetwork.IsConnected)
            {
                // 룸 접속 실행
                connectionInfoText.text = "매칭 중...";
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // 마스터 서버에 접속중이 아니라면, 마스터 서버에 접속 시도
                connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
                // 마스터 서버로의 재접속 시도
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }

    // (빈 방이 없어)랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // 접속 상태 표시
        connectionInfoText.text = "방 생성 중...";
        // 최대 4명을 수용 가능한 빈방을 생성
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        UpdatePlayerCounts();
        // 접속 상태 표시
        connectionInfoText.text = "방 참가 성공";
        outBtn.interactable = true;
        startBtn.interactable = true;
    }

    private void UpdatePlayerCounts()
    {
        if(connectionInfoText.text != "방 참가 성공")
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
    }

    public void GameStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 목표 인원 수 채웠으면, 맵 이동을 한다. 권한은 마스터 클라이언트만.
            // PhotonNetwork.AutomaticallySyncScene = true; 를 해줬어야 방에 접속한 인원이 모두 이동함.
            if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                PhotonNetwork.LoadLevel("InGame 1");
            }
        }
    }

    void Update()
    {
        UpdatePlayerCounts();
    }
}