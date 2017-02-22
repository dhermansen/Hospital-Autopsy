using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CaronteSharp;

namespace CaronteFX
{
  public class CNFractureEditor : CNMonoFieldEditor
  {
    private static readonly GUIContent frGlobalPatternCt_     = new GUIContent(CRStringManager.GetString("FrGlobalPattern"), CRStringManager.GetString("FrGlobalPatternTooltip"));
    private static readonly GUIContent frNDesiredPiecesCt_    = new GUIContent(CRStringManager.GetString("FrRoughNumberOfPieces"), CRStringManager.GetString("FrRoughNumberOfPiecesTooltip"));
    private static readonly GUIContent frSeedCt_              = new GUIContent(CRStringManager.GetString("FrSeed"), CRStringManager.GetString("FrSeedTooltip"));
    private static readonly GUIContent frInteriorMaterialCt_  = new GUIContent(CRStringManager.GetString("FrInteriorFacesMaterial"), CRStringManager.GetString("FrInteriorFacesMaterialTooltip"));
    private static readonly GUIContent frInteriorFacesSmoothingAngleCt_  = new GUIContent(CRStringManager.GetString("FrInteriorFacesSmoothingAngle"), CRStringManager.GetString("FrInteriorFacesSmoothingAngleTooltip"));


    private static readonly GUIContent frGeometryCt_          = new GUIContent("Steering geometry", "Add description");
    private static readonly GUIContent frGridResolutionCt_    = new GUIContent("Steering resolution", "Add description");
    private static readonly GUIContent frFocusModeCt_         = new GUIContent("Highest concentration", "Add description");  
    private static readonly GUIContent frDensityRateCt_       = new GUIContent("Lowest/highest concentration rate", "Add description");
    private static readonly GUIContent frTransitionLengthCt_  = new GUIContent("Transition length", "Add description");

    private static readonly GUIContent frReferenceSystemCt_     = new GUIContent("Reference system", "Add description");
    private static readonly GUIContent frReferenceSystemAxisCt_ = new GUIContent("Front direction", "Add description");
    private static readonly GUIContent frRaysNumberCt_          = new GUIContent("Rays number", "Add description");
    private static readonly GUIContent frRaysRateRandCt_        = new GUIContent("Rays angle randomness", "Add description");

    private static readonly GUIContent frRingsIntRadiusCt_           = new GUIContent("Annulus internal radius", "Add description");
    private static readonly GUIContent frRingsIntTransitionLengthCt_ = new GUIContent("Internal transition length", "Add description");
    private static readonly GUIContent frRingsIntTransitionDecayCt_  = new GUIContent("Internal transition decay", "Add description");

    private static readonly GUIContent frRingsExtRadiusCt_           = new GUIContent("Annulus external radius", "Add description");
    private static readonly GUIContent frRingsExtTransitionLengthCt_ = new GUIContent("External transition length", "");
    private static readonly GUIContent frRingsExtTransitionDecayCt_  = new GUIContent("External transition decay", "");

    private static readonly GUIContent frRingsNumberInsideAnnulusCt_ = new GUIContent("Rings number inside annulus", "");
    private static readonly GUIContent frRingsRateRandCt_            = new GUIContent("Rings radius randomness", "");
    private static readonly GUIContent frCreateCentralPieceCt_       = new GUIContent("Create central piece","");

    private static readonly GUIContent frNoiseRateCt_ = new GUIContent("Noise", "noiseRateContent");
    private static readonly GUIContent frTwistRateCt_ = new GUIContent("Twist", "Add description");

    private static readonly GUIContent frDoExtrusionEffectCt_ = new GUIContent(CRStringManager.GetString("FrExtrusionEffect"), CRStringManager.GetString("FrExtrusionEffectTooltip"));
    private static readonly GUIContent frDoCoordinateCt_      = new GUIContent(CRStringManager.GetString("FrCoordinate"), CRStringManager.GetString("FrCoordinateTooltip"));

    private static readonly GUIContent frHideParentObjectsAutoCt_ = new GUIContent(CRStringManager.GetString("FrAutoHideOriginalObjects"), CRStringManager.GetString("FrAutoHideOriginalObjectsTooltip"));

    private static readonly GUIContent frRectrictFractureZoneCt_ = new GUIContent(CRStringManager.GetString("FrRestrictFractureZone"), CRStringManager.GetString("FrRestrictFractureZoneTooltip"));
    private static readonly GUIContent frResctrictGeometryCt_    = new GUIContent(CRStringManager.GetString("FrRestrictionGeometry"),  CRStringManager.GetString("FrRestrictionGeometryTooltip"));
    private static readonly GUIContent frFractureZoneCt_         = new GUIContent(CRStringManager.GetString("FrFractureZone"),         CRStringManager.GetString("FrFractureZoneTooltip"));
    private static readonly GUIContent frUnfracturedMeshCt_      = new GUIContent(CRStringManager.GetString("FrUnfracturedMesh"),      CRStringManager.GetString("FrUnfracturedMeshTooltip"));

    private static readonly GUIContent frInteriorFacesNoiseCt_           = new GUIContent(CRStringManager.GetString("FrInteriorFacesNoise"),          CRStringManager.GetString("FrInteriorFacesNoiseTooltip"));
    private static readonly GUIContent frNoiseResolutionRateCt_          = new GUIContent(CRStringManager.GetString("FrNoiseResolutionRate"),         CRStringManager.GetString("FrNoiseResolutionRateTooltip"));
    private static readonly GUIContent frNoiseReliefAmplitudeRateCt_     = new GUIContent(CRStringManager.GetString("FrNoiseReliefAmplitureRate"),    CRStringManager.GetString("FrNoiseReliefAmplitureRateTooltip"));
    private static readonly GUIContent frNoiseReliefSerrationRateCt_     = new GUIContent(CRStringManager.GetString("FrNoiseReliefSerrationRate"),    CRStringManager.GetString("FrNoiseReliefSerrationRateTooltip"));
    private static readonly GUIContent frNoiseSeedCt_                    = new GUIContent(CRStringManager.GetString("FrNoiseSeed"),                    CRStringManager.GetString("FrNoiseSeedTooltip") );

    private static readonly GUIContent frOutputSectionCt_  = new GUIContent(CRStringManager.GetString("FrOutputSection"));
    private static readonly GUIContent frSelectPiecesCt_   = new GUIContent(CRStringManager.GetString("FrSelectPieces"), CRStringManager.GetString("FrSelectPiecesTooltip"));
    private static readonly GUIContent frPushRateCt_       = new GUIContent(CRStringManager.GetString("FrPushRate"), CRStringManager.GetString("FrPushRateTooltip"));
    private static readonly GUIContent frPushMultiplierCt_ = new GUIContent(CRStringManager.GetString("FrPushMultiplier"), CRStringManager.GetString("FrPushMultiplierTooltip"));

