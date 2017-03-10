using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SharpsController : MonoBehaviour {
    private GameObject left_kidney, right_kidney;

    // Use this for initialization
    void Start () {
        left_kidney = GameObject.Find("lKidney");
        right_kidney = GameObject.Find("rKidney");
    }
    private void destroy_joint(GameObject obj)
    {
        foreach (var fj in obj.GetComponentsInChildren<FixedJoint>()) {
            fj.connectedBody = null;
            Destroy(fj);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        if (other.gameObject.transform.IsChildOf(left_kidney.transform))
        {
            Debug.Log("Left kidney getting cut!");
            destroy_joint(left_kidney);
        }

        else if (other.gameObject.transform.IsChildOf(right_kidney.transform))
        {
            Debug.Log("Right kidney getting cut!");
            destroy_joint(right_kidney);
        }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
