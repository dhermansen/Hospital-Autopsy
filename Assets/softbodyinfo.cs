using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using uFlex;

public class slice_job
{
    public NobleMuffins.TurboSlicer.Guts.MeshSnapshot snapshot;
    public List<Vector3> pts;
    public Collider collider;
    public bool is_done = false;
}
public static class ft
{
    private static bool intersect(Vector3 p1, Vector3 p2, Vector3 p3, Ray ray)
    {
        // Vectors from p1 to p2/p3 (edges)
        Vector3 e1, e2;

        Vector3 p, q, t;
        float det, invDet, u, v;


        //Find vectors for two edges sharing vertex/point p1
        e1 = p2 - p1;
        e2 = p3 - p1;

        // calculating determinant 
        p = Vector3.Cross(ray.direction, e2);

        //Calculate determinat
        det = Vector3.Dot(e1, p);

        //if determinant is near zero, ray lies in plane of triangle otherwise not
        if (det > -1e-6 && det < 1e-6) { return false; }
        invDet = 1.0f / det;

        //calculate distance from p1 to ray origin
        t = ray.origin - p1;

        //Calculate u parameter
        u = Vector3.Dot(t, p) * invDet;

        //Check for ray hit
        if (u < 0 || u > 1) { return false; }

        //Prepare to test v parameter
        q = Vector3.Cross(t, e1);

        //Calculate v parameter
        v = Vector3.Dot(ray.direction, q) * invDet;

        //Check for ray hit
        if (v < 0 || u + v > 1) { return false; }

        if ((Vector3.Dot(e2, q) * invDet) > 1e-6)
        {
            //ray does intersect
            return true;
        }

        // No hit at all
        return false;
    }
    public static void find_triangles(slice_job sj)
    {
        for (int m = 0; m < sj.snapshot.indices.Count(); ++m)
        {
            for (int i = 0; i < sj.snapshot.indices[m].Count(); i += 3)
            {
                var p1 = sj.snapshot.vertices[sj.snapshot.indices[m][i + 0]];
                var p2 = sj.snapshot.vertices[sj.snapshot.indices[m][i + 1]];
                var p3 = sj.snapshot.vertices[sj.snapshot.indices[m][i + 2]];
                sj.collider.bounds.IntersectRay()
            }
        }
        sj.is_done = true;
    }
}
public class softbodyinfo : FlexProcessor {
    GameObject renal, cutting_cube;
    Transform pd1, pd2, pd3;
    Collider blade_collider;
    List<Vector3> pts = new List<Vector3>();
    float? time;
    bool has_cut = false;
    slice_job sj = new slice_job();

	// Use this for initialization
	void Start () {
        renal = GameObject.Find("RenalSystemColor");

        cutting_cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cutting_cube.transform.position = renal.transform.position + new Vector3(3.0f, -4.0f, -3.5f);
        cutting_cube.transform.localScale = new Vector3(0.2f, 1.0f, 4.0f);
        cutting_cube.name = "cuttingcube";

        var long_knife = GameObject.Find("Long Knife");
        var blade = long_knife.transform.Find("Blade");
        blade_collider = blade.GetComponent<MeshCollider>();
        pd1 = blade.Find("Attachable Slicer/PlaneDefinition1");
        pd2 = blade.Find("Attachable Slicer/PlaneDefinition2");
        pd3 = blade.Find("Attachable Slicer/PlaneDefinition3");
        //Debug.LogFormat("Number of indices in triangle list {0}", renal.GetComponent<SkinnedMeshRenderer>().sharedMesh.triangles.Count());
    }

    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        if (!time.HasValue)
            time = Time.time;
        var new_pts = CutFlexUtil.CutFlexSoft(renal.transform, pd1.position, pd3.position, pd2.position, blade_collider);
        if (new_pts != null)
            pts.AddRange(new_pts);

        if (!has_cut && Time.time > 2 + time.Value)
        {
            has_cut = true;
            Debug.Log("cutting");

            var box = cutting_cube.GetComponent<Collider>();
            CutFlexUtil.CutFlexSoft(renal.transform, box.bounds.center, box.bounds.center + new Vector3(0, box.bounds.extents.y, 0),
                box.bounds.center + new Vector3(0, 0, box.bounds.extents.z), box);
            var mesh = new Mesh();
            renal.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
            var indices = new int[mesh.subMeshCount][];
            for (int i = 0; i < mesh.subMeshCount; i++)
                indices[i] = mesh.GetIndices(i);

            sj.snapshot = new NobleMuffins.TurboSlicer.Guts.MeshSnapshot(mesh.vertices,
                                            new Vector3[0],
                                            //channelNormals ? mesh.normals : new Vector3[0],
                                            mesh.uv,
                                            new Vector2[0],
                                            //channelUV2 ? mesh.uv2 : new Vector2[0],
                                            new Vector4[0],
                                            //channelTangents ? mesh.tangents : new Vector4[0],
                                            indices,
                                            null);
            System.Threading.ThreadPool.QueueUserWorkItem((state_info) => ft.find_triangles(sj));
        }
    }
    public void Update()
    {
        if (sj.is_done)
        {
            Debug.Log("Done!");
        }
    }
    private void OnDrawGizmos()
    {
        //Debug.LogFormat("Number of points: {0}", pts.Count);
        Gizmos.color = Color.green;
        for (int i = 0; i < pts.Count; i += 3)
        {
            Gizmos.DrawLine(pts[i+0], pts[i + 1]);
            Gizmos.DrawLine(pts[i+1], pts[i + 2]);
            Gizmos.DrawLine(pts[i+2], pts[i + 0]);
        }
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireCube(renal.transform.position + new Vector3(3.0f, -4.0f, -3.5f), new Vector3(0.2f, 1.0f, 4.0f));
    }
}
