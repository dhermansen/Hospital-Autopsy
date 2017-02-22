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
  public class CNAnimatedbodyEditor : CNRigidbodyEditor
  {
    static readonly GUIContent anTypeCt_                              = new GUIContent( CRStringManager.GetString("AnAnimationType"),                     CRStringManager.GetString("AnAnimationTypeTooltip") );
    static readonly GUIContent anOverrideAnimationControllerToggleCt_ = new GUIContent( CRStringManager.GetString("AnOverrideAnimationControllerToggle"), CRStringManager.GetString("AnOverrideAnimationControllerToggleTooltip") );
    static readonly GUIContent anAnimationClipCt_                     = new GUIContent( CRStringManager.GetString("AnAnimationClip"),                     CRStringManager.GetString("AnAnimationClipTooltip") );
    static readonly GUIContent anTimeStartValueCt_                    = new GUIContent( CRStringManager.GetString("AnTimeStart"),                         CRStringManager.GetString("AnTimeStartTooltip") );
    static readonly GUIContent anTimeLengthValueCt_                   = new GUIContent( CRStringManager.GetString("AnTimeLength"),                        CRStringManager.GetString("AnTimeLengthTooltip") );
    static readonly GUIContent anResetCt_                             = new GUIContent( CRStringManager.GetString("AnResetTimeLength") );
                      

    static readonly GUIContent[] arrAnimationTypeStringsCt_ = new GUIContent[2] { new GUIContent( CRStringManager.GetString("AnAnimationClipType") ), new GUIContent(CRStringManager.GetString("AnCaronteFXClipType")) };


    public static Texture icon_;
    public override Texture TexIcon{ get{ return icon_; } }

    List<Animator>    listAnimator_    = new List<Animator>();
    List<CRAnimation> listCRAnimation_ = new List<CRAnimation>();

    List<RuntimeAnimatorController> listRtAnimatorController_   = new List<RuntimeAnimatorController>();
    List<RuntimeAnimatorController> listOvrrAnimatorController_ = new List<RuntimeAnimatorController>();

    new public CNAnimatedbody Data { get; set; }

    public CNAnimatedbodyEditor(CNAnimatedbody data, CNRigidbodyEditorState state)
      : base( data, state )
    {
      Data = (CNAnimatedbody)data;
    }
    //-----------------------------------------------------------------------------------
    public CNAnimatedbody.EAnimationType getAnimationType()
    {
      return Data.AnimationType;
    }
    //-----------------------------------------------------------------------------------
    public GameObject[] GetAnimationGameObjects<T>()
      where T:Component
    {
      List<GameObject> listGameObject = new List<GameObject>();

      List<GameObject> listBodyObjects = GetGameObjects();
      foreach (var go in listBodyObjects)
      {
        if (go.HasMesh())
        {
          T animation = CREditorUtils.GetFirstComponentInHierarhcy<T>(go);
          if (animation != null)
          {
            GameObject animationGO = animation.gameObject;
            if ( !listGameObject.Contains(animationGO) )
            {
              listGameObject.Add(animationGO);
            }
          }
        }
      }
      return listGameObject.ToArray();
    }
    //-----------------------------------------------------------------------------------
    public override void FreeResources()
    {
      base.FreeResources();
    }
    //----------------------------------------------------------------------------------
    public override void LoadInfo()
    {
      base.LoadInfo();
    }
    //----------------------------------------------------------------------------------
    public override void StoreInfo()
    {
      base.StoreInfo();
    }
    //-----------------------------------------------------------------------------------
    public override void AddGameObjects( UnityEngine.Object[] draggedObjects, bool recalculateFields )
    {
      FieldController.AddGameObjects( draggedObjects, recalculateFields );
    }
    //-----------------------------------------------------------------------------------
    public override void CreateBodies( GameObject[] arrGameObject )
    {
      CreateBodies(arrGameObject, "Caronte FX - Animated body creation", "Creating " + Data.Name + " animated bodies. ");
    }
    //-----------------------------------------------------------------------------------
    public override void DestroyBodies(GameObject[] arrGameObject)
    {
      DestroyBodies(arrGameObject, "Caronte FX - Animated body destruction", "Destroying " + Data.Name + " animated bodies. ");
    }
    //----------------------------------------------------------------------------------
    private void SampleAnimationController( GameObject go )
    {
      Animator animator = CREditorUtils.GetFirstComponentInHierarhcy<Animator>(go);
      if (animator != null)
      {
        if (Data.UN_AnimationClip != null && Data.OverrideAnimationController)
        {           
          OverrideAnimatorController(animator);
        }
        animator.Update(0.0f);
      }
    }
    //----------------------------------------------------------------------------------
    private void SampleCaronteFX(GameObject go, out bool isVertexAnimated)
    {
      isVertexAnimated = false;
      CRAnimation crAnimation = CREditorUtils.GetFirstComponentInHierarhcy<CRAnimation>(go);
      if (crAnimation != null)
      {
        crAnimation.LoadAnimation(true);
        crAnimation.SetFrame(0.0f);

        isVertexAnimated = crAnimation.IsVertexAnimated(go);

        listCRAnimation_.Add(crAnimation);
      }
    }
    //----------------------------------------------------------------------------------
    private void OverrideAnimatorController(Animator animator)
    {
      RuntimeAnimatorController controller = animator.runtimeAnimatorController;
 
      AnimatorOverrideController ovrrController = new AnimatorOverrideController();

      listAnimator_              .Add(animator);
      listRtAnimatorController_  .Add(controller);
      listOvrrAnimatorController_.Add(ovrrController);

      ovrrController.runtimeAnimatorController = CarAnimatorInfo.animatorSampler_;
      animator.runtimeAnimatorController = ovrrController;

      AnimationClip[] clips = ovrrController.animationClips;
      foreach (AnimationClip animClip in clips)
      {
        ovrrController[animClip] = Data.UN_AnimationClip;
      }
    }
    //----------------------------------------------------------------------------------
    private void RestoreAnimatorController()
    {
      for (int i = 0; i < listRtAnimatorController_.Count; i++)
      {
        Animator animator              = listAnimator_[i];
        RuntimeAnimatorController rt   = listRtAnimatorController_[i];
        RuntimeAnimatorController ovrr = listOvrrAnimatorController_[i];

        animator.runtimeAnimatorController = rt;
        Object.DestroyImmediate(ovrr);
      }

      listAnimator_              .Clear();
      listRtAnimatorController_  .Clear();
      listOvrrAnimatorController_.Clear();
    }
    //----------------------------------------------------------------------------------
    private void RestoreCaronteFX()
    {
      for (int i = 0; i < listCRAnimation_.Count; i++)
      {
        CRAnimation crAnimation = listCRAnimation_[i];
        crAnimation.PreviewInEditor = false;
        crAnimation.CloseAnimation();
      }

      listCRAnimation_.Clear();
    }
    //----------------------------------------------------------------------------------
    protected override void ActionCreateBody(GameObject go)
    {
      if (Data.AnimationType == CNAnimatedbody.EAnimationType.Animator)
      {
        SampleAnimationController(go);
        bool isVertexAnimated = false;
        SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
        if (smr != null)
        {
          isVertexAnimated = true;
        }
        eManager.CreateBody(Data, go, isVertexAnimated);
        RestoreAnimatorController();
      }
      else if (Data.AnimationType == CNAnimatedbody.EAnimationType.CaronteFX)
      {
        bool isVertexAnimated;
        SampleCaronteFX(go, out isVertexAnimated);
        eManager.CreateBody(Data, go, isVertexAnimated);
        RestoreCaronteFX();
      }
    }
    //----------------------------------------------------------------------------------
    protected override void ActionDestroyBody(GameObject go)
    {
      eManager.DestroyBody(Data, go);
    }
    //----------------------------------------------------------------------------------
    protected override void ActionCheckBodyForChanges( GameObject go, bool recreateIfInvalid )
    {
      eManager.CheckBodyForChanges(Data, go, recreateIfInvalid);
    }
    //----------------------------------------------------------------------------------
    private void DrawAnimationClip()
    {
      EditorGUILayout.BeginHorizontal();

      EditorGUI.BeginChangeCheck();
      Data.OverrideAnimationController = EditorGUILayout.Toggle( anOverrideAnimationControllerToggleCt_, Data.OverrideAnimationController, GUILayout.Width(200f) );
      if ( EditorGUI.EndChangeCheck() )
      {
        EditorUtility.SetDirty(Data);
        EditorApplication.delayCall += RecreateBodies;   
      }

      if (GUILayout.Button("Refresh initial position", GUILayout.Width(200f) ))
      {
        EditorApplication.delayCall += RecreateBodies;   
      }
      EditorGUILayout.EndHorizontal();

      EditorGUI.BeginDisabledGroup( !Data.OverrideAnimationController );
      EditorGUI.BeginChangeCheck();
      Data.UN_AnimationClip = (AnimationClip)EditorGUILayout.ObjectField( anAnimationClipCt_, Data.UN_AnimationClip, typeof(AnimationClip), true );
      if ( EditorGUI.EndChangeCheck() )
      {
        EditorUtility.SetDirty(Data);
        EditorApplication.delayCall += RecreateBodies;  
      }
      EditorGUI.EndDisabledGroup();
    }
    //----------------------------------------------------------------------------------
    private void DrawTimeStart()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField( anTimeStartValueCt_, Data.TimeStart );
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject( Data, "Change time start");
        Data.TimeStart = Mathf.Clamp(value, 0f, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //----------------------------------------------------------------------------------
    private void DrawTimeLength()
    {
      EditorGUI.BeginChangeCheck();
      var timelengthValue = CRGUIExtension.FloatTextField( anTimeLengthValueCt_, Data.TimeLength, 0.0f, 10000.0f, float.MaxValue, "-");
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject( Data, "Change time length");
        Data.TimeLength = Mathf.Clamp(timelengthValue, 0f, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //----------------------------------------------------------------------------------
    private void DrawTimeLengthReset()
    {
      GUI.SetNextControlName("reset");
      if (GUILayout.Button( anResetCt_, EditorStyles.miniButton, GUILayout.Width(50f)))
      {
        GUI.FocusControl("reset");
        Undo.RecordObject( Data, "Change time length");
        Data.TimeLength = float.MaxValue;
        EditorUtility.SetDirty(Data);
      }
    }
    //----------------------------------------------------------------------------------
    private void DrawAnimationTypeEnum()
    {
      EditorGUI.BeginChangeCheck();
      var animationTypeInt = EditorGUILayout.Popup( anTypeCt_, (int)Data.AnimationType, arrAnimationTypeStringsCt_ );
      if (EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change animation type");
        Data.AnimationType = (CNAnimatedbody.EAnimationType)animationTypeInt;
        EditorApplication.delayCall += RecreateBodies;   
        EditorUtility.SetDirty(Data);
      }
    }
    //----------------------------------------------------------------------------------
    protected override void RenderFieldsBody(bool isEditable)
    {
      EditorGUILayout.Space();
      EditorGUILayout.Space();

      CRGUIUtils.DrawSeparator();
      CRGUIUtils.Splitter();

      float originalLabelWidth = EditorGUIUtility.labelWidth;
      EditorGUIUtility.labelWidth = label_width;

      scroller_ = EditorGUILayout.BeginScrollView(scroller_);
      EditorGUI.BeginDisabledGroup( !isEditable );
      EditorGUILayout.Space();

      DrawDoCollide();
      EditorGUILayout.Space();
      
      EditorGUILayout.Space();
      EditorGUILayout.Space();

      DrawAnimationTypeEnum();
      EditorGUILayout.Space();

      if ( Data.AnimationType == CNAnimatedbody.EAnimationType.Animator )
      {
        DrawAnimationClip();
      }

      EditorGUILayout.Space();
      EditorGUILayout.Space();

      DrawTimeStart();

      GUILayout.BeginHorizontal();
      DrawTimeLength();
      DrawTimeLengthReset();
      GUILayout.EndHorizontal();
      EditorGUILayout.Space();

      CRGUIUtils.Splitter();
      EditorGUILayout.Space();
      EditorGUILayout.Space();

      DrawRestitution();
      DrawFrictionKinetic();

      GUILayout.BeginHorizontal();
      EditorGUI.BeginDisabledGroup(Data.FromKinetic);
      DrawFrictionStatic();
      EditorGUI.EndDisabledGroup();
      DrawFrictionStaticFromKinetic();
      GUILayout.EndHorizontal();

      CRGUIUtils.Splitter();
      EditorGUILayout.Space();

      DrawExplosionOpacity();
      CRGUIUtils.Splitter();
      EditorGUILayout.Space();

      EditorGUIUtility.labelWidth = originalLabelWidth;

      EditorGUI.EndDisabledGroup();
      EditorGUILayout.EndScrollView();
    }
  }


}

