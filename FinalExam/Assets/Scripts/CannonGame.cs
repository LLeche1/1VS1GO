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
    private const float shootForce = 100f;
    public GameObject cannonObj;
    public GameObject cannonBallObj;
    private GameObject maps;
    private GameObject leftSideCannons;
    private GameObject rightSideCannons;
    private GameObject topSideCannons;
    private GameObject bottomSideCannons;
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        maps = GameObject.Find("Maps");
        leftSideCannons = maps.transform.Find("CannonLeftSide").gameObject;
        rightSideCannons = maps.transform.Find("CannonRightSide").gameObject;
        topSideCannons = maps.transform.Find("CannonTopSide").gameObject;
        bottomSideCannons = maps.transform.Find("CannonBottomSide").gameObject;
    }

    void Start()
    {
        CannonGenerate();
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CannonBallSpawner();
        }
    }

    void CannonGenerate()
    {
        for (int i = 0; i < (rowSize / 2) - 1; i++)
        {
            GameObject leftSideCannon = Instantiate(cannonObj);
            leftSideCannon.transform.SetParent(leftSideCannons.transform);
            leftSideCannon.transform.position = new Vector3(leftSideCannons.transform.position.x, leftSideCannons.transform.position.y, leftSideCannons.transform.position.z + (2 * i));
            leftSideCannon.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

            GameObject rightSideCannon = Instantiate(cannonObj);
            rightSideCannon.transform.position = new Vector3(rightSideCannons.transform.position.x, rightSideCannons.transform.position.y, rightSideCannons.transform.position.z + (2 * i));
            rightSideCannon.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
            rightSideCannon.transform.SetParent(rightSideCannons.transform);

            GameObject topSideCannon = Instantiate(cannonObj);
            topSideCannon.transform.position = new Vector3(topSideCannons.transform.position.x + (2 * i), topSideCannons.transform.position.y, topSideCannons.transform.position.z);
            topSideCannon.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            topSideCannon.transform.SetParent(topSideCannons.transform);

            GameObject bottomSideCannon = Instantiate(cannonObj);
            bottomSideCannon.transform.position = new Vector3(bottomSideCannons.transform.position.x + (2 * i), bottomSideCannons.transform.position.y, bottomSideCannons.transform.position.z);
            bottomSideCannon.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            bottomSideCannon.transform.SetParent(bottomSideCannons.transform);
        }
    }

    void CannonBallSpawner()
    {
        if (randGenTrigger)
        {
            randGenTrigger = false;
            StartCoroutine(RandomCannonAttack());
        }
        if (lineGenTrigger)
        {
            lineGenTrigger = false;
            StartCoroutine(LineCannonAttack());
        }
    }

    IEnumerator RandomCannonAttack()
    {
        int randSide = Random.Range(0, 4);
        int randCannon = Random.Range(0, 15);
        PV.RPC(nameof(RandPosSync), RpcTarget.All, randSide, randCannon, CannonAttackType.Random); //randI?? randJ?? ?????? ???? ???????? ?? ???? RPC?? ???? ?????????? ?????? ????????.
        yield return new WaitForSeconds(1f);
        Debug.Log(randAtkCannon);
        PV.RPC(nameof(CannonBallCreateSysnc), RpcTarget.All, CannonAttackType.Random);
        randGenTrigger = true;
    }

    IEnumerator LineCannonAttack()
    {
        int randSide = Random.Range(0, 4);
        int randCannon = Random.Range(0, 2);
        PV.RPC(nameof(RandPosSync), RpcTarget.All, randSide, randCannon, CannonAttackType.Line); //randI?? randJ?? ?????? ???? ???????? ?? ???? RPC?? ???? ?????????? ?????? ????????.
        yield return new WaitForSeconds(1.5f);
        PV.RPC(nameof(CannonBallCreateSysnc), RpcTarget.All,CannonAttackType.Line);
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
    void CannonBallCreateSysnc(CannonAttackType attackType)
    {
        switch (attackType)
        {
            case CannonAttackType.Random:
                {
                    Transform genPos = null;
                    GameObject cannon = Instantiate(cannonBallObj, transform.Find("Cannons").transform);
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

                    if (lineAtkCannon == 0) { creatNum = 8; }
                    else if (lineAtkCannon == 1) { creatNum = 7; }

                    GameObject[] cannonBalls = new GameObject[creatNum];
                    for (int k = 0; k < creatNum; k++)
                    {
                        cannonBalls[k] = Instantiate(cannonBallObj, transform.Find("Cannons").transform);
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
