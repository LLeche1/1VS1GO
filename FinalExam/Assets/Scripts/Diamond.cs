using UnityEngine;
using System.Collections;

public class Diamond : MonoBehaviour
{
    [SerializeField]
    private float speed = 100;
    GameManager gameManager;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update ()
    {
        transform.Rotate(Vector3.forward * speed * Time.deltaTime);
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
                        gameManager.blueScore++;
                    }
                    else if (player.transform.GetComponent<PlayerController>().team == "Red")
                    {
                        gameManager.redScore++;
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
