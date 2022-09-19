using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Player
{
    private GameObject archer;
    public GameObject arrowObject;
    private GameObject bowObject;
    private Transform arrowPos;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        archer = GameObject.Find("Archer");
        bowObject = GameObject.Find("Erika_Archer_Body_Mesh");
        arrowPos = transform.Find("ArrowPos").GetComponent<Transform>();
    }

    void Update()
    {
        base.Update();
    }

    protected override void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isAttack)
        {
            isAttack = true;
            animator.SetBool("isAttack", isAttack);
            animator.SetTrigger("Recoil");
            GameObject arrow = Instantiate(arrowObject, bowObject.transform.position, arrowObject.transform.rotation);
            Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
            arrowRb.velocity = arrowPos.transform.forward * 30;
            StartCoroutine(AdjustAttackTime());

        }
/*        else if (Input.GetKeyUp(KeyCode.Z) && isAttack)
        {
            animator.SetTrigger("Recoil");
            GameObject arrow = Instantiate(arrowObject, bowObject.transform.position, bowObject.transform.rotation);
            StartCoroutine(AdjustAttackTime());
        }*/
    }
    // Update is called once per frame

}
