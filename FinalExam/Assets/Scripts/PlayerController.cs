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
    private JoyStick joyStick;
    private Button jumpBtn;
    private Button slideBtn;
    public bool isDead = false;
    PhotonView PV;
    Transform tr;
    Animator animator;
    Rigidbody rb;
    CameraController cameraController;

    public bool jumpKeyDown = false;
    private bool slideKeyDown = false;
    private bool isSlide = false;
    public bool isFallDown = false;
    private bool fallAble = true;
    private Vector2 jumpMoveDir;
    private float jumpMoveMag;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();

        if (PV.IsMine)
        {
            joyStick = GameObject.Find("JoyStick").GetComponent<JoyStick>();
            jumpBtn = GameObject.Find("Button_Jump").GetComponent<Button>();
            slideBtn = GameObject.Find("Button_Slider").GetComponent<Button>();
            if (jumpBtn != null)
            {
                jumpBtn.onClick.AddListener(ButtonJump);
            }
            if (slideBtn != null)
            {
                slideBtn.onClick.AddListener(ButtonSlide);
            }
        }
    }

    void Update()
    {
        if (PV.IsMine)
        {
            GetInput();
            JoyStickMove();
            Jump();
            Slide();
            FallDown();
            cameraController.player = gameObject;
        }
    }

    void GetInput()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        jDown = Input.GetButtonDown("Jump");
        slideKeyDown = Input.GetKeyDown(KeyCode.Q);
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
    }

    void JoyStickMove()
    {
        if (!isSlide && !isFallDown && !isJump)
        {
            moveMag = joyStick.inputDir.magnitude;

            animator.SetBool("isRun", inputDir != Vector2.zero);
            animator.SetFloat("Speed", moveMag);

            transform.Translate(new Vector3(inputDir.x, 0, inputDir.y) * speed * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(inputDir.x, 0f, inputDir.y)), 20f * Time.deltaTime);
            //벡터의 Euler Rotation 값을 알고 싶으면 LookRotation을 사용하면 된다.
        }
        else if (isJump)
        {
            transform.Translate(new Vector3(jumpMoveDir.x, 0, jumpMoveDir.y) * speed * Time.deltaTime, Space.World);
            transform.LookAt(transform.position + new Vector3(jumpMoveDir.x, 0, jumpMoveDir.y));
        }
    }

    void Jump() //키보드 조작
    {
        if (jDown && !isJump)
        {
            isJump = true;
            jumpMoveDir = inputDir;
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            animator.SetBool("isJump", isJump);
        }
    }

    public void ButtonJump() //UI 점프 버튼 조작
    {
        if (!isJump)
        {
            isJump = true;
            jumpMoveDir = inputDir;
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

    void Slide()
    {
        if (slideKeyDown && !isSlide)
        {
            isSlide = true;
            animator.SetBool("isSlide", isSlide);
            StartCoroutine(SlideCoolTime());
            rb.AddForce(new Vector3(0, jumpForce / 2, 0), ForceMode.Impulse); ;
        }
    }

    void ButtonSlide()
    {
        if (!isSlide)
        {
            isSlide = true;
            animator.SetBool("isSlide", isSlide);
            StartCoroutine(SlideCoolTime());
            rb.AddForce(new Vector3(0, jumpForce / 2, 0), ForceMode.Impulse); ;
        }
    }

    IEnumerator SlideCoolTime()
    {
        yield return new WaitForSeconds(1.167f);
        isSlide = false;
        animator.SetBool("isSlide", isSlide);
    }
    void FallDown()
    {
        if (isFallDown && fallAble)
        {
            fallAble = false;
            animator.SetBool("isSlide", false);
            animator.SetBool("isFallDown", isFallDown);
            StartCoroutine(FallUp());
        }
    }

    IEnumerator FallUp()
    {
        yield return new WaitForSeconds(1f);
        isFallDown = false;
        fallAble = true;
        animator.SetBool("isFallDown", isFallDown);
    }
}
