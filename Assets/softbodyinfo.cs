using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using uFlex;

public class softbodyinfo : FlexProcessor {
    GameObject renal;
    Transform pd1, pd2, pd3;
    Collider blade_collider;
    List<Vector3> pts = new List<Vector3>();
	// Use this for initialization
	void Start () {
        renal = GameObject.Find("RenalSystemColor");

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
        var new_pts = CutFlexUtil.CutFlexSoft(renal.transform, pd1.position, pd3.position, pd2.position, blade_collider);
        if (new_pts != null)
            pts.AddRange(new_pts);
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
        //Gizmos.DrawLine();
        //Gizmos.color = Color.green;
        //Gizmos.DrawCube(pd1.position, new Vector3(1.2f, 1.2f, 1.2f));
        //Gizmos.color = Color.red;
        //Gizmos.DrawCube(pd2.position, new Vector3(1.2f, 1.2f, 1.2f));
        //Gizmos.color = Color.blue;
        //Gizmos.DrawCube(pd3.position, new Vector3(1.2f, 1.2f, 1.2f));
    }
}
