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
using System.Collections;
using System.Collections.Generic;
using CaronteSharp;

namespace CaronteFX
{
  class CRSimulationDisplayer
  {      
    List<JointGroupsInfo>   listJointGroupsInfo_;
    List<ExplosionInfo>     listExplosionInfo_ ;    
    List<ContactEventInfo>  listContactEventInfo_;

    UN_SimulationStatistics statistics_;

    CRSpheresGenerator spheresGenerator_ = new CRSpheresGenerator();

    MeshComplex mcForUpdates = new MeshComplex();

    List<GameObject> listBodyGOEnabledVisible_  = new List<GameObject>();
    List<GameObject> listBodyGODisabledVisible_ = new List<GameObject>();
    List<GameObject> listBodyGOEnabledHide_     = new List<GameObject>();
    List<GameObject> listBodyGODisabledHide_    = new List<GameObject>();
    List<GameObject> listBodyGOSleeping_        = new List<GameObject>();

    List< Tuple2<GameObject, float> > listClothBodiesGORadius_ = new List< Tuple2<GameObject, float> >();

    List<Bounds> listJointPivotNormalIn_   = new List<Bounds>();
    List<Bounds> listJointPivotNormalOut_  = new List<Bounds>();

    List<Bounds> listJointPivotDeformatedIn_  = new List<Bounds>();
    List<Bounds> listJointPivotDeformatedOut_ = new List<Bounds>();

    List<Bounds> listJointPivotBreakingIn_  = new List<Bounds>();
    List<Bounds> listJointPivotBreakingOut_ = new List<Bounds>();

    List<Bounds> listJointPivotBrokenIn_  = new List<Bounds>();
    List<Bounds> listJointPivotBrokenOut_ = new List<Bounds>();

    HashSet<int> setUsedvertex_ = new HashSet<int>();

    uint  frameNumber_ = 0;

    float frameTime_   = 0.0f;
    float currentTime_ = 0.0f;

    bool  isChangeToReplayingDone_ = false;

    Dictionary<uint, Vector2> tableVisibilityInterval_ = new Dictionary<uint,Vector2>();

    public bool UpdateListsBodyGORequested
    {
      get;
      set;
    }

    public bool UpdateListJointsRequested
    {
      get;
      set;
    }

    public bool UpdateClothCollidersRequested
    {
      get;
      set;
    }

    public UN_SimulationStatistics Statistics
    {
      get { return statistics_;}
    }

    List<Bounds> listTmpBounds_ = new List<Bounds>();

    bool doDisplayInvisibleBodies_ = false;

    CREntityManager entityManager_;
    CRPlayer        simulationPlayer_;
    Caronte_Fx      fxData_;

    public CRSimulationDisplayer(CREntityManager entityManager, CRPlayer simulationPlayer)
    {
      entityManager_ = entityManager;
      simulationPlayer_ = simulationPlayer;
    }

    public void Init(Caronte_Fx fxData)
    {
      fxData_ = fxData;
      tableVisibilityInterval_.Clear();
      isChangeToReplayingDone_ = false;
      frameNumber_ = 0;
    }

    public void BuildVisibilityIntervals()
    {
      frameTime_ = 1.0f / fxData_.effect.frameRate_;
      entityManager_.SetDictionaryVisibleTimeInterval(tableVisibilityInterval_); 
      isChangeToReplayingDone_ = true;
    }

    public void Update()
    {
      if ( SimulationManager.ReadSimulationBufferUniqueUnsafe( BroadcastStart, UpdateBody ) )
      {
        statistics_           = SimulationManager.simStatistics_;
        listJointGroupsInfo_  = SimulationManager.listJgInfo_;
        listExplosionInfo_    = SimulationManager.listExplosionInfo_;
        listContactEventInfo_ = SimulationManager.listContactEventInfo_;
   
        UpdateListsBodyGORequested      = true;
        UpdateListJointsRequested       = true;
        UpdateClothCollidersRequested   = true;

        SceneView.RepaintAll();
      }

      if ( UpdateListsBodyGORequested )
      {
        CreateBodyBoxes();
      }

      if ( UpdateListJointsRequested )
      {
        CreateJointPivotBoxes();
      }

      if ( UpdateClothCollidersRequested )
      {
        CreateClothSpheres();
      }
    }

