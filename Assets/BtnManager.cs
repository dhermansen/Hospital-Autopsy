using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BtnManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private Text txt;
    private Image bkg;
    private Toggle toggle;
    private Color32 purple = new Color32(0x54, 0x02, 0x4f, 0xff);
    private Sprite bkg_on;
    private Sprite bkg_off;

    // Use this for initialization
    void Start()
    {
        txt = this.gameObject.GetComponentInChildren<Text>();
        bkg = this.gameObject.GetComponentInChildren<Image>();
        bkg.color = this.purple;
        bkg_on = Resources.Load<Sprite>("__blank");
        bkg_off = Resources.Load<Sprite>("SquareFrame");
        bkg.overrideSprite = bkg_off;

        toggle = this.gameObject.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener((on) =>
        {
            PointerExit();
            this.SendMessageUpwards("ToggleChange", toggle);
        });
        PointerExit();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData pd)
    {
        txt.color = Color.white;
        bkg.overrideSprite = bkg_on;
    }

    public void OnPointerExit(PointerEventData pd)
    {
        PointerExit();
    }
    private void PointerExit()
    {
        if (toggle.isOn)
        {
            txt.color = Color.white;
            bkg.overrideSprite = bkg_on;
        }
        else
        {
            txt.color = this.purple;
            bkg.overrideSprite = bkg_off;
        }
    }
}