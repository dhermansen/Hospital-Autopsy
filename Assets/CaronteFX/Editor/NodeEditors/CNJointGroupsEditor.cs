using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CaronteSharp;


namespace CaronteFX
{
  public class CNJointGroupsEditor : CommandNodeEditor
  {
    public static Texture icon_area_;
    public static Texture icon_vertices_;
    public static Texture icon_leaves_;
    public static Texture icon_locators_;

    protected static readonly GUIContent jgObjectsAFieldCt_                  = new GUIContent( CRStringManager.GetString("JgObjectsA"),                     CRStringManager.GetString("JgObjectsTooltip"));
    protected static readonly GUIContent jgObjectsBFieldCt_                  = new GUIContent( CRStringManager.GetString("JgObjectsB"),                     CRStringManager.GetString("JgObjectsTooltip"));
    protected static readonly GUIContent jgLocatorsCFieldCt_                 = new GUIContent( CRStringManager.GetString("JgLocatorsC"),                    CRStringManager.GetString("JgLocatorsCTooltip"));
    protected static readonly GUIContent jgLeavesFieldCt_                    = new GUIContent( CRStringManager.GetString("JgLeaves"),                       CRStringManager.GetString("JgLeavesTooltip"));
    protected static readonly GUIContent jgTrunkFieldCt_                     = new GUIContent( CRStringManager.GetString("JgTrunks"),                       CRStringManager.GetString("JgTrunksTooltip"));
    protected static readonly GUIContent jgCreationTimeCt_                   = new GUIContent( CRStringManager.GetString("JgCreationTime"),                 CRStringManager.GetString("JgCreationTimeTooltip"));
    protected static readonly GUIContent jgDistanceSearchCt_                 = new GUIContent( CRStringManager.GetString("JgDistanceSearch"),               CRStringManager.GetString("JgDistanceSearchTooltip"));
    protected static readonly GUIContent jgMatchingDistanceSearchCt_         = new GUIContent( CRStringManager.GetString("JgMatchingDistanceSearch"),       CRStringManager.GetString("JgMatchingDistanceSearchTooltip"));
    protected static readonly GUIContent jgAreaMinCt_                        = new GUIContent( CRStringManager.GetString("JgAreaMin"),                      CRStringManager.GetString("JgAreaMinTooltip"));
    protected static readonly GUIContent jgAngleMaxCt_                       = new GUIContent( CRStringManager.GetString("JgAngleMax"),                     CRStringManager.GetString("JgAngleMaxTooltip"));
    protected static readonly GUIContent jgNumberMaxPerPairCt_               = new GUIContent( CRStringManager.GetString("JgNumberMaxPerPair"),             CRStringManager.GetString("JgNumberMaxPerPairTooltip"));
    protected static readonly GUIContent jgLimitNumberOfProcessedJointsCt_   = new GUIContent( CRStringManager.GetString("JgLimitNumberOfProcessedJoints"), CRStringManager.GetString("JgLimitNumberOfProcessedJointsTooltip"));
    protected static readonly GUIContent jgMaxProcessedJointsPerPairCt_      = new GUIContent( CRStringManager.GetString("JgMaxProcessedJointsPerPair"),    CRStringManager.GetString("JgMaxProcessedJointsPerPairTooltip"));
    protected static readonly GUIContent jgDisableCollisionByPairsCt_        = new GUIContent( CRStringManager.GetString("JgDisableCollisionByPairs"),      CRStringManager.GetString("JgDisableCollisionByPairsTooltip"));
    protected static readonly GUIContent jgDisableAllCollisionBetweenABCt_   = new GUIContent( CRStringManager.GetString("JgDisableAllCollisionBetweenAB"), CRStringManager.GetString("JgDisableAllCollisionBetweenABTooltip"));
    protected static readonly GUIContent jgMaxForceNm2Ct_                    = new GUIContent( CRStringManager.GetString("JgMaxForceNm2"),                  CRStringManager.GetString("JgMaxForceNm2Tooltip"));
    protected static readonly GUIContent jgMaxForceNCt_                      = new GUIContent( CRStringManager.GetString("JgMaxForceN"),                    CRStringManager.GetString("JgMaxForceNTooltip"));
    protected static readonly GUIContent jgMaximumForceCt_                   = new GUIContent( CRStringManager.GetString("JgMaximumForce"),                 CRStringManager.GetString("JgMaximumForceTooltip"));
    protected static readonly GUIContent jgMaxForceRandCt_                   = new GUIContent( CRStringManager.GetString("JgMaxForceRand"),                 CRStringManager.GetString("JgMaxForceRandTooltip"));
    protected static readonly GUIContent jgForceRangeCt_                     = new GUIContent( CRStringManager.GetString("JgForceRange"),                   CRStringManager.GetString("JgForceRangeTooltip"));
    protected static readonly GUIContent jgForceProfileCt_                   = new GUIContent( CRStringManager.GetString("JgForceProfile"),                 CRStringManager.GetString("JgForceProfileTooltip"));
    protected static readonly GUIContent jgBreakIfDistanceExceededCt_        = new GUIContent( CRStringManager.GetString("JgBreakIfDistanceExceeded"),      CRStringManager.GetString("JgBreakIfDistanceExceededTooltip"));
    protected static readonly GUIContent jgBreakDistanceCt_                  = new GUIContent( CRStringManager.GetString("JgBreakDistance"),                CRStringManager.GetString("JgBreakDistanceTooltip"));
    protected static readonly GUIContent jgBreakDistanceRandCt_              = new GUIContent( CRStringManager.GetString("JgBreakDistanceRand"),            CRStringManager.GetString("JgBreakDistanceRandTooltip"));
    protected static readonly GUIContent jgBreakAllInPairIfFewUnbrokenCt_    = new GUIContent( CRStringManager.GetString("JgBreakAllInPairIfFewUnbroken"),  CRStringManager.GetString("JgBreakAllInPairIfFewUnbrokenTooltip"));
    protected static readonly GUIContent jgUnbrokenNumberToBreakAllCt_       = new GUIContent( CRStringManager.GetString("JgUnbrokenNumberToBreakAll"),     CRStringManager.GetString("JgUnbrokenNumberToBreakAllTooltip"));
    protected static readonly GUIContent jgBreakIfHingeCt_                   = new GUIContent( CRStringManager.GetString("JgBreakIfHinge"),                 CRStringManager.GetString("JgBreakIfHingeTooltip"));
    protected static readonly GUIContent jgEnableCollisionsIfBreakCt_        = new GUIContent( CRStringManager.GetString("JgEnableCollisionsIfBreak"),      CRStringManager.GetString("JgEnableCollisionsIfBreakTooltip"));
    protected static readonly GUIContent jgPlasticityCt_                     = new GUIContent( CRStringManager.GetString("JgPlasticity"),                   CRStringManager.GetString("JgPlasticityTooltip"));
    protected static readonly GUIContent jgPlasticityDistanceCt_             = new GUIContent( CRStringManager.GetString("JgPlasticityDistance"),           CRStringManager.GetString("JgPlasticityDistanceTooltip"));
    protected static readonly GUIContent jgPlasticityDistanceRandCt_         = new GUIContent( CRStringManager.GetString("JgPlasticityDistanceRand"),       CRStringManager.GetString("JgPlasticityDistanceRandTooltip"));
    protected static readonly GUIContent jgPlasticityAcquiredCt_             = new GUIContent( CRStringManager.GetString("JgPlasticityAcquired"),           CRStringManager.GetString("JgPlasticityAcquiredTooltip"));
    protected static readonly GUIContent jgLocatorsCreationModeCt_           = new GUIContent( CRStringManager.GetString("JgLocatorsCreationMode"),         CRStringManager.GetString("JgLocatorsCreationModeTooltip"));
    protected static readonly GUIContent jgDampingCt_                        = new GUIContent( CRStringManager.GetString("JgDamping"),                      CRStringManager.GetString("JgDampingTooltip")); 

