using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisionHelper : MonoBehaviour
{
    void Start()
    {
        var p = GameObject.FindGameObjectsWithTag("PlaneSlicer");
        if(p != null)
            foreach (var o in p)
            {
                var c = o.GetComponent<Collider>();
                if (c != null)
                {
                    Physics.IgnoreCollision(c, GetComponent<Collider>());
                }
            }
    }
}
