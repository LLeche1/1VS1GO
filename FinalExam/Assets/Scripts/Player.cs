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
    private float initHP = 100;
    public float currHP = 0;
    private float speed = 5;
    private float hAxis;
    private float vAxis;
    private float jAxis;



    public GameObject wagon;
    private GameObject[] player;
    private Image hpBar;
    private Vector3 moveDir;
    protected Animator animator;
    private PhotonView PV;
    

    public bool isAttack;
    private bool isJump;
    public bool isDead = false;
    private bool isHIt = false;
    public string characterType;
    public int myTeam;
    protected virtual void Start()
    {
        PV = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        hpBar = transform.Find("Canvas").transform.Find("HpBar").gameObject.GetComponent<Image>();
        hpBar.color = PV.IsMine ? Color.green : Color.red;
        wagon = GameObject.Find("Wagon");
        currHP = initHP;

    }

    protected virtual void Update()
    {
        if (PV.IsMine)
        {
            GetInput();
            Move();
            Jump();
            Attack();
            HpBar();
            Death();
            Camera.instance.playerTr = gameObject.transform;
        }
    }

    void GetInput()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        jAxis = Input.GetAxis("Jump");
    }

    void Change()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < player.Length; i++)
        {
            if (player[i].name != gameObject.name)
            {
                GameObject other = player[i];
                other.transform.Find("Hp_UI").transform.Find("HpBar").transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.red;
            }
        }
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
    public void Jump()
    {
        if (jAxis == 1 && !isJump)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 5, ForceMode.Impulse);
            isJump = true;
        }
    }
    protected virtual void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isAttack)
        {
            isAttack = true;
            animator.SetBool("isAttack", isAttack);
            animator.SetTrigger("Recoil");
            StartCoroutine(AdjustAttackTime());

        }
    }
    void Death()
    {
        if (currHP <= 0 && !isDead)
        {
            isDead = true;
        }
    }

    public void HpBar()
    {
        hpBar.fillAmount = currHP / initHP;
        hpBar.transform.position = gameObject.transform.position + new Vector3(0, 3, 0);
    }

    [PunRPC]
    protected void RPCHit()
    {
        isHIt = true;
        currHP += -50;
        HpBar();
        StartCoroutine(HitDelay());
    }
    protected void Hit()
    {
        photonView.RPC("RPCHit", RpcTarget.AllBuffered);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
        }

        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("hit");
            if (isAttack)
            {
                Player hitPlayer = collision.transform.GetComponent<Player>();
                hitPlayer.Hit();
            }
        }
    }
    protected IEnumerator AdjustAttackTime()
    {
        yield return new WaitForSeconds(0.683f);
        isAttack = false;
        animator.SetBool("isAttack", isAttack);
    }
    IEnumerator HitDelay()
    {
        yield return new WaitForSeconds(0.5f);
        isHIt = false;
    }
    private void OnTriggerEnter(Collider other)
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
            stream.SendNext(currHP);
        }
        else
        {
            currHP = (float)stream.ReceiveNext();
            //HpBar();
        }
    }
}
