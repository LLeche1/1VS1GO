using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float speed = 0;
    private float hAxis;
    private float vAxis;
    private float jAxis;
    private Vector3 moveDir;
    private bool isJump;

    void Update()
    {
        GetInput();
        Move();
        Jump();
    }

    void GetInput()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        jAxis = Input.GetAxis("Jump");
    }

    void Move()
    {
        if (vAxis == 0)
        {
            vAxis = 1;
            speed = 5;
        }
        else if (vAxis == 1)
        {
            speed = 10;
        }
        else if(vAxis == -1)
        {
            vAxis = 1;
            speed = 1;
        }
        gameObject.transform.Translate(new Vector3(hAxis, 0, vAxis) * speed * Time.deltaTime);
    }

    void Jump()
    {
        if (jAxis == 1 && !isJump)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 5, ForceMode.Impulse);
            isJump = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
        }
    }
}
