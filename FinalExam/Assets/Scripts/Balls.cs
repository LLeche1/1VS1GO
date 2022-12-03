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

    private Vector3 netPos;
    private Quaternion netRot;
    private Vector3 netVel;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
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
                PhotonNetwork.Destroy(gameObject);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(rb.rotation);
            stream.SendNext(rb.velocity);
        }
        else if (stream.IsReading)
        {
            rb.position = (Vector3) stream.ReceiveNext();
            rb.rotation = (Quaternion) stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
            rb.position += rb.velocity * lag;
        }
    }
}
