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
  /// Holds the data of a trigger by explosion node.
  /// </summary>
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNTriggerByExplosion")]
  public class CNTriggerByExplosion : CNTrigger
  {
    [SerializeField]
           CNField fieldExplosions_;
    public CNField FieldExplosions
    {
      get
      {
        if (fieldExplosions_ == null)
        {
          CNField.AllowedTypes allowedTypes = CNField.AllowedTypes.ExplosionNode;
                      
          fieldExplosions_ = new CNField( false, allowedTypes, false );
        }
        return fieldExplosions_;
      }
    }

    [SerializeField]
           CNField fieldBodies_;
    public CNField FieldBodies
    {
      get
      {
        if (fieldBodies_ == null)
        {
          CNField.AllowedTypes allowedTypes =   CNField.AllowedTypes.Geometry
                                              | CNField.AllowedTypes.BodyNode;
                      
          fieldBodies_ = new CNField( false, allowedTypes, false );
        }
        return fieldBodies_;
      }
    }

    protected override void CloneData(CommandNode node)
    {
      base.CloneData(node);

      CNTriggerByExplosion clone = (CNTriggerByExplosion)node;

      clone.fieldExplosions_  = fieldExplosions_.DeepClone();
      clone.fieldBodies_      = fieldBodies_    .DeepClone();
    }


    public override CommandNode DeepClone(GameObject dataHolder)
    {
      CNTriggerByExplosion clone = CommandNode.CreateInstance<CNTriggerByExplosion>(dataHolder);
      CloneData(clone);   
      return clone;
    }

    public override bool UpdateNodeReferences(Dictionary<CommandNode, CommandNode> dictNodeToClonedNode)
    {
      bool updateEntities  = field_.UpdateNodeReferences(dictNodeToClonedNode);
      bool updateExplosion = fieldExplosions_.UpdateNodeReferences(dictNodeToClonedNode);
      bool updateBodies    = fieldBodies_.UpdateNodeReferences(dictNodeToClonedNode);

      return (updateEntities || updateExplosion || updateBodies);
    }
  }
}//namespace CaronteFX


