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
  public class CRPlayerWindow : CRWindow<CRPlayerWindow>
  {
    CRPlayer        player_;
    CRPlayerView    playerView_;
    CRPlayer.Status status_;

    bool forcedExit_ = false;

    //-----------------------------------------------------------------------------------
    private const float width_  = 560f;
    private const float height_ = 165f;
    //-----------------------------------------------------------------------------------
    void OnEnable()
    {

    }
    //-----------------------------------------------------------------------------------
    void OnDisable()
    {
      if ( !(EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) )
      {
        if (!forcedExit_)
        {
          bool resetRequested = player_.ChangeToEditModeRequest();
          if (!resetRequested)
          {
            EditorApplication.delayCall += () => { ShowWindow(player_); };
          }
        }
      }
    }
    //-----------------------------------------------------------------------------------
    void Update()
    {
      player_.UpdateTimeSlider();

      if (status_ != player_.CurrentStatus)
      {
        titleContent = new GUIContent("CaronteFX Player - " + player_.GetStatusString());
        status_ = player_.CurrentStatus;
      }
    }
    //-----------------------------------------------------------------------------------
    public static void InstanceWillClose()
    {
      Instance = null;
    }
    //-----------------------------------------------------------------------------------
    public static void SetForcedExit()
    {
      if (Instance != null)
      {
        Instance.forcedExit_ = true;
      }
    }
    //-----------------------------------------------------------------------------------
    public static CRPlayerWindow ShowWindow( CRPlayer player )
    {
      if (Instance == null)
      {
        Instance = (CRPlayerWindow)EditorWindow.GetWindow(typeof(CRPlayerWindow), true, "CaronteFx Player - " + player.GetStatusString(), true);
        Instance.player_     = player;
        Instance.playerView_ = new CRPlayerView(player);
      }
      Instance.minSize = new Vector2(width_, height_);
      Instance.maxSize = new Vector2(width_, height_);
      Instance.status_ = player.CurrentStatus;
      Instance.Focus();
      
      return Instance;
    }
    //-----------------------------------------------------------------------------------
    void OnGUI()
    {
      playerView_.RenderGUI( new Rect(0, 0, position.width, position.height), true );
      Instance.titleContent = new GUIContent( "CaronteFX Player - " + player_.GetStatusTitle() );
    }
    //-----------------------------------------------------------------------------------
  }
}
