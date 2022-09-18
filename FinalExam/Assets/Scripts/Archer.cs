using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Player
{
    public GameObject arrowObject;
    private GameObject bowObject;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        bowObject = GameObject.Find("Erika_Archer_Body_Mesh");
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
            GameObject arrow = Instantiate(arrowObject, bowObject.transform.position, bowObject.transform.rotation);
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
