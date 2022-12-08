using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

enum CannonAttackType
{
    Random,
    Line
}

public class CannonGame : MonoBehaviourPunCallbacks
{
    const int rowSize = 32;
    const int colSize = 32;
    private int randAtkSide = 0;
    private int randAtkCannon = 0;
    private int lineAtkSide = 0;
    private int lineAtkCannon = 0;
    public bool randGenTrigger = true;
    public bool lineGenTrigger = true;
    public bool isDiamond = false;
    private const float shootForce = 100f;
    private GameObject maps;
    private GameObject leftSideCannons;
    private GameObject rightSideCannons;
    private GameObject topSideCannons;
    private GameObject bottomSideCannons;
    PhotonView PV;
    GameManager gameManager;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        maps = GameObject.Find("Maps");
        leftSideCannons = maps.transform.Find("CannonLeftSide").gameObject;
        rightSideCannons = maps.transform.Find("CannonRightSide").gameObject;
        topSideCannons = maps.transform.Find("CannonTopSide").gameObject;
        bottomSideCannons = maps.transform.Find("CannonBottomSide").gameObject;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && gameManager.isStart == true)
        {
            CannonBallSpawner();
            DiamondSpawner();
        }
    }

    void DiamondSpawner()
    {
        if (isDiamond == false)
        {
            isDiamond = true;
            float x = Random.Range(-12f, 14f);
            float z = Random.Range(-12f, 14f);
            GameObject diamond = PhotonNetwork.Instantiate("Diamond", new Vector3(x, 1, z), Quaternion.Euler(-90, 0, 0));
            diamond.transform.position = new Vector3(x, 1, z);
            PV.RPC("DiamondRpc", RpcTarget.All, diamond.GetComponent<PhotonView>().ViewID);
            StartCoroutine("DiamondDelay");
        }
    }

    [PunRPC]
    void DiamondRpc(int ID)
    {
        PhotonNetwork.GetPhotonView(ID).gameObject.transform.parent = transform.Find("Cannons");
    }

    IEnumerator DiamondDelay()
    {
        float time = 0;
        while (time < 50)
        {
            yield return gameManager.waitForSeconds;
            time += 10.0f * Time.deltaTime;
        }
        isDiamond = false;
        yield return null;
    }

    void CannonBallSpawner()
    {
        if (randGenTrigger)
        {
            randGenTrigger = false;
            StartCoroutine(RandomCannonBall());
        }
        if (lineGenTrigger)
        {
            lineGenTrigger = false;
            StartCoroutine(LineCannonBall());
        }
    }

    IEnumerator RandomCannonBall()
    {
        int randSide = Random.Range(0, 4);
        int randCannon = Random.Range(0, 15);
        RandPos(randSide, randCannon, CannonAttackType.Random);
        yield return gameManager.waitForSeconds5;
        CannonBallCreate(CannonAttackType.Random);
        randGenTrigger = true;
    }

    IEnumerator LineCannonBall()
    {
        int randSide = Random.Range(0, 4);
        int randCannon = Random.Range(0, 2);
        RandPos(randSide, randCannon, CannonAttackType.Line);
        yield return gameManager.waitForSeconds6;
        CannonBallCreate(CannonAttackType.Line);
        lineGenTrigger = true;
    }

    void RandPos(int sideNum, int cannonNum, CannonAttackType type)
    {
        switch (type)
        {
            case CannonAttackType.Random:
                randAtkSide = sideNum;
                randAtkCannon = cannonNum;
                break;
                
            case CannonAttackType.Line:
                lineAtkSide = sideNum;
                lineAtkCannon = cannonNum;
                break;
        }
    }

    [PunRPC]
    void RandomRpc(int ID, int side, int randAtk)
    {
        Transform genPos = null;
        randAtkCannon = randAtk;
        GameObject ball = PhotonNetwork.GetPhotonView(ID).gameObject;
        ball.transform.parent = transform.Find("Cannons").transform;

        switch (side)
        {
            case 0:
                genPos = leftSideCannons.transform.GetChild(randAtkCannon).transform.Find("Cannon").transform;
                ball.GetComponent<Rigidbody>().AddForce(new Vector3(shootForce, 0f, 0f), ForceMode.Impulse);
                break;
            case 1:
                genPos = rightSideCannons.transform.GetChild(randAtkCannon).transform.Find("Cannon").transform;
                ball.GetComponent<Rigidbody>().AddForce(new Vector3(-shootForce, 0f, 0f), ForceMode.Impulse);
                break;
            case 2:
                genPos = topSideCannons.transform.GetChild(randAtkCannon).transform.Find("Cannon").transform;
                ball.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, -shootForce), ForceMode.Impulse);
                break;
            case 3:
                genPos = bottomSideCannons.transform.GetChild(randAtkCannon).transform.Find("Cannon").transform;
                ball.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, shootForce), ForceMode.Impulse);
                break;
        }

        ball.transform.position = genPos.position;
    }

    [PunRPC]
    void LineRpc(int ID, int side, int lineAtk, int num)
    {
        Transform genPos = null;
        lineAtkCannon = lineAtk;
        GameObject ball = PhotonNetwork.GetPhotonView(ID).gameObject;
        ball.transform.parent = transform.Find("Cannons").transform;

        switch (side)
        {
            case 0:
                genPos = leftSideCannons.transform.GetChild((2 * num) + lineAtkCannon).transform.Find("Cannon").transform;
                ball.transform.position = genPos.position;
                ball.GetComponent<Rigidbody>().AddForce(new Vector3(shootForce, 0f, 0f), ForceMode.Impulse);

                break;

            case 1:
                genPos = rightSideCannons.transform.GetChild((2 * num) + lineAtkCannon).transform.Find("Cannon").transform;
                ball.transform.position = genPos.position;
                ball.GetComponent<Rigidbody>().AddForce(new Vector3(-shootForce, 0f, 0f), ForceMode.Impulse);

                break;

            case 2:
                genPos = topSideCannons.transform.GetChild((2 * num) + lineAtkCannon).transform.Find("Cannon").transform;
                ball.transform.position = genPos.position;
                ball.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, -shootForce), ForceMode.Impulse);

                break;

            case 3:
                genPos = bottomSideCannons.transform.GetChild((2 * num) + lineAtkCannon).transform.Find("Cannon").transform;
                ball.transform.position = genPos.position;
                ball.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, shootForce), ForceMode.Impulse);

                break;
            }
    }

    void CannonBallCreate(CannonAttackType attackType)
    {
        switch (attackType)
        {
            case CannonAttackType.Random:
            {
                GameObject cannon = PhotonNetwork.Instantiate("cannonBall", Vector3.zero, Quaternion.identity);
                PV.RPC("RandomRpc", RpcTarget.All, cannon.GetComponent<PhotonView>().ViewID, randAtkSide, randAtkCannon);
            }

            break;

            case CannonAttackType.Line:
            {
                int creatNum = 0;
                int num = 0;

                if (lineAtkCannon == 0) 
                { 
                    creatNum = 8; 
                }
                else if (lineAtkCannon == 1) 
                { 
                    creatNum = 7; 
                }

                for (int k = 0; k < creatNum; k++)
                {
                    GameObject cannon = PhotonNetwork.Instantiate("cannonBall", Vector3.zero, Quaternion.identity);
                    PV.RPC("LineRpc", RpcTarget.All, cannon.GetComponent<PhotonView>().ViewID, lineAtkSide, lineAtkCannon, num);
                    num++;
                }
            }
            break;
        }
    }
}
