using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool isAttack;
    public bool isDead = false;
    public string classType;
    public string myTeam;
    public float playerHp;
    public GameObject wagon;
    private float speed = 5;
    private float hAxis;
    private float vAxis;
    private float jAxis;
    private GameObject[] player;
    private Image hpBar;
    private Vector3 moveDir;
    protected Animator animator;
    private PhotonView PV;
    private bool isJump;
    private bool isHit = false;
    Camera camera;


    protected void Awake()
    {
        playerHp = 100;
        PV = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        hpBar = transform.Find("Canvas").transform.Find("HpBar").gameObject.GetComponent<Image>();
        hpBar.color = PV.IsMine ? Color.green : Color.red;
        wagon = GameObject.Find("Wagon");
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    protected void Update()
    {
        if (PV.IsMine)
        {
            GetInput();
            Move();
            Jump();
            Attack();
            HpBar();
            Death();
            Camera();
        }
    }

    void Camera()
    {
        camera.playerTr = gameObject.transform;
    }

    void GetInput()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        jAxis = Input.GetAxis("Jump");
    }

    void Move()
    {
        if (wagon.transform.position.z - transform.transform.position.z > 10)
        {
            if (vAxis == 0)
            {
                vAxis = 1;
                speed = 5;
                animator.SetFloat("AnimationSpeed", 1.0f);
            }
            else if (vAxis == 1)
            {
                speed = 10;
                animator.SetFloat("AnimationSpeed", 1.5f);
            }
            else if (vAxis == -1)
            {
                vAxis = 1;
                animator.SetFloat("AnimationSpeed", 0.75f);
                speed = 2;
            }
            Vector3 moveDir = new Vector3(hAxis, 0, 3f).normalized;
            transform.LookAt(transform.position + moveDir);
            transform.Translate(moveDir * speed * Time.deltaTime);
        }
    }

    void Jump()
    {
        if (jAxis == 1 && !isJump)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 5, ForceMode.Impulse);
            isJump = true;
        }
    }

    protected void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isAttack)
        {
            isAttack = true;
            animator.SetBool("isAttack", isAttack);
            animator.SetTrigger("Recoil");
            StartCoroutine(AdjustAttackTime());
        }
    }

    protected IEnumerator AdjustAttackTime()
    {
        yield return new WaitForSeconds(0.683f);
        isAttack = false;
        animator.SetBool("isAttack", isAttack);
    }

    void Death()
    {
        if (playerHp <= 0 && !isDead)
        {
            isDead = true;
        }
    }

    protected void Hit()
    {
        photonView.RPC("RpcHit", RpcTarget.All);
    }

    [PunRPC]
    protected void RpcHit()
    {
        isHit = true;
        playerHp += -50;
        HpBar();
        StartCoroutine(HitDelay());
    }

    void HpBar()
    {
        hpBar.fillAmount = playerHp / 100;
        hpBar.transform.position = gameObject.transform.position + new Vector3(0, 3, 0);
    }

    IEnumerator HitDelay()
    {
        yield return new WaitForSeconds(0.5f);
        isHit = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
        }

        if (collision.gameObject.tag == "Player")
        {
            if (isAttack)
            {
                collision.gameObject.transform.GetComponent<Player>().Hit();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wagon")
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 0.15f);
        }
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerHp);
        }
        else
        {
            playerHp = (float)stream.ReceiveNext();
        }
    }
}
