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
  /// Holds the data of a speed limiter node.
  /// </summary>
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNSpeedLimiter")]
  public class CNSpeedLimiter : CNEntity
  {
    [SerializeField]
    private float speed_limit_ = 20f;
    public float SpeedLimit
    {
      get { return speed_limit_; }
      set { speed_limit_ = value; }
    }

    [SerializeField]
    private float falling_speed_limit_ = 50f;
    public float FallingSpeedLimit
    {
      get { return falling_speed_limit_; }
      set { falling_speed_limit_ = value; }
    }
    //-----------------------------------------------------------------------------------
    protected override void CloneData(CommandNode node)
    {
      base.CloneData(node);

      CNSpeedLimiter clone = (CNSpeedLimiter)node;

      clone.speed_limit_         = speed_limit_;
      clone.falling_speed_limit_ = falling_speed_limit_;   
    }
    //-----------------------------------------------------------------------------------
    public override CommandNode DeepClone( GameObject dataHolder )
    {
      CNSpeedLimiter clone = dataHolder.AddComponent<CNSpeedLimiter>();      
      CloneData(clone);
      return clone;
    }
  }
}