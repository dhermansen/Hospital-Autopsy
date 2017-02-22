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
using System.IO;
using CaronteFX.AnimationFlags;
using CaronteSharp;

namespace CaronteFX
{
  using TGOFrameData    = Tuple2<Transform, CRGOKeyframe>;

  /// <summary>
  /// Used to bake Caronte Simulations.
  /// </summary>
  public class CRBakerGOAnim
  { 
    public enum AnimationFileType
    {
      CRAnimationAsset,
      TextAsset
    }
    //-----------------------------------------------------------------------------------
    //--------------------------------DATA MEMBERS---------------------------------------
    //-----------------------------------------------------------------------------------
    const int binaryVersion_ = 9;
    
    CNManager          manager_;
    CREntityManager    entityManager_;
    CRPlayer           player_;

    CRMeshCombiner     skinnedMeshCombiner_;
    CRMeshCombiner     frameMeshCombiner_;

    UnityEngine.Object meshesPrefab_;
    String             assetsPath_;
    String             fxName_;
    
    Dictionary<uint, string>        idBodyToRelativePath_;
    Dictionary<uint, string[]>      idBodyToBonesRelativePaths_;
    Dictionary<uint, GameObject>    idBodyToBakedGO_;
    Dictionary<uint, CRGOKeyframe>  idBodyToGOKeyframe_;
    Dictionary<uint, Vector2>       idBodyToVisibilityInterval_;

    Dictionary<Mesh, Mesh>          originalMeshToBakedMesh_;

    List<uint>       listIdBodyTmp_;
    List<GameObject> listGOTmp_;

    List<Transform>  listTransformTmp_;
    List<BoneWeight> listBoneWeightTmp_;
    List<Matrix4x4>  listBindPoseTmp_;

    HashSet<uint> setBoneBodies_;
    HashSet<uint> setVisibleBodies_;

    MeshComplex meshComplexForUpdate_ = new MeshComplex();

    int   frame_;
    int   bakingFrame_;

    int   frameCount_;

    int   frameStart_ = -1;
    int   frameEnd_   = -1;

    float visibilityShift_    = 0;
    float visibilityRangeMin_ = 0;
    float visibilityRangeMax_ = 0;

    public string animationName_;
    public string bakeObjectName_;
    public string bakeObjectPrefix_;

    public bool skinRopes_     = true;
    public bool skinClothes_   = false;

    public bool vertexCompression_    = true;
    public int  vertexCompressionIdx_ = 0;
    public bool vertexTangents_       = false;

    public bool bakeEvents_     = true;
    public bool bakeVisibility_ = true;
    public bool alignData_      = true;

    public bool bakeAllNodes_         = true;
    public bool combineMeshesInFrame_ = false;

    public Transform optionalRootTransform_ = null;
    public List<CNBody> listBodyNode_ = new List<CNBody>();

    public BitArray bitArrNeedsBaking_;
    public BitArray bitArrNeedsCollapsing_;

    public AnimationFileType animationFileType_ = AnimationFileType.TextAsset;
    public bool preserveCFXInFrameBake_ = false;

    GameObject  rootGameObject_;
    //-----------------------------------------------------------------------------------
    public int FrameStart
    {
      get
      {
        return frameStart_;
      }
      set
      {
        frameStart_ = Mathf.Clamp(value, 0, player_.MaxFrames);
      }
    }

    public int FrameEnd
    {
      get
      {
        return frameEnd_;
      }
      set
      {
        frameEnd_ = Mathf.Clamp(value, 0, player_.MaxFrames);
      }
    }

    public int MaxFrames
    {
      get
      {
        return ( player_.MaxFrames );
      }
    }

    public float FrameTime
    {
      get
      {
        return ( player_.FrameTime ); 
      }
    }

    public int FPS
    {
      get
      {
        return ( player_.FPS );
      }
    }

