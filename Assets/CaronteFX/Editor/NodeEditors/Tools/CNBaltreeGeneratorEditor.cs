using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using CaronteSharp;

namespace CaronteFX
{
  public class CNBalltreeGeneratorEditor : CNMonoFieldEditor 
  {
    private static readonly GUIContent   btSpherePopulationCt_    = new GUIContent(CRStringManager.GetString("BtSpherePopulation"), CRStringManager.GetString("BtSpherePopulationTooltip"));
    private static readonly GUIContent   btArrangementQualityCt_  = new GUIContent(CRStringManager.GetString("BtArrangementQuality"), CRStringManager.GetString("BtArrangementQualityTooltip"));
    private static readonly GUIContent   btHoleCoveringCt_        = new GUIContent(CRStringManager.GetString("BtHoleCovering"), CRStringManager.GetString("BtHoleCoveringTooltip"));
    private static readonly GUIContent   btCreationModeCt_        = new GUIContent(CRStringManager.GetString("BtCreationMode"), CRStringManager.GetString("BtCreationModeTooltip"));
    private static readonly GUIContent[] btCreationModesCt_       = new GUIContent[] { new GUIContent(CRStringManager.GetString("BtCreationModeUseRenderers")), new GUIContent(CRStringManager.GetString("BtCreationModeUseColliders")) };

    public static Texture icon_;
    public override Texture TexIcon { get { return icon_; } }

    class BalltreeId
    {
      public readonly uint id_;
      public readonly string name_;

      public BalltreeId(uint balltreeId, string name)
      {
        id_ = balltreeId;
        name_ = name;
      }

      public override bool Equals(System.Object obj)
      {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        BalltreeId btId = obj as BalltreeId;
        if ((System.Object)btId == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (id_ == btId.id_);
      }

      public bool Equals(BalltreeId btId)
      {
        // If parameter is null return false:
        if ((object)btId == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (id_ == btId.id_);
      }

      public override int GetHashCode()
      {
        return id_.GetHashCode();
      }
    }

    new CNBalltreeGenerator Data { get; set; }

    
    int   selectionModeIdx_;

    public CNBalltreeGeneratorEditor(CNBalltreeGenerator data, CommandNodeEditorState state)
      : base(data, state)
    {
      Data = (CNBalltreeGenerator)data;
    }

    public void DrawCreationMode()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Popup(btCreationModeCt_, (int)Data.CreationMode, btCreationModesCt_ );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject( Data, "Change " + btCreationModeCt_.text + " " + Data.Name );
        Data.CreationMode = (CNBalltreeGenerator.ECreationMode)value;
        EditorUtility.SetDirty( Data );
      }
    }

    protected void DrawNumberOfSpheres()
    {
      EditorGUI.BeginChangeCheck();     
      var value = EditorGUILayout.Slider( btSpherePopulationCt_, Data.BalltreeLOD, 0.0f, 1.0f );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change - " + btSpherePopulationCt_.text + " " + Data.Name);
        Data.BalltreeLOD = value;
        EditorUtility.SetDirty(Data);
      }
      EditorGUI.EndDisabledGroup();
    }

