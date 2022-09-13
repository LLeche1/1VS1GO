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
    private float hp = 100;
    private float speed = 0;
    private float hAxis;
    private float vAxis;
    private float jAxis;
    private bool isJump;
    private GameObject wagon;
    private GameObject[] player;
    private Image hpBar;
    private Vector3 moveDir;
    private Animator animator;
    private PhotonView PV;
    public bool isAttack;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        hpBar = transform.Find("Canvas").transform.GetChild(0).gameObject.GetComponent<Image>();
        hpBar.color = PV.IsMine ? Color.green : Color.red;
        wagon = GameObject.FindGameObjectWithTag("Wagon");
    }

    void Update()
    {
        if (PV.IsMine)
        {
            GetInput();
            Move();
            Jump();
            Attack();
            HpBar();
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
        for(int i = 0; i < player.Length; i++)
        {
            if(player[i].name != gameObject.name)
            {
                GameObject other = player[i];
                other.transform.Find("Hp_UI").transform.Find("HpBar").transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.red;
            }
        }
    }

    void Move()
    {
        if(gameObject.transform.position.z < wagon.transform.position.z - 10)
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
            else if (vAxis == -1)
            {
                vAxis = 1;
                speed = 1;
            }
            gameObject.transform.Translate(new Vector3(hAxis, 0, vAxis) * speed * Time.deltaTime);
        }
    }

    private void Jump()
    { 
        if (jAxis == 1 && !isJump)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 5, ForceMode.Impulse);
            isJump = true;
        }
    }
    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isAttack)
        {
            isAttack = true;
            animator.SetBool("isAttack", isAttack);
            StartCoroutine(AttackDelay());

        }
    }
    public void JumpBtn()
    {
        jAxis = 1;
        Jump();
    }

    private void HpBar()
    { 
        hpBar.fillAmount = hp * 0.01f;
        hpBar.transform.position = gameObject.transform.position + new Vector3(0, 3, 0);
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.917f);
        isAttack = false;
        animator.SetBool("isAttack", isAttack);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Wagon")
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 0.15f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hp);
        }
        else
        {
            hp = (float)stream.ReceiveNext();
        }
    }
}
