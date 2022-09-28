using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public string classType;
    public string lastScene = null;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
