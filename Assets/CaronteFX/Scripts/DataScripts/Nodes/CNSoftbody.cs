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
using UnityEngine;

namespace CaronteFX
{
  /// <summary>
  /// Holds the data of a soft bodies node.
  /// </summary>
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNSoftbody")]
  public class CNSoftbody : CNBody
  {
    [SerializeField]
    private int resolution_ = 64;
    public int Resolution
    {
      get { return resolution_;} 
      set { resolution_ = value; }
    }

    [SerializeField]
    private bool autoCollide_ = false;
    public bool AutoCollide
    {
      get { return autoCollide_; }
      set { autoCollide_ = value; }
    }

    [SerializeField]
    private float lengthStiffness_ = 0.3f;
    public float LengthStiffness
    {
      get { return lengthStiffness_; }
      set { lengthStiffness_ = value; }
    }

    [SerializeField]
    private float volumeStiffness_ = 0.3f;
    public float VolumeStiffness
    {
      get { return volumeStiffness_; }
      set { volumeStiffness_ = value; }
    }

    [SerializeField]
    private float areaStiffness_ = 0.3f;
    public float AreaStiffness
    {
      get { return areaStiffness_; }
      set { areaStiffness_ = value; }
    }

    [SerializeField]
    private bool plasticityFoldout_ = false;
    public bool PlasticityFoldout
    {
      get { return plasticityFoldout_; }
      set { plasticityFoldout_ = value; }
    }

    [SerializeField]
    private bool plasticity_ = false;
    public bool Plasticity
    {
      get { return plasticity_; }
      set { plasticity_ = value; }
    }

    [SerializeField]
    private float threshold_in01_ = 0.1f;
    public float Threshold_in01
    {
      get { return threshold_in01_; }
      set { threshold_in01_ = value; }
    }
    
    [SerializeField]
    private float acquired_in01_ = 0.8f;
    public float Acquired_in01
    {
      get { return acquired_in01_; }
      set { acquired_in01_ = value; }
    }

    [SerializeField]   
    private float compressionLimit_in01_ = 0.1f; 
    public float CompressionLimit_in01
    {
      get { return compressionLimit_in01_; }
      set { compressionLimit_in01_ = value; }
    }
    
    [SerializeField]   
    private float expansionLimit_in_1_100_ = 1.5f;
    public float ExpansionLimit_in_1_100
    {
      get { return expansionLimit_in_1_100_; }
      set { expansionLimit_in_1_100_ = value; }
    }
      
    [SerializeField]   
    private float dampingPerSecond_CM_ = 0.25f;
    public float DampingPerSecond_CM
    {
      get { return dampingPerSecond_CM_; }
      set { dampingPerSecond_CM_ = value; }
    }

    protected override void CloneData(CommandNode node)
    {
      base.CloneData(node);

      CNSoftbody clone = (CNSoftbody)node;

      clone.resolution_      = resolution_;

      clone.lengthStiffness_ = lengthStiffness_;
      clone.volumeStiffness_ = volumeStiffness_;
      clone.areaStiffness_   = areaStiffness_;

      clone.plasticity_      = plasticity_;

      clone.threshold_in01_           = threshold_in01_;
      clone.acquired_in01_            = acquired_in01_;
      clone.compressionLimit_in01_    = compressionLimit_in01_;
      clone.expansionLimit_in_1_100_  = expansionLimit_in_1_100_;

      clone.dampingPerSecond_CM_    = dampingPerSecond_CM_;
    }

    public override CommandNode DeepClone(GameObject dataHolder)
    {
      CNSoftbody clone = CommandNode.CreateInstance<CNSoftbody>(dataHolder);
      CloneData(clone);
      return clone;
    }

  } // class CNSoftbody...

} //namespace Caronte...