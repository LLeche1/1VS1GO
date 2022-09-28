using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Player
{
    bool AuraAble = true;

    // Start is called before the first frame update
    void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        /*if (PV.IsMine)
        {
            Shield();
            Aura();
            Debug.Log(AuraAble);
        }*/
    }

    /*void Shield()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animAdjVar = 1;
            animator.SetBool("Shield", true);
            
        }
        else if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            animator.SetBool("Shield", false);
            //animator.GetCurrentAnimatorStateInfo(0).normalizedTime 
        }
    }

    void Aura()
    {
        if (Input.GetKeyDown(KeyCode.Q) && AuraAble)
        {
            StartCoroutine(CoolTime(3.0f, AuraAble));
            animAdjVar = 1;
            animator.SetTrigger("Aura");
        }
    }

    IEnumerator CoolTime(float time, bool skil)
    {
        AuraAble = false;
        yield return new WaitForSeconds(time);
        AuraAble = true;

    }*/
}