    public void BroadcastStart( bool doDisplayInvisibles )
    {
      doDisplayInvisibleBodies_ = doDisplayInvisibles;

      bool isSimulating = simulationPlayer_.IsSimulating;
      bool isReplaying  = simulationPlayer_.IsReplaying;

      frameNumber_ = SimulationManager.frameNumber_;
      currentTime_ = frameNumber_ * frameTime_;

      if (isSimulating || isReplaying)
      {
        listBodyGOEnabledVisible_ .Clear();
        listBodyGODisabledVisible_.Clear();
        listBodyGOEnabledHide_    .Clear();
        listBodyGODisabledHide_   .Clear();
        listBodyGOSleeping_       .Clear();
      }
    }

    public void UpdateBody( BD_TYPE type, BodyInfo bodyInfo )
    {
      
      Transform trToUpdate = entityManager_.GetBodyTransformRef( bodyInfo.idBody_ );

      if ( trToUpdate == null )
        return;

      GameObject goToUpdate = trToUpdate.gameObject;

      bool bodyActive = (bodyInfo.broadcastFlag_ & BROADCASTFLAG.ACTIVE)   == BROADCASTFLAG.ACTIVE;
      bool isVisible  = (bodyInfo.broadcastFlag_ & BROADCASTFLAG.VISIBLE)  == BROADCASTFLAG.VISIBLE;
      bool isGhost    = (bodyInfo.broadcastFlag_ & BROADCASTFLAG.GHOST)    == BROADCASTFLAG.GHOST;
      bool isSleeping = (bodyInfo.broadcastFlag_ & BROADCASTFLAG.SLEEPING) == BROADCASTFLAG.SLEEPING;

      bool isInsideTimeInterval = true;
      if ( isChangeToReplayingDone_ )
      {
        Vector2 visibilityInterval = tableVisibilityInterval_[bodyInfo.idBody_];
        isInsideTimeInterval = (currentTime_ >= visibilityInterval.x) && (currentTime_ < visibilityInterval.y);
      }

      bool renderActive = ( (isVisible || isGhost) && isInsideTimeInterval ) 
                          || doDisplayInvisibleBodies_;

      goToUpdate.SetActive(renderActive);

      if ( !renderActive )
      {
        return;
      }

      if (isVisible)
      {
        if (bodyActive)
        {
          listBodyGOEnabledVisible_.Add(goToUpdate);
        }
        else
        {
          listBodyGODisabledVisible_.Add(goToUpdate);
        }
      }
      else
      {
        if (bodyActive)
        {
          listBodyGOEnabledHide_.Add(goToUpdate);
        }
        else
        {
          listBodyGODisabledHide_.Add(goToUpdate);
        }
      }

      if (isSleeping)
      {
        listBodyGOSleeping_.Add(goToUpdate);
      }

      switch (type)
      {
        case BD_TYPE.RIGIDBODY:
          {
            UpdateRigidBody(trToUpdate, bodyInfo);
            break;
          }
        
        case BD_TYPE.BODYMESH_ANIMATED_BY_MATRIX:
          {
            UpdateRigidBody(trToUpdate, bodyInfo);
            break;
          }
          
        case BD_TYPE.BODYMESH_ANIMATED_BY_VERTEX:
          {
            UpdateAnimatedByVertex(trToUpdate, bodyInfo);
            break;    
          }

        case BD_TYPE.SOFTBODY:
        case BD_TYPE.CLOTH:
          {
            UpdateVertexBody(trToUpdate, bodyInfo);
            break;
          }
      }
    }// UpdateBody


    private void UpdateRigidBody( Transform tr, BodyInfo bdInfo )
    {
      RigidBodyInfo rigidInfo = (RigidBodyInfo)bdInfo;

      tr.localPosition = rigidInfo.position_;
      tr.localRotation = rigidInfo.orientation_;
    }

