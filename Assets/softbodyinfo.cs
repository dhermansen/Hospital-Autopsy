using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using uFlex;

public class slice_job
{
    public List<int> affected_vertices;
    public Vector3[] cutting_quad;
    public Vector3[] vertices;
    public int[][] indices;
    public int[] triangles;
    public Matrix4x4 transform;
    public bool is_done = false;
    public bool is_processing = false;
}
public struct weight_thing
{
    public weight_thing(int bi, float w) : this()
    {
        bone_idx = bi;
        weight = w;
    }
    public int bone_idx;
    public float weight;
}
public static class ft
{
    public static void find_triangles(slice_job sj)
    {
        var new_indices = new int[sj.indices.Length][];
        sj.affected_vertices = new List<int>();
        for (int m = 0; m < sj.indices.Count(); ++m)
        {
            new_indices[m] = new int[sj.indices[m].Count()];
            for (int i = 0; i < sj.indices[m].Count(); i += 3)
            {
                var p1 = sj.transform.MultiplyPoint3x4(sj.vertices[sj.indices[m][i + 0]]);
                var p2 = sj.transform.MultiplyPoint3x4(sj.vertices[sj.indices[m][i + 1]]);
                var p3 = sj.transform.MultiplyPoint3x4(sj.vertices[sj.indices[m][i + 2]]);
                if (CutFlexUtil.intersects_quad(p1, p2, sj.cutting_quad[0], sj.cutting_quad[1], sj.cutting_quad[2], sj.cutting_quad[3]) ||
                    CutFlexUtil.intersects_quad(p2, p3, sj.cutting_quad[0], sj.cutting_quad[1], sj.cutting_quad[2], sj.cutting_quad[3]) ||
                    CutFlexUtil.intersects_quad(p3, p1, sj.cutting_quad[0], sj.cutting_quad[1], sj.cutting_quad[2], sj.cutting_quad[3]))
                {
                    sj.affected_vertices.Add(sj.indices[m][i + 0]);
                    sj.affected_vertices.Add(sj.indices[m][i + 1]);
                    sj.affected_vertices.Add(sj.indices[m][i + 2]);
                }
                else
                {
                    new_indices[m][i + 0] = sj.indices[m][i + 0];
                    new_indices[m][i + 1] = sj.indices[m][i + 1];
                    new_indices[m][i + 2] = sj.indices[m][i + 2];
                }
            }
        }
        sj.indices = new_indices;
        var new_triangles = new List<int>();
        for (int i = 0; i < sj.triangles.Length; i += 3)
        {
            var p1 = sj.transform.MultiplyPoint3x4(sj.vertices[sj.triangles[i + 0]]);
            var p2 = sj.transform.MultiplyPoint3x4(sj.vertices[sj.triangles[i + 1]]);
            var p3 = sj.transform.MultiplyPoint3x4(sj.vertices[sj.triangles[i + 2]]);
            if (!CutFlexUtil.intersects_quad(p1, p2, sj.cutting_quad[0], sj.cutting_quad[1], sj.cutting_quad[2], sj.cutting_quad[3]) &&
                !CutFlexUtil.intersects_quad(p2, p3, sj.cutting_quad[0], sj.cutting_quad[1], sj.cutting_quad[2], sj.cutting_quad[3]) &&
                !CutFlexUtil.intersects_quad(p3, p1, sj.cutting_quad[0], sj.cutting_quad[1], sj.cutting_quad[2], sj.cutting_quad[3]))
            {
                new_triangles.Add(sj.triangles[i + 0]);
                new_triangles.Add(sj.triangles[i + 1]);
                new_triangles.Add(sj.triangles[i + 2]);
            }
        }
        sj.triangles = new_triangles.ToArray();
        sj.is_done = true;
    }
}
public class softbodyinfo : FlexProcessor {
    GameObject renal;//, cutting_cube;
    Transform pd1, pd2, pd3, pd4;
    slice_job sj = new slice_job();

