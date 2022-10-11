using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks, IChatClientListener
{
    public GameObject gameManager;
    public GameObject main;
    public GameObject inGame;
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
    public GameObject lobbyShop;
    public GameObject lobbyShop_Purchase;
    public GameObject lobbyShop_Purchase_Success;
    public GameObject lobbyShop_Purchase_Fail;
    public TMP_Text lobbyShop_Gold;
    public TMP_Text lobbyShop_Crystal;
    public GameObject lobbyInventory;
    public TMP_Text lobbyInventory_Gold;
    public TMP_Text lobbyInventory_Crystal;
    public TMP_Text lobbyInventory_Name;
    public TMP_Text lobbyInventory_Level;
    public TMP_Text lobbyInventory_Exp;
    public GameObject lobbyRanking;
    public GameObject error;
    public TMP_Text errorInfo;
    public GameObject errorNetwork;
    public GameObject room;
    public TMP_Text roomPlayer;
    public GameObject roomLoading;
    public GameObject roomLoading_Slider;
    private float level;
    private float exp;
    private string nickName;
    private float gold;
    private float crystal;
    private float highest_Trophies;
    private float most_Wins;
    private float _1vs1;
    private float _2vs2;
    private float total_Play;
    private float mvp;
    private string gameVersion = "1";
    private string lastCanvas;
    private string errorType;
    private string lobbyShop_Name;
    public GraphicRaycaster graphicRaycaster;
    public ChatClient chatClient;
    private string channelName = "Global";
    public GameObject myChatPrefab;
    public GameObject otherChatPrefab;
    public GameObject chatScroll;
    public TMP_Text chatText;
    PhotonView PV;

    private bool isTest = false;

    void Awake()
    {
        Screen.SetResolution(2532, 1170, false);
        Application.targetFrameRate = 144;
        PlayFabSettings.TitleId = "9BF08";
        PhotonNetwork.AutomaticallySyncScene = true;
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        LoginLoad(loginRememberMe);
        PhotonNetwork.GameVersion = gameVersion;
    }

    void Update()
    {
        LastCanvas();
        LobbyChatEnter();
        SetData();
        if (this.chatClient != null)
        {
            this.chatClient.Service();
        }
    }

    public void Title_Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        title.transform.GetChild(2).gameObject.SetActive(false);
        titleLoading.SetActive(true);
        StartCoroutine(TitleLoadDelay());
    }

    IEnumerator TitleLoadDelay()
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
        if (loginID.text == "" && loginPW.text == "")
        {
            login.SetActive(true);
        }
        else if (loginID.text != "" && loginPW.text != "")
        {
            Login();
        }
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
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            (result) =>
            {
                foreach (var eachData in result.Data)
                {
                    if (eachData.Key == "NickName")
                    {
                        nickName = eachData.Value.Value;
                    }
                }
            },
            (error) => print("데이터 불러오기 실패"));

            PhotonNetwork.LocalPlayer.NickName = nickName;

            this.chatClient = new ChatClient(this);
            chatClient.UseBackgroundWorkerForSending = true;
            this.chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new AuthenticationValues(loginID.text));

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
                { "NickName", signUpID.text }
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

    public void LobbySet_Logout()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Main");
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
        if (userInfo_ChangeName_Name.text != "" && crystal >= 10)
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
            {
                { "Name", userInfo_ChangeName_Name.text }
            }
            },
            (result) => PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
                (result) =>
                {
                    foreach (var eachData in result.Data)
                    {
                        if (eachData.Key == "Name")
                        {
                            nickName = eachData.Value.Value;
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
            (result) =>
            {
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
        else if (userInfo_ChangeName_Name.text == "")
        {
            errorInfo.text = "UserName must be between 3 and 20 characters";
            errorType = "userInfo_ChangeName_Name_Empty";
            error.SetActive(true);
        }
        else if (crystal < 10)
        {
            errorInfo.text = "You need more than 10 Crystal";
            errorType = "userInfo_ChangeName_Name_Crystal";
            error.SetActive(true);
        }
    }

    public void LobbyShop()
    {
        lobbyShop.SetActive(true);
    }

    public void LobbyShop_Purchase()
    {
        lobbyShop_Name = EventSystem.current.currentSelectedGameObject.name;
        lobbyShop_Purchase.SetActive(true);
    }

    public void LobbyShop_Purchase_Yes()
    {
        if (lobbyShop_Name == "Gold_1500")
        {
            if(crystal >= 12)
            {
                PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                        new StatisticUpdate {StatisticName = "Gold", Value = int.Parse((gold + 1500).ToString())},
                        new StatisticUpdate {StatisticName = "Crystal", Value = int.Parse((crystal - 12).ToString())},
                    }
                },
                    (result) => {
                        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
                        (result) =>
                        {
                            foreach (var eachStat in result.Statistics)
                            {
                                if (eachStat.StatisticName == "Gold")
                                {
                                    gold = eachStat.Value;
                                }
                                if (eachStat.StatisticName == "Crystal")
                                {
                                    crystal = eachStat.Value;
                                }
                            }
                        },
                        (error) => { Debug.Log("값 로딩 실패"); });
                },
                (error) => { Debug.Log("값 저장 실패"); });
                lobbyShop_Purchase_Success.SetActive(true);
            }
            else
            {
                lobbyShop_Purchase_Fail.SetActive(true);
            }
        }
        else if (lobbyShop_Name == "Gold_4000")
        {
            if (crystal >= 48)
            {
                PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                        new StatisticUpdate {StatisticName = "Gold", Value = int.Parse((gold + 4000).ToString())},
                        new StatisticUpdate {StatisticName = "Crystal", Value = int.Parse((crystal - 48).ToString())},
                    }
                },
                    (result) => {
                        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
                        (result) =>
                        {
                            foreach (var eachStat in result.Statistics)
                            {
                                if (eachStat.StatisticName == "Gold")
                                {
                                    gold = eachStat.Value;
                                }
                                if (eachStat.StatisticName == "Crystal")
                                {
                                    crystal = eachStat.Value;
                                }
                            }
                        },
                        (error) => { Debug.Log("값 로딩 실패"); });
                    },
                (error) => { Debug.Log("값 저장 실패"); });
                lobbyShop_Purchase_Success.SetActive(true);
            }
            else
            {
                lobbyShop_Purchase_Fail.SetActive(true);
            }
        }
        else if (lobbyShop_Name == "Gold_12000")
        {
            if (crystal >= 120)
            {
                PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                        new StatisticUpdate {StatisticName = "Gold", Value = int.Parse((gold + 12000).ToString())},
                        new StatisticUpdate {StatisticName = "Crystal", Value = int.Parse((crystal - 120).ToString())},
                    }
                },
                    (result) => {
                        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
                        (result) =>
                        {
                            foreach (var eachStat in result.Statistics)
                            {
                                if (eachStat.StatisticName == "Gold")
                                {
                                    gold = eachStat.Value;
                                }
                                if (eachStat.StatisticName == "Crystal")
                                {
                                    crystal = eachStat.Value;
                                }
                            }
                        },
                        (error) => { Debug.Log("값 로딩 실패"); });
                    },
                (error) => { Debug.Log("값 저장 실패"); });
                lobbyShop_Purchase_Success.SetActive(true);
            }
            else
            {
                lobbyShop_Purchase_Fail.SetActive(true);
            }
        }
        else if (lobbyShop_Name == "Gold_25000")
        {
            if (crystal >= 240)
            {
                PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                        new StatisticUpdate {StatisticName = "Gold", Value = int.Parse((gold + 25000).ToString())},
                        new StatisticUpdate {StatisticName = "Crystal", Value = int.Parse((crystal - 240).ToString())},
                    }
                },
                    (result) => {
                        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
                        (result) =>
                        {
                            foreach (var eachStat in result.Statistics)
                            {
                                if (eachStat.StatisticName == "Gold")
                                {
                                    gold = eachStat.Value;
                                }
                                if (eachStat.StatisticName == "Crystal")
                                {
                                    crystal = eachStat.Value;
                                }
                            }
                        },
                        (error) => { Debug.Log("값 로딩 실패"); });
                    },
                (error) => { Debug.Log("값 저장 실패"); });
                lobbyShop_Purchase_Success.SetActive(true);
            }
            else
            {
                lobbyShop_Purchase_Fail.SetActive(true);
            }
        }
        else if (lobbyShop_Name == "Gold_60000")
        {
            if (crystal >= 490)
            {
                PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                        new StatisticUpdate {StatisticName = "Gold", Value = int.Parse((gold + 60000).ToString())},
                        new StatisticUpdate {StatisticName = "Crystal", Value = int.Parse((crystal - 490).ToString())},
                    }
                },
                    (result) => {
                        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
                        (result) =>
                        {
                            foreach (var eachStat in result.Statistics)
                            {
                                if (eachStat.StatisticName == "Gold")
                                {
                                    gold = eachStat.Value;
                                }
                                if (eachStat.StatisticName == "Crystal")
                                {
                                    crystal = eachStat.Value;
                                }
                            }
                        },
                        (error) => { Debug.Log("값 로딩 실패"); });
                    },
                (error) => { Debug.Log("값 저장 실패"); });
                lobbyShop_Purchase_Success.SetActive(true);
            }
            else
            {
                lobbyShop_Purchase_Fail.SetActive(true);
            }
        }
        else if (lobbyShop_Name == "Crystal_40")
        {
            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate {StatisticName = "Crystal", Value = int.Parse((crystal + 40).ToString())},
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
            lobbyShop_Purchase_Success.SetActive(true);
        }
        else if (lobbyShop_Name == "Crystal_220")
        {
            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate {StatisticName = "Crystal", Value = int.Parse((crystal + 220).ToString())},
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
            lobbyShop_Purchase_Success.SetActive(true);
        }
        else if (lobbyShop_Name == "Crystal_480")
        {
            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate {StatisticName = "Crystal", Value = int.Parse((crystal + 480).ToString())},
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
            lobbyShop_Purchase_Success.SetActive(true);
        }
        else if (lobbyShop_Name == "Crystal_1200")
        {
            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate {StatisticName = "Crystal", Value = int.Parse((crystal + 1200).ToString())},
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
            lobbyShop_Purchase_Success.SetActive(true);
        }
        else if (lobbyShop_Name == "Crystal_2100")
        {
            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate {StatisticName = "Crystal", Value = int.Parse((crystal + 2100).ToString())},
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
            lobbyShop_Purchase_Success.SetActive(true);
        }
    }

    public void LobbyShop_Purchase_Success_Ok()
    {
        lobbyShop_Purchase_Success.SetActive(false);
        lobbyShop_Purchase.SetActive(false);
    }

    public void LobbyShop_Purchase_Fail_Ok()
    {
        lobbyShop_Purchase_Fail.SetActive(false);
        lobbyShop_Purchase.SetActive(false);
    }

    public void LobbyInventory()
    {
        lobbyInventory.SetActive(true);
    }

    public void LobbyRanking()
    {
        lobbyRanking.SetActive(true);
    }

    public void LobbyStart()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void LobbyStart2()
    {
        isTest = true;
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
        if (isTest == true)
        {
            isTest = false;
            roomLoading.SetActive(true);
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                StartCoroutine(RoomLoadingDelay());
            }
        }
        else if (isTest == false)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                roomLoading.SetActive(true);
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    StartCoroutine(RoomLoadingDelay());
                }
            }
        }
    }

    IEnumerator RoomLoadingDelay()
    {
        float time = 0;
        while (time < 100)
        {
            yield return new WaitForEndOfFrame();
            time += 30.0f * Time.deltaTime;
            PV.RPC("RoomLoadingCountRpc", RpcTarget.All, time);
        }
        yield return null;
    }

    [PunRPC]
    public void RoomLoadingCountRpc(float time)
    {
        roomLoading_Slider.GetComponent<Slider>().value = time * 0.01f;
        roomLoading_Slider.transform.GetChild(1).GetComponent<TMP_Text>().text = time.ToString("0") + "%";
        if(time >= 100)
        {
            gameManager.SetActive(true);
            inGame.SetActive(true);
            main.SetActive(false);
            room.SetActive(false);
            roomLoading.SetActive(false);
        }
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
        if (lastCanvas == "login")
        {
            login.SetActive(false);
            title.SetActive(true);
        }
        else if (lastCanvas == "signUp")
        {
            signUp.SetActive(false);
            login.SetActive(true);
        }
        else if (lastCanvas == "error")
        {
            if (errorType == "login")
            {
                error.SetActive(false);
                login.SetActive(true);
            }
            else if (errorType == "signUp")
            {
                error.SetActive(false);
                signUp.SetActive(true);
            }
            else if (errorType == "userInfo_ChangeName_Name_Empty" || errorType == "userInfo_ChangeName_Name_Crystal")
            {
                error.SetActive(false);
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
        else if (lastCanvas == "lobbyShop")
        {
            lobbyShop.SetActive(false);
        }
        else if (lastCanvas == "lobbyShop_Purchase")
        {
            lobbyShop_Purchase.SetActive(false);
        }
        else if (lastCanvas == "lobbyInventory")
        {
            lobbyInventory.SetActive(false);
        }
        else if (lastCanvas == "lobbyRanking")
        {
            lobbyRanking.SetActive(false);
        }
    }

    void LastCanvas()
    {
        if (title.activeSelf == true)
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
                    if (error.activeSelf == true)
                    {
                        lastCanvas = "error";
                    }
                    else
                    {
                        lastCanvas = "userInfo_ChangeName";
                    }
                }
                else
                {
                    lastCanvas = "userInfo";
                }
            }
            else if(lobbyShop.activeSelf == true)
            {
                if(lobbyShop_Purchase.activeSelf == true)
                {
                    lastCanvas = "lobbyShop_Purchase";
                }
                else
                {
                    lastCanvas = "lobbyShop";
                }
            }
            else if (lobbyInventory.activeSelf == true)
            {
                lastCanvas = "lobbyInventory";
            }
            else if (lobbyRanking.activeSelf == true)
            {
                lastCanvas = "lobbyRanking";
            }
            else
            {
                lastCanvas = "lobby";
            }
        }
    }

    void SetData()
    {
        lobbyName.text = nickName;
        lobbyLevel.text = level.ToString();
        lobbyExp.text = exp.ToString() + "/500";
        lobbyExp_Slider.GetComponent<Slider>().value = exp / 500;
        lobbyGold.text = gold.ToString();
        lobbyCrystal.text = crystal.ToString();
        lobbyUserInfo_Level.text = level.ToString();
        lobbyUserInfo_Exp.text = exp.ToString() + "/500";
        lobbyUserInfo_Exp_Slider.GetComponent<Slider>().value = exp / 500;
        lobbyUserInfo_Name.text = nickName;
        lobbyUserInfo_Highest_Trophies.text = highest_Trophies.ToString();
        lobbyUserInfo_Most_Wins.text = most_Wins.ToString();
        lobbyUserInfo_1vs1.text = _1vs1.ToString();
        lobbyUserInfo_2vs2.text = _2vs2.ToString();
        lobbyUserInfo_Total_Play.text = total_Play.ToString();
        lobbyUserInfo_MVP.text = mvp.ToString();
        lobbyShop_Gold.text = gold.ToString();
        lobbyShop_Crystal.text = crystal.ToString();
        lobbyInventory_Gold.text = gold.ToString();
        lobbyInventory_Crystal.text = crystal.ToString();
        lobbyInventory_Name.text = nickName.ToString();
        lobbyInventory_Level.text = level.ToString();
        lobbyInventory_Exp.text = exp.ToString() + "/500";
    }

    public void OnApplicationQuit()
    {
        if (chatClient != null)
        {
            chatClient.Disconnect();
        }
    }

    public void OnConnected()
    {
        chatClient.Subscribe(channelName, 0);
        this.chatClient.SetOnlineStatus(ChatUserStatus.Online);
        Debug.Log("채팅 온라인");
    }

    private void SendChatMessage(string inputLine)
    {
        if (string.IsNullOrEmpty(inputLine))
        {
            return;
        }

        chatClient.PublishMessage(channelName, inputLine);
    }

    public void OnGetMessages(string channel, string[] senders, object[] messages)
    {
        if (channel.Equals(channelName))
        {
            ShowChannel(channelName);
        }
    }

    public void ShowChannel(string channelName)
    {
        if (string.IsNullOrEmpty(channelName))
        {
            Debug.Log("ShowChannel error");
            return;
        }

        ChatChannel channel = null;

        bool found = this.chatClient.TryGetChannel(channelName, out channel);

        if (!found)
        {
            Debug.Log("ShowChannel failed to find channel: " + channelName);
            return;
        }

        this.channelName = channelName;

        chatText.text = channel.ToStringMessages();

        for (int i = channel.Messages.Count - 1; i < channel.Messages.Count; i++)
        {
            ShowMessage(channel.Senders[i], channel.Messages[i]);
        }
    }

    public void ShowMessage(string sender, object message)
    {
        if (sender != loginID.text)
        {
            GameObject chat = Instantiate(otherChatPrefab, chatScroll.transform);
            chat.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = sender;
            chat.transform.GetChild(1).transform.GetChild(1).GetComponent<TMP_Text>().text = message.ToString();
        }
        else
        {
            GameObject chat = Instantiate(myChatPrefab, chatScroll.transform);
            chat.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = sender;
            chat.transform.GetChild(0).transform.GetChild(1).GetComponent<TMP_Text>().text = message.ToString();
        }
    }

    public void OnEnterSend()
    {
        SendChatMessage(lobbyChatInput.text);
        lobbyChatInput.text = "";
    }

    void LobbyChatEnter()
    {
        if (lobbyChat.activeSelf == true && Input.GetKeyDown(KeyCode.Return))
        {
            OnEnterSend();
        }
    }

    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {

    }

    public void OnDisconnected()
    {

    }

    public void OnChatStateChange(ChatState state)
    {

    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {

    }

    public void OnSubscribed(string[] channels, bool[] results)
    {

    }

    public void OnSubscribed(string channel, string[] users, Dictionary<object, object> properties)
    {

    }

    public void OnUnsubscribed(string[] channels)
    {

    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {

    }

    public void OnUserSubscribed(string channel, string user)
    {

    }

    public void OnUserUnsubscribed(string channel, string user)
    {

    }
}