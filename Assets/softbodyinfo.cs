using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using uFlex;

public class softbodyinfo : FlexProcessor {
    GameObject renal;
    GameObject long_knife;
    Transform pd1, pd2, pd3;
    Plane plane;
    Plane stop_plane;   
    Plane curr_stop_plane;
    float slice_thickness = 0.3f;
    float? time;
    int count = 0;
	// Use this for initialization
	void Start () {
        renal = GameObject.Find("RenalSystemColor");
        var fsm = renal.GetComponent<FlexShapeMatching>();
        plane = new Plane(new Vector3(1, 0, 0), renal.transform.position);
        stop_plane = new Plane(new Vector3(0, 0, 1), renal.transform.position + new Vector3(0, 0, 10));

        long_knife = GameObject.Find("Long Knife");
        pd1 = long_knife.transform.Find("CuttingPlane").transform;
        pd2 = long_knife.transform.Find("StopPlane").transform;
        pd3 = long_knife.transform.Find("Blade/Attachable Slicer/PlaneDefinition3").transform;
    }

    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        plane = new Plane(pd1.position, pd2.position, pd3.position);
        stop_plane = new Plane(Vector3.Cross(pd2.position - pd1.position, pd3.position - pd1.position), pd1.position, pd2.position);
        //if (!time.HasValue)
        //    time = Time.time;
        ////if (count % 2 == 0)
        //if ((-curr_stop_plane.distance * curr_stop_plane.normal).z > renal.transform.position.z)
        //{
        //    var secs = Time.time - time.Value;
        //    var origin = -stop_plane.distance * stop_plane.normal;
        //    curr_stop_plane = new Plane(stop_plane.normal, origin - 2 * secs * stop_plane.normal);

        //    CutFlexUtil.CutFlexSoft(renal.transform, plane, curr_stop_plane, slice_thickness);
        //}
    }
    private void draw_decent_plane(Plane p)
    {
        var origin = -p.normal * p.distance;
        Gizmos.DrawCube(origin, new Vector3(1, 1, 1));
        Gizmos.DrawLine(origin, origin + p.normal);
    }
    private void OnDrawGizmos()
    {
        //var fp = renal.GetComponent<FlexParticles>();
        //var fsm = renal.GetComponent<FlexShapeMatching>();
        //int shapeStart = 0;
        //for (var i = 0; i < fsm.m_shapesCount; ++i)
        //{
        //    int shapeEnd = fsm.m_shapeOffsets[i];
        //    var actual_ctr = CuttingUtil.shape_to_world(i, fsm);
        //    bool is_split = false;
        //    var ul = new Vector3?();
        //    var lr = new Vector3 ? ();
        //    for (var j = shapeStart; j < shapeEnd; ++j)
        //    {
        //        var idx = fsm.m_shapeIndices[j];
        //        var pos = fp.m_particles[idx].pos;
        //        ul = ul.HasValue ? Vector3.Min(ul.Value, pos) : pos;
        //        lr = lr.HasValue ? Vector3.Max(lr.Value, pos) : pos;
        //    }
        //    Gizmos.color = colors[i];

        //    var shape_center = (ul.Value + lr.Value) / 2.0f;
        //    Gizmos.DrawCube(shape_center, lr.Value - ul.Value);

        //    shapeStart = shapeEnd;
        //}
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(pd1.position, pd2.position);
        //Gizmos.DrawLine(pd2.position, pd3.position);
        //Gizmos.DrawLine(pd3.position, pd1.position);
        Gizmos.color = Color.red;
        draw_decent_plane(plane);
        Gizmos.color = Color.green;
        draw_decent_plane(stop_plane);
        //Gizmos.color = Color.red;
        //Gizmos.DrawCube(-curr_stop_plane.normal * curr_stop_plane.distance, new Vector3(0.1f, 20.0f, 0.1f));
        //Gizmos.color = Color.green;
        //Gizmos.DrawCube(-curr_stop_plane.normal * curr_stop_plane.distance + slice_thickness * curr_stop_plane.normal, new Vector3(0.1f, 20.0f, 0.1f));

        //for (int i = 0; i < fp.m_particlesCount; ++i)
        //{
        //    if (colors.Count > 0)
        //        Gizmos.color = colors[i];
        //    Gizmos.DrawCube(fp.m_particles[i].pos, new Vector3(0.2f, 0.2f, 0.2f));
        //}
        //var skin = renal.GetComponent<SkinnedMeshRenderer>();
        //var mesh = new Mesh();
        //skin.BakeMesh(mesh);
        ////var mesh = skin.sharedMesh;
        //for (int i = 0; i < mesh.vertexCount; ++i)
        //{
        //    var pos = renal.transform.rotation * mesh.vertices[i] + renal.transform.position;
        //    if (colors.Count != 0)
        //        Gizmos.color = colors[i];
        //    Gizmos.DrawCube(pos, new Vector3(0.2f, 0.2f, 0.2f));
        //}
        //Gizmos.color = Color.green;
        //Gizmos.DrawCube(pd1.position, new Vector3(1.2f, 1.2f, 1.2f));
        //Gizmos.color = Color.red;
        //Gizmos.DrawCube(pd2.position, new Vector3(1.2f, 1.2f, 1.2f));
        //Gizmos.color = Color.blue;
        //Gizmos.DrawCube(pd3.position, new Vector3(1.2f, 1.2f, 1.2f));
    }
}