    private UnityEngine.Object MeshesPrefab
    {
      get
      {
        if (meshesPrefab_ == null)
        {
          string meshesPrefabPath = AssetDatabase.GenerateUniqueAssetPath(assetsPath_ + "/" + animationName_ +  "_meshes.prefab");
          meshesPrefab_ = PrefabUtility.CreateEmptyPrefab(meshesPrefabPath);
        }
        return meshesPrefab_;
      }
    }
    //----------------------------------------------------------------------------------
    public CRBakerGOAnim( CNManager manager, CREntityManager entityManager, CRPlayer player )
    {
      manager_       = manager;
      entityManager_ = entityManager;
      player_        = player;

      skinnedMeshCombiner_ = new CRMeshCombiner(true);
      frameMeshCombiner_   = new CRMeshCombiner(false);

      meshesPrefab_ = null;
      assetsPath_   = string.Empty;

      fxName_ = string.Empty;

      idBodyToRelativePath_       = new Dictionary<uint, string>();
      idBodyToBonesRelativePaths_ = new Dictionary<uint,string[]>();
      idBodyToBakedGO_            = new Dictionary<uint, GameObject>();
      idBodyToGOKeyframe_         = new Dictionary<uint, CRGOKeyframe>();
      idBodyToVisibilityInterval_ = new Dictionary<uint, Vector2>();

      originalMeshToBakedMesh_ = new Dictionary<Mesh,Mesh>();

      listIdBodyTmp_     = new List<uint>();
      listGOTmp_         = new List<GameObject>();

      listTransformTmp_  = new List<Transform>();
      listBoneWeightTmp_ = new List<BoneWeight>();
      listBindPoseTmp_   = new List<Matrix4x4>();

      setBoneBodies_    = new HashSet<uint>();
      setVisibleBodies_ = new HashSet<uint>();

      preserveCFXInFrameBake_ = false;
    }
    //----------------------------------------------------------------------------------
    public void BuildBakerInitData()
    {
      string fxDataName = manager_.FxData.name;

      animationName_    = fxDataName + "_anim";
      bakeObjectName_   = fxDataName + "_baked";
      bakeObjectPrefix_ = "snapshot";

      frameStart_ = 0;
      frameEnd_   = player_.MaxFrames;

      BuildListNodesForBaking();
    }
    //----------------------------------------------------------------------------------
    private void BuildListNodesForBaking()
    {
      manager_.GetListBodyNodesForBake( listBodyNode_ );

      int nBodyNodes = listBodyNode_.Count;

      bitArrNeedsBaking_     = new BitArray(nBodyNodes, true);
      bitArrNeedsCollapsing_ = new BitArray(nBodyNodes, true);

      for (int i = 0; i < nBodyNodes; i++ )
      {
        CNBody bodyNode = listBodyNode_[i];
        int nBodies = entityManager_.GetNumberOfBodiesFromBodyNode(bodyNode);

        bool isRigidbody = bodyNode is CNRigidbody;
        bool isAnimated  = bodyNode is CNAnimatedbody;

        bitArrNeedsBaking_[i]     = !isAnimated;
        bitArrNeedsCollapsing_[i] = ( (nBodies >= 5) && (isRigidbody && !isAnimated) );
      }  
    }
    //----------------------------------------------------------------------------------
    private List< Tuple2<CNBody, bool> > GetListBodyNodeToBake()
    {
      List< Tuple2<CNBody, bool> > listBodyNodesToBake = new List< Tuple2<CNBody, bool> >();
      int nBodies = listBodyNode_.Count;

      for (int i = 0; i < nBodies; i++)
      {
        CNBody bodyNode = listBodyNode_[i];

        bool needsBake       = bitArrNeedsBaking_[i];
        bool needsCollapsing = bitArrNeedsCollapsing_[i];

        if (needsBake)
        {
          if ( bodyNode is CNRigidbody )
          {
            listBodyNodesToBake.Add( Tuple2.New(bodyNode, needsCollapsing) );
          }
          else
          {
            listBodyNodesToBake.Add( Tuple2.New(bodyNode, false) );
          }      
        }
      }

      return listBodyNodesToBake;
    }
    //----------------------------------------------------------------------------------
    public void ClearData()
    {
      skinnedMeshCombiner_.Clear();
      frameMeshCombiner_  .Clear();

      meshesPrefab_ = null;
      assetsPath_   = string.Empty;
      fxName_       = string.Empty;

      idBodyToRelativePath_      .Clear();
      idBodyToBonesRelativePaths_.Clear();
      idBodyToBakedGO_           .Clear();
      idBodyToGOKeyframe_        .Clear();
      idBodyToVisibilityInterval_.Clear();

      originalMeshToBakedMesh_.Clear();

      listIdBodyTmp_    .Clear();
      listGOTmp_        .Clear();

      listTransformTmp_ .Clear();
      listBoneWeightTmp_.Clear();
      listBindPoseTmp_  .Clear();

      setBoneBodies_   .Clear();
      setVisibleBodies_.Clear();
    }
    //----------------------------------------------------------------------------------
    public void BakeSimulationAsAnim()
    {
      string fxName = manager_.FxData.name;

      frameCount_ = (frameEnd_ - frameStart_ + 1);
      
      visibilityShift_ = FrameTime * frameStart_;

      visibilityRangeMin_ = 0f;
      visibilityRangeMax_ = (frameCount_ - 1) * FrameTime;

      List< Tuple2<CNBody, bool> > listBodyNodeToBake = GetListBodyNodeToBake();

      string folder;
      int pathIndex;
      bool assetsPath = CRFileUtils.DisplaySaveFolderDialog("Animation assets folder...", out folder, out pathIndex );
      if (!assetsPath)
      {
        return;
      }
     
      InitBake(folder, pathIndex, fxName);
      CreateRootGameObject(bakeObjectName_);
      CheckBodiesVisibility();   
      CreateBakedObjects(listBodyNodeToBake);   
      BakeAnimBinaryFile();
      SetStartData();
      FinishBake();   
    }
    //----------------------------------------------------------------------------------
    public void BakeCurrentFrame()
    {
      string fxName = manager_.FxData.name + "_frame_" + player_.Frame;
      List< Tuple2<CNBody, bool> > listBodyNodeToBake = GetListBodyNodeToBake();

      string folder;
      int pathIndex;
      bool assetsPath = CRFileUtils.DisplaySaveFolderDialog("Frame assets folder...", out folder, out pathIndex);
      if (!assetsPath)
      {
        return;
      }
     
      InitBake(folder, pathIndex, fxName);
      CreateRootGameObject(bakeObjectName_ + "_frame_" + player_.Frame);
      rootGameObject_.AddComponent<CRAnimation>();
      CreateFrameBakedObjects(listBodyNodeToBake);     
      FinishBake();
    }
    //----------------------------------------------------------------------------------
    private void InitBake(string folderPath, int pathIdx, string fxName)
    {
      ClearData();
      assetsPath_ = folderPath.Substring(pathIdx);
      fxName_ = fxName;
      SimulationManager.SetBroadcastMode( UN_BROADCAST_MODE.BAKING );  
      SimulationManager.PauseHotOn();
    }
    //----------------------------------------------------------------------------------
    private void FinishBake()
    {      
      SimulationManager.PauseOn();
      SimulationManager.SetBroadcastMode( UN_BROADCAST_MODE.SIMULATING );
      ClearData();
      EditorGUIUtility.PingObject(rootGameObject_);
      Selection.activeGameObject = rootGameObject_;
    }      
    //----------------------------------------------------------------------------------
    private void CreateRootGameObject(string bakeObjectName)
    {
      string uniqueName = CREditorUtils.GetUniqueGameObjectName(bakeObjectName);
      rootGameObject_   = new GameObject(uniqueName);

      if (optionalRootTransform_ != null)
      {
        Transform rootTr = rootGameObject_.transform;
        rootTr.localPosition = optionalRootTransform_.position;
        rootTr.localRotation = optionalRootTransform_.rotation;
        rootTr.localScale    = Vector3.one;
      }
    }
    //----------------------------------------------------------------------------------
    private void CreateBakedObjects(List< Tuple2<CNBody, bool> > listBodyNodeToBake)
    {
      listIdBodyTmp_.Clear();
      int nBodyNodes = listBodyNodeToBake.Count;

      for (int i = 0; i < nBodyNodes; i++)
      {
        Tuple2<CNBody, bool> tupleBodyNodeNeedsCollapsing_ = listBodyNodeToBake[i]; 

        CNBody bodyNode      = tupleBodyNodeNeedsCollapsing_.First;
        bool needsCollapsing = tupleBodyNodeNeedsCollapsing_.Second;

        bool isRope  = bodyNode as CNRope;
        bool isCloth = bodyNode as CNCloth;

        entityManager_.BuildListBodyIdFromBodyNode( bodyNode, listIdBodyTmp_ );

        if (listIdBodyTmp_.Count > 0)
        {
          string bodyNodeName = i + "_" + bodyNode.Name;

          GameObject nodeGO = new GameObject(bodyNodeName);
          nodeGO.transform.parent = rootGameObject_.transform;

          if (needsCollapsing)
          {
            CreateRgSkinnedGameObjects(bodyNodeName, bodyNode, listIdBodyTmp_, nodeGO);
          }
          else
          {
            bool isRopeSkinned  = isRope && skinRopes_;
            bool isClothSkinned = isCloth && skinClothes_;
            if (isRopeSkinned)
            {
              CreateRopeSkinnedGameObjects(bodyNodeName, bodyNode, listIdBodyTmp_, nodeGO);
            }
            else if (isClothSkinned)
            {
              CreateClothSkinnedGameObjects(bodyNodeName, bodyNode, listIdBodyTmp_, nodeGO);
            }
            else
            {
              CreateBakedGameObjects(bodyNodeName, bodyNode, listIdBodyTmp_, nodeGO);
            }
          }
        }
      }

      InitKeyframeData();
    }
    //----------------------------------------------------------------------------------
    private void CreateFrameBakedObjects(List< Tuple2<CNBody, bool> > listBodyNodeToBake)
    {
      listIdBodyTmp_.Clear();
      int nBodyNodes = listBodyNodeToBake.Count;

      List<uint> listIdBodyTotal = new List<uint>();

      for (int i = 0; i < nBodyNodes; i++)
      {
        Tuple2<CNBody, bool> tupleBodyNodeNeedsCollapsing_ = listBodyNodeToBake[i]; 

        CNBody bodyNode = tupleBodyNodeNeedsCollapsing_.First;
        entityManager_.BuildListBodyIdFromBodyNode( bodyNode, listIdBodyTmp_ );
        listIdBodyTotal.AddRange(listIdBodyTmp_);
      }

      Matrix4x4 m_WORLD_to_LOCALCOMBINED = CalculateWorldToLocalMatrixSimulating(listIdBodyTotal);
      Vector3 position = -( m_WORLD_to_LOCALCOMBINED.GetColumn(3) );

      frameMeshCombiner_.SetWorldToLocalClearingInfo( m_WORLD_to_LOCALCOMBINED );

      for (int i = 0; i < nBodyNodes; i++)
      {
        Tuple2<CNBody, bool> tupleBodyNodeNeedsCollapsing_ = listBodyNodeToBake[i]; 

        CNBody bodyNode = tupleBodyNodeNeedsCollapsing_.First;

        entityManager_.BuildListBodyIdFromBodyNode( bodyNode, listIdBodyTmp_ );

        if (listIdBodyTmp_.Count > 0)
        {
          if (combineMeshesInFrame_)
          {
            CreateFrameBakedGameObjectsCombined(listIdBodyTmp_, position);
          }
          else
          {
            string bodyNodeName = bodyNode.Name;

            string prefix = string.Empty;
            if (bakeObjectPrefix_ != string.Empty)
            {
              prefix += bakeObjectPrefix_ + "_";
            }

            GameObject nodeGO = new GameObject( prefix + bodyNodeName);

            nodeGO.transform.parent = rootGameObject_.transform;
            CreateFrameBakedGameObjects(listIdBodyTmp_, nodeGO, prefix);
          }        
        }
      }

      if ( frameMeshCombiner_.CanGenerateMesh() )
      {
        GenerateCombinedGameObject(position);
      }

      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh(); 
    }
    //----------------------------------------------------------------------------------
    private void CreateBakedGameObjects(string bodyNodeName, CNBody bodyNode, List<uint> listBodyId, GameObject nodeGO)
    {
      foreach( var idBody in listBodyId )
      { 
        GameObject originalGO = entityManager_.GetGOFromIdBody(idBody);

        if (!setVisibleBodies_.Contains(idBody) || originalGO == null ) 
        {
          continue;
        }

        GameObject bakedGO = (GameObject)UnityEngine.Object.Instantiate(originalGO);
        SetBakedGameObject( bakedGO, originalGO, nodeGO, idBody, listGOTmp_ );

        if ( bakedGO.HasMesh() )
        {
          bool isRope = entityManager_.IsRope(idBody);

          if ( isRope )
          {
            Tuple2<Mesh, Vector3> ropeInit = entityManager_.GetRopeInit(idBody);
            Vector3 center                 = ropeInit.Second;
            CREditorUtils.SetMesh(bakedGO, ropeInit.First);

            bakedGO.transform.localPosition = center;
            bakedGO.transform.localRotation = Quaternion.identity;
            bakedGO.transform.localScale    = Vector3.one;

            AssetDatabase.AddObjectToAsset(ropeInit.First, MeshesPrefab);
          }
          else
          {
            Mesh mesh = bakedGO.GetMesh();

            bool isSoftbody = entityManager_.IsSoftbody(idBody);
            bool isCloth    = entityManager_.IsCloth(idBody);
            bool isInProject = AssetDatabase.Contains( mesh.GetInstanceID() );

            if (isSoftbody || isCloth || !isInProject)
            {
              Mesh bakedMesh;
              if ( originalMeshToBakedMesh_.ContainsKey(mesh) )
              {
                bakedMesh = originalMeshToBakedMesh_[mesh];
              }
              else
              {
                bakedMesh = UnityEngine.Object.Instantiate(mesh);
                bakedMesh.name = mesh.name;
                originalMeshToBakedMesh_.Add(mesh, bakedMesh);
                AssetDatabase.AddObjectToAsset(bakedMesh, MeshesPrefab);
              }

              CREditorUtils.SetMesh(bakedGO, bakedMesh);
            }
          }
        }

        string relativePath = bodyNodeName + "/" + bakedGO.name;
        idBodyToRelativePath_.Add(idBody, relativePath);
        idBodyToBakedGO_     .Add(idBody, bakedGO);
      }
    }
    //----------------------------------------------------------------------------------
    private void CreateRgSkinnedGameObjects( string bodyNodeName, CNBody bodyNode, List<uint> listBodyId, GameObject nodeGO )
    { 
      Matrix4x4 m_WORLD_to_LOCALCOMBINED = CalculateWorldToLocalMatrix(listBodyId);
      skinnedMeshCombiner_.SetWorldToLocalClearingInfo( m_WORLD_to_LOCALCOMBINED );

      GameObject bonesGO = new GameObject(nodeGO.name + "_bones"); 
      bonesGO.transform.parent = nodeGO.transform;

      int skinnedIdx = 0;

      foreach( var idBody in listBodyId )
      {                 
        GameObject originalGO = entityManager_.GetGOFromIdBody(idBody);

        if (!setVisibleBodies_.Contains(idBody) || originalGO == null ) 
        {
          continue;
        }

        GameObject bakedBone  = new GameObject();
        SetBakedBoneGameObject( bakedBone, originalGO, nodeGO, idBody );

        bakedBone.transform.parent = bonesGO.transform;

        if (!skinnedMeshCombiner_.CanAddGameObject( originalGO ) )
        {
          GenerateSkinGameObject(nodeGO, skinnedIdx);
          skinnedIdx++;
        }
        skinnedMeshCombiner_.AddGameObject( originalGO, bakedBone );
        string relativePath = bodyNodeName + "/" + bonesGO.name + "/" + bakedBone.name;

        idBodyToRelativePath_.Add(idBody, relativePath);
        idBodyToBakedGO_     .Add(idBody, bakedBone);

        setBoneBodies_.Add(idBody);
      }

      if( skinnedMeshCombiner_.CanGenerateMesh() )
      {
        GenerateSkinGameObject(nodeGO, skinnedIdx );
      } 
    }
    //----------------------------------------------------------------------------------
    private void GenerateSkinGameObject(GameObject nodeGO, int skinnedIdx)
    {
      Material[] arrMaterial;
      Transform[] arrBone;

      Mesh skinnedMesh = skinnedMeshCombiner_.GenerateMesh(out arrMaterial, out arrBone);
      skinnedMesh.name = nodeGO.name + "_" + skinnedIdx;

      GameObject go = new GameObject(nodeGO.name + "_skinned_" + skinnedIdx); 
      go.transform.parent = nodeGO.transform;

      SkinnedMeshRenderer smr = go.AddComponent<SkinnedMeshRenderer>();

      smr.bones               = arrBone;
      smr.sharedMesh          = skinnedMesh;   
      smr.sharedMaterials     = arrMaterial;
      smr.updateWhenOffscreen = true;

      Bounds worldBounds    = smr.bounds;
      go.transform.position = worldBounds.center;

      Mesh newMesh = UnityEngine.Object.Instantiate(skinnedMesh);
      newMesh.name = skinnedMesh.name;

      CREditorUtils.SetMesh(go, newMesh);
      AssetDatabase.AddObjectToAsset(newMesh, MeshesPrefab);
      
      skinnedMeshCombiner_.Clear();
    }
    //----------------------------------------------------------------------------------
    private void CreateRopeSkinnedGameObjects( string bodyNodeName, CNBody bodyNode, List<uint> listBodyId, GameObject nodeGO )
    { 
      foreach( var idBody in listBodyId )
      { 
        GameObject originalGO = entityManager_.GetGOFromIdBody(idBody);

        if (!setVisibleBodies_.Contains(idBody) || originalGO == null) 
        {
          continue;
        }

        GameObject bakedGO = (GameObject)UnityEngine.Object.Instantiate(originalGO);
        
        GameObject  rootBoneGameObject;
        Transform[] boneTransforms;
        SetSkinnedRopeGameObject( bakedGO, originalGO, nodeGO, idBody, listGOTmp_, out rootBoneGameObject, out boneTransforms );

        int nBones = boneTransforms.Length;

        string relativePath = bodyNodeName + "/" + rootBoneGameObject.name;
        idBodyToRelativePath_.Add(idBody, relativePath);

        string[] arrBoneRelativePath = new string[nBones];
        for (int i = 0; i < nBones; i++)
        {
          Transform tr = boneTransforms[i];
          arrBoneRelativePath[i] = relativePath + "/" + tr.gameObject.name;
        }

        idBodyToBakedGO_           .Add(idBody, rootBoneGameObject);
        idBodyToBonesRelativePaths_.Add(idBody, arrBoneRelativePath);
        setBoneBodies_             .Add(idBody);
      }
    }
    //----------------------------------------------------------------------------------
    private void CreateClothSkinnedGameObjects( string bodyNodeName, CNBody bodyNode, List<uint> listBodyId, GameObject nodeGO )
    { 
      foreach( var idBody in listBodyId )
      { 
        GameObject originalGO = entityManager_.GetGOFromIdBody(idBody);

        if (!setVisibleBodies_.Contains(idBody) || originalGO == null) 
        {
          continue;
        }

        GameObject bakedGO = (GameObject)UnityEngine.Object.Instantiate(originalGO);
        
        GameObject  rootBoneGameObject;
        Transform[] boneTransforms;
        SetSkinnedClothGameObject( bakedGO, originalGO, nodeGO, idBody, listGOTmp_, out rootBoneGameObject, out boneTransforms );

        int nBones = boneTransforms.Length;

        string relativePath = bodyNodeName + "/" + rootBoneGameObject.name;
        idBodyToRelativePath_.Add(idBody, relativePath);

        string[] arrBoneRelativePath = new string[nBones];
        for (int i = 0; i < nBones; i++)
        {
          Transform tr = boneTransforms[i];
          arrBoneRelativePath[i] = relativePath + "/" + tr.gameObject.name;
        }

        idBodyToBakedGO_           .Add(idBody, rootBoneGameObject);
        idBodyToBonesRelativePaths_.Add(idBody, arrBoneRelativePath);
        setBoneBodies_             .Add(idBody);
      }
    }
    //----------------------------------------------------------------------------------
    private void CreateFrameBakedGameObjects(List<uint> listBodyId, GameObject nodeGO, string prefix)
    {
      foreach( var idBody in listBodyId )
      { 
        Transform simulatingTr = entityManager_.GetBodyTransformRef(idBody);

        GameObject simulatingGO = simulatingTr.gameObject;
        if (simulatingGO.activeSelf)
        {
          GameObject bakedGO  = (GameObject)UnityEngine.Object.Instantiate(simulatingGO);

          bakedGO.name = prefix + simulatingGO.name;
          bakedGO.hideFlags = HideFlags.None;

          Caronte_Fx_Body fxBody = bakedGO.GetComponent<Caronte_Fx_Body>();
          if (fxBody != null && !preserveCFXInFrameBake_)
          {
            UnityEngine.Object.DestroyImmediate(fxBody);
          }

          bakedGO.transform.parent = nodeGO.transform;
          if ( bakedGO.HasMesh() )
          {
            Mesh mesh = bakedGO.GetMesh();
            bool isInProject = AssetDatabase.Contains( mesh.GetInstanceID() );
            
            if ( entityManager_.IsRope(idBody) )
            {
              Tuple2<Mesh, MeshUpdater> meshUpdater = entityManager_.GetBodyMeshRenderUpdaterRef(idBody);
              Mesh newMesh = UnityEngine.Object.Instantiate(meshUpdater.First);
              CREditorUtils.SetMesh(bakedGO, newMesh);
              AssetDatabase.AddObjectToAsset(newMesh, MeshesPrefab);
            }
            else if (!isInProject)
            {
              Mesh newMesh = UnityEngine.Object.Instantiate(mesh);
              newMesh.name = mesh.name;
              newMesh.hideFlags = HideFlags.None;
              CREditorUtils.SetMesh(bakedGO, newMesh);
              AssetDatabase.AddObjectToAsset(newMesh, MeshesPrefab);
            }
          }
        }

      }
    }
    //----------------------------------------------------------------------------------
    private void CreateFrameBakedGameObjectsCombined(List<uint> listBodyId, Vector3 position)
    {
      foreach( var idBody in listBodyId )
      { 
        Transform simulatingTr  = entityManager_.GetBodyTransformRef(idBody);
        GameObject simulatingGO = simulatingTr.gameObject;

        if (simulatingGO.activeSelf)
        {
          if (!frameMeshCombiner_.CanAddGameObject( simulatingGO ) )
          {
            GenerateCombinedGameObject(position);
          }

          frameMeshCombiner_.AddGameObject( simulatingGO );
        }
      }
    }
    //----------------------------------------------------------------------------------
    private void GenerateCombinedGameObject(Vector3 position)
    {
      Material[] arrMaterial;;

      Mesh mesh = frameMeshCombiner_.GenerateMesh(out arrMaterial);
      mesh.name = fxName_;

      GameObject go = new GameObject(fxName_ + "_combined"); 
      go.transform.parent = rootGameObject_.transform;

      MeshFilter mf = go.AddComponent<MeshFilter>();
      mf.sharedMesh = mesh;

      MeshRenderer mr = go.AddComponent<MeshRenderer>();  
      mr.sharedMaterials = arrMaterial;
 
      go.transform.position = position;

      AssetDatabase.AddObjectToAsset(mesh, MeshesPrefab);
      
      frameMeshCombiner_.Clear();
    }
    //----------------------------------------------------------------------------------
    private Matrix4x4 CalculateWorldToLocalMatrix(List<uint> listBodyId)
    {
      Bounds box_WORLD = new Bounds();
      int nBodies = listBodyId.Count;

      for( int a = 0; a < nBodies; a++ )
      {        
        uint idBody = listBodyId[a];
        GameObject originalGO = entityManager_.GetGOFromIdBody(idBody);

        if ( originalGO == null )
        {
          continue;
        }

        Mesh mesh = originalGO.GetMesh();

        if ( mesh == null )
        {
          continue;
        }

        Renderer rd = originalGO.GetComponent<Renderer>();
        if ( rd == null )
        {
          continue;
        }

        Matrix4x4 m_MODEL_to_WORLD = originalGO.transform.localToWorldMatrix;
        Bounds bounds_world = new Bounds();
        CRGeometryUtils.CreateBoundsTransformed( mesh.bounds, m_MODEL_to_WORLD, out bounds_world );

        if (a == 0)
        {
          box_WORLD = bounds_world;
        }
        else
        {
          box_WORLD.Encapsulate( bounds_world );
        }
      }

      Matrix4x4 matrix_WORLD_to_LOCAL = new Matrix4x4();
      matrix_WORLD_to_LOCAL.SetTRS( -box_WORLD.center, Quaternion.identity, Vector3.one );

      return matrix_WORLD_to_LOCAL;
    }
    //----------------------------------------------------------------------------------
    private Matrix4x4 CalculateWorldToLocalMatrixSimulating(List<uint> listBodyId)
    {
      Bounds box_WORLD = new Bounds();
      int nBodies = listBodyId.Count;

      for( int a = 0; a < nBodies; a++ )
      {        
        uint idBody = listBodyId[a];
        Transform simulatingTr = entityManager_.GetBodyTransformRef(idBody);
        if ( simulatingTr == null )
        {
          continue;
        }

        GameObject simulatingGO = simulatingTr.gameObject;
        Mesh mesh = simulatingGO.GetMesh();

        if ( mesh == null )
        {
          continue;
        }

        Renderer rd = simulatingGO.GetComponent<Renderer>();
        if ( rd == null )
        {
          continue;
        }

        Bounds bounds_world = rd.bounds;

        if (a == 0)
        {
          box_WORLD = bounds_world;
        }
        else
        {
          box_WORLD.Encapsulate( bounds_world );
        }
      }

      Matrix4x4 matrix_WORLD_to_LOCAL = new Matrix4x4();
      matrix_WORLD_to_LOCAL.SetTRS( -box_WORLD.center, Quaternion.identity, Vector3.one );

      return matrix_WORLD_to_LOCAL;
    }
    //----------------------------------------------------------------------------------
    private void InitKeyframeData()
    {
      SimulationManager.SetReplayingFrame( (uint)frameStart_, false );
      bool read = false;
      do
      {
        System.Threading.Thread.Sleep( 5 );
        read = SimulationManager.ReadSimulationBufferUniqueUnsafe( BroadcastStartDelegate, InitKeyFrame );
      } while (!read);
    }
    //----------------------------------------------------------------------------------
    private void BakeFrame(int frame)
    {
      SimulationManager.SetReplayingFrame( (uint)frame, false );        
      bool read = false;
      do
      {
        System.Threading.Thread.Sleep( 5 );
        read = SimulationManager.ReadSimulationBufferUniqueUnsafe( BroadcastStartDelegate, BakeBodyKeyFrame );
      } while (!read);
    }
    //----------------------------------------------------------------------------------
    private void CheckVisibility(int frame)
    {
      SimulationManager.SetReplayingFrame( (uint)frame, false );
      bool read = false;
      do
      {
        System.Threading.Thread.Sleep( 5 );
        read = SimulationManager.ReadSimulationBufferUniqueUnsafe( BroadcastStartDelegate, CheckBodyVisibility );
      } while (!read);
    }
    //----------------------------------------------------------------------------------
    private void BroadcastStartDelegate( bool doDisplayInvisibleBodies )
    {

    }
    //----------------------------------------------------------------------------------
    private void SetBakedGameObject( GameObject bakedGO, GameObject originalGO, GameObject nodeGO, uint idBody, List<GameObject> listGameObjectToDestroy )
    {
      listGameObjectToDestroy.Clear();
      entityManager_.GetRemoveBodyChildrenList( originalGO, bakedGO, listGameObjectToDestroy );
      foreach( GameObject go in listGameObjectToDestroy )
      {
        UnityEngine.Object.DestroyImmediate(go);
      }
      listGameObjectToDestroy.Clear();

      bakedGO.name = (idBody + 1).ToString() + "_" + originalGO.name;

      bakedGO.transform.parent        = originalGO.transform.parent;
      bakedGO.transform.localPosition = originalGO.transform.localPosition;
      bakedGO.transform.localRotation = originalGO.transform.localRotation;
      bakedGO.transform.localScale    = originalGO.transform.localScale;

      bakedGO.transform.parent = nodeGO.transform;

      Mesh bakedMesh;
      CREditorUtils.ReplaceSkinnedMeshRendererForMeshRenderer( bakedGO, out bakedMesh );
      if (bakedMesh != null)
      {
        AssetDatabase.AddObjectToAsset(bakedMesh, MeshesPrefab);
      }
     
      Transform simTransform = entityManager_.GetBodyTransformRef(idBody);
      bakedGO.SetActive( simTransform.gameObject.activeInHierarchy );

      Caronte_Fx_Body cfxBody = bakedGO.GetComponent<Caronte_Fx_Body>();
      if (cfxBody != null)
      {
        UnityEngine.Object.DestroyImmediate( cfxBody );
      }
    }
    //----------------------------------------------------------------------------------
    private void SetSkinnedRopeGameObject( GameObject bakedGO, GameObject originalGO, GameObject nodeGO, uint idBody, List<GameObject> listAuxGameObjectToDestroy,
                                           out GameObject rootBonesGameObject, out Transform[] arrBoneTransform )
    {
      listAuxGameObjectToDestroy.Clear();
      entityManager_.GetRemoveBodyChildrenList( originalGO, bakedGO, listAuxGameObjectToDestroy );
      foreach( GameObject go in listAuxGameObjectToDestroy )
      {
        UnityEngine.Object.DestroyImmediate(go);
      }
      listAuxGameObjectToDestroy.Clear();

      bakedGO.name = (idBody + 1).ToString() + "_" + originalGO.name;

      CREditorUtils.ReplaceMeshRendererForSkinnedMeshRenderer(bakedGO);

      Tuple2<Mesh, Vector3> ropeInit = entityManager_.GetRopeInit(idBody);   
 
      Mesh meshInit = ropeInit.First;

      Vector3 center;
      MeshComplex meshComplex = RopeManager.GetMeshRenderStraight(idBody, out center);
      
      Mesh meshInitStraight  = new Mesh();
      meshInitStraight.name = meshInit.name;

      meshInitStraight.vertices  = meshComplex.arrPosition_;
      meshInitStraight.normals   = meshComplex.arrNormal_;
      meshInitStraight.tangents  = meshComplex.arrTan_;
      meshInitStraight.uv        = meshComplex.arrUV_;
      meshInitStraight.triangles = meshComplex.arrIndex_;

      bakedGO.transform.localPosition = center;
      bakedGO.transform.localRotation = Quaternion.identity;
      bakedGO.transform.localScale    = Vector3.one;

      bakedGO.transform.parent = nodeGO.transform;

      InitSkinnedRope(idBody, bakedGO, nodeGO, meshInitStraight, out rootBonesGameObject, out arrBoneTransform);

      bakedGO.SetActive( true );

      Caronte_Fx_Body cfxBody = bakedGO.GetComponent<Caronte_Fx_Body>();
      if (cfxBody != null)
      {
        UnityEngine.Object.DestroyImmediate( cfxBody );
      }
    }
    //----------------------------------------------------------------------------------
    private void SetSkinnedClothGameObject( GameObject bakedGO, GameObject originalGO, GameObject nodeGO, uint idBody, List<GameObject> listAuxGameObjectToDestroy,
                                           out GameObject rootBonesGameObject, out Transform[] arrBoneTransform )
    {
      listAuxGameObjectToDestroy.Clear();
      entityManager_.GetRemoveBodyChildrenList( originalGO, bakedGO, listAuxGameObjectToDestroy );
      foreach( GameObject go in listAuxGameObjectToDestroy )
      {
        UnityEngine.Object.DestroyImmediate(go);
      }
      listAuxGameObjectToDestroy.Clear();

      bakedGO.name = (idBody + 1).ToString() + "_" + originalGO.name;

      CREditorUtils.ReplaceMeshRendererForSkinnedMeshRenderer(bakedGO);

      Mesh originalMesh = originalGO.GetMesh();
      
      bakedGO.transform.parent = nodeGO.transform;

      bakedGO.transform.position   = originalGO.transform.position;
      bakedGO.transform.rotation   = originalGO.transform.rotation;
      bakedGO.transform.localScale = originalGO.transform.lossyScale;
 
      InitSkinnedCloth(idBody, bakedGO, nodeGO, originalMesh, out rootBonesGameObject, out arrBoneTransform);

      bakedGO.SetActive( true );

      Caronte_Fx_Body cfxBody = bakedGO.GetComponent<Caronte_Fx_Body>();
      if (cfxBody != null)
      {
        UnityEngine.Object.DestroyImmediate( cfxBody );
      }
    }
    //----------------------------------------------------------------------------------
    private void InitSkinnedRope( uint idBody, GameObject bakedGO, GameObject nodeGO, Mesh meshToSkin, out GameObject bonesRootGO, out Transform[] arrBoneTransform )
    {
      CaronteSharp.SkelDefinition skelDefinition;
      RopeManager.QuerySkelDefinition(idBody, out skelDefinition);

      Matrix4x4 m_MODEL_to_WORLD     = Matrix4x4.TRS(skelDefinition.root_translation_MODEL_to_WORLD_, skelDefinition.root_rotation_MODEL_to_WORLD_, skelDefinition.root_scale_MODEL_to_WORLD_);
      Matrix4x4 m_MODEL_to_ROOT_BONE = m_MODEL_to_WORLD.inverse * bakedGO.transform.localToWorldMatrix;

      Mesh skinnedMesh;
      CRGeometryUtils.CreateMeshTransformed( meshToSkin, m_MODEL_to_ROOT_BONE, out skinnedMesh );

      listBoneWeightTmp_.Clear();

      int nVertex = skinnedMesh.vertexCount;
      for(int i = 0; i < nVertex; i++)
      {
        CaronteSharp.Tuple4_INDEX indexes = skelDefinition.arrIndexBone_[i];
        CaronteSharp.Tuple4_FLOAT weights = skelDefinition.arrWeightBone_[i];

        BoneWeight bw = new BoneWeight();
        bw.boneIndex0 = (int)indexes.a_;
        bw.weight0    = weights.a_;
        bw.boneIndex1 = (int)indexes.b_;
        bw.weight1    = weights.b_;

        listBoneWeightTmp_.Add( bw );
      }

      skinnedMesh.boneWeights = listBoneWeightTmp_.ToArray();

      bonesRootGO = new GameObject(bakedGO.name + "_bones"); 

      bonesRootGO.transform.localPosition = skelDefinition.root_translation_MODEL_to_WORLD_;
      bonesRootGO.transform.localRotation = skelDefinition.root_rotation_MODEL_to_WORLD_;
      bonesRootGO.transform.localScale    = skelDefinition.root_scale_MODEL_to_WORLD_;

      bonesRootGO.transform.parent = nodeGO.transform; 

      int nBones = skelDefinition.arrTranslation_BONE_to_MODEL_.Length;

      listBindPoseTmp_ .Clear();
      listTransformTmp_.Clear();

      for (int i = 0; i < nBones; i++)
      {
        GameObject bone = new GameObject(bakedGO.name + "_" + i);

        bone.transform.parent = bonesRootGO.transform;

        bone.transform.localPosition = skelDefinition.arrTranslation_BONE_to_MODEL_[i];
        bone.transform.localRotation = skelDefinition.arrRotation_BONE_to_MODEL_[i];
        bone.transform.localScale    = skelDefinition.arrScale_BONE_to_MODEL_[i];

        Matrix4x4 bindpose = bone.transform.worldToLocalMatrix * bonesRootGO.transform.localToWorldMatrix;

        listBindPoseTmp_ .Add(bindpose);
        listTransformTmp_.Add(bone.transform);

      }

      skinnedMesh.bindposes = listBindPoseTmp_.ToArray();
      arrBoneTransform      = listTransformTmp_.ToArray();


      //Set init state
      CaronteSharp.SkelState skelState;
      RopeManager.QuerySkelState(idBody, out skelState);

      bonesRootGO.transform.localPosition = skelState.root_translation_MODEL_to_WORLD_;
      bonesRootGO.transform.localRotation = skelState.root_rotation_MODEL_to_WORLD_;
      bonesRootGO.transform.localScale    = skelState.root_scale_MODEL_to_WORLD_;

      bakedGO.transform.localPosition = skelState.root_translation_MODEL_to_WORLD_;
      bakedGO.transform.localRotation = skelState.root_rotation_MODEL_to_WORLD_;
      bakedGO.transform.localScale    = skelState.root_scale_MODEL_to_WORLD_;

      for (int i = 0; i < nBones; i++)
      {
        GameObject bone = listTransformTmp_[i].gameObject;

        bone.transform.localPosition = skelState.arrTranslation_BONE_to_MODEL_[i];
        bone.transform.localRotation = skelState.arrRotation_BONE_to_MODEL_[i];
        bone.transform.localScale    = skelState.arrScale_BONE_to_MODEL_[i];
      }

      SkinnedMeshRenderer smr = bakedGO.GetComponent<SkinnedMeshRenderer>();
      smr.sharedMesh          = skinnedMesh;
      smr.bones               = arrBoneTransform;
      smr.updateWhenOffscreen = true;

      AssetDatabase.AddObjectToAsset(skinnedMesh, MeshesPrefab);
    }
    //----------------------------------------------------------------------------------
    private void InitSkinnedCloth( uint idBody, GameObject bakedGO, GameObject nodeGO, Mesh meshToSkin, out GameObject bonesRootGO, out Transform[] arrBoneTransform )
    {
      CaronteSharp.SkelDefinition skelDefinition;
      ClothManager.Cl_QuerySkelDefinition(idBody, out skelDefinition);

      Matrix4x4 m_MODEL_to_WORLD     = Matrix4x4.TRS(skelDefinition.root_translation_MODEL_to_WORLD_, skelDefinition.root_rotation_MODEL_to_WORLD_, skelDefinition.root_scale_MODEL_to_WORLD_);
      Matrix4x4 m_MODEL_to_ROOT_BONE = m_MODEL_to_WORLD.inverse * bakedGO.transform.localToWorldMatrix;

      Mesh skinnedMesh;
      CRGeometryUtils.CreateMeshTransformed( meshToSkin, m_MODEL_to_ROOT_BONE, out skinnedMesh );

      listBoneWeightTmp_.Clear();

      int nVertex = skinnedMesh.vertexCount;
      for(int i = 0; i < nVertex; i++)
      {
        CaronteSharp.Tuple4_INDEX indexes = skelDefinition.arrIndexBone_[i];
        CaronteSharp.Tuple4_FLOAT weights = skelDefinition.arrWeightBone_[i];

        BoneWeight bw = new BoneWeight();

        bw.boneIndex0 = (int)indexes.a_;
        bw.weight0    = weights.a_;
  
        bw.boneIndex1 = (int)indexes.b_;
        bw.weight1    = weights.b_;

        bw.boneIndex2 = (int)indexes.c_;
        bw.weight2    = weights.c_;

        bw.boneIndex3 = (int)indexes.d_;
        bw.weight3    = weights.d_;
 
        listBoneWeightTmp_.Add( bw );
      }

      skinnedMesh.boneWeights = listBoneWeightTmp_.ToArray();

      bonesRootGO = new GameObject(bakedGO.name + "_bones"); 

      bonesRootGO.transform.localPosition = skelDefinition.root_translation_MODEL_to_WORLD_;
      bonesRootGO.transform.localRotation = skelDefinition.root_rotation_MODEL_to_WORLD_;
      bonesRootGO.transform.localScale    = skelDefinition.root_scale_MODEL_to_WORLD_;

      bonesRootGO.transform.parent = nodeGO.transform; 

      int nBones = skelDefinition.arrTranslation_BONE_to_MODEL_.Length;

      listBindPoseTmp_ .Clear();
      listTransformTmp_.Clear();

      for (int i = 0; i < nBones; i++)
      {
        GameObject bone = new GameObject(bakedGO.name + "_" + i);

        bone.transform.parent = bonesRootGO.transform;

        bone.transform.localPosition = skelDefinition.arrTranslation_BONE_to_MODEL_[i];
        bone.transform.localRotation = skelDefinition.arrRotation_BONE_to_MODEL_[i];
        bone.transform.localScale    = skelDefinition.arrScale_BONE_to_MODEL_[i];

        Matrix4x4 bindpose = bone.transform.worldToLocalMatrix * bonesRootGO.transform.localToWorldMatrix;

        listBindPoseTmp_ .Add(bindpose);
        listTransformTmp_.Add(bone.transform);

      }

      skinnedMesh.bindposes = listBindPoseTmp_.ToArray();
      arrBoneTransform      = listTransformTmp_.ToArray();

      //Set init state
      CaronteSharp.SkelState skelState;
      ClothManager.Cl_QuerySkelState(idBody, out skelState);

      bonesRootGO.transform.localPosition = skelState.root_translation_MODEL_to_WORLD_;
      bonesRootGO.transform.localRotation = skelState.root_rotation_MODEL_to_WORLD_;
      bonesRootGO.transform.localScale    = skelState.root_scale_MODEL_to_WORLD_;

      bakedGO.transform.localPosition = skelState.root_translation_MODEL_to_WORLD_;
      bakedGO.transform.localRotation = skelState.root_rotation_MODEL_to_WORLD_;
      bakedGO.transform.localScale    = skelState.root_scale_MODEL_to_WORLD_;

      for (int i = 0; i < nBones; i++)
      {
        GameObject bone = listTransformTmp_[i].gameObject;

        bone.transform.localPosition = skelState.arrTranslation_BONE_to_MODEL_[i];
        bone.transform.localRotation = skelState.arrRotation_BONE_to_MODEL_[i];
        bone.transform.localScale    = skelState.arrScale_BONE_to_MODEL_[i];
      }

      SkinnedMeshRenderer smr = bakedGO.GetComponent<SkinnedMeshRenderer>();
      smr.sharedMesh          = skinnedMesh;
      smr.bones               = arrBoneTransform;
      smr.updateWhenOffscreen = true;

      AssetDatabase.AddObjectToAsset(skinnedMesh, MeshesPrefab);
    }
    //----------------------------------------------------------------------------------
    private void SetBakedBoneGameObject( GameObject bakedGO, GameObject originalGO, GameObject nodeGO, uint idBody )
    {
      bakedGO.name = (idBody + 1).ToString() + "_" + originalGO.name;

      bakedGO.transform.parent = originalGO.transform.parent;

      bakedGO.transform.localPosition = originalGO.transform.localPosition;
      bakedGO.transform.localRotation = originalGO.transform.localRotation;
      bakedGO.transform.localScale    = originalGO.transform.localScale;

      bakedGO.transform.parent     = nodeGO.transform;
      bakedGO.transform.localScale = Vector3.one;
    }
    //----------------------------------------------------------------------------------
    private void CheckBodiesVisibility()
    {
      int updateFramesDelta = Mathf.Min( Mathf.Max(frameCount_ / 5, 1), 5);

      for (bakingFrame_ = frameStart_; bakingFrame_ <= frameEnd_; bakingFrame_++ )
      {
        frame_ = bakingFrame_ - frameStart_; 
   
        if ( (frame_ % updateFramesDelta) == 0 )
        {
          float progress = (float) frame_ / (float) frameCount_;
          string progressInfo = "Frame " + frame_ + " of " + frameCount_ + "."; 
          EditorUtility.DisplayProgressBar("CaronteFx - Checking visibility. ", progressInfo, progress );
        }

        CheckVisibility(bakingFrame_);
      }

      CheckVisibility(frameStart_);
    }
    //----------------------------------------------------------------------------------
    private void BakeAnimBinaryFile()
    { 
      int nGameObjects = idBodyToGOKeyframe_.Count;

      MemoryStream ms = new MemoryStream();
      if ( ms != null )
      {
        BinaryWriter bw = new BinaryWriter(ms);
        if (bw != null)
        {
          bw.Write(binaryVersion_);
          bw.Write(vertexCompression_);
          bw.Write(vertexTangents_);
          bw.Write(frameCount_);
          bw.Write(FPS);
          bw.Write(nGameObjects);
          
          EFileHeaderFlags fileheaderFlags = EFileHeaderFlags.NONE;
          if (alignData_)
          {
            fileheaderFlags |= EFileHeaderFlags.ALIGNEDDATA;
          }
          
          if (vertexCompression_)
          {
            if (vertexCompressionIdx_ == 0)
            {
              fileheaderFlags |= EFileHeaderFlags.BOXCOMPRESSION;
            }
            else if (vertexCompressionIdx_ == 1)
            {
              fileheaderFlags |= EFileHeaderFlags.FIBERCOMPRESSION;
              fileheaderFlags |= EFileHeaderFlags.VERTEXLOCALSYSTEMS;
            }
          }

          bool isFiberCompression = fileheaderFlags.FlagCheck(EFileHeaderFlags.FIBERCOMPRESSION);
          bw.Write((uint)fileheaderFlags);

          Dictionary<uint, int> idBodyToIdGameObjectInFile = new Dictionary<uint, int>();
          List<TGOFrameData> listGOFrameData = new List<TGOFrameData>();

          foreach (var element in idBodyToGOKeyframe_)
          {
            uint idBody = element.Key;
            CRGOKeyframe goKeyframe = element.Value;

            GameObject go = idBodyToBakedGO_[idBody];

            idBodyToIdGameObjectInFile.Add(idBody, listGOFrameData.Count);
            listGOFrameData.Add(new Tuple2<Transform, CRGOKeyframe>(go.transform, goKeyframe));
          }

          BakeFrame( frameStart_ );
          int updateFramesDelta = Mathf.Min( Mathf.Max(frameCount_ / 5, 1), 5);
          int frame = 0;
          VerticesAnimationCompressor[] arrVerticesAnimationCompressor = new VerticesAnimationCompressor[nGameObjects];

          if (isFiberCompression)
          {
            for (int i = 0; i < listGOFrameData.Count; i++)
            {
              TGOFrameData goFrameData = listGOFrameData[i];
              Transform tr = goFrameData.First;
              CRGOKeyframe goKeyframe = goFrameData.Second;
              CRFrameWriterUtils.InitVerticesAnimationCompressor(tr, goKeyframe, ref arrVerticesAnimationCompressor[i]);
            }
            
            EditorUtility.DisplayProgressBar("CaronteFX - Bake animation", "Bake first pass. ", 0f);
            for (int bakingFrame_ = frameStart_; bakingFrame_ <= frameEnd_; bakingFrame_++)
            {
              if( (frame % updateFramesDelta) == 0 )
              {        
                EditorUtility.DisplayProgressBar("CaronteFX - Bake animation", "Bake first pass. Frame " + bakingFrame_ + ".", (float)bakingFrame_/(float)frameEnd_ );
              }
  
              BakeFrame( bakingFrame_ );

              for (int j = 0; j < listGOFrameData.Count; j++)
              {
                TGOFrameData goFrameData = listGOFrameData[j];
                CRGOKeyframe goKeyframe = goFrameData.Second;
                VerticesAnimationCompressor vac = arrVerticesAnimationCompressor[j];
                CRFrameWriterUtils.VerticesAnimationCompressorFirstPass(goKeyframe, vac);
              }

              frame++;
            }
          }

          BakeFrame( frameStart_ );

          VertexNormalsFastUpdater vnfu = null;
          if (CRVersionChecker.IsAdvanceCompressionVersion())
          {
            vnfu = new VertexNormalsFastUpdater();
          }

          foreach (var element in idBodyToGOKeyframe_)
          {
            uint idBody = element.Key;
            CRGOKeyframe goKeyframe = element.Value;

            int idx = idBodyToIdGameObjectInFile[idBody];

            string pathRelativeTo       = idBodyToRelativePath_[idBody];
            int vertexCount             = goKeyframe.VertexCount;
            int boneCount               = goKeyframe.BoneCount;
            Vector2 visibleTimeInterval = idBodyToVisibilityInterval_[idBody];

            bw.Write(pathRelativeTo);
            bw.Write(vertexCount);
            bw.Write(boneCount);

            bw.Write(visibleTimeInterval.x);
            bw.Write(visibleTimeInterval.y);

            VerticesAnimationCompressor vac = arrVerticesAnimationCompressor[idx];
            if ( vac != null)
            {       
              byte[] definitionBytes = vac.GetDefinitionAsBytes();
              bw.Write(definitionBytes);
            }

            if (isFiberCompression && vertexCount > 0)
            {
              TGOFrameData goFrameData = listGOFrameData[idx];

              Transform tr = goFrameData.First;
              GameObject go = tr.gameObject;

              Mesh mesh = go.GetMesh();
              if (mesh != null)
              {
                List<Vector3> listPosition = goKeyframe.GetVertexesPosInFrame();
                List<Vector3> listNormal = goKeyframe.GetVertexesNorInFrame();

                vnfu.Calculate(listPosition.ToArray(), listNormal.ToArray(), mesh.triangles);
                byte[] vertexDataBytes = vnfu.VertexDataAsBytes();
                bw.Write(vertexDataBytes);
              }
            }

            if (boneCount > 0)
            {
              string[] bonesRelativePaths = idBodyToBonesRelativePaths_[idBody];
              for (int i = 0; i < bonesRelativePaths.Length; i++)
              {
                bw.Write( bonesRelativePaths[i] );
              }
            }
          }

          if (vnfu != null)
          {
            vnfu.Dispose();
          }

          Dictionary<uint, CNContactEmitter> tableIdToContactEmitter = entityManager_.GetTableIdToContactEmitter();
          bw.Write( tableIdToContactEmitter.Count );

          Dictionary<uint, int> idContactEmitterToIdEmitterInFile = new Dictionary<uint, int>();
          List<CNContactEmitter> listContactEmitter = new List<CNContactEmitter>();

          foreach (var pair in tableIdToContactEmitter)
          {
            uint idContactEmitter   = pair.Key;
            CNContactEmitter ceNode = pair.Value;

            idContactEmitterToIdEmitterInFile.Add(idContactEmitter, listContactEmitter.Count);
            listContactEmitter.Add(ceNode);
            string contactEmitterName = ceNode.Name;

            bw.Write(contactEmitterName);
          }

          long[] fOffsets = new long[frameCount_];
          long fOffsetsP = ms.Position;
          for (int i = 0; i < frameCount_; i++)
          {
            long val = 0;
            bw.Write(val);
          }

          frame = 0;
          for (int bakingFrame_ = frameStart_; bakingFrame_ <= frameEnd_; bakingFrame_++ )
          {
            if( (frame % updateFramesDelta) == 0 )
            {        
              float progress = (float)frame / (float)frameCount_;
              string progressInfo = "Baking to " + rootGameObject_.name + ". " + "Frame " + frame + " of " + frameCount_ + "."; 
              EditorUtility.DisplayProgressBar("CaronteFx - Baking animation Clip (BinaryFile)", progressInfo, progress);
            }

            fOffsets[frame] = ms.Position;
            BakeFrame( bakingFrame_ );
            
            for (int j = 0; j < listGOFrameData.Count; j++)
            {
              CRGOKeyframe goKeyframe = listGOFrameData[j].Second;
              VerticesAnimationCompressor vac = arrVerticesAnimationCompressor[j];
              CRFrameWriterUtils.WriteGOKeyframe(goKeyframe, vac, vertexCompression_, vertexTangents_, fileheaderFlags, ms, bw);
            }

            WriteFrameContactEvents(idBodyToIdGameObjectInFile, idContactEmitterToIdEmitterInFile, bakeEvents_, ms, bw);
            frame++;
          }

          ms.Position = fOffsetsP;
          for (int i = 0; i < frameCount_; i++)
          {
            bw.Write(fOffsets[i]);
          }

          EditorUtility.ClearProgressBar();

          CreateAssetAndAnimationComponent(ms);

          bw.Close();
          ms.Close();
        }
      }   
    }
    //----------------------------------------------------------------------------------
    public void WriteFrameContactEvents(Dictionary<uint, int> idBodyToIdGameObjectInFile, Dictionary<uint, int> idContactEmitterToIdEmitterInFile, bool bakeEvents, MemoryStream ms, BinaryWriter bw)
    {
      int nFrameEvents = 0;
      long nFrameEventsPos = ms.Position;
      bw.Write(nFrameEvents);

      if (bakeEvents)
      {
        List<CaronteSharp.ContactEventInfo> listFrameEventInfo = SimulationManager.listContactEventInfo_;

        foreach (CaronteSharp.ContactEventInfo evInfo in listFrameEventInfo)
        {
          uint idBodyA = evInfo.a_bodyId_;
          uint idBodyB = evInfo.b_bodyId_;

          if (idBodyToIdGameObjectInFile.ContainsKey(idBodyA) &&
              idBodyToIdGameObjectInFile.ContainsKey(idBodyB))
          {
            uint idEntity = evInfo.idEntity_;
            int idEmitterInFile = idContactEmitterToIdEmitterInFile[idEntity];
            bw.Write(idEmitterInFile);

            bw.Write( (int)CRAnimationEvData.EEventDataType.Contact );

            int idGameObjectInFileA = idBodyToIdGameObjectInFile[idBodyA];
            int idGameObjectInFileB = idBodyToIdGameObjectInFile[idBodyB];

            bw.Write(idGameObjectInFileA);
            bw.Write(idGameObjectInFileB);
          }
          else
          {
            continue;
          }
          bw.Write(evInfo.position_.x);
          bw.Write(evInfo.position_.y);
          bw.Write(evInfo.position_.z);

          bw.Write(evInfo.a_v_.x);
          bw.Write(evInfo.a_v_.y);
          bw.Write(evInfo.a_v_.z);

          bw.Write(evInfo.b_v_.x);
          bw.Write(evInfo.b_v_.y);
          bw.Write(evInfo.b_v_.z);

          bw.Write(evInfo.relativeSpeed_N_);
          bw.Write(evInfo.relativeSpeed_T_);

          bw.Write(evInfo.relativeP_N_);
          bw.Write(evInfo.relativeP_T_);

          nFrameEvents++;
        }
      }

      long currentPos = ms.Position;
      ms.Position = nFrameEventsPos;
      bw.Write(nFrameEvents);
      ms.Position = currentPos;
    }
    //----------------------------------------------------------------------------------
    private void CreateAssetAndAnimationComponent( MemoryStream ms )
    {
      AssetDatabase.Refresh(); 

      CRAnimation crAnimation = rootGameObject_.GetComponent<CRAnimation>();
      if ( crAnimation == null )
      {
        crAnimation = rootGameObject_.AddComponent<CRAnimation>();
      }

      if (animationFileType_ == AnimationFileType.CRAnimationAsset)
      {
        CRAnimationAsset animationAsset = CRAnimationAsset.CreateInstance<CRAnimationAsset>();
        animationAsset.Bytes = ms.ToArray();

        string assetFilePath = AssetDatabase.GenerateUniqueAssetPath( assetsPath_ + "/" + animationName_ + ".asset");
        AssetDatabase.CreateAsset( animationAsset, assetFilePath );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(); 

        crAnimation.AddAnimationAndSetActive(animationAsset);
      }
      else if (animationFileType_ == AnimationFileType.TextAsset)
      {
        string assetFilePath = AssetDatabase.GenerateUniqueAssetPath( assetsPath_ + "/" + animationName_ + ".bytes");

        FileStream fs = new FileStream(assetFilePath, FileMode.Create);
        byte[] arrByte = ms.ToArray();
        fs.Write(arrByte, 0, arrByte.Length);
        fs.Close();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(); 

        TextAsset crAnimationText = (TextAsset)AssetDatabase.LoadAssetAtPath(assetFilePath, typeof(TextAsset));
        crAnimation.AddAnimationAndSetActive(crAnimationText);
      }

      EditorUtility.SetDirty(crAnimation);
    }
    //----------------------------------------------------------------------------------
    public void InitKeyFrame(BD_TYPE type, BodyInfo bodyInfo)
    {
      uint idBody = bodyInfo.idBody_;

      if( idBodyToBakedGO_.ContainsKey(idBody) )
      {     
        GameObject go = idBodyToBakedGO_[idBody];
        
        int nVertexCount = 0;
        int nBonesCount  = 0;

        if ( type == BD_TYPE.BODYMESH_ANIMATED_BY_VERTEX ||
             type == BD_TYPE.SOFTBODY || 
             type == BD_TYPE.CLOTH )
        {
          Mesh mesh = go.GetMesh();

          if (mesh != null)
          {
            nVertexCount = mesh.vertexCount;
          }
          else
          {
            nBonesCount = idBodyToBonesRelativePaths_[idBody].Length;
          }
        }

        Vector2 visibleTimeInterval = entityManager_.GetVisibleTimeInterval( idBody );

        float newStart = Mathf.Clamp(visibleTimeInterval.x - visibilityShift_, visibilityRangeMin_, visibilityRangeMax_);
        float newEnd   = Mathf.Clamp(visibleTimeInterval.y - visibilityShift_, visibilityRangeMin_, visibilityRangeMax_);

        if (bakeVisibility_)
        {
          visibleTimeInterval.x = newStart;
          visibleTimeInterval.y = newEnd;
        }
        else
        {
          visibleTimeInterval.x = 0f;
          visibleTimeInterval.y = float.MaxValue;
        }

        idBodyToVisibilityInterval_.Add( idBody, visibleTimeInterval );
        idBodyToGOKeyframe_        .Add( idBody, new CRGOKeyframe(nVertexCount, nBonesCount) );
      }
    }
    //----------------------------------------------------------------------------------
    public void BakeBodyKeyFrame( BD_TYPE type, BodyInfo bodyInfo )
    {
      uint idBody = bodyInfo.idBody_;
      if ( idBodyToGOKeyframe_.ContainsKey(idBody) )
      {
        CRGOKeyframe goKeyframe = idBodyToGOKeyframe_[idBody];

        bool hasFrameData = (bodyInfo.broadcastFlag_ & CaronteSharp.BROADCASTFLAG.GHOST) == CaronteSharp.BROADCASTFLAG.GHOST ||
                            (bodyInfo.broadcastFlag_ & CaronteSharp.BROADCASTFLAG.VISIBLE) == CaronteSharp.BROADCASTFLAG.VISIBLE;

        switch (type)
        {
          case BD_TYPE.RIGIDBODY:
          case BD_TYPE.BODYMESH_ANIMATED_BY_MATRIX:
            {
              RigidBodyInfo rbInfo = bodyInfo as RigidBodyInfo;
              goKeyframe.SetBodyKeyframe( hasFrameData, rbInfo.position_, rbInfo.orientation_ );
              break;
            }

          case BD_TYPE.BODYMESH_ANIMATED_BY_VERTEX:
            {
              BodyMeshInfo bodymeshInfo = bodyInfo as BodyMeshInfo;
              goKeyframe.SetBodyKeyframe( hasFrameData, bodymeshInfo.position_, bodymeshInfo.orientation_ );
              
              if (hasFrameData)
              {
                Vector3[] arrNormal;
                Vector4[] arrTangent;
                CalculateMeshData( bodymeshInfo.idBody_, bodymeshInfo.arrVertices_, out arrNormal, out arrTangent );
                goKeyframe.SetVertexKeyframe( bodymeshInfo.arrVertices_, arrNormal, arrTangent );
              }
              break;
            }

          case BD_TYPE.SOFTBODY:
            {
              SoftBodyInfo sbInfo = (SoftBodyInfo)bodyInfo;

              if ( setBoneBodies_.Contains(idBody) )
              {
                SkelState skelState;
                RopeManager.QuerySkelState(idBody, out skelState);

                goKeyframe.SetBodyKeyframe( hasFrameData, skelState.root_translation_MODEL_to_WORLD_, skelState.root_rotation_MODEL_to_WORLD_ );
                if (hasFrameData)
                {
                  goKeyframe.SetBonesKeyframe( skelState.arrTranslation_BONE_to_MODEL_, skelState.arrRotation_BONE_to_MODEL_, skelState.arrScale_BONE_to_MODEL_ );
                }
              }
              else
              { 
                goKeyframe.SetBodyKeyframe( hasFrameData, sbInfo.center_, Quaternion.identity );
                if (hasFrameData)
                {
                  Vector3[] arrNormal;
                  Vector4[] arrTangent;

                  CalculateMeshData( sbInfo.idBody_, sbInfo.arrVerticesRender_, out arrNormal, out arrTangent );    
                  goKeyframe.SetVertexKeyframe( sbInfo.arrVerticesRender_, arrNormal, arrTangent );
                }    
              }

              break;
            }

          case BD_TYPE.CLOTH:
            {
              SoftBodyInfo sbInfo = (SoftBodyInfo)bodyInfo;

              if ( setBoneBodies_.Contains(idBody) )
              {
                SkelState skelState;
                ClothManager.Cl_QuerySkelState(idBody, out skelState);

                goKeyframe.SetBodyKeyframe( hasFrameData, skelState.root_translation_MODEL_to_WORLD_, skelState.root_rotation_MODEL_to_WORLD_ );
                if (hasFrameData)
                {
                  goKeyframe.SetBonesKeyframe( skelState.arrTranslation_BONE_to_MODEL_, skelState.arrRotation_BONE_to_MODEL_, skelState.arrScale_BONE_to_MODEL_ );
                }
              }
              else
              { 
                goKeyframe.SetBodyKeyframe( hasFrameData, sbInfo.center_, Quaternion.identity );
                if (hasFrameData)
                {
                  Vector3[] arrNormal;
                  Vector4[] arrTangent;
                  CalculateMeshData( sbInfo.idBody_, sbInfo.arrVerticesRender_, out arrNormal, out arrTangent );    
                  goKeyframe.SetVertexKeyframe( sbInfo.arrVerticesRender_, arrNormal, arrTangent );
                }    
              }
              break;
            }
        }
      }
    }
    //----------------------------------------------------------------------------------
    private void CheckBodyVisibility(BD_TYPE type, BodyInfo bodyInfo)
    {
      uint idBody = bodyInfo.idBody_;
      bool isVisible = ( bodyInfo.broadcastFlag_ & BROADCASTFLAG.VISIBLE ) == BROADCASTFLAG.VISIBLE;
      bool isGhost   = ( bodyInfo.broadcastFlag_ & BROADCASTFLAG.GHOST ) == BROADCASTFLAG.GHOST;

      if (isVisible || isGhost)
      {
        setVisibleBodies_.Add(idBody);
      }
    }
    //----------------------------------------------------------------------------------
    private void SetStartData()
    {
      float startTime = frameStart_ * FrameTime;

      foreach( var pair in idBodyToVisibilityInterval_ )
      {
        uint idBody = pair.Key;
        Vector2 visibilityInterval = pair.Value;

        GameObject go = idBodyToBakedGO_[idBody];

        if (bakeVisibility_)
        {
          bool isVisible = startTime >= visibilityInterval.x && startTime <= visibilityInterval.y;

          if ( setBoneBodies_.Contains(idBody) )
          {     
            if (!isVisible)
            {
              go.transform.localScale = Vector3.zero;
            }
          }
          else
          {
            go.SetActive( isVisible );
          }
        }
        else
        {
          if ( setBoneBodies_.Contains(idBody) )
          {     
            go.transform.localScale = Vector3.one;
          }
          else
          {
            go.SetActive( true );
          }
        }
      }
    }
    //----------------------------------------------------------------------------------
    private void CalculateMeshData( uint idBody, Vector3[] arrVertices, out Vector3[] arrNormal, out Vector4[] arrTangent)
    {
      Tuple2<UnityEngine.Mesh, MeshUpdater> meshData = entityManager_.GetBodyMeshRenderUpdaterRef(idBody);

      UnityEngine.Mesh mesh   = meshData.First;
      MeshUpdater meshUpdater = meshData.Second;

      mesh.vertices = arrVertices;
      meshComplexForUpdate_.Set( mesh, null );

      CaronteSharp.Tools.UpdateVertexNormalsAndTangents( meshUpdater, meshComplexForUpdate_ );
      
      mesh.normals  = meshComplexForUpdate_.arrNormal_;
      mesh.tangents = meshComplexForUpdate_.arrTan_;

      arrNormal   = meshComplexForUpdate_.arrNormal_;
      arrTangent  = meshComplexForUpdate_.arrTan_;
    }
  }
}