using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Chariot : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // layer 7 = RunningGameObjects
        if (collision.gameObject.layer == 7)
        {
            collision.transform.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-4f, 4f), 2f, 2f) * 10f, ForceMode.Impulse);
        }
    }
}
