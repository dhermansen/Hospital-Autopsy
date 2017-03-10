﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class LocalAvatarHandController : MonoBehaviour {
    Dictionary<string, Transform> hands = new Dictionary<string, Transform>();
    Dictionary<string, string> target_to_hand = new Dictionary<string, string>();
    OvrAvatar ovr;
    VRTK_InteractGrab left_grab, right_grab;
    VRTK_InteractUse right_use;
    private GameObject pickups;
    private FixedJoint fixed_joint;
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
        right_use = right_grab.GetComponent<VRTK_InteractUse>();
        left_grab.ControllerGrabInteractableObject += new ObjectInteractEventHandler(onGrab);
        left_grab.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(onUngrab);
        right_grab.ControllerGrabInteractableObject += new ObjectInteractEventHandler(onGrab);
        right_grab.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(onUngrab);
        right_use.ControllerUseInteractableObject += new ObjectInteractEventHandler(onUse);
        right_use.ControllerUnuseInteractableObject += new ObjectInteractEventHandler(onUnuse);

        pickups = GameObject.Find("Pickups");
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
    private void onUse(object sender, ObjectInteractEventArgs e)
    {
        if (e.target == pickups)
        {
            for (int i = 0; i < pickups.transform.childCount; ++i)
            {
                var ch = pickups.transform.GetChild(i);
                ch.gameObject.SetActive(ch.name != "Open");
            }
            var cp = pickups.GetComponent<PickupsTrigger>().currentPickup();
            if (cp)
            {
                //Debug.Log("Going to pick up " + cp.name);
                fixed_joint = pickups.AddComponent<FixedJoint>();
                fixed_joint.connectedBody = cp.GetComponent<Rigidbody>();
            }
        }
    }
    private void onUnuse(object sender, ObjectInteractEventArgs e)
    {
        if (e.target == pickups)
        {
            for (int i = 0; i < pickups.transform.childCount; ++i)
            {
                var ch = pickups.transform.GetChild(i);
                ch.gameObject.SetActive(ch.name != "Closed");
            }
            if (fixed_joint)
            {
                //Debug.Log("Going to drop " + fixed_joint.name);
                fixed_joint.connectedBody = null;
                Destroy(fixed_joint);
            }
        }
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
