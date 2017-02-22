using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CaronteFX
{
  public class CNAimedFallEditor : CNEntityEditor
  {
    public static Texture icon_;
    public override Texture TexIcon { get{ return icon_; } }
    
    CNFieldController FieldControllerAimGameObjects { get; set; }

    new CNAimedFall Data { get; set; }

    public CNAimedFallEditor( CNAimedFall data, CommandNodeEditorState state )
      : base( data, state)
    {
      Data = (CNAimedFall)data;
    }
    //----------------------------------------------------------------------------------
    public override void Init()
    {
      base.Init();

      FieldControllerAimGameObjects = new CNFieldController( Data, Data.FieldAimGameObjects, eManager, goManager );
      FieldControllerAimGameObjects.SetFieldType( CNField.AllowedTypes.GameObject  );
      FieldControllerAimGameObjects.SetCalculatesDiff(true);
    }
    //----------------------------------------------------------------------------------
    public override void LoadInfo()
    {
      FieldController              .RestoreFieldInfo();
      FieldControllerAimGameObjects.RestoreFieldInfo();
    }
    //----------------------------------------------------------------------------------
    public override void StoreInfo()
    {
      FieldController              .StoreFieldInfo();
      FieldControllerAimGameObjects.StoreFieldInfo();
    }
    //----------------------------------------------------------------------------------
    public override void BuildListItems()
    {
      FieldController .BuildListItems();
      FieldControllerAimGameObjects.BuildListItems();
    }
    //----------------------------------------------------------------------------------
    public override bool RemoveNodeFromFields( CommandNode node )
    {
      Undo.RecordObject(Data, "CaronteFX - Remove node from fields");
      bool removedFromBodies         = Data.Field              .RemoveNode(node);
      bool removedFromAimGameObjects = Data.FieldAimGameObjects.RemoveNode(node);
 
      return ( removedFromBodies || removedFromAimGameObjects );
    }
    //----------------------------------------------------------------------------------
    public override void SetScopeId(uint scopeId)
    {
      FieldController              .SetScopeId(scopeId);
      FieldControllerAimGameObjects.SetScopeId(scopeId);
    }
    //----------------------------------------------------------------------------------
    public override void FreeResources()
    {
      FieldController              .DestroyField();
      FieldControllerAimGameObjects.DestroyField();

      eManager.DestroyEntity( Data );
    }
    //----------------------------------------------------------------------------------
    public override void CreateEntitySpec()
    {
      eManager.CreateAimedFall( Data );
    }
    //----------------------------------------------------------------------------------
    public override void ApplyEntitySpec()
    {
      GameObject[] arrGameObjectBody = FieldController.GetUnityGameObjects();
      GameObject[] arrGameObjectAim  = FieldControllerAimGameObjects.GetUnityGameObjects();

      eManager.RecreateAimedFall( Data, arrGameObjectBody, arrGameObjectAim );
    }
    //----------------------------------------------------------------------------------
    public void AddGameObjectsToBodies( UnityEngine.Object[] objects, bool recalculateFields )
    {
      FieldController.AddGameObjects( objects, recalculateFields );
    }
    //----------------------------------------------------------------------------------
    public void AddGameObjectsToAim( UnityEngine.Object[] objects, bool recalculateFields )
    {
      FieldControllerAimGameObjects.AddGameObjects( objects, recalculateFields );
    }
    //----------------------------------------------------------------------------------
    private void DrawSpeedRate()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider("Speed rate", Data.SpeedRate, 0f, 1f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change speed rate - " + Data.Name);
        Data.SpeedRate = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //----------------------------------------------------------------------------------
    private void DrawReleaseThreshold()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider("Release threshold", Data.ReleaseThreshold, 0f, 1f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change release threshold - " + Data.Name);
        Data.ReleaseThreshold = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //----------------------------------------------------------------------------------
    public override void RenderGUI(Rect area, bool isEditable)
    {
      GUILayout.BeginArea(area);

      RenderTitle(isEditable, true, false);

      EditorGUI.BeginDisabledGroup(!isEditable);

      RenderFieldObjects( "Bodies", FieldController,  true, true, CNFieldWindow.Type.extended );
      
      EditorGUILayout.Space();
      EditorGUILayout.Space();

      CRGUIUtils.DrawSeparator();
      CRGUIUtils.Splitter();

      scroller_ = EditorGUILayout.BeginScrollView(scroller_);

      EditorGUILayout.Space();
      EditorGUILayout.Space();

      DrawTimer();
      
      EditorGUILayout.Space();

      DrawSpeedRate();
      DrawReleaseThreshold();
      
      EditorGUILayout.Space();
      EditorGUILayout.Space();

      RenderFieldObjects( "Target objects", FieldControllerAimGameObjects, true, false, CNFieldWindow.Type.normal );

      EditorGUI.EndDisabledGroup();
      
      EditorGUILayout.Space();
      EditorGUILayout.Space();

      EditorGUILayout.EndScrollView();

      GUILayout.EndArea();
    }
  }
}

