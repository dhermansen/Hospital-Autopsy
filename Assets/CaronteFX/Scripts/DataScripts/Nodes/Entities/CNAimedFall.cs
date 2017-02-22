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
  /// Contains the data of an aimed fall node.
  /// </summary>
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNAimedFall")] 
  public class CNAimedFall : CNEntity
  {
    public override CNField Field 
    { 
      get
      {
        if (field_ == null)
        {
          CNField.AllowedTypes allowedTypes =   CNField.AllowedTypes.Geometry 
                                              | CNField.AllowedTypes.BodyNode;

          field_ = new CNField( false, allowedTypes, false );
        }
        return field_;
      }
    }

    [SerializeField]
           CNField fieldAimGameObjects_;
    public CNField FieldAimGameObjects
    {
      get
      {
        if (fieldAimGameObjects_ == null)
        {
          CNField.AllowedTypes allowedTypes = CNField.AllowedTypes.GameObject;
                      
          fieldAimGameObjects_ = new CNField( false, allowedTypes, false );
        }
        return fieldAimGameObjects_;
      }
    }


    [SerializeField]
    float speedRate_ = 0.5f;
    public float SpeedRate
    {
      get { return speedRate_; }
      set { speedRate_ = Mathf.Clamp(value, 0f, 1f); }
    }

    [SerializeField]
    float release_threshold_ = 0.5f;
    public float ReleaseThreshold
    {
      get { return release_threshold_; }
      set { release_threshold_ = Mathf.Clamp(value, 0f, 1f); }
    }
    protected override void CloneData(CommandNode node)
    {
      base.CloneData(node);

      CNAimedFall clone = (CNAimedFall)node;

      clone.fieldAimGameObjects_ = fieldAimGameObjects_.DeepClone();

      clone.speedRate_         = speedRate_;
      clone.release_threshold_ = release_threshold_;
    }

    public override CommandNode DeepClone(GameObject dataHolder)
    {
      CNAimedFall clone = CommandNode.CreateInstance<CNAimedFall>(dataHolder);    
      CloneData(clone);
      return clone;
    }

    public override bool UpdateNodeReferences(Dictionary<CommandNode, CommandNode> dictNodeToClonedNode)
    {
      bool wasAnyUpdatedA = field_.UpdateNodeReferences(dictNodeToClonedNode);
      bool wasAnyUpdatedB = fieldAimGameObjects_.UpdateNodeReferences(dictNodeToClonedNode);

      return (wasAnyUpdatedA || wasAnyUpdatedB);
    }
  }

}//namespace CaronteFX