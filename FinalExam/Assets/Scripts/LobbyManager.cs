using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject title;
    public GameObject titleLoading;
    public GameObject login;
    public TMP_InputField loginID;
    public TMP_InputField loginPW;
    public TMP_InputField signUpID;
    public TMP_InputField signUpPW;
    public Button loginBtn;
    public Toggle loginRememberMe;
    public GameObject signUp;
    public GameObject lobby;
    public TMP_Text lobbyLevel;
    public TMP_Text lobbyExp;
    public GameObject lobbyExp_Slider;
    public TMP_Text lobbyName;
    public TMP_Text lobbyGold;
    public TMP_Text lobbyCrystal;
    public GameObject lobbySet;
    public GameObject lobbySet_Language;
    public GameObject lobbyChat;
    public TMP_Text[] lobbyChatText;
    public TMP_InputField lobbyChatInput;
    public GameObject userInfo;
    public GameObject userInfo_ChangeName;
    public TMP_InputField userInfo_ChangeName_Name;
    public TMP_Text lobbyUserInfo_Level;
    public TMP_Text lobbyUserInfo_Exp;
    public GameObject lobbyUserInfo_Exp_Slider;
    public TMP_Text lobbyUserInfo_Name;
    public TMP_Text lobbyUserInfo_Highest_Trophies;
    public TMP_Text lobbyUserInfo_Most_Wins;
    public TMP_Text lobbyUserInfo_1vs1;
    public TMP_Text lobbyUserInfo_2vs2;
    public TMP_Text lobbyUserInfo_Total_Play;
    public TMP_Text lobbyUserInfo_MVP;
    public GameObject error;
    public TMP_Text errorInfo;
    public GameObject errorNetwork;
    public GameObject room;
    public TMP_Text roomPlayer;
    public GameObject roomCount;
    public TMP_Text roomCountText;
    private float level;
    private float exp;
    private string name;
    private float gold;
    private float crystal;
    private float highest_Trophies;
    private float most_Wins;
    private float _1vs1;
    private float _2vs2;
    private float total_Play;
    private float mvp;



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
        PlayFabSettings.TitleId = "9BF08";
        Application.targetFrameRate = 144;
        PhotonNetwork.AutomaticallySyncScene = true;
        PV = GetComponent<PhotonView>();
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        //Cursor.lockState = CursorLockMode.Confined;
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
        StartCoroutine(LoadDelay());
    }

    IEnumerator LoadDelay()
    {
        float time = 0;
        while (time < 100)
        {
            yield return new WaitForEndOfFrame();
            time += 30.0f * Time.deltaTime;
            titleLoading.GetComponent<Slider>().value = time * 0.01f;
            titleLoading.transform.GetChild(1).GetComponent<TMP_Text>().text = time.ToString("0") + "%";
        }
        titleLoading.SetActive(false);
        login.SetActive(true);
        loginBtn.interactable = true;
        yield return null;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("온라인");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("오프라인");
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Login()
    {
        LoginMemory(loginRememberMe);
        loginBtn.interactable = false;
        var request = new LoginWithPlayFabRequest { Username = loginID.text, Password = loginPW.text };
        PlayFabClientAPI.LoginWithPlayFab(request, LoginSuccess, LoginFail);
        loginBtn.interactable = true;
    }

    void LoginSuccess(LoginResult result)
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("로비");
            PhotonNetwork.JoinLobby();
            PhotonNetwork.LocalPlayer.NickName = loginID.text;
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            (result) => {
                foreach (var eachData in result.Data)
                {
                    if (eachData.Key == "Name")
                    {
                        name = eachData.Value.Value;
                    }
                }
            },
            (error) => print("데이터 불러오기 실패"));

            PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
            (result) =>
            {
                foreach (var eachStat in result.Statistics)
                {
                    if (eachStat.StatisticName == "Level")
                    {
                        level = eachStat.Value;
                    }
                    else if (eachStat.StatisticName == "Exp")
                    {
                        exp = eachStat.Value;
                    }
                    else if (eachStat.StatisticName == "Gold")
                    {
                        gold = eachStat.Value;
                    }
                    else if (eachStat.StatisticName == "Crystal")
                    {
                        crystal = eachStat.Value;
                    }
                    else if (eachStat.StatisticName == "Highest_Trophies")
                    {
                        highest_Trophies = eachStat.Value;
                    }
                    else if (eachStat.StatisticName == "Most_Wins")
                    {
                        most_Wins = eachStat.Value;
                    }
                    else if (eachStat.StatisticName == "1vs1")
                    {
                        _1vs1 = eachStat.Value;
                    }
                    else if (eachStat.StatisticName == "2vs2")
                    {
                        _2vs2 = eachStat.Value;
                    }
                    else if (eachStat.StatisticName == "Total_Play")
                    {
                        total_Play = eachStat.Value;
                    }
                    else if (eachStat.StatisticName == "MVP")
                    {
                        mvp = eachStat.Value;
                    }
                }
            },
            (error) => { Debug.Log("값 로딩 실패"); });
        }
        else
        {
            Debug.Log("오프라인");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    void LoginFail(PlayFabError error2)
    {
        login.SetActive(false);
        error.SetActive(true);
        errorInfo.text = error2.GenerateErrorReport();
        errorType = "login";
        Debug.LogWarning("로그인 실패");
        loginID.text = null;
        loginPW.text = null;
    }

    public void Register()
    {
        var request = new RegisterPlayFabUserRequest { Username = signUpID.text, Password = signUpPW.text, RequireBothUsernameAndEmail = false };
        PlayFabClientAPI.RegisterPlayFabUser(request, RegisterSuccess, RegisterFailure);
    }

    private void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        signUp.SetActive(false);
        login.SetActive(true);
        Debug.Log("가입 성공");
        var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "Name", signUpID.text } } };
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "Name", signUpID.text }
            }
        },
        (result) => print("데이터 저장 성공"),
        (error) => print("데이터 저장 실패"));

        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate {StatisticName = "Level", Value = int.Parse(1.ToString())},
                new StatisticUpdate {StatisticName = "Exp", Value = int.Parse(0.ToString())},
                new StatisticUpdate {StatisticName = "Gold", Value = int.Parse(0.ToString())},
                new StatisticUpdate {StatisticName = "Crystal", Value = int.Parse(0.ToString())},
                new StatisticUpdate {StatisticName = "Highest_Trophies", Value = int.Parse(0.ToString())},
                new StatisticUpdate {StatisticName = "Most_Wins", Value = int.Parse(0.ToString())},
                new StatisticUpdate {StatisticName = "1vs1", Value = int.Parse(0.ToString())},
                new StatisticUpdate {StatisticName = "2vs2", Value = int.Parse(0.ToString())},
                new StatisticUpdate {StatisticName = "Total_Play", Value = int.Parse(0.ToString())},
                new StatisticUpdate {StatisticName = "MVP", Value = int.Parse(0.ToString())}
            }
        },
        (result) => { Debug.Log("값 저장 완료"); },
        (error) => { Debug.Log("값 저장 실패"); });
    }

    private void RegisterFailure(PlayFabError error2)
    {
        signUp.SetActive(false);
        error.SetActive(true);
        errorInfo.text = error2.GenerateErrorReport();
        errorType = "signUp";
        Debug.LogWarning("가입 실패");
        signUpID.text = null;
        signUpPW.text = null;
    }

    public override void OnJoinedLobby()
    {
        lobby.SetActive(true);
        title.SetActive(false);
        login.SetActive(false);
    }

    public void LoginSignUp()
    {
        signUp.SetActive(true);
        login.SetActive(false);
    }

    public void LobbySet()
    {
        lobbySet.SetActive(true);
    }

    public void LobbySet_Language()
    {
        lobbySet_Language.SetActive(true);
    }

    public void LobbyChat()
    {
        lobbyChat.SetActive(true);
    }

    public void UserInfo()
    {
        userInfo.SetActive(true);
    }

    public void UserInfo_ChangeName()
    {
        userInfo_ChangeName.SetActive(true);
    }

    public void UserInfo_ChangeName_Name()
    {
        if(userInfo_ChangeName_Name.text != "")
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
            {
                { "Name", userInfo_ChangeName_Name.text }
            }
            },
            (result) => PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
                (result) => {
                    foreach (var eachData in result.Data)
                    {
                        if (eachData.Key == "Name")
                        {
                            name = eachData.Value.Value;
                        }
                    }
                },
                (error) => print("데이터 불러오기 실패")),
            (error) => print("데이터 저장 실패"));

            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate {StatisticName = "Crystal", Value = int.Parse((crystal - 10).ToString())},
            }
            },
            (result) => {
                PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
                (result) =>
                {
                    foreach (var eachStat in result.Statistics)
                    {
                        if (eachStat.StatisticName == "Crystal")
                        {
                            crystal = eachStat.Value;
                        }
                    }
                },
                (error) => { Debug.Log("값 로딩 실패"); });
            },
            (error) => { Debug.Log("값 저장 실패"); });

            userInfo_ChangeName.SetActive(false);
            userInfo_ChangeName_Name.text = "";
        }



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
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            roomCount.SetActive(true);
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(StartDelay());
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
    }

    IEnumerator StartDelay()
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

    public void Back()
    {
        if(lastCanvas == "login")
        {
            login.SetActive(false);
            title.SetActive(true);
        }
        else if (lastCanvas == "signUp")
        {
            signUp.SetActive(false);
            login.SetActive(true);
        }
        else if(lastCanvas == "error")
        {
            if(errorType == "login")
            {
                error.SetActive(false);
                login.SetActive(true);
            }
            if (errorType == "signUp")
            {
                error.SetActive(false);
                signUp.SetActive(true);
            }
        }
        else if (lastCanvas == "lobbySet")
        {
            lobbySet.SetActive(false);
        }
        else if (lastCanvas == "lobbySet_Language")
        {
            lobbySet_Language.SetActive(false);
        }
        else if (lastCanvas == "lobbyChat")
        {
            lobbyChat.SetActive(false);
        }
        else if (lastCanvas == "userInfo")
        {
            userInfo.SetActive(false);
        }
        else if (lastCanvas == "userInfo_ChangeName")
        {
            userInfo_ChangeName.SetActive(false);
        }
    }

    void LastCanvas()
    {
        if(title.activeSelf == true)
        {
            if (login.activeSelf == true)
            {
                lastCanvas = "login";
            }
            else if (signUp.activeSelf == true)
            {
                lastCanvas = "signUp";
            }
            else if (error.activeSelf == true)
            {
                lastCanvas = "error";
            }
            else
            {
                lastCanvas = "title";
            }
        }
        else if (lobby.activeSelf == true && room.activeSelf == false)
        {
            if (lobbySet.activeSelf == true)
            {
                if (lobbySet_Language.activeSelf == true)
                {
                    lastCanvas = "lobbySet_Language";
                }
                else
                {
                    lastCanvas = "lobbySet";
                }
            }
            else if (lobbyChat.activeSelf == true)
            {
                lastCanvas = "lobbyChat";
            }
            else if (userInfo.activeSelf == true)
            {
                if (userInfo_ChangeName.activeSelf == true)
                {
                    lastCanvas = "userInfo_ChangeName";
                }
                else
                {
                    lastCanvas = "userInfo";
                }
            }
            else
            {
                lastCanvas = "lobby";
            }
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

    void SetData()
    {
        lobbyName.text = name;
        lobbyLevel.text = level.ToString();
        lobbyExp.text = exp.ToString() + "/500";
        lobbyExp_Slider.GetComponent<Slider>().value = exp / 500;
        lobbyGold.text = gold.ToString();
        lobbyCrystal.text = crystal.ToString();
        lobbyUserInfo_Level.text = level.ToString();
        lobbyUserInfo_Exp.text = exp.ToString() + "/500";
        lobbyUserInfo_Exp_Slider.GetComponent<Slider>().value = exp / 500;
        lobbyUserInfo_Name.text = name;
        lobbyUserInfo_Highest_Trophies.text = highest_Trophies.ToString();
        lobbyUserInfo_Most_Wins.text = most_Wins.ToString();
        lobbyUserInfo_1vs1.text = _1vs1.ToString();
        lobbyUserInfo_2vs2.text = _2vs2.ToString();
        lobbyUserInfo_Total_Play.text = total_Play.ToString();
        lobbyUserInfo_MVP.text = mvp.ToString();
    }

    void Update()
    {
        LastCanvas();
        LobbyChatEnter();
        SetData();
    }
}