using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BallShootingGame : MonoBehaviourPunCallbacks
{
    private PhotonView PV;
    [SerializeField] private GameObject land;
    [SerializeField] private GameObject scoreBoard;
    private List<GameObject> detectionPlates = new List<GameObject>();
    private Vector3 leftSpawnPos = new Vector3(-7.5f, -1f, 0f);
    private Vector3 rightSpawnPos = new Vector3(20f, -1f, 0f);
    public GameObject[] balls;
    private Vector3 spawnPos = Vector3.zero;
    private Vector3 forceDir;
    private int ballNum = 0;
    
    private bool ballGenTrigger = true;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        BoardGenerate();
        initScoreBoard();
    }

    // Update is called once per frame
    void Update()
    {
        BallRandomSpawner();
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
            detectionPlates.Add(transform.Find("BallShottingGameScoreBoard(Clone)").Find("BallDetectionPlates").GetChild(i).GetChild(0).gameObject);
        }
        /*if (PhotonNetwork.IsMasterClient)
        {
            foreach (var plate in detectionPlates)
            {
                int i = Random.Range(0, 3);
                PV.RPC("PlateRandomActive", RpcTarget.All, plate, i);
            }
        }*/
        if (true)
        {
            foreach (var plate in detectionPlates)
            {
                int i = Random.Range(0, 3);
                PlateRandomActive(plate, i);
            }
        }

    }

    [PunRPC]
    void PlateRandomActive(GameObject plate, int r)
    {
        plate.transform.GetChild(r).gameObject.SetActive(true);
    }
    void BallRandomSpawner()
    {
        if (ballGenTrigger)
        {
            ballGenTrigger = false;
            StartCoroutine(BallRanddomSpawn());
        }
    }

    IEnumerator BallRanddomSpawn()
    {
        /*if (PhotonNetwork.IsMasterClient)
        {
            
        }*/
        int randSide = Random.Range(0, 2);
        int randNum = Random.Range(2, 9);
        int randBall = Random.Range(0, 3);
        Vector3 fDir = new Vector3(((randSide == 0) ? 1 : -1) * Random.Range(50, 100) / 100f, Random.Range(50, 100) / 100f, 0f).normalized;
        //PV.RPC("RandomPosSync", RpcTarget.All, randSide, randNum, randBall, fDir);
        RandomPosSync(randSide, randNum, randBall, fDir);
        yield return new WaitForSeconds(1f);
        spawn(spawnPos, forceDir);
        ballGenTrigger = true;
    }
    void RandomPosSync(int randS, int randN, int randB, Vector3 fDir)
    {
        switch (randS)
        {
            case 0:
                spawnPos = leftSpawnPos;
                break;
            case 1:
                spawnPos = rightSpawnPos;
                break;
        }
        spawnPos = new Vector3(spawnPos.x, spawnPos.y, randN);
        ballNum = randB;
        forceDir = fDir;

    }
    void GoalSpotRandomSpawn()
    {

    }
    void spawn(Vector3 sPos, Vector3 fDir)
    {
        GameObject ball = Instantiate(balls[ballNum]);
        ball.transform.position = spawnPos;
        ball.GetComponent<Rigidbody>().AddForce(fDir * 0.6f, ForceMode.Impulse);
    }
}
