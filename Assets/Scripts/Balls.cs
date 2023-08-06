using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Balls : MonoBehaviourPunCallbacks, IPunObservable
{
    private PhotonView PV;
    private Rigidbody rb;
    private Vector3 remotePos;

    private float lag;
    private Vector3 netPos;
    private Quaternion curnRot;
    private Quaternion netRot;
    private Vector3 netVel;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        /*PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;*/
        if (!PhotonNetwork.IsMasterClient)
        {
            rb.useGravity = false;
        }
    }

    private void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if (transform.position.y < -10f)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rb.velocity);
        }
        else if (stream.IsReading)
        {
            rb.position = (Vector3)stream.ReceiveNext();
            rb.rotation = (Quaternion)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();


            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
            rb.position += rb.velocity * lag;
        }
    }
}
