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
  /// Holds the data of a welder node.
  /// </summary>
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNWelder")] 
  public class CNWelder : CNMonoField
  {
    public override CNField Field
    {
      get
      {
        if (field_ == null)
        {
          field_ = new CNField(false, false);
        }
        return field_;
      }
    }

    [SerializeField]
    GameObject weldGameObject_;
    public UnityEngine.GameObject WeldGameObject
    {
      get { return weldGameObject_; }
      set { weldGameObject_ = value; }
    }

    public override CommandNode DeepClone(GameObject dataHolder)
    {
      CNWelder clone = CommandNode.CreateInstance<CNWelder>(dataHolder);
      CloneData(clone);
      return clone;
    }

  } //namespace CNWelder
}