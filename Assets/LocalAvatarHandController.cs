using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class LocalAvatarHandController : MonoBehaviour {
    Dictionary<string, Transform> hands = new Dictionary<string, Transform>();
    Dictionary<string, string> target_to_hand = new Dictionary<string, string>();
    OvrAvatar ovr;
    VRTK_InteractGrab left_grab, right_grab;
    VRTK_InteractUse left_use, right_use;
    private GameObject pickups;
    private GameObject scissors;
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
        left_use = left_grab.GetComponent<VRTK_InteractUse>();
        right_use = right_grab.GetComponent<VRTK_InteractUse>();
        left_grab.ControllerGrabInteractableObject += new ObjectInteractEventHandler(onGrab);
        left_grab.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(onUngrab);
        right_grab.ControllerGrabInteractableObject += new ObjectInteractEventHandler(onGrab);
        right_grab.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(onUngrab);
        right_use.ControllerUseInteractableObject += new ObjectInteractEventHandler(onUse);
        right_use.ControllerUnuseInteractableObject += new ObjectInteractEventHandler(onUnuse);
        left_use.ControllerUseInteractableObject += new ObjectInteractEventHandler(onUse);
        left_use.ControllerUnuseInteractableObject += new ObjectInteractEventHandler(onUnuse);

        pickups = GameObject.Find("Pickups");
        scissors = GameObject.Find("Scissors");
    }
    private void onGrab(object sender, ObjectInteractEventArgs e)
    {
        if (!target_to_hand.ContainsKey(e.target.name))
            return;
        //Debug.Log(e.target.name);
        if (e.controllerIndex == 0)
            ovr.LeftHandCustomPose = hands["HandLeft" + target_to_hand[e.target.name]];
        else
            ovr.RightHandCustomPose = hands["HandRight" + target_to_hand[e.target.name]];

        if (e.target == scissors)
        {
            activateAllBut(scissors, "Closed");
        }
    }
    private void onUngrab(object sender, ObjectInteractEventArgs e)
    {
        if (e.controllerIndex == 0)
            ovr.LeftHandCustomPose = null;
        else if (e.controllerIndex == 1)
            ovr.RightHandCustomPose = null;
    }
    private void activateAllBut(GameObject g, string name)
    {
        for (int i = 0; i < g.transform.childCount; ++i)
        {
            var ch = g.transform.GetChild(i);
            ch.gameObject.SetActive(ch.name != name);
        }
    }
    private void onUse(object sender, ObjectInteractEventArgs e)
    {
        if (e.target == pickups)
        {
            activateAllBut(pickups, "Open");
            var cp = pickups.GetComponent<PickupsTrigger>().currentPickup();
            if (cp)
            {
                //Debug.Log("Going to pick up " + cp.name);
                fixed_joint = pickups.AddComponent<FixedJoint>();
                fixed_joint.connectedBody = cp.GetComponent<Rigidbody>();
            }
        }
        if (e.target == scissors)
        {
            if (e.controllerIndex == 0)
                ovr.LeftHandCustomPose = hands["HandLeft_ScissorsClosed"];
            else
                ovr.RightHandCustomPose = hands["HandRight_ScissorsClosed"];

            activateAllBut(scissors, "Open");
        }
    }
    private void onUnuse(object sender, ObjectInteractEventArgs e)
    {
        if (e.target == pickups)
        {
            activateAllBut(pickups, "Closed");
            if (fixed_joint)
            {
                //Debug.Log("Going to drop " + fixed_joint.name);
                fixed_joint.connectedBody = null;
                Destroy(fixed_joint);
            }
        }
        if (e.target == scissors)
        {
            if (e.controllerIndex == 0)
                ovr.LeftHandCustomPose = hands["HandLeft_ScissorsOpen"];
            else
                ovr.RightHandCustomPose = hands["HandRight_ScissorsOpen"];

            activateAllBut(scissors, "Closed");
        }
    }
    // Update is called once per frame
    void Update () {
    }
}
