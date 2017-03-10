using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrganInteract : MonoBehaviour {
    List<GameObject> children;
	// Use this for initialization
	void Start () {
        //GetComponent<HingeJoint>().connectedBody = transform.parent.GetComponent<Rigidbody>();
        children = (from t in GetComponentsInChildren<Transform>().ToList() select t.gameObject).ToList();
        foreach (var t in children)
        {
            var io = t.AddComponent<VRTK.VRTK_InteractableObject>();
            io.isGrabbable = true;
            var fj = t.AddComponent<VRTK.GrabAttachMechanics.VRTK_FixedJointGrabAttach>();
            fj.precisionGrab = true;
            fj.breakForce = float.PositiveInfinity;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
