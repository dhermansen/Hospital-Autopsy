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
  /// Holds the data of a substituter node.
  /// </summary>
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNSubstituter")]
  public class CNSubstituter : CNEntity 
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
    private float probability_ = 1.0f;
    public float Probability
    {
      get { return probability_; }
      set { probability_ = value; }
    }

    [SerializeField]
    private uint probabilitySeed_ = 63216;
    public uint ProbabilitySeed
    {
      get { return probabilitySeed_; }
      set { probabilitySeed_ = value; }
    }


    public override CommandNode DeepClone(GameObject dataHolder)
    {
      CNSubstituter clone = CommandNode.CreateInstance<CNSubstituter>(dataHolder);
      
      clone.field_   = field_ .DeepClone();
      clone.fieldA_  = fieldA_.DeepClone();
      clone.fieldB_  = fieldB_.DeepClone();

      clone.Name   = Name;
      clone.timer_ = Timer;
      clone.Probability = probability_;
      clone.ProbabilitySeed = probabilitySeed_;
    
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
