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
using System;

namespace CaronteFX
{

  public class CRAboutWindow: CRWindow<CRAboutWindow>
  {
    [Flags]
    public enum VersionType
    {
      Free,
      Evaluation,
      Pro,
      Premium
    }

    public enum CompressionType
    {
      Normal,
      Advanced
    }
    
    string companyName_;
    Texture companyIcon_;
    string versionString_;

    VersionType versionType_;
    CompressionType compressionType_; 
    DateTime endTrialDateTime_;

    public static CRAboutWindow ShowWindow()
    {
      if (Instance == null)
      {
        Instance = (CRAboutWindow)EditorWindow.GetWindow(typeof(CRAboutWindow), true, "About CaronteFX");
      }

      float width = 380f;
      float height = 210f;

      Instance.minSize = new Vector2(width, height);
      Instance.maxSize = new Vector2(width, height);
    
      Instance.Focus();    
      return Instance;
    }

    void OnEnable()
    {
      string version = CaronteSharp.Caronte.GetNativeDllVersion();

      string versionTypeName;
      if ( CRVersionChecker.IsPremiumVersion() && CRVersionChecker.IsEvaluationVersion() )
      {
        versionTypeName = " PREMIUM TRIAL";
        versionType_ = VersionType.Premium & VersionType.Evaluation;
        endTrialDateTime_ = CRVersionChecker.GetEvaluationPeriodEndDate();
      }
      else if (CRVersionChecker.IsPremiumVersion() )
      {
        versionTypeName = " PREMIUM";
        versionType_ = VersionType.Premium;
      }
      else if (CRVersionChecker.IsFreeVersion() )
      {
        versionTypeName = " FREE";
        versionType_ = VersionType.Free;
      }
      else if (CRVersionChecker.IsEvaluationVersion() )
      {
        versionTypeName = " PRO TRIAL";
        versionType_ = VersionType.Evaluation;
        endTrialDateTime_ = CRVersionChecker.GetEvaluationPeriodEndDate();
      }
      else
      {
        versionTypeName = " PRO";
        versionType_ = VersionType.Pro;
      }

      if (CRVersionChecker.IsAdvanceCompressionVersion())
      {
        compressionType_ = CompressionType.Advanced;
      }
      else
      {
        compressionType_ = CompressionType.Normal;
      }

      companyIcon_ = CREditorResource.LoadEditorTexture(CRVersionChecker.CompanyIconName);
      versionString_ = "Version: " + version + versionTypeName + " \n(Compression type: " + compressionType_.ToString() + ")";
    }

    void OnLostFocus()
    {
      Close();
    }

    public void OnGUI()
    {
      GUI.DrawTexture(new Rect(-30f, -10f, 230f, 230f), CRManagerEditor.ic_logoCaronte_);

      GUILayout.BeginArea( new Rect( 180f, 5f, 195f, 210f ) );   
      GUILayout.FlexibleSpace();

      if (versionType_ == VersionType.Evaluation)
      {
        GUILayout.Label( new GUIContent("EVALUATION version.\n\nAny commercial use, \ncopying, or redistribution of \nthis plugin is strictly forbidden.\n" ), EditorStyles.miniLabel );
        GUILayout.Label( new GUIContent("Trial expiration date is:\n\n" + endTrialDateTime_.ToShortDateString() + " (m/d/y)"), EditorStyles.miniLabel );
      }
      if (CRVersionChecker.CompanyName != string.Empty)
      {
        GUILayout.Label( new GUIContent("This version is exclusive for " + CRVersionChecker.CompanyName + "\ninternal use.\n"), EditorStyles.miniLabel );
        GUILayout.Label( new GUIContent(companyIcon_), GUILayout.MaxWidth(69.7f), GUILayout.MaxHeight(32f) );
      }

      GUILayout.FlexibleSpace();

      GUILayout.Label( new GUIContent("Powered by Caronte physics engine."), EditorStyles.miniLabel );
      GUILayout.Label( new GUIContent("(c) 2016 Next Limit Technologies."), EditorStyles.miniLabel );
      GUILayout.Label( new GUIContent( versionString_ ), EditorStyles.miniLabel );

      EditorGUILayout.Space();

      GUILayout.EndArea();
    }

  }
}
