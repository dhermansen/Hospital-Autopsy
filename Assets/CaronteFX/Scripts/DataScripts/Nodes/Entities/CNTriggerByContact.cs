// ***********************************************************
//	Copyright 2016 Next Limit Technologies, http://www.nextlimit.com
//	All rights reserved.
//
//	THIS SOFTWARE IS PROVIDED 'AS IS' AND WITHOUT ANY EXPRESS OR
//	IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
//	WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
//
// ***********************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CaronteFX
{
  /// <summary>
  /// Holds the data of a trigger by contact node.
  /// </summary>
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNTriggerByContact")]
  public class CNTriggerByContact : CNTrigger
  {
    [SerializeField]
           CNField fieldA_;
    public CNField FieldA
    {
      get
      {
        if (fieldA_ == null)
        {
          CNField.AllowedTypes allowedTypes =   CNField.AllowedTypes.Geometry
                                              | CNField.AllowedTypes.BodyNode;
                      
          fieldA_ = new CNField( false, allowedTypes, false );
        }
        return fieldA_;
      }
    }

    [SerializeField]
           CNField fieldB_;
    public CNField FieldB
    {
      get
      {
        if (fieldB_ == null)
        {
          CNField.AllowedTypes allowedTypes =   CNField.AllowedTypes.Geometry
                                              | CNField.AllowedTypes.BodyNode;
                      
          fieldB_ = new CNField( false, allowedTypes, false );
        }
        return fieldB_;
      }
    }

    [SerializeField]
    float speedMin_ = 0f;
    public float SpeedMinN
    {
      get { return speedMin_; }
      set { speedMin_ = value; }
    }

    [SerializeField]
    float speedMin_T_ = 0f;
    public float SpeedMinT
    {
      get { return speedMin_T_; }
      set { speedMin_T_ = value; }
    }

    [SerializeField]
    bool triggerForInvolvedBodies_ = false;
    public bool TriggerForInvolvedBodies
    {
      get { return triggerForInvolvedBodies_; }
      set { triggerForInvolvedBodies_ = value; }
    }

    protected override void CloneData(CommandNode node)
    {
      base.CloneData(node);

      CNTriggerByContact clone = (CNTriggerByContact) node;
      clone.fieldA_ = fieldA_.DeepClone();
      clone.fieldB_ = fieldB_.DeepClone();

      clone.speedMin_                 = SpeedMinN;
      clone.speedMin_T_               = SpeedMinT;
      clone.triggerForInvolvedBodies_ = TriggerForInvolvedBodies;
    }

    public override CommandNode DeepClone(GameObject dataHolder)
    {
      CNTriggerByContact clone = CommandNode.CreateInstance<CNTriggerByContact>(dataHolder);
      CloneData(clone);
      return clone;
    }

    public override bool UpdateNodeReferences(Dictionary<CommandNode, CommandNode> dictNodeToClonedNode)
    {
      bool updateEntityField = field_.UpdateNodeReferences(dictNodeToClonedNode);
      bool updateA = fieldA_.UpdateNodeReferences(dictNodeToClonedNode);
      bool updateB = fieldB_.UpdateNodeReferences(dictNodeToClonedNode);

      return (updateEntityField || updateA || updateB);
    }

  }


}


