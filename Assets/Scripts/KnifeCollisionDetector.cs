using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeCollisionDetector : MonoBehaviour
{
    public KnifeController knifeController;

    /// <summary>
    /// each transform in this array is supposed to contain two child objects
    /// named 'rayStart' and 'rayEnd' for the ray to be defined in 3d space
    /// </summary>
    public Transform[] rayCoordinates;

    private void FixedUpdate()
    {

        foreach (Transform r in rayCoordinates)
        {
            Transform collisionRayOrigin = ChildByName(r, "rayStart");
            Transform collisionRayEnd = ChildByName(r, "rayEnd");

            RaycastHit hit;
            Ray ray = new Ray();
            ray.origin = collisionRayOrigin.position;
            ray.direction = collisionRayEnd.position - collisionRayOrigin.position;

            Debug.DrawRay(ray.origin, ray.direction * Vector3.Distance(ray.origin, collisionRayEnd.position), Color.green);

            if (Physics.Raycast(
                ray,
                out hit))
            {
                if (hit.distance < Vector3.Distance(collisionRayOrigin.position, collisionRayEnd.position))
                {
                    if (hit.transform.tag == "ToSlice" && knifeController.state != KnifeState.STATIONARY)
                    {
                        FindObjectOfType<Slicer>().Slice(hit.transform.gameObject);
                        Destroy(hit.transform.gameObject);
                        knifeController.StartSlicing();
                        return;
                    }
                    else
                    {
                        if(hit.transform.gameObject.tag == "Ground")
                            knifeController.SetState(KnifeState.STATIONARY);
                        // reaload current level
                        if (hit.transform.gameObject.name == "Finish")
                        {
                            FindObjectOfType<ColorLerpToTransperent>().targetColor = Color.black;

                            StartCoroutine(EndOflevel());
                        }
                    }
                }
            }
        }
    }

    IEnumerator EndOflevel()
    {
        yield return new WaitForSeconds(1);
        Application.LoadLevel(0);
    }


    private Transform ChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }
        }
        return null;
    }
}
