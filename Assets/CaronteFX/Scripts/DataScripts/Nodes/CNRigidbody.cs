// ***********************************************************
//	Copyright 2016 Next Limit Technologies, http://www.nextlimit.com
//	All rights reserved.
//
//	THIS SOFTWARE IS PROVIDED 'AS IS' AND WITHOUT ANY EXPRESS OR
//	IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
//	WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
//
// ***********************************************************

using System.Collections;
using System;
using UnityEngine;

namespace CaronteFX
{
  /// <summary>
  /// Holds the data of a rigid bodies node.
  /// </summary>
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNRigidbody")]
  public class CNRigidbody : CNBody
  {
    [SerializeField]
    bool useBallTree_ = false;
    public bool UseBallTree
    {
      get { return useBallTree_; }
      set { useBallTree_ = value; }
    }

    [SerializeField]
    int balltreeLOD_ = 1;
    public int BalltreeLOD
    {
      get { return balltreeLOD_; }
      set { balltreeLOD_ = value; }
    }

    [SerializeField]
    protected bool isFiniteMass_ = true;
    public bool IsFiniteMass 
    { 
      get { return isFiniteMass_; }
      set { isFiniteMass_ = value; }
    }
    //-----------------------------------------------------------------------------------
    protected override void CloneData(CommandNode node)
    {
      base.CloneData(node);

      CNRigidbody clone = (CNRigidbody)node;

      clone.useBallTree_  = useBallTree_;
      clone.balltreeLOD_  = balltreeLOD_;
      clone.isFiniteMass_ = isFiniteMass_;
    }
    //-----------------------------------------------------------------------------------
    public override CommandNode DeepClone(GameObject dataHolder)
    {
      CNRigidbody clone = CommandNode.CreateInstance<CNRigidbody>(dataHolder);   
      CloneData(clone);
      return clone;
    }
    //-----------------------------------------------------------------------------------
  } //class CNRigidbody

}//namespace CaronteFX...