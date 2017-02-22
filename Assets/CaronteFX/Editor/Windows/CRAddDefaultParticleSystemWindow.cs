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
using UnityEditor.Events;
using System.Collections;
using System.Collections.Generic;

namespace CaronteFX
{

  public class CRAddDefaultParticleSystemWindow : CRWindow<CRAddDefaultParticleSystemWindow>
  {
    CRAnimation crAnimation_;

    public static CRAddDefaultParticleSystemWindow ShowWindow(CRAnimation crAnimation)
    {
      if (Instance == null)
      {
        Instance = (CRAddDefaultParticleSystemWindow)EditorWindow.GetWindow(typeof(CRAddDefaultParticleSystemWindow), true, "CaronteFX - Add default particle system");
        Instance.crAnimation_ = crAnimation;
      }

      float width  = 300f;
      float height = 50f;

      Instance.minSize = new Vector2(width, height);
      Instance.maxSize = new Vector2(width, height);

      Instance.Focus();
      return Instance;
    }

    public void DrawAddDefaultDust()
    {
      EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
      if (GUILayout.Button("Add default dust"))
      {    
        if (crAnimation_ != null)
        {
          AddDefaultDustParticlesEventsReceiver();
        }
      }
      EditorGUI.EndDisabledGroup();
    }

    private void AddDefaultDustParticlesEventsReceiver()
    {
      UnityEngine.Object prefabObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/CaronteFX/Scripts/RuntimeScripts/Animation/EventsExamples/DustExample/DustParticleEventsReceiver.prefab");
      UnityEngine.GameObject dustEventsReceiverGO = (GameObject)PrefabUtility.InstantiatePrefab(prefabObject);
      PrefabUtility.DisconnectPrefabInstance(dustEventsReceiverGO);
      dustEventsReceiverGO.transform.parent = crAnimation_.transform;

      CRDustEventsReceiver dustER = dustEventsReceiverGO.GetComponent<CRDustEventsReceiver>();
      UnityEventTools.AddPersistentListener(crAnimation_.animationEvent, dustER.ProcessAnimationEvent);
      EditorUtility.SetDirty(crAnimation_);
    }

    void OnLostFocus()
    {
      Close();
    }

    public void OnGUI()
    {
      if (crAnimation_ == null)
      {
        Close();
      }

      EditorGUILayout.Space();
      DrawAddDefaultDust();
      GUILayout.FlexibleSpace();
      EditorGUILayout.Space();
    }
  }

}