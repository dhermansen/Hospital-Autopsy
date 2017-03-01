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
    private GameObject block_submenu;
    private GameObject female_image;
    private GameObject male_image;
    private GameObject generate_btn;
    private GameObject female_renal;
    private GameObject female_heart;
    private GameObject female_lungs;
    private GameObject female_hepatic;
    private GameObject male_renal;
    private GameObject male_heart;
    private GameObject male_lungs;
    private GameObject male_hepatic;
    GameObject inactivate(string name)
    {
        //Debug.Log("inactivating " + name);
        var go = GameObject.Find(name);
        go.SetActive(false);
        return go;
    }
    // Use this for initialization
    void Start()
    {
        mode_submenu = inactivate("ModeTG");
        generate_btn = inactivate("Generate_Btn");

        female_renal = inactivate("Renal_Female");
        female_heart = inactivate("Heart_Female");
        female_lungs = inactivate("Lungs_Female");
        female_hepatic = inactivate("Hepatic_Female");
        female_image = inactivate("FemaleImagePanel");

        male_renal = inactivate("Renal_Male");
        male_heart = inactivate("Heart_Male");
        male_lungs = inactivate("Lungs_Male");
        male_hepatic = inactivate("Hepatic_Male");
        male_image = inactivate("MaleImagePanel");

        block_submenu = inactivate("BlockTG");
        
    }

    public void ToggleChange(Toggle t)
    {
        if (t.isOn)
            this.actives.Add(t.name);
        else
            this.actives.Remove(t.name);

        if (t.name == "Mode_Btn")
            mode_submenu.SetActive(t.isOn);
        if (t.name == "Female_Btn")
        {
            block_submenu.SetActive(t.isOn);
            female_image.SetActive(t.isOn);
        }
        if (t.name == "Male_Btn")
        {
            block_submenu.SetActive(t.isOn);
            male_image.SetActive(t.isOn);
        }
        female_renal.SetActive(actives.Contains("Female_Btn") && actives.Contains("Renal_Btn"));
        male_renal.SetActive(actives.Contains("Male_Btn") && actives.Contains("Renal_Btn"));
        female_heart.SetActive(actives.Contains("Female_Btn") && actives.Contains("Heart_Btn"));
        male_heart.SetActive(actives.Contains("Male_Btn") && actives.Contains("Heart_Btn"));
        female_lungs.SetActive(actives.Contains("Female_Btn") && actives.Contains("Lungs_Btn"));
        male_lungs.SetActive(actives.Contains("Male_Btn") && actives.Contains("Lungs_Btn"));
        female_hepatic.SetActive(actives.Contains("Female_Btn") && actives.Contains("Hepatic_Btn"));
        male_hepatic.SetActive(actives.Contains("Male_Btn") && actives.Contains("Hepatic_Btn"));

        if (actives.Contains("Renal_Btn") && actives.Contains("Practice_Btn")) {
            generate_btn.SetActive(true);
            //var gbt = generate_btn.GetComponent<Text>();
            //gbt.text = "Reset";
        }
        //else if (actives.Contains("Liver") && actives.Contains("Testing"))
        //{

        //}
        else
        {
            generate_btn.SetActive(false);
            //var gbt = generate_btn.GetComponent<Text>();
            //gbt.text = "Reset";
        }
    }
}