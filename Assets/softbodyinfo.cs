using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using uFlex;

public class slice_job
{
    public NobleMuffins.TurboSlicer.Guts.MeshSnapshot snapshot;
    public List<Vector3> pts;
    public List<Vector3> quad_pts;
    public Matrix4x4 transform;
    public bool is_done = false;
}
public static class ft
{
    private static bool intersects(Vector3 tri1, Vector3 tri2, Vector3 tri3, Vector3 p1, Vector3 p2)
    {
        var d = p2 - p1;
        // Vectors from p1 to p2/p3 (edges)
        var e1 = tri2 - tri1;
        var e2 = tri3 - tri1;
        var s1 = Vector3.Cross(d, e2);
        var divisor = Vector3.Dot(s1, e1);
        //if determinant is near zero, ray lies in plane of triangle otherwise not
        if (Mathf.Abs(divisor) < 1e-6)
            return false;
        var inv_divisor = 1.0f / divisor;

        //calculate distance from p1 to ray origin
        var d1 = p1 - tri1;
        var b1 = Vector3.Dot(d1, s1) * inv_divisor;
        if (b1 < 0.0f || b1 > 1.0f)
            return false;
        var s2 = Vector3.Cross(d1, e1);
        var b2 = Vector3.Dot(d, s2) * inv_divisor;
        if (b2 < 0.0f || b1 + b2 > 1.0f)
            return false;

        var t = Vector3.Dot(e2, s2) * inv_divisor;
        return t >= 0.0f && t <= 1.0f;
    }
    private static bool intersects_quad(List<Vector3> quad_pts, Vector3 p1, Vector3 p2)
    {
        return intersects(quad_pts[0], quad_pts[1], quad_pts[2], p1, p2) ||
               intersects(quad_pts[0], quad_pts[2], quad_pts[3], p1, p2);
    }
    public static void find_triangles(slice_job sj)
    {
        sj.pts = new List<Vector3>();
        for (int m = 0; m < sj.snapshot.indices.Count(); ++m)
        {
            for (int i = 0; i < sj.snapshot.indices[m].Count(); i += 3)
            {
                var p1 = sj.transform.MultiplyPoint3x4(sj.snapshot.vertices[sj.snapshot.indices[m][i + 0]]);
                var p2 = sj.transform.MultiplyPoint3x4(sj.snapshot.vertices[sj.snapshot.indices[m][i + 1]]);
                var p3 = sj.transform.MultiplyPoint3x4(sj.snapshot.vertices[sj.snapshot.indices[m][i + 2]]);
                if (intersects_quad(sj.quad_pts, p1, p2) ||
                    intersects_quad(sj.quad_pts, p2, p3) ||
                    intersects_quad(sj.quad_pts, p3, p1))
                {
                    sj.pts.Add(p1);
                    sj.pts.Add(p2);
                    sj.pts.Add(p3);
                }
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
    bool has_added = false;
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
        CutFlexUtil.CutFlexSoft(renal.transform, pd1.position, pd3.position, pd2.position, blade_collider);

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
            sj.transform = renal.transform.localToWorldMatrix;
            var ul = box.bounds.center + new Vector3(0, -box.bounds.extents.y / 2, -box.bounds.extents.z / 2);
            var ur = box.bounds.center + new Vector3(0, +box.bounds.extents.y / 2, -box.bounds.extents.z / 2);
            var lr = box.bounds.center + new Vector3(0, +box.bounds.extents.y / 2, +box.bounds.extents.z / 2);
            var ll = box.bounds.center + new Vector3(0, -box.bounds.extents.y / 2, +box.bounds.extents.z / 2);
            sj.quad_pts = new List<Vector3>{ ul, ur, lr, ll };
            System.Threading.ThreadPool.QueueUserWorkItem((state_info) => ft.find_triangles(sj));
            //ft.find_triangles(sj);
        }
    }
    public void Update()
    {
        if (sj.is_done && !has_added)
        {
            has_added = true;
            pts = sj.pts;
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
        Gizmos.color = Color.red;
        //Gizmos.DrawCube(box.bounds.center + new Vector3(0, box.bounds.extents.y, 0
    }
}
