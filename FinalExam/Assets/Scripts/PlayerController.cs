using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private float hAxis;
    private float vAxis;
    public bool jDown = false;
    private bool isJump = false;
    private bool isMove = false;
    private float speed = 5.0f;
    private float jumpForce = 5.0f;
    private Vector2 inputDir = Vector2.zero;

    private Vector3 moveDir;
    private float moveMag;

    private PhotonView PV;
    private Transform tr;
    private Animator animator;
    private Rigidbody rb;


    private JoyStick joyStick;
    private Button jumpBt;

    // Start is called before the first frame update

    private void Awake()
    {

        PV = GetComponent<PhotonView>();
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();


        if (PV.IsMine)
        {
            joyStick = GameObject.Find("JoyStick").GetComponent<JoyStick>();
            jumpBt = GameObject.Find("JumpButton").GetComponent<Button>();
            if (jumpBt != null)
            {
                jumpBt.onClick.AddListener(ButtonJump);
            }
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            GetInput();
            Jump();
            JoyStickMove();
            //Move();
        }
    }

    void GetInput()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        jDown = Input.GetButtonDown("Jump");
        this.inputDir = joyStick.inputDir;

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

        transform.Translate(new Vector3(inputDir.x, 0, inputDir.y) * speed * Time.deltaTime, Space.World);
        transform.LookAt(transform.position + new Vector3(inputDir.x, 0, inputDir.y));
    }
    void JoyStickMove()
    {
        moveMag = joyStick.inputDir.magnitude;

        animator.SetBool("isRun", inputDir != Vector2.zero);
        animator.SetFloat("Speed", moveMag);

        transform.Translate(new Vector3(inputDir.x, 0, inputDir.y) * speed * Time.deltaTime, Space.World);
        transform.LookAt(transform.position + new Vector3(inputDir.x, 0, inputDir.y));
    }

    void Jump()
    {
        if (jDown && !isJump)
        {
            isJump = true;
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            animator.SetBool("isJump", true);
        }
    }
    public void ButtonJump()
    {
        if (!isJump)
        {
            isJump = true;
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            animator.SetBool("isJump", true);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Floor")
        {
            isJump = false;
            animator.SetBool("isJump", false);
        }
    }
}
