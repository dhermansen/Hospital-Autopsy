using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverController : MonoBehaviour {
    private GameObject current_orig;
    private GameObject current_copy;
    private GameObject ppanel;
    private GameObject fimgpanel;
    private GameObject mimgpanel;
    bool female_pressed;

    private GameObject dup(GameObject orig)
    {
        //orig.SetActive(true);
        var copy = GameObject.Instantiate(orig);
        copy.transform.SetParent(orig.transform.parent);
        copy.name = orig.name;
        copy.SetActive(true);
        orig.SetActive(false);
        return copy;
    }
    // Use this for initialization
    void Start () {
        current_orig = GameObject.Find("Renal");
        //current_copy = dup(current_orig);
        //current_copy.SetActive(false);

        ppanel = GameObject.Find("PracticePanel");
        ppanel.SetActive(false);
        fimgpanel = GameObject.Find("FemaleImagePanel");
        mimgpanel = GameObject.Find("MaleImagePanel");
    }

    // Update is called once per frame
    void Update () {
		
	}
    void GeneratePressed(Button btn)
    {
        var gbt = btn.gameObject.GetComponentInChildren<Text>();
        if (gbt.text.Contains("G"))
        {
            //Move current system back to original location.
            // (May need to reset hinges, etc.)
            current_copy.SetActive(true);
            ppanel.SetActive(true);
            female_pressed = fimgpanel.activeSelf;
            fimgpanel.SetActive(false);
            mimgpanel.SetActive(false);
            gbt.text = "  RESET";

        }
        else
        {
            //Activate current system
            GameObject.Destroy(current_copy);
            current_copy = dup(current_orig);
        }
    }
}