    public override Texture TexIcon
    {
      get
      { 
        switch (Data.CreationMode)
        {
        case CNJointGroups.CreationModeEnum.ByContact:
          return icon_area_;

        case CNJointGroups.CreationModeEnum.ByMatchingVertices:
          return icon_vertices_;

        case CNJointGroups.CreationModeEnum.ByStem:
          return icon_leaves_;
        
        default:
          return icon_locators_;
        }
      } 
    }

    protected CNFieldController FieldControllerA { get; set; }
    protected CNFieldController FieldControllerB { get; set; }
    protected CNFieldController FieldControllerC { get; set; }

    public enum LocatorsModeEnum
    {   
      Positions, 
      Vertexes,  
      BoxCenters,
      None
    }

    LocatorsModeEnum locatorsMode_;
    public LocatorsModeEnum LocatorsMode
    {
      set
      {
        locatorsMode_ = value;

        switch (locatorsMode_)
        {
          case LocatorsModeEnum.Positions:
            Data.CreationMode = CNJointGroups.CreationModeEnum.AtLocatorsPositions;
          break;

          case LocatorsModeEnum.Vertexes:
            Data.CreationMode = CNJointGroups.CreationModeEnum.AtLocatorsVertexes;
          break;

          case LocatorsModeEnum.BoxCenters:
            Data.CreationMode = CNJointGroups.CreationModeEnum.AtLocatorsBBoxCenters;
          break;
        }
      }
    }

