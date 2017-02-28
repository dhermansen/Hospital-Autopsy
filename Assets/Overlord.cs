using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Overlord : MonoBehaviour
{
    private HashSet<string> actives = new HashSet<string>();
    //All submenus to hide/show on button selection
    private GameObject mode_submenu;
    private GameObject gender_submenu;
    GameObject inactivate(string name)
    {
        var go = this.gameObject.transform.Find(name).gameObject;
        go.SetActive(false);
        return go;
    }
    // Use this for initialization
    void Start()
    {
        mode_submenu = inactivate("ModeTG");
        gender_submenu = inactivate("GenderTG");
    }

    public void ToggleChange(Toggle t)
    {
        Debug.Log(t.name);
        if (t.isOn)
            this.actives.Add(t.name);
        else
            this.actives.Remove(t.name);

        foreach (var n in this.actives)
            Debug.LogWarning(n);

        mode_submenu.SetActive(t.name == "Mode_Btn" && t.isOn);
        if (actives.Contains("Male") && actives.Contains("Instruction")) {
            //
        }
    }
}