// ***********************************************************
//	Copyright 2016 Next Limit Technologies, http://www.nextlimit.com
//	All rights reserved.
//
//	THIS SOFTWARE IS PROVIDED 'AS IS' AND WITHOUT ANY EXPRESS OR
//	IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
//	WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
//
// ***********************************************************

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CaronteFX
{

public delegate void RepaintAction();

public class CRManagerEditor : CRWindow<CRManagerEditor>
{
  private static CRManagerEditor window = null;


  [MenuItem("Window/CaronteFX Editor")]
  public static EditorWindow Init()
  {
    if (window == null)
    {
      System.Type gameViewType  = System.Type.GetType("UnityEditor.GameView, UnityEditor");
      System.Type sceneViewType = System.Type.GetType("UnityEditor.SceneView, UnityEditor");

      Type[] dockTypes = new Type[2] { gameViewType, sceneViewType };
      window = EditorWindow.GetWindow<CRManagerEditor>("CaronteFX Ed", true, dockTypes);
    }

    return window;
  }

  // Declare the event to which editor code will hook itself.
  public event RepaintAction WantRepaint;
    
  public CRListBox NodesListBox
  {
    get;
    set;
  }

  IView paramsView_ = null;
  bool  deferredContextClick_ = false;
 
  //---------------------------------------------------------------------------------- 
  private float min_width_nodeManager_          = 150f;
  private float min_height_nodeManager_         = 180f;
  private float min_width_nodeParams_           = 545f;
  private float height_simulation_controls_     = 70f;


  private Rect menusRect_ = new Rect(0f, 20f, 1000f, 50f);
  private Rect nodeManager_area_;
  private Rect nodeParams_area_;
  private Rect nodeParams_box_;
  private Rect simControlsRect_;

  private Vector2 scrollerWindow_;
  private bool firstOnGUI_;

  [SerializeField]
  private bool simulatingBeginFirst_ = false;
  public bool SimulatingBeginFirst
  {
    get { return simulatingBeginFirst_; }
    set { simulatingBeginFirst_ = value; }
  }

  public Caronte_Fx FxData
  {
    get { return Controller.FxData; }
  }
  //----------------------------------------------------------------------------------
  public CNManager controller_ = null;
  public CNManager Controller 
  { 
    get { return controller_; }
  }

  public enum Tab
  {
    General        = 0,
    Bodies         = 1,
    Joints         = 2,
    Fractures      = 3,
    Tools          = 4,
    MotorsServos   = 5,
    Daemons        = 6,
    Triggers       = 7,
    Actions        = 8,
    Size           = 9
  }

  //Editor resources
  private bool resourcesLoaded = false;

  Tab   selectedTab_ = Tab.Bodies;
  string[] tabNames_ = new string[] { "General", "Bodies", "Joints", "Fracturers", "Tools", "Motors & Servos", "Daemons", "Triggers", "Actions" };

  public static Texture ic_logoCaronte_;

  Texture ic_duplicate_;
  Texture ic_group_;

  Texture ic_parameter_modifier_;
  Texture ic_trigger_byTimer_;
  Texture ic_trigger_byContact_;
  Texture ic_trigger_byDetector_;
  Texture ic_trigger_byExplosion_;
  Texture ic_trigger_byForce_;
  Texture ic_substituter_;

  Texture ic_rigidbodies_;
  Texture ic_irresponsivebodies_;
  Texture ic_animatedbodies_;
  Texture ic_softbodies_;
  Texture ic_cloths_;
  Texture ic_ropes_;

  Texture ic_rigid_glue_;
  Texture ic_multijoint_area_;
  Texture ic_multijoint_vertices_;
  Texture ic_multijoint_leaves_;
  Texture ic_multijoint_locators_;

  Texture ic_motors_linear_;
  Texture ic_motors_angular_;
  Texture ic_servos_linear_;
  Texture ic_servos_angular_;

  Texture ic_fractureuniform_;
  Texture ic_fracturegeometry_;
  Texture ic_fractureradial_;
  Texture ic_welder_;
  Texture ic_tessellator_;
  Texture ic_procedural_;
  Texture ic_selector_;
  Texture ic_materialsubstituter_;
  Texture ic_balltreeCreator_;

  Texture ic_gravity_;
  Texture ic_explosion_;
  Texture ic_wind_;
  Texture ic_aimed_force_;
  Texture ic_speed_limiter_;
  Texture ic_attractor_;
  Texture ic_jet_;

  Texture ic_contact_emitter_;
  Texture ic_script_player_;

  Texture ic_gameobject_;
  Texture ic_nameselector_;

  Texture ic_first_; 
  Texture ic_last_;   
  Texture ic_prev_;   
  Texture ic_next_;   
  Texture ic_play_;   
  Texture ic_pause_;  
  Texture ic_stop_; 
  Texture ic_loop_;

  Texture ic_rec_;

  //-----------------------------------------------------------------------------------
  void OnEnable()
  {
    InitEditor();
   
    try 
    {
      controller_ = CNManager.Instance;
    }
    catch
    {
      return;
    }

    if (controller_ != null)
    {
      controller_.SetManagerEditor(this);
    }

    titleContent             = new GUIContent("Caronte FX");
    
    wantsMouseMove           = true;
    resourcesLoaded          = false;
    autoRepaintOnSceneChange = true;
    firstOnGUI_              = true;

    CRStringManager.Init();
  }
  //-----------------------------------------------------------------------------------
  void OnDisable()
  {
    controller_ = null;
    RepaintSubscribers();
    SceneView.RepaintAll();
  }
  //-----------------------------------------------------------------------------------
  void InitEditor()
  {
    Instance = this;
    GameObject activeGameObject = Selection.activeGameObject;
    Selection.activeGameObject  = null;
    Selection.activeGameObject  = activeGameObject;
  }
  //-----------------------------------------------------------------------------------
  public void RepaintSubscribers()
  {
    Repaint();
    if (WantRepaint != null)
    {
      WantRepaint();
    }  
  }
  //-----------------------------------------------------------------------------------
  void OnLostFocus()
  {
    if (Controller != null && NodesListBox != null )
    {
      NodesListBox.Focused = false;
    }
  }
  //-----------------------------------------------------------------------------------
  void OnFocus()
  {
    if (NodesListBox != null)
      NodesListBox.Focused = true;
  }
  //-----------------------------------------------------------------------------------
  void OnGUI()
  {
    ShowNotifications();

    if (EditorApplication.isPlayingOrWillChangePlaymode)
    {
      return;
    }
    LoadFX();
    if (Controller == null || FxData == null || !Controller.IsInited)
    {
      return;
    }

    LoadEditorResources();

    Color currentColor = GUI.color;
    DrawToolStrip();

    scrollerWindow_ = EditorGUILayout.BeginScrollView(scrollerWindow_);
    DrawIconList();

    if (firstOnGUI_)
    {
      Repaint();
      firstOnGUI_ = false;
    }

    Event e = Event.current;
    if(e.type == EventType.Repaint)
    {
      menusRect_ = GUILayoutUtility.GetLastRect();
    }

    CalculateWindowDimensions();
    NodesListBox.RenderGUI(nodeManager_area_); 
    DrawSimulationControls(simControlsRect_, Controller.BlockEdition);

    GUI.Box(nodeParams_box_, "");
    DrawParamsView();
    
    EditorGUILayout.EndScrollView();
    GUI.color = currentColor;

    ProcessActions(e);
  }
  //----------------------------------------------------------------------------------
  private void ProcessActions(Event e)
  {
    if (e.type == EventType.Repaint)
    {
      Controller.DoDeferredActions();
      if (deferredContextClick_)
      {
        Controller.ContextClickExternal();
        deferredContextClick_ = false;
        Repaint();
      }

      if (Controller != null)
      {
        IView newView = Controller.GetFocusedNodeView();
        if (paramsView_ != newView)
        {
          paramsView_ = newView;
          Repaint();
        }    
      }
    }
  }
  //-----------------------------------------------------------------------------------
  void ShowNotifications()
  {
    if (Controller == null)
    {
      this.ShowNotification(new GUIContent("There was a problem loading CaronteFX dll.\n\n Please close this window and restart Unity. After that, reimport the CaronteFX package.\n\n If the problem persists contact Next Limit support."));
      return;
    }

    if ( EditorApplication.isPlaying )
    {
      this.ShowNotification( new GUIContent("Play Mode - (To work with CaronteFX exit Unity play mode)") );
      return;
    }

    if ( EditorApplication.isPlayingOrWillChangePlaymode )
    {
      this.ShowNotification( new GUIContent("Entering Play Mode") );
      return;
    }

    if ( EditorApplication.isCompiling )
    {
      this.ShowNotification( new GUIContent("Code Compiling") );
      return;
    }

    if (!IsFocused && (Controller == null || !Controller.IsInited || FxData == null ) )
    {
      this.ShowNotification(new GUIContent("To start using CaronteFX focus its editor window (click on this window)"));
      return;
    }
  }
  //-----------------------------------------------------------------------------------
  private void LoadFX()
  {
    if ( IsFocused && Controller != null && FxData == null )
    {
      Controller.LoadEffectsArray();
      if ( FxData == null )
      {
        DrawGUINoFxDataInScene();
        return;
      }
    }
  }
  //-----------------------------------------------------------------------------------
  private void CalculateWindowDimensions()
  {
    float width  = position.width;
    float height = position.height;

    float width_nodeManager  = Mathf.Max(width * 0.25f, min_width_nodeManager_);
    float height_nodeManager = Mathf.Max(height - height_simulation_controls_ - menusRect_.yMax - 50f, min_height_nodeManager_);

    float width_nodeParams  = Mathf.Max( (position.width * 0.75f) - 30f, min_width_nodeParams_ );
    float height_nodeParams = height_nodeManager + height_simulation_controls_ + 10f;

    float contentWidth  = width_nodeManager + width_nodeParams + 30f;
    float contentHeight = height_nodeParams + 15f;

    if (contentWidth > width)
    {
      height_nodeManager -= 15f;
      height_nodeParams  -= 15f;
      contentHeight      -= 15f;
    }
    if (contentHeight > (height - menusRect_.height - 40f) )
    {
      width_nodeParams -= 15f;
      contentWidth     -= 15f;
    }

    simControlsRect_  = new Rect(10f, menusRect_.yMax + 9f, width_nodeManager, height_simulation_controls_);
    nodeManager_area_ = new Rect(10f, simControlsRect_.yMax + 10f, width_nodeManager, height_nodeManager + 1f);

    nodeParams_area_  = new Rect(Mathf.Ceil(nodeManager_area_.xMax + 10f), Mathf.Ceil(menusRect_.yMax + 10f), Mathf.Ceil(width_nodeParams), Mathf.Ceil(height_nodeParams) );
    nodeParams_box_   = new Rect(nodeParams_area_.xMin - 1f, nodeParams_area_.yMin - 1f, nodeParams_area_.width + 2f, nodeParams_area_.height + 2f);

    GUILayoutUtility.GetRect(contentWidth, contentHeight);
  }
  //-----------------------------------------------------------------------------------
  private void DrawParamsView()
  {
    if ( paramsView_ != null && Controller.IsInited )   
    {
      paramsView_.RenderGUI( nodeParams_area_, !Controller.BlockEdition );
    }
    else
    {
      GUI.DrawTexture(nodeParams_area_, ic_logoCaronte_, ScaleMode.ScaleToFit );
    }  
  }
  //-----------------------------------------------------------------------------------
  private void DrawGUINoFxDataInScene()
  {
    EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
    Rect fileButtonRect = GUILayoutUtility.GetRect(new GUIContent("FX GameObject"), EditorStyles.toolbarDropDown);
    if ( GUI.Button(fileButtonRect, "FX GameObject", EditorStyles.toolbarDropDown) )
    {
      GenericMenu fileMenu = new GenericMenu();
      fileMenu.AddItem(new GUIContent("New FX"), false, Controller.CreateNewFx);
      fileMenu.DropDown(new Rect(fileButtonRect.xMin, 0, 16, 16));
    }  
    GUILayout.FlexibleSpace();
    EditorGUILayout.EndHorizontal();
    GUILayout.Space(10f);
    EditorGUILayout.BeginHorizontal();
    GUILayout.Space(10f);
    EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) );
    GUILayout.Label("To create an effect: \n\n\n- Menu/FX GameObject -> New FX\n\n                or\n\n- Click on Create FX GameObject", EditorStyles.boldLabel);
    GUILayout.EndVertical();
    GUILayout.Space(10f);
    EditorGUILayout.BeginVertical();
    MessageBox("There is not any FX GameObject in the scene. To create a new one you have two options:\n\n - Menu/FX GameObject -> New FX \n\n - Click on create FX GameObject button.", MessageBoxType.Info);
    GUILayout.Space(10f);
    GUIStyle boldButton = new GUIStyle(GUI.skin.button);
    boldButton.fontStyle = FontStyle.Bold;
    if (GUILayout.Button("Create FX GameObject", boldButton, GUILayout.Height(30f) ) )
    {
      Controller.CreateNewFx();
    }
    EditorGUILayout.EndVertical();
    GUILayout.Space(10f);
    EditorGUILayout.EndHorizontal();
    GUILayout.Space(10f);
  }
  //-----------------------------------------------------------------------------------
  void DrawToolStrip()
  {
    GUILayout.BeginHorizontal( EditorStyles.toolbar);

    #region FX GameObject
    Rect fxButtonRect = GUILayoutUtility.GetRect(new GUIContent("FX GameObject"), EditorStyles.toolbarDropDown);
    if (GUI.Button(fxButtonRect, "FX GameObject", EditorStyles.toolbarDropDown))
    {
      GenericMenu fxMenu = new GenericMenu();
      if ( Controller.BlockEdition )
      {
        fxMenu.AddDisabledItem(new GUIContent("New FX") );
        fxMenu.AddDisabledItem(new GUIContent("Subeffects menu... (experimental)") );
        fxMenu.AddSeparator("");
        fxMenu.AddDisabledItem(new GUIContent("Select FX in Hierarchy") );
        fxMenu.AddSeparator("");
        fxMenu.AddDisabledItem(new GUIContent("Quit edition"));
      }
      else
      {   
        fxMenu.AddItem(new GUIContent("New FX"), false, Controller.CreateNewFx);
        fxMenu.AddItem(new GUIContent("Subeffects menu... (experimental)"), false, () => { CRAddFXMenu.ShowWindow(Controller); } );
        fxMenu.AddSeparator("");
        fxMenu.AddItem(new GUIContent("Select FX in Hierarchy"), false, () =>
        {
          EditorGUIUtility.PingObject(FxData.gameObject);
          Selection.activeGameObject = FxData.gameObject;
        });
        fxMenu.AddSeparator("");
        fxMenu.AddItem(new GUIContent("Quit Edition"), false, () =>
        {
          Controller.Deinit();
          Controller.FxData = null;
          Close();
        });
        
      }
      fxMenu.DropDown(new Rect(fxButtonRect.xMin, 0, 16, 16));
    }
    #endregion

    #region Scene FXs
    Event         evCurrent = Event.current;
    EventType evCurrentType = evCurrent.type;

    string longestName;
    string[] effectNames = Controller.GetFxNames(out longestName);

    GUIStyle fxRectStyle = EditorStyles.toolbarPopup;
    fxRectStyle.fontStyle = FontStyle.Bold;

    Rect effectRect = GUILayoutUtility.GetRect(new GUIContent(longestName), fxRectStyle);
    if ( ( evCurrentType == EventType.MouseDown && effectRect.Contains(evCurrent.mousePosition) ) ||
         ( Controller.CurrentFxIdx > (effectNames.Length - 1) ) )
    {
      Controller.RefreshEffectsArray();
      effectNames = Controller.GetFxNames(out longestName);
      Repaint();
    }

    EditorGUI.BeginDisabledGroup(Controller.BlockEdition);
    int fxIdx = EditorGUI.Popup(effectRect, Controller.CurrentFxIdx, effectNames, fxRectStyle);
    EditorGUI.EndDisabledGroup();
    if (fxIdx != Controller.CurrentFxIdx)
    {
      Controller.ChangeToFx(fxIdx);
    }
    #endregion

    #region SceneGUIOverlay
    Rect overlayRect = GUILayoutUtility.GetRect( new GUIContent("SceneGUI overlay"), EditorStyles.toolbarButton);
    EditorGUI.BeginChangeCheck();
    FxData.ShowOverlay = GUI.Toggle(overlayRect, FxData.ShowOverlay,new GUIContent("SceneGUI overlay"), EditorStyles.toolbarButton);
    if (EditorGUI.EndChangeCheck() )
    {
      EditorUtility.SetDirty(FxData);
    }
    #endregion

    #region Determinism toggle
    Rect determinsmToggleRect = GUILayoutUtility.GetRect(new GUIContent("Rebuild on simulate"), EditorStyles.toolbarButton);
    FxData.DeterministicMode = GUI.Toggle(determinsmToggleRect, FxData.DeterministicMode, new GUIContent("Rebuild on simulate"), EditorStyles.toolbarButton);
    #endregion

    GUILayout.FlexibleSpace();

    #region Help Menu
    Rect helpButtonRect = GUILayoutUtility.GetRect(new GUIContent("Help"), EditorStyles.toolbarDropDown);
    if (GUI.Button(helpButtonRect, "Help", EditorStyles.toolbarDropDown))
    {
      GenericMenu helpMenu = new GenericMenu();

      helpMenu.AddItem(new GUIContent("About CaronteFX..."), false, ShowAboutCaronteFXWindow);
      helpMenu.AddSeparator("");
      helpMenu.AddItem(new GUIContent("Reset CaronteFX"), false, Controller.ResetSimulation );
      helpMenu.DropDown(new Rect(helpButtonRect.x, 0, 16, 16));
    }
    #endregion

    GUILayout.EndHorizontal();  
  }
  //-----------------------------------------------------------------------------------
  private void ShowAboutCaronteFXWindow()
  {
    CRAboutWindow.ShowWindow();
  }
  //-----------------------------------------------------------------------------------
  private void DrawIconList()
  {
    EditorGUILayout.Space();
    EditorGUILayout.BeginHorizontal();
    GUILayout.Space(10f);
    
    GUIStyle styleTabButton = new GUIStyle(EditorStyles.toolbarButton);
    styleTabButton.fontSize    = 10;
    styleTabButton.fixedHeight = 14f;
    styleTabButton.onNormal.background  = styleTabButton.onActive.background;

    selectedTab_ = (Tab) GUILayout.SelectionGrid( (int)selectedTab_, tabNames_, 5, styleTabButton );
    
    GUILayout.Space(10f);
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();

    GUILayout.Space(10f);

    GUIStyle styleIconButton = new GUIStyle(EditorStyles.toolbarButton);
    styleIconButton.fixedHeight = 50f;
    styleIconButton.fixedWidth  = 85f;

    styleIconButton.margin        = new RectOffset( 0, 0, 0, 0 );
    styleIconButton.border        = new RectOffset( 5, 5, 5, 5 );
    styleIconButton.imagePosition = ImagePosition.ImageAbove;
    styleIconButton.padding       = new RectOffset( 0, 0, 5, 15);
      
    switch (selectedTab_)
    {
      case Tab.General:
        //DrawItemIcon( new GUIContent( "Enable/Disable", ic_enabledisable_, "Enable/Disable Node"), styleIconButton, 
        //  () => {  } );
        DrawIconButton( new GUIContent( CRStringManager.GetString("Duplicate"), ic_duplicate_, CRStringManager.GetString("DuplicateTooltip") ), styleIconButton, 
          () => { Controller.DuplicateSelection(); } );
        GUILayout.Space(10f);
        DrawIconButton( new GUIContent( CRStringManager.GetString("Group"), ic_group_, CRStringManager.GetString("GroupTooltip") ), styleIconButton, 
          () => { Controller.CreateNodeUnique<CNGroup>("Group"); } );
        GUILayout.Space(10f);
        break;

      case Tab.Bodies:
         DrawIconButton( new GUIContent( CRStringManager.GetString("Rigid"), ic_rigidbodies_, CRStringManager.GetString("RigidTooltip") ), styleIconButton,               
           () => { Controller.CreateNodeUnique<CNRigidbody>("RigidBodies"); } );
         DrawIconButton( new GUIContent( CRStringManager.GetString("Irresponsive"), ic_irresponsivebodies_, CRStringManager.GetString("IrresponsiveTooltip") ), styleIconButton, 
           () => { Controller.CreateIrresponsiveBodiesNode(); } );
         DrawIconButton( new GUIContent( CRStringManager.GetString("Animated"), ic_animatedbodies_, CRStringManager.GetString("AnimatedTooltip") ), styleIconButton,         
           () => { Controller.CreateNodeUnique<CNAnimatedbody>("AnimatedBodies"); } );
         GUILayout.Space(20f);
         DrawIconButton( new GUIContent( CRStringManager.GetString("Soft"), ic_softbodies_, CRStringManager.GetString("SoftTooltip") ), styleIconButton,                   
           () => { Controller.CreateNodeUnique<CNSoftbody>("SoftBodies"); } );
         DrawIconButton( new GUIContent( CRStringManager.GetString("Cloth"), ic_cloths_, CRStringManager.GetString("ClothTooltip") ), styleIconButton,                   
           () => { Controller.CreateClothBodiesNode(); } );
         DrawIconButton( new GUIContent( CRStringManager.GetString("Rope"), ic_ropes_, CRStringManager.GetString("RopeTooltip") ), styleIconButton,                   
           () => { Controller.CreateRopeBodiesNode();  } );
        break;

      case Tab.Joints:
         DrawIconButton( new GUIContent( CRStringManager.GetString("RigidGlue"), ic_rigid_glue_, CRStringManager.GetString("RigidGlueTooltip") ), styleIconButton,             
           () => { Controller.CreateRigidglueNode(); } ); 
         GUILayout.Space(10f);
         DrawIconButton( new GUIContent( CRStringManager.GetString("ByCloseAreaJoints"), ic_multijoint_area_, CRStringManager.GetString("ByCloseAreaJointsTooltip") ), styleIconButton,             
           () => { Controller.CreateMultiJointNodeArea(); } ); 
         DrawIconButton( new GUIContent( CRStringManager.GetString("ByCloseVerticesJoints"), ic_multijoint_vertices_, CRStringManager.GetString("ByCloseVerticesJointsTooltip") ), styleIconButton,             
           () => { Controller.CreateMultiJointNodeVertices(); } );
         DrawIconButton( new GUIContent( CRStringManager.GetString("ByLeavesJoints"), ic_multijoint_leaves_, CRStringManager.GetString("ByLeavesJointsTooltip") ), styleIconButton,
           () => { Controller.CreateMultiJointNodeLeaves(); } );
         DrawIconButton( new GUIContent( CRStringManager.GetString("ByLocatorsJoints"), ic_multijoint_locators_, CRStringManager.GetString("ByLocatorsJointsTooltip") ), styleIconButton,
           () => { Controller.CreateMultiJointNodeLocators(); } );
          break;

      case Tab.Fractures:
         DrawIconButton(new GUIContent( CRStringManager.GetString("FracturerUniform"), ic_fractureuniform_, CRStringManager.GetString("FracturerUniformTooltip") ), styleIconButton, 
           () => { Controller.CreateFracturerNodeUniform(); } );
         GUILayout.Space(20f);
         DrawIconButton(new GUIContent( CRStringManager.GetString("FracturerByGeometry"), ic_fracturegeometry_, CRStringManager.GetString("FracturerByGeometryTooltip") ), styleIconButton,
           () => { Controller.CreateFracturerNodeGeometry(); } );
         GUILayout.Space(20f);
         DrawIconButton(new GUIContent( CRStringManager.GetString("FracturerRadial"), ic_fractureradial_, CRStringManager.GetString("FracturerRadialTooltip") ), styleIconButton,
           () => { Controller.CreateFracturerNodeRadial(); } );
         GUILayout.Space(20f);
        break;

      case Tab.Tools:
         DrawIconButton(new GUIContent( CRStringManager.GetString("Welder"), ic_welder_, CRStringManager.GetString("WelderTooltip") ), styleIconButton,
           () => { Controller.CreateNodeUnique<CNWelder>("Welder"); } );
         DrawIconButton(new GUIContent( CRStringManager.GetString("Selector"), ic_selector_, CRStringManager.GetString("SelectorTooltip") ), styleIconButton,
           () => { Controller.CreateNodeUnique<CNSelector>("Selector"); } );
         GUILayout.Space(10f);
         DrawIconButton(new GUIContent( CRStringManager.GetString("Tessellator"), ic_tessellator_, CRStringManager.GetString("TessellatorTooltip") ), styleIconButton,
           () => { Controller.CreateNodeUnique<CNTessellator>("Tessellator"); } );
         DrawIconButton(new GUIContent( CRStringManager.GetString("HelperMesh"), ic_procedural_, CRStringManager.GetString("HelperMeshTooltip") ), styleIconButton,
           () => { Controller.CreateNodeUnique<CNHelperMesh>("Helper Mesh"); } );
         GUILayout.Space(10f);
         DrawIconButton(new GUIContent( CRStringManager.GetString("MaterialSubstituter"), ic_materialsubstituter_, CRStringManager.GetString("MaterialSubstituterTooltip") ), styleIconButton,
           () => {  
                    if (!CRMaterialSubstituterWindow.IsOpen)
                    {
                      CRMaterialSubstituterWindow matSubMenu = ScriptableObject.CreateInstance<CRMaterialSubstituterWindow>();
                      matSubMenu.titleContent = new GUIContent("CaronteFX - Material Substituter");
                    }
                    CRMaterialSubstituterWindow.Instance.ShowUtility();
                 } );
         GUILayout.Space(10f);
         if (CRVersionChecker.IsPremiumVersion())
         {
           DrawIconButton(new GUIContent( CRStringManager.GetString("BalltreeGenerator"), ic_balltreeCreator_, CRStringManager.GetString("BalltreeGeneratorTooltip") ), styleIconButton,
           () => {  Controller.CreateNodeUnique<CNBalltreeGenerator>("BalltreeGenerator"); } );
         }
         break;

      case Tab.MotorsServos:
         DrawIconButton( new GUIContent( CRStringManager.GetString("MotorsLinear"), ic_motors_linear_, CRStringManager.GetString("MotorsLinearTooltip")), styleIconButton,             
           () => { Controller.CreateMotorsLinearNode(); } ); 
         DrawIconButton( new GUIContent( CRStringManager.GetString("MotorsAngular"), ic_motors_angular_, CRStringManager.GetString("MotorsAngularTooltip")), styleIconButton,             
           () => { Controller.CreateMotorsAngularNode(); } );
         GUILayout.Space(10f);
         DrawIconButton( new GUIContent( CRStringManager.GetString("ServosLinear"), ic_servos_linear_,  CRStringManager.GetString("ServosLinearTooltip")), styleIconButton,
           () => { Controller.CreateServosLinearNode(); } );
         DrawIconButton( new GUIContent( CRStringManager.GetString("ServosAngular"), ic_servos_angular_, CRStringManager.GetString("ServosAngularTooltip")), styleIconButton,
           () => { Controller.CreateServosAngularNode(); } );
          break;

      case Tab.Daemons:
         DrawIconButton(new GUIContent( CRStringManager.GetString("DaemonGravity"), ic_gravity_, CRStringManager.GetString("DaemonGravityTooltip")), styleIconButton,
           () => { Controller.CreateNodeUnique<CNGravity>("Gravity"); } );
         DrawIconButton(new GUIContent( CRStringManager.GetString("DaemonExplosion"), ic_explosion_, CRStringManager.GetString("DaemonExplosionTooltip")), styleIconButton,
           () => { Controller.CreateExplosionNode(); } );
         DrawIconButton(new GUIContent( CRStringManager.GetString("DaemonWind"), ic_wind_, CRStringManager.GetString("DaemonWindTooltip")), styleIconButton,
           () => { Controller.CreateNodeUnique<CNWind>("Wind"); } );
         DrawIconButton(new GUIContent( CRStringManager.GetString("DaemonJet"), ic_jet_, CRStringManager.GetString("DaemonJetTooltip")), styleIconButton,
           () => { Controller.CreateNodeUnique<CNJet>("Jet"); } );
         DrawIconButton(new GUIContent( CRStringManager.GetString("DaemonSpeedLimiter"), ic_speed_limiter_, CRStringManager.GetString("DaemonSpeedLimiterTooltip") ), styleIconButton,
           () => { Controller.CreateNodeUnique<CNSpeedLimiter>("SpeedLimiter"); } );
         DrawIconButton(new GUIContent( CRStringManager.GetString("DaemonAttractor"), ic_attractor_, CRStringManager.GetString("DaemonAttractorTooltip") ), styleIconButton,
           () => { Controller.CreateNodeUnique<CNAttractor>("Attractor"); } );
         if (CRVersionChecker.IsPremiumVersion())
         {
           DrawIconButton(new GUIContent( CRStringManager.GetString("DaemonAimedFall"), ic_aimed_force_, CRStringManager.GetString("DaemonAimedFallTooltip") ), styleIconButton,
             () => { Controller.CreateNodeUnique<CNAimedFall>("AimedFall"); } );
         }
         break;

      case Tab.Triggers:
        DrawIconButton(new GUIContent( CRStringManager.GetString("TriggerByTime"), ic_trigger_byTimer_, CRStringManager.GetString("TriggerByTimeTooltip") ), styleIconButton,
          () => { Controller.CreateNodeUnique<CNTriggerByTime>("TriggerByTime"); } );
        GUILayout.Space(10f);
        DrawIconButton(new GUIContent( CRStringManager.GetString("TriggerByContact"), ic_trigger_byContact_, CRStringManager.GetString("TriggerByContactTooltip") ), styleIconButton,
          () => { Controller.CreateNodeUnique<CNTriggerByContact>("TriggerByContact"); } );
        DrawIconButton(new GUIContent( CRStringManager.GetString("TriggerByDetector"), ic_trigger_byDetector_, CRStringManager.GetString("TriggerByDetectorTooltip") ), styleIconButton,
          () => { Controller.CreateNodeUnique<CNTriggerByDetector>("TriggerByDetector"); } );
        if ( CRVersionChecker.IsPremiumVersion() )
        {
          DrawIconButton(new GUIContent( CRStringManager.GetString("TriggerByForce"), ic_trigger_byForce_, CRStringManager.GetString("TriggerByForceTooltip") ), styleIconButton,
          () => { Controller.CreateNodeUnique<CNTriggerByForce>("TriggerByPressureF"); } );
        }
        GUILayout.Space(10f);
        DrawIconButton(new GUIContent( CRStringManager.GetString("TriggerByExplosion"), ic_trigger_byExplosion_, CRStringManager.GetString("TriggerByExplosionTooltip") ), styleIconButton,
          () => { Controller.CreateNodeUnique<CNTriggerByExplosion>("TriggerByExplosion"); } );
        break;

      case Tab.Actions:
        DrawIconButton(new GUIContent( CRStringManager.GetString("ParameterModifier"), ic_parameter_modifier_, CRStringManager.GetString("ParameterModifierTooltip") ), styleIconButton,
          () => { Controller.CreateNodeUnique<CNParameterModifier>("Modifier"); });
        DrawIconButton(new GUIContent( CRStringManager.GetString("Substituter"), ic_substituter_, CRStringManager.GetString("SubstituterTooltip") ), styleIconButton,
          () => { Controller.CreateNodeUnique<CNSubstituter>("Substituter"); } );
        DrawIconButton(new GUIContent( CRStringManager.GetString("ContactEmitter"), ic_contact_emitter_, CRStringManager.GetString("ContactEmitterTooltip") ), styleIconButton,
           () => { Controller.CreateNodeUnique<CNContactEmitter>("ContactEmitter"); } );
        if (CRVersionChecker.IsPremiumVersion())
        {
          DrawIconButton(new GUIContent( CRStringManager.GetString("ScriptPlayer"), ic_script_player_, CRStringManager.GetString("ScriptPlayerTooltip") ), styleIconButton,
             () => { Controller.CreateNodeUnique<CNScriptPlayer>("ScriptPlayer"); } );
        }
        break;

      default:
        throw new NotImplementedException();
    }
    GUILayout.Space(10f);
    EditorGUILayout.EndHorizontal();
  }
  //-----------------------------------------------------------------------------------
  private void DrawIconButton( GUIContent content, GUIStyle styleIconButton, Action pressedAction )
  { 
    EditorGUI.BeginDisabledGroup(Controller.BlockEdition);
    if (GUILayout.Button(new GUIContent(content.image, content.tooltip), styleIconButton))
    {
      pressedAction();
    }

    Rect bRect = GUILayoutUtility.GetLastRect();
    Rect lRect  = new Rect( bRect.xMin, bRect.yMax - 17f, bRect.width, 20f );
    
    GUIStyle styleLabel = new GUIStyle(EditorStyles.miniLabel);
    styleLabel.alignment = TextAnchor.MiddleCenter;
    styleLabel.clipping  = TextClipping.Clip;
    String text;
    
    if (content.text.Length < 14 )
    {
      text = content.text;
    }
    else
    {
      text = content.text.Substring(0, 10) + "...";
    }
    GUI.Label(lRect, text, styleLabel );
    EditorGUI.EndDisabledGroup();
  }
  //-----------------------------------------------------------------------------------
  private void DrawSimulationControls( Rect simControlsRect, bool isSimulatingOrReplaying )
  {
    GUILayout.BeginArea(simControlsRect, GUI.skin.box);
    GUILayout.Space(4f);
    GUIStyle styleTitle = new GUIStyle(GUI.skin.label);
    styleTitle.fontSize = 13;
    GUILayout.BeginHorizontal();
    GUILayout.Label("Mode: ", styleTitle, GUILayout.Width(45f));

    string statusString = Controller.Player.GetStatusString();
    styleTitle.fontStyle = FontStyle.Bold;
    styleTitle.alignment = TextAnchor.MiddleLeft;
    GUILayout.Label(statusString, styleTitle);
    GUILayout.EndHorizontal();

    GUILayout.Space(4);

    GUIStyle styleButton = new GUIStyle(GUI.skin.button);
    styleButton.fontSize = 13;

    GUILayout.BeginHorizontal();
    if (!isSimulatingOrReplaying)
    {
      EditorGUI.BeginDisabledGroup(!Controller.IsInited);
      if (GUILayout.Button("Simulate", styleButton, GUILayout.Height(28f)) )
      {
        Controller.StartSimulating();
      }
      EditorGUI.EndDisabledGroup();
    }
    else
    {
      EditorGUI.BeginDisabledGroup(!Controller.IsInited || Controller.Player.StopRequested);
      if (GUILayout.Button("Focus Player", styleButton, GUILayout.Height(28f)) )
      {
        Controller.ShowPlayer();
      }
      EditorGUI.EndDisabledGroup();
    }
    GUILayout.EndHorizontal();

    EditorGUILayout.Space();
    GUILayout.EndArea();
  }
  //----------------------------------------------------------------------------------
  public void SetDeferredContextClick()
  {
    deferredContextClick_ = true;
  }
  //-----------------------------------------------------------------------------------
  void LoadEditorResources()
  {
    if ( !resourcesLoaded )
    {
      bool isUnityFree   = !UnityEditorInternal.InternalEditorUtility.HasPro();
      bool isCaronteFree = CRVersionChecker.IsFreeVersion();

      if ( isCaronteFree && isUnityFree )
      {
        ic_logoCaronte_ = CREditorResource.LoadEditorTexture("cr_logo_carontefxfree_unityfree");
      }
      else if ( isCaronteFree && !isUnityFree )
      {
        ic_logoCaronte_ = CREditorResource.LoadEditorTexture("cr_logo_carontefxfree_unitypro");
      }
      else if ( !isCaronteFree && isUnityFree )
      {
        ic_logoCaronte_ = CREditorResource.LoadEditorTexture("cr_logo_carontefxpro_unityfree");
      }
      else
      {
        ic_logoCaronte_ = CREditorResource.LoadEditorTexture("cr_logo_carontefxpro_unitypro");
      }

      LoadNodeResources();

      CarAnimatorInfo.animatorSampler_ = CREditorResource.LoadEditorAnimationController("cr_anim_sampler");

      resourcesLoaded = true;
    }
  }
  //-----------------------------------------------------------------------------------
  void LoadNodeResources()
  {
    ic_duplicate_ = CREditorResource.LoadEditorTexture("cr_icon_duplicate");
    ic_group_     = CREditorResource.LoadEditorTexture("cr_icon_group");

    ic_trigger_byTimer_     = CREditorResource.LoadEditorTexture("cr_icon_trigger_bytime");
    ic_trigger_byContact_   = CREditorResource.LoadEditorTexture("cr_icon_trigger_bycontact");
    ic_trigger_byDetector_  = CREditorResource.LoadEditorTexture("cr_icon_trigger_bydetector");
    ic_trigger_byForce_     = CREditorResource.LoadEditorTexture("cr_icon_trigger_byforce");
    ic_trigger_byExplosion_ = CREditorResource.LoadEditorTexture("cr_icon_trigger_byexplosion");

    ic_parameter_modifier_ = CREditorResource.LoadEditorTexture("cr_icon_parameter_modifier");
    ic_substituter_        = CREditorResource.LoadEditorTexture("cr_icon_substituter");

    ic_rigidbodies_         = CREditorResource.LoadEditorTexture("cr_icon_rigidbody");
    ic_irresponsivebodies_  = CREditorResource.LoadEditorTexture("cr_icon_irresponsive");
    ic_animatedbodies_      = CREditorResource.LoadEditorTexture("cr_icon_animated");
    ic_softbodies_          = CREditorResource.LoadEditorTexture("cr_icon_softbody");
    ic_cloths_              = CREditorResource.LoadEditorTexture("cr_icon_cloth");
    ic_ropes_               = CREditorResource.LoadEditorTexture("cr_icon_rope");      

    ic_rigid_glue_             = CREditorResource.LoadEditorTexture("cr_icon_glue_area");
    ic_multijoint_area_        = CREditorResource.LoadEditorTexture("cr_icon_joints_area");
    ic_multijoint_vertices_    = CREditorResource.LoadEditorTexture("cr_icon_joints_vertices");
    ic_multijoint_leaves_      = CREditorResource.LoadEditorTexture("cr_icon_joints_leaves");
    ic_multijoint_locators_    = CREditorResource.LoadEditorTexture("cr_icon_joints_locators");

    ic_motors_linear_  = CREditorResource.LoadEditorTexture("cr_icon_motors_linear");
    ic_motors_angular_ = CREditorResource.LoadEditorTexture("cr_icon_motors_angular");
    ic_servos_linear_  = CREditorResource.LoadEditorTexture("cr_icon_servos_linear");
    ic_servos_angular_ = CREditorResource.LoadEditorTexture("cr_icon_servos_angular");

    ic_fractureuniform_     = CREditorResource.LoadEditorTexture("cr_icon_fractureuniform");
    ic_fracturegeometry_    = CREditorResource.LoadEditorTexture("cr_icon_fracturegeometry");
    ic_fractureradial_      = CREditorResource.LoadEditorTexture("cr_icon_fractureradial");

    ic_welder_             = CREditorResource.LoadEditorTexture("cr_icon_welder");
    ic_tessellator_        = CREditorResource.LoadEditorTexture("cr_icon_tessellator");
    ic_procedural_         = CREditorResource.LoadEditorTexture("cr_icon_procedural");
    ic_selector_           = CREditorResource.LoadEditorTexture("cr_icon_selector_bygeom");

    ic_materialsubstituter_ = CREditorResource.LoadEditorTexture("cr_icon_material_substituter");
    ic_balltreeCreator_     = CREditorResource.LoadEditorTexture("cr_icon_balltree_creator");

    ic_gravity_             = CREditorResource.LoadEditorTexture("cr_icon_gravity");
    ic_explosion_           = CREditorResource.LoadEditorTexture("cr_icon_explosion");
    ic_wind_                = CREditorResource.LoadEditorTexture("cr_icon_wind");
    ic_aimed_force_         = CREditorResource.LoadEditorTexture("cr_icon_aimed_force");
    ic_speed_limiter_       = CREditorResource.LoadEditorTexture("cr_icon_speed_limiter");
    ic_attractor_           = CREditorResource.LoadEditorTexture("cr_icon_attractor");
    ic_jet_                 = CREditorResource.LoadEditorTexture("cr_icon_jet");

    ic_contact_emitter_     = CREditorResource.LoadEditorTexture("cr_icon_contact_emitter");
    ic_script_player_       = CREditorResource.LoadEditorTexture("cr_icon_script_player");

    ic_gameobject_          = CREditorResource.LoadEditorTexture("cr_icon_gameobject");
    ic_nameselector_        = CREditorResource.LoadEditorTexture("cr_icon_nameselector");

    bool isPro = UnityEditorInternal.InternalEditorUtility.HasPro();
    if (isPro)
    {
      ic_first_  = CREditorResource.LoadEditorTexture("player/cr_icon_first_pro");
      ic_last_   = CREditorResource.LoadEditorTexture("player/cr_icon_last_pro");
      ic_prev_   = CREditorResource.LoadEditorTexture("player/cr_icon_prev_pro");
      ic_next_   = CREditorResource.LoadEditorTexture("player/cr_icon_next_pro");
      ic_play_   = CREditorResource.LoadEditorTexture("player/cr_icon_play_pro");
      ic_pause_  = CREditorResource.LoadEditorTexture("player/cr_icon_pause_pro");
      ic_stop_   = CREditorResource.LoadEditorTexture("player/cr_icon_stop_pro");
      ic_loop_   = CREditorResource.LoadEditorTexture("player/cr_icon_loop_pro");   
    }
    else
    {
      ic_first_  = CREditorResource.LoadEditorTexture("player/cr_icon_first");
      ic_last_   = CREditorResource.LoadEditorTexture("player/cr_icon_last");
      ic_prev_   = CREditorResource.LoadEditorTexture("player/cr_icon_prev");
      ic_next_   = CREditorResource.LoadEditorTexture("player/cr_icon_next");
      ic_play_   = CREditorResource.LoadEditorTexture("player/cr_icon_play");
      ic_pause_  = CREditorResource.LoadEditorTexture("player/cr_icon_pause");
      ic_stop_   = CREditorResource.LoadEditorTexture("player/cr_icon_stop");
      ic_loop_   = CREditorResource.LoadEditorTexture("player/cr_icon_loop");   
    }

    ic_rec_ = CREditorResource.LoadEditorTexture("player/cr_icon_recbutton");

    CNGroupEditor.icon_                        = ic_group_;

    CNParameterModifierEditor.icon_            = ic_parameter_modifier_;
    CNTriggerByTimeEditor.icon_                = ic_trigger_byTimer_;
    CNTriggerByContactEditor.icon_             = ic_trigger_byContact_;
    CNTriggerByDetectorEditor.icon_            = ic_trigger_byDetector_;
    CNTriggerByForceEditor.icon_               = ic_trigger_byForce_;
    CNTriggerByExplosionEditor.icon_           = ic_trigger_byExplosion_;

    CNSubstituterEditor.icon_                  = ic_substituter_;

    CNRigidbodyEditor.icon_responsive_         = ic_rigidbodies_;
    CNRigidbodyEditor.icon_irresponsive_       = ic_irresponsivebodies_;
    CNAnimatedbodyEditor.icon_                 = ic_animatedbodies_;
    CNSoftbodyEditor.icon_                     = ic_softbodies_;
    CNClothEditor.icon_                        = ic_cloths_;
    CNRopeEditor.icon_                         = ic_ropes_;

    CNRigidGlueEditor.icon_rigid_glue_         = ic_rigid_glue_;
    CNJointGroupsEditor.icon_area_             = ic_multijoint_area_;
    CNJointGroupsEditor.icon_vertices_         = ic_multijoint_vertices_;
    CNJointGroupsEditor.icon_leaves_           = ic_multijoint_leaves_;
    CNJointGroupsEditor.icon_locators_         = ic_multijoint_locators_;

    CNServosEditor.icon_motor_linear_          = ic_motors_linear_;
    CNServosEditor.icon_motor_angular_         = ic_motors_angular_;
    CNServosEditor.icon_servo_linear_          = ic_servos_linear_;
    CNServosEditor.icon_servo_angular_         = ic_servos_angular_;

    CNFractureEditor.icon_uniform_             = ic_fractureuniform_;
    CNFractureEditor.icon_geometry_            = ic_fracturegeometry_;
    CNFractureEditor.icon_radial_              = ic_fractureradial_;
    CNWelderEditor.icon_                       = ic_welder_;
    CNTessellatorEditor.icon_                  = ic_tessellator_;
    CNHelperMeshEditor.icon_                   = ic_procedural_;
    CNSelectorEditor.icon_                     = ic_selector_;
    CNBalltreeGeneratorEditor.icon_            = ic_balltreeCreator_;

    CNGravityEditor.icon_                      = ic_gravity_;
    CNExplosionEditor.icon_                    = ic_explosion_;
    CNWindEditor.icon_                         = ic_wind_;
    CNAimedForceEditor.icon_                   = ic_aimed_force_;
    CNAimedFallEditor.icon_                    = ic_aimed_force_;
    CNSpeedLimiterEditor.icon_                 = ic_speed_limiter_;
    CNAttractorEditor.icon_                    = ic_attractor_;
    CNJetEditor.icon_                          = ic_jet_;

    CNContactEmitterEditor.icon_               = ic_contact_emitter_;
    CNScriptPlayerEditor.icon_                 = ic_script_player_;
    
    CNFieldController.icon_gameobject_       = ic_gameobject_;
    CNFieldController.icon_nameselector_     = ic_nameselector_;

    CRPlayerView.first_ = ic_first_;
    CRPlayerView.last_  = ic_last_;
    CRPlayerView.prev_  = ic_prev_;
    CRPlayerView.next_  = ic_next_;
    CRPlayerView.play_  = ic_play_;
    CRPlayerView.pause_ = ic_pause_;
    CRPlayerView.stop_  = ic_stop_;
    CRPlayerView.loop_  = ic_loop_;
    CRPlayerView.rec_   = ic_rec_;

    CNFracture.commonMaterial_   = CREditorResource.LoadEditorMaterial("cr_material_checkboard_5");
    CNHelperMeshEditor.material_ = CREditorResource.LoadEditorMaterial("cr_display_normals");
  }
  //----------------------------------------------------------------------------------
  public void FillNodeMenu(bool blockEdition, GenericMenu nodeMenu, bool context)
  {
    if (blockEdition)
    {
      nodeMenu.AddDisabledItem(new GUIContent("Add RigidBodies"));
      nodeMenu.AddDisabledItem(new GUIContent("Add Irresponsives"));
      nodeMenu.AddDisabledItem(new GUIContent("Add AnimatedBodies"));
      nodeMenu.AddSeparator("");
      nodeMenu.AddDisabledItem(new GUIContent("Add SoftBodies"));
      nodeMenu.AddDisabledItem(new GUIContent("Add ClothBodies"));
      nodeMenu.AddDisabledItem(new GUIContent("Add RopeBodies"));
    }
    else
    {
      nodeMenu.AddItem(new GUIContent("Add RigidBodies"), false, () => { Controller.CreateNodeUnique<CNRigidbody>("RigidBodies"); });
      nodeMenu.AddItem(new GUIContent("Add Irresponsives"), false, () => { Controller.CreateIrresponsiveBodiesNode(); });
      nodeMenu.AddItem(new GUIContent("Add AnimatedBodies"), false, () => { Controller.CreateNodeUnique<CNAnimatedbody>("AnimatedBodies"); });
      nodeMenu.AddSeparator("");
      nodeMenu.AddItem(new GUIContent("Add SoftBodies"), false, () => { Controller.CreateNodeUnique<CNSoftbody>("SoftBodies"); });
      nodeMenu.AddItem(new GUIContent("Add ClothBodies"), false, () => { Controller.CreateClothBodiesNode(); });
      nodeMenu.AddItem(new GUIContent("Add RopeBodies"), false, () => { Controller.CreateRopeBodiesNode(); });
    }
  }
  #region statics
  

  public enum MessageBoxType 
  {
    Info = 0,
    Warning = 1,
    Error = 2
  }

  public static void MessageBox(string message, MessageBoxType type)
  {
    MessageType messageType;
    if (type == MessageBoxType.Error) messageType = MessageType.Error;
    else if (type == MessageBoxType.Warning) messageType = MessageType.Warning;
    else messageType = MessageType.Info;

    EditorGUILayout.HelpBox(message, messageType);
  }


  public static CommandNode GetSelectedNode()
  {
    CNManager manager = Instance.Controller;
    return ( manager.GetSelectedNode() );
  }

  #endregion

  //----------------------------------------------------------------------------------
  private void SaveFX()
  {
    string path = EditorUtility.SaveFilePanelInProject("Save FX", ".prefab", "prefab", "Please enter a file name to save the data to");
    if (!string.IsNullOrEmpty(path))
    {
      Transform parent = FxData.gameObject.transform.parent;
      if (parent != null)
      {
        GameObject parentGO = parent.gameObject;
        UnityEngine.Object pref = PrefabUtility.CreateEmptyPrefab(path);
        PrefabUtility.ReplacePrefab(parentGO, pref);
        AssetDatabase.SaveAssets();
      }
    }
  }
  //----------------------------------------------------------------------------------

} // class CRManagerEditor

} //namespace CaronteFX...
