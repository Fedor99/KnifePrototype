using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public float followSpeed = 0.2f;
    public Transform transformToFollow;
    private Vector3 positionDifference;

    // Start is called before the first frame update
    void Start()
    {
        positionDifference = transformToFollow.position - transform.position;   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, transformToFollow.position - positionDifference, followSpeed * Time.smoothDeltaTime);
    }
}