    protected static readonly GUIContent[] arrLocatorsModeCt_ = new GUIContent[] { new GUIContent(CRStringManager.GetString("JgLocatorsModePositions")), new GUIContent(CRStringManager.GetString("JgLocatorsModeVertexes")), new GUIContent(CRStringManager.GetString("JgLocatorsModeBBoxCenters")) };

    public bool MaximumForce
    {
      get
      {
        return (Data.ForceMaxMode == CNJointGroups.ForceMaxModeEnum.Unlimited);
      }

      set 
      { 
        if (value)
        {
          Data.ForceMaxMode = CNJointGroups.ForceMaxModeEnum.Unlimited;
        }
        else
        {
          Data.ForceMaxMode = CNJointGroups.ForceMaxModeEnum.ConstantLimit;
        }         
      }
    }

    protected new CNJointGroups Data { get; set; }

    //-----------------------------------------------------------------------------------
    public CNJointGroupsEditor( CNJointGroups data, CommandNodeEditorState state )
      : base( data, state )
    {
      Data = (CNJointGroups)data;
    }
    //-----------------------------------------------------------------------------------
    public override void Init()
    {
      base.Init();

      CNField.AllowedTypes allowedTypes =   CNField.AllowedTypes.Geometry 
                                          | CNField.AllowedTypes.BodyNode;

      FieldControllerA = new CNFieldController( Data, Data.ObjectsA, eManager, goManager );
      FieldControllerA.SetFieldType( allowedTypes );
      FieldControllerA.SetCalculatesDiff(true);
      FieldControllerA.IsBodyField = true;

      FieldControllerB = new CNFieldController( Data, Data.ObjectsB, eManager, goManager );
      FieldControllerB.SetFieldType( allowedTypes );
      FieldControllerB.SetCalculatesDiff(true);
      FieldControllerB.IsBodyField = true;

      FieldControllerC = new CNFieldController( Data, Data.LocatorsC, eManager, goManager );
      FieldControllerC.SetFieldType( CNField.AllowedTypes.Locator | CNField.AllowedTypes.Geometry );
      FieldControllerC.SetCalculatesDiff(true);
    }
    //-----------------------------------------------------------------------------------
    protected void SetLocatorsOption()
    {
      switch (Data.CreationMode)
      {
        case CNJointGroups.CreationModeEnum.AtLocatorsPositions:
          locatorsMode_ = LocatorsModeEnum.Positions;
          break;

        case CNJointGroups.CreationModeEnum.AtLocatorsVertexes:
          locatorsMode_ = LocatorsModeEnum.Vertexes;
          break;

        case CNJointGroups.CreationModeEnum.AtLocatorsBBoxCenters:
          locatorsMode_ = LocatorsModeEnum.BoxCenters;
          break;

        default:
          locatorsMode_ = LocatorsModeEnum.None;
          break;
      }
    }
    //-----------------------------------------------------------------------------------
    public void CreateEntities()
    {
      if ( !IsExcludedInHierarchy ) 
      {
        GameObject[] arrGameObjectA;
        GameObject[] arrGameObjectB;
        Vector3[]    arrLocatorsC;

        GetFieldGameObjects( FieldControllerA, out arrGameObjectA );
        GetFieldGameObjects( FieldControllerB, out arrGameObjectB );

        bool fieldAIsReallyEmpty = FieldControllerA.HasNoReferences();
        bool fieldBIsReallyEmpty = FieldControllerB.HasNoReferences();

        GetFieldLocators( FieldControllerC, out arrLocatorsC );

        eManager.CreateMultiJoint( Data, arrGameObjectA, arrGameObjectB, arrLocatorsC, fieldAIsReallyEmpty, fieldBIsReallyEmpty );
        cnManager.SceneSelection();

        LoadState();
      }
    }
    //-----------------------------------------------------------------------------------
    public virtual void DestroyEntities()
    {
      GameObject[] arrGameObjectA;
      GameObject[] arrGameObjectB;

      GetFieldGameObjects( FieldControllerA, out arrGameObjectA );
      GetFieldGameObjects( FieldControllerB, out arrGameObjectB );

      eManager.DestroyMultiJoint( Data, arrGameObjectA, arrGameObjectB );
    }
    //-----------------------------------------------------------------------------------
    public virtual void RecreateEntities()
    {
      DestroyEntities();
      CreateEntities();
    }
    //-----------------------------------------------------------------------------------
    public void EditEntites()
    {
      eManager.EditMultiJoint( Data );
    }
    //-----------------------------------------------------------------------------------
    protected void GetFieldGameObjects( CNFieldController fieldController, out GameObject[] arrGameObject )
    {
      arrGameObject = fieldController.GetUnityGameObjects();
    }
    //-----------------------------------------------------------------------------------
    private void GetFieldLocators( CNFieldController fieldController, out Vector3[] arrLocations )
    {
      GameObject[] gameObjects = fieldController.GetUnityGameObjects();

      List<Vector3> listLocatorPosition = new List<Vector3>();
      switch ( Data.CreationMode )
      {
        case CNJointGroups.CreationModeEnum.AtLocatorsPositions:
          for (int i = 0; i < gameObjects.Length; i++)
          {
            Transform tr = gameObjects[i].transform;
            if ( tr.childCount == 0 )
            {
              listLocatorPosition.Add(tr.position);
            }
          }
          break;

        case CNJointGroups.CreationModeEnum.AtLocatorsBBoxCenters:
          for (int i = 0; i < gameObjects.Length; ++i)
          {
            Renderer renderer = gameObjects[i].GetComponent<Renderer>();
            if ( renderer != null )
            {
              Bounds bbox = renderer.bounds;
              listLocatorPosition.Add(bbox.center);
            }
          }
          break;

        case CNJointGroups.CreationModeEnum.AtLocatorsVertexes:
          for (int i = 0; i < gameObjects.Length; ++i)
          {
            GameObject go = gameObjects[i];
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();

            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
              UnityEngine.Mesh mesh = meshFilter.sharedMesh;
              UnityEngine.Mesh meshTransformed;

              CRGeometryUtils.CreateMeshTransformed( mesh, go.transform.localToWorldMatrix, out meshTransformed );
              Vector3[] meshVertices = meshTransformed.vertices;
              for (int j = 0; j < meshVertices.Length; ++j)
              {
                listLocatorPosition.Add(meshVertices[j]);
              }   

              UnityEngine.Object.DestroyImmediate( meshTransformed );
            }
          }
          break;

        default:
          break;
      }

