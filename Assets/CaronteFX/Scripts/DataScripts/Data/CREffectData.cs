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
  /// Contains general simulation data options.
  /// </summary>
  [Serializable]
  public class CREffectData
  {
    #region Declarations

    public CNGroup rootNode_;
    public CNGroup subeffectsNode_;

    public string  name_;
   
    public  float quality_       = 50f;
    public  float antiJittering_ = 50f;

    public float totalTime_ = 10f; // max time to simulate
    public int frameRate_   = 30;  // frames per second

    public bool  byUserDeltaTime_     = false;
    public float deltaTime_           = -1f;
    public float calculatedDeltaTime_ = -1f;

    public bool  byUserCharacteristicObjectProperties_ = false; //automatic properties by default
    public float thickness_                            = -1f;
    public float calculatedThickness_                  = -1f;
    public float length_                               = -1f;
    public float calculatedLength_                     = -1f;

    #endregion
    //-----------------------------------------------------------------------------------
    public void SetDefault()
    {
      quality_       = 50f;
      antiJittering_ = 50f;

      totalTime_ = 10f;  
      frameRate_ = 30; 

      byUserDeltaTime_     = false;
      deltaTime_           = -1f;
      calculatedDeltaTime_ = -1f;

      byUserCharacteristicObjectProperties_ = false; 
      thickness_                            = -1f;
      calculatedThickness_                  = -1f;
      length_                               = -1f;
      calculatedLength_                     = -1f;
    }
    //-----------------------------------------------------------------------------------
    public void SetLastUsedProperties(float deltaTime, float length, float thickness)
    {
      calculatedDeltaTime_ = deltaTime;
      calculatedLength_    = length;
      calculatedThickness_ = thickness;
    }
    //-----------------------------------------------------------------------------------
  }
}
