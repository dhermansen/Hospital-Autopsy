using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupsTrigger : MonoBehaviour {
    private GameObject renal;
    private GameObject current;
	// Use this for initialization
	void Start () {
        renal = GameObject.Find("Renal");
	}
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent && other.transform.IsChildOf(renal.transform))
        {
            current = other.gameObject;
            //Debug.Log("Enter Collider: " + other.name);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        current = null;
        //if (other.transform.parent && other.transform.IsChildOf(renal.transform))
            //Debug.Log("Exit Collider: " + other.name);
    }

    public GameObject currentPickup()
    {
        return current;
    }
    // Update is called once per frame
    void Update () {
		
	}
}
