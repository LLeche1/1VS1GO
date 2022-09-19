using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public float distance;
    public float height;
    private Transform player;

    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 pos = player.position - (Vector3.forward * distance) + (Vector3.up * height);
        gameObject.transform.position = pos; 
    }
}
