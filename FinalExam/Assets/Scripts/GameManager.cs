using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Image greenHp;
    public Image redHp;
    Wagon wagon;

    void Awake()
    {
        Application.targetFrameRate = 144;
        wagon = GameObject.Find("Wagon").GetComponent<Wagon>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Hp();
    }

    void Hp()
    {
        greenHp.fillAmount = wagon.greenHp * 0.01f;
        redHp.fillAmount = wagon.redHp * 0.01f;

        if(wagon.greenHp == 0)
        {

        }
    }
}
