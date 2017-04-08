using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using uFlex;

public class softbodyinfo : FlexProcessor {
    GameObject renal, cutting_cube;
    Transform pd1, pd2, pd3;
    Collider blade_collider;
    List<Vector3> pts = new List<Vector3>();
    float? time;
    bool has_cut = false;
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