    private void UpdateAnimatedByVertex( Transform tr, BodyInfo bdInfo )
    {
      BodyMeshInfo bdmeshInfo = (BodyMeshInfo)bdInfo;
   
      tr.localPosition = bdmeshInfo.position_;
      tr.localRotation = bdmeshInfo.orientation_;

      bool isAnimatedMesh = entityManager_.IsBMeshAnimatedByArrPos(bdmeshInfo.idBody_);

      if ( isAnimatedMesh )
      {
        tr.localScale = Vector3.one;

        Tuple2<UnityEngine.Mesh, MeshUpdater> meshData = entityManager_.GetBodyMeshRenderUpdaterRef(bdInfo.idBody_);

        UnityEngine.Mesh meshToUpdate = meshData.First;
        MeshUpdater meshUpdater       = meshData.Second;

        meshToUpdate.vertices = bdmeshInfo.arrVertices_;

        mcForUpdates.Clear();
        mcForUpdates.Set(meshToUpdate, null);

        CaronteSharp.Tools.UpdateVertexNormalsAndTangents( meshUpdater, mcForUpdates );

        meshToUpdate.normals  = mcForUpdates.arrNormal_;
        meshToUpdate.tangents = mcForUpdates.arrTan_;

        meshToUpdate.RecalculateBounds();
      }
    }

    private void UpdateVertexBody( Transform tr, BodyInfo bdInfo )
    {
      SoftBodyInfo softInfo = (SoftBodyInfo)bdInfo;

      tr.localPosition = softInfo.center_;
      tr.localRotation = Quaternion.identity;
      tr.localScale    = Vector3.one;

      GameObject go = tr.gameObject;
      Caronte_Fx_Body cfxBody = go.GetComponent<Caronte_Fx_Body>();

      if (cfxBody != null && entityManager_.HasBodyMeshColliderRef(bdInfo.idBody_) )
      {
        UnityEngine.Mesh meshCollider = entityManager_.GetBodyMeshColliderRef(bdInfo.idBody_);

        if (meshCollider != null)
        {
          meshCollider.vertices = softInfo.arrVerticesCollider_;     
          meshCollider.RecalculateNormals();
          meshCollider.RecalculateBounds();

          cfxBody.SetCustomColliderMesh(meshCollider); 
        }
      }
     
      Tuple2<UnityEngine.Mesh, MeshUpdater> meshRenderData = entityManager_.GetBodyMeshRenderUpdaterRef(bdInfo.idBody_);
      UnityEngine.Mesh meshToUpdate = meshRenderData.First;
      MeshUpdater meshUpdater       = meshRenderData.Second;

      meshToUpdate.vertices = softInfo.arrVerticesRender_; 
      
      mcForUpdates.Clear();
      mcForUpdates.Set(meshToUpdate, null);
      
      CaronteSharp.Tools.UpdateVertexNormalsAndTangents( meshUpdater, mcForUpdates );

      meshToUpdate.normals  = mcForUpdates.arrNormal_;
      meshToUpdate.tangents = mcForUpdates.arrTan_;

      meshToUpdate.RecalculateBounds();
    }

    public void CreateBodyBoxes()
    {
      if ( fxData_.DrawBodyBoxes )
      {

        if ( SimulationManager.IsEditing() )
        {
           entityManager_.UpdateListsBodyGameObject( listBodyGOEnabledVisible_, 
                                                     listBodyGODisabledVisible_,
                                                     listBodyGOEnabledHide_, 
                                                     listBodyGODisabledHide_ );
        }

        fxData_.ClearBodyMeshes();

        GenerateBodyBoxMeshes();
        SceneView.RepaintAll();
      }

      UpdateListsBodyGORequested = false;
    }

