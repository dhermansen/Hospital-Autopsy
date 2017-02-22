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
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNTriggerByForce")]
  public class CNTriggerByForce : CNTrigger
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
    float forceMin_ = 30f;
    public float ForceMin
    {
      get { return forceMin_; }
      set { forceMin_ = value; }
    }

    [SerializeField]
    bool isForcePerMass_ = true;
    public bool IsForcePerMass
    {
      get { return isForcePerMass_; }
      set { isForcePerMass_ = value; }
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

      CNTriggerByForce clone = (CNTriggerByForce) node;
      clone.fieldA_ = fieldA_.DeepClone();

      clone.forceMin_                 = forceMin_;
      clone.isForcePerMass_           = isForcePerMass_;
      clone.triggerForInvolvedBodies_ = TriggerForInvolvedBodies;
    }

    public override CommandNode DeepClone(GameObject dataHolder)
    {
      CNTriggerByForce clone = CommandNode.CreateInstance<CNTriggerByForce>(dataHolder);
      CloneData(clone);
      return clone;
    }

    public override bool UpdateNodeReferences(Dictionary<CommandNode, CommandNode> dictNodeToClonedNode)
    {
      bool updateEntityField = field_.UpdateNodeReferences(dictNodeToClonedNode);
      bool updateA = fieldA_.UpdateNodeReferences(dictNodeToClonedNode);

      return (updateEntityField || updateA);
    }

  }


}