    private static readonly GUIContent[] arrTabName_              = new GUIContent[] { new GUIContent(CRStringManager.GetString("FrTabParameters")),          new GUIContent(CRStringManager.GetString("FrTabStats")) };
    private static readonly GUIContent[] arrRestrictionMode_      = new GUIContent[] { new GUIContent(CRStringManager.GetString("FrStrictlyInside")),         new GUIContent(CRStringManager.GetString("FrInsidePlusBoundary") ), new GUIContent( CRStringManager.GetString("FrStrictlyOutside") ), new GUIContent(CRStringManager.GetString("FrOutsidePlusBoundary") ) };
    private static readonly GUIContent[] arrUnfracturedZoneNames_ = new GUIContent[] { new GUIContent(CRStringManager.GetString("FrUniteDisconnectedParts")), new GUIContent(CRStringManager.GetString("FrSeparateDisconnectedParts"))  };

    public static Texture icon_uniform_;
    public static Texture icon_geometry_;
    public static Texture icon_radial_;

    public override Texture TexIcon 
    { 
      get
      { 
        switch (Data.ChopMode)
        {
        case CNFracture.CHOP_MODE.VORONOI_UNIFORM:
          return icon_uniform_;

        case CNFracture.CHOP_MODE.VORONOI_BY_GEOMETRY:
          return icon_geometry_;

        case CNFracture.CHOP_MODE.VORONOI_RADIAL:
          return icon_radial_;
        
        default:
          return null;
        }
      } 
    }
   
    int   tabIndex_;
    int   cropModeIdx_;
    float pushRate_;
    float pushMultiplier_;

    new CNFracture Data { get; set; }

    public CNFractureEditor( CNFracture data, CommandNodeEditorState state )
      : base(data, state)
    {
      Data = (CNFracture)data;
    }

    public void CheckUpdate(out int[] unityIdsAdded, out int[] unityIdsRemoved)
    {
      unityIdsAdded   = null;
      unityIdsRemoved = null;
      Data.NeedsUpdate = false;                
    }

    protected override void LoadState()
    {
      base.LoadState();

      pushRate_       = Data.PushRate;
      pushMultiplier_ = Data.PushMultiplier;
    }

    public override void ValidateState()
    {
      base.ValidateState();

      if ( (pushRate_ != Data.PushRate || pushMultiplier_ != Data.PushMultiplier) )
      {
        SeparatePieces();

        pushRate_       = Data.PushRate;
        pushMultiplier_ = Data.PushMultiplier;
      }  
    }


    public void Chop()
    {
      GameObject[] goToChop = FieldController.GetUnityGameObjects();
      int numObjects = goToChop.Length;

      string errorMessage = string.Empty;

      if ( numObjects == 0 )
      {
        errorMessage = "Objects field must contain at least one object with geometry";
      }

      if (Data.RestrictFractureZone)
      {
        if ( Data.CropGeometry == null || (Data.CropGeometry != null && !Data.CropGeometry.HasMesh() ) )
        {
          errorMessage = "Crop geometry GameObject must contain a mesh";
        }
      }

      if (Data.ChopMode == CNFracture.CHOP_MODE.VORONOI_BY_GEOMETRY)
      {
        if (Data.ChopGeometry == null )
        {
          errorMessage = "Specifying a steering geometry is mandatory";
        }
        else if ( !Data.ChopGeometry.HasMesh() )
        {
          errorMessage = "Steering geometry GameObject must contain a mesh";
        }
      }

      if (Data.ChopMode == CNFracture.CHOP_MODE.VORONOI_RADIAL)
      {
        if (Data.ReferenceSystem == null)
        {
          errorMessage = "Specifying the reference system is mandatory";
        } 
      }

      if ( errorMessage != string.Empty )
      {
        EditorUtility.DisplayDialog("CaronteFX", errorMessage, "Ok");
        return;
      }

      Undo.RecordObject(Data, "Chop - " + Data.Name);

      List<GameObject>  listParentGO           = new List<GameObject>();
      List<MeshComplex> listParentMesh_car     = new List<MeshComplex>();
      List<Matrix4x4>   listMatrixModelToWorld = new List<Matrix4x4>();
      List<Material>    listMaterial           = new List<Material>();
      Dictionary<Material, int> dictMaterialMaterialIdx = new Dictionary<Material,int>();

      for (int i = 0; i < numObjects; i++)
      {
        GameObject go = goToChop[i];

        Mesh un_mesh;
        bool wasBaked = go.GetMeshOrBakedMesh( out un_mesh );

        if (un_mesh != null)
        {
          Renderer rn = go.GetComponent<Renderer>();
          Material[] arrMaterial = rn.sharedMaterials;

          List<int> listMeshMaterialIdx = new List<int>();
         
          for (int m = 0; m < arrMaterial.Length; m++)
          {
            Material material = arrMaterial[m];

            if (material == null)
            {
              listMeshMaterialIdx.Add(-2);
              continue;
            }

            if (!dictMaterialMaterialIdx.ContainsKey(material))
            {
              dictMaterialMaterialIdx[material] = listMaterial.Count;  
              listMaterial.Add(material);
            }

            int materialIdx = dictMaterialMaterialIdx[material];
            listMeshMaterialIdx.Add(materialIdx);
          }


          MeshComplex mc = new MeshComplex();
          mc.Set( un_mesh, listMeshMaterialIdx.ToArray() );

          if (wasBaked)
          {
            Object.DestroyImmediate(un_mesh);
          }

          listParentMesh_car    .Add( mc );
          listParentGO          .Add( go );
          listMatrixModelToWorld.Add( go.transform.localToWorldMatrix );
        }
      }

      Bounds globalBounds = CREditorUtils.GetGlobalBoundsWorld( listParentGO );

      ChopRequest cr = new ChopRequest();

      cr.doKeepUVCoords_          = true;
      cr.doKeepVertexNormals_     = true;
      cr.arrMeshToChop_           = listParentMesh_car.ToArray();
      cr.arrMatrixModelToWorld_   = listMatrixModelToWorld.ToArray();
      
      bool isOnePieceInputOnly = listParentMesh_car.Count == 1;

      cr.doGlobalPattern_                           = Data.DoGlobalPattern && !isOnePieceInputOnly;
      cr.seed_                                      = (uint)Data.Seed;
      cr.smoothingAngle_internalFacesVertexNormals_ = Data.InternalFacesSmoothingAngle * Mathf.Deg2Rad;

      bool chopModeUniform  = false;
      bool chopModeGeometry = false;
      bool chopModeRadial   = false;

      cr.pProgressFunction_ = null;

      switch (Data.ChopMode)
      {
        case CNFracture.CHOP_MODE.VORONOI_UNIFORM:
          cr.chopMode_ = CaronteSharp.CP_CHOP_MODE.CP_CHOP_MODE_VORONOI_UNIFORM;
          chopModeUniform = ChopModeUniform( cr );
          break;

        case CNFracture.CHOP_MODE.VORONOI_BY_GEOMETRY:
          cr.chopMode_ = CaronteSharp.CP_CHOP_MODE.CP_CHOP_MODE_VORONOI_BY_STEERING_GEOMETRY;
          chopModeGeometry = ChopModeGeometry( cr );
          break;

        case CNFracture.CHOP_MODE.VORONOI_RADIAL:
          cr.chopMode_ = CaronteSharp.CP_CHOP_MODE.CP_CHOP_MODE_VORONOI_RADIAL;
          chopModeRadial = ChopModeRadial( cr );
          break;

      }

      if ( !chopModeUniform && !chopModeGeometry && !chopModeRadial )
      {
        return;
      }


      CaronteSharp.MeshComplex[] arrMeshPieceCaronte;
      uint[]      arrMeshPieceParentIdx;
      Vector3[]   arrMeshPiecePosition;
      ArrayIndex  arrInsideOutsideIdx;

      ChopRestrictionRequest wr = GetRestrictRequest();

      EditorUtility.DisplayProgressBar( Data.Name, "Chopping...", 1.0f);
      if ( CRVersionChecker.IsPremiumVersion() )
      {
        ChopNoiseRequest cnr = GetNoiseRequest();
        CaronteSharp.Tools.FractureMeshesV3Premium(cr, wr, cnr, out arrMeshPieceCaronte, out arrMeshPieceParentIdx, out arrMeshPiecePosition, out arrInsideOutsideIdx);
      }
      else
      {
        CaronteSharp.Tools.FractureMeshesV3(cr, wr, out arrMeshPieceCaronte, out arrMeshPieceParentIdx, out arrMeshPiecePosition, out arrInsideOutsideIdx);
      }
      
      bool thereIsOutput = arrMeshPieceCaronte.Length > 0;
      if (CRVersionChecker.IsFreeVersion() && !thereIsOutput )
      {
        EditorUtility.DisplayDialog("CaronteFX - Free version", "CaronteFX Free version can only fracture the meshes included in the example scenes and the unity primitives (cube, plane, sphere, etc.)", "Ok");
      }

      if (thereIsOutput)
      {
        List<GameObject> listChoppedParentGO;
        CreateListChoppedParentGO( listParentGO, out listChoppedParentGO );

        EditorUtility.DisplayProgressBar( Data.Name, "Creating pieces...", 1.0f);

        List< List<int> >        listListMaterialIdx;
        List< UnityEngine.Mesh > listMeshPieceUnity;
        List< int >              listInputMeshIdx;
        
        CRGeometryUtils.CreateMeshesFromCaronte( arrMeshPieceCaronte, out listMeshPieceUnity, out listInputMeshIdx, out listListMaterialIdx );

        Transform oldParent = DestroyOldObjects();

        Undo.RecordObject(Data, "Chop - " + Data.Name);
        CreateNewObjects( globalBounds.center, listParentGO, listChoppedParentGO, listMaterial, listMeshPieceUnity,
                          listListMaterialIdx, listInputMeshIdx, arrMeshPieceParentIdx, arrMeshPiecePosition, arrInsideOutsideIdx.arrIdx_ );

        if (oldParent != null)
        {
          Data.GameObjectChoppedRoot.transform.parent = oldParent;
        }

        SeparatePieces();
        ApplyUVsPostProcess();
        CalculateStatistics();

        EditorUtility.ClearProgressBar();

        Undo.SetCurrentGroupName("Chop - " + Data.Name);
        Undo.CollapseUndoOperations( Undo.GetCurrentGroup() );
        EditorUtility.SetDirty(Data);
      }
    }

