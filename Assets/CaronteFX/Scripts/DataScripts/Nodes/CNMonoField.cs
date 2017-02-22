// ***********************************************************
//	Copyright 2016 Next Limit Technologies, http://www.nextlimit.com
//	All rights reserved.
//
//	This source code is free for all non-commercial uses.
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
  /// Holds the data of a node that contains at least a field.
  /// </summary>
  public abstract class CNMonoField : CommandNode
  {
    [SerializeField]
    protected CNField field_;
    public abstract CNField Field { get; }
        
    //-----------------------------------------------------------------------------------
    public override bool UpdateNodeReferences(Dictionary<CommandNode, CommandNode> dictNodeToClonedNode)
    {
      return (field_.UpdateNodeReferences( dictNodeToClonedNode ));
    }
    //-----------------------------------------------------------------------------------
    protected override void CloneData(CommandNode node)
    {
      base.CloneData(node);
      CNMonoField clone = (CNMonoField)node;
      
      if (field_ != null)
      {
        clone.field_ = field_.DeepClone();
      }    
    }

  }
}
