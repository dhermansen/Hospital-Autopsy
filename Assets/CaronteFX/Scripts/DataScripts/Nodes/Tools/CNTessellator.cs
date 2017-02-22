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
  /// Holds the data for a tessellator node.
  /// </summary>
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNTessellator")] 
  public class CNTessellator : CNMonoField
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
    float maxEdgeLength_ = 0.5f;
    public float MaxEdgeDistance
    {
      get { return maxEdgeLength_; }
      set { maxEdgeLength_ = Mathf.Clamp( value, 0.0001f, float.MaxValue ); }
    }

    [SerializeField]
    bool limitByMeshDimensions_ = true;
    public bool LimitByMeshDimensions
    {
      get { return limitByMeshDimensions_; }
      set { limitByMeshDimensions_ = value; }
    }

    [SerializeField]
    Mesh[] arrTessellatedMesh_;
    public Mesh[] ArrTessellatedMesh
    {
      get { return arrTessellatedMesh_; }
      set { arrTessellatedMesh_ = value; }
    }

    [SerializeField]
    GameObject[] arrTessellatedGO_;
    public GameObject[] ArrTessellatedGO
    {
      get { return arrTessellatedGO_; }
      set { arrTessellatedGO_ = value; }
    }

    [SerializeField]
    GameObject nodeGO_;
    public GameObject NodeGO
    {
      get { return nodeGO_; }
      set { nodeGO_ = value; }
    }

    protected override void CloneData(CommandNode node)
    {
      base.CloneData(node);
      CNTessellator clone = (CNTessellator) node;

      clone.maxEdgeLength_         = maxEdgeLength_;
      clone.limitByMeshDimensions_ = limitByMeshDimensions_;
    }

    public override CommandNode DeepClone(GameObject dataHolder)
    {
      CNTessellator clone = CommandNode.CreateInstance<CNTessellator>(dataHolder);
      CloneData(clone);
      return clone;
    }

  } //namespace CNWelder
}