    private void GenerateBodyBoxMeshes()
    {
      CREditorUtils.GetListBoundsFromListGO( listBodyGOEnabledVisible_, listTmpBounds_ );
      CRBoxGenerator.GenerateBoxMeshes( listTmpBounds_, fxData_.listMeshBodyBoxesEnabledVisible_ );

      CREditorUtils.GetListBoundsFromListGO( listBodyGODisabledVisible_, listTmpBounds_ );
      CRBoxGenerator.GenerateBoxMeshes( listTmpBounds_, fxData_.listMeshBodyBoxesDisabledVisible_ );

      if ( fxData_.DrawSleepingState )
      {
        CREditorUtils.GetListBoundsFromListGO( listBodyGOSleeping_, listTmpBounds_ );
        CRBoxGenerator.GenerateBoxMeshes( listTmpBounds_, fxData_.listMeshBodyBoxesSleeping_ );
      }

      if ( fxData_.ShowInvisibles )
      {
        CREditorUtils.GetListBoundsFromListGO( listBodyGOEnabledHide_, listTmpBounds_ );
        CRBoxGenerator.GenerateBoxMeshes( listTmpBounds_, fxData_.listMeshBodyBoxesEnabledHide_ );

        CREditorUtils.GetListBoundsFromListGO( listBodyGODisabledHide_, listTmpBounds_ );
        CRBoxGenerator.GenerateBoxMeshes( listTmpBounds_, fxData_.listMeshBodyBoxesDisabledHide_ );
      }
    }

    public void CreateClothSpheres()
    {
      fxData_.ClearSphereMeshes();
      if ( fxData_.DrawClothColliders )
      {
        if ( SimulationManager.IsEditing() )
        {
          entityManager_.UpdateListClothBodiesGameObjectsEditing(listClothBodiesGORadius_);
        }
        else
        {
          entityManager_.UpdateListClothBodiesGameObjectsSimulatingOrReplaying(listClothBodiesGORadius_);
        }
        GenerateSphereMeshes();
        SceneView.RepaintAll();
      }

      UpdateClothCollidersRequested = false;
    }

    private void GenerateSphereMeshes()
    {
      foreach( var tuple in listClothBodiesGORadius_ )
      {
        GameObject go = tuple.First;

        if ( go == null)
        {
          continue;
        }
        
        float radius  = tuple.Second;

        Caronte_Fx_Body crBody = go.GetComponent<Caronte_Fx_Body>();
        
        if (crBody != null)
        {
          Mesh colliderMesh;
          Matrix4x4 colliderMatrixModelToWorld;

          bool isBakedColliderMesh = crBody.GetColliderMesh(out colliderMesh, out colliderMatrixModelToWorld );
    
          if (colliderMesh != null)
          {
            AddMeshSpheres(colliderMesh, colliderMatrixModelToWorld, radius);

            if (isBakedColliderMesh)
            {
              UnityEngine.Object.DestroyImmediate(colliderMesh);
            }
          }
        }
      }
    }

    public void AddMeshSpheres( Mesh mesh, Matrix4x4 m_LOCAL_to_WORLD, float radius )
    {
      Vector3[] vertices = mesh.vertices;
      int[]     indices  = mesh.triangles;

      setUsedvertex_.Clear();
      int nIndices = indices.Length;

      for (int i = 0; i < nIndices; i+=3)
      {     
        if ( !spheresGenerator_.CanAddSphere8Faces6Vertices() )
        {
          fxData_.listMeshClothSpheres_.Add( spheresGenerator_.GenerateMesh() );
        }

        int vertexA = indices[i + 0];
        int vertexB = indices[i + 1];
        int vertexC = indices[i + 2];

        Vector3 center = (vertices[vertexA] +  vertices[vertexB] + vertices[vertexC]) / 3.0f ;

        Vector3 centerWORLD = m_LOCAL_to_WORLD.MultiplyPoint( center );
        spheresGenerator_.AddOrthoSphere8Faces6Vertices( centerWORLD, radius );    
      }

      foreach ( Vector3 vertex in vertices )
      {

        if ( !spheresGenerator_.CanAddSphere8Faces6Vertices() )
        {
          fxData_.listMeshClothSpheres_.Add( spheresGenerator_.GenerateMesh() );
        }

        Vector3 vertexWORLD = m_LOCAL_to_WORLD.MultiplyPoint( vertex );
        spheresGenerator_.AddOrthoSphere8Faces6Vertices( vertexWORLD, radius );        
      }

      if ( spheresGenerator_.CanGenerateMesh() )
      {
        fxData_.listMeshClothSpheres_.Add( spheresGenerator_.GenerateMesh() );
      }
    }


