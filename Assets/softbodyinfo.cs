using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using uFlex;

public class softbodyinfo : FlexProcessor {
    GameObject uterer, flex;
    bool has_cut = false;
	// Use this for initialization
	void Start () {
        //var shapes = GetComponent<FlexShapeMatching>();
        //var particles = GetComponent<FlexParticles>();
        //Debug.LogFormat("# shapes: {0}", shapes.m_shapesCount);
        //var mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        //Debug.LogFormat("# mesh verts: {0}", mesh.vertexCount);
        uterer = GameObject.Find("Cube");
        flex = GameObject.Find("Flex");
    }

    // Update is called once per frame
    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        var colliders = flex.GetComponent<FlexColliders>();
		if (!has_cut)
        {
            has_cut = true;
            var fp = uterer.GetComponent<FlexParticles>();
            Debug.Log("Uterer pos: " + uterer.transform.position);
            var p = new Plane(new Vector3(1, 0, 0), uterer.transform.position);
            CutFlexUtil.CutFlexSoft(uterer.transform, p);
        }
	}
    private void OnDrawGizmos()
    {
        var fp = uterer.GetComponent<FlexParticles>();
        Gizmos.DrawCube(uterer.transform.position, new Vector3(0.1f, 20, 20));

    }
}
