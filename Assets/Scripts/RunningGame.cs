using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RunningGame : MonoBehaviourPunCallbacks
{
    GameManager gameManager;
    GameObject maps;
    GameObject frontLineBoard;
    public Vector3 preBoardPos = Vector3.zero;
    public List<GameObject> TrackList;
    private GameObject fowardPlayer;
    public Vector3 fowardPos = Vector3.zero;
    public bool isFirstTrackCreated = false;
    public bool isChariotSpawnerOn = false;
    public bool isRemoverOn = false;

    public GameObject chariot;
    //[HideInInspector]
    public GameObject chariotInstance = null;
    public float chariotSpeed = 0.8f;
    const int chariotGenTime = 30;

    const int rTrackPatternCount = 11;
    const int trackLength = 20;
    public GameObject firstTrack;
    public GameObject runningTrack;

    float distance;
    public TMP_Text distanceText;
    public GameObject warningMessageBox;
    public GameObject warningOnPos;
    public GameObject warningOffPos;
    public bool warningUITrigger = false;

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
            
            if (gameManager.isStart == true)
            {
                TrackGenerator();
                ChariotSpawner();
                ChariotAcceleration();
            }
        }
        
        Warning();
        RemovePastTrack();
    }

    void FirstTrackSet()
    {
        if (isFirstTrackCreated == false)
        {
            isFirstTrackCreated = true;
            PV.RPC(nameof(FirstTrackCreateSync), RpcTarget.All);
            for (int i = 0; i < 7; i++)
            {
                int randPatternNum = Random.Range(0, rTrackPatternCount);
                preBoardPos += new Vector3(0f, 0f, trackLength);
                PV.RPC(nameof(RunningTrackCreateSync), RpcTarget.All, preBoardPos, randPatternNum);
            }
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
            if (fowardPos.z < player.transform.position.z)
                fowardPos.z = player.transform.position.z;
        }
        if (TrackList[TrackList.Count - 1].transform.position.z - fowardPos.z < trackLength * 6)
        {
            int randPatternNum = Random.Range(0, rTrackPatternCount);
            preBoardPos += new Vector3(0f, 0f, trackLength);
            PV.RPC(nameof(RunningTrackCreateSync), RpcTarget.All, preBoardPos, randPatternNum);
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
        if (isChariotSpawnerOn == false)
        {
            isChariotSpawnerOn = true;
            StartCoroutine(ChariotSpawn());
        }
    }

    IEnumerator ChariotSpawn()
    {
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
        if (chariotInstance != null)
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
        foreach (var player in gameManager.players)
        {
            if (player.GetComponent<PhotonView>().IsMine == true)
            {
                distance = player.transform.position.z - chariotInstance.transform.position.z - 10;
                if (distance <= 0)
                {
                    distance = 0;
                }
            }
        }

        return distance;
    }

    void Warning()
    {
        if (chariotInstance != null)
        {
            distanceText.text = "Distance: " + DistancePlayerAndChariot().ToString("0");
            if ((DistancePlayerAndChariot() < 150 && DistancePlayerAndChariot() > 0) && (warningUITrigger == false))
            {
                warningUITrigger = true;
            }

            if (warningUITrigger == true && DistancePlayerAndChariot() <= 0)
            {
                warningMessageBox.GetComponent<RectTransform>().transform.position = Vector3.Lerp(warningMessageBox.GetComponent<RectTransform>().transform.position
                    , warningOffPos.GetComponent<RectTransform>().position, 2 * Time.deltaTime);
            }
            else if (warningUITrigger == true)
            {
                warningMessageBox.GetComponent<RectTransform>().transform.position = Vector3.Lerp(warningMessageBox.GetComponent<RectTransform>().transform.position
                    , warningOnPos.GetComponent<RectTransform>().position, 2 * Time.deltaTime);
            }
        }
    }
}