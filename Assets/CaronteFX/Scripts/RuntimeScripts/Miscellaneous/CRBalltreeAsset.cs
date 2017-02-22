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
  /// Balltree asset to avoid recreation of balltrees if it's not needed.
  /// </summary>
  public class CRBalltreeAsset : ScriptableObject 
  {
    [SerializeField]
    int version_ = 0;
    public int Version
    {
      get { return version_; }
    }

    [SerializeField]
    private byte[] checkHeaderBytes_;
    public byte[] CheckHeaderBytes
    {
      get { return checkHeaderBytes_; }
      set { checkHeaderBytes_ = value; }
    }

    [SerializeField]
    private byte[] balltreeBytes_;
    public byte[] BalltreeBytes
    {
      get { return balltreeBytes_; }
      set { balltreeBytes_ = value; }
    }

    [SerializeField]
    private CRSphere[] arrLeafSphere_;
    public CRSphere[] LeafSpheres
    {
      get { return arrLeafSphere_; }
      set { arrLeafSphere_ = value; }
    }
  }
}
