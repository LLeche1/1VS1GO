using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Box : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    PhotonView PV;
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
            PV.RPC("disappear", RpcTarget.All);
    }

    [PunRPC]
    void disappear()
    {
        gameObject.active = false;
    }
}
