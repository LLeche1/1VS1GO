using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    float hAxis;
    float vAxis;
    Vector3 moveDir;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        move();
    }

    private void move()
    {
        Vector3 moveDir = new Vector3(hAxis, 0, vAxis).normalized;
        transform.Translate(moveDir * 10 * Time.deltaTime);
        
    }

    private void GetInput()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
    }


}
