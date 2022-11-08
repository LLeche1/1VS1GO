using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
//using UnityEngine.AddressableAssets;
//using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using System;
#if UNITY_IOS
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
using LocalNotification = UnityEngine.iOS.LocalNotification;
#endif

public class LobbyManager : MonoBehaviourPunCallbacks, IChatClientListener
{
    public GameObject gameManager;
    public GameObject main;
    public GameObject inGame;
    public GameObject title;
    public GameObject update;
    public TMP_Text updateSize;
    //private AsyncOperationHandle updateHandle;
    public GameObject updateLoading;
    public GameObject titleLoading;
    public GameObject login;
    public TMP_InputField loginID;
    public TMP_InputField loginPW;
    public TMP_InputField signUpID;
    public TMP_InputField signUpPW;
    public TMP_InputField signUpNickName;
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
    public GameObject lobbySet_Push_No;
    public GameObject lobbySet_Push_Yes;
    public AudioMixer audioMixer;
    public Slider lobbySet_Fx;
    public Slider lobbySet_Music;
    public GameObject lobbySet_Vibration_No;
    public GameObject lobbySet_Vibration_Yes;
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
    public GameObject lobbyRanking_Scroll;
    public GameObject rankingPrefab;
    public TMP_Text lobbyRanking_Count;
    public TMP_Text lobbyRanking_Name;
    public TMP_Text lobbyRanking_Highest_Trophies;
    public GameObject lobbyResult;
    public TMP_Text lobbyResult_Text;
    public TMP_Text lobbyResult_Level;
    public TMP_Text lobbyResult_Exp;
    public GameObject lobbyResult_Exp_Slider;
    public TMP_Text lobbyResult_Gold;
    public TMP_Text lobbyResult_Crystal;
    public TMP_Text lobbyResult_Trophy;
    public GameObject lobbyResult_Continue;
    public GameObject lobbyLevelUp;
    public TMP_Text lobbyLevelUp_Level;
    public TMP_Text lobbyLevelUp_Gold;
    public TMP_Text lobbyLevelUp_Crystal;
    public GameObject error;
    public TMP_Text errorInfo;
    public GameObject errorNetwork;
    public GameObject lobby1vs1;
    public TMP_Text lobby1vs1_Count;
    public GameObject roomLoading;
    public GameObject roomLoading_Slider;
    public float level;
    public float exp;
    public float maxExp;
    public string nickName;
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
    private int isPush = 0;
    public float fxValue = 1;
    public float musicValue = 1;
    public int isVibration = 0;
    private bool isMatching = false;
    public ChatClient chatClient;
    private string channelName = "Global";
    public GameObject myChatPrefab;
    public GameObject otherChatPrefab;
    public GameObject chatScroll;
    public TMP_Text chatText;
    public GameManager game_Manager;
    PhotonView PV;

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
        #if UNITY_IOS
        NotificationServices.ClearLocalNotifications();
        NotificationServices.CancelAllLocalNotifications();
        NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
        #endif
        LoginLoad(loginRememberMe);
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.KeepAliveInBackground = 100;
        PhotonNetwork.ConnectUsingSettings();
        titleLoading.SetActive(true);
        StartCoroutine(TitleLoadDelay());
        /*Addressables.GetDownloadSizeAsync("default").Completed +=
            (AsyncOperationHandle<long> SizeHandle) =>
            {
                float size1 = 0;
                string size2 = null;
                if(SizeHandle.Result >= 1024)
                {
                    if(SizeHandle.Result >= 1048576)
                    {
                        size1 = SizeHandle.Result / 1048576;
                        size2 = size1.ToString();
                        updateSize.text = "Size : " + size2 + " MB";
                    }
                    else
                    {
                        size1 = SizeHandle.Result / 1024;
                        size2 = size1.ToString();
                        updateSize.text = "Size : " + size2 + " KB";
                    }
                }
                Addressables.Release(SizeHandle);
                if(SizeHandle.Result == 0)
                {
                    titleLoading.SetActive(true);
                    StartCoroutine(TitleLoadDelay());
                }
                else 
                {
                    update.SetActive(true);
                }
            };*/
    }

    void Update()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Service();
        }

        if (PhotonNetwork.IsConnected)
        {
            LastCanvas();
            LobbyChatEnter();
            SetData();
        }
    }
    
    /*public void UpdateHandle()
    {
        update.transform.Find("Popup").transform.Find("Button_Update").gameObject.SetActive(false);
        update.transform.Find("Popup").transform.Find("Slider_Loading").gameObject.SetActive(true);
        
        updateHandle = Addressables.DownloadDependenciesAsync("default");
        StartCoroutine(UpdateProgress());
        updateHandle.Completed +=
            (AsyncOperationHandle Handle) =>
            {
                Debug.Log("다운로드 완료!");
                Addressables.Release(Handle);
                SceneManager.LoadScene("Main");
            };
    }

    IEnumerator UpdateProgress()
    {
        float percent = 0;
        while(percent < 100)
        {
            yield return new WaitForEndOfFrame();
            percent = updateHandle.PercentComplete * 100;
            updateLoading.transform.GetChild(1).GetComponent<TMP_Text>().text = percent.ToString("0") + "%";
            updateLoading.GetComponent<Slider>().value = percent * 0.01f;
        }
        yield return null;
    }*/

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
                    else if (eachStat.StatisticName == "MaxExp")
                    {
                        maxExp = eachStat.Value;
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
        var request = new RegisterPlayFabUserRequest { Username = signUpID.text, Password = signUpPW.text, DisplayName = signUpNickName.text, RequireBothUsernameAndEmail = false };
        PlayFabClientAPI.RegisterPlayFabUser(request, RegisterSuccess, RegisterFailure);
    }

    private void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        signUp.SetActive(false);
        login.SetActive(true);
        Debug.Log("가입 성공");
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "NickName", signUpNickName.text }
            }
        },
        (result) => print("데이터 저장 성공"),
        (error) => print("데이터 저장 실패"));

        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate {StatisticName = "Level", Value = 1},
                new StatisticUpdate {StatisticName = "Exp", Value = 0},
                new StatisticUpdate {StatisticName = "MaxExp", Value = 10},
                new StatisticUpdate {StatisticName = "Gold", Value = 0},
                new StatisticUpdate {StatisticName = "Crystal", Value = 0},
                new StatisticUpdate {StatisticName = "Highest_Trophies", Value = 0},
                new StatisticUpdate {StatisticName = "Most_Wins", Value = 0},
                new StatisticUpdate {StatisticName = "1vs1", Value = 0},
                new StatisticUpdate {StatisticName = "2vs2", Value = 0},
                new StatisticUpdate {StatisticName = "Total_Play", Value = 0},
                new StatisticUpdate {StatisticName = "MVP", Value = 0}
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
        signUpNickName.text = null;
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
        if(isPush == 0)
        {
            lobbySet_Push_Yes.SetActive(false);
            lobbySet_Push_No.SetActive(true);
        }
        else if (isPush == 1)
        {
            lobbySet_Push_Yes.SetActive(true);
            lobbySet_Push_No.SetActive(false);
        }

        lobbySet_Fx.value = fxValue;
        lobbySet_Music.value = musicValue;

        if (isVibration == 0)
        {
            lobbySet_Vibration_Yes.SetActive(false);
            lobbySet_Vibration_No.SetActive(true);
        }
        else if (isVibration == 1)
        {
            lobbySet_Vibration_Yes.SetActive(true);
            lobbySet_Vibration_No.SetActive(false);
        }

        lobbySet.SetActive(true);
    }

    public void LobbySet_Push()
    {
        if(isPush == 0)
        {
            lobbySet_Push_Yes.SetActive(true);
            lobbySet_Push_No.SetActive(false);
            isPush = 1;
            PlayerPrefs.SetInt("isPush", isPush);
        }
        else if (isPush == 1)
        {
            lobbySet_Push_Yes.SetActive(false);
            lobbySet_Push_No.SetActive(true);
            isPush = 0;
            PlayerPrefs.SetInt("isPush", isPush);
        }
    }

    public void LobbySet_Fx()
    {
        audioMixer.SetFloat("FX", Mathf.Log10(lobbySet_Fx.value) * 20);
        PlayerPrefs.SetFloat("fxValue", lobbySet_Fx.value);
        fxValue = PlayerPrefs.GetFloat("fxValue");
    }

    public void LobbySet_Music()
    {
        audioMixer.SetFloat("Music", Mathf.Log10(lobbySet_Music.value) * 20);
        PlayerPrefs.SetFloat("musicValue", lobbySet_Music.value);
        musicValue = PlayerPrefs.GetFloat("musicValue");
    }

    public void LobbySet_Vibration()
    {
        if (isVibration == 0)
        {
            lobbySet_Vibration_Yes.SetActive(true);
            lobbySet_Vibration_No.SetActive(false);
            isVibration = 1;
            PlayerPrefs.SetInt("isVibration", isVibration);
        }
        else if (isVibration == 1)
        {
            lobbySet_Vibration_Yes.SetActive(false);
            lobbySet_Vibration_No.SetActive(true);
            isVibration = 0;
            PlayerPrefs.SetInt("isVibration", isVibration);
        }
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

    public void LobbyUserInfo()
    {
        userInfo.SetActive(true);
    }

    public void LobbyUserInfo_ChangeName()
    {
        userInfo_ChangeName.SetActive(true);
    }

    public void LobbyUserInfo_ChangeName_Name()
    {
        if (userInfo_ChangeName_Name.text != "" && crystal >= 10)
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
            {
                { "NickName", userInfo_ChangeName_Name.text }
            }
            },
            (result) => PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
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
                (error) => Debug.Log("데이터 불러오기 실패")),
            (error) => Debug.Log("데이터 저장 실패"));

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

            PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = userInfo_ChangeName_Name.text
            },
            result => {
               
            },
            error => { Debug.Log("값 저장 실패"); });

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
        var child = lobbyRanking_Scroll.GetComponentsInChildren<Transform>();

        foreach (var item in child)
        {
            if(item.name != "Content")
            {
                Destroy(item.gameObject);
            }
        }

        lobbyRanking_Count.transform.GetChild(0).gameObject.SetActive(false);
        lobbyRanking_Count.transform.GetChild(1).gameObject.SetActive(false);
        lobbyRanking_Count.transform.GetChild(2).gameObject.SetActive(false);

        lobbyRanking.SetActive(true);
        GetLeaderboard();
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest { StatisticName = "Highest_Trophies", MaxResultsCount = 9 };
        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderBoard, OnErrorLeaderBoard);
    }

    void OnGetLeaderBoard(GetLeaderboardResult result)
    {
        int count = 0;
        foreach (PlayerLeaderboardEntry player in result.Leaderboard)
        {
            count++;
            GameObject ranking = Instantiate(rankingPrefab, lobbyRanking_Scroll.transform);
            ranking.transform.GetChild(0).GetComponent<TMP_Text>().text = count.ToString();
            ranking.transform.GetChild(2).GetComponent<TMP_Text>().text = player.DisplayName;
            ranking.transform.GetChild(3).GetComponent<TMP_Text>().text = player.StatValue.ToString();
            if (count == 1)
            {
                ranking.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            }
            else if (count == 2)
            {
                ranking.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            }
            else if (count == 3)
            {
                ranking.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
            }

            if (player.DisplayName == nickName)
            {
                lobbyRanking_Count.text = count.ToString();
                if(count == 1)
                {
                    lobbyRanking_Count.transform.GetChild(0).gameObject.SetActive(true);
                }
                else if (count == 2)
                {
                    lobbyRanking_Count.transform.GetChild(1).gameObject.SetActive(true);
                }
                else if (count == 3)
                {
                    lobbyRanking_Count.transform.GetChild(2).gameObject.SetActive(true);
                }
            }
        }
    }

    void OnErrorLeaderBoard(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    public void LobbyStart()
    {
        if (isMatching == false)
        {
            lobby1vs1.transform.GetChild(0).gameObject.SetActive(false);
            lobby1vs1.transform.GetChild(1).gameObject.SetActive(false);
            lobby1vs1.transform.GetChild(2).gameObject.SetActive(true);
            lobby1vs1.transform.GetChild(3).gameObject.SetActive(true);
            isMatching = true;
            PhotonNetwork.JoinRandomRoom();
            StartCoroutine(LobbyMatchingCount());
        }
        else if (isMatching == true)
        {
            lobby1vs1.transform.GetChild(0).gameObject.SetActive(true);
            lobby1vs1.transform.GetChild(1).gameObject.SetActive(true);
            lobby1vs1.transform.GetChild(2).gameObject.SetActive(false);
            lobby1vs1.transform.GetChild(3).gameObject.SetActive(false);
            isMatching = false;
            PhotonNetwork.LeaveRoom();
        }
    }

    IEnumerator LobbyMatchingCount()
    {
        float time = 0;
        while (roomLoading.activeSelf == false)
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
            lobby1vs1_Count.text = TimeSpan.FromSeconds(time).ToString(@"m\:ss");
        }
        yield return null;
    }

    public override void OnJoinedRoom()
    {
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
        if (isVibration == 1)
        {
            isMatching = false;
            lobby1vs1.transform.GetChild(0).gameObject.SetActive(true);
            lobby1vs1.transform.GetChild(1).gameObject.SetActive(true);
            lobby1vs1.transform.GetChild(2).gameObject.SetActive(false);
            lobby1vs1.transform.GetChild(3).gameObject.SetActive(false);
            roomLoading.SetActive(true);
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                StartCoroutine(RoomLoadingDelay());
            }
        }
        else if (isVibration == 0)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                isMatching = false;
                lobby1vs1.transform.GetChild(0).gameObject.SetActive(true);
                lobby1vs1.transform.GetChild(1).gameObject.SetActive(true);
                lobby1vs1.transform.GetChild(2).gameObject.SetActive(false);
                lobby1vs1.transform.GetChild(3).gameObject.SetActive(false);
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
            roomLoading.SetActive(false);
        }
    }

    public void LobbyResult()
    {
        main.SetActive(true);
        float preLevel = level;
        float preExp = exp;
        float preMaxExp = maxExp;
        lobbyResult_Level.text = level.ToString();
        lobbyResult_Exp.text = preExp.ToString() + "/" + maxExp;
        lobbyResult_Exp_Slider.GetComponent<Slider>().value = preExp / maxExp;

        if (game_Manager.isWin == 0)
        {
            lobbyResult.transform.GetChild(1).gameObject.SetActive(false);
            lobbyResult_Text.text = "Defeat";
            lobbyResult_Gold.text = "300";
            lobbyResult_Crystal.text = "0";
            if(highest_Trophies >= 10)
            {
                lobbyResult_Trophy.text = "-10";
            }
            else if (highest_Trophies < 10)
            {
                lobbyResult_Trophy.text = "-" + highest_Trophies;
            }


            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate {StatisticName = "Gold", Value = int.Parse((gold + 300).ToString())},
                    new StatisticUpdate {StatisticName = "Highest_Trophies", Value = int.Parse((highest_Trophies - 10).ToString())},
                    new StatisticUpdate {StatisticName = "Exp", Value = int.Parse((exp + 3).ToString())},
                    new StatisticUpdate {StatisticName = "Total_Play", Value = int.Parse((total_Play + 1).ToString())}
                }
            },
            (result) =>
            {
                PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
(result) =>
{
    foreach (var eachStat in result.Statistics)
    {
        if (eachStat.StatisticName == "Gold")
        {
            gold = eachStat.Value;
        }
        else if (eachStat.StatisticName == "Highest_Trophies")
        {
            highest_Trophies = eachStat.Value;
            if (highest_Trophies < 0)
            {
                PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                                new StatisticUpdate {StatisticName = "Highest_Trophies", Value = 0}
                    }
                },
                (result) => {
                    PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
                    (result) =>
                    {
                        foreach (var eachStat in result.Statistics)
                        {
                            if (eachStat.StatisticName == "Highest_Trophies")
                            {
                                highest_Trophies = eachStat.Value;
                            }
                        }
                    },
                    (error) => { Debug.Log("값 로딩 실패"); });
                },
            (error) => { Debug.Log("값 저장 실패"); });
            }
        }
        else if (eachStat.StatisticName == "Exp")
        {
            exp = eachStat.Value;
            if (exp >= maxExp)
            {
                PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                                new StatisticUpdate {StatisticName = "Level", Value = int.Parse((level + 1).ToString())},
                                new StatisticUpdate {StatisticName = "Exp", Value = int.Parse((exp - maxExp).ToString())},
                                new StatisticUpdate {StatisticName = "MaxExp", Value = int.Parse((10 + (level * 10)).ToString())}
                    }
                },
                (result) => {
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
                            else if (eachStat.StatisticName == "MaxExp")
                            {
                                maxExp = eachStat.Value;
                            }
                        }
                    },
                    (error) => { Debug.Log("값 로딩 실패"); });
                },
                (error) => { Debug.Log("값 저장 실패"); });
            }
        }
        else if (eachStat.StatisticName == "Total_Play")
        {
            total_Play = eachStat.Value;
        }
    }
},
(error) => { Debug.Log("값 로딩 실패"); });
            },
            (error) => { Debug.Log("값 저장 실패"); });
        }
        else if (game_Manager.isWin == 1)
        {
            lobbyResult.transform.GetChild(1).gameObject.SetActive(true);
            lobbyResult_Text.text = "Victory";
            lobbyResult_Gold.text = "1000";
            lobbyResult_Crystal.text = "10";
            lobbyResult_Trophy.text = "10";

            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate {StatisticName = "Gold", Value = int.Parse((gold + 1000).ToString())},
                    new StatisticUpdate {StatisticName = "Crystal", Value = int.Parse((crystal + 10).ToString())},
                    new StatisticUpdate {StatisticName = "Highest_Trophies", Value = int.Parse((highest_Trophies + 10).ToString())},
                    new StatisticUpdate {StatisticName = "Exp", Value = int.Parse((exp + 10).ToString())},
                    new StatisticUpdate {StatisticName = "Total_Play", Value = int.Parse((total_Play + 1).ToString())},
                    new StatisticUpdate {StatisticName = "1vs1", Value = int.Parse((_1vs1 + 1).ToString())}
                }
            },
            (result) =>
            {
                PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
(result) =>
{
    foreach (var eachStat in result.Statistics)
    {
        if (eachStat.StatisticName == "Gold")
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
            if (highest_Trophies < 0)
            {
                PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                                new StatisticUpdate {StatisticName = "Highest_Trophies", Value = 0}
                    }
                },
                (result) => {
                    PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
                    (result) =>
                    {
                        foreach (var eachStat in result.Statistics)
                        {
                            if (eachStat.StatisticName == "Highest_Trophies")
                            {
                                highest_Trophies = eachStat.Value;
                            }
                        }
                    },
                    (error) => { Debug.Log("값 로딩 실패"); });
                },
            (error) => { Debug.Log("값 저장 실패"); });
            }
        }
        else if (eachStat.StatisticName == "Exp")
        {
            exp = eachStat.Value;
            if (exp >= maxExp)
            {
                PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                                new StatisticUpdate {StatisticName = "Level", Value = int.Parse((level + 1).ToString())},
                                new StatisticUpdate {StatisticName = "Exp", Value = int.Parse((exp - maxExp).ToString())},
                                new StatisticUpdate {StatisticName = "MaxExp", Value = int.Parse((10 + (level * 10)).ToString())}
                    }
                },
                (result) => {
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
                            else if (eachStat.StatisticName == "MaxExp")
                            {
                                maxExp = eachStat.Value;
                            }
                        }
                    },
                    (error) => { Debug.Log("값 로딩 실패"); });
                },
                (error) => { Debug.Log("값 저장 실패"); });
            }
        }
        else if (eachStat.StatisticName == "Total_Play")
        {
            total_Play = eachStat.Value;
        }
        else if (eachStat.StatisticName == "1vs1")
        {
            _1vs1 = eachStat.Value;
        }
    }
},
(error) => { Debug.Log("값 로딩 실패"); });
            },
            (error) => { Debug.Log("값 저장 실패"); });
        }
        else if (game_Manager.isWin == 2)
        {
            lobbyResult.transform.GetChild(1).gameObject.SetActive(false);
            lobbyResult_Text.text = "Draw";
            lobbyResult_Gold.text = "500";
            lobbyResult_Crystal.text = "3";
            lobbyResult_Trophy.text = "3";

            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate {StatisticName = "Gold", Value = int.Parse((gold + 500).ToString())},
                    new StatisticUpdate {StatisticName = "Crystal", Value = int.Parse((crystal + 3).ToString())},
                    new StatisticUpdate {StatisticName = "Highest_Trophies", Value = int.Parse((highest_Trophies + 3).ToString())},
                    new StatisticUpdate {StatisticName = "Exp", Value = int.Parse((exp + 5).ToString())},
                    new StatisticUpdate {StatisticName = "Total_Play", Value = int.Parse((total_Play + 1).ToString())}
                }
            },
            (result) =>
            {
                PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
(result) =>
{
    foreach (var eachStat in result.Statistics)
    {
        if (eachStat.StatisticName == "Gold")
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
            if (highest_Trophies < 0)
            {
                PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                                new StatisticUpdate {StatisticName = "Highest_Trophies", Value = 0}
                    }
                },
                (result) => {
                    PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
                    (result) =>
                    {
                        foreach (var eachStat in result.Statistics)
                        {
                            if (eachStat.StatisticName == "Highest_Trophies")
                            {
                                highest_Trophies = eachStat.Value;
                            }
                        }
                    },
                    (error) => { Debug.Log("값 로딩 실패"); });
                },
            (error) => { Debug.Log("값 저장 실패"); });
            }
        }
        else if (eachStat.StatisticName == "Exp")
        {
            exp = eachStat.Value;
            if (exp >= maxExp)
            {
                PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                                new StatisticUpdate {StatisticName = "Level", Value = int.Parse((level + 1).ToString())},
                                new StatisticUpdate {StatisticName = "Exp", Value = int.Parse((exp - maxExp).ToString())},
                                new StatisticUpdate {StatisticName = "MaxExp", Value = int.Parse((10 + (level * 10)).ToString())}
                    }
                },
                (result) => {
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
                            else if (eachStat.StatisticName == "MaxExp")
                            {
                                maxExp = eachStat.Value;
                            }
                        }
                    },
                    (error) => { Debug.Log("값 로딩 실패"); });
                },
                (error) => { Debug.Log("값 저장 실패"); });
            }
        }
        else if (eachStat.StatisticName == "Total_Play")
        {
            total_Play = eachStat.Value;
        }
    }
},
(error) => { Debug.Log("값 로딩 실패"); });
            },
            (error) => { Debug.Log("값 저장 실패"); });
        }

        lobbyResult.SetActive(true);
        StartCoroutine(LobbyResult_Exp(preLevel, preExp, preMaxExp));
    }

    IEnumerator LobbyResult_Exp(float preLevel, float preExp, float preMaxExp)
    {
        float time = 0;
        bool isLevelUp = false;
        while (time < 1)
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }

        while (preLevel != level)
        {
            float delay = 0;
            if(game_Manager.isWin == 0)
            {
                preExp += Time.deltaTime;
            }
            else if(game_Manager.isWin == 1)
            {
                preExp += 2.5f * Time.deltaTime;
            }
            else if(game_Manager.isWin == 2)
            {
                preExp += 1.5f * Time.deltaTime;
            }
            lobbyResult_Level.text = preLevel.ToString();
            lobbyResult_Exp.text = preExp.ToString("0") + "/" + preMaxExp;
            lobbyResult_Exp_Slider.GetComponent<Slider>().value = preExp / preMaxExp;

            while (delay < 0.001f)
            {
                if (game_Manager.isWin == 0)
                {
                    yield return new WaitForEndOfFrame();
                    delay += Time.deltaTime;
                }
                else if (game_Manager.isWin == 1)
                {
                    yield return new WaitForEndOfFrame();
                    delay += Time.deltaTime;
                }
                else if (game_Manager.isWin == 2)
                {
                    yield return new WaitForEndOfFrame();
                    delay += Time.deltaTime;
                }
            }

            if (preExp >= preMaxExp)
            {
                preExp = 0;
                preMaxExp = preMaxExp * (preLevel + 1);
                preLevel++;
                isLevelUp = true;
            }
        }

        if (preLevel == level)
        {
            while (preExp < exp)
            {
                float delay = 0;
                if(game_Manager.isWin == 0)
                {
                    preExp += Time.deltaTime;
                }
                else if(game_Manager.isWin == 1)
                {
                    preExp += 2.5f * Time.deltaTime;
                }
                else if(game_Manager.isWin == 2)
                {
                    preExp += 1.5f * Time.deltaTime;
                }
                lobbyResult_Level.text = preLevel.ToString();
                lobbyResult_Exp.text = preExp.ToString("0") + "/" + preMaxExp;
                lobbyResult_Exp_Slider.GetComponent<Slider>().value = preExp / preMaxExp;

                while (delay < 0.001f)
                {
                    if (game_Manager.isWin == 0)
                    {
                        yield return new WaitForEndOfFrame();
                        delay += Time.deltaTime;
                    }
                    else if (game_Manager.isWin == 1)
                    {
                        yield return new WaitForEndOfFrame();
                        delay += Time.deltaTime;
                    }
                    else if (game_Manager.isWin == 2)
                    {
                        yield return new WaitForEndOfFrame();
                        delay += Time.deltaTime;
                    }
                }
            }
        }

        if(isLevelUp == true)
        {
            lobbyLevelUp.SetActive(true);
            LobbyLevelUp();
            isLevelUp = false;
        }

        lobbyResult_Continue.SetActive(true);
        yield return null;
    }

    void LobbyLevelUp()
    {
        lobbyLevelUp_Level.text = level.ToString();
        lobbyLevelUp_Gold.text = 5000.ToString();
        lobbyLevelUp_Crystal.text = 50.ToString();

        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate {StatisticName = "Gold", Value = int.Parse((gold + 5000).ToString())},
                new StatisticUpdate {StatisticName = "Crystal", Value = int.Parse((crystal + 50).ToString())}
            }
        },
        (result) =>
        {
            PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
                (result) =>
                {
                    foreach (var eachStat in result.Statistics)
                    {
                        if (eachStat.StatisticName == "Gold")
                        {
                            gold = eachStat.Value;
                        }
                        else if (eachStat.StatisticName == "Crystal")
                        {
                            crystal = eachStat.Value;
                        }
                    }
                },
                (error) => { Debug.Log("값 로딩 실패"); });
            },
        (error) => { Debug.Log("값 저장 실패"); });
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

        isPush = PlayerPrefs.GetInt("isPush");

        if(PlayerPrefs.GetFloat("fxValue") != 0)
        {
            fxValue = PlayerPrefs.GetFloat("fxValue");
        }

        if (PlayerPrefs.GetFloat("musicValue") != 0)
        {
            musicValue = PlayerPrefs.GetFloat("musicValue");
        }

        isVibration = PlayerPrefs.GetInt("isVibration");
        audioMixer.SetFloat("FX", Mathf.Log10(fxValue) * 20);
        audioMixer.SetFloat("Music", Mathf.Log10(musicValue) * 20);
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
        else if (lastCanvas == "lobbyResult")
        {
            PhotonNetwork.LeaveRoom();
            inGame.SetActive(false);
            game_Manager.Reset();
            lobbyResult_Continue.SetActive(false);
            lobbyResult.SetActive(false);
        }
        else if (lastCanvas == "lobbyLevelUp")
        {
            lobbyLevelUp.SetActive(false);
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
        else if (lobby.activeSelf == true)
        {
            if (lobbySet.activeSelf == true)
            {
                lastCanvas = "lobbySet";
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
            else if (lobbyResult.activeSelf == true)
            {
                if (lobbyLevelUp.activeSelf == true)
                {
                    lastCanvas = "lobbyLevelUp";
                }
                else
                {
                    lastCanvas = "lobbyResult";
                }
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
        lobbyExp.text = exp.ToString() + "/" + maxExp;
        lobbyExp_Slider.GetComponent<Slider>().value = exp / maxExp;
        if(lobby.activeSelf == true)
        {
            StartCoroutine(LobbyExp_Delay());
        }
        lobbyGold.text = gold.ToString();
        lobbyCrystal.text = crystal.ToString();
        lobbyUserInfo_Level.text = level.ToString();
        lobbyUserInfo_Exp.text = exp.ToString() + "/" + maxExp;
        lobbyUserInfo_Exp_Slider.GetComponent<Slider>().value = exp / maxExp;
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
        lobbyInventory_Name.text = nickName;
        lobbyInventory_Level.text = level.ToString();
        lobbyInventory_Exp.text = exp.ToString() + "/" + maxExp;
        lobbyRanking_Name.text = nickName;
        lobbyRanking_Highest_Trophies.text = highest_Trophies.ToString();
    }

    IEnumerator LobbyExp_Delay()
    {
        float time = 0;
        while(time < 1f)
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
        lobbyExp_Slider.GetComponent<Slider>().enabled = true;
        yield return null;
    }

    public void Notification()
    {
        #if UNITY_IOS
        UnityEngine.iOS.LocalNotification notice = new UnityEngine.iOS.LocalNotification();
        notice.alertTitle = "알림";
        notice.alertBody = "게임 종료";
        notice.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
        notice.applicationIconBadgeNumber = 1;
        notice.fireDate = DateTime.Now.AddSeconds(3);
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notice);
        #endif  
    }

    public void OnApplicationQuit()
    {
        if (chatClient != null)
        {
            chatClient.Disconnect();
        }

        #if UNITY_IOS
        if(isPush == 1)
        {
            Notification();
        }
        #endif
    }

    // 채팅서버
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