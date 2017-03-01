using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Generate : ButtonStyler
{

    private Button btn;

    // Use this for initialization
    void Start()
    {
        base.StartStyler();
        btn = this.gameObject.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            this.SendMessageUpwards("GeneratePressed", btn);
        });
    }
}