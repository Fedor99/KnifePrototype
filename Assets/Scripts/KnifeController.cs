using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    Rigidbody rigidbody;
    VerticalObstacleDetector verticalObstacleDetector;

    public Collider[] collidersToEnable;

    public float upShift = 5;
    public float rightShift = 5;
    public float upSpeed = 0.25f;
    public float sideSpeed = 15f;
    public float sideDeceleration = 1f;
    public float fallSpeed = 10f;
    public float revolutionSpeed = 0.1f;
    public float revolutionCompleteAtAngle = -400; // z angle
    public float current_revolutionCompleteAtAngle = -400; // z angle
    float zAngle = 0;

    public float freeFromCollisionsTimerValue = 0.2f;
    private float freeFromCollisionsTimer;

    public KnifeState state = KnifeState.STATIONARY;

    public float slicingTimerValue = 0.2f;
    private float slicingTimer = 0;

    public void StartSlicing()
    {
        slicingTimer = slicingTimerValue;
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        verticalObstacleDetector = GetComponent<VerticalObstacleDetector>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (state == KnifeState.REVOLVING)
                current_revolutionCompleteAtAngle -= 360;
            SetState(KnifeState.REVOLVING);
            rigidbody.velocity = Vector3.zero;
        }

        if (state == KnifeState.REVOLVING)
            freeFromCollisionsTimer -= Time.deltaTime;

        if (slicingTimer > 0)
            slicingTimer -= Time.deltaTime;

        if (state == KnifeState.STATIONARY)
            foreach (var c in collidersToEnable)
                c.enabled = true;
        else
            foreach (var c in collidersToEnable)
                c.enabled = false;

    }



    private void FixedUpdate()
    {
        UserTriggeredRevolution();
    }

    float goUpTo_Y;
    float goRightTo_Z;
    bool fallDownFlag = false;

    private void UserTriggeredRevolution()
    {
        float lerpY = transform.position.y;
        float lerpZ = transform.position.z;

        if (fallDownFlag && (state == KnifeState.FALLING || state == KnifeState.REVOLVING))
        {
            lerpY -= fallSpeed * Time.deltaTime;
        }

        if (state != KnifeState.STATIONARY)
        {
            lerpZ = transform.position.z + (goRightTo_Z * sideSpeed * Time.deltaTime);

            goRightTo_Z = Mathf.Lerp(goRightTo_Z, 0, sideDeceleration);
        }

        if (state == KnifeState.REVOLVING)
        {

            if (transform.position.y > goUpTo_Y)
                fallDownFlag = true;

            if (!fallDownFlag)
            {
                lerpY = Mathf.Lerp(transform.position.y, goUpTo_Y + 1, upSpeed);
            }

            if (zAngle > current_revolutionCompleteAtAngle)
            {
                zAngle = Mathf.Lerp(zAngle, current_revolutionCompleteAtAngle - 5, revolutionSpeed);

                rigidbody.MoveRotation(Quaternion.Euler(
                    rigidbody.rotation.eulerAngles.x,
                    rigidbody.rotation.eulerAngles.y,
                    zAngle));
            }
            else
                SetState(KnifeState.FALLING);
        }
        else
        {
            if (zAngle < -360f) zAngle = zAngle + 360f;
            current_revolutionCompleteAtAngle = revolutionCompleteAtAngle;
        }

        // if in the process of slicing - dont move right
        if (slicingTimer > 0)
            lerpZ = transform.position.z;

        rigidbody.MovePosition(new Vector3(transform.position.x, lerpY, lerpZ));
    }

    public void SetState(KnifeState state)
    {
        switch (state)
        {
            case KnifeState.FALLING:
                if (this.state == state) return;

                break;
            case KnifeState.REVOLVING:

                goUpTo_Y = transform.position.y + upShift;

                // check for vertical obstacle on the right
                // in case if there is obstacle detected on the right - move towards the left side
                // if obstacle is detected on both sides - move up
                bool isNearverticalObstacle_onTheRight = verticalObstacleDetector.IsNearVerticalObstacle(Vector3.forward);
                bool isNearverticalObstacle_onTheLeft = verticalObstacleDetector.IsNearVerticalObstacle(-Vector3.forward);
                if (!isNearverticalObstacle_onTheRight)
                    goRightTo_Z = 1;
                else
                    if (!isNearverticalObstacle_onTheLeft)
                    goRightTo_Z = -1;
                else
                    goRightTo_Z = 0;

                fallDownFlag = false;

                if (this.state == state) return;
                freeFromCollisionsTimer = freeFromCollisionsTimerValue;
                break;
            case KnifeState.STATIONARY:
                if (this.state == state) return;
                if (freeFromCollisionsTimer > 0 && this.state == KnifeState.REVOLVING)
                    return;
                break;
        }
        this.state = state;
    }
}

public enum KnifeState { REVOLVING, STATIONARY, FALLING }
