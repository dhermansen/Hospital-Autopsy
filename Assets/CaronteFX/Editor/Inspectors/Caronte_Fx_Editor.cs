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
  [CustomEditor(typeof(Caronte_Fx))]
  public class Caronte_Fx_Editor : Editor
  {

    public override void OnInspectorGUI()
    {
      Caronte_Fx fxData = target as Caronte_Fx;
      
      EditorGUILayout.Space();
      EditorGUILayout.Space();
      GUILayout.BeginHorizontal();  
      GUILayout.FlexibleSpace();
      if (GUILayout.Button("Open in CaronteFX Editor", GUILayout.Height(30f) ) )
      { 
        CRManagerEditor editor = (CRManagerEditor)CRManagerEditor.Init();
        editor.Controller.SetFxDataActive(fxData);
      }
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      EditorGUILayout.Space();
      EditorGUILayout.Space();
    }
  }
}