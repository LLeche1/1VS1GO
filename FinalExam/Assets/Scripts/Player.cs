using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private GameObject hpBar;
    private Image hpBarImg;
    private float hp = 100;
    private float speed = 0;
    private float hAxis;
    private float vAxis;
    private float jAxis;
    private Vector3 moveDir;
    private bool isJump;
    public bool isAttack = false;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        hpBar = GameObject.FindGameObjectWithTag("hpBar");
        hpBarImg = GameObject.FindGameObjectWithTag("hpBarImg").GetComponent<Image>();
        //Debug.Log(hpBarImg);
        GetInput();
        Move();
        Jump();
        HpBar();
        Attack();
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
        hpBarImg.fillAmount = hp * 0.01f;
        hpBar.transform.position = gameObject.transform.position + new Vector3(0, 3f, 0);
    }
    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.951f);
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
}
