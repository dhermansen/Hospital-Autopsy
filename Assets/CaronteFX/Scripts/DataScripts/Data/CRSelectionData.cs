// ***********************************************************
//	Copyright 2016 Next Limit Technologies, http://www.nextlimit.com
//	All rights reserved.
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
  /// Contains Selection data of the nodes Hierarchy.
  /// </summary>
  [Serializable]
  public class CRSelectionData
  { 
    [SerializeField]
    private CommandNode focusedNode_;
    public CommandNode FocusedNode
    {
      get { return focusedNode_; }
      set { focusedNode_ = value; }
    }

    [SerializeField]
    private CommandNode lastSelectedNode_;
    public CommandNode LastSelectedNode
    {
      get { return lastSelectedNode_; }
      set { lastSelectedNode_ = value; }
    }

    [SerializeField]
    private List<CommandNode> listSelectedNode_ = new List<CommandNode>();
    public List<CommandNode> ListSelectedNode
    {
      get { return listSelectedNode_; }
      set { listSelectedNode_ = value; }
    }

    public int SelectedNodeCount
    {
      get { return listSelectedNode_.Count; }
    }

    [SerializeField]
    private List<CommandNode> listSelectedNodeAndChildren_ = new List<CommandNode>();
    public List<CommandNode> ListSelectedNodeAndChildren
    {
      get { return listSelectedNodeAndChildren_; }
      set { listSelectedNodeAndChildren_ = value;}
    }
  }// CRSelectionData...

}// namespace CaronteFX...
