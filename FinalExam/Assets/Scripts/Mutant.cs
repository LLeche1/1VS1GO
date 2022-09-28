using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mutant : Player
{
    bool isCollaspe = false;

    bool collaspeAble = true;
    
    // Start is called before the first frame update
    private void Awake()
    {
        base.Awake();

    }
    // Update is called once per frame
    void Update()
    {
        base.Update();
        if(PV.IsMine)
        {
            Attack();
            Collapsion();
        }
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0) && !isAttack/* && *!otherSkillUsing()*/)
        {
            animAdjVar = 1;
            isAttack = true;
            animator.SetBool("isAttack", true);
        }
    }

    void Collapsion()
    {
        if (Input.GetKeyDown(KeyCode.Q)/* && !isCollaspe*/)
        {
            animAdjVar = 1;
            isCollaspe = true;
            isAttack = true;
            animator.SetBool("Collapsion", true);
        }
    }
}
