using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RunningGame : MonoBehaviourPunCallbacks
{
    const int trackNum = 10;
    const int trackLength = 16;
    private int[] randNumArray;
    public GameObject firstTrack;
    public GameObject endTrack;
    public GameObject[] tracks;
    PhotonView PV;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        FirstEndTrackSet();
        if (PhotonNetwork.IsMasterClient)
        {
            randNumArray = RandNumGenerate();
            PV.RPC("TrackNumSync", RpcTarget.All, randNumArray);
            //PV.RPC("BoardSetting", RpcTarget.All);
        }
        BoardSetting();
    }

    void FirstEndTrackSet()
    {
        GameObject fT = Instantiate(firstTrack, this.transform);
        fT.transform.position = Vector3.zero;
        fT.transform.parent = GameObject.Find("InGame").transform.GetChild(1).transform;

        GameObject eT = Instantiate(endTrack, this.transform);
        eT.transform.position = new Vector3(0f, 0f, trackLength * (trackNum + 1));
        eT.transform.parent = GameObject.Find("InGame").transform.GetChild(1).transform;
    }

    [PunRPC]
    void BoardSetting()
    {
        for (int i = 1; i <= trackNum; i++)
        {
            int randVal = Random.Range(0, tracks.Length);
            GameObject track = Instantiate(tracks[randNumArray[i - 1]]);
            track.transform.position = new Vector3(0f, 0f, trackLength * i);
            track.transform.parent = GameObject.Find("InGame").transform.GetChild(1).transform;
        }
    }
    int[] RandNumGenerate()
    {
        int[] numArray = new int[trackNum];
        for (int i = 0; i < trackNum; i++)
        {
            numArray[i] = Random.Range(0, tracks.Length);
        }
        return numArray;
    }

    [PunRPC]
    void TrackNumSync(int[] numArray)
    {
        randNumArray = numArray;
    }
}
