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
    public int[][] indices;
    public Matrix4x4 transform;
    public bool is_done = false;
}
public static class ft
{
    public static void find_triangles(slice_job sj)
    {
        sj.indices = new int[sj.snapshot.indices.Count()][];
        sj.pts = new List<Vector3>();
        for (int m = 0; m < sj.snapshot.indices.Count(); ++m)
        {
            sj.indices[m] = new int[sj.snapshot.indices[m].Count()];
            for (int i = 0; i < sj.snapshot.indices[m].Count(); i += 3)
            {
                var p1 = sj.transform.MultiplyPoint3x4(sj.snapshot.vertices[sj.snapshot.indices[m][i + 0]]);
                var p2 = sj.transform.MultiplyPoint3x4(sj.snapshot.vertices[sj.snapshot.indices[m][i + 1]]);
                var p3 = sj.transform.MultiplyPoint3x4(sj.snapshot.vertices[sj.snapshot.indices[m][i + 2]]);
                if (CutFlexUtil.intersects_quad(p1, p2, sj.quad_pts[0], sj.quad_pts[1], sj.quad_pts[2], sj.quad_pts[3]) ||
                    CutFlexUtil.intersects_quad(p2, p3, sj.quad_pts[0], sj.quad_pts[1], sj.quad_pts[2], sj.quad_pts[3]) ||
                    CutFlexUtil.intersects_quad(p3, p1, sj.quad_pts[0], sj.quad_pts[1], sj.quad_pts[2], sj.quad_pts[3]))
                {
                    sj.pts.Add(p1);
                    sj.pts.Add(p2);
                    sj.pts.Add(p3);
                }
                else
                {
                    sj.indices[m][i + 0] = sj.snapshot.indices[m][i + 0];
                    sj.indices[m][i + 1] = sj.snapshot.indices[m][i + 1];
                    sj.indices[m][i + 2] = sj.snapshot.indices[m][i + 2];
                }
            }
        }
        sj.is_done = true;
    }
}
public class softbodyinfo : FlexProcessor {
    GameObject renal, cutting_cube;
    Transform pd1, pd2, pd3, pd4;
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
        cutting_cube.transform.position = renal.transform.position + new Vector3(3.0f, -4.5f, -3.5f);
        cutting_cube.transform.localScale = new Vector3(0.2f, 1.1f, 4.0f);
        cutting_cube.name = "cuttingcube";

        var long_knife = GameObject.Find("Long Knife");
        var blade = long_knife.transform.Find("Blade");
        blade_collider = blade.GetComponent<MeshCollider>();
        pd1 = blade.Find("Attachable Slicer/PlaneDefinition1");
        pd2 = blade.Find("Attachable Slicer/PlaneDefinition2");
        pd3 = blade.Find("Attachable Slicer/PlaneDefinition3");
        pd4 = blade.Find("Attachable Slicer/PlaneDefinition4");
        //Debug.LogFormat("Number of indices in triangle list {0}", renal.GetComponent<SkinnedMeshRenderer>().sharedMesh.triangles.Count());
    }

    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        if (!time.HasValue)
            time = Time.time;
        //CutFlexUtil.CutFlexSoft(renal.transform, pd1.position, pd3.position, pd2.position, blade_collider);

        if (!has_cut && Time.time > 2 + time.Value)
        {
            has_cut = true;
            Debug.Log("cutting");

            var box = cutting_cube.GetComponent<Collider>();
            var ul = box.bounds.center + new Vector3(0, -box.bounds.extents.y / 2, -box.bounds.extents.z / 2);
            var ur = box.bounds.center + new Vector3(0, +box.bounds.extents.y / 2, -box.bounds.extents.z / 2);
            var lr = box.bounds.center + new Vector3(0, +box.bounds.extents.y / 2, +box.bounds.extents.z / 2);
            var ll = box.bounds.center + new Vector3(0, -box.bounds.extents.y / 2, +box.bounds.extents.z / 2);
            CutFlexUtil.CutFlexSoft(renal.transform, ul, ur, lr, ll, box);
            /*
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
            sj.quad_pts = new List<Vector3>{ ul, ur, lr, ll };
            System.Threading.ThreadPool.QueueUserWorkItem((state_info) => ft.find_triangles(sj));
            //ft.find_triangles(sj);
            */
        }
    }
    public void Update()
    {
        if (sj.is_done && !has_added)
        {
            has_added = true;
            pts = sj.pts;
            var smr = renal.GetComponent<SkinnedMeshRenderer>();
            smr.sharedMesh.subMeshCount = sj.indices.Length;
            for (int j = 0; j < sj.snapshot.indices.Length; j++)
            {
                int[] array;
                if (sj.snapshot.indices[j].Length > 0)
                {
                    array = sj.indices[j];
                }
                else
                {
                    array = new int[] { 0, 0, 0 };
                }
                smr.sharedMesh.SetIndices(array, MeshTopology.Triangles, j);
            }
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

        var size = new Vector3(0.2f, 0.2f, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(pd1.position, size);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(pd2.position, size);
        Gizmos.color = Color.grey;
        Gizmos.DrawCube(pd3.position, size);
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(pd4.position, size);
    }
}
