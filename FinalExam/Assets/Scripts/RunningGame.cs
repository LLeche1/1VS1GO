using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RunningGame : MonoBehaviourPunCallbacks
{
    const int trackNum = 10;
    const int trackLength = 16;
    private int randN = 0;
    public GameObject firstTrack;
    public GameObject endTrack;
    public GameObject[] tracks;
    PhotonView PV;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        FirstEndTrackSet();
        BoardSetting();
    }

    void FirstEndTrackSet()
    {
        GameObject fT = Instantiate(firstTrack, this.transform);
        fT.transform.position = Vector3.zero;
        fT.transform.parent = GameObject.Find("InGame").transform.GetChild(1).transform;

        GameObject eT = Instantiate(endTrack, this.transform);
        eT.transform.position = new Vector3(0f, 0f, trackLength * trackNum);
        eT.transform.parent = GameObject.Find("InGame").transform.GetChild(1).transform;
    }

    void BoardSetting()
    {
        for (int i = 1; i < trackNum; i++)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int randVal = Random.Range(0, tracks.Length);
                PV.RPC("TrackNumSynch", RpcTarget.All, randVal);
            }
            GameObject track = Instantiate(tracks[randN]);
            track.transform.position = new Vector3(0f, 0f, trackLength * i);
            track.transform.parent = GameObject.Find("InGame").transform.GetChild(1).transform;
        }
    }

    [PunRPC]
    void TrackNumSynch(int r)
    {
        randN = r;
    }
}
