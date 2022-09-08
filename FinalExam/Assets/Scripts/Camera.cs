using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform player;
    public float distance;
    public float height;

    void Update()
    {
        Vector3 pos = player.position - (Vector3.forward * distance) + (Vector3.up * height);
        gameObject.transform.position = pos;
    }
}
