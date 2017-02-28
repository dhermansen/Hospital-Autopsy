using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HiddenMenu : MonoBehaviour {
    private GenerateBtn1 gbtn;
	// Use this for initialization
	void Start () {
        Debug.Log("Start called");
        gbtn = this.GetComponentInParent<GenerateBtn1>();
        if (!gbtn)
            Debug.LogErrorFormat("Didn't get GenerateBtn1");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDisable()
    {
        Debug.Log("Disabling");
        if (!gbtn)
            return;
        var togs = this.gameObject.GetComponentsInChildren<Toggle>();
        foreach (Toggle t in togs)
            gbtn.RemoveToggle(t);

    }
    private void OnEnable()
    {
        if (!gbtn)
            return;
        var togs = this.gameObject.GetComponentsInChildren<Toggle>();
        foreach (Toggle t in togs)
            gbtn.AddToggle(t);
    }
}

