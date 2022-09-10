using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wagon : MonoBehaviour
{
    public float greenHp = 100f;
    public float redHp = 100f;
    public GameObject player;
    private float speed = 7f;

    void Update()
    {
        if(player.transform.position.z + 60 > gameObject.transform.position.z)
        {
            gameObject.transform.Translate(new Vector3(-1, 0, 0) * speed * Time.deltaTime);
        }
    }
}
