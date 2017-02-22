using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CaronteSharp;

namespace CaronteFX
{
  public class CNAttractorEditor : CNEntityEditor
  {
    public static Texture icon_;

    public override Texture TexIcon { get{ return icon_; } }

    protected string[] attractorTypes = new string[] { "FORCE", "ACCELERATION" };

    new CNAttractor Data { get; set; }

    public CNAttractorEditor(CNAttractor data, CommandNodeEditorState state)
      : base( data, state )
    {
      Data = data;
    }

    public override void SceneSelection()
    {
      FieldController.SceneSelectionTopMost();
      FieldController.IsBodyField = true;
    }

    public override void FreeResources()
    {
      FieldController.DestroyField();
      eManager.DestroyEntity( Data );
    }

    public override void CreateEntitySpec()
    {
      eManager.CreateAttractor(Data);
    }

    public override void ApplyEntitySpec()
    { 
      GameObject[] arrGameObject = FieldController.GetUnityGameObjects();
      eManager.RecreateAttractor(Data, arrGameObject, Data.AttractorGameObject);
    }

    private void DrawIsRepulsorVsAttractor()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle("Repulsor", Data.IsRepulsorVsAttractor );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change repulsor - " + Data.Name);
        Data.IsRepulsorVsAttractor = value;
        EditorUtility.SetDirty( Data );
      }
    }

    private void DrawIsForceVsAcceleration()
    {
      int selectedValue = Data.IsForceVsAcceleration ? 0 : 1;
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Popup("Modifier type", selectedValue, attractorTypes );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change repulsor - " + Data.Name);
        Data.IsForceVsAcceleration = (value == 0);
        EditorUtility.SetDirty( Data );
      }
    }

    private void DrawMagnitude()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField("Intensity", Data.ForceOrAcceleration );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change intensity - " + Data.Name);
        Data.ForceOrAcceleration = Mathf.Clamp(value, 0f, float.MaxValue);
        EditorUtility.SetDirty( Data );
      }
    }

    private void DrawRadius()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField("Range", Data.Radius );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change range - " + Data.Name);
        Data.Radius = Mathf.Clamp(value, 0f, float.MaxValue);
        EditorUtility.SetDirty( Data );
      }
    }

    private void DrawDecay()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider("Decay", Data.Decay, 0f, 1f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change decay - " + Data.Name);
        Data.Decay = Mathf.Clamp(value, 0f, float.MaxValue);
        EditorUtility.SetDirty( Data );
      }
    }

    private void DrawAttractorGameObject()
    {
      EditorGUI.BeginChangeCheck();
      var value = ( GameObject)EditorGUILayout.ObjectField("Body GameObject", Data.AttractorGameObject, typeof(GameObject), true );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Body GameObject - " + Data.Name);
        Data.AttractorGameObject = value;
        EditorUtility.SetDirty( Data );
      }
    }

    public override void RenderGUI(Rect area, bool isEditable)
    {
      GUILayout.BeginArea(area);
      
      RenderTitle(isEditable, true, false);

      EditorGUI.BeginDisabledGroup( !isEditable );
      RenderFieldObjects("Bodies", FieldController, true, true, CNFieldWindow.Type.extended);

      EditorGUILayout.Space();
      EditorGUILayout.Space();

      CRGUIUtils.DrawSeparator();
      CRGUIUtils.Splitter();

      EditorGUILayout.Space();
      EditorGUILayout.Space();
      bool currentMode = EditorGUIUtility.wideMode;
      EditorGUIUtility.wideMode = true;
      DrawTimer();
      EditorGUILayout.Space();

      DrawIsRepulsorVsAttractor();
      EditorGUILayout.Space();
      DrawIsForceVsAcceleration();
      DrawMagnitude();
      DrawRadius();
      DrawDecay();
      EditorGUILayout.Space();
      DrawAttractorGameObject();

      EditorGUIUtility.wideMode = currentMode;
      
      EditorGUILayout.Space();
      EditorGUI.EndDisabledGroup();
      GUILayout.EndArea();
    }
  }
}