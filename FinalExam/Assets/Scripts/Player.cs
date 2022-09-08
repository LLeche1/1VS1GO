using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    private float hAxis;
    private float vAxis;
    private float jDown;
    private Vector3 moveDir;
    private bool isJump;

    void Update()
    {
        GetInput();
        Move();
    }

    void GetInput()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        jDown = Input.GetAxis("Jump");
    }

    void Move()
    {
        gameObject.transform.Translate(new Vector3(hAxis, jAxis, vAxis) * speed * Time.deltaTime);
        //moveDir = (Vector3.forward * vAxis + Vector3.right * hAxis).normalized;
        //gameObject.transform.Translate(moveDir * speed * Time.deltaTime);
    }

    void Jump()
    {
        if (jDown && !isJump)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
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
