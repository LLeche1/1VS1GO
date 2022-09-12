using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wagon : MonoBehaviour
{
    public float greenHp = 100f;
    public float redHp = 100f;
    private GameObject[] player;
    private GameObject distant;
    private float speed = 7f;

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
        if (other.transform.tag == "Player" && other.transform.GetComponent<Player>().isAttack == true)
        {
            greenHp += -10;
        }
    }
}
