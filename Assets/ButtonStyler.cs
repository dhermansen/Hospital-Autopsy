using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonStyler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text txt;
    private Image bkg;
    public Color32 purple = new Color32(0x54, 0x02, 0x4f, 0xff);
    private Sprite bkg_on;
    private Sprite bkg_off;

    // Use this for initialization
    public void StartStyler()
    {
        txt = this.gameObject.GetComponentInChildren<Text>();
        bkg = this.gameObject.GetComponentInChildren<Image>();
        bkg.color = this.purple;
        bkg_on = Resources.Load<Sprite>("__blank");
        bkg_off = Resources.Load<Sprite>("SquareFrame");
        bkg.overrideSprite = bkg_off;

        PointerExit(false);
    }

    public void OnPointerEnter(PointerEventData pd)
    {
        txt.color = Color.white;
        bkg.overrideSprite = bkg_on;
    }

    public virtual void OnPointerExit(PointerEventData pd)
    {
        PointerExit(false);
    }
    public void PointerExit(bool stayOn)
    {
        if (stayOn)
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
