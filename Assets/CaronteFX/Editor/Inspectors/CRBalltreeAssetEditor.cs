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
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace CaronteFX
{
  [CustomEditor(typeof(CRBalltreeAsset))]
  public class CRBalltreeAssetEditor : Editor
  {
    CRBalltreeAsset btAsset_;

    void OnEnable()
    {
      btAsset_ = (CRBalltreeAsset)target;
    }

    public override void OnInspectorGUI()
    {
      EditorGUILayout.LabelField("CaronteFX - Balltree Asset");
      CRGUIUtils.Splitter();
      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Number of spheres: ", btAsset_.LeafSpheres.Length.ToString() );
    }

  }
}
