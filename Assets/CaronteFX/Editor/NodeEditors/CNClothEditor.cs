using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CaronteFX
{
  public class CNClothEditorState : CNBodyEditorState
  {
    public float cloth_bend_;                
    public float cloth_stretch_;             
    public float cloth_dampingBend_;     
    public float cloth_dampingStretch_;  
    public float cloth_collisionRadius_;
    public bool  disableCollisionNearJoints_;
    public bool  safeMode_integration_;
    public bool  useColliderUVs_;
  }

  public class CNClothEditor : CNBodyEditor
  {
    static readonly GUIContent clCollisionRadiusCt_           = new GUIContent(CRStringManager.GetString("ClCollisionRadius"),           CRStringManager.GetString("ClCollisionRadiusTooltip"));
    static readonly GUIContent clSafeModeIntegration_         = new GUIContent(CRStringManager.GetString("ClSafeModeIntegration"),       CRStringManager.GetString("ClSafeModeIntegrationTooltip"));
    static readonly GUIContent clUseColliderUVsAsRegions_     = new GUIContent(CRStringManager.GetString("ClUseColliderUVsAsRegions"),   CRStringManager.GetString("ClUseColliderUVsAsRegionsTooltip"));
    static readonly GUIContent clBendStiffness_               = new GUIContent(CRStringManager.GetString("ClBendStiffness"),             CRStringManager.GetString("ClBendStiffnessTooltip"));
    static readonly GUIContent clStretchStiffness_            = new GUIContent(CRStringManager.GetString("ClStretchStiffness"),          CRStringManager.GetString("ClStretchStiffnessTooltip"));
    static readonly GUIContent clBendDamping_                 = new GUIContent(CRStringManager.GetString("ClBendDamping"),               CRStringManager.GetString("ClBendDampingTooltip"));
    static readonly GUIContent clStretchDamping_              = new GUIContent(CRStringManager.GetString("ClStretchDamping"),            CRStringManager.GetString("ClStretchDampingTooltip"));        

    public static Texture icon_;
    public override Texture TexIcon 
    { 
      get{ return icon_; } 
    }

    new CNClothEditorState state_;
    new CNCloth Data { get; set; }
    //-----------------------------------------------------------------------------------
    public CNClothEditor( CNCloth data, CNClothEditorState state)
      : base(data, state)
    {
      Data = (CNCloth)data;
      state_ = state;
    }
    //-----------------------------------------------------------------------------------
    protected override void LoadState()
    {
      base.LoadState();

      state_.cloth_bend_                 = Data.Cloth_Bend;          
      state_.cloth_stretch_              = Data.Cloth_Stretch;
      state_.cloth_dampingBend_          = Data.Cloth_DampingBend;
      state_.cloth_dampingStretch_       = Data.Cloth_DampingStretch;
      state_.cloth_collisionRadius_      = Data.Cloth_CollisionRadius;
      state_.disableCollisionNearJoints_ = Data.DisableCollisionNearJoints;
      state_.safeMode_integration_       = Data.SafeModeIntegration;
      state_.useColliderUVs_             = Data.UseColliderUVs;
      
    }
    //-----------------------------------------------------------------------------------
    public override void ValidateState()
    {
      base.ValidateState();

      ValidateCollisionRadius();
      ValidateBendStretch();
      ValidateDampingStretchBend();
      ValidateDisableCollisionNearJoints();
      ValidateSafeModeIntegration();
      ValidateColliderUVs();
    
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
    private void ValidateCollisionRadius()
    {
      if (state_.cloth_collisionRadius_ != Data.Cloth_CollisionRadius )
      {
        DestroyBodies();
        CreateBodies();
        Debug.Log("Changed collision radius");
        state_.cloth_collisionRadius_ = Data.Cloth_CollisionRadius;
      }
    }
    //-----------------------------------------------------------------------------------
    private void ValidateBendStretch()
    {
      if ( state_.cloth_bend_ != Data.Cloth_Bend || 
           state_.cloth_stretch_ != Data.Cloth_Stretch )
      {
        eManager.SetBendStretch( Data );
        Debug.Log("Changed BendStretch");  

        state_.cloth_bend_    = Data.Cloth_Bend;
        state_.cloth_stretch_ = Data.Cloth_Stretch;
      }
    }
    //-----------------------------------------------------------------------------------
    private void ValidateDampingStretchBend()
    {
      if ( state_.cloth_dampingStretch_ != Data.Cloth_DampingStretch ||
           state_.cloth_dampingBend_ != Data.Cloth_DampingBend )
      {
        eManager.SetDampingStretchBend( Data );
        Debug.Log("Changed Damping BendStretch");  

        state_.cloth_dampingStretch_ = Data.Cloth_DampingStretch;
        state_.cloth_dampingBend_    = Data.Cloth_DampingBend;
      }
    }
    //-----------------------------------------------------------------------------------
    private void ValidateDisableCollisionNearJoints()
    {
      if ( state_.disableCollisionNearJoints_ != Data.DisableCollisionNearJoints )
      {
        eManager.SetDisableCollisionNearJoints( Data );
        Debug.Log("Changed disable collisions near Joints");  

        state_.disableCollisionNearJoints_ = Data.DisableCollisionNearJoints;
      }
    }
    //-----------------------------------------------------------------------------------
    private void ValidateSafeModeIntegration()
    {
      if ( state_.safeMode_integration_ != Data.SafeModeIntegration )
      {
        DestroyBodies();
        CreateBodies();

        Debug.Log("Changed cloth integration mode");
        state_.safeMode_integration_ = Data.SafeModeIntegration;
      }
    }
    //-----------------------------------------------------------------------------------
    private void ValidateColliderUVs()
    {
      if (state_.useColliderUVs_ != Data.UseColliderUVs)
      {
        DestroyBodies();
        CreateBodies();

        Debug.Log("Changed cloth use collider UVs");
        state_.useColliderUVs_ = Data.UseColliderUVs;
      }
    }
    //-----------------------------------------------------------------------------------
    public override void CreateBodies(GameObject[] arrGameObject)
    {
      CreateBodies(arrGameObject, "Caronte FX - Cloth creation", "Creating " + Data.Name + " cloth. ");
    }
    //-----------------------------------------------------------------------------------
    public override void DestroyBodies(GameObject[] arrGameObject)
    {
      DestroyBodies(arrGameObject, "CaronteFX - Cloth destruction", "Destroying " + Data.Name + "cloth. ");
    }
    //-----------------------------------------------------------------------------------
    public override void DestroyBodies(int[] arrInstanceId)
    {
      DestroyBodies(arrInstanceId, "Caronte FX - Softbody destruction", "Destroying " + Data.Name + " cloth. ");
    }
    //-----------------------------------------------------------------------------------
    protected override void ActionCreateBody( GameObject go )
    {
      eManager.CreateBody(Data, go);
    }
    //-----------------------------------------------------------------------------------
    protected override void ActionDestroyBody( GameObject go )
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
    private void DrawDoAutocollide()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle( bdAutoCollideCt_, Data.Cloth_AutoCollide );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change auto collide - " + Data.Name);
        Data.Cloth_AutoCollide = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawDisableCollisionsAtJoints()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle( bdDisableCollisionsAtJointsCt_, Data.DisableCollisionNearJoints );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change disable collision at joints - " + Data.Name);
        Data.DisableCollisionNearJoints = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawSafeModeIntegration()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle( clSafeModeIntegration_, Data.SafeModeIntegration );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change safe mode integration - " + Data.Name);
        Data.SafeModeIntegration = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawClothGrouping()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle(clUseColliderUVsAsRegions_, Data.UseColliderUVs);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Use collider UVs for grouping - " + Data.Name);
        Data.UseColliderUVs = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawCollisionRadius()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField( clCollisionRadiusCt_, Data.Cloth_CollisionRadius);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change collision radius - " + Data.Name);
        Data.Cloth_CollisionRadius = Mathf.Clamp(value, 0f, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawBendStiffness()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField( clBendStiffness_, Data.Cloth_Bend );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change bend stiffness - " + Data.Name);
        Data.Cloth_Bend = Mathf.Clamp(value, 0f, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawStretchStiffness()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField( clStretchStiffness_, Data.Cloth_Stretch );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change stretch stiffness - " + Data.Name);
        Data.Cloth_Stretch = Mathf.Clamp(value, 1f, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawDampingBend()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField( clBendDamping_, Data.Cloth_DampingBend);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change bend damping - " + Data.Name);
        Data.Cloth_DampingBend = Mathf.Clamp(value, 0f, 10000f);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawDampingStretch()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField( clStretchDamping_, Data.Cloth_DampingStretch);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change stretch damping - " + Data.Name);
        Data.Cloth_DampingStretch = Mathf.Clamp(value, 0f, 10000f);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected override void RenderFieldsBody(bool isEditable)
    {
      EditorGUILayout.Space();
      EditorGUILayout.Space();

      CRGUIUtils.DrawSeparator();
      CRGUIUtils.Splitter();

      scroller_ = EditorGUILayout.BeginScrollView(scroller_);
      
      EditorGUILayout.Space();
      float originalLabelWidth    = EditorGUIUtility.labelWidth;
      EditorGUIUtility.labelWidth = 180f;

      EditorGUI.BeginDisabledGroup(!isEditable);
      {
        DrawDoCollide();
        DrawDoAutocollide();
        DrawDisableCollisionsAtJoints();
        EditorGUILayout.Space();

        DrawGUIMassOptions();

        GUILayout.Space(simple_space);
        DrawCollisionRadius();
        DrawSafeModeIntegration();
        if (CRVersionChecker.IsPremiumVersion())
        {
          DrawClothGrouping();
        }

        GUILayout.Space(simple_space);
        DrawBendStiffness();  
        DrawStretchStiffness();
        GUILayout.Space(simple_space);
        DrawDampingBend();
        DrawDampingStretch();
        GUILayout.Space(simple_space);

        CRGUIUtils.Splitter();
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
        DrawDampingPerSecondWorld();
        GUILayout.Space(simple_space);

        bool currentMode = EditorGUIUtility.wideMode;
        EditorGUIUtility.wideMode = true;
        DrawLinearVelocity();
        DrawAngularVelocity();

        CRGUIUtils.Splitter();
        GUILayout.Space(simple_space);
        DrawExplosionOpacity();
        DrawExplosionResponsiveness();
        EditorGUIUtility.wideMode = currentMode;
      }
      EditorGUI.EndDisabledGroup();
    
      CRGUIUtils.Splitter();
      EditorGUILayout.Space();

      EditorGUIUtility.labelWidth = originalLabelWidth;

      EditorGUILayout.Space();     
      EditorGUILayout.EndScrollView();
      }
    //-----------------------------------------------------------------------------------
  }
}

