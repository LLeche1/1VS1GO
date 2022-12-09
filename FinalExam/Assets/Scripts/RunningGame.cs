using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RunningGame : MonoBehaviourPunCallbacks
{
    GameManager gameManager;
    GameObject maps;
    GameObject frontLineBoard;
    Vector3 preBoardPos;
    public List<GameObject> TrackList;
    public bool isFirstTrackCreated = false;
    public bool isChariotSpawnerOn = false;
    public bool isRemoverOn = false;

    public GameObject chariot;
    [HideInInspector]
    public GameObject chariotInstance = null;
    public float chariotSpeed = 1f;
    const int chariotGenTime = 15;

    const int rTrackPatternCount = 11;
    const int trackLength = 20;
    private int[] randNumArray;
    public GameObject firstTrack;
    public GameObject runningTrack;

    float distance;

    PhotonView PV;

    void Awake()
    {
        TrackList = new List<GameObject>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        maps = transform.Find("Maps").gameObject;
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            FirstTrackSet();
            TrackGenerator();
            if (gameManager.isStart == true)
            {
                ChariotSpawner();
                ChariotAcceleration();
            }
        }
        RemovePastTrack();
    }

    void FirstTrackSet()
    {
        if (isFirstTrackCreated == false)
        {
            isFirstTrackCreated = true;
            PV.RPC(nameof(FirstTrackCreateSync), RpcTarget.All);
        }
    }

    [PunRPC]
    void FirstTrackCreateSync()
    {
        GameObject fT = Instantiate(firstTrack);
        TrackList.Add(fT);
        fT.transform.position = Vector3.zero;
        fT.transform.parent = maps.transform;
        preBoardPos = Vector3.zero;
    }

    void TrackGenerator()
    {
        foreach (var player in gameManager.players)
        {
            if (TrackList[TrackList.Count - 1].transform.position.z - player.transform.position.z < trackLength * 6)
            {
                int randPatternNum = Random.Range(0, rTrackPatternCount);
                preBoardPos += new Vector3(0f, 0f, trackLength);
                PV.RPC(nameof(RunningTrackCreateSync), RpcTarget.All, preBoardPos, randPatternNum);
            }
        }
    }

    [PunRPC]
    void RunningTrackCreateSync(Vector3 initPos, int randNum)
    {
        GameObject obj = Instantiate(runningTrack, maps.transform);
        TrackList.Add(obj);
        obj.transform.position = initPos;
        obj.transform.Find("ObstaclePattern").GetChild(randNum).gameObject.SetActive(true);
    }

    void ChariotSpawner()
    {
        if(isChariotSpawnerOn == false)
        {
            isChariotSpawnerOn = true;
            StartCoroutine(ChariotSpawn());
        }
    }

    IEnumerator ChariotSpawn()
    {
        float timer = 0;
        yield return new WaitForSeconds(chariotGenTime);
        ChariotInstantiate();
        PV.RPC(nameof(IsRemoverSync), RpcTarget.All);
    }

    void ChariotInstantiate()
    {
        chariotInstance = PhotonNetwork.Instantiate("Chariot", new Vector3(-4f, 0f, 0f), Quaternion.identity);
        PV.RPC(nameof(ChariotParentSync), RpcTarget.All, chariotInstance.GetComponent<PhotonView>().ViewID);
        chariotInstance.transform.localScale = new Vector3(5.5f, 2.5f, 3f);
    }

    [PunRPC]
    void ChariotParentSync(int ID)
    {
        PhotonNetwork.GetPhotonView(ID).gameObject.transform.parent = transform.Find("Maps");
    }

    [PunRPC]
    void IsRemoverSync()
    {
        isRemoverOn = true;
    }

    void ChariotAcceleration()
    {
        if(chariotInstance != null)
        {
            chariotInstance.GetComponent<Rigidbody>().AddForce(Vector3.forward * chariotSpeed, ForceMode.Acceleration);
        }
    }

    void RemovePastTrack()
    {
        if (isRemoverOn == true)
        {
            chariotInstance = GameObject.FindGameObjectWithTag("Chariot").gameObject;
            if (chariotInstance != null)
            {
                foreach (var track in TrackList)
                {
                    if (chariotInstance.transform.position.z > track.transform.position.z + trackLength * 8)
                    {
                        TrackList.Remove(track);
                        Destroy(track.gameObject);
                        break;
                    }
                }
            }
        }
    }
    public float DistancePlayerAndChariot()
    {
        foreach (var myplayer in gameManager.players)
        {
            if(myplayer.GetComponent<PhotonView>().IsMine == true)
            {
                distance = myplayer.transform.position.z - chariotInstance.transform.position.z - 10;
            }
        }
        return distance;
    }
}