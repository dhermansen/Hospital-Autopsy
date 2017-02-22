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
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNSelector")] 
  public class CNSelector : CNMonoField
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
    
    public enum SELECTION_MODE
    {
      INSIDE,
      OUTSIDE
    };

    [SerializeField]
    GameObject selectorGO_;
    public GameObject SelectorGO
    {
      get { return selectorGO_; }
      set { selectorGO_ = value; }
    }

    [SerializeField]
    private SELECTION_MODE selectionMode_ = SELECTION_MODE.INSIDE;
    public SELECTION_MODE SelectionMode
    {
      get { return selectionMode_; }
      set { selectionMode_ = value; }
    }

    [SerializeField]
    private bool frontierPieces_ = true;
    public bool FrontierPieces
    {
      get { return frontierPieces_; }
      set { frontierPieces_ = value; }
    }

    [SerializeField]
    private bool complementary_ = false;
    public bool Complementary
    {
      get { return complementary_; }
      set { complementary_ = value; }
    }

    public override CommandNode DeepClone(GameObject dataHolder)
    {
      CNSelector clone = CommandNode.CreateInstance<CNSelector>(dataHolder); 
      CloneData(clone);
      return clone;
    }

  } //namespace CNWelder
}

