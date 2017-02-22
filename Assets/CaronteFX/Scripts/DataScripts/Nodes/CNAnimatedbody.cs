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
  /// Holds the data of a node of kinematic bodies.
  /// </summary>
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNAnimatedbody")]
  public class CNAnimatedbody : CNRigidbody
  { 
    public enum EAnimationType
    {
      Animator,
      CaronteFX,
    }

    [SerializeField]
    EAnimationType animationType_ = EAnimationType.Animator;
    public EAnimationType AnimationType
    {
      get { return animationType_; }
      set { animationType_ = value; }
    }


    [SerializeField]
    bool overrideAnimationController_ = true;
    public bool OverrideAnimationController
    {
      get { return overrideAnimationController_;  }
      set { overrideAnimationController_ = value; }
    }

    [SerializeField]
    AnimationClip un_animationClip_;
    public AnimationClip UN_AnimationClip
    {
      get { return un_animationClip_; }
      set { un_animationClip_ = value; }
    }

    [SerializeField]
    float timeStart_ = 0.0f;
    public float TimeStart
    {
      get { return timeStart_; }
      set { timeStart_ = value; }
    }

    [SerializeField]
    float timeLength_ = float.MaxValue;
    public float TimeLength
    {
      get { return timeLength_; }
      set { timeLength_ = value; }
    }

    protected override void CloneData(CommandNode node)
    {
      base.CloneData(node);

      CNAnimatedbody clone = (CNAnimatedbody)node;

      clone.overrideAnimationController_ = overrideAnimationController_;
      clone.un_animationClip_ = un_animationClip_;
      clone.timeStart_        = timeStart_;
      clone.timeLength_       = timeLength_;
    }

    public override CommandNode DeepClone(GameObject dataHolder)
    {
      CNAnimatedbody clone = CNAnimatedbody.CreateInstance<CNAnimatedbody>(dataHolder);   
      base.CloneData(clone);
      return clone;
    }
  }// class CNAnimatedbody...

}// namespace CaronteFX