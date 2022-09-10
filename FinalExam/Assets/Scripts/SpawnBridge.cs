using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBridge : MonoBehaviour
{
    public GameObject bridge;
    public GameObject player;
    private GameObject spawnBridge;
    private int count = 0;

    void Start()
    {
        if(gameObject.name == "Bridge")
        {
            Generate();
        }
    }

    void Update()
    {
        if(this.gameObject.transform.position.z + 60 < player.transform.position.z)
        {
            if(this.gameObject.name != "Bridge")
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