    public void CreateJointPivotBoxes()
    {
      if ( fxData_.DrawJoints && listJointGroupsInfo_ != null )
      {
        fxData_.ClearJointMeshes();

        int nJointGroups = listJointGroupsInfo_.Count;

        bool isReplaying = SimulationManager.IsReplaying();
        bool drawOnlySelected = fxData_.DrawOnlySelected;

        for (int i = 0; i < nJointGroups; i++)
        {
          ClearJointsBounds();
          JointGroupsInfo jgInfo = listJointGroupsInfo_[i];
          uint idJointGroups = jgInfo.idEntity_;

          List<JointPivotInfo> listJointPivot = jgInfo.listJointPivotInfo_;
          int nJointPivot = listJointPivot.Count;

          if ( isReplaying && drawOnlySelected && 
               !fxData_.listJointGroupsIdsSelected_.Contains(idJointGroups) &&
               !fxData_.listRigidGlueIdsSelected_.Contains(idJointGroups) )
          {
            continue;
          }

          for (int j = 0; j < nJointPivot; j++)
          {
            CreateJointPivotBounds(listJointPivot[j], fxData_.JointsSize);
          }

          GenerateJointsMeshes( idJointGroups );

        }

        SceneView.RepaintAll();
      }

      UpdateListJointsRequested = false;
    }

    private void ClearJointsBounds()
    {
      listJointPivotNormalIn_.Clear();
      listJointPivotNormalOut_.Clear();

      listJointPivotDeformatedIn_.Clear();
      listJointPivotDeformatedOut_.Clear();

      listJointPivotBreakingIn_.Clear();
      listJointPivotBreakingOut_.Clear();

      listJointPivotBrokenIn_.Clear();
      listJointPivotBrokenOut_.Clear();
    }

    private void GenerateJointsMeshes( uint idJointGroups )
    {
      CRBoxGenerator.GenerateBoxMeshesWithId(idJointGroups, listJointPivotNormalIn_,      fxData_.listMeshJointBoxesNormalIn_);
      CRBoxGenerator.GenerateBoxMeshesWithId(idJointGroups, listJointPivotNormalOut_,     fxData_.listMeshJointBoxesNormalOut_);
      CRBoxGenerator.GenerateBoxMeshesWithId(idJointGroups, listJointPivotDeformatedIn_,  fxData_.listMeshJointBoxesDeformatedIn_);
      CRBoxGenerator.GenerateBoxMeshesWithId(idJointGroups, listJointPivotDeformatedOut_, fxData_.listMeshJointBoxesDeformatedOut_);
      CRBoxGenerator.GenerateBoxMeshesWithId(idJointGroups, listJointPivotBreakingIn_,    fxData_.listMeshJointBoxesBreakingIn_);
      CRBoxGenerator.GenerateBoxMeshesWithId(idJointGroups, listJointPivotBreakingOut_,   fxData_.listMeshJointBoxesBreakingOut_);
      CRBoxGenerator.GenerateBoxMeshesWithId(idJointGroups, listJointPivotBrokenIn_,      fxData_.listMeshJointBoxesBrokenIn_);
      CRBoxGenerator.GenerateBoxMeshesWithId(idJointGroups, listJointPivotBrokenOut_,     fxData_.listMeshJointBoxesBrokenOut_);
    }

