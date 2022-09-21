using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Camera : MonoBehaviour
{
    public static Camera instance;
    public float distance;
    public float height;
    string playerObjName;
    private GameObject playerObj;
    public Transform playerTr;

    void Start()
    {
        instance = this;
    }
    void Update()
    {
        Vector3 pos = playerTr.position - (Vector3.forward * distance) + (Vector3.up * height);
        gameObject.transform.position = pos;
    }
}
