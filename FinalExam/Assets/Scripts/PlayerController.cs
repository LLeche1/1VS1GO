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
    public bool isJump = false;
    private bool isMove = false;
    private bool isHit = false;
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
    Material material;

    public bool jumpKeyDown = false;
    private bool slideKeyDown = false;
    private bool isSlide = false;
    public bool isFallDown = false;
    private bool fallAble = true;
    public Vector2 jumpMoveDir;
    private float jumpMoveMag;

    private void Awake()
    {
        material = transform.Find("Bodies").Find("MainBody01").GetComponent<SkinnedMeshRenderer>().sharedMaterial;
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

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1.0f || Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1.0f || inputDir != Vector2.zero)
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

            animator.SetBool("isRun", isMove);
            animator.SetFloat("Speed", moveMag);
            if (isMove)
            {
                transform.Translate(new Vector3(inputDir.x, 0, inputDir.y) * speed * Time.deltaTime, Space.World);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(inputDir.x, 0f, inputDir.y)), 20f * Time.deltaTime);
                //������ Euler Rotation ���� �˰� ������ LookRotation�� ����ϸ� �ȴ�.
            }
        }
        else if (isJump)
        {
            transform.Translate(new Vector3(jumpMoveDir.x, 0, jumpMoveDir.y) * speed * Time.deltaTime, Space.World);
            //transform.LookAt(transform.position + new Vector3(jumpMoveDir.x, 0, jumpMoveDir.y));
        }
    }

    void Jump() //Ű���� ����
    {
        if (jDown && !isJump)
        {
            jumpMoveDir = inputDir;
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            isJump = true;
            animator.SetBool("isJump", isJump);
        }
    }

    public void ButtonJump() //UI ���� ��ư ����
    {
        if (!isJump)
        {
            jumpMoveDir = inputDir;
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            isJump = true;
            animator.SetBool("isJump", true);
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
        animator.SetBool("isFallDown", isFallDown);
        fallAble = true;
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.tag == "Floor")
        {
            isJump = false;
            animator.SetBool("isJump", false);
            if (isHit)
            {

            }
        }

        if (collision.transform.tag == "CannonBall")
        {
            Vector3 contatsDir = collision.contacts[0].normal;
            rb.velocity = Vector3.zero;
            rb.AddForce(new Vector3(0f, 3f, 0f), ForceMode.Impulse);
            jumpMoveDir = new Vector2(contatsDir.x, contatsDir.z).normalized * 1.5f;
            isJump = true;
            animator.SetBool("isJump", isJump);

            collision.transform.GetComponent<Rigidbody>().velocity = -contatsDir.normalized * 15f;
        }

        if (collision.transform.tag == "Spike")
        {
            if (!isHit)
            {
                rb.AddForce(new Vector3(0f, 1.5f, 0f), ForceMode.Impulse);
                jumpMoveDir = new Vector2(0f, -0.5f);
                isJump = true;
                isHit = true;
                animator.SetBool("isJump", isJump);
                StartCoroutine(UnHittable());
            }
        }
        if (collision.transform.tag == "JumpPad")
        {

            isJump = true;
            jumpMoveDir = inputDir;
            rb.velocity = Vector3.zero;
            rb.AddForce(new Vector3(0, 10f, 0), ForceMode.Impulse);
            animator.SetBool("isJump", true);

        }

        if (collision.transform.tag == "SpeedPad")
        {

        }
    }
    IEnumerator UnHittable()
    {
        material.SetColor("_Color", Color.red);
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 5; i++)
        {
            material.SetColor("_Color", Color.white);
            yield return new WaitForSeconds(0.25f);
            material.SetColor("_Color", Color.grey);
            yield return new WaitForSeconds(0.25f);
        }
        material.SetColor("_Color", Color.white);
        isHit = false;
    }
}
