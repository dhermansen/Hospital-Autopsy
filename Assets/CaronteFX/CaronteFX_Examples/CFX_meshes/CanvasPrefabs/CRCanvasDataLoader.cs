using UnityEngine;
using System.Collections;

namespace CaronteFX
{
  public class CRPersistentCanvasInfo : ScriptableObject
  {
    public bool hideCanvasDefinition = false;
    public bool hideCanvasBaked      = false;
  }
  [AddComponentMenu("CaronteFX/Miscellaneous/CRCanvasDataLoader")]
  public class CRCanvasDataLoader : MonoBehaviour
  {    
    public GameObject definitionPanel_;
    public GameObject bakedPanel_;

    public CRPersistentCanvasInfo persistenInfo_;

    void Awake()
    {
      LoadCanvasDefintionData();
      LoadCanvasBakedData();
    }

    void LoadCanvasDefintionData()
    {
      if (definitionPanel_ != null)
      {
        definitionPanel_.SetActive(!persistenInfo_.hideCanvasDefinition);
      }
    }

    void LoadCanvasBakedData()
    {
      if (bakedPanel_ != null)
      {
        bakedPanel_.SetActive(!persistenInfo_.hideCanvasBaked);
      }
    }

    public void HideCanvasDefinitionPanel(bool hide)
    {
      persistenInfo_.hideCanvasDefinition = hide;
      LoadCanvasDefintionData();
    }

    public void HideCanvasBakedPanel(bool hide)
    {
      persistenInfo_.hideCanvasBaked = hide;
      LoadCanvasBakedData();
    }

  }
}


