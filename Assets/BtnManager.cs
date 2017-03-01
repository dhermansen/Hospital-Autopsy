using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnManager : ButtonStyler
{
    private Toggle toggle;

    // Use this for initialization
    void Start()
    {
        base.StartStyler();
        toggle = this.gameObject.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener((on) =>
        {
            PointerExit(on);
            this.SendMessageUpwards("ToggleChange", toggle);
        });
    }

    public override void OnPointerExit(PointerEventData pd)
    {
        PointerExit(toggle.isOn);
    }
}