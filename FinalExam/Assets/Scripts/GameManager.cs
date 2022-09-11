using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float time = 300;
    public Image greenHp;
    public Image redHp;
    public Text timeText;
    Wagon wagon;

    void Awake()
    {
        Application.targetFrameRate = 144;
        wagon = GameObject.Find("Wagon").GetComponent<Wagon>();
    }

    void Update()
    {
        Hp();
        limitTime();
    }

    void Hp()
    {
        greenHp.fillAmount = wagon.greenHp * 0.01f;
        redHp.fillAmount = wagon.redHp * 0.01f;

        if(wagon.greenHp == 0)
        {

        }
    }

    void limitTime()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            timeText.text = TimeSpan.FromSeconds(time).ToString(@"m\:ss");
        }
    }
}
