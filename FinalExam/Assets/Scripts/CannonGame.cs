using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CannonGame : MonoBehaviourPunCallbacks
{
    const int rowSize = 16;
    const int colSize = 16;
    private int i = 0;
    private int j = 0;
    public bool genAble = true;
    private const float shootForce = 1500f;
    public GameObject boardBlockObj;
    public GameObject cannonObj;
    public GameObject cannonBallObj;
    private GameObject map;
    private GameObject leftSideCannons, rightSideCannons, topSideCannons, bottomSideCannons;
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
        PV.RPC("ObstacleSpawner", RpcTarget.All);   
    }

    void BoardGenerate()
    {
        for (int i = 0; i < rowSize; i++)
        {
            for (int j = 0; j < colSize; j++)
            {
                GameObject boardblock = Instantiate(boardBlockObj);
                boardblock.transform.SetParent(map.transform);
                boardblock.transform.position = new Vector3(j, 0, i);
            }
        }
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
    void ObstacleSpawner()
    {
        if (genAble)
        {
            genAble = false;
            StartCoroutine(RandPosGenObtacle());
        }
    }

    IEnumerator RandPosGenObtacle()
    {
        int ranI = 0; int ranJ = 0;
        if (PhotonNetwork.IsMasterClient)
        {
            ranI = Random.Range(0, 4);
            ranJ = Random.Range(0, 8);
            PV.RPC("RandPosSynch", RpcTarget.All, ranI, ranJ);
        }
        yield return new WaitForSeconds(0.5f);
        Transform genPos = null;
        
        GameObject cannon = Instantiate(cannonBallObj, transform.Find("Cannons").transform);
        switch (i)
        {
            case 0:
                genPos = leftSideCannons.transform.GetChild(j).transform.Find("Cannon").transform;
                cannon.GetComponent<Rigidbody>().AddForce(new Vector3(shootForce, 0f, 0f), ForceMode.Impulse);
                break;
            case 1:
                genPos = rightSideCannons.transform.GetChild(j).transform.Find("Cannon").transform;
                cannon.GetComponent<Rigidbody>().AddForce(new Vector3(-shootForce, 0f, 0f), ForceMode.Impulse);
                break;
            case 2:
                genPos = topSideCannons.transform.GetChild(j).transform.Find("Cannon").transform;
                cannon.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, -shootForce), ForceMode.Impulse);
                break;
            case 3:
                genPos = bottomSideCannons.transform.GetChild(j).transform.Find("Cannon").transform;
                cannon.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, shootForce), ForceMode.Impulse);
                break;
        }
        cannon.transform.position = genPos.position;
        genAble = true;
    }

    [PunRPC]
    void RandPosSynch(int a, int b)
    {
        i = a;
        j = b;
    }
}
