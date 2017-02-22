using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CaronteFX.Scripting;


namespace CaronteFX
{
  public class CNScriptPlayerEditor : CNMonoFieldEditor
  {
    public static Texture icon_;
    public override Texture TexIcon 
    { 
      get{ return icon_; } 
    }

    System.Type[] simulationScriptTypes_;
    string[]      simulationScriptNames_;
    int           selectedTypeIdx_;

    CarSimulationScript simulationScript_;
    Editor              simulationScriptEditor_;

    new CNScriptPlayer Data { get; set; }

    public CNScriptPlayerEditor( CNScriptPlayer data, CommandNodeEditorState state )
      : base(data, state)
    {
      Data = (CNScriptPlayer)data;

      Assembly editorAssembly = GetEditorAssembly();

      // Get all classes derived from CRSimulationScript
		  simulationScriptTypes_ = ( from t in editorAssembly.GetTypes()
									               where t.IsSubclassOf( typeof(CarSimulationScript) )
		                             select t ).ToArray();

      simulationScriptNames_ = new string[simulationScriptTypes_.Length];

      for (int i = 0; i < simulationScriptTypes_.Length; i++)
      {
        simulationScriptNames_[i] = simulationScriptTypes_[i].Name;
      }  
      
      RefreshSimulationScript();
    }
    //-----------------------------------------------------------------------------------
    public override void Init()
    {
      base.Init();

      FieldController.SetCalculatesDiff(false);
      FieldController.SetFieldType(CNField.AllowedTypes.All);
      FieldController.IsBodyField = false;    
    }
    //-----------------------------------------------------------------------------------
    public void InitSimulationScriptObject()
    {
      if (simulationScript_ != null)
      {
        List<CarObject> listCarObject = new List<CarObject>();

        GameObject[] arrGameObject = FieldController.GetUnityGameObjects();
        List<CarObject> listCarObjectAuxBodies = eManager.GetListCarObjectFromArrGO(arrGameObject);

        listCarObject.AddRange(listCarObjectAuxBodies);

        CommandNode[] arrCommandNode = FieldController.GetCommandNodesNotBody();
        List<CarObject> listCarObjectAuxNodes = eManager.GetListCarObjectFromArrNode(arrCommandNode);

        listCarObject.AddRange(listCarObjectAuxNodes);

        simulationScript_.Init(listCarObject);
      }
    }
    //-----------------------------------------------------------------------------------
    public Scripting.CarSimulationScript GetSimulationScript()
    {
      return simulationScript_;
    }
    //-----------------------------------------------------------------------------------
    private void RefreshSimulationScript()
    {
      if ( Data.SimulationScript != null && simulationScript_ != Data.SimulationScript )
      {
        simulationScript_ = (CarSimulationScript)Data.SimulationScript;
        simulationScriptEditor_ = Editor.CreateEditor(Data.SimulationScript);
      }

      if (Data.SimulationScript == null )
      {
        simulationScript_ = null;
        simulationScriptEditor_ = null;
      }
    }
    //-----------------------------------------------------------------------------------
    public Assembly GetEditorAssembly()
    {
      return Assembly.Load(new AssemblyName("Assembly-CSharp-Editor"));
    }
    //-----------------------------------------------------------------------------------
    public override void RenderGUI(Rect area, bool isEditable)
    {
      GUILayout.BeginArea( area );

      RenderTitle(isEditable);

      EditorGUI.BeginDisabledGroup(!isEditable);
      RenderFieldObjects( "Objects", FieldController, true, true, CNFieldWindow.Type.extended );
      EditorGUI.EndDisabledGroup();
      EditorGUILayout.Space();

      CRGUIUtils.DrawSeparator();

      EditorGUILayout.Space();
      Data.SimulationScript = (CarSimulationScript) EditorGUILayout.ObjectField("Script asset instance", Data.SimulationScript, typeof(CarSimulationScript), true);

      if (Data.SimulationScript == null)
      {
        EditorGUILayout.Space();
        selectedTypeIdx_ = EditorGUILayout.Popup("Script asset type", selectedTypeIdx_, simulationScriptNames_);
        if (GUILayout.Button("Create new script asset"))
		    {
			    var asset = ScriptableObject.CreateInstance( simulationScriptTypes_[selectedTypeIdx_] );

          string path = EditorUtility.SaveFilePanelInProject("CaronteFX - Script Instance", simulationScriptNames_[selectedTypeIdx_], "asset", "" );
          if (path != string.Empty)
          {
            AssetDatabase.CreateAsset( asset, path );

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(); 

            Data.SimulationScript = asset;
            EditorUtility.SetDirty(Data);    
          }
        }
      }


      CRGUIUtils.Splitter();
      scroller_ = EditorGUILayout.BeginScrollView(scroller_);

      EditorGUILayout.Space();
      EditorGUILayout.Space();

      RefreshSimulationScript();

      if ( simulationScriptEditor_ != null && simulationScript_ != null )
      {
        simulationScriptEditor_.DrawDefaultInspector();
      }   

      EditorGUILayout.EndScrollView();
      GUILayout.EndArea();
    }
  }
}