      arrLocations = listLocatorPosition.ToArray();
    }
    //-----------------------------------------------------------------------------------
    public override void SetActivityState()
    {
      base.SetActivityState();
      eManager.SetActivity( Data );
    }
    //-----------------------------------------------------------------------------------
    public override void SetVisibilityState()
    {
      base.SetVisibilityState();
      eManager.SetVisibility( Data );
    }
    //-----------------------------------------------------------------------------------
    public override void SetExcludedState()
    {
      base.SetExcludedState();
      if (IsExcludedInHierarchy)
      {
        DestroyEntities();
      }
      else
      {
        CreateEntities();
      }
      EditorUtility.ClearProgressBar();
    }
    //-----------------------------------------------------------------------------------
    public void AddGameObjectsToA( UnityEngine.Object[] draggedObjects, bool recalculateFields )
    {
      FieldControllerA.AddGameObjects( draggedObjects, recalculateFields );
    }
    //-----------------------------------------------------------------------------------
    public void AddGameObjectsToB( UnityEngine.Object[] draggedObjects, bool recalculateFields )
    {
      FieldControllerB.AddGameObjects( draggedObjects, recalculateFields );
    }
    //-----------------------------------------------------------------------------------
    public void AddGameObjectsToC( UnityEngine.Object[] draggedObjects, bool recalculateFields )
    {
      FieldControllerC.AddGameObjects( draggedObjects, recalculateFields );
    }
    //-----------------------------------------------------------------------------------  
    public bool IsLocatorsModeActive()
    {
      return ( Data.CreationMode == CNJointGroups.CreationModeEnum.AtLocatorsBBoxCenters ||
               Data.CreationMode == CNJointGroups.CreationModeEnum.AtLocatorsPositions ||
               Data.CreationMode == CNJointGroups.CreationModeEnum.AtLocatorsVertexes );
    }
    //-----------------------------------------------------------------------------------
    public override void FreeResources()
    {
      DestroyEntities();
      FieldControllerA.DestroyField();
      FieldControllerB.DestroyField();
      FieldControllerC.DestroyField();
    }
    //-----------------------------------------------------------------------------------
    public override void LoadInfo()
    {
      FieldControllerA.RestoreFieldInfo();
      FieldControllerB.RestoreFieldInfo();
      FieldControllerC.RestoreFieldInfo();
    }
    //-----------------------------------------------------------------------------------
    public override void StoreInfo()
    {
      FieldControllerA.StoreFieldInfo();
      FieldControllerB.StoreFieldInfo();
      FieldControllerC.StoreFieldInfo();
    }
    //-----------------------------------------------------------------------------------
    public override void BuildListItems()
    {
      FieldControllerA.BuildListItems();
      FieldControllerB.BuildListItems();
      FieldControllerC.BuildListItems();
    }
    //-----------------------------------------------------------------------------------
    public override void SetScopeId(uint scopeId)
    {
      FieldControllerA.SetScopeId( scopeId );
      FieldControllerB.SetScopeId( scopeId );
      FieldControllerC.SetScopeId( scopeId );
    }
    //-----------------------------------------------------------------------------------
    public override bool RemoveNodeFromFields(CommandNode node)
    {
      Undo.RecordObject(Data, "CaronteFX - Remove node from fields");

      bool removedNodeA = Data.ObjectsA.RemoveNode(node);
      bool removedNodeB = Data.ObjectsB.RemoveNode(node);
      bool removedNodeC = Data.LocatorsC.RemoveNode(node);

      bool removed = removedNodeA || removedNodeB || removedNodeC;
      if (removed)
      {
        Data.NeedsUpdate = true;
      }

      return removed;
    }
    //-----------------------------------------------------------------------------------
    public void CheckUpdate()
    {
      bool creationModeWithLocators = ( Data.CreationMode == CNJointGroups.CreationModeEnum.AtLocatorsBBoxCenters ||
                                        Data.CreationMode == CNJointGroups.CreationModeEnum.AtLocatorsPositions ||
                                        Data.CreationMode == CNJointGroups.CreationModeEnum.AtLocatorsVertexes);

      bool updateNeeded = Data.NeedsUpdate || (   FieldControllerA.IsUpdateNeeded() ||
                                                  FieldControllerB.IsUpdateNeeded() ||
                                                 ( FieldControllerC.IsUpdateNeeded() && creationModeWithLocators) );

      if (updateNeeded)
      {
        DestroyEntities();
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawForceMax()
    {
      GUIContent forceMaxCt_;
      if (Data.CreationMode == CNJointGroups.CreationModeEnum.ByContact)
      {
        forceMaxCt_ = jgMaxForceNm2Ct_;
      }
      else
      {
        forceMaxCt_ = jgMaxForceNCt_;
      }
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField( forceMaxCt_, Data.ForceMax );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + forceMaxCt_.text + " - " + Data.Name);
        Data.ForceMax = Mathf.Clamp(value, 0, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawMaximumForce()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.ToggleLeft(jgMaximumForceCt_, MaximumForce, GUILayout.Width(80f) );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + jgMaximumForceCt_.text + " - " + Data.Name);
        MaximumForce = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawForceMaxRand()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider( jgMaxForceRandCt_, Data.ForceMaxRand, 0.0f, 1.0f); 
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + jgMaxForceRandCt_.text + " - " + Data.Name);
        Data.ForceMaxRand = Mathf.Clamp(value, 0, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawForceRange()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField( jgForceRangeCt_, Data.ForceRange );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + jgForceRangeCt_.text + " - " + Data.Name);
        Data.ForceRange = Mathf.Clamp(value, 0, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawForceProfile()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.CurveField( jgForceProfileCt_, Data.ForceProfile, Color.green, new Rect( 0f, 0f, 1f, 1f) );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + jgForceProfileCt_.text + " - " + Data.Name);
        Data.ForceProfile = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawBreakIfDistanceExceeded()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle( Data.BreakIfDistExcedeed );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + jgBreakIfDistanceExceededCt_.text + " - " + Data.Name);
        Data.BreakIfDistExcedeed = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawDistanceForBreak()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(jgBreakDistanceCt_, Data.DistanceForBreak);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + jgBreakDistanceCt_.text + " - " + Data.Name);
        Data.DistanceForBreak = Mathf.Clamp( value, 0f, float.MaxValue );
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawDistanceForBreakRand()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider(jgBreakDistanceRandCt_, Data.DistanceForBreakRand, 0.0f, 1.0f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + jgBreakDistanceRandCt_.text + " - " + Data.Name);
        Data.DistanceForBreakRand = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawBreakAllIfLeftFewUnbroken()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle(jgBreakAllInPairIfFewUnbrokenCt_ , Data.BreakAllIfLeftFewUnbroken);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + jgBreakAllInPairIfFewUnbrokenCt_.text + " - " + Data.Name);
        Data.BreakAllIfLeftFewUnbroken = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawUnbrokenNumberForBreakAll()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.IntField(jgUnbrokenNumberToBreakAllCt_, Data.UnbrokenNumberForBreakAll);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + jgUnbrokenNumberToBreakAllCt_.text + " - " + Data.Name);
        Data.UnbrokenNumberForBreakAll = Mathf.Clamp(value, 0, int.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawBreakIfHinge()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle(jgBreakIfHingeCt_, Data.BreakIfHinge);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change "  + jgBreakIfHingeCt_.text + " - " + Data.Name);
        Data.BreakIfHinge = value;
        EditorUtility.SetDirty(Data);
      }
    }    
    //-----------------------------------------------------------------------------------
    protected void DrawEnableCollisionIfBreak()
    {
     EditorGUI.BeginChangeCheck(); 
     var value = EditorGUILayout.Toggle(jgEnableCollisionsIfBreakCt_, Data.EnableCollisionIfBreak);
     if (EditorGUI.EndChangeCheck())
     {
       Undo.RecordObject(Data, "Change " + jgEnableCollisionsIfBreakCt_.text + " - " + Data.Name);
       Data.EnableCollisionIfBreak = value;
       EditorUtility.SetDirty(Data);
     }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawPlasticity()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle( Data.Plasticity );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + jgPlasticityCt_.text + " - " + Data.Name);
        Data.Plasticity = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawDistanceForPlasticity()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(jgPlasticityDistanceCt_, Data.DistanceForPlasticity);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + jgPlasticityDistanceCt_.text + " - " + Data.Name);
        Data.DistanceForPlasticity = Mathf.Clamp(value, 0f, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawDistanceForPlasticityRand()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider(jgPlasticityDistanceRandCt_, Data.DistanceForPlasticityRand, 0f, 1f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change  " + jgPlasticityDistanceRandCt_.text + " - " + Data.Name);
        Data.DistanceForPlasticityRand = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawPlasticityRateAcquired()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider(jgPlasticityAcquiredCt_, Data.PlasticityRateAcquired, 0f, 1f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + jgPlasticityAcquiredCt_.text + " - " + Data.Name);
        Data.PlasticityRateAcquired = Mathf.Clamp(value, 0f, 1f);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    public void DrawDamping()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(jgDampingCt_, Data.Damping);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + jgDampingCt_.text + " - " + Data.Name);
        Data.Damping = Mathf.Clamp(value, 0f, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    public override void RenderGUI( Rect area, bool isEditable )
    {
      GUILayout.BeginArea( area );
      
      RenderTitle(isEditable, true, true, true);       
      EditorGUI.BeginDisabledGroup(!isEditable);
      if (Data.CreationMode != CNJointGroups.CreationModeEnum.ByStem)
      {
        RenderFieldObjects( jgObjectsAFieldCt_, FieldControllerA, true, true, CNFieldWindow.Type.extended );
        RenderFieldObjects( jgObjectsBFieldCt_, FieldControllerB, true, true, CNFieldWindow.Type.extended );
      }
      else
      {
        RenderFieldObjects( jgLeavesFieldCt_, FieldControllerA, true, true, CNFieldWindow.Type.extended );
        RenderFieldObjects( jgTrunkFieldCt_, FieldControllerB, true, true, CNFieldWindow.Type.extended );
      }
      EditorGUILayout.Space();
      EditorGUILayout.Space();

      if (GUILayout.Button( Data.NeedsUpdate ? CRStringManager.GetString("CreateRecreate*") : CRStringManager.GetString("CreateRecreate"), GUILayout.Height(30f) ))
      {
        RecreateEntities();
      }
      EditorGUI.EndDisabledGroup();

      CRGUIUtils.DrawSeparator();
      CRGUIUtils.Splitter();

      scroller_ = EditorGUILayout.BeginScrollView(scroller_);

      float originalLabelwidth = EditorGUIUtility.labelWidth;
      EditorGUIUtility.labelWidth = 200f;

      EditorGUI.BeginDisabledGroup(!isEditable);
      {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUI.BeginChangeCheck();
        RenderCreationParams( Data.CreationMode );
        if( EditorGUI.EndChangeCheck() && eManager.IsMultiJointCreated(Data) )
        {
          DestroyEntities();
        }

        EditorGUILayout.Space();

        CRGUIUtils.Splitter();    
        EditorGUILayout.Space();

        //FORCES
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(MaximumForce);
        DrawForceMax();
        EditorGUI.EndDisabledGroup();
        DrawMaximumForce();   
        EditorGUILayout.EndHorizontal();
        EditorGUI.BeginDisabledGroup(MaximumForce);
        DrawForceMaxRand();
        DrawForceRange();
        DrawForceProfile();
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space();
        DrawDamping();
      }
      EditorGUI.EndDisabledGroup();

      CRGUIUtils.Splitter();
      EditorGUILayout.Space();

      //BREAK
      EditorGUILayout.BeginHorizontal();
      Data.BreakFoldout = EditorGUILayout.Foldout(Data.BreakFoldout, jgBreakIfDistanceExceededCt_ );

      EditorGUI.BeginDisabledGroup(!isEditable);
      {
        GUILayout.Space(145f);
        DrawBreakIfDistanceExceeded();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        {
          if (Data.BreakFoldout)
          {
            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(!Data.BreakIfDistExcedeed);
            DrawDistanceForBreak();
            DrawDistanceForBreakRand();
            EditorGUI.EndDisabledGroup();
          }
          CRGUIUtils.Splitter();
          EditorGUILayout.Space();
          DrawBreakAllIfLeftFewUnbroken();
          EditorGUI.BeginDisabledGroup(!Data.BreakAllIfLeftFewUnbroken);
          DrawUnbrokenNumberForBreakAll();
          EditorGUI.EndDisabledGroup();
          DrawBreakIfHinge();
          DrawEnableCollisionIfBreak();
        }
      }
      EditorGUI.EndDisabledGroup();

      CRGUIUtils.Splitter();
      EditorGUILayout.Space();

      //PLASTICITY
      EditorGUILayout.BeginHorizontal();
      Data.PlasticityFoldout = EditorGUILayout.Foldout(Data.PlasticityFoldout, jgPlasticityCt_);

      EditorGUI.BeginDisabledGroup(!isEditable);
      {
        GUILayout.Space(145f);
        DrawPlasticity();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        if (Data.PlasticityFoldout)
        {
          EditorGUILayout.Space();
          
          EditorGUI.BeginDisabledGroup(!Data.Plasticity);
          DrawDistanceForPlasticity();
          DrawDistanceForPlasticityRand();
          DrawPlasticityRateAcquired();
          EditorGUI.EndDisabledGroup();
        }
      }  
      EditorGUI.EndDisabledGroup();
      CRGUIUtils.Splitter();

      EditorGUIUtility.labelWidth = originalLabelwidth;
      EditorGUILayout.EndScrollView();
      GUILayout.EndArea();
    } // RenderGUI
    //-----------------------------------------------------------------------------------
    protected void DrawContactDistanceSearch()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(jgDistanceSearchCt_, Data.ContactDistanceSearch);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change " + jgDistanceSearchCt_.text + " - " + Data.Name);
        Data.ContactDistanceSearch = Mathf.Clamp(value, 0f, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawContactAreaMin()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(jgAreaMinCt_, Data.ContactAreaMin);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change  " + jgAreaMinCt_.text + " - " + Data.Name);
        Data.ContactAreaMin = Mathf.Clamp(value, 0f, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawContactAngleMaxInDegrees()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(jgAngleMaxCt_, Data.ContactAngleMaxInDegrees);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change " + jgAngleMaxCt_.text + " - " + Data.Name );
        Data.ContactAngleMaxInDegrees = Mathf.Clamp(value, 0f, 180f);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawContactNumberMax()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.IntField(jgNumberMaxPerPairCt_, Data.ContactNumberMax);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change " + jgNumberMaxPerPairCt_.text + " - " + Data.Name );
        Data.ContactNumberMax = Mathf.Clamp(value, 0, int.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawMatchingDistanceSearch()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(jgMatchingDistanceSearchCt_, Data.MatchingDistanceSearch);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change " + jgMatchingDistanceSearchCt_.text + " - " + Data.Name);
        Data.MatchingDistanceSearch = Mathf.Clamp(value, 0f, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawLocatorsCreationMode()
    {
      EditorGUI.BeginChangeCheck();
      var value = (LocatorsModeEnum) EditorGUILayout.Popup( jgLocatorsCreationModeCt_, (int)locatorsMode_, arrLocatorsModeCt_);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change creation mode - " + Data.Name);
        LocatorsMode = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawLimitNumberOfActiveJoints()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle(jgLimitNumberOfProcessedJointsCt_, Data.LimitNumberOfActiveJoints);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change " + jgLimitNumberOfProcessedJointsCt_.text + " - " + Data.Name);
        Data.LimitNumberOfActiveJoints = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawActiveJointsMaxInABPair()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.IntField(jgMaxProcessedJointsPerPairCt_, Data.ActiveJointsMaxInABPair);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change  " + jgMaxProcessedJointsPerPairCt_.text + " - " + Data.Name);
        Data.ActiveJointsMaxInABPair = Mathf.Clamp(value, 0, int.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawDisableCollisionsByPairs()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle(jgDisableCollisionByPairsCt_, Data.DisableCollisionsByPairs);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change  " + jgDisableCollisionByPairsCt_.text + " - " + Data.Name);
        Data.DisableCollisionsByPairs = value;
        if (value)
        {
          Data.DisableAllCollisionsOfAsWithBs = false;
        }
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawDisableAllCollisionsOfAsWithBs()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle(jgDisableAllCollisionBetweenABCt_, Data.DisableAllCollisionsOfAsWithBs);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change " + jgDisableAllCollisionBetweenABCt_.text + " - " + Data.Name);
        Data.DisableAllCollisionsOfAsWithBs = value;
        if (value)
        {
          Data.DisableCollisionsByPairs = false;
        } 
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawCreationTimer()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(jgCreationTimeCt_, Data.DelayedCreationTime);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change " + jgCreationTimeCt_.text + " - " + Data.Name);
        Data.DelayedCreationTime = Mathf.Clamp(value, 0, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void RenderCreationParams( CNJointGroups.CreationModeEnum creationMode)
    {
      SetLocatorsOption();

      DrawCreationTimer();
      EditorGUILayout.Space();

      if ( Data.CreationMode == CNJointGroups.CreationModeEnum.ByContact )
      {
        DrawContactDistanceSearch();
        DrawContactAreaMin();
        DrawContactAngleMaxInDegrees();
        DrawContactNumberMax();
      }
      else if ( Data.CreationMode == CNJointGroups.CreationModeEnum.ByMatchingVertices )
      {
        DrawMatchingDistanceSearch();
      }
      else if ( locatorsMode_ != LocatorsModeEnum.None )
      {
        DrawLocatorsCreationMode();
        GUILayout.Space(simple_space);
        RenderFieldObjects( jgLocatorsCFieldCt_, FieldControllerC, Data.IsCreateModeAtLocators, true, CNFieldWindow.Type.normal);   
      }

      GUILayout.Space(simple_space);
      DrawLimitNumberOfActiveJoints();
      EditorGUI.BeginDisabledGroup(!Data.LimitNumberOfActiveJoints);
      DrawActiveJointsMaxInABPair();
      EditorGUI.EndDisabledGroup();

      EditorGUILayout.Space();

      DrawDisableCollisionsByPairs();
      DrawDisableAllCollisionsOfAsWithBs();  
    }

  } //JointGroupsView

} // namespace Caronte...


