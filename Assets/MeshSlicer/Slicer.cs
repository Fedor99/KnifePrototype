using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slicer : MonoBehaviour
{
   // public MeshFilter debug_meshToSlice;
    public GameObject plane;
    public GameObject planeParent;

    public GameObject emptyMeshFilterPrefab;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public Vector3 rigthForce;
    public Vector3 leftForce;

    private void Start()
    {
        //Slice(debug_meshToSlice.gameObject);
    }

    public void Slice(GameObject objectToSlice)
    {
        objectToSlice.GetComponent<Collider>().isTrigger = true;
        var meshToSlice = objectToSlice.GetComponent<MeshFilter>();
        //planeParent.transform.position = meshToSlice.transform.position;
        //planeParent.SetActive(true);

        // get initial values
        initialPosition = meshToSlice.transform.position;
        initialRotation = meshToSlice.transform.rotation;

        MakeInvisible(meshToSlice);

        //// move to the beginning of world coordinates
        //meshToSlice.transform.position = new Vector3(0, 0, 0);
        //meshToSlice.transform.rotation = Quaternion.identity;
        //// change plane rotation with respect to the initial mesh obejct rotation
        //Vector3 inversedRot = initialRotation.eulerAngles * 1;
        //planeParent.transform.rotation = Quaternion.Euler(inversedRot.x, inversedRot.y, inversedRot.z);


        SlicePack leftSlice = new SlicePack(Instantiate(emptyMeshFilterPrefab));
        SlicePack rightSlice = new SlicePack(Instantiate(emptyMeshFilterPrefab));

        var trigs = meshToSlice.mesh.triangles;
        var verts = meshToSlice.mesh.vertices;
        for (int i = 0; i < trigs.Length; i+=3)
        {
            int[] t = new int[3] { trigs[i], trigs[i+1], trigs[i+2] };

            Transform meshTransform = meshToSlice.transform;

            // position of vertices in world coordinates
            Vector3[] _v = new Vector3[3];
            for (int vi = 0; vi < 3; vi++)
                _v[vi] = meshTransform.TransformPoint(verts[t[vi]]);

            // position of vertices in local coordinates of a plane
            Vector3[] v = new Vector3[3];
            for (int vi = 0; vi < 3; vi++)
                //v[vi] = plane.transform.InverseTransformPoint(verts[t[vi]]);// shoud be _v[vi]
                v[vi] = plane.transform.InverseTransformPoint(_v[vi]);

            if (AllVerticesOnTheLeft(v))
                leftSlice.AddTriangle(_v);
            else if (AllVerticesOnTheRight(v))
                rightSlice.AddTriangle(_v);
            else
            {
                GenerateSlice(leftSlice, 1, _v, v);
                GenerateSlice(rightSlice, -1, _v, v);
            }
        }

        Destroy(objectToSlice);

        rightSlice.PrepeareMesh(initialPosition, initialRotation, rigthForce);
        leftSlice.PrepeareMesh(initialPosition, initialRotation, leftForce);
    }


    private void GenerateSlice(SlicePack slice, int side, Vector3[] _v, Vector3[] v)
    {
        // local coordinates of a mesh
        //for (int i = 0; i < 3; i++)
        //    _v[i] = meshToSlice.transform.InverseTransformPoint(_v[i]);

        // left
        {
            var res = OnlyTwoOnOneSide(side, v);
            // if only two vertices on this side of the plane
            if (res != null)
            {
                // in world coordinates
                Vector3 p1 = _v[res[0]];
                Vector3 p2 = _v[res[1]];
                Vector3 p3 = _v[res[2]];
                Vector3? h1 = GetH(p1, p3);
                Vector3? h2 = GetH(p2, p3);
                if (h1 != null && h2 != null)
                {
                    slice.AddTriangle(new Vector3[] { p2, (Vector3)h1, (Vector3)h2 });
                    slice.AddTriangle(new Vector3[] { p1, p2, (Vector3)h1 });
                }
            }
        }
        {
            // with one one vertex on this side
            var res = OnlyOneOnOneSide(side, v);
            // if one only one vertex on this side of the plane
            if (res != null)
            {
                // in world coordinates
                Vector3 p3 = _v[res[0]];
                Vector3 p2 = _v[res[1]];
                Vector3 p1 = _v[res[2]];
                Vector3? h3_1 = GetH(p3, p1);
                Vector3? h3_2 = GetH(p3, p2);
                if (h3_1 != null && h3_2 != null)
                {
                    slice.AddTriangle(new Vector3[] { p3, (Vector3)h3_1, (Vector3)h3_2 });
                }
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param array of three vertex positions in local coordinates of a slice plane="v"></param>
    /// <returns>return true of all the vertices are on one side of the plane</returns>
    private bool AllVerticesOnTheRight(Vector3[] v)
    {
        if (v[0].y > 0 && v[1].y > 0 && v[2].y > 0) return true;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param array of three vertex positions in local coordinates of a slice plane="v"></param>
    /// <returns>return true of all the vertices are on one side of the plane</returns>
    private bool AllVerticesOnTheLeft(Vector3[] v)
    {
        if (v[0].y < 0 && v[1].y < 0 && v[2].y < 0) return true;
        return false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param vertex postion in world coordinates="pStart"></param>
    /// <param vertex postion in world coordinates="pEnd"></param>
    /// <returns>returns hit point (if any)</returns>
    private Vector3? GetH(Vector3 pStart, Vector3 pEnd)
    {
        RaycastHit hit;
        Ray ray = new Ray();
        ray.origin = pStart;
        ray.direction = pEnd - pStart;
        Vector3? h1= null;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, layerMask: LayerMask.GetMask("SlicePlane")))
        {
            h1 = hit.point;
        }
        return h1;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param value of one for left side and negative one for right side="side"></param>
    /// <param vertices of a triangle="v"></param>
    /// <returns>returns indexes of three vertices, two on this side of a plane, null if no two such vertices were found</returns>
    private List<int> OnlyTwoOnOneSide(int side, Vector3[] v)
    {
        return OnlyNOnOneSide(side, v, 2);
    }

    private List<int> OnlyOneOnOneSide(int side, Vector3[] v)
    {
        return OnlyNOnOneSide(side, v, 1);
    }

    private List<int> OnlyNOnOneSide(int side, Vector3[] v, int N)
    {
        List<int> vertexIndexesInATrianglesThatAreOnThisSide = new List<int>();
        List<int> indexes = new List<int> { 0, 1, 2 };
        for (int i = 0; i < v.Length; i++)
        {
            // check side
            if (v[i].y * side < 0) // check if left if side == 1, right if -1
            {
                vertexIndexesInATrianglesThatAreOnThisSide.Add(i);
                indexes.Remove(i);
            }
        }
        if (vertexIndexesInATrianglesThatAreOnThisSide.Count == N)
        {
            // add third element, which is p3
            vertexIndexesInATrianglesThatAreOnThisSide.AddRange(indexes);
            return vertexIndexesInATrianglesThatAreOnThisSide;
        }
        return null;
    }

    public void MakeInvisible(MeshFilter meshObject)
    {
        GameObject o = meshObject.gameObject;
        MeshRenderer mr = o.GetComponent<MeshRenderer>();
        mr.enabled = false;
        o.GetComponent<Collider>().isTrigger = true;
    }
}


public class SlicePack
{
    public SlicePack(GameObject instantiated_obemptyMeshFilterPrefab)
    {
        o = instantiated_obemptyMeshFilterPrefab;
        newTrigs = new List<int>();
        newVerts = new List<Vector3>();
    }

    public GameObject o;
    public List<int> newTrigs;
    public List<Vector3> newVerts;

    public void PrepeareMesh(Vector3 pos, Quaternion rot, Vector3 addForce)
    {
        o.GetComponent<MeshFilter>().mesh = new Mesh();
        o.GetComponent<MeshFilter>().mesh.vertices = newVerts.ToArray();
        o.GetComponent<MeshFilter>().mesh.triangles = newTrigs.ToArray();
        o.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        o.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        o.AddComponent<MeshCollider>().convex = true;

        //o.transform.position = pos;
        //o.transform.rotation = rot;

        o.AddComponent<Rigidbody>().AddForce(addForce, ForceMode.Impulse);
        o.GetComponent<RbForce>().AddForce(addForce);
    }

    public void AddTriangle(Vector3[] _v)
    {
        newTrigs.Add(newVerts.Count);
        newTrigs.Add(newVerts.Count + 1);
        newTrigs.Add(newVerts.Count + 2);

        newTrigs.Add(newVerts.Count + 5);
        newTrigs.Add(newVerts.Count + 4);
        newTrigs.Add(newVerts.Count + 3);

        newVerts.Add(_v[0]);
        newVerts.Add(_v[1]);
        newVerts.Add(_v[2]);

        newVerts.Add(_v[0]);
        newVerts.Add(_v[1]);
        newVerts.Add(_v[2]);
    }
}