    protected void DrawArrangementQuality()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider( btArrangementQualityCt_, Data.BalltreePrecision, 0.0f, 1.0f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Chage " + btArrangementQualityCt_.text + Data.BalltreePrecision);
        Data.BalltreePrecision = value;
        EditorUtility.SetDirty(Data);
      }
    }

    protected void DrawBalltreeHoleCovering()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider( btHoleCoveringCt_, Data.BalltreeHoleCovering, 0.0f, 1.0f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Chage " + btHoleCoveringCt_.text + Data.BalltreeHoleCovering);
        Data.BalltreeHoleCovering = value;
        EditorUtility.SetDirty(Data);
      }
    }

    public void CreateBalltrees()
    {
      string folder;
      int pathIndex;
      bool assetsPath = CRFileUtils.DisplaySaveFolderDialog("CaronteFX - Balltree assets folder...", out folder, out pathIndex);
      if (!assetsPath)
      {
        return;
      }
      folder = folder.Substring(pathIndex );

      Dictionary<GameObject, uint> dictionaryGameObjectIdBalltree = new Dictionary<GameObject, uint>();
      HashSet<BalltreeId> hashsetIdBalltree = new HashSet<BalltreeId>();

      GameObject[] arrGameObject = FieldController.GetUnityGameObjects();
      RgInit rgInit = new RgInit();
 
      Matrix4x4 m_MODEL_to_WORLD = Matrix4x4.identity;
      bool isBakedRenderMesh = false;

      int nGameObject = arrGameObject.Length;

      for(int i = 0; i < nGameObject; i++)
      {
        int currentGOIdx = i+1;
        float progress = (float)currentGOIdx / (float)nGameObject;
        EditorUtility.DisplayProgressBar("CaronteFX - Balltree Generator", "Creating balltree for " + nGameObject + " GameObjects. GameObject " + currentGOIdx + "." , progress );
        GameObject go = arrGameObject[i];
        Mesh balltreeMesh = null;

        Caronte_Fx_Body cfxBody = CRBodyUtils.AddBodyComponentIfHasMesh(go);
        if (cfxBody != null)
        {
          if (Data.CreationMode == CNBalltreeGenerator.ECreationMode.USERENDERERS)
          {
            CRBodyUtils.GetRenderMeshData(go, ref balltreeMesh, out m_MODEL_to_WORLD, ref isBakedRenderMesh);
          }
          else if (Data.CreationMode == CNBalltreeGenerator.ECreationMode.USECOLLLIDERS)
          {
            CRBodyUtils.GetColliderMeshData(go, ref balltreeMesh, out m_MODEL_to_WORLD, ref isBakedRenderMesh);
          }

          if (balltreeMesh != null)
          {
            SetRgInitForBalltree(go, balltreeMesh, rgInit);
            uint id = RigidbodyManager.CreateBalltree(rgInit);

            dictionaryGameObjectIdBalltree.Add(go, id);
            hashsetIdBalltree.Add(new BalltreeId(id, balltreeMesh.name));
          }
        }

      }

      int balltreeIdx = 1;
      int nBaltrees = hashsetIdBalltree.Count;
     
      Dictionary<uint, CRBalltreeAsset> dictionaryIdBalltreeBalltreeAsset = new Dictionary<uint, CRBalltreeAsset>();
      foreach(BalltreeId balltreeId in hashsetIdBalltree)
      {
        float progress = (float)balltreeIdx / (float)nBaltrees;
        EditorUtility.DisplayProgressBar("CaronteFX - Balltree Generator", "Saving " + nBaltrees + " balltree assets. Balltree  " + balltreeIdx + "." , progress );

        byte[] balltree_bytes            = RigidbodyManager.GetBalltreeBytes(balltreeId.id_);
        byte[] balltreeCheckheader_bytes = RigidbodyManager.GetBalltreeCheckheaderBytes(balltreeId.id_);
        CRSphere[] arrLeafSphere         = RigidbodyManager.GetBalltreeSpheres(balltreeId.id_);

        CRBalltreeAsset btAsset = CreateBallTreeAsset(balltreeId.name_, folder, balltree_bytes, balltreeCheckheader_bytes, arrLeafSphere);

        dictionaryIdBalltreeBalltreeAsset.Add(balltreeId.id_, btAsset);
        balltreeIdx++;
      }
      EditorUtility.ClearProgressBar();

      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();

      ICollection<GameObject> goCollection = dictionaryGameObjectIdBalltree.Keys;
      foreach(GameObject go in goCollection)
      {
        Caronte_Fx_Body cfxBody = go.GetComponent<Caronte_Fx_Body>();
        if (cfxBody == null)
        {
          cfxBody = go.AddComponent<Caronte_Fx_Body>();
        }

        uint idBalltree = dictionaryGameObjectIdBalltree[go];
        CRBalltreeAsset btAsset = dictionaryIdBalltreeBalltreeAsset[idBalltree];
        cfxBody.SetBalltreeAsset(btAsset);
        EditorUtility.SetDirty(cfxBody);
      }
    }

    private CRBalltreeAsset CreateBallTreeAsset(string name, string folderPath, byte[] balltree_bytes, byte[] balltreeCheckheader_bytes, CRSphere[] arrLeafSphere)
    {
      CRBalltreeAsset btAsset = CRBalltreeAsset.CreateInstance<CRBalltreeAsset>();
      btAsset.BalltreeBytes = balltree_bytes;
      btAsset.CheckHeaderBytes = balltreeCheckheader_bytes;
      btAsset.LeafSpheres = arrLeafSphere;

      string assetFilePath = AssetDatabase.GenerateUniqueAssetPath( folderPath + "/" + "Bt_" + name + ".asset");
      AssetDatabase.CreateAsset( btAsset, assetFilePath );

      return btAsset;
    }

    private void SetRgInitForBalltree(GameObject go, Mesh mesh, RgInit rgInit)
    {
      rgInit.name_ = go.name;

      MeshComplex balltreeMC = new MeshComplex();
      balltreeMC.Set(mesh);

      rgInit.meshCollider_Model_ = balltreeMC;

      Matrix4x4 m_MODEL_to_WORLD = go.transform.localToWorldMatrix;
      Vector3 scale = m_MODEL_to_WORLD.GetScalePre();
      Vector3 normalizedScale = scale / scale.x;

      Matrix4x4 m_MODEL_to_NORMALIZEDSCALE = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, normalizedScale);
      rgInit.m_MODEL_to_WORLD_ = m_MODEL_to_NORMALIZEDSCALE;

      rgInit.useBalltree_ = true;

      float radius_rate   = 1.0f - Data.BalltreeLOD;
      float radius_rateSq = radius_rate * radius_rate;
      float radius_rateSqClamped = Mathf.Clamp(radius_rateSq, 0.02f, 1.0f);
      rgInit.bt_radius_rate_ = radius_rateSqClamped;

      float requested_resolution1 = 10.0f / Mathf.Pow(radius_rateSqClamped, 5.0f);
      float requested_resolution2 = 10000.0f * (300.0f - 299.0f * (1.0f - Data.BalltreePrecision) );
      float mixedResolution = Mathf.Max(requested_resolution1, requested_resolution2);

      rgInit.bt_resolution_ = (uint)Mathf.Clamp(mixedResolution, 10000.0f, 3000000.0f);
      rgInit.bt_diameterMaxHoleToCover_rel_ = Mathf.Clamp(Data.BalltreeHoleCovering, 0.0001f, 1.0f);
    }

    public override void RenderGUI(Rect area, bool isEditable)
    {
      GUILayout.BeginArea(area);

      RenderTitle(isEditable, false, false);

      EditorGUI.BeginDisabledGroup(!isEditable);
      RenderFieldObjects("Objects", FieldController, true, true, CNFieldWindow.Type.normal );

      EditorGUILayout.Space();
      EditorGUILayout.Space();

      if ( GUILayout.Button("Create Balltrees", GUILayout.Height(30f) ) )
      {
        CreateBalltrees();
      }

      EditorGUI.EndDisabledGroup();
      CRGUIUtils.DrawSeparator();

      scroller_ = EditorGUILayout.BeginScrollView(scroller_);

      EditorGUILayout.Space();
      EditorGUILayout.Space();

      float originalLabelwidth = EditorGUIUtility.labelWidth;
      EditorGUIUtility.labelWidth = 200f;

      DrawCreationMode(); 
      EditorGUILayout.Space();
      DrawNumberOfSpheres();
      DrawArrangementQuality();

      EditorGUILayout.Space();
      DrawBalltreeHoleCovering();
      EditorGUILayout.Space();

      EditorGUIUtility.labelWidth = originalLabelwidth;

      EditorGUILayout.EndScrollView();

      GUILayout.EndArea();
    } // RenderGUI

  }

}
