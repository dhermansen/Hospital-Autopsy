using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour {
    private Vector3 origTrans;

    // Define initial position
    void Start()
    {
        origTrans = transform.position;
	}
	
	// Create clone of original object upon collision
	void OnTriggerEnter (Collider other) {
        if (other.gameObject.tag == "respawn")
        {
            transform.position = origTrans;
        }
	}
}