    private void CalculateStatistics()
    {
      GameObject[] goToChop = FieldController.GetUnityGameObjects();

      int nInputObjects = 0;
      int inputVertices = 0;
      int inputIndices = 0;

      int nOutputPieces = 0;
      int outputVertices = 0;
      int outputIndices = 0;

      for (int i = 0; i < goToChop.Length; i++)
      {
        GameObject go = goToChop[i];
        Mesh mesh = go.GetMesh();

        if ( mesh != null )
        {
          nInputObjects++;
          inputVertices += mesh.vertexCount;
          inputIndices+= mesh.triangles.Length;
        }
      }

      Data.InputObjects    = nInputObjects;
      Data.InputVertices   = inputVertices;
      Data.InputTriangles  = inputIndices / 3;

      GameObject[] arrGOChopped = Data.ArrChoppedGameObject;
      for (int i = 0; i < arrGOChopped.Length; i++)
      {
        GameObject go = arrGOChopped[i];
        Mesh mesh = go.GetMesh();

        if ( mesh != null )
        {
          nOutputPieces++;
          outputVertices += mesh.vertexCount;
          outputIndices+= mesh.triangles.Length;
        }
      }

      Data.OutputPieces    = nOutputPieces;
      Data.OutputVertices  = outputVertices;
      Data.OutputTriangles = outputIndices / 3;
    }

    private void CreateListChoppedParentGO( List<GameObject> listParentGO, out List<GameObject> listChoppedParentGO )
    {
      listChoppedParentGO = new List<GameObject>();

      // Create ParentGO Pieces
      for ( int i = 0; i < listParentGO.Count; i++)
      {
        GameObject parentGO       = listParentGO[i];
        GameObject chopParentGO   = parentGO.CreateDummy(parentGO.name + "_chopped");
        listChoppedParentGO.Add(chopParentGO);
      }

    }

    private Transform DestroyOldObjects()
    {
      if ( Data.GameObjectChoppedRoot == null )
      {
        return null;
      }

      Transform oldParent = Data.GameObjectChoppedRoot.transform.parent;

      Undo.DestroyObjectImmediate(Data.GameObjectChoppedRoot);

      return oldParent;
    }

    private ChopRestrictionRequest GetRestrictRequest()
    {
      ChopRestrictionRequest wr = new ChopRestrictionRequest();
      wr.doRestrictionZone_ = Data.RestrictFractureZone;

      GameObject go = Data.CropGeometry;
      if (go != null)
      {
        Mesh mesh = go.GetMesh();

        if (mesh != null)
        {
          UnityEngine.Mesh transformedMesh;

          Matrix4x4 m_Local_to_World  = go.transform.localToWorldMatrix;
          CRGeometryUtils.CreateMeshTransformed(mesh, m_Local_to_World, out transformedMesh);

          MeshSimple cropGeometry = new MeshSimple();
          cropGeometry.Set( transformedMesh );
          Object.DestroyImmediate( transformedMesh );

          wr.meshCropGeometry_ = cropGeometry;
        }      
      }

      wr.cropMode_                     = (CROP_MODE)Data.CropMode;
      wr.weldAllRemainingsTogether_    = Data.WeldInOnePiece;
      wr.includeBoundary_              = Data.FrontierPieces;
      wr.isAbleToClassifyDisconnected_ = true;

      return wr;
    }

