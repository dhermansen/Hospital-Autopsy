using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GenerateBtn1 : MonoBehaviour {
    private Toggle gbtn;
    private List<Toggle> toggles;
    // Use this for initialization
    void Start () {
        toggles = this.gameObject.GetComponentsInChildren<Toggle>().ToList<Toggle>();

        foreach (Toggle comp in this.toggles)
        {
            Debug.Log(comp.name + ": " + comp.GetType().ToString());
        }
    }
    private void CheckGenerate()
    {
        Debug.Log("Checked:");
        var names = toggles.Where((t) => t.isOn).Select((t) => t.name);
        foreach (var name in names)
            Debug.LogWarning(name);
        if (names.Contains("Instruction") && names.Contains("Renal"))
        {
            gbtn.gameObject.SetActive(true);
            //gbtn.onClick.AddListener(() =>
            //{
            //    var txt = gbtn.gameObject.GetComponent<Text>();
            //    if (txt.text == "Reset") {
            //        txt.text = "Generate";
            //    }
            //    else {
            //        txt.text = "Reset";
            //    }
            //});
            Debug.Log("Generate Instruction Renal");
            Debug.Log(gbtn.name);
        }

    }
    public void AddToggle(Toggle t)
    {
        this.toggles.Add(t);
        Debug.Log(t.name);
        if (t.name == "Generate_Btn")
            gbtn = t;
        t.onValueChanged.AddListener((bool b) => CheckGenerate());
        Debug.LogFormat("Number stored: {0}", this.toggles.Count());
    }
    public void RemoveToggle(Toggle t)
    {
        if (!this.toggles.Remove(t))
            Debug.LogWarning("Unable to remove " + t.name);
        Debug.LogFormat("Number stored: {0}", this.toggles.Count());
    }
	// Update is called once per frame
	void Update () {
		
	}
}
