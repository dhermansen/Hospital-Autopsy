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

  public class CRMakeScreenshotsWindow : CRWindow<CRMakeScreenshotsWindow>
  {
    CRAnimation crAnimation_;

    public static CRMakeScreenshotsWindow ShowWindow(CRAnimation crAnimation)
    {
      if (Instance == null)
      {
        Instance = (CRMakeScreenshotsWindow)EditorWindow.GetWindow(typeof(CRMakeScreenshotsWindow), true, "CaronteFX - Make screenshots");
        Instance.crAnimation_ = crAnimation;
      }

      float width  = 250f;
      float height = 60f;

      Instance.minSize = new Vector2(width, height);
      Instance.maxSize = new Vector2(width, height);

      Instance.Focus();
      return Instance;
    }

    public void DrawMakeCameraScreenshots()
    {
      EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
      if ( GUILayout.Button(new GUIContent("Record screenshots in play mode") ) )
      {
        MakeCameraScreenshots();
      }
      if ( GUILayout.Button(new GUIContent("Remove camera recorder component") ) ) 
      {
        RemoveCameraRecorder();
      }
      EditorGUI.EndDisabledGroup();
    }


    public void MakeCameraScreenshots()
    {
      Camera mainCamera = Camera.main;

      if (mainCamera != null)
      {
        string folderPath = EditorUtility.SaveFolderPanel("CaronteFX - Select Folder", "", "");
        if (folderPath != string.Empty)
        {
          EditorApplication.isPlaying = true;

          GameObject go = mainCamera.gameObject;
          EditorGUIUtility.PingObject( go );

          CRCameraCapturer cameraCapturer = go.GetComponent<CRCameraCapturer>();
          if (cameraCapturer == null)
          {
            cameraCapturer = Undo.AddComponent<CRCameraCapturer>(go);
          }

          Undo.RecordObject( go, "Change camera capturer ");
          
          cameraCapturer.enabled     = true;
          cameraCapturer.cranimation = crAnimation_;
          cameraCapturer.folder      = folderPath;

          Undo.CollapseUndoOperations( Undo.GetCurrentGroup() );
        }
      }
    }

    public void RemoveCameraRecorder()
    {
      Camera mainCamera = Camera.main;
      if (mainCamera != null)
      {
        GameObject go = mainCamera.gameObject;
        CRCameraCapturer cameraCapturer = go.GetComponent<CRCameraCapturer>();
        if (cameraCapturer != null)
        {
          Undo.DestroyObjectImmediate(cameraCapturer);
          EditorGUIUtility.PingObject( go );
        }
      }
    }

    void OnLostFocus()
    {
      Close();
    }

    public void OnGUI()
    {
      EditorGUILayout.Space();
      GUILayout.FlexibleSpace();
      DrawMakeCameraScreenshots();
      EditorGUILayout.Space();
    }
  }
}