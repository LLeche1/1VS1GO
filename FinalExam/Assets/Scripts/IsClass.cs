using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsClass : MonoBehaviour
{
    public string classType;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
