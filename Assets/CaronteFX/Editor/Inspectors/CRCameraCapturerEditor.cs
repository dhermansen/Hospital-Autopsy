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

namespace CaronteFX
{
  [CustomEditor(typeof(CRCameraCapturer))]
  public class CRCameraCapturerEditor : Editor
  {
    CRCameraCapturer capturer_;

    void OnEnable()
    {
      capturer_ = (CRCameraCapturer)target;
    }

    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      if ( GUILayout.Button("Open screen shots folder") )
      {
        EditorUtility.RevealInFinder(capturer_.folder);
      }
    }
  }
}

