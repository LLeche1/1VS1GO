using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BallDetection : MonoBehaviour
{
    GameManager gameManager;
    PhotonView PV;
    public string plateType;
    public bool isGoal;
    private bool isChange = false;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        PV = transform.GetComponent<PhotonView>();
    }
    public void PlateTypeSelect(int n)
    {
        if (n == 0)
        {
            plateType = "SoccerBallPlate";
        }
        else if (n == 1)
        {
            plateType = "BasketBallPlate";
        }
        else if (n == 2)
        {
            plateType = "BeachBallPlate";
        }
    }
    [PunRPC]
    void BallShootingGameScore(string team)
    {
        if (team == "Blue")
        {
            gameManager.blueScore++;
        }
        else if (team == "Red")
        {
            gameManager.redScore++;
        }
    }
    [PunRPC]
    void PlateRandomChange(int r)
    {
        StartCoroutine(PlateChange(r));
    }
    IEnumerator PlateChange(int r)
    {
        isChange = true;
        transform.Find(plateType + "Image").gameObject.SetActive(false);
        transform.Find("GoalImage").gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        transform.Find("GoalImage").gameObject.SetActive(false);
        transform.GetChild(r).gameObject.SetActive(true);
        PlateTypeSelect(r);
        isChange = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (other.gameObject.name + "Plate" == plateType)
            {
                if (other.transform.tag == "BlueBall")
                {
                    PV.RPC(nameof(BallShootingGameScore), RpcTarget.All, "Blue");
                    PV.RPC(nameof(PlateRandomChange), RpcTarget.All, Random.Range(0, 3));
                    PhotonNetwork.Destroy(other.gameObject);
                }
                else if (other.transform.tag == "RedBall")
                {
                    PV.RPC(nameof(BallShootingGameScore), RpcTarget.All, "Red");
                    PV.RPC(nameof(PlateRandomChange), RpcTarget.All, Random.Range(0, 3));
                    PhotonNetwork.Destroy(other.gameObject);
                }
            }
        }
    }
}
