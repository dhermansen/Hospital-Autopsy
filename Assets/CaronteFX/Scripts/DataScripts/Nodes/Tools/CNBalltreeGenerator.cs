using UnityEngine;
using System.Collections;

namespace CaronteFX
{
  /// <summary>
  /// Holds the data of a balltree creator node.
  /// </summary>

  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNBalltreeGenerator")] 
  public class CNBalltreeGenerator : CNMonoField
  {
    public override CNField Field
    {
      get
      {
        if (field_ == null)
        {
          field_ = new CNField(false, false);
        }
        return field_;
      }
    }

    public enum ECreationMode
    {
      USERENDERERS,
      USECOLLLIDERS
    }

    [SerializeField]
    ECreationMode creationMode_;
    public ECreationMode CreationMode
    {
      get { return creationMode_; }
      set { creationMode_ = value; }
    }

    [SerializeField]
    float balltreeLOD_ = 0.5f;
    public float BalltreeLOD
    {
      get { return balltreeLOD_; }
      set { balltreeLOD_ = value; }
    }

    [SerializeField]
    float balltreePrecision_ = 0.5f;
    public float BalltreePrecision
    {
      get { return balltreePrecision_; }
      set { balltreePrecision_ = value; }
    }

    [SerializeField]
    float balltreeHoleCovering_ = 0.0f;
    public float BalltreeHoleCovering
    {
      get { return balltreeHoleCovering_; }
      set { balltreeHoleCovering_ = value; }
    }

    protected override void CloneData(CommandNode node)
    {
      base.CloneData(node);
      CNBalltreeGenerator clone = (CNBalltreeGenerator)node;

      clone.creationMode_      = creationMode_;
      clone.balltreeLOD_       = balltreeLOD_;
      clone.balltreePrecision_ = balltreePrecision_;
    }

    public override CommandNode DeepClone(GameObject dataHolder)
    {
      CNSelector clone = CommandNode.CreateInstance<CNSelector>(dataHolder); 
      CloneData(clone);
      return clone;
    }
  }
}
