using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CaronteFX
{
  
  public class CNTriggerByForceEditor : CNTriggerEditor
  {
    public static Texture icon_;
    public override Texture TexIcon { get{ return icon_; } }
    
    CNFieldController FieldControllerA { get; set; }

    protected GUIContent[] triggeringModes_  = new GUIContent[] { new GUIContent("GLOBAL"), new GUIContent("INDIVIDUAL") };

    new CNTriggerByForce Data { get; set; }
    public CNTriggerByForceEditor( CNTriggerByForce data, CommandNodeEditorState state)
      : base( data, state )
    {
      Data = (CNTriggerByForce)data;
    }
    //----------------------------------------------------------------------------------
    public override void Init()
    {
      base.Init();
 
      CNField.AllowedTypes allowedTypes =   CNField.AllowedTypes.Geometry
                                          | CNField.AllowedTypes.RigidBodyNode;

      FieldControllerA = new CNFieldController( Data, Data.FieldA, eManager, goManager );
      FieldControllerA.SetFieldType( allowedTypes );
      FieldControllerA.IsBodyField = true;
    }
    //----------------------------------------------------------------------------------
    public override void LoadInfo()
    {
      FieldController .RestoreFieldInfo();
      FieldControllerA.RestoreFieldInfo();
    }
    //----------------------------------------------------------------------------------
    public override void StoreInfo()
    {
      FieldController .StoreFieldInfo();
      FieldControllerA.StoreFieldInfo();
    }
    //----------------------------------------------------------------------------------
    public override void BuildListItems()
    {
      FieldController .BuildListItems();
      FieldControllerA.BuildListItems();
    }
    //----------------------------------------------------------------------------------
    public override bool RemoveNodeFromFields( CommandNode node )
    {
      Undo.RecordObject(Data, "CaronteFX - Remove node from fields");
      bool removed = Data.Field.RemoveNode(node);
      bool removedFromA = Data.FieldA.RemoveNode(node);
      return ( removed || removedFromA );
    }
    //----------------------------------------------------------------------------------
    public override void SetScopeId(uint scopeId)
    {
      FieldController.SetScopeId(scopeId);
      FieldControllerA.SetScopeId(scopeId);
    }
    //----------------------------------------------------------------------------------
    public override void FreeResources()
    {
      FieldController.DestroyField();
      FieldControllerA.DestroyField();

      eManager.DestroyEntity( Data );
    }
    //-----------------------------------------------------------------------------------
    public void AddGameObjectsToA( UnityEngine.Object[] draggedObjects, bool recalculateFields )
    {
      FieldControllerA.AddGameObjects(draggedObjects, recalculateFields);
    }
    //----------------------------------------------------------------------------------
    public override void CreateEntitySpec()
    {
      eManager.CreateTriggerByForce( Data );
    }
    //----------------------------------------------------------------------------------
    public override void ApplyEntitySpec()
    {
      GameObject[]  arrGameObjectA = FieldControllerA.GetUnityGameObjects();
      CommandNode[] arrCommandNode = FieldController.GetCommandNodes();

      eManager.RecreateTriggerByForce( Data, arrGameObjectA, arrCommandNode );
    }
    //----------------------------------------------------------------------------------
    public void DrawTriggeringMode()
    {
      EditorGUI.BeginChangeCheck();

      int optionIdx = Data.TriggerForInvolvedBodies ? 1 : 0;
      var value = EditorGUILayout.Popup(new GUIContent("Triggering mode"), optionIdx, triggeringModes_ );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject( Data, "Change triggering mode - " + Data.Name);
        Data.TriggerForInvolvedBodies = (value == 1);
        EditorUtility.SetDirty( Data );
      }
    }
    //----------------------------------------------------------------------------------
    public void DrawForceMin()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField( "Pressure force min.", Data.ForceMin );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject( Data, "Change pressure force min. - " + Data.Name);
        Data.ForceMin = Mathf.Clamp(value, 0f, float.MaxValue);
        EditorUtility.SetDirty( Data );
      }
    }
    //----------------------------------------------------------------------------------
    public void DrawIsForcePerMass()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle( "Force per mass unit", Data.IsForcePerMass );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject( Data, "Change force per mass unit - " + Data.Name);
        Data.IsForcePerMass = value;
        EditorUtility.SetDirty( Data );
      }
    }
    //----------------------------------------------------------------------------------
    public override void RenderGUI(Rect area, bool isEditable)
    {
      GUILayout.BeginArea(area);

      RenderTitle(isEditable, true, false);
      scroller_ = EditorGUILayout.BeginScrollView(scroller_);

      EditorGUI.BeginDisabledGroup(!isEditable);

      RenderFieldObjects( "Rigid bodies", FieldControllerA, true, false, CNFieldWindow.Type.extended );

      EditorGUILayout.Space();
      CRGUIUtils.Splitter();
            
      float originalLabelWidth = EditorGUIUtility.labelWidth;
      EditorGUIUtility.labelWidth = 200f;

      EditorGUILayout.Space();
      EditorGUILayout.Space();

      RenderFieldObjects( "Attentive Nodes", FieldController, true, false, CNFieldWindow.Type.extended );
      EditorGUILayout.Space();
      CRGUIUtils.Splitter();
      EditorGUILayout.Space();
      DrawTriggeringMode();
      EditorGUILayout.Space();
      DrawForceMin();
      DrawIsForcePerMass();
      EditorGUI.EndDisabledGroup();
      
      EditorGUILayout.Space();
      EditorGUILayout.Space();

      EditorGUIUtility.labelWidth = originalLabelWidth;

      EditorGUILayout.EndScrollView();

      GUILayout.EndArea();
    }
  }

}
