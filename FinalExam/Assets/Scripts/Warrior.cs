using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Player
{
    bool isAura = false;
    bool isShield = false;

    bool shieldAble = true;
    bool auraAble = true;

    // Start is called before the first frame update
    void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (PV.IsMine)
        {
            Attack();
            Shield();
            Aura();
            Debug.Log(auraAble);
        }
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0) && !isAttack && !otherSkillUsing())
        {
            animAdjVar = 1;
            isAttack = true;
            animator.SetBool("isAttack", true);
        }
    }
    void Shield()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isShield && shieldAble && !otherSkillUsing())
        {
            animAdjVar = 1;
            animator.SetBool("Shield", true);

        }
        else if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            animator.SetBool("Shield", false);
        }
    }

    void Aura()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isAura && auraAble && !otherSkillUsing())
        {
            isAttack = true;
            animAdjVar = 1;
            StartCoroutine(CoolTime(5.0f, "Aura"));
            animator.SetBool("Aura", true);
        }
    }

    void OtherSkillLock()
    {

    }

    bool otherSkillUsing()
    {
        if (isAttack || isAura || isShield)
            return true;
        else
            return false;
    }

    IEnumerator CoolTime(float time,string skillname)
    {
        switch (skillname)
        {
            case "Aura":
                auraAble = false;
                break;
            case "Shield":
                shieldAble = false;
                break;
        }

        yield return new WaitForSeconds(time);

        switch (skillname)
        {
            case "Aura":
                auraAble = true;
                break;
            case "Shield":
                shieldAble = true;
                break;
        }
    }
}
