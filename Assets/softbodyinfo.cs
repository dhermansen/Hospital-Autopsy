﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using uFlex;

public class softbodyinfo : FlexProcessor {
    GameObject renal;
    Plane plane;
    bool has_cut = false;
    List<Color> colors = new List<Color>();
	// Use this for initialization
	void Start () {
        renal = GameObject.Find("RenalSystemColor");
        var fsm = renal.GetComponent<FlexShapeMatching>();
        plane = new Plane(new Vector3(1, 0, 0), renal.transform.position);
    }

    // Update is called once per frame
    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {

        if (!has_cut)
        {
            has_cut = true;
            //colors = CuttingUtil.CutFlexSoft(renal.transform, plane);
        }


    }

    private void OnDrawGizmos()
    {
        //return;
        var fp = renal.GetComponent<FlexParticles>();
        var fsm = renal.GetComponent<FlexShapeMatching>();
        int shapeStart = 0;
        for (var i = 0; i < fsm.m_shapesCount; ++i)
        {
            int shapeEnd = fsm.m_shapeOffsets[i];
            var actual_ctr = CuttingUtil.shape_to_world(i, fsm);
            bool is_split = false;
            var ul = new Vector3?();
            var lr = new Vector3 ? ();
            for (var j = shapeStart; j < shapeEnd; ++j)
            {
                var idx = fsm.m_shapeIndices[j];
                var pos = fp.m_particles[idx].pos;
                ul = ul.HasValue ? Vector3.Min(ul.Value, pos) : pos;
                lr = lr.HasValue ? Vector3.Max(lr.Value, pos) : pos;
            }
            Gizmos.color = colors[i];

            var shape_center = (ul.Value + lr.Value) / 2.0f;
            Gizmos.DrawCube(shape_center, lr.Value - ul.Value);
     
            shapeStart = shapeEnd;
        }
        //Gizmos.color = Color.red;
        //Gizmos.DrawCube(renal.transform.position, new Vector3(0.1f, 20.0f, 20.0f));
        //for (int i = 0; i < fp.m_particlesCount; ++i)
        //{
        //    if (colors.Count > 0)
        //        Gizmos.color = colors[i];
        //    Gizmos.DrawCube(fp.m_particles[i].pos, new Vector3(0.2f, 0.2f, 0.2f));
        //}
        //var skin = renal.GetComponent<SkinnedMeshRenderer>();
        //var mesh = new Mesh();
        //skin.BakeMesh(mesh);
        //var mesh = skin.sharedMesh;
        //for (int i = 0; i < mesh.vertexCount; ++i)
        //{
        //    var pos = renal.transform.rotation * mesh.vertices[i] + renal.transform.position;
        //    if (colors.Count != 0)
        //        Gizmos.color = colors[i];
        //    Gizmos.DrawCube(pos, new Vector3(0.2f, 0.2f, 0.2f));
        //}
    }
}
