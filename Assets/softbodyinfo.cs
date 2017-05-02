using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using uFlex;
using VRTK;

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

public class softbodyinfo : FlexProcessor {
    GameObject renal;
    Transform[] lk_pts = new Transform[4];
    Transform[] sl_pts = new Transform[4];
    Transform[] sc_pts = new Transform[4];
    Transform scissors_closed;
    slice_job sj = new slice_job();
    bool has_cut = false;
    GameObject[] tmp_cut_objs = new GameObject[4];
    Transform[] tmp_cut_pts = new Transform[4];

    bool long_knife_held, scalpel_held, scissors_held;
    // Use this for initialization
    void Start () {
        Vector3[] positions = new Vector3[4];
        positions[0] = new Vector3(1, 0, -22);
        positions[1] = new Vector3(1, 5, -22);
        positions[2] = new Vector3(1, 5, -27);
        positions[3] = new Vector3(1, 0, -27);
        for (int i = 0; i < 4; ++i)
        {
            tmp_cut_objs[i] = new GameObject();
            tmp_cut_objs[i].transform.position = positions[i];
            tmp_cut_pts[i] = tmp_cut_objs[i].transform;
        }

        renal = GameObject.Find("RenalSystemColor");
        var long_knife = GameObject.Find("Long Knife");
        var blade = long_knife.transform.Find("Blade");
        lk_pts[0] = blade.Find("Attachable Slicer/PlaneDefinition1");
        lk_pts[1] = blade.Find("Attachable Slicer/PlaneDefinition2");
        lk_pts[2] = blade.Find("Attachable Slicer/PlaneDefinition3");
        lk_pts[3] = blade.Find("Attachable Slicer/PlaneDefinition4");

        var scalpel = GameObject.Find("Scalpel");
        var large_blade = scalpel.transform.Find("Large_Blade");
        sl_pts[0] = large_blade.Find("Attachable Slicer/PlaneDefinition1");
        sl_pts[1] = large_blade.Find("Attachable Slicer/PlaneDefinition2");
        sl_pts[2] = large_blade.Find("Attachable Slicer/PlaneDefinition3");
        sl_pts[3] = large_blade.Find("Attachable Slicer/PlaneDefinition4");

        var scissors = GameObject.Find("Scissors");
        scissors_closed = scissors.transform.Find("Closed/Left Arm");
        sc_pts[0] = scissors_closed.Find("Attachable Slicer/PlaneDefinition1");
        sc_pts[1] = scissors_closed.Find("Attachable Slicer/PlaneDefinition2");
        sc_pts[2] = scissors_closed.Find("Attachable Slicer/PlaneDefinition3");
        sc_pts[3] = scissors_closed.Find("Attachable Slicer/PlaneDefinition4");
        //Debug.LogFormat("Number of indices in triangle list {0}", renal.GetComponent<SkinnedMeshRenderer>().sharedMesh.triangles.Count());
        var smr = renal.GetComponent<SkinnedMeshRenderer>();
        smr.sharedMesh = Instantiate(smr.sharedMesh) as Mesh;
        smr.sharedMesh.MarkDynamic();

        var left_grab = GameObject.Find("LeftController").GetComponent<VRTK_InteractGrab>();
        var set_held = new ObjectInteractEventHandler((sender, e) =>
        {
            long_knife_held = e.target == long_knife;
            scalpel_held = e.target == scalpel;
            scissors_held = e.target == scissors;
        });
        left_grab.ControllerGrabInteractableObject += set_held;
        left_grab.ControllerUngrabInteractableObject += set_held;
        var right_grab = GameObject.Find("RightController").GetComponent<VRTK_InteractGrab>();
        right_grab.ControllerGrabInteractableObject += set_held;
        right_grab.ControllerUngrabInteractableObject += set_held;
    }
    private void queue_work(Transform[] pts)
    {
        if (!sj.is_processing && CutFlexUtil.CutFlexSoft(renal.transform, pts[0].position, pts[1].position, pts[2].position, pts[3].position))
        {
            var mesh = new Mesh();
            renal.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
            sj.indices = new int[mesh.subMeshCount][];
            for (int i = 0; i < mesh.subMeshCount; i++)
                sj.indices[i] = mesh.GetIndices(i);
            //sj.triangles = mesh.triangles;
            sj.vertices = mesh.vertices;
            sj.transform = renal.transform.localToWorldMatrix;
            sj.cutting_quad = new Vector3[4] { pts[0].position, pts[1].position, pts[2].position, pts[3].position };
            sj.is_processing = true;
            sj.is_done = false;
            //System.Threading.ThreadPool.QueueUserWorkItem((state_info) => slice_mesh.find_triangles(sj));
            slice_mesh.find_triangles(sj);
        }
    }
    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    //public override void PreContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        if (long_knife_held)
            queue_work(lk_pts);
        if (scalpel_held)
            queue_work(sl_pts);
        if (scissors_closed.gameObject.activeInHierarchy && scissors_held)
            queue_work(sc_pts);

        if (Time.time > 3 && Time.time < 6 && !has_cut)
        {
            has_cut = true;
            queue_work(tmp_cut_pts);
        }
        if (Time.time > 6 && has_cut)
        {
            has_cut = false;
            var ovr = GameObject.Find("UI").GetComponent<OverController>();
            ovr.restore_mesh_public();
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
        if (sj.is_done && sj.is_processing)
        {
            var smr = renal.GetComponent<SkinnedMeshRenderer>();

            smr.sharedMesh.MarkDynamic();
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
            //smr.sharedMesh.triangles = sj.triangles;
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

                var weight_sum = bIdxs.Sum(bidx => bidx.weight);
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
    private void viz_blade(Transform[] pts, float scale = 1.0f)
    {
        var size = new Vector3(0.2f, 0.2f, 0.2f) * scale;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(pts[0].position, size);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(pts[1].position, size);
        Gizmos.color = Color.grey;
        Gizmos.DrawCube(pts[2].position, size);
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(pts[3].position, size);
    }
    private void OnDrawGizmos()
    {
        viz_blade(tmp_cut_pts, 1.0f);
        viz_blade(lk_pts);
        viz_blade(sl_pts);
        if (scissors_closed.gameObject.activeInHierarchy)
            viz_blade(sc_pts);
    }
}
