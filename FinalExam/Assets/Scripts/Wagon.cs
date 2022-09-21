using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wagon : MonoBehaviourPunCallbacks
{
    public float oneTeamHP = 100f;
    public float twoTeamHP = 100f;
    private GameObject[] player;
    private GameObject distant;
    private PhotonView PV;
    private float speed = 7f;
    private bool isHit = false;

    void Start()
    {
        PV = GetComponent<PhotonView>();
    }
    void Update()
    {
        distance();
    }

    void distance()
    {
        player = GameObject.FindGameObjectsWithTag("Player");

        if (player.Length == 1)
        {
            distant = player[0];
        }
        else
        {
            for (int i = 0; i < player.Length - 1; i++)
            {
                if (player[i].transform.position.z < player[i + 1].transform.position.z)
                {
                    distant = player[i];
                }
                else
                {
                    distant = player[i + 1];
                }
            }
        }

        if (distant.transform.position.z + 60 > gameObject.transform.position.z)
        {
            gameObject.transform.Translate(new Vector3(-1, 0, 0) * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" && other.transform.GetComponent<Player>().isAttack == true && isHit == false)
        {
            TeamType playerTeam = (TeamType)other.transform.GetComponent<Player>().myTeam;
            PV.RPC("WagonHit", RpcTarget.All, playerTeam);
        }
    }

    [PunRPC]
    void WagonHit(TeamType teamType)
    {
        isHit = true;
        if (teamType == TeamType.oneTeam)
            oneTeamHP += -30;
        else
            twoTeamHP += -30;
        StartCoroutine(WagonHitDelay());
    }
    IEnumerator WagonHitDelay()
    {
        yield return new WaitForSeconds(1.0f);
        isHit = false;
    }
}
