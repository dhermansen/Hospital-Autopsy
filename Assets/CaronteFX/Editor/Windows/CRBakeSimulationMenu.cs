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
  public class CRBakeSimulationMenu : CRWindow<CRBakeSimulationMenu>
  {
    CRBakerGOAnim   simulationBaker_;

    float width  = 350f;
    float height = 690f;

    Vector2    scroller_;

    List<CNBody> listBodyNode_;

    BitArray bitArrNeedsBaking_;
    BitArray bitArrNeedsCollapsing_;

    private string[] arrVertexCompressionModes_;

    CNManager Controller { get; set; }

    int StartFrame
    {
      get
      {
        return simulationBaker_.FrameStart;
      }
    }
   
    int EndFrame
    {
      get
      {
        return simulationBaker_.FrameEnd;
      }
    }

    int MaxFrames
    {
      get { return simulationBaker_.MaxFrames; }
    }

    void OnEnable()
    {
      Instance = this;

      this.minSize = new Vector2(width, height);
      this.maxSize = new Vector2(width, height);

      if (CRVersionChecker.IsAdvanceCompressionVersion())
      {
        arrVertexCompressionModes_ = new string[2] { "Box (medium compression)", "Fiber (high compression)" };
      }
      else
      {
        arrVertexCompressionModes_ = new string[1] { "Box (medium compression)" };
      }

      Controller = CNManager.Instance;
      Controller.Player.pause();

      simulationBaker_ = Controller.SimulationBaker;

      listBodyNode_          = simulationBaker_.listBodyNode_;

      bitArrNeedsBaking_     = simulationBaker_.bitArrNeedsBaking_;
      bitArrNeedsCollapsing_ = simulationBaker_.bitArrNeedsCollapsing_;
    }

    void SetStartEndFrames(int startFrame, int endFrame)
    {
      simulationBaker_.FrameStart = Mathf.Clamp(startFrame, 0, MaxFrames);
      simulationBaker_.FrameEnd   = Mathf.Clamp(endFrame,  0, MaxFrames);
    }

    void OnGUI()
    {
      Rect nodesArea    = new Rect( 5, 5, width - 10, ( (height - 10) / 2 ) ); 
      Rect nodesAreaBox = new Rect( nodesArea.xMin, nodesArea.yMin, nodesArea.width + 1, nodesArea.height + 1 );
      GUI.Box(nodesAreaBox, "");

      GUILayout.BeginArea(nodesArea);
      GUILayout.BeginHorizontal();

      GUIStyle styleTitle = new GUIStyle(GUI.skin.label);
      styleTitle.fontStyle = FontStyle.Bold;

      GUILayout.Label( "Nodes to bake:", styleTitle);
      GUILayout.EndHorizontal();

      CRGUIUtils.DrawSeparator();

      GUILayout.BeginHorizontal();

      int bodyNodeCount = listBodyNode_.Count;
      EditorGUILayout.BeginHorizontal();
      DrawToggleMixed( bodyNodeCount );
      Rect rect = GUILayoutUtility.GetLastRect();
      GUILayout.Space( 40f );
      EditorGUILayout.LabelField("Collapse/Skin");
      EditorGUILayout.EndHorizontal();
      GUILayout.FlexibleSpace();

      GUILayout.EndHorizontal();
      
      Rect boxRect      = new Rect( nodesAreaBox.xMin - 5, rect.yMax, nodesAreaBox.width - 90f, nodesAreaBox.yMax - rect.yMax );
      Rect collapseRect = new Rect( boxRect.xMax, boxRect.yMin, 90f, boxRect.height );

      GUI.Box(boxRect, "");
      GUI.Box(collapseRect, "");

      scroller_ = GUILayout.BeginScrollView(scroller_);
      
      for (int i = 0; i < bodyNodeCount; i++)
      {
        GUILayout.BeginHorizontal();
        CNBody bodyNode = listBodyNode_[i];
        string name = bodyNode.Name;

        bitArrNeedsBaking_[i] = EditorGUILayout.ToggleLeft(name, bitArrNeedsBaking_[i], GUILayout.Width(250f) );
        GUILayout.Space(35f);

        bool isRigid    = bodyNode is CNRigidbody;
        bool isAnimated = bodyNode is CNAnimatedbody;

        if (isRigid && !isAnimated)
        {
          bitArrNeedsCollapsing_[i] = EditorGUILayout.Toggle(bitArrNeedsCollapsing_[i]);
        }
        
        GUILayout.EndHorizontal();
      }

      EditorGUILayout.EndScrollView();

      GUILayout.EndArea();
      GUILayout.FlexibleSpace();

      Rect optionsArea = new Rect( 5, nodesArea.yMax, width - 10, (height - 10) / 2 );
      GUILayout.BeginArea(optionsArea);
      
      EditorGUILayout.Space();
      EditorGUILayout.Space();

      float start = EditorGUILayout.IntField("Frame Start : ", StartFrame );
      float end   = EditorGUILayout.IntField("Frame End   : ", EndFrame );

      EditorGUILayout.MinMaxSlider( new GUIContent("Frames:"), ref start, ref end, 0, MaxFrames );

      SetStartEndFrames( (int)start, (int)end );

      EditorGUILayout.Space();

      GUILayout.BeginHorizontal();
      simulationBaker_.animationFileType_ = (CRBakerGOAnim.AnimationFileType)EditorGUILayout.EnumPopup("File type", simulationBaker_.animationFileType_);
      GUILayout.EndHorizontal();

      EditorGUILayout.Space();

      GUILayout.BeginHorizontal();
      simulationBaker_.skinRopes_ = EditorGUILayout.Toggle("Skin ropes", simulationBaker_.skinRopes_);
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      simulationBaker_.skinClothes_ = EditorGUILayout.Toggle("Skin clothes", simulationBaker_.skinClothes_);
      GUILayout.EndHorizontal();

      EditorGUILayout.Space();

      GUILayout.BeginHorizontal();
      simulationBaker_.vertexCompression_ = EditorGUILayout.Toggle("Vertex compression", simulationBaker_.vertexCompression_);
      GUILayout.EndHorizontal();

      EditorGUI.BeginDisabledGroup(!simulationBaker_.vertexCompression_);
      simulationBaker_.vertexCompressionIdx_ = EditorGUILayout.Popup( "Compression mode", simulationBaker_.vertexCompressionIdx_, arrVertexCompressionModes_);
      EditorGUI.EndDisabledGroup();

      bool isFiberCompression = simulationBaker_.vertexCompression_ && simulationBaker_.vertexCompressionIdx_ == 1;
      EditorGUI.BeginDisabledGroup(isFiberCompression);
      GUILayout.BeginHorizontal();
      simulationBaker_.vertexTangents_ = EditorGUILayout.Toggle("Save tangents", simulationBaker_.vertexTangents_);
      GUILayout.EndHorizontal();
      EditorGUI.EndDisabledGroup();

      GUILayout.BeginHorizontal();
      simulationBaker_.alignData_ = EditorGUILayout.Toggle("Align data", simulationBaker_.alignData_);
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      simulationBaker_.bakeEvents_ = EditorGUILayout.Toggle("Save events", simulationBaker_.bakeEvents_);
      GUILayout.EndHorizontal();

      //simulationBaker_.bakeVisibility_ = EditorGUILayout.Toggle("Save visibility", simulationBaker_.bakeVisibility_);
      
      EditorGUILayout.Space();

      GUILayout.BeginHorizontal();
      simulationBaker_.animationName_ = EditorGUILayout.TextField("Animation name", simulationBaker_.animationName_);
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      simulationBaker_.bakeObjectName_ = EditorGUILayout.TextField("Root gameobject name", simulationBaker_.bakeObjectName_ );
      GUILayout.EndHorizontal();

      EditorGUILayout.Space();

      GUILayout.BeginHorizontal();
      simulationBaker_.optionalRootTransform_ = (Transform) EditorGUILayout.ObjectField("Optional root transform", simulationBaker_.optionalRootTransform_, typeof(Transform), true );
      GUILayout.EndHorizontal();

      EditorGUILayout.Space();
      EditorGUILayout.Space();
      if (GUILayout.Button("Bake!"))
      {   
        if (simulationBaker_.FrameEnd <= simulationBaker_.FrameStart)
        {
          EditorUtility.DisplayDialog("CaronteFX - Invalid frames", "Frame End must be above Frame Start", "Ok");
          return;
        }
        EditorApplication.delayCall += () => 
        { 
          simulationBaker_.BakeSimulationAsAnim(); 
          Close();
        };
      }

      GUILayout.BeginHorizontal();

      GUILayout.EndHorizontal();

      GUILayout.EndArea();
    }

   void DrawToggleMixed( int bodyNodeCount )
   {
     EditorGUI.BeginChangeCheck();
     if (bodyNodeCount > 0)
     {
       bool value = bitArrNeedsBaking_[0];
       for (int i = 1; i < bodyNodeCount; ++i)
       {
         if ( value != bitArrNeedsBaking_[i] )
         {
           EditorGUI.showMixedValue = true;
           break;
         }
       }
       simulationBaker_.bakeAllNodes_ = value;
     }

     simulationBaker_.bakeAllNodes_ = EditorGUILayout.ToggleLeft("All", simulationBaker_.bakeAllNodes_);
     EditorGUI.showMixedValue = false;
     if (EditorGUI.EndChangeCheck())
     {
       for (int i = 0; i < bodyNodeCount; ++i)
       {
         bitArrNeedsBaking_[i] = simulationBaker_.bakeAllNodes_;
       }
     }
     EditorGUI.showMixedValue = false;
   }
  }
}

