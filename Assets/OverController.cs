using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverController : MonoBehaviour {
    private GameObject current;
    private GameObject ppanel;
    private GameObject fimgpanel;
    private GameObject mimgpanel;
    bool female_pressed;
    // Use this for initialization
    void Start () {
        current = GameObject.Find("Renal");//.GetComponent<RenalController>();
        current.SetActive(false);
        ppanel = GameObject.Find("PracticePanel");
        ppanel.SetActive(false);
        fimgpanel = GameObject.Find("FemaleImagePanel");
        //fimgpanel.SetActive(false);
        mimgpanel = GameObject.Find("MaleImagePanel");
        //mimgpanel.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		
	}
    void GeneratePressed(Button btn)
    {
        Debug.Log("It's pressed!");
        var gbt = btn.gameObject.GetComponentInChildren<Text>();
        Debug.Log("Text " + gbt.text);
        if (gbt.text.Contains("G"))
        {
            //Move current system back to original location.
            // (May need to reset hinges, etc.)
            current.SetActive(true);
            ppanel.SetActive(true);
            female_pressed = fimgpanel.activeSelf;
            fimgpanel.SetActive(false);
            mimgpanel.SetActive(false);
            gbt.text = "  RESET";

        }
        else
        {
            gbt.text = "  GENERATE";
            //Activate current system
            //current.reset();
            ppanel.SetActive(false);
            fimgpanel.SetActive(female_pressed);
            mimgpanel.SetActive(!female_pressed);
        }
    }
}
