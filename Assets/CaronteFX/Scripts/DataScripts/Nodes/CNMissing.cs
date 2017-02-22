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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaronteFX
{
  /// <summary>
  /// Holds the data of a node of Unknown type.
  /// </summary>
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNMissing")]
  public class CNMissing : CommandNode
  {
    public override string ListName 
    {
      get
      {
        return "[n] (missing node)"; 
      }
    }
    
    [SerializeField]
    private CommandNode parentNodeOfMissing_;
    public CommandNode ParentNodeOfMissing
    {
      get
      {
        return parentNodeOfMissing_;
      }
      set
      {
        parentNodeOfMissing_ = value;
      }
    }

    [SerializeField]
    private int childIdxOfParent_;
    public int ChildIdxOfParent
    {
      get
      {
        return childIdxOfParent_;
      }
      set
      {
        childIdxOfParent_ = value;
      }
    }

    public override CommandNode DeepClone( GameObject go )
    {
      throw new NotImplementedException();
    }

    public override bool UpdateNodeReferences(Dictionary<CommandNode, CommandNode> dictNodeToClonedNode)
    {
      throw new NotImplementedException();
    }
  }
}