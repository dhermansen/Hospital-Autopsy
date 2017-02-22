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

namespace CaronteFX
{
  /// <summary>
  /// Contains the data of an entity node. (Daemons, triggers, substituter, etc.)
  /// </summary>
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNEntity")]
  public abstract class CNEntity : CNMonoField
  {
    public override CNField Field 
    { 
      get
      {
        if (field_ == null)
        {
          CNField.AllowedTypes allowedTypes =   CNField.AllowedTypes.Geometry 
                                              | CNField.AllowedTypes.BodyNode;

          field_ = new CNField( false, allowedTypes, true );
        }
        return field_;
      }
    }

    [SerializeField]
    protected float timer_;
    public float Timer
    {
      get { return timer_; }
      set { timer_ = Mathf.Clamp(value, 0f, float.MaxValue); }
    }

    protected override void CloneData(CommandNode node)
    {
      base.CloneData(node);
      
      CNEntity clone = (CNEntity) node;
      clone.timer_ = timer_;
    }
  }
}

