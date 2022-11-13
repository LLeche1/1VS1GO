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
    public GameObject cannonBall;
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
        while (time < 100)
        {
            yield return new WaitForEndOfFrame();
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
        PV.RPC("RandPosSync", RpcTarget.All, randSide, randCannon, CannonAttackType.Random);
        yield return new WaitForSeconds(1f);
        PV.RPC("CannonBallCreateSync", RpcTarget.All, CannonAttackType.Random);
        randGenTrigger = true;
    }

    IEnumerator LineCannonBall()
    {
        int randSide = Random.Range(0, 4);
        int randCannon = Random.Range(0, 2);
        PV.RPC("RandPosSync", RpcTarget.All, randSide, randCannon, CannonAttackType.Line);
        yield return new WaitForSeconds(1.5f);
        PV.RPC("CannonBallCreateSync", RpcTarget.All,CannonAttackType.Line);
        lineGenTrigger = true;
    }

    [PunRPC]
    void RandPosSync(int sideNum, int cannonNum, CannonAttackType type)
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
    void CannonBallCreateSync(CannonAttackType attackType)
    {
        switch (attackType)
        {
            case CannonAttackType.Random:
            {
                Transform genPos = null;
                GameObject cannon = Instantiate(cannonBall, transform.Find("Cannons").transform);

                switch (randAtkSide)
                {
                    case 0:
                        genPos = leftSideCannons.transform.GetChild(randAtkCannon).transform.Find("Cannon").transform;
                        cannon.GetComponent<Rigidbody>().AddForce(new Vector3(shootForce, 0f, 0f), ForceMode.Impulse);
                        break;
                    case 1:
                        genPos = rightSideCannons.transform.GetChild(randAtkCannon).transform.Find("Cannon").transform;
                        cannon.GetComponent<Rigidbody>().AddForce(new Vector3(-shootForce, 0f, 0f), ForceMode.Impulse);
                        break;
                    case 2:
                        genPos = topSideCannons.transform.GetChild(randAtkCannon).transform.Find("Cannon").transform;
                        cannon.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, -shootForce), ForceMode.Impulse);
                        break;
                    case 3:
                        genPos = bottomSideCannons.transform.GetChild(randAtkCannon).transform.Find("Cannon").transform;
                        cannon.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, shootForce), ForceMode.Impulse);
                        break;
                }
                cannon.transform.position = genPos.position;
            }

            break;

            case CannonAttackType.Line:
            {
                Transform genPos = null;
                int creatNum = 0;

                if (lineAtkCannon == 0) 
                { 
                    creatNum = 8; 
                }
                else if (lineAtkCannon == 1) 
                { 
                    creatNum = 7; 
                }

                GameObject[] cannonBalls = new GameObject[creatNum];

                for (int k = 0; k < creatNum; k++)
                {
                    cannonBalls[k] = Instantiate(cannonBall, transform.Find("Cannons").transform);
                }

                switch (lineAtkSide)
                {
                    case 0:
                        for (int l = 0; l < creatNum; l++)
                        {
                            genPos = leftSideCannons.transform.GetChild((2 * l) + lineAtkCannon).transform.Find("Cannon").transform;
                            cannonBalls[l].transform.position = genPos.position;
                            cannonBalls[l].GetComponent<Rigidbody>().AddForce(new Vector3(shootForce, 0f, 0f), ForceMode.Impulse);
                        }

                        break;

                    case 1:
                        for (int l = 0; l < creatNum; l++)
                        {
                            genPos = rightSideCannons.transform.GetChild((2 * l) + lineAtkCannon).transform.Find("Cannon").transform;
                            cannonBalls[l].transform.position = genPos.position;
                            cannonBalls[l].GetComponent<Rigidbody>().AddForce(new Vector3(-shootForce, 0f, 0f), ForceMode.Impulse);
                        }
                        break;

                    case 2:
                        for (int l = 0; l < creatNum; l++)
                        {
                            genPos = topSideCannons.transform.GetChild((2 * l) + lineAtkCannon).transform.Find("Cannon").transform;
                            cannonBalls[l].transform.position = genPos.position;
                            cannonBalls[l].GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, -shootForce), ForceMode.Impulse);
                        }

                        break;

                    case 3:
                        for (int l = 0; l < creatNum; l++)
                        {
                            genPos = bottomSideCannons.transform.GetChild((2 * l) + lineAtkCannon).transform.Find("Cannon").transform;
                            cannonBalls[l].transform.position = genPos.position;
                            cannonBalls[l].GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, shootForce), ForceMode.Impulse);
                        }

                        break;
                    }
                }
                break;
        }
    }
}
