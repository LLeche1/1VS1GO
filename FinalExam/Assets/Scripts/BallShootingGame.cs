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
    private List<GameObject> detectionPlates = new List<GameObject>();
    private Vector3 leftSpawnPos = new Vector3(-7.5f, -1f, 0f);
    private Vector3 rightSpawnPos = new Vector3(20f, -1f, 0f);
    public GameObject[] balls;
    public GameObject[] spawners;
    public bool ballGenTrigger = true;
    private Vector3 spawnPos = Vector3.zero;
    private Vector3 forceDir;
    private int ballNum = 0;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Start()
    {
        //BoardGenerate();
        if (PhotonNetwork.IsMasterClient)
        {
            initScoreBoard();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient && gameManager.isStart == true)
        {
            BallRandomSpawner();
        }
    }

    void BoardGenerate()
    {
        Instantiate(land, transform);
        Instantiate(scoreBoard, transform);
    }

    void initScoreBoard()
    {
        for (int i = 0; i < 9; i++)
        {
            detectionPlates.Add(transform.Find("Maps").Find("BallShottingGameScoreBoard").Find("BallDetectionPlates").GetChild(i).GetChild(0).gameObject);
        }

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
        /*int randSide = Random.Range(0, 2);
        int randZPos = Random.Range(2, 9);*/
        int randBall = Random.Range(0, 3);
        int randSpawnPoint = Random.Range(0, 7);
        //Vector3 fDir = new Vector3(((randSide == 0) ? 1 : -1) * Random.Range(50, 100) / 100f, Random.Range(50, 100) / 100f, 0f).normalized;
        SpawnBallValueSetting(randSpawnPoint, randBall);
        yield return new WaitForSeconds(2f);
        SpawnBall();
        ballGenTrigger = true;
    }
    void SpawnBallValueSetting(int randSP, int randB)
    {
        /*switch (randS)
        {
            case 0:
                spawnPos = leftSpawnPos;
                break;
            case 1:
                spawnPos = rightSpawnPos;
                break;
        }*/
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
