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
using CaronteSharp;


namespace CaronteFX
{
  public class CRTimeSliderView
  {
    GUIStyle maxFramesStyle_;
    CRPlayer player_;

    int frameRequest_;
    int lastFrameRequest_;

    public CRTimeSliderView( CRPlayer player )
    {
      player_ = player;
      maxFramesStyle_ = new GUIStyle(EditorStyles.label);
      maxFramesStyle_.alignment = TextAnchor.MiddleRight;
      lastFrameRequest_ = -1;
    }

    public void RenderGUI()
    {
        EditorGUI.BeginDisabledGroup( player_.IsSimulating );
        EditorGUI.BeginChangeCheck();
        frameRequest_ = EditorGUILayout.IntSlider( player_.Frame, 0, player_.MaxFrames, 
                                                   GUILayout.ExpandWidth(true), GUILayout.Height(20f) );
        if (EditorGUI.EndChangeCheck() && ( lastFrameRequest_ != frameRequest_ ) )
        {
          lastFrameRequest_ = frameRequest_;
          player_.SetPauseFrame(frameRequest_, true);
        }
        GUILayout.Space(5f);
        EditorGUILayout.LabelField("of ", GUILayout.Width(20f), GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField(player_.MaxFramesString, maxFramesStyle_, GUILayout.Width(45f) ); 
        EditorGUI.EndDisabledGroup();
    }
  }
}
