using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBelowPosition : MonoBehaviour
{
    public float Y_Limit = -100;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < Y_Limit)
            Destroy(gameObject);
    }
}
