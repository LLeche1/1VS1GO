using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    private void Update()
    {
        OutLandDestroy();
    }
    void OutLandDestroy()
    {
        if(transform.position.y < -1f)
        {
            Destroy(gameObject);
        }
    }
}
