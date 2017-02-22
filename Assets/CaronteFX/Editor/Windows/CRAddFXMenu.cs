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
  public class CRAddFXMenu : CRWindow<CRAddFXMenu>
  {
    static float width_  = 450f;
    static float height_ = 350f;
    
    Vector2 scrollPos_;

    List< Tuple4<Caronte_Fx, int, bool, bool> > listCaronteFx_  = new List< Tuple4<Caronte_Fx, int, bool, bool> >();
    BitArray arrEffectsToInclude;

    CNManager cnManager_;

    public static CRAddFXMenu ShowWindow(CNManager cnManager)
    {
      if (Instance == null)
      {
        Instance = (CRAddFXMenu)EditorWindow.GetWindow( typeof(CRAddFXMenu), true, "CaronteFX - Subeffects menu");
        Instance.cnManager_ = cnManager;
        Instance.BuildEffectList();
      }

      Instance.minSize = new Vector2(width_, height_);
      Instance.maxSize = new Vector2(width_, height_);

      Instance.Focus();
      return Instance;
    }

    void OnLostFocus()
    {
      Close();
    }

    /// <summary>
    /// Builds a list with all the scene fx and their status(included/not included)
    /// </summary>
    void BuildEffectList()
    {
      List< Tuple2<Caronte_Fx, int> > listCaronteFx;
      CRDataUtils.GetCaronteFxsRelations( cnManager_.FxData, out listCaronteFx );

      for (int i = 0; i < listCaronteFx.Count; i++)
      {
        Caronte_Fx fx     = listCaronteFx[i].First;
        int depth         = listCaronteFx[i].Second;

        bool alreadyAdded = cnManager_.IsEffectIncluded(fx);
        bool wouldProduceInfiniteRecursion = cnManager_.RootNodeAlreadyContainedInNode(fx);

        listCaronteFx_.Add( Tuple4.New( fx, depth, alreadyAdded, wouldProduceInfiniteRecursion ) );
      }
      arrEffectsToInclude = new BitArray(listCaronteFx_.Count);
      for (int i = 0; i < arrEffectsToInclude.Length; i++)
      {
        arrEffectsToInclude[i] = listCaronteFx_[i].Third;
      }   
    }

    void OnGUI()
    {
      Rect effectsArea = new Rect( 5, 5, width_ - 10, (height_ - 10) * 0.9f );

      GUILayout.BeginArea(effectsArea, GUI.skin.box);
      EditorGUILayout.BeginHorizontal();
      {
        GUIStyle styleTitle = new GUIStyle(GUI.skin.label);
        styleTitle.fontStyle = FontStyle.Bold;
        GUILayout.Label("Scene FXs:", styleTitle);   
      }
      EditorGUILayout.EndHorizontal();

      CRGUIUtils.Splitter();
      EditorGUILayout.Space();


      scrollPos_ = GUILayout.BeginScrollView(scrollPos_);
      {
        for (int i = 0; i < listCaronteFx_.Count; i++)
        {
          Caronte_Fx fx = listCaronteFx_[i].First;
          bool cannotBeAdded = listCaronteFx_[i].Fourth;
          if (fx != cnManager_.FxData)
          {
            GUILayout.BeginHorizontal();
            {
              string name = fx.name;
              if (cannotBeAdded)
              {
                name += " - (Recursive inclusion detected)";
              }
              EditorGUI.BeginDisabledGroup(cannotBeAdded);
              arrEffectsToInclude[i] = EditorGUILayout.ToggleLeft(name, arrEffectsToInclude[i]);
              EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndHorizontal();
          }
        }
      }
      GUILayout.EndScrollView();

      GUILayout.EndArea();
      Rect buttonsArea = new Rect( effectsArea.xMin, effectsArea.yMax, width_ - 10, (height_ - 10) * 0.1f );
      
      GUILayout.BeginArea(buttonsArea);
      {
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        {
          GUILayout.FlexibleSpace();
          if (GUILayout.Button("Ok", GUILayout.Width(165f)))
          {
            Close();
            ModifyInclusions();    
          }
          GUILayout.FlexibleSpace();
          if (GUILayout.Button("Cancel", GUILayout.Width(165f)))
          {
            Close();
          }
          GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();
      }
      GUILayout.EndArea();
    }

    private void ModifyInclusions()
    {

      List<GameObject> listGameObjectFxToInclude = new List<GameObject>();
      List<GameObject> listGameObjectFxToDeinclude = new List<GameObject>();
      for (int i = 0; i < listCaronteFx_.Count; i++)
      {
        bool wasIncluded = listCaronteFx_[i].Third;
        bool cannotBeAdded = listCaronteFx_[i].Fourth;
        bool hasToBeIncluded = arrEffectsToInclude[i];

        if (wasIncluded != hasToBeIncluded)
        {
          Caronte_Fx fx = listCaronteFx_[i].First;
          if (hasToBeIncluded && !cannotBeAdded)
          {
            listGameObjectFxToInclude.Add(fx.gameObject);
          }
          else
          {
            listGameObjectFxToDeinclude.Add(fx.gameObject);
          }
        }
      }

      if (listGameObjectFxToDeinclude.Count > 0 || listGameObjectFxToInclude.Count > 0)
      {
        cnManager_.ModifyEffectsIncluded(listGameObjectFxToDeinclude, listGameObjectFxToInclude);    
      }
    }
  }
}

