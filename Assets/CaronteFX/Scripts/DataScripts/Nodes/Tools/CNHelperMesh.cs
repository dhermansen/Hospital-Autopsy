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
  /// Holds the data of a helper mesh node.
  /// </summary>
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNHelperMesh")] 
  public class CNHelperMesh : CommandNode
  {
    [SerializeField]
    UnityEngine.Mesh helperMesh_;
    public UnityEngine.Mesh HelperMesh
    {
      get { return helperMesh_; }
      set { helperMesh_ = value; }
    }

    [SerializeField]
    GameObject helperGO_;
    public UnityEngine.GameObject HelperGO
    {
      get { return helperGO_; }
      set { helperGO_ = value; }
    }

    [SerializeField]
    uint randomSeed_ = 0;
    public uint RandomSeed
    {
      get { return randomSeed_; }
      set { randomSeed_ = value; }
    }
       
    [SerializeField]                                  
    uint resolution_ = 1;
    public uint Resolution
    {
      get { return resolution_; }
      set { resolution_ = value; }
    }
                     
    [SerializeField]                    
    uint nBumps_ = 0;
    public uint NBumps
    {
      get { return nBumps_; }
      set { nBumps_ = (uint)Mathf.Clamp( (int)value, 0, 100000); }
    }
                        
    [SerializeField]                 
    float radiusMin_ = 0.1f;
    public float RadiusMin
    {
      get { return radiusMin_; }
      set { radiusMin_ = Mathf.Clamp(value, 0, float.MaxValue); }
    }
          
    [SerializeField]                               
    float radiusMax_ = 1f;
    public float RadiusMax
    {
      get { return radiusMax_; }
      set { radiusMax_ = Mathf.Clamp(value, 0, float.MaxValue); }
    }

    protected override void CloneData(CommandNode node)
    {
      base.CloneData(node);

      CNHelperMesh clone = (CNHelperMesh)node;

      clone.RandomSeed = RandomSeed;                          
      clone.Resolution = Resolution;     
      clone.NBumps     = NBumps;   
      clone.RadiusMin  = RadiusMin;               
      clone.RadiusMax  = RadiusMax;
    }

    public override CommandNode DeepClone(GameObject dataHolder)
    {
      CNHelperMesh clone = CommandNode.CreateInstance<CNHelperMesh>(dataHolder);
      CloneData(clone);
      return clone;
    }

    public override bool UpdateNodeReferences(Dictionary<CommandNode, CommandNode> dictNodeToClonedNode)
    {
      return false;
    }
  } //namespace CNWelder
}
