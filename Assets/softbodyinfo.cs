using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using uFlex;

public class softbodyinfo : MonoBehaviour {
    GameObject ureter;
    bool has_cut = false;
	// Use this for initialization
	void Start () {
        //var shapes = GetComponent<FlexShapeMatching>();
        //var particles = GetComponent<FlexParticles>();
        //Debug.LogFormat("# shapes: {0}", shapes.m_shapesCount);
        //var mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        //Debug.LogFormat("# mesh verts: {0}", mesh.vertexCount);
        ureter = GameObject.Find("left_ureter");
    }

    // Update is called once per frame
    void Update () {
		if (!has_cut)
        {
            has_cut = true;
            var fp = ureter.GetComponent<FlexParticles>();
            var p = new Plane(new Vector3(1, 0, 0), fp.m_bounds.center);
            CutFlexUtil.CutFlexSoft(ureter.transform, p);
        }
	}
    private void OnDrawGizmos()
    {
        var fp = ureter.GetComponent<FlexParticles>();
        Gizmos.DrawCube(ureter.transform.position, new Vector3(1,20,20));

    }
}
