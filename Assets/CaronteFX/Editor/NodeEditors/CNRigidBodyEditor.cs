using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


namespace CaronteFX
{
  public class CNRigidbodyEditorState : CNBodyEditorState
  {
    public bool useballTree_;
    public int  ballTreeLOD_;
  }

  public class CNRigidbodyEditor : CNBodyEditor
  {
    public static Texture icon_responsive_;
    public static Texture icon_irresponsive_;

    public override Texture TexIcon 
    { 
      get
      { 
        if (Data.IsFiniteMass)
        {
          return icon_responsive_;
        }
        else
        {
         return icon_irresponsive_; 
        }
      }
    }

    new CNRigidbodyEditorState state_;
    new CNRigidbody Data { get; set; }

    //-----------------------------------------------------------------------------------
    public CNRigidbodyEditor( CNRigidbody data, CNRigidbodyEditorState state )
      : base( data, state )
    {
      Data = (CNRigidbody)data;
      state_ = state;
    }
    //-----------------------------------------------------------------------------------
    protected override void LoadState()
    {
      base.LoadState();

      state_.useballTree_ = Data.UseBallTree;
      state_.ballTreeLOD_ = Data.BalltreeLOD;
    }
    //-----------------------------------------------------------------------------------
    public override void ValidateState()
    {
      base.ValidateState();

      ValidateBallTreeParams();
      EditorUtility.ClearProgressBar();
    }
    //-----------------------------------------------------------------------------------
    protected override void ValidateVelocity()
    {
      if ( state_.velocityStart_ != Data.VelocityStart )
      {
        eManager.SetVelocity( Data );
        Debug.Log("Changed linear velocity");
        state_.velocityStart_ = Data.VelocityStart;
      }
    }
    //-----------------------------------------------------------------------------------
    protected override void ValidateOmega()
    {
      if ( state_.omegaStart_inDegSeg_ != Data.OmegaStart_inDegSeg )
      {
        eManager.SetVelocity( Data );
        Debug.Log("Changed angular velocity");
        state_.omegaStart_inDegSeg_ = Data.OmegaStart_inDegSeg;
      }
    }
    //-----------------------------------------------------------------------------------
    private void ValidateBallTreeParams()
    {
      if ( state_.useballTree_ != Data.UseBallTree ||
           state_.ballTreeLOD_ != Data.BalltreeLOD )
      {
        RecreateBodies();
        Debug.Log("Changed use BallTree");  
        state_.useballTree_ = Data.UseBallTree;
        state_.ballTreeLOD_ = Data.BalltreeLOD;
      }
    }
    //-----------------------------------------------------------------------------------
    public void SetResponsiveness( bool responsive )
    {
      if (Data.IsFiniteMass != responsive )
      {
        Data.IsFiniteMass = responsive;

        if (!IsExcludedInHierarchy)
        {      
          GameObject[] arrGameObject = FieldController.GetUnityGameObjects();
          eManager.SetResponsiveness( Data, arrGameObject );   
        }

        EditorUtility.SetDirty( Data );
      }
    }
    //-----------------------------------------------------------------------------------
    public override void CreateBodies( GameObject[] arrGameObject )
    {
      CreateBodies(arrGameObject, "Caronte FX - Rigidbody creation", "Creating " + Data.Name + " rigidbodies. ");
    }
    //-----------------------------------------------------------------------------------
    public override void DestroyBodies(GameObject[] arrGameObject)
    {
      DestroyBodies(arrGameObject, "Caronte FX - Rigidbody destruction", "Destroying " + Data.Name + " rigidbodies. ");
    }
    //-----------------------------------------------------------------------------------
    public override void DestroyBodies(int[] arrInstanceId)
    {
      DestroyBodies(arrInstanceId, "Caronte FX - Rigidbody destruction", "Destroying " + Data.Name + " rigidbodies. ");
    }
    //-----------------------------------------------------------------------------------
    protected override void ActionCreateBody( GameObject go )
    {
      eManager.CreateBody(Data, go );
    }
    //-----------------------------------------------------------------------------------
    protected override void ActionDestroyBody( GameObject go)
    {
      eManager.DestroyBody(Data, go);
    }
    //-----------------------------------------------------------------------------------
    protected override void ActionDestroyBody( int instanceId )
    {
      eManager.DestroyBody(Data, instanceId);
    }
    //-----------------------------------------------------------------------------------
    protected override void ActionCheckBodyForChanges( GameObject go, bool recreateIfInvalid )
    {
      eManager.CheckBodyForChanges(Data, go, recreateIfInvalid);
    }
    //-----------------------------------------------------------------------------------
    protected void DrawUseBallTree()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle( new GUIContent("Use BallTree"), Data.UseBallTree );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change use balltree - " + Data.Name);
        Data.UseBallTree = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawBallTreeLOD()
    {
      EditorGUI.BeginDisabledGroup(!Data.UseBallTree);
      EditorGUI.BeginChangeCheck();
      
      var value = EditorGUILayout.IntSlider( new GUIContent("BallTree LOD"), Data.BalltreeLOD, 0, 2 );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change baltree lod - " + Data.Name);
        Data.BalltreeLOD = value;
        EditorUtility.SetDirty(Data);
      }
      EditorGUI.EndDisabledGroup();
    }
    //-----------------------------------------------------------------------------------
    protected override void RenderFieldsBody(bool isEditable)
    {
      EditorGUILayout.Space();
      EditorGUILayout.Space();

      CRGUIUtils.DrawSeparator();
      CRGUIUtils.Splitter();

      float originalLabelWidth = EditorGUIUtility.labelWidth;
      EditorGUIUtility.labelWidth = label_width;

      scroller_ = EditorGUILayout.BeginScrollView(scroller_);
      EditorGUI.BeginDisabledGroup(!isEditable);
      EditorGUILayout.Space();
      DrawDoCollide();
      EditorGUILayout.Space();


      if (Data.IsFiniteMass)
      {
        DrawGUIMassOptions();
        DrawIsShell();
      }

      GUILayout.Space(simple_space);

      DrawRestitution();
      DrawFrictionKinetic();

      GUILayout.BeginHorizontal();
      EditorGUI.BeginDisabledGroup(Data.FromKinetic);
      DrawFrictionStatic();
      EditorGUI.EndDisabledGroup();
      DrawFrictionStaticFromKinetic();    
      GUILayout.EndHorizontal();

      GUILayout.Space(simple_space);

      if (Data.IsFiniteMass)
      {
        DrawDampingPerSecondWorld();
      }

      GUILayout.Space(simple_space);
      bool currentMode = EditorGUIUtility.wideMode;
      EditorGUIUtility.wideMode = true;

      DrawLinearVelocity();
      DrawAngularVelocity();

      EditorGUILayout.Space();
      CRGUIUtils.Splitter();
      EditorGUILayout.Space();

      DrawExplosionOpacity();

      if (Data.IsFiniteMass)
      {
        DrawExplosionResponsiveness();
      }
      
      CRGUIUtils.Splitter();
      EditorGUILayout.Space();

      EditorGUIUtility.labelWidth = originalLabelWidth;
      EditorGUIUtility.wideMode = currentMode;

      EditorGUI.EndDisabledGroup();
       
      EditorGUILayout.EndScrollView();
    }
    //-----------------------------------------------------------------------------------

  }

}
