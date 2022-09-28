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
    private Vector3 moveDir;
    protected Animator animator;
    private PhotonView PV;
    private bool isJump;
    private bool isHit = false;
    private Camera camera;
    GameManager gameManager;

    private GameObject circle;

    private const float maxAnimSpeed = 12f;
    private float animSpeed = 1;
    protected float animAdjVar = 1;
    protected void Awake()
    {
        playerHp = 100;
        PV = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        wagon = GameObject.Find("Wagon");
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    protected void Update()
    {
        if (PV.IsMine)
        {
            GetInput();
            Move();
            Jump();
            Attack();
            Camera();
            AdjustAnimation();
        }

        //circle = GameObject.Find(PhotonNetwork.LocalPlayer.NickName + "Circle");
        //circle.transform.position = new Vector3(gameObject.transform.position.x, 0.6f, gameObject.transform.position.z);
    }

    void Camera()
    {
        camera.cameraPlayer = gameObject;
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
            animSpeed = speed / maxAnimSpeed;
            if (vAxis == 0)
            {
                if (speed > 7)
                    speed += -15f * Time.deltaTime;
                else
                    speed += 15f * Time.deltaTime;
                animator.SetFloat("AnimationSpeed", animSpeed);
            }
            else if (vAxis == 1f)
            {
                if (speed < 12)
                    speed += 10f * Time.deltaTime;
                animator.SetFloat("AnimationSpeed", animSpeed);
            }
            else if (vAxis == -1f)
            {
                animator.SetFloat("AnimationSpeed", animSpeed);

                if (speed > 3)
                    speed += -10f * Time.deltaTime;
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
    void AdjustAnimation()
    {

        if (animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.7)
        {
            if (animAdjVar >= 0)
                animAdjVar -= 5 * Time.deltaTime;
        }

        if (animator.GetCurrentAnimatorStateInfo(2).normalizedTime > 0.7)
        {
            if (animAdjVar >= 0)
                animAdjVar -= 5 * Time.deltaTime;
        }

        animator.SetLayerWeight(1, animAdjVar);
        animator.SetLayerWeight(2, animAdjVar);

        if (animator.GetLayerWeight(1) < 0.2f)
        {
            animator.SetBool("isAttack", false);
            animator.SetBool("Aura", false);
            isAttack = false;
        }
        if (animator.GetLayerWeight(2) < 0.2f)
        {
            animator.SetBool("isAttack", false);
            animator.SetBool("Collapsion", false);
            isAttack = false;
        }
        if (!isAttack)
        {
            animAdjVar = 1;
        }

    }
    protected void Attack()
    {
        if (Input.GetMouseButtonDown(0) && !isAttack)
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

    protected void Hit()
    {
        photonView.RPC("RpcHit", RpcTarget.All);
    }

    [PunRPC]
    protected void RpcHit()
    {
        isHit = true;
        playerHp += -50;
        if (playerHp <= 0)
        {
            if(myTeam == "a")
            {
                gameManager.bKill++;
                gameManager.aDeath++;
            }
            else if(myTeam == "b")
            {
                gameManager.aKill++;
                gameManager.bDeath++;
            }
            isDead = true;
        }
        StartCoroutine(HitDelay());
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
            animator.SetBool("isJump", false);
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