    private ChopNoiseRequest GetNoiseRequest()
    {
      ChopNoiseRequest cnr = new ChopNoiseRequest();

      cnr.doNoise_                  = Data.DoNoise;
      cnr.noiseResolutionRate_      = Data.NoiseResolutionRate;
      cnr.noiseReliefAmplitudeRate_ = Data.NoiseReliefAmplitudeRate;
      cnr.noiseReliefSerrationRate_ = Data.NoiseReliefSerrationRate;
      cnr.seed0_                    = Data.NoiseSeed0;
      cnr.doExtrusionEffect_        = Data.NoiseDoExtrusionEffect;

      return cnr;
    }

    public void ChangeInteriorMaterial(Material interiorMaterial)
    {
      if (Data.ArrChoppedGameObject != null && Data.ArrInteriorSubmeshIdx != null)
      {
        int nGameObjects = Data.ArrChoppedGameObject.Length;

        for (int i = 0; i < nGameObjects; i++)
        {
          GameObject go = Data.ArrChoppedGameObject[i];
          if ( go != null )
          {
            Renderer   renderer  = go.GetComponent<Renderer>();
            Material[] materials = renderer.sharedMaterials;

            int subMeshIdx = Data.ArrInteriorSubmeshIdx[i];
            if (subMeshIdx != -1)
            {
              materials[subMeshIdx] = interiorMaterial;  
              renderer.sharedMaterials = materials;
            }
          }
        }
      }
    }

    private void CreateNewObjects( Vector3 centerPosition, List<GameObject> listParentGO, List<GameObject> listChoppedParentGO, List<Material> listMaterial, List<UnityEngine.Mesh> listMeshPieceUnity,
                                   List< List< int > > listListMaterialIdx, List<int> listInputMeshIdx, uint[] arrMeshPieceParentIdx, Vector3[] arrMeshPiecePosition, uint[] arrOutsideInsideIdx )
    {  
      List< GameObject > listChoppedGO = new List<GameObject>();
      List< Tuple2<GameObject, GameObject> > listOutsideInsideGOParent = new List< Tuple2<GameObject, GameObject> >();

      bool hasBeenSplitted = arrOutsideInsideIdx != null && arrOutsideInsideIdx.Length > 0;

      // Create InsideOutsidePieces
      for ( int i = 0; i < listParentGO.Count; i++)
      {
        GameObject parentGO     = listParentGO[i];
        GameObject chopParentGO = listChoppedParentGO[i];

        if ( hasBeenSplitted )
        {
          GameObject outsideGO = parentGO.CreateDummy(parentGO.name + "_outside");
          GameObject insideGO  = parentGO.CreateDummy(parentGO.name + "_inside");

          outsideGO.transform.parent = chopParentGO.transform;
          insideGO.transform.parent  = chopParentGO.transform;

          listOutsideInsideGOParent.Add( Tuple2.New( outsideGO, insideGO ) );
        }
        
        if ( Data.HideParentObjectAuto )
        {
          Undo.RecordObject( parentGO, "Change activate state - " + parentGO.name );
          parentGO.SetActive(false);
          EditorUtility.SetDirty( parentGO );
        }
      }

      GameObject chopRoot = new GameObject( Data.Name + "_output" );
      chopRoot.transform.position = centerPosition;
      Undo.RegisterCreatedObjectUndo(chopRoot, "Created " + Data.Name + "_output");

      Data.GameObjectChoppedRoot = chopRoot;
      
      Dictionary<GameObject, int> dictChoppedGOInteriorSubmeshIdx = new Dictionary<GameObject,int>();
      List<Material> listMaterialPiece = new List<Material>();

      int nMeshPieces = listMeshPieceUnity.Count;
      for (int i = 0; i < nMeshPieces; i++)
      {
        UnityEngine.Mesh meshPiece = listMeshPieceUnity[i];
        List<int> listMaterialIdx  = listListMaterialIdx[i];

        listMaterialPiece.Clear();
        int idInteriorMaterial = -1;
 
        for( int j = 0; j < listMaterialIdx.Count; j++ ) 
        {
          int materialIdx = listMaterialIdx[j];

          Material mat;
          if (materialIdx == -1)
          {
            mat = Data.InteriorMaterial;
            idInteriorMaterial = j;         
          }
          else if ( materialIdx == -2)
          {
            mat = null;
          }
          else
          {
            mat = listMaterial[materialIdx];    
          }
          listMaterialPiece.Add(mat);         
        }

        int inputMeshIdx = listInputMeshIdx[i];

        Vector3 goPosition = arrMeshPiecePosition[inputMeshIdx];
        int parentIdx      = (int)arrMeshPieceParentIdx[inputMeshIdx];

        // if has parent piece
        if (parentIdx != -1)
        {
          GameObject parentGO     = listParentGO[parentIdx];
          GameObject chopParentGO = listChoppedParentGO[parentIdx];

          GameObject goPiece = CRGeometryUtils.CreatePiece( i, parentGO.name, listMaterialPiece, meshPiece, goPosition,  chopParentGO );
          dictChoppedGOInteriorSubmeshIdx.Add(goPiece, idInteriorMaterial);
          
          if (hasBeenSplitted)
          {
            Tuple2<GameObject, GameObject> tupleOutsideInside = listOutsideInsideGOParent[parentIdx];

            GameObject outsideGO = tupleOutsideInside.First;
            GameObject insideGO  = tupleOutsideInside.Second;

            uint outsideInsideIdx = arrOutsideInsideIdx[inputMeshIdx];
            if( outsideInsideIdx == 0 )
            {
              goPiece.transform.parent = outsideGO.transform;
              goPiece.name = parentGO.name + "_out_" + i;
            }
            else if( outsideInsideIdx == 1 )
            {
              goPiece.transform.parent = insideGO.transform;
              goPiece.name = parentGO.name + "_in_" + i;
            }
          }

          listChoppedGO.Add(goPiece);
        }
        else
        {
          GameObject goPiece = CRGeometryUtils.CreatePiece( 0, "Unfracture_zone", listMaterialPiece, meshPiece, goPosition, chopRoot );
          dictChoppedGOInteriorSubmeshIdx.Add(goPiece, idInteriorMaterial);
        }
      }
      
      foreach( Tuple2<GameObject, GameObject> tupleOutsideInside in listOutsideInsideGOParent )
      {
        GameObject outside = tupleOutsideInside.First;
        if (outside.transform.childCount == 0)
        {
          Object.DestroyImmediate(outside);
        }

        GameObject inside = tupleOutsideInside.Second;
        if (inside.transform.childCount == 0)
        {
          Object.DestroyImmediate(inside);
        }
      }

      for (int i = 0; i < listChoppedParentGO.Count; i++)
      {
        GameObject chopParentGO = listChoppedParentGO[i];

        chopParentGO.transform.parent = Data.GameObjectChoppedRoot.transform;
        chopParentGO.transform.SetAsFirstSibling();
      }

      Selection.activeGameObject = Data.GameObjectChoppedRoot ;
      SaveNewChopInfo(dictChoppedGOInteriorSubmeshIdx);
    }