    private void CreateJointPivotBounds(JointPivotInfo jpInfo, float size)
    {

      List<Bounds> currentBoundsListIn  = null;
      List<Bounds> currentBoundsListOut = null;
      switch (jpInfo.pivotState_)
      {
        case ENUM_JOINT_PIVOT_STATE.JOINT_PIVOT_STATE_DEFORMATED_NO:
          currentBoundsListIn  = listJointPivotNormalIn_;
          currentBoundsListOut = listJointPivotNormalOut_;
          break;
        
        case ENUM_JOINT_PIVOT_STATE.JOINT_PIVOT_STATE_DEFORMATED_YES:
          currentBoundsListIn  = listJointPivotDeformatedIn_;
          currentBoundsListOut = listJointPivotNormalOut_;
          break;
        
        case ENUM_JOINT_PIVOT_STATE.JOINT_PIVOT_STATE_BREAKING:
          currentBoundsListIn  = listJointPivotBreakingIn_;
          currentBoundsListOut = listJointPivotBreakingOut_;
          break;
       
        case ENUM_JOINT_PIVOT_STATE.JOINT_PIVOT_STATE_BROKEN:
          currentBoundsListIn  = listJointPivotBrokenIn_;
          currentBoundsListOut = listJointPivotBrokenOut_;
          break;
        
        default:
          return;
      } 

      Vector3 sizeBoxA = ( Mathf.Log(size) / 10f ) * Vector3.one;
      Vector3 centerA  = new Vector3( jpInfo.posA_.x, jpInfo.posA_.y, jpInfo.posA_.z );

      Vector3 sizeBoxB = ( Mathf.Log(size) / 7f ) * Vector3.one;
      Vector3 centerB  = new Vector3( jpInfo.posB_.x, jpInfo.posB_.y, jpInfo.posB_.z );

      currentBoundsListIn .Add( new Bounds( centerA, sizeBoxA ) );
      currentBoundsListOut.Add( new Bounds( centerB, sizeBoxB ) );
    }

    public void RenderExplosions(float opacity)
    {
      bool isSimulating = CaronteSharp.SimulationManager.IsSimulating();
      bool isReplaying  = CaronteSharp.SimulationManager.IsReplaying();

      bool isSimulatingOrReplaying = (isSimulating || isReplaying);

      if (listExplosionInfo_ != null && ( (isSimulatingOrReplaying && frameNumber_ != 0) || !isSimulatingOrReplaying) ) 
      {
        int nExplosions = listExplosionInfo_.Count;

        for (int i = 0; i < nExplosions; i++)
        {
          ExplosionInfo exInfo = listExplosionInfo_[i];
          RenderExplosion(exInfo, opacity, isSimulatingOrReplaying);
        }
      }

    }
  
    private void RenderExplosion( ExplosionInfo exInfo, float opacity, bool isSimulatingOrReplaying )
    {
      CNExplosion exNode = entityManager_.GetExplosionNode(exInfo.idEntity_);
      if (exNode == null)
      {
        return;
      }

      Vector3 center = exInfo.center_;

      List<BeamInfo> arrBeamInfo = exInfo.listBeam_;
      int nBeams = arrBeamInfo.Count;

      Color red = Color.red;
      red.a = opacity;

      Color yellow = Color.yellow;
      yellow.a = opacity;

      Color cyan = Color.cyan;
      cyan.a = opacity;

      int step = (int)Mathf.Log(nBeams);

      step = exNode.RenderStepSize;
      Quaternion rotation = Quaternion.identity;

      if ( exNode.Explosion_Transform != null &&  !isSimulatingOrReplaying )
      {
        Transform tr = exNode.Explosion_Transform;
        center   = tr.position;
        rotation = tr.rotation;
      }

      for ( int i = 0; i < nBeams; i += step )
      {
        BeamInfo beamInfo = arrBeamInfo[i];

        Vector3 beam_u         = rotation * beamInfo.beam_u_;
        Vector3 segmentLengths = beamInfo.segmentLenths_;

        if (segmentLengths.x < float.Epsilon ) continue;
        Handles.color = red;
        Vector3 pos_A = center;
        Vector3 pos_B = pos_A + (beam_u * segmentLengths.x);
        Handles.DrawLine(pos_A, pos_B);

          
        if (segmentLengths.y < float.Epsilon ) continue;
        Handles.color = yellow;
        pos_A = pos_B;
        pos_B += beam_u * segmentLengths.y;
        Handles.DrawLine(pos_A, pos_B);
          

        if (segmentLengths.z < float.Epsilon) continue;
        Handles.color = cyan;
        pos_A = pos_B;
        pos_B += beam_u * segmentLengths.z;
        Handles.DrawLine(pos_A, pos_B);
      }
    }