	// Use this for initialization
	void Start () {
        renal = GameObject.Find("RenalSystemColor");

        var long_knife = GameObject.Find("Long Knife");
        var blade = long_knife.transform.Find("Blade");
        pd1 = blade.Find("Attachable Slicer/PlaneDefinition1");
        pd2 = blade.Find("Attachable Slicer/PlaneDefinition2");
        pd3 = blade.Find("Attachable Slicer/PlaneDefinition3");
        pd4 = blade.Find("Attachable Slicer/PlaneDefinition4");
        //Debug.LogFormat("Number of indices in triangle list {0}", renal.GetComponent<SkinnedMeshRenderer>().sharedMesh.triangles.Count());
        var smr = renal.GetComponent<SkinnedMeshRenderer>();
        smr.sharedMesh = Instantiate(smr.sharedMesh) as Mesh;
    }

    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        if (!sj.is_processing && CutFlexUtil.CutFlexSoft(renal.transform, pd1.position, pd2.position, pd3.position, pd4.position))
        {
            var mesh = new Mesh();
            renal.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
            sj.indices = new int[mesh.subMeshCount][];
            for (int i = 0; i < mesh.subMeshCount; i++)
                sj.indices[i] = mesh.GetIndices(i);
            sj.triangles = mesh.triangles;
            sj.vertices = mesh.vertices;
            sj.transform = renal.transform.localToWorldMatrix;
            sj.cutting_quad = new Vector3[4] { pd1.position, pd2.position, pd3.position, pd4.position };
            sj.is_processing = true;
            sj.is_done = false;
            System.Threading.ThreadPool.QueueUserWorkItem((state_info) => ft.find_triangles(sj));
        }
    }
    private string stringize(BoneWeight bw)
    {
        return string.Format("idxs: {0} {1} {2} {3}; weights: {4} {5} {6} {7}",
            bw.boneIndex0, bw.boneIndex1, bw.boneIndex2, bw.boneIndex3,
            bw.weight0, bw.weight1, bw.weight2, bw.weight3);
    }
    public void Update()
    {
        if (sj.is_done)
        {
            var smr = renal.GetComponent<SkinnedMeshRenderer>();
            smr.sharedMesh.subMeshCount = sj.indices.Length;
            for (int j = 0; j < sj.indices.Length; j++)
            {
                int[] array;
                if (sj.indices[j].Length > 0)
                {
                    array = sj.indices[j];
                }
                else
                {
                    array = new int[] { 0, 0, 0 };
                }
                smr.sharedMesh.SetIndices(array, MeshTopology.Triangles, j);
            }
            smr.sharedMesh.triangles = sj.triangles;
            var shapes = renal.GetComponent<FlexShapeMatching>();
            var plane = new Plane(sj.cutting_quad[0], sj.cutting_quad[1], sj.cutting_quad[2]);
            var new_bone_weights = smr.sharedMesh.boneWeights;
            foreach (var vidx in sj.affected_vertices)
            {
                var v = sj.transform.MultiplyPoint3x4(sj.vertices[vidx]);
                var bw = smr.sharedMesh.boneWeights[vidx];
                var bIdxs = new List<weight_thing> { new weight_thing(bw.boneIndex0, bw.weight0),
                                                     new weight_thing(bw.boneIndex1, bw.weight1),
                                                     new weight_thing(bw.boneIndex2, bw.weight2),
                                                     new weight_thing(bw.boneIndex3, bw.weight3) };
                bIdxs = bIdxs.Select(bidx => plane.SameSide(v, CutFlexUtil.shape_to_world(bidx.bone_idx, shapes)) ?
                    bidx : new weight_thing(bidx.bone_idx, 0.0f)).ToList();
                bIdxs.Sort((lhs, rhs) => lhs.weight > rhs.weight ? -1 : 1);

                var weight_sum = bIdxs.Sum(bidx => bidx.weight);// bw.weight0 + bw.weight1 + bw.weight2 + bw.weight3;
                bw.boneIndex0 = bIdxs[0].bone_idx;
                bw.weight0 = bIdxs[0].weight / weight_sum;
                bw.boneIndex1 = bIdxs[1].bone_idx;
                bw.weight1 = bIdxs[1].weight / weight_sum;
                bw.boneIndex2 = bIdxs[2].bone_idx;
                bw.weight2 = bIdxs[2].weight / weight_sum;
                bw.boneIndex3 = bIdxs[3].bone_idx;
                bw.weight3 = bIdxs[3].weight / weight_sum;

                new_bone_weights[vidx] = bw;
            }
            smr.sharedMesh.boneWeights = new_bone_weights;
            sj.is_processing = false;
        }
    }
    private void OnDrawGizmos()
    {
        var size = new Vector3(0.2f, 0.2f, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(pd1.position, size);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(pd2.position, size);
        Gizmos.color = Color.grey;
        Gizmos.DrawCube(pd3.position, size);
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(pd4.position, size);

        //if (sj.vertices != null && sj.vertices.Length > 7468)
        //{
        //    var smr = renal.GetComponent<SkinnedMeshRenderer>();
        //    var shapes = renal.GetComponent<FlexShapeMatching>();
        //    var p0 = CutFlexUtil.shape_to_world(smr.sharedMesh.boneWeights[7468].boneIndex0, shapes);
        //    var p1 = CutFlexUtil.shape_to_world(smr.sharedMesh.boneWeights[7468].boneIndex1, shapes);
        //    var p2 = CutFlexUtil.shape_to_world(smr.sharedMesh.boneWeights[7468].boneIndex2, shapes);
        //    var p3 = CutFlexUtil.shape_to_world(smr.sharedMesh.boneWeights[7468].boneIndex3, shapes);
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawCube(p0, size);
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawCube(p1, size);
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawCube(p2, size);
        //    Gizmos.color = Color.black;
        //    Gizmos.DrawCube(p3, size);
        //}
    }
}
