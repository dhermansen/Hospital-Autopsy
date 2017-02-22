//ropeJS by youtube.com/vulgerstal
//How to use:
//1) Drag and Drop RopeJS Prefab

#pragma strict
function Start () {

    GetComponent.<HingeJoint>().connectedBody=transform.parent.GetComponent.<Rigidbody>();

}
