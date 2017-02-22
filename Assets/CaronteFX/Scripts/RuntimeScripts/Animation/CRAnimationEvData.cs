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
using System;
using System.Collections;

namespace CaronteFX
{
  /// <summary>
  /// Holds the data of a general animation event.
  /// </summary>
  [Serializable]
  public class CRAnimationEvData
  {
    public enum EEventDataType
    {
     None,
     Contact
    }

    public string emitterName_;
    public EEventDataType type_;
  }

  /// <summary>
  /// Holds the data of a contact event.
  /// </summary>
  [Serializable]
  public class CRContactEvData : CRAnimationEvData
  {
    public GameObject GameObjectA;
    public GameObject GameObjectB;

    public Vector3 position_;
    public Vector3 velocityA_;
    public Vector3 velocityB_;

    public float relativeSpeed_N_;
    public float relativeSpeed_T_;

    public float relativeP_N_;
    public float relativeP_T_;
  }
}
