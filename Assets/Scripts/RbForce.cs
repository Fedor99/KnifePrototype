using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RbForce : MonoBehaviour
{
    private Vector3 force;

    public void AddForce(Vector3 f)
    {
        force = f;
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<Rigidbody>() != null)
            GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }
}
