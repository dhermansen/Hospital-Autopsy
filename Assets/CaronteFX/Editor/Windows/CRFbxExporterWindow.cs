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
using System.Collections.Generic;

namespace CaronteFX
{

  public class CRFbxExporterWindow : CRWindow<CRFbxExporterWindow>
  {
    CRAnimation crAnimation_;
    bool startAnimationInFrame1_ = true;
    bool exportVisibilityAsScale_ = true;

    public static CRFbxExporterWindow ShowWindow(CRAnimation crAnimation)
    {
      if (Instance == null)
      {
        Instance = (CRFbxExporterWindow)EditorWindow.GetWindow(typeof(CRFbxExporterWindow), true, "CaronteFX - Fbx Exporter");
        Instance.crAnimation_ = crAnimation;
      }

      float width  = 300f;
      float height = 100f;

      Instance.minSize = new Vector2(width, height);
      Instance.maxSize = new Vector2(width, height);

      Instance.Focus();
      return Instance;
    }

    public void DrawExportToFbx()
    {
      EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
      if (GUILayout.Button("Export FBX with Global Skinning..."))
      {
        string filePath = EditorUtility.SaveFilePanel("Export FBX with Global Skinning...", Application.dataPath, "", "fbx");
        if (filePath != string.Empty)
        {
          CRFbxCollapsedExporter fbxExporter = new CRFbxCollapsedExporter();
          fbxExporter.ExportToFbx(filePath, crAnimation_.gameObject, crAnimation_, startAnimationInFrame1_, exportVisibilityAsScale_);
        }
        else
        {
          Debug.Log("Invalid fbx filepath");
        }
      }
      if (GUILayout.Button("Export FBX with Current Skinning..."))
      {
        string filePath = EditorUtility.SaveFilePanel("Export FBX with Current Skinning...)", Application.dataPath, "", "fbx");
        if (filePath != string.Empty)
        {
          CRFbxExporter fbxExporter = new CRFbxExporter();
          fbxExporter.ExportToFbx(filePath, crAnimation_.gameObject, crAnimation_, false, startAnimationInFrame1_, exportVisibilityAsScale_);
        }
        else
        {
          Debug.Log("Invalid fbx filepath");
        }
      }

      EditorGUI.EndDisabledGroup();
    }

    private void DrawStartInFirstFrame()
    {
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField("Start animations in frame 1", GUILayout.Width(170f));
      startAnimationInFrame1_ = EditorGUILayout.Toggle(startAnimationInFrame1_, GUILayout.Width(10f));
      EditorGUILayout.EndHorizontal(); 
    }

    private void DrawExportVisibilityAsScale()
    {
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField("Export visibility as scale", GUILayout.Width(170f));
      exportVisibilityAsScale_ = EditorGUILayout.Toggle(exportVisibilityAsScale_, GUILayout.Width(10f));
      EditorGUILayout.EndHorizontal(); 
    }

    void OnLostFocus()
    {
      Close();
    }

    public void OnGUI()
    {
      EditorGUILayout.Space();
      DrawStartInFirstFrame();
      DrawExportVisibilityAsScale();
      GUILayout.FlexibleSpace();
      DrawExportToFbx();
      EditorGUILayout.Space();
    }
  }

}