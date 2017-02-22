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
using System.Linq;
using UnityEngine;

namespace CaronteFX
{
  /// <summary>
  /// Group node hierarchy.
  /// </summary>
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNGroup")]
  public class CNGroup : CNMonoField
  {
    public override CNField Field
    {
      get
      {
        if (field_ == null)
        {
          field_ = new CNField(false, CNField.AllowedTypes.Geometry, CNField.ScopeFlag.Inherited, true);
        }
        return field_;
      }
    }

    [SerializeField]
    private bool isOpen_ = true;
    public bool IsOpen
    {
      get
      {
        return isOpen_;
      }
      set
      {
        isOpen_ = value;
      }
    }

    private bool isOpenAux_ = false;
    public bool IsOpenAux
    {
      get
      {
        return isOpenAux_;
      }
      set
      {
        isOpenAux_ = value;
      }
    }

    [SerializeField]
    private bool isEffectRoot_ = false;
    public new bool IsEffectRoot
    {
      get { return isEffectRoot_; }
      set { isEffectRoot_ = value; }
    }

    [SerializeField]
    private bool isSubeffectsFolder_ = false;
    public new bool IsSubeffectsFolder
    {
      get { return isSubeffectsFolder_; }
      set { isSubeffectsFolder_ = true; }
    }

    public enum CARONTEFX_SCOPE
    {
      SCENE = 0,
      CARONTEFX_GAMEOBJECT_PARENT = 1,
      CARONTEFX_GAMEOBJECT = 2,
    };

    [SerializeField]
    CARONTEFX_SCOPE caronteFX_scope_ = CARONTEFX_SCOPE.SCENE;
    public CARONTEFX_SCOPE CaronteFX_scope
    {
      get { return caronteFX_scope_; }
      set { caronteFX_scope_ = value; }
    }

    //----------------------------------------------------------------------------------
    protected override void CloneData(CommandNode node)
    {
      base.CloneData(node);

      CNGroup clonedgroup = (CNGroup)node;

      clonedgroup.isOpen_             = isOpen_;
      clonedgroup.isOpenAux_          = isOpenAux_;
      clonedgroup.isEffectRoot_       = isEffectRoot_;
      clonedgroup.isSubeffectsFolder_ = isSubeffectsFolder_;
    }
    //----------------------------------------------------------------------------------
    public override CommandNode DeepClone( GameObject go )
    {
      CNGroup clonedGroup = CRTreeNode.CreateInstance<CNGroup>(go);
      CloneData( clonedGroup );

      for ( int i = 0 ; i < ChildCount; ++i )
      {
        CommandNode child = (CommandNode) Children[i];
        CommandNode cloneChild = child.DeepClone( go );
        cloneChild.Parent = clonedGroup;
      }

      return clonedGroup;
    }
    //----------------------------------------------------------------------------------
    public override bool UpdateNodeReferences(Dictionary<CommandNode, CommandNode> dictNodeToClonedNode)
    {
      bool wasAnyUpdated = field_.UpdateNodeReferences(dictNodeToClonedNode);
      
      for ( int i = 0 ; i < ChildCount; ++i )
      {
        CommandNode child = (CommandNode) Children[i];
        wasAnyUpdated |= child.UpdateNodeReferences(dictNodeToClonedNode);
      }

      return ( wasAnyUpdated );
    }
    //----------------------------------------------------------------------------------

  }//class CNGroup

}//namespace Caronte