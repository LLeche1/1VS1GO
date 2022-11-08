using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;

public class CannonDiamond : MonoBehaviour
{
    [SerializeField]
    private float speed = 100;
    GameManager gameManager;
    PhotonView PV;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        PV = GetComponent<PhotonView>();
    }

    void Update ()
    {
        transform.Rotate(Vector3.forward * speed * Time.deltaTime);
    }

    [PunRPC]
    void Score(string team)
    {
        if(team == "Blue")
        {
            gameManager.blueScore++;
        }
        else if (team == "Red")
        {
            gameManager.redScore++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            foreach(GameObject player in gameManager.players)
            {
                if(player.name == other.name)
                {
                    if (player.transform.GetComponent<PlayerController>().team == "Blue")
                    {
                        PV.RPC("Score", RpcTarget.All, "Blue");
                    }
                    else if (player.transform.GetComponent<PlayerController>().team == "Red")
                    {
                        PV.RPC("Score", RpcTarget.All, "Red");
                    }
                }
            }
            //PhotonNetwork.Destroy(gameObject);
        }
    }
}
