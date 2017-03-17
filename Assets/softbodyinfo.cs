using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using uFlex;

public class softbodyinfo : FlexProcessor {
    GameObject renal;
    Plane plane;
    bool has_cut = false;
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
            var fp = renal.GetComponent<FlexParticles>();
            //CuttingUtil.CutFlexSoft(renal.transform, plane);
        }

    }

    private void OnDrawGizmos()
    {
        return;
        var fp = renal.GetComponent<FlexParticles>();
        var fsm = renal.GetComponent<FlexShapeMatching>();
        int shapeStart = 0;
        for (var i = 0; i < fsm.m_shapesCount; ++i)
        {
            int shapeEnd = fsm.m_shapeOffsets[i];
            var actual_ctr = CuttingUtil.shape_to_world(i, fsm);
            bool is_split = false;
            for (var j = shapeStart; j < shapeEnd; ++j)
            {
                var idx = fsm.m_shapeIndices[j];
                if (!plane.SameSide(actual_ctr, fp.m_particles[idx].pos))
                {
                    //Gizmos.color = Color.green;
                    //Gizmos.DrawCube(fp.m_particles[idx].pos, new Vector3(0.5f, 0.5f, 0.5f));
                    is_split = true;
                }
            }
            if (is_split)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(actual_ctr, new Vector3(0.5f, 0.5f, 0.5f));
            }
            shapeStart = shapeEnd;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawCube(renal.transform.position, new Vector3(0.1f, 20.0f, 20.0f));
    }
}