    public void RenderStatistics()
    {
      GUIStyle styleStats = new GUIStyle(EditorStyles.miniBoldLabel);
      styleStats.normal.textColor = Color.blue;
      
      EditorGUILayout.LabelField("Statistics: ");

      Rect rect = GUILayoutUtility.GetRect(new GUIContent("Stats: "), EditorStyles.label );
      Rect testRect = new Rect(rect.xMin, rect.yMin, 110, 1);
      CRGUIUtils.Splitter( Color.black, testRect);

      uint built_nRigids_   = statistics_.built_nRigids_;
      uint built_nBodyMesh_ = statistics_.built_nBodyMesh_;
      uint built_nSoftbody_ = statistics_.built_nSoftbodies_;
      uint build_nCloth_    = statistics_.built_nCloth_;

      string nrigid        = built_nRigids_.ToString();
      string nirresponsive = built_nBodyMesh_.ToString();
      string nsoftbody     = built_nSoftbody_.ToString();
      string ncloth        = build_nCloth_.ToString();

      EditorGUILayout.LabelField("RigidBodies: ",  nrigid,        styleStats );
      EditorGUILayout.LabelField("Irresponsive: ", nirresponsive, styleStats );
      EditorGUILayout.LabelField("SoftBodies: ",   nsoftbody,     styleStats );
      EditorGUILayout.LabelField("ClothBodies: ",  ncloth,        styleStats );
      
      EditorGUILayout.Space();

      EditorGUILayout.LabelField("Joints: ", statistics_.jointGroupsInf_.nJoints_.ToString(), styleStats );
      EditorGUILayout.LabelField("Servos: ", statistics_.nServos_.ToString(), styleStats );

      GUILayout.FlexibleSpace();

      DrawTimers( styleStats );
      EditorGUILayout.Space();
      EditorGUILayout.Space();
      EditorGUILayout.Space();
    }

    private void DrawTimers(GUIStyle styleStats)
    {
      bool isSimulating = SimulationManager.IsSimulating();
      bool isReplaying  = SimulationManager.IsReplaying();
      bool isSimulatingOrReplaying = isSimulating || isReplaying;
      TimeSpan tRT;
      TimeSpan tSim;
      if (isSimulatingOrReplaying)
      {
        tRT  = TimeSpan.FromSeconds( SimulationManager.GetSimulatingRealTime() );
        tSim = TimeSpan.FromSeconds( SimulationManager.GetTimeSimulated() );


        SceneView.RepaintAll();
      }
      else
      {
        tRT  = TimeSpan.FromSeconds( 0 );
        tSim = TimeSpan.FromSeconds( 0 );
      }

      string tRTString = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", 
                                         tRT.Hours, 
                                         tRT.Minutes, 
                                         tRT.Seconds, 
                                         tRT.Milliseconds);
      EditorGUILayout.LabelField("Real time: ", tRTString, styleStats);

      string tSimString = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", 
                                         tSim.Hours, 
                                         tSim.Minutes, 
                                         tSim.Seconds, 
                                         tSim.Milliseconds);
      EditorGUILayout.LabelField("Simulated time: ", tSimString, styleStats);
    }

    public void RenderContactEvents(float size)
    {
      if (listContactEventInfo_ != null)
      {
        int nContactEvents = listContactEventInfo_.Count;

        for (int i = 0; i < nContactEvents; i++)
        {
          ContactEventInfo ceInfo = listContactEventInfo_[i];
          RenderContactEvent(ceInfo, size);
        }
      }
    }

    private void RenderContactEvent(ContactEventInfo ceInfo, float size)
    {
      Handles.color = Color.red;
      Handles.SphereCap(0,ceInfo.position_, Quaternion.identity, size);
    }




  }

} //namespace Caronte