using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float hAxis;
    private float vAxis;
    private bool jDown = false;
    private bool isJump = false;
    private bool isMove = false;    
    private float speed = 5.0f;
    private float jumpForce = 5.0f;

    private Vector3 moveDir;
    private float moveMag;

    private Transform tr;
    private Animator animator;
    private Rigidbody rb;
    

    // Start is called before the first frame update

    private void Awake()
    {
        
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Jump();
        //Debug.Log(moveDir);
        //Debug.Log(isMove);
    }

    void GetInput()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        jDown = Input.GetButtonDown("Jump");

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1.0f || Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1.0f)
        {
            isMove = true;
        }
        else
        {
            isMove = false;
        }
    }
    
    void Move()
    {
        moveDir = new Vector3(hAxis, 0, vAxis);
        moveMag = new Vector3(hAxis, 0, vAxis).magnitude;
        tr.Translate(moveDir * speed * Time.deltaTime, Space.World);

        if (isMove)
        {
            tr.LookAt(tr.position + moveDir);
        }

        animator.SetBool("isRun", moveDir != Vector3.zero);
        animator.SetFloat("Speed", moveMag);
    }

    void Jump()
    {
        if(jDown && !isJump)
        {
            isJump = true;
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            animator.SetBool("isJump", true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Floor")
        {
            isJump = false;
            animator.SetBool("isJump", false);
        }
    }
}
