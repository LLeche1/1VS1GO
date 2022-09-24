using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Camera : MonoBehaviour
{
    public Transform playerTr;
    private float distance = 10;
    private float height = 3;
    private GameObject playerObj;

    void Update()
    {
        Vector3 pos = playerTr.position - (Vector3.forward * distance) + (Vector3.up * height);
        gameObject.transform.position = pos;
    }
}
