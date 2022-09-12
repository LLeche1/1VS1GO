using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBridge : MonoBehaviour
{
    public GameObject bridge;
    private GameObject[] player;
    private GameObject distant;
    private GameObject spawnBridge;
    private int count = 0;

    void Start()
    {
        if (gameObject.name == "Bridge")
        {
            Generate();
        }
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
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].transform.position.z < player[i + 1].transform.position.z)
                {
                    distant = player[i];
                }
            }
        }
        if (this.gameObject.transform.position.z + 60 < distant.transform.position.z)
        {
            if (this.gameObject.name != "Bridge")
            {
                Destroy();
            }
        }
    }

    void Generate()
    {
        count += 1;
        spawnBridge = Instantiate(bridge, new Vector3(0, -10, 60 * count), Quaternion.Euler(new Vector3(-90, 0, 0)));
        Invoke("Generate", 8.5f);
    }

    void Destroy()
    {
        Destroy(this.gameObject);
    }
}