    private bool ChopModeUniform( ChopRequest cr )
    {
      if ( Data.NDesiredPieces == 0 )
      {
        return false;
      }
      cr.nDesiredPieces_ = (uint)Data.NDesiredPieces;
      cr.doExtrusionEffect_ = Data.DoExtrusionEffect;
      cr.doCoordinate_      = Data.DoCoordinate;

      return true;
    }

    private bool ChopModeGeometry( ChopRequest cr )
    {
      if ( (Data.ChopGeometry == null) || !Data.ChopGeometry.HasMesh() || Data.NDesiredPieces <= 0 )
      {
        return false;
      }

      Matrix4x4 m_Local_to_World  = Data.ChopGeometry.transform.localToWorldMatrix;
      UnityEngine.Mesh un_chopGeometry = Data.ChopGeometry.GetMesh();
      
      UnityEngine.Mesh transformedMesh;
      CRGeometryUtils.CreateMeshTransformed( un_chopGeometry, m_Local_to_World, out transformedMesh );

      CaronteSharp.MeshSimple car_chopGeometry = new CaronteSharp.MeshSimple();
      car_chopGeometry.Set(transformedMesh);
      Object.DestroyImmediate(transformedMesh);

      cr.nDesiredPieces_    = (uint)Data.NDesiredPieces;  
      cr.meshFocusGeometry_ = car_chopGeometry;
      cr.focusMode_         = (PSBG_FOCUS_MODE)Data.FocusMode;
      cr.gridResolution_    = (uint)Data.GridResolution;
      cr.densityRate_       = Data.DensityRate;
      cr.transitionLength_  = Data.TransitionLength;
      cr.doExtrusionEffect_ = Data.DoExtrusionEffect;
      cr.doCoordinate_      = Data.DoCoordinate;

      return true;
    }

    private bool ChopModeRadial( ChopRequest cr )
    {
      if (Data.ReferenceSystem == null)
      {
        Debug.Log("Radial mode requires specifying a reference system.");
        return false;      
      }

      Transform tr = Data.ReferenceSystem.transform;
      Matrix4x4 m_LOCAL_to_WORLD = tr.localToWorldMatrix;

      cr.focusSystem_center_ = Data.ReferenceSystem.position;

      switch (Data.ReferenceSystemAxis)
      {
        case CNFracture.AxisDir.x:
          cr.focusSystem_axisDir_ = m_LOCAL_to_WORLD.MultiplyVector(Data.ReferenceSystem.right);
          break;
        case CNFracture.AxisDir.y:
          cr.focusSystem_axisDir_ = m_LOCAL_to_WORLD.MultiplyVector(Data.ReferenceSystem.up);
          break;
        case CNFracture.AxisDir.z:
          cr.focusSystem_axisDir_ = m_LOCAL_to_WORLD.MultiplyVector(Data.ReferenceSystem.forward);
          break;
      }

      cr.rays_number_   = (uint)Data.RaysNumber;
      cr.rays_rateRand_ = Data.RaysRateRand;

      cr.rings_numberInsideAnnulus_ = (uint)Data.RingsNumberInsideAnnulus;
      cr.rings_intRadius_           = Data.RingsIntRadius;
      cr.rings_extRadius_           = Data.RingsExtRadius;
      cr.rings_intTransitionLength_ = Data.RingsIntTransitionLength;
      cr.rings_extTransitionLength_ = Data.RingsExtTransitionLength;
      cr.rings_intTransitionDecay_  = Data.RingsIntTransitionDecay;
      cr.rings_extTransitionDecay_  = Data.RingsExtTransitionDecay;;
      
      cr.rings_rateRand_ = Data.RingsRateRand;
      cr.doCentralPiece_ = Data.DoCentralPiece;

      cr.noiseRate_    = Data.NoiseRate;
      cr.twistRate_    = Data.TwistRate;
      cr.doCoordinate_ = Data.DoCoordinate;

      return true;   
    }

    private void SaveNewChopInfo(Dictionary<GameObject, int> dictChoppedGOInteriorSubmeshIdx)
    {     
      List<GameObject> listChoppedGameObject = new List<GameObject>();

      GameObject[] arrChoppedGO = CREditorUtils.GetAllChildObjectsWithGeometry(Data.GameObjectChoppedRoot, true);
      listChoppedGameObject.AddRange(arrChoppedGO);
   
      int nTotalPieces = listChoppedGameObject.Count;

      Data.ArrChoppedGameObject = listChoppedGameObject.ToArray();

      Data.ArrInteriorSubmeshIdx           = new int[nTotalPieces];
      Data.ArrChoppedMesh                  = new Mesh[nTotalPieces];
      Data.ArrGameObject_Bounds_Chopped    = new Bounds[nTotalPieces];
      Data.ArrGameObject_Chopped_Positions = new Vector3[nTotalPieces];

      for (int i = 0; i < nTotalPieces; i++)
      {
        GameObject go = listChoppedGameObject[i];

        int interiorSubMeshIdx = dictChoppedGOInteriorSubmeshIdx[go];

        Data.ArrChoppedMesh[i]                  = go.GetMesh();
        Data.ArrInteriorSubmeshIdx[i]           = interiorSubMeshIdx;
        Data.ArrGameObject_Bounds_Chopped[i]    = go.GetWorldBounds();
        Data.ArrGameObject_Chopped_Positions[i] = go.transform.localPosition;
      }

      EditorUtility.SetDirty( Data );
    }

