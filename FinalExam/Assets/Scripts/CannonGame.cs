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
    const int rowSize = 16;
    const int colSize = 16;
    private int randAtkSide = 0;
    private int randAtkCannon = 0;
    private int lineAtkSide = 0;
    private int lineAtkCannon = 0;
    public bool randGenTrigger = true;
    public bool lineGenTrigger = true;
    private const float shootForce = 75f;
    public GameObject landBlockObj;
    public GameObject cannonObj;
    public GameObject cannonBallObj;
    private GameObject map;
    private GameObject leftSideCannons;
    private GameObject rightSideCannons;
    private GameObject topSideCannons;
    private GameObject bottomSideCannons;
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        map = GameObject.Find("Maps");
        leftSideCannons = map.transform.Find("CannonLeftSide").gameObject;
        rightSideCannons = map.transform.Find("CannonRightSide").gameObject;
        topSideCannons = map.transform.Find("CannonTopSide").gameObject;
        bottomSideCannons = map.transform.Find("CannonBottomSide").gameObject;
    }

    void Start()
    {
        BoardGenerate();
        CannonGenerate();
    }

    void Update()
    {
        PV.RPC("CannonBallSpawner", RpcTarget.All);
    }

    void BoardGenerate()
    {
        GameObject land = Instantiate(landBlockObj, map.transform);
    }

    void CannonGenerate()
    {
        for (int i = 0; i < rowSize / 2; i++)
        {
            GameObject leftSideCannon = Instantiate(cannonObj);
            leftSideCannon.transform.position = new Vector3(-1f, 0f, 2 * i);
            leftSideCannon.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            leftSideCannon.transform.SetParent(leftSideCannons.transform);


            GameObject rightSideCannon = Instantiate(cannonObj);
            rightSideCannon.transform.position = new Vector3(16f, 0f, 2 * i);
            rightSideCannon.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
            rightSideCannon.transform.SetParent(rightSideCannons.transform);

            GameObject topSideCannon = Instantiate(cannonObj);
            topSideCannon.transform.position = new Vector3(2 * i, 0f, 16f);
            topSideCannon.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            topSideCannon.transform.SetParent(topSideCannons.transform);

            GameObject bottomSideCannon = Instantiate(cannonObj);
            bottomSideCannon.transform.position = new Vector3(2 * i, 0f, -1f);
            bottomSideCannon.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            bottomSideCannon.transform.SetParent(bottomSideCannons.transform);
        }

        leftSideCannons.transform.position = new Vector3(-0.5f, 0f, 0f);
        rightSideCannons.transform.position = new Vector3(0.5f, 0f, 1f);
        topSideCannons.transform.position = new Vector3(0f, 0f, 0.5f);
        bottomSideCannons.transform.position = new Vector3(1f, 0f, -0.5f);
    }

    [PunRPC]
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
        int ranI = 0; int ranJ = 0;
        if (PhotonNetwork.IsMasterClient)
        {
            ranI = Random.Range(0, 4);
            ranJ = Random.Range(0, 8);
            PV.RPC("RandPosSync", RpcTarget.All, ranI, ranJ,CannonAttackType.Random); //randI?? randJ?? ?????? ???? ???????? ?? ???? RPC?? ???? ?????????? ?????? ????????.
        }
        yield return new WaitForSeconds(0.5f);
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
        randGenTrigger = true;
    }

    IEnumerator LineCannonAttack()
    {
        int ranI = 0; int ranJ = 0;
        if (PhotonNetwork.IsMasterClient)
        {
            ranI = Random.Range(0, 4);
            ranJ = Random.Range(0, 2);
            PV.RPC("RandPosSync", RpcTarget.All, ranI, ranJ, CannonAttackType.Line); //randI?? randJ?? ?????? ???? ???????? ?? ???? RPC?? ???? ?????????? ?????? ????????.
        }
        yield return new WaitForSeconds(2.2f);
        Transform genPos = null;

        GameObject[] cannons = new GameObject[4];
        for (int k = 0; k < 4; k++)
        {
            cannons[k] = Instantiate(cannonBallObj, transform.Find("Cannons").transform);
        }
        switch (lineAtkSide)
        {
            case 0:
                for (int l = 0; l < 4; l++)
                {
                    genPos = leftSideCannons.transform.GetChild((2 * l) + lineAtkCannon).transform.Find("Cannon").transform;
                    cannons[l].transform.position = genPos.position;
                    cannons[l].GetComponent<Rigidbody>().AddForce(new Vector3(shootForce, 0f, 0f), ForceMode.Impulse);
                }
                break;
            case 1:
                for (int l = 0; l < 4; l++)
                {
                    genPos = rightSideCannons.transform.GetChild((2 * l) + lineAtkCannon).transform.Find("Cannon").transform;
                    cannons[l].transform.position = genPos.position;
                    cannons[l].GetComponent<Rigidbody>().AddForce(new Vector3(-shootForce, 0f, 0f), ForceMode.Impulse);
                }
                break;
            case 2:
                for (int l = 0; l < 4; l++)
                {
                    genPos = topSideCannons.transform.GetChild((2 * l) + lineAtkCannon).transform.Find("Cannon").transform;
                    cannons[l].transform.position = genPos.position;
                    cannons[l].GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, -shootForce), ForceMode.Impulse);
                }
                break;
            case 3:
                for (int l = 0; l < 4; l++)
                {
                    genPos = bottomSideCannons.transform.GetChild((2 * l) + lineAtkCannon).transform.Find("Cannon").transform;
                    cannons[l].transform.position = genPos.position;
                    cannons[l].GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, shootForce), ForceMode.Impulse);
                }
                break;
        }
        lineGenTrigger = true;
    }

    [PunRPC]
    void RandPosSync(int a, int b, CannonAttackType type)
    {
        switch (type)
        {
            case CannonAttackType.Random:
                randAtkSide = a;
                randAtkCannon = b;
                break;
            case CannonAttackType.Line:
                lineAtkSide = a;
                lineAtkCannon = b;
                break;
        }
    }
}
