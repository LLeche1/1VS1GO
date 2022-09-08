using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform player;
    public float distance = 16.0f;
    public float height = 6.0f;
    public float damping = 10.0f;

    void Update()
    {
        Vector3 pos = player.position - (Vector3.forward * distance) + (Vector3.up * height);
        gameObject.transform.position = Vector3.Slerp(player.position, pos, Time.deltaTime * damping);
        gameObject.transform.LookAt(player.position + (Vector3.up * 2));
    }
}
