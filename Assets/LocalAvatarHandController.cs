using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class LocalAvatarHandController : MonoBehaviour {
    Dictionary<string, Transform> hands = new Dictionary<string, Transform>();
    Dictionary<string, string> target_to_hand = new Dictionary<string, string>();
    OvrAvatar ovr;
    VRTK_InteractGrab left_grab, right_grab;
	// Use this for initialization
	void Start () {
        target_to_hand["Long Knife"] = "_Knife";
        target_to_hand["Scalpel"] = "_Scalpel";
        target_to_hand["Pickups"] = "_PickupsOpen";
        target_to_hand["Scissors"] = "_ScissorsOpen";
        target_to_hand["Probe"] = "_Probe";
        foreach (var hand in Resources.LoadAll<Transform>("Hands"))
            hands[hand.name] = hand;
        ovr = this.GetComponentInChildren<OvrAvatar>();
        left_grab = GameObject.Find("LeftController").GetComponent<VRTK_InteractGrab>();
        right_grab = GameObject.Find("RightController").GetComponent<VRTK_InteractGrab>();
        left_grab.ControllerGrabInteractableObject += new ObjectInteractEventHandler(onGrab);
        left_grab.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(onUngrab);
        right_grab.ControllerGrabInteractableObject += new ObjectInteractEventHandler(onGrab);
        right_grab.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(onUngrab);

    }
    private void onGrab(object sender, ObjectInteractEventArgs e)
    {
        if (!target_to_hand.ContainsKey(e.target.name))
            return;
        Debug.Log(e.target.name);
        if (e.controllerIndex == 0)
            ovr.LeftHandCustomPose = hands["HandLeft" + target_to_hand[e.target.name]];
        else
            ovr.RightHandCustomPose = hands["HandRight" + target_to_hand[e.target.name]];
    }
    private void onUngrab(object sender, ObjectInteractEventArgs e)
    {
        if (e.controllerIndex == 0)
            ovr.LeftHandCustomPose = null;
        else if (e.controllerIndex == 1)
            ovr.RightHandCustomPose = null;
    }

    // Update is called once per frame
    void Update () {
        //var lgo = left_grab.GetGrabbedObject();
        //if (lgo)
        //    Debug.Log(lgo.ToString());

        //var rgo = right_grab.GetGrabbedObject();
        //if (rgo)
        //    Debug.Log(rgo.ToString());
        //var lk = GameObject.Find("Long Knife").GetComponent<VRTK_InteractableObject>();
        //if (lk.IsGrabbed())
        //{
        //    Debug.Log("Something grabbed: " + lk.GetGrabbingObject().name);
        //}
    }
}
