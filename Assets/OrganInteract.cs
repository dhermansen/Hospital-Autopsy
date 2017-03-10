using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;

public class OrganInteract : MonoBehaviour {

    //[Header("Something")]
    //[Tooltip("Set to true if you want this to work.")]
    //public bool works = false;
    private List<GameObject> children;
	// Use this for initialization
	void Start () {
        //GetComponent<HingeJoint>().connectedBody = transform.parent.GetComponent<Rigidbody>();
        children = (from t in GetComponentsInChildren<Transform>().ToList() select t.gameObject).ToList();
        foreach (var t in children)
        {
            var io = t.AddComponent<VRTK_InteractableObject>();
            io.isGrabbable = true;
            io.InteractableObjectGrabbed += new InteractableObjectEventHandler(onGrab);
            io.InteractableObjectUsed += new InteractableObjectEventHandler(onUse);
            io.InteractableObjectUngrabbed+= new InteractableObjectEventHandler(onUngrab);
            io.InteractableObjectUnused += new InteractableObjectEventHandler(onUnuse);
            io.isUsable = true;
            var fj = t.AddComponent<VRTK_FixedJointGrabAttach>();
            fj.precisionGrab = true;
            fj.breakForce = float.PositiveInfinity;
        }

    }
    void onUse(object sender, InteractableObjectEventArgs args)
    {
        var used = sender as VRTK_InteractableObject;
        //Debug.Log("use " + used.name);

    }
    void onUnuse(object sender, InteractableObjectEventArgs args)
    {
        var used = sender as VRTK_InteractableObject;
    }
    void onGrab(object sender, InteractableObjectEventArgs args)
    {
        //Debug.Log(args.interactingObject.name);
        var grabbed = sender as VRTK_InteractableObject;
        //Debug.Log("grab " + grabbed.name);
    }
    void onUngrab(object sender, InteractableObjectEventArgs args)
    {
        var grabbed = sender as VRTK_InteractableObject;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