    private void SeparatePieces()
    {
      int numPieces = Data.ArrGameObject_Bounds_Chopped.Length;
      Box[]  arrBox = new Box[numPieces];
      for (int i = 0; i < numPieces; i++)
      {
        Bounds bounds = Data.ArrGameObject_Bounds_Chopped[i];
        arrBox[i] = new Box(bounds.min, bounds.max);
      }
      Vector3[] arrDeltaPush;
      CaronteSharp.Tools.CalculateDeltasToPushAwayPieces(arrBox, Data.PushMultiplier, out arrDeltaPush);
      
      for (int i = 0; i < numPieces; i++)
      {
        GameObject go = Data.ArrChoppedGameObject[i];
        if (go != null)
        {
          Transform tr  = go.transform;
          tr.localPosition = Data.ArrGameObject_Chopped_Positions[i] + (tr.worldToLocalMatrix.MultiplyVector(arrDeltaPush[i]) * Data.PushRate);
        }
      }
    }

    private void ApplyUVsPostProcess()
    {
      CRMeshUtils.ModifyInternalUVs(Data);
    }

    private void DrawIsRestrictedFracture()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle(frRectrictFractureZoneCt_, Data.RestrictFractureZone );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frRectrictFractureZoneCt_.text + " - " + Data.Name);
        Data.RestrictFractureZone = value;
        if (value)
        {
          Data.DoNoise = !value;
        }
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawCropGeometry()
    {
      EditorGUI.BeginDisabledGroup(!Data.RestrictFractureZone);
      EditorGUI.BeginChangeCheck();
      var value = (GameObject) EditorGUILayout.ObjectField(frResctrictGeometryCt_, Data.CropGeometry, typeof(GameObject), true );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Restriction geometry - " + Data.Name);
        Data.CropGeometry = value;
        EditorUtility.SetDirty(Data);
      }
      EditorGUI.EndDisabledGroup();
    }

    private void LoadGUICropMode()
    {
      if ( Data.CropMode == CNFracture.CROP_MODE.INSIDE )
      {
        if ( !Data.FrontierPieces )
        {
          cropModeIdx_ = 0;
        }
        else
        {
          cropModeIdx_ = 1;
        }
      }
      else if ( Data.CropMode == CNFracture.CROP_MODE.OUTSIDE )
      {
        if (!Data.FrontierPieces)
        {
          cropModeIdx_ = 2;
        }
        else
        {
          cropModeIdx_ = 3;
        }
      }
    }

    private void DrawCropMode()
    {
      EditorGUI.BeginDisabledGroup(!Data.RestrictFractureZone);
      LoadGUICropMode();
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Popup( frFractureZoneCt_, cropModeIdx_, arrRestrictionMode_ );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change crop mode - " + Data.Name);
        cropModeIdx_ = value;
        SetCropMode();
        EditorUtility.SetDirty( Data );
      }
      EditorGUI.EndDisabledGroup();
    }

    private void DrawUnfracturedZone()
    {
      EditorGUI.BeginDisabledGroup(!Data.RestrictFractureZone);
      EditorGUI.BeginChangeCheck();
      int unfractureZoneIdx = Data.WeldInOnePiece ? 0 : 1;
      var value = EditorGUILayout.Popup( frUnfracturedMeshCt_, unfractureZoneIdx, arrUnfracturedZoneNames_ );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change unfractured zone - " + Data.Name);
        Data.WeldInOnePiece = (value == 0);
        EditorUtility.SetDirty(Data);
      }
      EditorGUI.EndDisabledGroup();
    }

    private void DrawPushRate()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider(frPushRateCt_, Data.PushRate, 0.0f, 1.0f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frPushRateCt_.text + " - " + Data.Name);
        Data.PushRate = value;
        EditorUtility.SetDirty(Data);
      } 
    }

    private void DrawPushMultiplier()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(frPushMultiplierCt_, Data.PushMultiplier);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frPushMultiplierCt_.text + " - " + Data.Name);
        Data.PushMultiplier = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawIsNoiseFracture()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle(frInteriorFacesNoiseCt_, Data.DoNoise);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frInteriorFacesNoiseCt_.text + " - " + Data.Name);
        Data.DoNoise = value;
        if (value)
        {
          Data.RestrictFractureZone = !value;
        }
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawNoiseResolutionRate()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider(frNoiseResolutionRateCt_, Data.NoiseResolutionRate, 0f, 1f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frNoiseResolutionRateCt_.text + " - " + Data.Name);
        Data.NoiseResolutionRate = Mathf.Clamp01(value);
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawNoiseAmplitudeRate()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider(frNoiseReliefAmplitudeRateCt_, Data.NoiseReliefAmplitudeRate, 0f, 1f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frNoiseReliefAmplitudeRateCt_.text + " - " + Data.Name);
        Data.NoiseReliefAmplitudeRate = Mathf.Clamp01(value);
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawNoiseSerrationRate()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider(frNoiseReliefSerrationRateCt_, Data.NoiseReliefSerrationRate, 0f, 1f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frNoiseReliefSerrationRateCt_.text + " - " + Data.Name);
        Data.NoiseReliefSerrationRate = Mathf.Clamp01(value);
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawNoiseSeed()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.IntField(frNoiseSeedCt_, (int)Data.NoiseSeed0);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frNoiseSeedCt_.text + Data.Name);
        Data.NoiseSeed0 = (uint)Mathf.Clamp(value, 0f, int.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
  
    private void DrawInteriorFacesSmoothingAngle()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider(frInteriorFacesSmoothingAngleCt_, Data.InternalFacesSmoothingAngle, 0f, 180f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frInteriorFacesSmoothingAngleCt_.text + Data.Name);
        Data.InternalFacesSmoothingAngle = Mathf.Clamp(value, 0.0f, 180.0f);
      }
    }


    private void DrawModifyInteriorFaces()
    {
      if ( GUILayout.Button("Modify interior faces UVs...") )
      {
        CRModifyInteriorFacesWindow.ShowWindow(Data);
      }
    }

    public override void RenderGUI(Rect area, bool isEditable)
    {
      GUILayout.BeginArea(area);
      
      RenderTitle(isEditable, false, false);

      EditorGUI.BeginDisabledGroup(!isEditable);
      RenderFieldObjects( "Objects", FieldController, true, true, CNFieldWindow.Type.normal );

      EditorGUILayout.Space();
      EditorGUILayout.Space();

      EditorGUILayout.BeginHorizontal();
      if ( GUILayout.Button("Chop", GUILayout.Height(30f)) )
      {
        Chop();
      }

      if ( GUILayout.Button("Save assets..", GUILayout.Height(30f), GUILayout.Width(100f) ) )
      {
        SaveChopResult();
      }

      EditorGUILayout.EndHorizontal();

      EditorGUI.EndDisabledGroup();
      CRGUIUtils.DrawSeparator();
      CRGUIUtils.Splitter();

      tabIndex_ = GUILayout.SelectionGrid(tabIndex_, arrTabName_, 2);
      CRGUIUtils.Splitter();

      scroller_ = EditorGUILayout.BeginScrollView(scroller_);

      if (tabIndex_ == 1)
      {
        EditorGUILayout.LabelField("Last chop input geometry objects " + Data.InputObjects );
        EditorGUILayout.LabelField("Last chop input vertices: "  + Data.InputVertices );
        EditorGUILayout.LabelField("Last chop input triangles: " + Data.InputTriangles );

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Last chop output pieces: "    + Data.OutputPieces );
        EditorGUILayout.LabelField("Last chop output vertices: "  + Data.OutputVertices );
        EditorGUILayout.LabelField("Last chop output triangles: " + Data.OutputTriangles );
      }
      else
      {
        EditorGUI.BeginDisabledGroup(!isEditable);

        float originalLabelwidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 200f;

        EditorGUILayout.Space();

        switch (Data.ChopMode)
        {
          case CNFracture.CHOP_MODE.VORONOI_UNIFORM:
            DrawGUICommon();
            break;
          case CNFracture.CHOP_MODE.VORONOI_BY_GEOMETRY: 
            DrawGUIGeometry();
            break;
          case CNFracture.CHOP_MODE.VORONOI_RADIAL:
            DrawGUIRadial();
            break;
        }

        EditorGUILayout.Space();
        CRGUIUtils.Splitter();
        EditorGUILayout.Space();

        DrawIsRestrictedFracture();
        EditorGUILayout.Space();
        DrawCropGeometry();
        DrawCropMode();
        EditorGUILayout.Space();
        DrawUnfracturedZone();

        CRGUIUtils.Splitter();

        EditorGUILayout.Space();
        if ( CRVersionChecker.IsPremiumVersion() )
        {
          DrawIsNoiseFracture();
          EditorGUI.BeginDisabledGroup(!Data.DoNoise);
          EditorGUILayout.Space();
          DrawNoiseResolutionRate();
          DrawNoiseAmplitudeRate();
          DrawNoiseSerrationRate();
          DrawNoiseSeed();
          //DrawNoiseExtrussionEffect();
          EditorGUI.EndDisabledGroup();
          CRGUIUtils.Splitter();
        }

        GUIStyle centerLabel = new GUIStyle(EditorStyles.largeLabel);
        centerLabel.alignment = TextAnchor.MiddleCenter;
        centerLabel.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField(frOutputSectionCt_, centerLabel);

        EditorGUI.BeginDisabledGroup(Data.GameObjectChoppedRoot == null);
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();

        if ( GUILayout.Button(frSelectPiecesCt_) )
        {
          Selection.activeGameObject = Data.GameObjectChoppedRoot;
        }

        DrawPushRate();
        DrawPushMultiplier();
        ValidateState();

        EditorGUI.EndDisabledGroup();

        EditorGUIUtility.labelWidth = originalLabelwidth;

        EditorGUI.EndDisabledGroup();
      }

      EditorGUILayout.EndScrollView();
      GUILayout.EndArea();  
    }
 

    void SetCropMode()
    {
      if ( cropModeIdx_ == 0 )
      {
        Data.CropMode       = CNFracture.CROP_MODE.INSIDE;
        Data.FrontierPieces = false;
      }
      else if (cropModeIdx_ == 1)
      {
        Data.CropMode       = CNFracture.CROP_MODE.INSIDE;
        Data.FrontierPieces = true;
      }
      else if ( cropModeIdx_ == 2 )
      {
        Data.CropMode       = CNFracture.CROP_MODE.OUTSIDE;
        Data.FrontierPieces = false;
      }
      else if ( cropModeIdx_ == 3 )
      {
        Data.CropMode       = CNFracture.CROP_MODE.OUTSIDE;
        Data.FrontierPieces = true;
      }
    }

    private bool HasUnsavedChopReferences()
    {
      if (Data.GameObjectChoppedRoot == null)
      {
        return false;
      }

      if ( CREditorUtils.IsAnyUnsavedMeshInHierarchy(Data.GameObjectChoppedRoot) )
      {
        return true;
      }
      else
      {
        return false;
      }    
    }

    private void SaveChopResult()
    {
      if (!HasUnsavedChopReferences())
      {
        EditorUtility.DisplayDialog("CaronteFX - Info", "There is not any mesh to save in assets.", "ok" );
        return;
      }

      CREditorUtils.SaveAnyUnsavedMeshInHierarchy(Data.GameObjectChoppedRoot, false);
    }

    private void DrawGlobalPattern()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle(frGlobalPatternCt_, Data.DoGlobalPattern);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frGlobalPatternCt_.text + " - " + Data.Name);
        Data.DoGlobalPattern = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawNDesiredPieces()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.IntField(frNDesiredPiecesCt_, Data.NDesiredPieces);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frNDesiredPiecesCt_.text + " - " + Data.Name);
        Data.NDesiredPieces = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawSeed()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.IntField(frSeedCt_, Data.Seed);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frSeedCt_.text + " - " + Data.Name);
        Data.Seed = Mathf.Clamp(value, 0, int.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawInteriorMaterial()
    {
      EditorGUI.BeginChangeCheck();
      var value = (Material) EditorGUILayout.ObjectField(frInteriorMaterialCt_, Data.InteriorMaterial, typeof(Material), true );
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change " + frInteriorMaterialCt_.text + " - " + Data.Name);
        ChangeInteriorMaterial( value );
        Data.InteriorMaterial = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawDoExtrussionEffect()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle(frDoExtrusionEffectCt_, Data.DoExtrusionEffect);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frDoExtrusionEffectCt_.text + " - " + Data.Name);
        Data.DoExtrusionEffect = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawDoCoordinate()
    {
      EditorGUI.BeginDisabledGroup( Data.RestrictFractureZone );
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle(frDoCoordinateCt_, Data.DoCoordinate);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frDoCoordinateCt_.text + " - " + Data.Name);
        Data.DoCoordinate = value;
        EditorUtility.SetDirty(Data);
      }
      EditorGUI.EndDisabledGroup();
    }

    private void DrawHideParentObjectAuto()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle( frHideParentObjectsAutoCt_, Data.HideParentObjectAuto );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frHideParentObjectsAutoCt_.text + " - " + Data.Name);
        Data.HideParentObjectAuto = value;
        EditorUtility.SetDirty(Data);
      }
    }

    void DrawGUICommon()
    {
      DrawGlobalPattern(); 
      DrawNDesiredPieces();
      DrawSeed();
      EditorGUILayout.Space();
      DrawInteriorMaterial();
      DrawInteriorFacesSmoothingAngle();
      DrawModifyInteriorFaces();
      EditorGUILayout.Space();
      DrawDoExtrussionEffect();
      DrawDoCoordinate();
      EditorGUILayout.Space();
      DrawHideParentObjectAuto();
    }

    private void DrawChopGeometry()
    {
      EditorGUI.BeginChangeCheck();
      var value = (GameObject) EditorGUILayout.ObjectField(frGeometryCt_, Data.ChopGeometry, typeof(GameObject), true );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frGeometryCt_.text + " - " + Data.Name);
        Data.ChopGeometry = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawGridResolution()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.IntField(frGridResolutionCt_, Data.GridResolution );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frGridResolutionCt_.text + " - " + Data.Name);
        Data.GridResolution = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawFocusMode()
    {
      EditorGUI.BeginChangeCheck();
      var value = (CNFracture.FOCUS_MODE) EditorGUILayout.EnumPopup(frFocusModeCt_, Data.FocusMode );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frFocusModeCt_.text + " - " + Data.Name);
        Data.FocusMode = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawDensityRate()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(frDensityRateCt_, Data.DensityRate );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frDensityRateCt_.text + " - " + Data.Name);
        Data.DensityRate = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawTransitionLength()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(frTransitionLengthCt_, Data.TransitionLength);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frTransitionLengthCt_.text + " - " + Data.Name);
        Data.TransitionLength = value;
        EditorUtility.SetDirty(Data);
      }
    }

    void DrawGUIGeometry()
    {
      DrawGUICommon();
      EditorGUILayout.Space();
      CRGUIUtils.Splitter();
      EditorGUILayout.Space();
      EditorGUILayout.Space();
      DrawChopGeometry();
      DrawGridResolution();
      DrawFocusMode();
      DrawDensityRate();
      DrawTransitionLength();
    }

    private void DrawReferenceSystem()
    {
      EditorGUI.BeginChangeCheck();
      var value = (Transform) EditorGUILayout.ObjectField(frReferenceSystemCt_, Data.ReferenceSystem, typeof(Transform), true );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frReferenceSystemCt_.text + " - " + Data.Name);
        Data.ReferenceSystem = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawReferenceSystemAxis()
    {
      EditorGUI.BeginChangeCheck();
      var value = (CNFracture.AxisDir) EditorGUILayout.EnumPopup(frReferenceSystemAxisCt_, Data.ReferenceSystemAxis);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frReferenceSystemAxisCt_.text + " - " + Data.Name);
        Data.ReferenceSystemAxis = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawRaysNumber()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.IntField(frRaysNumberCt_, Data.RaysNumber);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frRaysNumberCt_.text + " - " + Data.Name);
        Data.RaysNumber = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawRaysRateRand()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(frRaysRateRandCt_, Data.RaysRateRand);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frRaysRateRandCt_.text + " - " + Data.Name);
        Data.RaysRateRand = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawIntRadius()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(frRingsIntRadiusCt_, Data.RingsIntRadius);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frRingsIntRadiusCt_.text + " - " + Data.Name);
        Data.RingsIntRadius = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawRingsIntTransitionLength()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(frRingsIntTransitionLengthCt_, Data.RingsIntTransitionLength);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frRingsIntTransitionLengthCt_.text + " - " + Data.Name);
        Data.RingsIntTransitionLength = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawRingsIntTransitionDecay()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(frRingsIntTransitionDecayCt_, Data.RingsIntTransitionDecay);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frRingsIntTransitionDecayCt_.text  + " - "+ Data.Name);
        Data.RingsIntTransitionDecay = value;
        EditorUtility.SetDirty(Data);
      }
    }


    private void DrawRingsExtRadius()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(frRingsExtRadiusCt_, Data.RingsExtRadius);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frRingsExtRadiusCt_.text + " - " + Data.Name);
        Data.RingsExtRadius = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawRingsExtTransitionLength()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(frRingsExtTransitionLengthCt_, Data.RingsExtTransitionLength);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frRingsExtTransitionLengthCt_.text + " - " + Data.Name);
        Data.RingsExtTransitionLength = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawRingsExtTransitionDecay()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(frRingsExtTransitionDecayCt_, Data.RingsExtTransitionDecay);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frRingsExtTransitionDecayCt_.text + " - " + Data.Name);
        Data.RingsExtTransitionDecay = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawRingsNumberInsideAnnulus()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.IntField(frRingsNumberInsideAnnulusCt_, Data.RingsNumberInsideAnnulus);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frRingsNumberInsideAnnulusCt_.text + " - " + Data.Name);
        Data.RingsNumberInsideAnnulus = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawRingsRateRand()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(frRingsRateRandCt_, Data.RingsRateRand);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frRingsRateRandCt_.text + " - " + Data.Name);
        Data.RingsRateRand = value;
        EditorUtility.SetDirty(Data);
      }
    }


    private void DrawDoCentralPiece()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle(frCreateCentralPieceCt_, Data.DoCentralPiece);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frCreateCentralPieceCt_.text + " - " + Data.Name);
        Data.DoCentralPiece = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawNoiseRate()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(frNoiseRateCt_, Data.NoiseRate);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frNoiseRateCt_.text + " - " + Data.Name);
        Data.NoiseRate = value;
        EditorUtility.SetDirty(Data);
      }
    }

    private void DrawTwistRate()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField(frTwistRateCt_, Data.TwistRate);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change " + frTwistRateCt_.text + " - " + Data.Name);
        Data.TwistRate = value;
        EditorUtility.SetDirty(Data);
      }
    }

    void DrawGUIRadial()
    {
      DrawGlobalPattern();
      DrawSeed();
      DrawInteriorMaterial();
      DrawInteriorFacesSmoothingAngle();
      DrawModifyInteriorFaces();
      EditorGUILayout.Space();
      DrawDoExtrussionEffect();
      DrawDoCoordinate();
      EditorGUILayout.Space();
      DrawHideParentObjectAuto();

      EditorGUILayout.Space();
      CRGUIUtils.Splitter();
      EditorGUILayout.Space();
      EditorGUILayout.Space();

      DrawReferenceSystem();
      DrawReferenceSystemAxis();
      
      EditorGUILayout.Space();
      DrawRaysNumber(); 
      DrawRaysRateRand();

      EditorGUILayout.Space();
      CRGUIUtils.Splitter();
      EditorGUILayout.Space();
      EditorGUILayout.Space();
      DrawIntRadius();
      DrawRingsIntTransitionLength();
      DrawRingsIntTransitionDecay();
      
      EditorGUILayout.Space();
      DrawRingsExtRadius();
      DrawRingsExtTransitionLength();
      DrawRingsExtTransitionDecay();

      EditorGUILayout.Space();
      CRGUIUtils.Splitter();
      EditorGUILayout.Space();
      EditorGUILayout.Space();
      DrawRingsNumberInsideAnnulus();
      DrawRingsRateRand();
      DrawDoCentralPiece();
      EditorGUILayout.Space();
      DrawNoiseRate();
      DrawTwistRate();
    }




  } //class CNFractureView
} // namespace Caronte

