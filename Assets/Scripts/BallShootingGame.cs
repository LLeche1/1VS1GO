using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BallShootingGame : MonoBehaviourPunCallbacks
{
    private GameManager gameManager;
    private PhotonView PV;
    [SerializeField] private GameObject land;
    [SerializeField] private GameObject scoreBoard;
    public List<GameObject> detectionPlates;
    public GameObject[] balls;
    public GameObject[] spawners;
    public bool initScoreBoardTrigger = true;
    public bool ballGenTrigger = true;
    private Vector3 spawnPos = Vector3.zero;
    private Vector3 forceDir;
    private int ballNum = 0;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        detectionPlates = new List<GameObject>();
    }
    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient && gameManager.isStart == true)
        {
            initScoreBoard();
            BallRandomSpawner();
        }
    }

    void initScoreBoard()
    {
        if(initScoreBoardTrigger == true)
        {
            initScoreBoardTrigger = false;
            PV.RPC(nameof(goalPostInit),RpcTarget.All);

            if (true)
            {
                foreach (var plate in detectionPlates)
                {
                    int r = Random.Range(0, 3);
                    int id = plate.GetComponent<PhotonView>().ViewID;
                    PV.RPC(nameof(PlateRandomActive), RpcTarget.All, id, r);
                }
            }
        }
    }
    [PunRPC]
    void goalPostInit()
    {
        for (int i = 0; i < 9; i++)
            {
                
                detectionPlates.Add(transform.Find("Maps").Find("BallShottingGameScoreBoard").Find("BallDetectionPlates").GetChild(i).GetChild(0).gameObject);
                transform.Find("Maps").Find("BallShottingGameScoreBoard").Find("BallDetectionPlates").GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(false);
                transform.Find("Maps").Find("BallShottingGameScoreBoard").Find("BallDetectionPlates").GetChild(i).GetChild(0).GetChild(1).gameObject.SetActive(false);
                transform.Find("Maps").Find("BallShottingGameScoreBoard").Find("BallDetectionPlates").GetChild(i).GetChild(0).GetChild(2).gameObject.SetActive(false);
                transform.Find("Maps").Find("BallShottingGameScoreBoard").Find("BallDetectionPlates").GetChild(i).GetChild(0).GetChild(3).gameObject.SetActive(false);
            }
        
    }
    [PunRPC]
    void PlateRandomActive(int ID, int randN)
    {
        GameObject plate = PhotonNetwork.GetPhotonView(ID).gameObject;
        plate.transform.GetChild(randN).gameObject.SetActive(true);
        plate.GetComponent<BallDetection>().PlateTypeSelect(randN);
    }
    void BallRandomSpawner()
    {
        if (ballGenTrigger == true)
        {
            ballGenTrigger = false;
            StartCoroutine(BallRandomSpawn());
        }
    }

    IEnumerator BallRandomSpawn()
    {
        int randBall = Random.Range(0, 3);
        int randSpawnPoint = Random.Range(0, 7);
        SpawnBallValueSetting(randSpawnPoint, randBall);
        yield return new WaitForSeconds(2f);
        SpawnBall();
        ballGenTrigger = true;
    }
    void SpawnBallValueSetting(int randSP, int randB)
    {
        spawnPos = spawners[randSP].transform.GetChild(1).position;
        ballNum = randB;
        forceDir = spawners[randSP].transform.GetChild(1).forward;

    }
    void SpawnBall()
    {
        string ballName = "";
        if (ballNum == 0) ballName = "BasketBall"; if (ballNum == 1) ballName = "BeachBall"; if (ballNum == 2) ballName = "SoccerBall";
        GameObject ball = PhotonNetwork.Instantiate(ballName, spawnPos, Quaternion.identity);
        PV.RPC(nameof(SpawnBallRpc), RpcTarget.All, ball.GetComponent<PhotonView>().ViewID, ballName);
    }
    [PunRPC]
    void SpawnBallRpc(int ID, string ballname)
    {
        GameObject ball = PhotonNetwork.GetPhotonView(ID).gameObject;
        ball.name = ballname;
        ball.transform.parent = transform.Find("Maps").Find("Balls");
        ball.transform.position = spawnPos;
        ball.GetComponent<Rigidbody>().AddForce(forceDir * 0.6f, ForceMode.Impulse);
    }
}
