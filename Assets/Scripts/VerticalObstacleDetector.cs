using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalObstacleDetector : MonoBehaviour
{
    public float maxHitdetectionDistance = 2f;
    public int numberOfHits = 0;

    public bool IsNearVerticalObstacle(Vector3 vectorSide)
    {
        int n = 0;
        int _n = 3;
        for (float y = 0; y < _n; y++)
        {
            Ray r = new Ray();
            r.origin = transform.position + new Vector3(0, y/1f, 0);
            r.direction = vectorSide;
            Debug.DrawRay(r.origin, r.direction * 100, Color.red);

            RaycastHit hit;
            if (Physics.Raycast(r, out hit, Mathf.Infinity,
                layerMask: LayerMask.GetMask("Ground")))
            {
                if (hit.distance < maxHitdetectionDistance)
                {
                    n++;
                }
            }
        }
        numberOfHits = n;

        return n == _n;
    }
}
