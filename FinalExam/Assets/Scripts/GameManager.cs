using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Material[] Skyboxes;
    public GameObject tutorialPlayer;
    public GameObject cameraObject;
    public GameObject tutorial;
    public GameObject runningGame;
    public GameObject cannonGame;
    public GameObject speedGame;
    public GameObject ui;
    public GameObject fade;
    public GameObject[] players;
    public GameObject joystick;
    public GameObject pause;
    public GameObject set;
    public AudioMixer audioMixer;
    public Slider set_Fx;
    public Slider set_Music;
    public GameObject set_Vibration_Yes;
    public GameObject set_Vibration_No;
    public TMP_Text timeText;
    public TMP_Text myScoreText;
    public TMP_Text otherScoreText;
    private float limitTime;
    public bool isTutorial = false;
    public bool isTutorial2 = false;
    public bool isTutorial4 = false;
    public bool isTutorial6 = false;
    public int tutorialNum = 0;
    public bool isStart = false;
    public bool isFinish = false;
    private bool isGiveUp = false;
    public bool redReady = false;
    public bool blueReady = false;
    private string lastCanvas;
    public int blueScore = 0;
    public int redScore = 0;
    public int blueRound = 0;
    public int redRound = 0;
    public int isWin = 0;
    public int random = 0;
    public List<int> randomList = new List<int>{1, 2, 3, 1, 2};
    public int randomNum = 5;
    private string team = null;
    public bool isRandom = false;
    PhotonView PV;
    LobbyManager lobbyManager;
    public GameObject joyStick;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
    }

    void Update()
    {
        if(lobbyManager.isTutorial == 1)
        {
            Tutorial();
        }
        else if(lobbyManager.isTutorial == 2)
        {
            if(PhotonNetwork.IsMasterClient && isRandom == false)
            {
                RoundStart();
            }

            if (isStart == true)
            {
                Score();
                PV.RPC("Statue", RpcTarget.All);
                if (PhotonNetwork.IsMasterClient)
                {
                    limitTime -= Time.deltaTime;
                    PV.RPC("LimitTime", RpcTarget.All, limitTime);
                }
            }

            RenderSettings.skybox.SetFloat("_Rotation", Time.time * 2f);
            LastCanvas();
        }
    }

    void Tutorial()
    {
        if(isTutorial == false && tutorialNum == 0)
        {
            isTutorial = true;
            tutorial.SetActive(true);
            isStart = true;
            GameObject player = Instantiate(tutorialPlayer, Vector3.zero, Quaternion.identity);
            player.transform.parent = GameObject.Find("InGame").transform;
            player.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            cameraObject.GetComponent<CameraController>().player = player;
            cameraObject.transform.GetComponent<CameraController>().enabled = true;
            ui.SetActive(true);
            ui.transform.Find("Time_Score").gameObject.SetActive(false);
            ui.transform.Find("Button_Pause").gameObject.SetActive(false);
            ui.transform.Find("JoyStick").gameObject.SetActive(true);
            ui.transform.Find("Button_Jump").gameObject.SetActive(false);
            ui.transform.Find("Button_Slide").gameObject.SetActive(false);
            ui.transform.Find("Button_Attack").gameObject.SetActive(false);
            ui.transform.Find("Button_Run").gameObject.SetActive(false);
            ui.transform.Find("Tutorial").gameObject.SetActive(true);
            ui.transform.Find("Tutorial").transform.Find("Message").gameObject.SetActive(true);
            ui.transform.Find("Tutorial").transform.Find("Message").transform.Find("Chat").transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "Let`s Drag  joystick with your left thumb to get moving.";
            tutorialNum = 1;
        }
        else if(isTutorial2 == false && tutorialNum == 2)
        {
            if(joyStick.GetComponent<JoyStick>().lever.anchoredPosition.x != 0)
            {
                isTutorial2 = true;
                Invoke("TutorialBtn", 2f);
            }
        }
        else if(isTutorial4 == false && tutorialNum == 4)
        {
            GameObject btn = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            if(btn.name == "Button_Jump")
            {
                isTutorial4 = true;
                Invoke("TutorialBtn", 2f);
            }
        }
        else if(isTutorial6 == false && tutorialNum == 6)
        {
            GameObject btn = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            if(btn.name == "Button_Slide")
            {
                isTutorial6 = true;
                Invoke("TutorialBtn", 2f);
            }
        }
    }

    public void TutorialBtn()
    {
        if(tutorialNum == 1)
        {
            ui.transform.Find("Tutorial").transform.Find("Message").gameObject.SetActive(false);
            ui.transform.Find("Tutorial").transform.Find("Hand").gameObject.SetActive(true);
            ui.transform.Find("Tutorial").transform.Find("Hand").transform.Find("Fx").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-920, -235, 0);
            ui.transform.Find("Tutorial").transform.Find("Hand").transform.Find("Fx").gameObject.transform.localScale = new Vector3(70, 70, 70);
            ui.transform.Find("Tutorial").transform.Find("Hand").transform.Find("Image_Hand").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-1100, -50, 0);
            tutorialNum = 2;
        }
        else if(tutorialNum == 2)
        {
            ui.transform.Find("Tutorial").transform.Find("Message").gameObject.SetActive(true);
            ui.transform.Find("Tutorial").transform.Find("Hand").gameObject.SetActive(false);
            ui.transform.Find("Tutorial").transform.Find("Message").transform.Find("Chat").transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "Let`s touch  jump Button with your right thumb to get jumping.";
            ui.transform.Find("Button_Jump").gameObject.SetActive(true);
            tutorialNum = 3;
        }
        else if(tutorialNum == 3)
        {
            ui.transform.Find("Tutorial").transform.Find("Message").gameObject.SetActive(false);
            ui.transform.Find("Tutorial").transform.Find("Hand").gameObject.SetActive(true);
            ui.transform.Find("Tutorial").transform.Find("Hand").transform.Find("Fx").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(760, -330, 0);
            ui.transform.Find("Tutorial").transform.Find("Hand").transform.Find("Fx").gameObject.transform.localScale = new Vector3(60, 60, 60);
            ui.transform.Find("Tutorial").transform.Find("Hand").transform.Find("Image_Hand").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(430, -130, 0);
            tutorialNum = 4;
        }
        else if(tutorialNum == 4)
        {
            ui.transform.Find("Tutorial").transform.Find("Message").gameObject.SetActive(true);
            ui.transform.Find("Tutorial").transform.Find("Hand").gameObject.SetActive(false);
            ui.transform.Find("Tutorial").transform.Find("Message").transform.Find("Chat").transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "Let`s touch  Slider Button with your right thumb to get jumping.";
            ui.transform.Find("Button_Slide").gameObject.SetActive(true);
            tutorialNum = 5;
        }
        else if(tutorialNum == 5)
        {
            ui.transform.Find("Tutorial").transform.Find("Message").gameObject.SetActive(false);
            ui.transform.Find("Tutorial").transform.Find("Hand").gameObject.SetActive(true);
            ui.transform.Find("Tutorial").transform.Find("Hand").transform.Find("Fx").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(1065, -30, 0);
            ui.transform.Find("Tutorial").transform.Find("Hand").transform.Find("Fx").gameObject.transform.localScale = new Vector3(60, 60, 60);
            ui.transform.Find("Tutorial").transform.Find("Hand").transform.Find("Image_Hand").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(800, 200, 0);
            tutorialNum = 6;
        }
        else if(tutorialNum == 6)
        {
            ui.transform.Find("Tutorial").transform.Find("Message").gameObject.SetActive(true);
            ui.transform.Find("Tutorial").transform.Find("Hand").gameObject.SetActive(false);
            ui.transform.Find("Tutorial").transform.Find("Message").transform.Find("Chat").transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "Now, Let`s play game.";
            tutorialNum = 7;
        }
        else if(tutorialNum == 7)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Destroy(player);
            gameObject.SetActive(false);
            lobbyManager.main.SetActive(true);
            lobbyManager.inGame.SetActive(false);
            ui.transform.Find("Tutorial").gameObject.SetActive(false);
            ui.transform.Find("Time_Score").gameObject.SetActive(true);
            ui.transform.Find("Button_Pause").gameObject.SetActive(true);
            tutorial.SetActive(false);
            isTutorial = false;
            isStart = false;
            lobbyManager.TutorialFinish();
        }
    }

    void RoundStart()
    {
        isRandom = true;
        int i = Random.Range(0, randomNum);
        random = randomList[i];
        PV.RPC("RandomMap", RpcTarget.All, random);
        randomList.RemoveAt(i);
        randomNum--;
    }

    [PunRPC]
    void RandomMap(int rand)
    {
        random = rand;

        if (random == 1)
        {
            runningGame.SetActive(true);
            RenderSettings.skybox = Skyboxes[0];
            RenderSettings.skybox.SetFloat("_Rotation", 0);
        }
        else if (random == 2)
        {
            cannonGame.SetActive(true);
            cannonGame.transform.GetComponent<CannonGame>().randGenTrigger = true;
            cannonGame.transform.GetComponent<CannonGame>().lineGenTrigger = true;
            RenderSettings.skybox = Skyboxes[1];
            RenderSettings.skybox.SetFloat("_Rotation", 0);
            limitTime = 60;
        }
        else if (random == 3)
        {
            speedGame.SetActive(true);
            RenderSettings.skybox = Skyboxes[2];
            RenderSettings.skybox.SetFloat("_Rotation", 0);
            limitTime = 30;
        }
        
        Generate();
    }

    void Generate()
    {
        Vector3 position = Vector3.zero;

        if (random == 1)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                position = new Vector3(-8, 0, 10);
                team = "Blue";
            }
            else
            {
                position = new Vector3(0, 0, 10);
                team = "Red";
            }
        }
        else if (random == 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                position = new Vector3(4, 0, 0);
                team = "Blue";
            }
            else
            {
                position = new Vector3(-4, 0, 0);
                team = "Red";
            }
        }
        else if (random == 3)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                position = new Vector3(4, 0, -485);
                team = "Blue";
            }
            else
            {
                position = new Vector3(-4, 0, -485);
                team = "Red";
            }
        }

        GameObject player = PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
        player.name = lobbyManager.nickName;
        player.transform.GetComponent<PlayerController>().team = team;
        player.transform.parent = GameObject.Find("InGame").transform;
        //PV.RPC("Set", RpcTarget.All, player.GetComponent<PhotonView>().ViewID);
        cameraObject.GetComponent<CameraController>().player = player;
        cameraObject.transform.GetComponent<CameraController>().enabled = true;
        players = GameObject.FindGameObjectsWithTag("Player");
        StartCoroutine(Fade());
    }

    [PunRPC]
    void Set(int ID)
    {
        PhotonNetwork.GetPhotonView(ID).name = lobbyManager.nickName;
        PhotonNetwork.GetPhotonView(ID).transform.GetComponent<PlayerController>().team = team;
        PhotonNetwork.GetPhotonView(ID).transform.parent = GameObject.Find("InGame").transform;
    }

    IEnumerator Fade()
    {
        ui.SetActive(false);
        fade.SetActive(true);
        cameraObject.transform.position = cameraObject.GetComponent<CameraController>().player.transform.position - (Vector3.forward * 10) + (Vector3.up * 17);
        float count = 1;

        while (count > 0)
        {
            count -= 0.3f * Time.deltaTime;
            yield return new WaitForSeconds(0.01f);

            if (cameraObject.transform.position.y > 7.3f)
            {
                cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, cameraObject.transform.position.y - (Time.deltaTime * 3.1f), cameraObject.transform.position.z);
            }
            else if (cameraObject.transform.position.y < 7.3f)
            {
                cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, 7.3f, cameraObject.transform.position.z);
            }
        }

        if (random == 1)
        {
            ui.transform.Find("JoyStick").gameObject.SetActive(true);
            ui.transform.Find("Button_Jump").gameObject.SetActive(true);
            ui.transform.Find("Button_Slide").gameObject.SetActive(true);
            ui.transform.Find("Button_Attack").gameObject.SetActive(false);
            ui.transform.Find("Button_Run").gameObject.SetActive(false);
        }
        else if (random == 2)
        {
            ui.transform.Find("JoyStick").gameObject.SetActive(true);
            ui.transform.Find("Button_Jump").gameObject.SetActive(true);
            ui.transform.Find("Button_Slide").gameObject.SetActive(true);
            ui.transform.Find("Button_Attack").gameObject.SetActive(true);
            ui.transform.Find("Button_Run").gameObject.SetActive(false);
        }
        else if (random == 3)
        {
            ui.transform.Find("JoyStick").gameObject.SetActive(false);
            ui.transform.Find("Button_Jump").gameObject.SetActive(false);
            ui.transform.Find("Button_Slide").gameObject.SetActive(false);
            ui.transform.Find("Button_Attack").gameObject.SetActive(false);
            ui.transform.Find("Button_Run").gameObject.SetActive(true);
        }

        if (team == "Blue")
        {
            PV.RPC("BlueReady", RpcTarget.All);
        }
        else if (team == "Red")
        {
            PV.RPC("RedReady", RpcTarget.All);
        }

        bool check = false;

        if (lobbyManager.isVibration == 1)
        {
            check = true;
        }

        while (check == false)
        {
            yield return new WaitForSeconds(0.1f);
            if (blueReady == true && redReady == true)
            {
                check = true;
            }
        }

        players = GameObject.FindGameObjectsWithTag("Player");
        fade.SetActive(false);
        isStart = true;
        isFinish = false;
        ui.SetActive(true);
        blueReady = false;
        redReady = false;

        yield return null;
    }

    [PunRPC]
    void BlueReady()
    {
        blueReady = true;
    }

    [PunRPC]
    void RedReady()
    {
        redReady = true;
    }

    [PunRPC]
    void LimitTime(float limit)
    {
        limitTime = limit;
        if(random == 1)
        {
            timeText.text = "∞";
            timeText.fontSize = 100;
        }
        else
        {
            if(limitTime > 0)
            {
                timeText.text = TimeSpan.FromSeconds(limitTime).ToString(@"m\:ss");
                timeText.fontSize = 50;
            }
            else if(limitTime <= 0)
            {
                timeText.text = TimeSpan.FromSeconds(0).ToString(@"m\:ss");
                timeText.fontSize = 50;
            }
        }
    }

    void Score()
    {
        foreach (GameObject player in players)
        {
            if (player.name == lobbyManager.nickName)
            {
                if (player.GetComponent<PlayerController>().team == "Blue")
                {
                    myScoreText.text = blueScore.ToString();
                    otherScoreText.text = redScore.ToString();
                }
                else if (player.GetComponent<PlayerController>().team == "Red")
                {
                    myScoreText.text = redScore.ToString();
                    otherScoreText.text = blueScore.ToString();
                }
            }
        }
    }

    [PunRPC]
    void Statue()
    {
        if(isFinish == false)
        {
            foreach (GameObject player in players)
            {
                if (player.transform.position.y < -10)
                {
                    if (player.GetComponent<PlayerController>().team == "Blue")
                    {
                        PV.RPC("RedRound", RpcTarget.All);
                    }
                    else if (player.GetComponent<PlayerController>().team == "Red")
                    {
                        PV.RPC("BlueRound", RpcTarget.All);
                    }                    
                    
                    RoundCheck();
                }

                if (cannonGame.activeSelf == true)
                {
                    if (limitTime <= 0)
                    {
                        if (player.name == lobbyManager.nickName)
                        {
                            if (player.GetComponent<PlayerController>().team == "Blue")
                            {
                                if (blueScore > redScore)
                                {
                                    PV.RPC("BlueRound", RpcTarget.All);
                                }
                                else if (blueScore < redScore)
                                {
                                    PV.RPC("RedRound", RpcTarget.All);
                                }
                                else if (blueScore == redScore)
                                {
                                    if(team == "Blue")
                                    {
                                        PV.RPC("BlueRound", RpcTarget.All);
                                    }
                                    else if(team == "Red")
                                    {
                                        PV.RPC("RedRound", RpcTarget.All);
                                    }
                                }
                            }
                            else if (player.GetComponent<PlayerController>().team == "Red")
                            {
                                if (redScore > blueScore)
                                {
                                    PV.RPC("RedRound", RpcTarget.All);
                                }
                                else if (redScore < blueScore)
                                {
                                    PV.RPC("BlueRound", RpcTarget.All);
                                }
                                else if (redScore == blueScore)
                                {
                                    if(team == "Blue")
                                    {
                                        PV.RPC("BlueRound", RpcTarget.All);
                                    }
                                    else if(team == "Red")
                                    {
                                        PV.RPC("RedRound", RpcTarget.All);
                                    }
                                }
                            }

                            RoundCheck();
                        }
                    }
                }
                else if (speedGame.activeSelf == true)
                {
                    if (player.transform.position.z >= 450)
                    {
                        if (player.GetComponent<PlayerController>().team == "Blue")
                        {
                            PV.RPC("BlueRound", RpcTarget.All);
                        }
                        else if (player.GetComponent<PlayerController>().team == "Red")
                        {
                            PV.RPC("RedRound", RpcTarget.All);
                        }

                        RoundCheck();
                    }

                    if (limitTime <= 0)
                    {
                        if(team == "Blue")
                        {
                            PV.RPC("BlueRound", RpcTarget.All);
                        }
                        else if(team == "Red")
                        {
                            PV.RPC("RedRound", RpcTarget.All);
                        }

                        RoundCheck();
                    }
                }
            }
        }
    }

    [PunRPC]
    void BlueRound()
    {
        blueRound++;
    }

    [PunRPC]
    void RedRound()
    {
        redRound++;
    }

    void RoundCheck()
    {
        isFinish = true;

        if(team == "Blue")
        {
            PV.RPC("BlueReady", RpcTarget.All);
        }
        else if(team == "Red")
        {
            PV.RPC("RedReady", RpcTarget.All);
        }

        if(blueRound < 3 && redRound < 3)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(NextRound());
            }
        }
        else
        {
            StartCoroutine(RoundFinish());
        }
    }

    IEnumerator NextRound()
    {
        bool check = false;

        if (lobbyManager.isVibration == 1)
        {
            check = true;
        }

        while (check == false)
        {
            yield return new WaitForSeconds(0.1f);
            if (blueReady == true && redReady == true)
            {
                check = true;
            }
        }

        PV.RPC("NextRoundRpc", RpcTarget.All);
    }

    [PunRPC]
    void NextRoundRpc()
    {
        var child = transform.GetComponents<Transform>();

        if (random == 1)
        {
            child = runningGame.transform.GetChild(0).GetComponentsInChildren<Transform>();
            runningGame.SetActive(false);
            runningGame.GetComponent<RunningGame>().isChariotSpawnerOn = false;
            runningGame.GetComponent<RunningGame>().isFirstTrackCreated = false;
        }
        else if (random == 2)
        {
            child = cannonGame.transform.GetChild(1).GetComponentsInChildren<Transform>();
            cannonGame.SetActive(false);
            cannonGame.GetComponent<CannonGame>().isDiamond = false;
        }
        else if (random == 3)
        {
            speedGame.SetActive(false);
        }

        foreach (var item in child)
        {
            if (random == 1 && item.name != "Maps")
            {
                Destroy(item.gameObject);
            }
            else if (random == 2 && item.name != "Cannons")
            {
                Destroy(item.gameObject);
            }
        }

        foreach(GameObject player in players)
        {
            Destroy(player);
        }
        isStart = false;
        isFinish = false;
        blueReady = false;
        redReady = false;
        blueScore = 0;
        redScore = 0;
        isRandom = false;
        joystick.GetComponent<JoyStick>().Reset();
    }

    IEnumerator RoundFinish()
    {
        bool check = false;

        if (lobbyManager.isVibration == 1)
        {
            check = true;
        }

        while (check == false)
        {
            yield return new WaitForSeconds(0.1f);
            if (blueReady == true && redReady == true)
            {
                check = true;
            }
        }

        if(blueRound == 3)
        {
            if(team == "Blue")
            {
                isWin = 1;
            }
            else if(team == "Red")
            {
                isWin = 0;
            }
        }
        else if(redRound == 3)
        {                
            if(team == "Blue")
            {
                isWin = 0;
            }
            else if(team == "Red")                
            {
               isWin = 1;
            }
        }
        lobbyManager.LobbyResult();
        Reset();
    }

    public void Reset()
    {
        var child = transform.GetComponents<Transform>();

        if (random == 1)
        {
            child = runningGame.transform.GetChild(0).GetComponentsInChildren<Transform>();
            runningGame.SetActive(false);
            runningGame.GetComponent<RunningGame>().isChariotSpawnerOn = false;
            runningGame.GetComponent<RunningGame>().isFirstTrackCreated = false;
        }
        else if (random == 2)
        {
            child = cannonGame.transform.GetChild(1).GetComponentsInChildren<Transform>();
            cannonGame.SetActive(false);
            cannonGame.GetComponent<CannonGame>().isDiamond = false;
        }
        else if (random == 3)
        {
            speedGame.SetActive(false);
        }

        foreach (var item in child)
        {
            if (random == 1 && item.name != "Maps")
            {
                Destroy(item.gameObject);
            }
            else if (random == 2 && item.name != "Cannons")
            {
                Destroy(item.gameObject);
            }
        }

        foreach (GameObject player in players)
        {
            Destroy(player);
        }

        pause.SetActive(false);
        set.SetActive(false);
        cameraObject.transform.GetComponent<CameraController>().enabled = false;
        isStart = false;
        isFinish = false;
        isGiveUp = false;
        blueReady = false;
        redReady = false;
        blueScore = 0;
        redScore = 0;
        random = 0;
        team = null;
        blueRound = 0;
        redRound = 0;
        isRandom = false;
        randomList = new List<int>{1, 2, 3, 1, 2};
        randomNum = 5;
        gameObject.SetActive(false);
        joystick.GetComponent<JoyStick>().Reset();
    }

    public void Pause()
    {
        pause.SetActive(true);
    }

    public void Set()
    {
        set_Fx.value = lobbyManager.fxValue;
        set_Music.value = lobbyManager.musicValue;
        if (lobbyManager.isVibration == 0)
        {
            set_Vibration_Yes.SetActive(false);
            set_Vibration_No.SetActive(true);
        }
        else if (lobbyManager.isVibration == 1)
        {
            set_Vibration_Yes.SetActive(true);
            set_Vibration_No.SetActive(false);
        }
        set.SetActive(true);
    }

    public void Set_Fx()
    {
        audioMixer.SetFloat("FX", Mathf.Log10(set_Fx.value) * 20);
        PlayerPrefs.SetFloat("fxValue", set_Fx.value);
        lobbyManager.fxValue = PlayerPrefs.GetFloat("fxValue");
    }

    public void Set_Music()
    {
        audioMixer.SetFloat("Music", Mathf.Log10(set_Music.value) * 20);
        PlayerPrefs.SetFloat("musicValue", set_Music.value);
        lobbyManager.musicValue = PlayerPrefs.GetFloat("musicValue");
    }

    public void Set_Vibration()
    {
        if (lobbyManager.isVibration == 0)
        {
            set_Vibration_Yes.SetActive(true);
            set_Vibration_No.SetActive(false);
            lobbyManager.isVibration = 1;
            PlayerPrefs.SetInt("isVibration", lobbyManager.isVibration);
            lobbyManager.isVibration = PlayerPrefs.GetInt("isVibration");
        }
        else if (lobbyManager.isVibration == 1)
        {
            set_Vibration_Yes.SetActive(false);
            set_Vibration_No.SetActive(true);
            lobbyManager.isVibration = 0;
            PlayerPrefs.SetInt("isVibration", lobbyManager.isVibration);
            lobbyManager.isVibration = PlayerPrefs.GetInt("isVibration");
        }
    }

    public void GiveUp()
    {
        isGiveUp = true;
        PV.RPC("GiveUp_Rpc", RpcTarget.All);
    }

    [PunRPC]
    void GiveUp_Rpc()
    {
        foreach (GameObject player in players)
        {
            if (isGiveUp == true)
            {
                isWin = 0;
            }
            else if (isGiveUp == false)
            {
                isWin = 1;
            }

            lobbyManager.LobbyResult();
            Reset();
        }
    }

    public void Back()
    {
        if (lastCanvas == "Pause")
        {
            pause.SetActive(false);
        }
        else if (lastCanvas == "Set")
        {
            set.SetActive(false);
        }
    }

    void LastCanvas()
    {
        if (pause.activeSelf == true)
        {
            if (set.activeSelf == true)
            {
                lastCanvas = "Set";
            }
            else
            {
                lastCanvas = "Pause";
            }
        }
    }
}
