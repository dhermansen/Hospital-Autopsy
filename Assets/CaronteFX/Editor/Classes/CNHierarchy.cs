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
  public class CNHierarchy : IListController
  {
    private CRManagerEditor   managerEditor_;
    private CNManager         manager_;
    private CREntityManager   entityManager_;
    private CRGOManager       goManager_;
    private CNEditorFactory   nodeEditorFactory_;

    private Caronte_Fx        fxData_;
    private CNGroup           rootNode_;
    private CNGroup           subeffectsNode_;
    
    private List<CommandNode> listCommandNode_;
    private List<CommandNode> listCommandNodeGUI_; 
    private List<int>         listCommandNodeGUIIndentation_;
    private List<CommandNode> listCommandNodeGUIAux_;
    private List<int>         listCommandNodeGUIAuxIndentation_;

    private HashSet<CommandNode> hashSetAlreadyVisitedNodesAux_;

    private List<CommandNode> listClonedNodeAux_;
    private Dictionary<CommandNode, CommandNode> dictNodeToClonedNodeAux_;

    private List<CommandNode> listNodesToDeleteDeferred_;
    private List<CommandNode> listEffectRootNode_;
    
    #region HIERARCHY NODE EDITORS
    private Dictionary<CommandNode, CommandNodeEditor> dictCommandNodeEditor_;

    private List<CommandNodeEditor>    listCommandNodeEditor_; 
    private List<CNGroupEditor>        listGroupEditor_;
    private List<CNBodyEditor>         listBodyEditor_;
    private List<CNAnimatedbodyEditor> listAnimatedBodyEditor_;
    private List<CNJointGroupsEditor>  listMultiJointEditor_;
    private List<CNServosEditor>       listServosEditor_;
    private List<CNEntityEditor>       listEntityEditor_;
    private List<CNScriptPlayerEditor> listScriptPlayerEditor_;

    private List<CommandNodeEditor>    listCommandNodeEditorAux_;
    private List<CRTreeNode>           listTreeNodeAux_;
 
    public CommandNode RootNode
    {
      get { return rootNode_; }
    }

    public List<CommandNode> ListCommandNode
    {
      get { return listCommandNode_; }
    }

    public List<CommandNodeEditor> ListCommandNodeEditor
    {
      get { return listCommandNodeEditor_; }
    }

    public List<CNGroupEditor> ListGroupEditor
    {
      get { return listGroupEditor_; }
    }

    public List<CNBodyEditor> ListBodyEditor
    {
      get { return listBodyEditor_; }
    }

    public List<CNAnimatedbodyEditor> ListAnimatedBodyEditor
    {
      get { return listAnimatedBodyEditor_; }
    }

    public List<CNJointGroupsEditor> ListMultiJointEditor
    {
      get { return listMultiJointEditor_; }
    }

    public List<CNServosEditor> ListServosEditor
    {
      get { return listServosEditor_; }
    }

    public List<CNEntityEditor> ListEntityEditor
    {
      get { return listEntityEditor_; }
    }

    public List<CNScriptPlayerEditor> ListScriptPlayerEditor
    {
      get { return listScriptPlayerEditor_; }
    }
    #endregion

    #region SELECTION DATA
    public CRSelectionData SelectionData
    {
      get { return fxData_.SelectionData; }
    }

    public List<CommandNode> Selection
    {
      get { return SelectionData.ListSelectedNode; }
    }

    public List<CommandNode> SelectionWithChildren
    {
      get { return SelectionData.ListSelectedNodeAndChildren; }
    }

    private int NumSelectedNode
    {
      get { return SelectionData.SelectedNodeCount; }
    }
     
    public CommandNode FocusedNode 
    { 
      get { return SelectionData.FocusedNode; }
      set { SelectionData.FocusedNode = value; }
    }

    public CommandNode LastSelectedNode
    {
      get { return SelectionData.LastSelectedNode; }
      set { SelectionData.LastSelectedNode = value; }
    }

    public CommandNodeEditor CurrentNodeEditor
    {
      get
      {
        if (FocusedNode != null && dictCommandNodeEditor_.ContainsKey( FocusedNode ) )
        {
          return dictCommandNodeEditor_[FocusedNode];
        }
        return null;       
      }
    }

    [Flags]
    enum SelectionFlags
    {
      None         = 0,
      Root         = (1 << 0),
      Deletable    = (1 << 1),
      Renamable    = (1 << 2),
      Clonable     = (1 << 3),
      Effect       = (1 << 4),
      Missing      = (1 << 5),
      Enabled      = (1 << 6),
      Disabled     = (1 << 7),
      Visible      = (1 << 8),
      Hidden       = (1 << 9),
      Excluded     = (1 << 10),
      Included     = (1 << 11),
      Group        = (1 << 12),        
      Body         = (1 << 13),
      Irresponsive = (1 << 14),
      Responsive   = (1 << 15),
      MultiJoint   = (1 << 16),
      All          = ~(-1 << 17)
    }
    #endregion

    #region ILISTCONTROLLER PROPERTIES

    private int itemIdxEditing_ = -1;
    public int ItemIdxEditing
    {
      get { return itemIdxEditing_; }
      set { itemIdxEditing_ = value; }
    }

    private string itemIdxEditingName_ = string.Empty;
    public string ItemIdxEditingName
    {
      get { return itemIdxEditingName_; }
      set { itemIdxEditingName_ = value; }
    }

    private bool blockEdition_ = false;
    public bool BlockEdition
    {
      get { return blockEdition_; }
      set { blockEdition_ = value; }
    }

    public int NumVisibleElements
    {
      get { return listCommandNodeGUI_.Count; }
    }
    #endregion

    #region ILISTCONTROLLER METHODS
    public bool ItemIsNull(int itemIdx)
    {
      CommandNode node = listCommandNodeGUI_[itemIdx];
      return (node == null);
    }
    //-----------------------------------------------------------------------------------   
    public bool ItemIsBold(int itemIdx)
    {
      CommandNode node = listCommandNodeGUI_[itemIdx];
      CNGroup groupNode = node as CNGroup;

      if (groupNode != null && (groupNode.IsEffectRoot || groupNode.IsSubeffectsFolder) )
      {
        return true;
      }
      return false;
    }
    //-----------------------------------------------------------------------------------
    public bool ItemIsSelectable(int itemIdx)
    {
      return true;
    }
    //-----------------------------------------------------------------------------------
    public bool ItemIsSelected(int itemIdx)
    {
      if (Selection.Contains( listCommandNodeGUI_[itemIdx]) )
      {
        return true;
      }
      return false; 
    }
    //-----------------------------------------------------------------------------------
    public int ItemIndentLevel(int itemIdx)
    {
      return listCommandNodeGUIIndentation_[itemIdx];
    }
    //-----------------------------------------------------------------------------------
    public bool ItemIsGroup(int itemIdx)
    {
      CommandNode node = listCommandNodeGUI_[itemIdx];
      return (node is CNGroup);
    }
    //-----------------------------------------------------------------------------------
    public bool ItemIsExpanded(int itemIdx)
    {
      CommandNode node = listCommandNodeGUI_[itemIdx];
      CNGroup groupNode = (CNGroup)node;
      return groupNode.IsOpen;
    }
    //-----------------------------------------------------------------------------------
    public void ItemSetExpanded(int itemIdx, bool open)
    {
      CommandNode node = listCommandNodeGUI_[itemIdx];
      CNGroup groupNode = (CNGroup)node;

      if (itemIdx == 0)
      {
        groupNode.IsOpen = true;
      }
      else
      {
        if (open != groupNode.IsOpen)
        {
          groupNode.IsOpen = open;
          EditorUtility.SetDirty(groupNode);
          RebuildNodeListForGUI();
        }
      }
    }
    //-----------------------------------------------------------------------------------
    public bool ItemIsDraggable(int itemIdx)
    {
      CommandNode node = listCommandNodeGUI_[itemIdx];
      if ( node.IsEffectRoot || IsNodeOrAncestorIsSubeffectsNode(node) )
        return false;

      return true;
    }
    //-----------------------------------------------------------------------------------
    public bool ItemHasContext(int itemIdx)
    {
      CommandNode node = listCommandNodeGUI_[itemIdx];
      if ( node.IsSubeffectsFolder )
        return false;

      return true;
    }
    //-----------------------------------------------------------------------------------
    public bool ItemIsEditable(int itemIdx)
    {
      return true;
    }
    //-----------------------------------------------------------------------------------
    public bool ItemIsDisabled(int itemIdx)
    {
      CommandNode node = listCommandNodeGUI_[itemIdx];
      if (IsNodeEnabledInHierarchy(node))
        return false;
      
      return true;
    }
    //-----------------------------------------------------------------------------------
    public bool ItemIsExcluded(int itemIdx)
    {
      CommandNode node = listCommandNodeGUI_[itemIdx];
      if (IsNodeExcludedInHierarchy(node))
        return true;
      
      return false;
    }
    //-----------------------------------------------------------------------------------
    public bool ItemIsValidDragTarget(int itemIdx, string dragDropIdentifier)
    {
      if (!SimulationManager.IsEditing()) return false;

      CustomDragData receivedDragData  = DragAndDrop.GetGenericData(dragDropIdentifier) as CustomDragData;
      UnityEngine.Object[] dragObjects = DragAndDrop.objectReferences;

      bool anyHasMesh = CREditorUtils.CheckIfAnySceneGameObjects( dragObjects );
      CommandNode node = listCommandNodeGUI_[itemIdx];

      if ( IsNodeOrAncestorIsSubeffectsNode(node) )
      {
        return false;
      }

      if ( receivedDragData != null || (anyHasMesh && !node.IsEffectRoot ) )
      {
        return true;
      }
      else
      {
        return false;
      }
    }
    //-----------------------------------------------------------------------------------
    public bool ListIsValidDragTarget()
    {
      if (!SimulationManager.IsEditing()) return false;

      UnityEngine.Object[] dragObjects = DragAndDrop.objectReferences;

      return ( CREditorUtils.CheckIfAnySceneGameObjects( dragObjects ) );
    }
    //-----------------------------------------------------------------------------------
    public void MoveDraggedItem(int itemFromIdx, int itemToIdx, bool edgeDrag)
    {
      if ( !SimulationManager.IsEditing() ) return;
      if ( itemFromIdx == itemToIdx ) return;
 
      CommandNode nodeTo  = listCommandNodeGUI_[itemToIdx];
      CNGroup nodeToGroup = nodeTo as CNGroup;
      
      if ( nodeToGroup == null && !edgeDrag )
      {
        return; 
      }

      bool updateGUI = false;

      Undo.RecordObject(fxData_, "CaronteFX - Undo move nodes");

      foreach (CommandNode node in Selection)
      {
        MoveNodeInHierarchy( node, nodeTo, edgeDrag, ref updateGUI );
        EditorUtility.SetDirty(node);
      }

      EditorUtility.SetDirty(nodeTo);
      Undo.CollapseUndoOperations( Undo.GetCurrentGroup() );

      if (updateGUI)
      {
        RebuildNodeListForGUI();
        RecalculateFieldsAutomatic();
      }
    }
    //-----------------------------------------------------------------------------------
    public void AddDraggedObjects(int itemToIdx, UnityEngine.Object[] objects)
    {
      if (!SimulationManager.IsEditing()) return;

      if (itemToIdx == -1)
      {
        AddDraggedObjects(null, objects);
      }
      else
      {
        CommandNode node = listCommandNodeGUI_[itemToIdx];
        AddDraggedObjects(node, objects);
      } 
    }
    //-----------------------------------------------------------------------------------
    public void RemoveSelected()
    {
      int   nSelectedNode     = NumSelectedNode;
      bool  isEditingNodeName = itemIdxEditing_ != -1;

      if (!blockEdition_ && nSelectedNode > 0 && !isEditingNodeName)
      {
        AddToRemoveNodeDefeerredList(Selection);
      }
    }
    //-----------------------------------------------------------------------------------
    public void SetResponsiveness( List<CommandNode> listCommandNode, bool responsive )
    {
      foreach( CommandNode commandNode in listCommandNode )
      {
        CommandNodeEditor nodeEditor = dictCommandNodeEditor_[commandNode];
        CNRigidbodyEditor rigidEditor = nodeEditor as CNRigidbodyEditor;
        if (rigidEditor != null)
        {
          rigidEditor.SetResponsiveness( responsive );
        }
      }
    }
    //-----------------------------------------------------------------------------------
    public void SetMultiJointCreationMode( List<CommandNode> listCommandNode, CNJointGroups.CreationModeEnum creationMode )
    {
      foreach( CommandNode commandNode in listCommandNode )
      {
        CNJointGroups jgNode = commandNode as CNJointGroups;
        if (jgNode != null)
        {
          jgNode.CreationMode = creationMode;
        }
      }
    }
    //-----------------------------------------------------------------------------------
    public void UnselectSelected()
    {
      FocusedNode = null;
      Selection.Clear();

      managerEditor_.Repaint();
      CNFieldWindow.CloseIfOpen();
    }
    //-----------------------------------------------------------------------------------
    public string GetItemName(int nodeIdx)
    {
      CommandNode node = listCommandNodeGUI_[nodeIdx];
      return (node.Name);
    }
    //-----------------------------------------------------------------------------------
    public void SetItemName(int nodeIdx, string name)
    {
      if (name == string.Empty)
        return;
     
      if (nodeIdx == 0)
      {
        fxData_.gameObject.name = name;
      }
      CommandNode node = listCommandNodeGUI_[nodeIdx];
      node.Name = name;

      managerEditor_.RepaintSubscribers();
    }
    //-----------------------------------------------------------------------------------
    public string GetItemListName(int nodeIdx)
    {
      CommandNode node = listCommandNodeGUI_[nodeIdx];
      return (node.ListName);
    }
    //-----------------------------------------------------------------------------------
    public Texture GetItemIcon(int nodeIdx)
    {
      CommandNode node = listCommandNodeGUI_[nodeIdx];
      CommandNodeEditor nodeEditor = dictCommandNodeEditor_[node];

      if ( nodeEditor != null )
      {
        return nodeEditor.TexIcon;
      }
      return null;
    }
    //-----------------------------------------------------------------------------------
    public void OnClickItem(int itemIdx, bool ctrlPressed, bool shiftPressed, bool altPressed, bool isUpClick)
    {
      if (isUpClick && NumSelectedNode < 2)
      {
        return;
      }

      Undo.RecordObject( fxData_, "change node selection - " + fxData_.name );

      CommandNode prevSelectedNode = LastSelectedNode;
      CommandNode clickedNode      = listCommandNodeGUI_[itemIdx];

      //mousedown
      if ( !isUpClick )
      {
        if (ctrlPressed)
        {
          if (Selection.Contains(clickedNode))
          {
            Selection.Remove(clickedNode);
          }
          else
          {
            Selection.Add(clickedNode);
          }
        }
        else if (shiftPressed)
        {
          if (prevSelectedNode != null)
          {
            int prevIndex = listCommandNodeGUI_.IndexOf(prevSelectedNode);
            int range = itemIdx - prevIndex;
            int increment = (range > 0) ? 1 : -1;

            while (prevIndex != itemIdx)
            {
              CommandNode node = listCommandNodeGUI_[prevIndex + increment];
              if (Selection.Contains(node))
              {
                Selection.Remove(node);
              }
              else
              {
                Selection.Add(node);
              }
              prevIndex += increment;
            }

            Undo.CollapseUndoOperations( Undo.GetCurrentGroup() );
          }
        }

        if ( !Selection.Contains(clickedNode) )
        {
          ClearFocusNode( clickedNode );
        }
        else
        {
          SceneSelection();
          return;
        }
      }
      //mouseup
      else
      {
        if ( !ctrlPressed && !shiftPressed )
        {
          ClearFocusNode( clickedNode );
        }    
      }

      LastSelectedNode = clickedNode;
      EditorUtility.SetDirty(fxData_);
    }
    //----------------------------------------------------------------------------------
    public void OnSelectItemPrev()
    {
      int minIdx = 0;
      int maxIdx = listCommandNodeGUI_.Count - 1;

      int lastSelectedIdx = listCommandNodeGUI_.FindIndex( (node) => { return node == LastSelectedNode; });
      int selectedIdx = Mathf.Clamp(lastSelectedIdx - 1, minIdx, maxIdx);

      CommandNode newNode = listCommandNodeGUI_[selectedIdx];

      FocusAndSelect(newNode);
      SceneSelection();
      EditorUtility.SetDirty(fxData_);
    }
    //----------------------------------------------------------------------------------
    public void OnSelectItemNext()
    {
      int minIdx = 0;
      int maxIdx = listCommandNodeGUI_.Count - 1;

      int lastSelectedIdx = listCommandNodeGUI_.FindIndex((node) => { return node == LastSelectedNode; });
      int selectedIdx = Mathf.Clamp(lastSelectedIdx + 1, minIdx, maxIdx);

      CommandNode newNode = listCommandNodeGUI_[selectedIdx];

      FocusAndSelect(newNode);
      SceneSelection();
      EditorUtility.SetDirty(fxData_);
    }
    //----------------------------------------------------------------------------------
    private void ClearFocusNode( CommandNode node )
    {
      Selection.Clear();
      Selection.Add(node);

      FocusedNode = node;
      SceneSelection();
    }
    //----------------------------------------------------------------------------------
    public void OnContextClickItem(int itemIdx, bool ctrlPressed, bool shiftPressed, bool altPressed)
    {
      CommandNode clickedNode = listCommandNodeGUI_[itemIdx];
      if (!Selection.Contains(clickedNode))
      {
        OnClickItem(itemIdx, ctrlPressed, shiftPressed, altPressed, false);
      }
      ContextClick();
    }
    //----------------------------------------------------------------------------------
    public void OnContextClickList()
    {
      ContextClick();
    }
    //----------------------------------------------------------------------------------
    public void OnDoubleClickItem(int itemIdx)
    {
      if ( dictCommandNodeEditor_.ContainsKey(FocusedNode) )
      {
        CommandNodeEditor nodeEditor = dictCommandNodeEditor_[FocusedNode];

        EditorApplication.delayCall -= nodeEditor.SceneSelection;
        EditorApplication.delayCall += nodeEditor.SceneSelection;
      }

      SceneView sv = SceneView.lastActiveSceneView;
      if (sv != null)
      {
        sv.FrameSelected();
      }
    }
    //-----------------------------------------------------------------------------------
    public IView GetFocusedNodeView()
    {
      if(FocusedNode != null)
      { 
        if ( dictCommandNodeEditor_.ContainsKey(FocusedNode) )
        {
          return dictCommandNodeEditor_[FocusedNode];
        }
      }

      return null;
    }
    //-----------------------------------------------------------------------------------
    public void GUIUpdateRequested()
    {
      CRManagerEditor.RepaintIfOpen();
    }
    //-----------------------------------------------------------------------------------
    public void FinishRenderingItems()
    {

    }
    #endregion
    //-----------------------------------------------------------------------------------
    public CNHierarchy(CNManager manager, CRManagerEditor managerEditor, CREntityManager entityManager, CRGOManager goManager)
    {
      manager_       = manager;
      managerEditor_ = managerEditor;
      entityManager_ = entityManager;
      goManager_     = goManager;

      nodeEditorFactory_ = new CNEditorFactory(this);

      listCommandNode_ = new List<CommandNode>();

      hashSetAlreadyVisitedNodesAux_ = new HashSet<CommandNode>();

      listCommandNodeGUI_            = new List<CommandNode>();
      listCommandNodeGUIIndentation_ = new List<int>();
      listCommandNodeGUIAux_            = new List<CommandNode>();
      listCommandNodeGUIAuxIndentation_ = new List<int>();

      listClonedNodeAux_       = new List<CommandNode>();
      dictNodeToClonedNodeAux_ = new Dictionary<CommandNode, CommandNode>();

      dictCommandNodeEditor_  = new Dictionary<CommandNode,CommandNodeEditor>();

      listNodesToDeleteDeferred_ = new List<CommandNode>();

      listEffectRootNode_      = new List<CommandNode>();
      
      listCommandNodeEditor_   = new List<CommandNodeEditor>(); 
      listGroupEditor_         = new List<CNGroupEditor>();
      listBodyEditor_          = new List<CNBodyEditor>();
      listAnimatedBodyEditor_  = new List<CNAnimatedbodyEditor>();
      listServosEditor_        = new List<CNServosEditor>();
      listMultiJointEditor_    = new List<CNJointGroupsEditor>();
      listEntityEditor_        = new List<CNEntityEditor>();
      listScriptPlayerEditor_  = new List<CNScriptPlayerEditor>();

      listCommandNodeEditorAux_ = new List<CommandNodeEditor>();
      listTreeNodeAux_          = new List<CRTreeNode>(); 
    }
    //----------------------------------------------------------------------------------
    public void Init()
    {
      fxData_         = manager_.FxData;
      rootNode_       = manager_.RootNode;
      subeffectsNode_ = manager_.SubeffectsNode;

      BlockEdition = false;

      goManager_.HierarchyChange();

      RemoveNullNodes();
      RebuildNodeEditors();
      RebuildLists();   

      UpdateHierarchyEffectsScope();

      RecalculateFieldsStart();
    }
    //-----------------------------------------------------------------------------------
    public void Deinit()
    {
      listCommandNode_    .Clear();
      hashSetAlreadyVisitedNodesAux_.Clear();

      listCommandNodeGUI_              .Clear();
      listCommandNodeGUIIndentation_   .Clear();
      listCommandNodeGUIAux_           .Clear();
      listCommandNodeGUIAuxIndentation_.Clear();

      listClonedNodeAux_      .Clear();     
      dictNodeToClonedNodeAux_.Clear();

      dictCommandNodeEditor_ .Clear();

      listEffectRootNode_  .Clear();

      listCommandNodeEditor_ .Clear();
      listGroupEditor_       .Clear();
      listBodyEditor_        .Clear();
      listAnimatedBodyEditor_.Clear(); 
      listMultiJointEditor_  .Clear();
      listServosEditor_      .Clear();
      listEntityEditor_      .Clear();
      listScriptPlayerEditor_.Clear();

      listCommandNodeEditorAux_.Clear();
      listTreeNodeAux_         .Clear();
  
      FieldManager.ClearAllFields();
    }
    //-----------------------------------------------------------------------------------
    public bool AreCurrentEffectsValid()
    {
      foreach( CommandNode effectNode in listEffectRootNode_)
      {
        if ( effectNode == null)
        {
          return false;
        }
      }
      return true;
    }
    //----------------------------------------------------------------------------------
    public bool IsEffectIncluded(Caronte_Fx fx)
    {
      for (int i = 0; i < listEffectRootNode_.Count; i++)
      {
        CommandNode node = listEffectRootNode_[i];

        if (node == fx.e_getRootNode())
        {
          return true;
        }
      }
      return false;
    }
    //-----------------------------------------------------------------------------------
    public bool RootNodeAlreadyContainedInNode(Caronte_Fx fx)
    {     
      CommandNode node = fx.e_getRootNode();
      return node.Find((CRTreeNode treeNode) => { return treeNode == rootNode_; });
    }
    //-----------------------------------------------------------------------------------
    private bool IsNodeOrAncestorIsSubeffectsNode(CommandNode node)
    {
      CommandNode currentNode = node;
      CommandNode parent = null;

      if (currentNode == subeffectsNode_)
      {
        return true;
      }

      do
      {
        parent = (CommandNode)currentNode.Parent;
        currentNode = parent;
        if (parent == subeffectsNode_)
        {
          return true;
        }
      } while (parent != null);
      
      return false;
    }
    //-----------------------------------------------------------------------------------
    public bool IsNodeEnabledInHierarchy(CommandNode node)
    {
      bool parentEnabled = true;
      CommandNode parent = (CommandNode)node.Parent;
      if ( parent != null && node != rootNode_ )
      {
        parentEnabled = IsNodeEnabledInHierarchy(parent);
      }
      return node.IsNodeEnabled && parentEnabled; 
    }
    //-----------------------------------------------------------------------------------
    public bool IsNodeVisibleInHierarchy(CommandNode node)
    {
      bool parentVisible = true;
      CommandNode parent = (CommandNode)node.Parent;
      if ( parent != null && node != rootNode_ )
      {
        parentVisible = IsNodeVisibleInHierarchy(parent);
      }
      return node.IsNodeVisible && parentVisible; 
    }
    //-----------------------------------------------------------------------------------
    public bool IsNodeExcludedInHierarchy(CommandNode node)
    {
      bool parentExcluded = false;
      CommandNode parent = (CommandNode)node.Parent;
      if ( parent != null && node != rootNode_ )
      {
        parentExcluded = IsNodeExcludedInHierarchy(parent);
      }
      return node.IsNodeExcluded || parentExcluded; 
    }
    //-----------------------------------------------------------------------------------
    private void RebuildLists()
    {
      RebuildNodeListForGUI();
      RebuildNodeLists();  
    }
    //-----------------------------------------------------------------------------------
    private void RebuildNodeLists()
    {
      listCommandNode_   .Clear();
      listEffectRootNode_.Clear();

      listCommandNodeEditor_ .Clear(); 
      listGroupEditor_       .Clear();
      listBodyEditor_        .Clear();
      listAnimatedBodyEditor_.Clear();
      listMultiJointEditor_  .Clear();
      listServosEditor_      .Clear();
      listEntityEditor_      .Clear();
      listScriptPlayerEditor_.Clear();

      hashSetAlreadyVisitedNodesAux_.Clear();
      rootNode_.Traversal(AddNodeToLists, HasNodeNotBeenVisitedAlready);
    }
    //----------------------------------------------------------------------------------
    private void AddNodeToLists(CRTreeNode treeNode)
    {
      CommandNode node = (CommandNode)treeNode;

      listCommandNode_.Add( node );
      listCommandNodeEditor_.Add( (CommandNodeEditor)dictCommandNodeEditor_[node] );

      CNGroup groupNode = node as CNGroup;
      if (groupNode != null)
      {
        groupNode.IsOpenAux = groupNode.IsOpen;
        listGroupEditor_.Add( (CNGroupEditor)dictCommandNodeEditor_[groupNode] );

        if (groupNode.IsEffectRoot)
        {
          listEffectRootNode_.Add(groupNode);
        }
      }

      CNBody bodyNode = node as CNBody;
      if (bodyNode != null)
      {
        listBodyEditor_.Add( (CNBodyEditor)dictCommandNodeEditor_[bodyNode] );
      }

      CNAnimatedbody animatedBodyNode = node as CNAnimatedbody;
      if (animatedBodyNode != null)
      {
        listAnimatedBodyEditor_.Add( (CNAnimatedbodyEditor)dictCommandNodeEditor_[animatedBodyNode] );
      }

      CNJointGroups multiJointNode = node as CNJointGroups;
      if (multiJointNode != null)
      {
        listMultiJointEditor_.Add( (CNJointGroupsEditor)dictCommandNodeEditor_[multiJointNode] );
      }

      CNServos servosNode = node as CNServos;
      if (servosNode != null)
      {
        listServosEditor_.Add( (CNServosEditor)dictCommandNodeEditor_[servosNode] );
      }

      CNEntity entityNode = node as CNEntity;
      if (entityNode != null)
      {
        listEntityEditor_.Add( (CNEntityEditor)dictCommandNodeEditor_[entityNode] );
      }

      CNScriptPlayer scriptPlayerNode = node as CNScriptPlayer;
      if (scriptPlayerNode != null)
      {
        listScriptPlayerEditor_.Add( (CNScriptPlayerEditor)dictCommandNodeEditor_[scriptPlayerNode] );
      }
    }
    //-----------------------------------------------------------------------------------
    private bool HasNodeNotBeenVisitedAlready(CRTreeNode treeNode)
    {
      CommandNode node = (CommandNode)treeNode;
      if (hashSetAlreadyVisitedNodesAux_.Contains((CommandNode)treeNode))
      {
        return false;
      }

      hashSetAlreadyVisitedNodesAux_.Add(node);
      return true;
    }
    //-----------------------------------------------------------------------------------
    private void RebuildNodeEditors()
    {
      RebuildNodeEditorHierarchy();
      SetNodeScopes();
    }
    //----------------------------------------------------------------------------------
    private void RebuildNodeEditorHierarchy()
    {
      dictCommandNodeEditor_.Clear();
      BuildNodeEditorHierarchy(rootNode_);
    }
    //----------------------------------------------------------------------------------
    private void BuildNodeEditorHierarchy(CommandNode node)
    {
      hashSetAlreadyVisitedNodesAux_.Clear();
      node.Traversal(CreateNodeEditorIfNotExist, HasNodeNotBeenVisitedAlready);
    }
    //----------------------------------------------------------------------------------
    private void CreateNodeEditorIfNotExist(CRTreeNode treeNode)
    {
      CommandNode node = (CommandNode)treeNode;
      if ( !dictCommandNodeEditor_.ContainsKey(node) )
      {
        CommandNodeEditor nodeEditor = CreateNodeEditor(node);
        dictCommandNodeEditor_[node] = nodeEditor;
      }
    }
    //----------------------------------------------------------------------------------
    private CommandNodeEditor CreateNodeEditor(CommandNode node)
    {
      CommandNodeEditor editor = null;
      if ( nodeEditorFactory_.CanCrateEditor(node) )
      {
        editor = nodeEditorFactory_.CreateNodeEditor(node);
        editor.LoadInfo();
      }

      return editor;
    }
    //----------------------------------------------------------------------------------
    private void RemoveNodeEditorHierarchy(CommandNode node)
    {
      hashSetAlreadyVisitedNodesAux_.Clear();
      node.Traversal(RemoveNodeEditorIfExist, HasNodeNotBeenVisitedAlready);
    }
    //----------------------------------------------------------------------------------
    private void RemoveNodeEditorIfExist(CRTreeNode treeNode)
    {
      CommandNode node = (CommandNode)treeNode;
      if ( dictCommandNodeEditor_.ContainsKey(node) )
      {
        RemoveNodeEditor(node);
      }
    }
    //----------------------------------------------------------------------------------
    public void RemoveNodeEditor(CommandNode node)
    {
      CommandNodeEditor nodeEditor = dictCommandNodeEditor_[node];
      nodeEditor.FreeResources();

      dictCommandNodeEditor_[node] = null;
      dictCommandNodeEditor_.Remove(node);
    }
    //-----------------------------------------------------------------------------------
    public CommandNodeEditor GetNodeEditor(CommandNode node)
    {
      return dictCommandNodeEditor_[node];
    }
    //-----------------------------------------------------------------------------------
    private void RebuildNodeListForGUI()
    {
      listCommandNodeGUI_           .Clear();
      listCommandNodeGUIIndentation_.Clear();

      hashSetAlreadyVisitedNodesAux_.Clear();   
      rootNode_.Traversal(AddToGUINodeList, IsNotSubEffectsNodeAndNotAlreadyVisited, IsNodeGroupAndOpen, 0);
      subeffectsNode_.Traversal(AddToGUINodeList, HasNotBeenVisitedAlreadyAndIsSubeffectWithChildren, IsNodeGroupAndOpen, 0);
    }
    //----------------------------------------------------------------------------------
    public void GetListNodeGUIAux(CommandNode node, out List<CommandNode> listCommandNodeGUIAux, out List<int> listCommandNodeGUIAuxIndentation)
    {
      listCommandNodeGUIAux_.Clear();
      listCommandNodeGUIAuxIndentation_.Clear();

      hashSetAlreadyVisitedNodesAux_.Clear();
      if (node == null)
      {
        rootNode_.Traversal(AddToGUIAuxNodeList, IsNotSubEffectsNodeAndNotAlreadyVisited, IsNodeGroupAndOpenAux, 0);
        subeffectsNode_.Traversal(AddToGUIAuxNodeList, HasNotBeenVisitedAlreadyAndIsSubeffectWithChildren, IsNodeGroupAndOpenAux, 0);
      }
      else
      {
        node.Traversal(AddToGUIAuxNodeList, IsNotSubEffectsNodeAndNotAlreadyVisited, IsNodeGroupAndOpenAux, 0);
      }
      
      listCommandNodeGUIAux            = listCommandNodeGUIAux_;
      listCommandNodeGUIAuxIndentation = listCommandNodeGUIAuxIndentation_;
    }

    //----------------------------------------------------------------------------------
    private void AddToGUINodeList(CRTreeNode treeNode, int indentation)
    {
      CommandNode node = (CommandNode)treeNode;

      listCommandNodeGUI_.Add(node);
      listCommandNodeGUIIndentation_.Add(indentation);
    }
    //----------------------------------------------------------------------------------
    private void AddToGUIAuxNodeList(CRTreeNode treeNode, int indentation)
    {
      CommandNode node = (CommandNode)treeNode;

      listCommandNodeGUIAux_.Add(node);
      listCommandNodeGUIAuxIndentation_.Add(indentation);
    }
    //----------------------------------------------------------------------------------
    private bool IsNotSubEffectsNodeAndNotAlreadyVisited(CRTreeNode treeNode)
    {
      CommandNode node = (CommandNode)treeNode;
      if (hashSetAlreadyVisitedNodesAux_.Contains((CommandNode)treeNode))
      {
        return false;
      }

      CNGroup groupNode = treeNode as CNGroup;
      if (groupNode != null)
      {
        bool isSubEffectsFolder = groupNode.IsSubeffectsFolder;
        if (isSubEffectsFolder)
        {
          return false;
        }
      }
      hashSetAlreadyVisitedNodesAux_.Add(node);
      return true;
    }
    //----------------------------------------------------------------------------------
    private bool HasNotBeenVisitedAlreadyAndIsSubeffectWithChildren(CRTreeNode treeNode)
    {
      CommandNode node = (CommandNode)treeNode;
      if (hashSetAlreadyVisitedNodesAux_.Contains(node))
      {
        return false;
      }

      hashSetAlreadyVisitedNodesAux_.Add(node);

      CNGroup groupNode = treeNode as CNGroup;
      if (groupNode != null)
      {
        if (groupNode.IsSubeffectsFolder)
        {
          if (groupNode.ChildCount > 0)
          {
            return true;
          }
          else
          {
            return false;
          }
        }
      }
      return true;
    }
    //----------------------------------------------------------------------------------
    private bool IsNodeGroupAndOpen(CRTreeNode treeNode)
    {
      CNGroup groupNode = treeNode as CNGroup;
      if (groupNode != null)
      {
        return groupNode.IsOpen;
      }
      return false;
    }
    //----------------------------------------------------------------------------------
    private bool IsNodeGroupAndOpenAux(CRTreeNode treeNode)
    {
      CNGroup groupNode = treeNode as CNGroup;
      if (groupNode != null)
      {
        return groupNode.IsOpenAux;
      }
      return false;
    }
    //----------------------------------------------------------------------------------
    private void CreateEntitiesInHierarchy(CommandNode node)
    {
      hashSetAlreadyVisitedNodesAux_.Clear();
      node.Traversal(CreateEntityInHierarchy, HasNodeNotBeenVisitedAlready);
    }
    //----------------------------------------------------------------------------------
    public void CreateEntityInHierarchy(CRTreeNode treeNode)
    {
      CommandNodeEditor nodeEditor = GetNodeEditor((CommandNode)treeNode);
      CNEntityEditor entityEditor = nodeEditor as CNEntityEditor;
      if ( entityEditor != null )
      {
        entityEditor.CreateEntity();
      }
    }
    //----------------------------------------------------------------------------------
    public void DuplicateSelection()
    {  
      int nNodes = NumSelectedNode;
      if (nNodes > 0)
      {
        listClonedNodeAux_.Clear();
        dictNodeToClonedNodeAux_.Clear();
        
        CommandNode.DepthComparer dc = new CommandNode.DepthComparer();
        Selection.Sort(dc);

        CommandNode cloneToSelect = null;

        for (int i = 0; i < nNodes; i++)
        {
          CommandNode node = Selection[i];
          bool nodeHasToBeCloned = true;
          for (int j = i + 1; j < nNodes; j++)
          {
            CommandNode otherNode = Selection[j];
            if ( otherNode.isAncestorOf( node ) )
            {
              nodeHasToBeCloned = false;
              break;
            }
          }

          if (nodeHasToBeCloned)
          {
            CommandNode clonedNode = CloneNode( node );
            AddToDictNodeToCloned( node, clonedNode );
            listClonedNodeAux_.Add(clonedNode);
            cloneToSelect = clonedNode;
          }      
        }

        UpdateClonedNodeReferences();
        SetNodeScopes();

        FocusAndSelect(cloneToSelect);

        EditorUtility.SetDirty( fxData_.TargetGetDataHolder() );

        RebuildLists();
        SceneSelection();
      }
      else
      {
        EditorUtility.DisplayDialog("CaronteFX", "At least a node must be selected.", "Ok");
      }
    }
    //----------------------------------------------------------------------------------
    private void AddToDictNodeToCloned( CommandNode node, CommandNode clonedNode )
    {
      dictNodeToClonedNodeAux_.Add(node, clonedNode);
      
      CNGroup groupNode = node as CNGroup;
      CNGroup groupNodeCloned = clonedNode as CNGroup;
      if (groupNode != null && groupNodeCloned != null)
      {
        for (int i = 0; i < groupNode.ChildCount; i++)
        {
          CommandNode childNode = (CommandNode)groupNode.Children[i];
          CommandNode childNodeCloned = (CommandNode)groupNodeCloned.Children[i];

          AddToDictNodeToCloned( childNode, childNodeCloned );
        }      
      }
    }
    //----------------------------------------------------------------------------------
    private void MoveNodeInHierarchy( CommandNode node, CommandNode nodeTo, bool edgeDrag, ref bool updateGUI )
    {
      CNGroup originalParent = (CNGroup)node.Parent;
      if (originalParent != null)
      {
        Undo.RecordObject(originalParent, "CaronteFX - Move node in hierarchy");
      }
   
      CommandNode attachAfterNode = null;
      CNGroup newParent;

      if (edgeDrag)
      {
        int lastVisibleIdx = listCommandNodeGUI_.Count - 1;
        CommandNode lastVisibleNode = listCommandNodeGUI_[lastVisibleIdx];

        attachAfterNode = nodeTo;
        CNGroup attachAfterNodeGroup = attachAfterNode as CNGroup;

        if (attachAfterNodeGroup != null && lastVisibleNode != attachAfterNodeGroup && attachAfterNodeGroup.IsOpen)
        {
          newParent = attachAfterNodeGroup;
        }
        else
        {
          newParent = (CNGroup)attachAfterNode.Parent;
        }
      }
      else
      {
        newParent = (CNGroup)nodeTo;
      }

      Undo.RecordObject(newParent, "CaronteFX - Undo move nodes");
      Undo.RecordObject(node, "CaronteFX - Undo move nodes");

      bool isNewParent  = originalParent != newParent;
      bool isNotHimself = node != newParent;
      if ((isNewParent || edgeDrag) && isNotHimself && !node.isAncestorOf(newParent))
      {
        AttachChild(newParent, node, attachAfterNode);
        updateGUI = true;
      }
    }
    //----------------------------------------------------------------------------------
    public void AttachChild(CommandNode parent, CommandNode child, CommandNode attachAfterNode )
    {
      CNGroup childGroup     = child as CNGroup;
      bool childIsEffectRoot = childGroup != null && childGroup.IsEffectRoot;

      bool wasEnabled  = IsNodeEnabledInHierarchy(child);
      bool wasVisible  = IsNodeVisibleInHierarchy(child);
      bool wasExcluded = IsNodeExcludedInHierarchy(child);

      if ( parent.EffectRoot == child.EffectRoot || childIsEffectRoot || child.Parent == null )
      {
        child.Parent = null;
        if (attachAfterNode != null)
        {
          parent.Children.AddAfter(child, attachAfterNode);
        }
        else
        {
          parent.Children.Add(child);
        }

        if ( dictCommandNodeEditor_.ContainsKey(child) )
        {
          CommandNodeEditor childEditor  = dictCommandNodeEditor_[child];
          if (IsNodeEnabledInHierarchy(child) != wasEnabled)
          {
            childEditor.SetActivityState();
          }

          if (IsNodeVisibleInHierarchy(child) != wasVisible)
          {
            childEditor.SetVisibilityState();
          }

          if (IsNodeExcludedInHierarchy(child) != wasExcluded)
          {
            childEditor.SetExcludedState();
          }
        
          CNGroup group = parent as CNGroup;
          CommandNodeEditor parentEditor = dictCommandNodeEditor_[parent];

          if (group != null && !childIsEffectRoot)
          {
            CNGroupEditor groupEditor = (CNGroupEditor)parentEditor;
            uint scopeId = groupEditor.GetFieldId();
            childEditor.SetScopeId(scopeId);
          }
        }  

        EditorUtility.SetDirty(parent);
        EditorUtility.SetDirty(child);
      }
    }

    //----------------------------------------------------------------------------------
    private CommandNode CloneNode(CommandNode node)
    {
      CommandNode clone = node.DeepClone( node.GetDataHolder() );
      clone.Name = GetClonedUniqueNodeName(clone.Name);  
        
      CommandNode parent = (CommandNode)(node.Parent);
      BuildNodeEditorHierarchy(clone);
      CreateEntitiesInHierarchy(clone);

      AttachChild( parent, clone, node );

      return clone;
    }
    //----------------------------------------------------------------------------------
    private void UpdateClonedNodeReferences()
    {
      foreach(CommandNode clonedNode in listClonedNodeAux_)
      {
        bool wasAnyUpdate = clonedNode.UpdateNodeReferences( dictNodeToClonedNodeAux_ );
        if (wasAnyUpdate)
        {
          EditorUtility.SetDirty(clonedNode);
        }
      }
    }
    //----------------------------------------------------------------------------------
    public Texture GetNodeIcon( CommandNode node )
    {
      if ( dictCommandNodeEditor_.ContainsKey(node) ) 
      {
        CommandNodeEditor nodeEditor = dictCommandNodeEditor_[node];
        if (nodeEditor != null)
        {
          return nodeEditor.TexIcon;
        }
      }

      return null;
    }
    //----------------------------------------------------------------------------------
    public string GetUniqueNodeName(string nameRequest)
    {
      List<CRTreeNode> listTreeNode = new List<CRTreeNode>();
      rootNode_.GetHierarchyPlainList(listTreeNode);

      HashSet<string> hashNodeName = new HashSet<string>();
      foreach (CRTreeNode treeNode in listTreeNode)
      {
        CommandNode node = (CommandNode)treeNode;
        hashNodeName.Add(node.Name);
      }

      int idx = 0;
      while (hashNodeName.Contains(nameRequest + "_" + idx))
      {
        idx++;
      }

      return nameRequest + "_" + idx;
    }
    //----------------------------------------------------------------------------------
    public string GetClonedUniqueNodeName(string nameRequest)
    {
      List<CRTreeNode> listTreeNode = new List<CRTreeNode>();
      rootNode_.GetHierarchyPlainList(listTreeNode);

      HashSet<string> hashNodeName = new HashSet<string>();
      foreach (CRTreeNode treeNode in listTreeNode)
      {
        CommandNode node = (CommandNode)treeNode;
        hashNodeName.Add(node.Name);
      }

      int idx = 1;
      while (hashNodeName.Contains(nameRequest + " (" + idx + ")" ))
      {
        idx++;
      }

      return nameRequest + " (" + idx + ")";
    }
    //-----------------------------------------------------------------------------------
    public T CreateNodeUnique<T>(string name, Action<T> postcreateAction = null)
      where T : CommandNode
    {
      GameObject dataHolder = fxData_.GetDataGameObject();
     
      Undo.RecordObject(dataHolder, "CaronteFX - Create node");

      T node = CreateNode<T>(name, postcreateAction);

      bool updateGUI = false;
      if (FocusedNode != null && !IsNodeOrAncestorIsSubeffectsNode(FocusedNode))
      {
        MoveNodeInHierarchy(node, FocusedNode, true, ref updateGUI);
      }
      else
      {
        AttachChild(rootNode_, node, null);
      }

      FocusAndSelect(node);
      ExpandHierarchyUntilRoot(node);

      EditorUtility.SetDirty( dataHolder.gameObject );

      AddNodeToLists(node);
      RebuildNodeListForGUI(); 

      SceneSelection();
      RecalculateFieldsDueToUserAction();

      Undo.SetCurrentGroupName("CaronteFX - Create node");
      Undo.CollapseUndoOperations( Undo.GetCurrentGroup() );

      return node;
    }
    //-----------------------------------------------------------------------------------
    private T CreateNodeInstance<T>(GameObject dataHolder)
      where T : CRTreeNode
    {
      T instance = Undo.AddComponent<T>(dataHolder);
      instance.Parent = null;
      instance.Children = new CRTreeNodeList(instance);
      return instance;
    }
    //-----------------------------------------------------------------------------------
    private T CreateNode<T>(string name, Action<T> postcreateAction)
      where T : CommandNode
    {
      GameObject dataObject = fxData_.GetDataGameObject();

      T node = (T)CreateNodeInstance<T>(dataObject);    
      node.Name = GetUniqueNodeName(name);

      if (postcreateAction != null)
      {
        postcreateAction(node);
      }

      CommandNodeEditor nodeEditor = CreateNodeEditor(node);
      dictCommandNodeEditor_[node] = nodeEditor;

      CNEntityEditor entityEditor = nodeEditor as CNEntityEditor;
      if ( entityEditor != null )
      {
        entityEditor.CreateEntity();
      }

      return node;
    }
    //----------------------------------------------------------------------------------
    public void CreateMultiJointAreaNode()
    {
      CreateMultiJoint( CNJointGroups.CreationModeEnum.ByContact );
    }
    //----------------------------------------------------------------------------------
    public void CreateMultiJointVerticesNode()
    {
      CreateMultiJoint( CNJointGroups.CreationModeEnum.ByMatchingVertices );
    }
    //----------------------------------------------------------------------------------
    public void CreateMultiJointLeavesNode()
    {
      CreateMultiJoint( CNJointGroups.CreationModeEnum.ByStem );
    }
    //----------------------------------------------------------------------------------
    public void CreateMultiJointLocatorsNode()
    {
      CreateMultiJoint( CNJointGroups.CreationModeEnum.AtLocatorsPositions );
    }
    //----------------------------------------------------------------------------------
    public void CreateRigidGlueNode()
    {
      CreateRigidGlue();
    }
    //----------------------------------------------------------------------------------
    public void CreateServosLinearNode()
    {
      CreateServos( true, true );
    }
    //----------------------------------------------------------------------------------
    public void CreateServosAngularNode()
    {
      CreateServos( false, true );
    }
    //----------------------------------------------------------------------------------
    public void CreateMotorsLinearNode()
    {
      CreateServos( true, false );
    }
    //----------------------------------------------------------------------------------
    public void CreateMotorsAngularNode()
    {
      CreateServos( false, false );
    }
    //----------------------------------------------------------------------------------
    public void CreateFracturerUniformNode()
    {
      CreateFractureNode( CNFracture.CHOP_MODE.VORONOI_UNIFORM );
    }
    //----------------------------------------------------------------------------------
    public void CreateFracturerGeometryNode()
    {
      CreateFractureNode( CNFracture.CHOP_MODE.VORONOI_BY_GEOMETRY );
    }
    //----------------------------------------------------------------------------------
    public void CreateFracturerRadialNode()
    {
      CreateFractureNode( CNFracture.CHOP_MODE.VORONOI_RADIAL );
    }
    //-----------------------------------------------------------------------------------
    public CNRigidbody CreateIrresponsiveBodiesNode()
    {
      Action<CNRigidbody> postCreationAction = (CNRigidbody irNode) =>
      {
        irNode.IsFiniteMass = false;
        irNode.ExplosionOpacity = 1f;
        irNode.ExplosionResponsiveness = 0f;
      };

      return ( CreateNodeUnique<CNRigidbody>("IrresponsiveBodies", postCreationAction) );
    }
    //-----------------------------------------------------------------------------------
    public CNRope CreateRopeBodiesNode()
    {
      Action<CNRope> postCreationAction = (CNRope rpNode) =>
      {
        rpNode.DampingPerSecond_CM    = 100f;
        rpNode.DampingPerSecond_WORLD = 1f;
      };

      return ( CreateNodeUnique<CNRope>("RopeBodies", postCreationAction) );
    }
    //----------------------------------------------------------------------------------
    public CNFracture CreateFractureNode( CNFracture.CHOP_MODE chopMode )
    {
      Action<CNFracture> postCreationAction = (CNFracture fracturerNode) =>
      {
        fracturerNode.ChopMode = chopMode;
      };

      return ( CreateNodeUnique<CNFracture>("Fracturer", postCreationAction) );
    }
    //----------------------------------------------------------------------------------
    public CNJointGroups CreateMultiJoint(CNJointGroups.CreationModeEnum creationMode)
    {
      string name = string.Empty;
      switch (creationMode)
      {
        case CNJointGroups.CreationModeEnum.ByContact:
          name = "JointsByCloseArea";
          break;
        case CNJointGroups.CreationModeEnum.ByMatchingVertices:
          name = "JointsByCloseVertices";
          break;
        case CNJointGroups.CreationModeEnum.ByStem:
          name = "JointsByLeaves";
          break;
        case CNJointGroups.CreationModeEnum.AtLocatorsPositions:
          name = "JointsAtLocators";
          break;
      }

      Action<CNJointGroups> postCreationAction = (CNJointGroups mjNode) =>
      {
        mjNode.CreationMode = creationMode;
        if (creationMode ==CNJointGroups.CreationModeEnum.ByContact)
        {
          mjNode.BreakIfHinge = true;
        }      
      };

      return ( CreateNodeUnique<CNJointGroups>(name, postCreationAction) );
    }
    //----------------------------------------------------------------------------------
    public CNServos CreateServos(bool isLinearOrAngular, bool isPositionOrVelocity )
    {
      Action<CNServos> postCreationAction = (CNServos svNode) =>
      {
        svNode.IsLinearOrAngular    = isLinearOrAngular;
        svNode.IsPositionOrVelocity = isPositionOrVelocity;
      };

      string name;
      if (isLinearOrAngular)
      {
        if (isPositionOrVelocity)
        {
          name = "ServosLinear";
        }
        else
        {
          name = "MotorsLinear";
        }
      }
      else
      {
        if (isPositionOrVelocity)
        {
          name = "ServosAngular";
        }
        else
        {
          name = "MotorsAngular";
        }
      }  

      return ( CreateNodeUnique<CNServos>(name, postCreationAction) );
    }
    //----------------------------------------------------------------------------------
    public CNJointGroups CreateRigidGlue()
    {
      Action<CNJointGroups> postCreationAction = (CNJointGroups jgNode) =>
      {
        jgNode.CreationMode = CNJointGroups.CreationModeEnum.ByContact;
        jgNode.IsRigidGlue = true;
      };

      return ( CreateNodeUnique<CNJointGroups>("RigidGlue", postCreationAction) );
    }
    //----------------------------------------------------------------------------------
    public CNExplosion CreateExplosionNode()
    {
      Action<CNExplosion> postCreationAction = (CNExplosion exNode) =>
      {
        GameObject go = new GameObject(exNode.Name);
        exNode.Explosion_Transform = go.transform;
      };

      return ( CreateNodeUnique<CNExplosion>("Explosion", postCreationAction) );
    }
    //----------------------------------------------------------------------------------
    public CNCloth CreateClothBodiesNode()
    {
      Action<CNCloth> postCreationAction = (CNCloth clNode) =>
      {
        clNode.Density = 0.2f;
      };

      return ( CreateNodeUnique<CNCloth>("ClothBodies", postCreationAction) );
    }
    //----------------------------------------------------------------------------------
    public void SetManagerEditor( CRManagerEditor managerEditor )
    {
      managerEditor_ = managerEditor;
    }
    //----------------------------------------------------------------------------------
    public void RenameSelection(int itemIdx)
    {
      ItemIdxEditing = itemIdx;
      ItemIdxEditingName = GetItemName(itemIdx);
    }
    //----------------------------------------------------------------------------------
    public void UpdateHierarchyEffectsScope()
    {
      foreach(CommandNode node in listEffectRootNode_)
      {
        CNEffectEditor effectEditor = (CNEffectEditor)dictCommandNodeEditor_[node];
        effectEditor.ApplyEffectScope();
      }
    }
    //----------------------------------------------------------------------------------
    public void SetNodeScopes()
    {
      hashSetAlreadyVisitedNodesAux_.Clear();
      SetNodeScopes(rootNode_, uint.MaxValue);
    }
    //----------------------------------------------------------------------------------
    private void SetNodeScopes(CommandNode node, uint scopeId)
    {
      if (!HasNodeNotBeenVisitedAlready(node))
      {
        return;
      }
      CommandNodeEditor nodeEditor = GetNodeEditor(node);
      nodeEditor.SetScopeId(scopeId);
      
      CNGroup groupNode = node as CNGroup;
      if ( groupNode != null )
      {
        CNGroupEditor groupEditor = (CNGroupEditor)nodeEditor;
        scopeId = groupEditor.GetFieldId();

        for (int i = 0; i < groupNode.ChildCount; ++i)
        {
          CommandNode childNode = (CommandNode)groupNode.Children[i];
          SetNodeScopes(childNode, scopeId);
        }
      }
    }
    //----------------------------------------------------------------------------------
    public void SceneSelection()
    {
      GUI.FocusControl("");
        
      SelectionWithChildren.Clear();

      CRTreeNode.CRTreeTraversalDelegate addNodesIfNotIncluded = (treeNode) => 
      { 
        if (!SelectionWithChildren.Contains( (CommandNode)treeNode ) )
        {
          SelectionWithChildren.Add( (CommandNode)treeNode );
        }
      };

      hashSetAlreadyVisitedNodesAux_.Clear();
      foreach( CommandNode node in Selection )
      {
        if ( node != null)
        {
          node.Traversal( addNodesIfNotIncluded, HasNodeNotBeenVisitedAlready );     
        }
      }

      entityManager_.GetListIdMultiJointFromListNodes( SelectionWithChildren, 
                                                       fxData_.listJointGroupsIdsSelected_, 
                                                       fxData_.listRigidGlueIdsSelected_ );
      SceneView.RepaintAll();
      managerEditor_.Repaint();
    }
    //----------------------------------------------------------------------------------
    public void FocusAndSelect(CommandNode node)
    {    
      Selection.Clear();
      Selection.Add(node);
      LastSelectedNode = node;
      FocusedNode = node;
    }
    //----------------------------------------------------------------------------------
    public void ExpandHierarchyUntilRoot(CommandNode node)
    {
      CNGroup parent = (CNGroup)node.Parent;
      while (parent != rootNode_)
      {
        parent.IsOpen = true;
        EditorUtility.SetDirty(parent);
        parent = (CNGroup)parent.Parent;
      }
    }
    //----------------------------------------------------------------------------------
    public void ContextClick()
    {
      SelectionFlags selectionFlags;
      CalculateSelectionFlags(out selectionFlags);
      CreateContextMenu(selectionFlags);
    }
    //----------------------------------------------------------------------------------
    private void CalculateSelectionFlags( out SelectionFlags selectionFlags )
    {
      if (NumSelectedNode == 0 )
      {
        selectionFlags = SelectionFlags.None;
        return;
      }  

      selectionFlags = SelectionFlags.All;
      foreach (CommandNode selectedNode in Selection)
      {
        if (selectedNode.IsNodeExcluded)
        {
          selectionFlags &= ~SelectionFlags.Included;
        }
        else
        {
          selectionFlags &= ~SelectionFlags.Excluded;
        }

        if (selectedNode.IsNodeEnabled)
        {
          selectionFlags &= ~SelectionFlags.Disabled;
        }
        else
        {
          selectionFlags &= ~SelectionFlags.Enabled;
        }

        if (selectedNode.IsNodeVisible)
        {
          selectionFlags &= ~SelectionFlags.Hidden;
        }
        else
        {
          selectionFlags &= ~SelectionFlags.Visible;
        }

        CNGroup groupNode = selectedNode as CNGroup;
        if ( groupNode != null )
        {
            if ( groupNode.IsEffectRoot || 
                 groupNode.IsSubeffectsFolder )
            {
              selectionFlags &= ~SelectionFlags.Deletable;
              selectionFlags &= ~SelectionFlags.Renamable;
              selectionFlags &= ~SelectionFlags.Clonable; 
            }    
        }
        else
        {
          selectionFlags &= ~SelectionFlags.Group;
        }

        CNBody  bodyNode = selectedNode as CNBody;
        if (bodyNode == null)
        {
          selectionFlags &= ~SelectionFlags.Body;
        }

        CNRigidbody rigidNode = selectedNode as CNRigidbody;
        CNAnimatedbody animatedNode = selectedNode as CNAnimatedbody;
        if (rigidNode == null || animatedNode != null)
        {
          selectionFlags &= ~SelectionFlags.Irresponsive;
          selectionFlags &= ~SelectionFlags.Responsive;
        }
        else
        {
          if (rigidNode.IsFiniteMass)
          {
            selectionFlags &= ~SelectionFlags.Irresponsive;
          }
          else
          {
            selectionFlags &= ~SelectionFlags.Responsive;
          }
        }

        CNJointGroups jointGroups = selectedNode as CNJointGroups;
        if (jointGroups == null)
        {
          selectionFlags &= ~SelectionFlags.MultiJoint;
        }
      }
    }
    //----------------------------------------------------------------------------------
    private void CreateContextMenu(SelectionFlags selectionFlags)
    {
      bool none            = (selectionFlags == SelectionFlags.None);

      bool deletable       = (selectionFlags & SelectionFlags.Deletable)    == SelectionFlags.Deletable;
      bool renamable       = (selectionFlags & SelectionFlags.Renamable)    == SelectionFlags.Renamable;
      bool clonable        = (selectionFlags & SelectionFlags.Clonable)     == SelectionFlags.Clonable;

      bool allExcluded     = (selectionFlags & SelectionFlags.Excluded)     == SelectionFlags.Excluded;
      bool allEnabled      = (selectionFlags & SelectionFlags.Enabled)      == SelectionFlags.Enabled;
      bool allDisabled     = (selectionFlags & SelectionFlags.Disabled)     == SelectionFlags.Disabled;
      bool allVisible      = (selectionFlags & SelectionFlags.Visible)      == SelectionFlags.Visible;
      bool allHidden       = (selectionFlags & SelectionFlags.Hidden)       == SelectionFlags.Hidden;
      bool allGroup        = (selectionFlags & SelectionFlags.Group)        == SelectionFlags.Group;
      bool allBody         = (selectionFlags & SelectionFlags.Body)         == SelectionFlags.Body;
      bool allResponsive   = (selectionFlags & SelectionFlags.Responsive)   == SelectionFlags.Responsive;
      bool allIrresponsive = (selectionFlags & SelectionFlags.Irresponsive) == SelectionFlags.Irresponsive;

      int focusedNodeIdx = listCommandNodeGUI_.IndexOf(FocusedNode);
      GenericMenu menu = new GenericMenu();

      bool blockEdition = manager_.BlockEdition;

      if (blockEdition)
      {
        return;
      }

      if (none)
      {
        managerEditor_.FillNodeMenu(blockEdition, menu, true);
      }
      else
      {
        int nSelectedNodes = NumSelectedNode;

        if (allExcluded)
        {
          menu.AddItem( new GUIContent("Include"), false, () => { SetExcludeStateSelection(false); } );
          menu.AddItem( new GUIContent("Include and enable GameObjects"), false, () => { SetExcludeStateSelection(false); SetSelectionGameObjectsVisibility(true); } );
        }
        else
        {
          menu.AddItem( new GUIContent("Exclude"), false, () => { SetExcludeStateSelection(true); } );
          menu.AddItem( new GUIContent("Exclude and disable GameObjects"), false, () => { SetExcludeStateSelection(true); SetSelectionGameObjectsVisibility(false); } );
        }

        menu.AddSeparator("");

        if (allEnabled)
        {
           menu.AddItem( new GUIContent("Enable Off"), false, () => { SetEnableStateSelection(false); } );   
        }
        else if (allDisabled)
        {
           menu.AddItem( new GUIContent("Enable On"), false, () => { SetEnableStateSelection(true); } );   
        }
        else
        {
          menu.AddDisabledItem( new GUIContent("Enable") );
        }

        if (allHidden)
        {
          menu.AddItem( new GUIContent("Visible On"), false, () => { SetVisibilityStateSelection(true); } );
        }
        else if (allVisible)
        {
          menu.AddItem( new GUIContent("Visible Off"), false, () => { SetVisibilityStateSelection(false); } );
        }
        else
        {
          menu.AddDisabledItem( new GUIContent( "Visible") );
        }
        menu.AddSeparator("");
   

        if(allGroup || allBody)
        {
          menu.AddItem( new GUIContent("Enable GameObjects"),  false, () => { SetSelectionGameObjectsVisibility(true);  } );
          menu.AddItem( new GUIContent("Disable GameObjects"), false, () => { SetSelectionGameObjectsVisibility(false); } );  
          menu.AddSeparator("");
          menu.AddItem( new GUIContent("Select GameObjects"), false, () => { SelectAllBodyAndFracturerGameObjects(); } );
          menu.AddSeparator("");
        }

        if (clonable)
        {
          menu.AddItem(new GUIContent("Duplicate"), false, DuplicateSelection);
        }
        else
        {
          menu.AddDisabledItem(new GUIContent("Duplicate"));
        }


        if ( renamable &&  nSelectedNodes == 1 )
        {
          menu.AddItem(new GUIContent("Rename"), false, () => { RenameSelection(focusedNodeIdx); });     
        }
        else
        {
          menu.AddDisabledItem(new GUIContent("Rename"));
        }

        if (deletable)
        {
          menu.AddItem(new GUIContent("Delete"), false, RemoveSelected);
        }
        else
        {
          menu.AddDisabledItem(new GUIContent("Delete"));
        }

        if (allResponsive)
        {
          menu.AddSeparator("");
          menu.AddItem(new GUIContent("Change to irresponsive"), false, () => { SetResponsiveness( Selection, false ); } );
        }     
        else if (allIrresponsive)
        {
          menu.AddSeparator("");
          menu.AddItem(new GUIContent("Change to responsive"), false, () => { SetResponsiveness( Selection, true ); } );
        }
      }

      menu.ShowAsContext();
    }
    //----------------------------------------------------------------------------------
    public void ModifyEffectsIncluded(List<GameObject> listCaronteFXGameObjectToDeinclude, List<GameObject> listCaronteFxGameObjectToInclude)
    {
      List<CommandNode> listCommandNodeAux = new List<CommandNode>();
      foreach (GameObject go in listCaronteFXGameObjectToDeinclude)
      {
        Caronte_Fx _fxData = go.GetComponent<Caronte_Fx>();
        CNGroup    _fxDataRoot = _fxData.e_getRootNode();

        if ( listEffectRootNode_.Contains(_fxDataRoot) )
        {
          _fxDataRoot.Parent = null;
          EditorUtility.SetDirty(_fxDataRoot);
        }

        listCommandNodeAux.Clear();
        GetAllNodes(_fxDataRoot, listCommandNodeAux);
    
        foreach(CommandNode node in listCommandNodeAux)
        {
          CommandNodeEditor nodeEditor = GetNodeEditor(node);
          listCommandNodeEditor_.Remove(nodeEditor);
        }

        //Remove deleted nodes from other fields
        foreach (CommandNode node in listCommandNodeAux)
        {
          RemoveNodeFromFields(node);    
        }
      }

      foreach (GameObject go in listCaronteFxGameObjectToInclude)
      {
        Caronte_Fx _fxData = go.GetComponent<Caronte_Fx>();
        CNGroup    _fxDataRoot = _fxData.e_getRootNode();

        if ( rootNode_ == _fxDataRoot                     ||
             listEffectRootNode_.Contains(_fxDataRoot)    )
        {
          continue;
        }

        AttachChild(subeffectsNode_, _fxDataRoot, null);
      }

      EditorUtility.SetDirty(subeffectsNode_);
      
      manager_.Deinit();
      manager_.Init();
    }
    //----------------------------------------------------------------------------------
    private void RecalculateFieldsStart()
    {
      FieldManager.RecalculateFields();
      foreach (CommandNodeEditor cnEditor in listCommandNodeEditor_)
      {
        cnEditor.StoreInfo();
      }
    }
    //----------------------------------------------------------------------------------
    public void RecalculateFieldsAutomatic()
    {
      if ( SimulationManager.IsEditing() )
      {
        RemoveNullNodes();

        UpdateHierarchyEffectsScope();
        UpdateFields();
        UpdateFieldLists();

        managerEditor_.RepaintSubscribers();
      }
    }
    //----------------------------------------------------------------------------------
    public void RecalculateFieldsDueToUserAction()
    {
      if ( SimulationManager.IsEditing() )
      {
        RemoveNullNodes();

        foreach( CommandNode node in ListCommandNode )
        {
          Undo.RecordObject(node, "Node field modified");
        }

        UpdateHierarchyEffectsScope();
        UpdateFields();
        UpdateFieldLists();

        managerEditor_.RepaintSubscribers();
      }
    }
    //----------------------------------------------------------------------------------
    private void UpdateFields()
    {
      FieldManager.RecalculateFields();

      foreach (CommandNodeEditor cnEditor in listCommandNodeEditor_)
      {
        cnEditor.StoreInfo();
      }

      foreach (CNBodyEditor bodyEditor in listBodyEditor_)
      {
        int[] idsUnityAdded, idsUnityRemoved;
        bodyEditor.CheckUpdate(out idsUnityAdded, out idsUnityRemoved);
        bodyEditor.DestroyBodies(idsUnityRemoved);    
      }

      foreach (CNJointGroupsEditor mjEditor in listMultiJointEditor_)
      {
        mjEditor.CheckUpdate();
      }

      foreach (CNBodyEditor bodyEditor in listBodyEditor_)
      {
        int[] idsUnityAdded, idsUnityRemoved;
        bodyEditor.CheckUpdate(out idsUnityAdded, out idsUnityRemoved);

        GameObject[] arrGoAdded = CRGUIUtils.GetGameObjectsFromIds(idsUnityAdded);
        bodyEditor.CreateBodies(arrGoAdded);
      }

      EditorUtility.ClearProgressBar();
    }
    //-----------------------------------------------------------------------------------
    public void UpdateFieldLists()
    {
      foreach (CommandNodeEditor cnEditor in listCommandNodeEditor_)
      {
        cnEditor.BuildListItems();
      }

      if (CNFieldWindow.IsOpen)
      {
        CNFieldWindow.Update();
      }
    }
    //-----------------------------------------------------------------------------------
    public void RemoveNodeDelayed(CommandNode node)
    {
      listNodesToDeleteDeferred_.Add(node);
    }
    //-----------------------------------------------------------------------------------
    private void AddToRemoveNodeDefeerredList(List<CommandNode> listNodes)
    {
      listNodesToDeleteDeferred_.AddRange(listNodes);
    }
    //-----------------------------------------------------------------------------------
    public void RemoveNodesDefeerred()
    {
      int nNodesToRemove = listNodesToDeleteDeferred_.Count;

      if (nNodesToRemove > 0)
      {
        List<CommandNode> listNodeToDelete = new List<CommandNode>();
        CreateFullNodeList(rootNode_, listNodesToDeleteDeferred_, listNodeToDelete, false);

        listNodesToDeleteDeferred_.Clear();
        Undo.RecordObject(fxData_, "CaronteFX - Remove nodes");

        //Remove deleted nodes from other fields
        foreach (CommandNode node in listNodeToDelete)
        {
          CommandNode parent = (CommandNode)node.Parent;

          Undo.RecordObject(parent, "CaronteFX - Set parent node null");
          Undo.RecordObject(node, "CaronteFX - Set parent node null");

          RemoveNodeFromFields(node);    
        }

        //Destroy node and editor
        foreach (CommandNode node in listNodeToDelete)
        {
          RemoveNodeEditor(node);

          CRTreeNode parent = node.Parent;
          node.Parent = null;
          if (parent != null)
          {
            EditorUtility.SetDirty(parent);
          }

          Undo.DestroyObjectImmediate(node);
        }
        
        Undo.SetCurrentGroupName("CaronteFX - Remove nodes");
        Undo.CollapseUndoOperations( Undo.GetCurrentGroup() );

        EditorUtility.ClearProgressBar();

        RemoveNullNodes();
        RebuildLists();
        UnselectSelected();
      }
    }
    //-----------------------------------------------------------------------------------
    private void RemoveNodeFromFields(CommandNode node)
    {
      foreach (CommandNodeEditor nodeEditor in listCommandNodeEditor_)
      {
        nodeEditor.RemoveNodeFromFields(node);
      }
    }
    //----------------------------------------------------------------------------------
    private void CreateFullNodeList(CommandNode rootSearchNode, List<CommandNode> listItems, List<CommandNode> listItemsFull, bool prospectHierarchy)
    {
      if ( prospectHierarchy || listItems.Contains(rootSearchNode) )
      {
        prospectHierarchy = true;
        if ( !listItemsFull.Contains(rootSearchNode) )
        {
          listItemsFull.Add(rootSearchNode);
        }
      }

      CNGroup groupNode = rootSearchNode as CNGroup;
      if (groupNode != null)
      {
        for (int i = 0; i < groupNode.ChildCount; ++i)
        {
          CommandNode childNode = (CommandNode)groupNode.Children[i];
          CreateFullNodeList(childNode, listItems, listItemsFull, prospectHierarchy);
        }
      }
    }
    //----------------------------------------------------------------------------------
    private void GetAllNodes(CommandNode rootSearchNode, List<CommandNode> listAllNodes)
    {
      listAllNodes.Add(rootSearchNode);

      CNGroup groupNode = rootSearchNode as CNGroup;
      if (groupNode != null)
      {
        for (int i = 0; i < groupNode.ChildCount; ++i)
        {
          CommandNode childNode = (CommandNode)groupNode.Children[i];
          GetAllNodes(childNode, listAllNodes);
        }
      }
    }
    //----------------------------------------------------------------------------------
    public void SetActivityState(CRTreeNodeList listTreeNode)
    {
      foreach (CRTreeNode treeNode in listTreeNode )
      {
        CommandNode node = treeNode as CommandNode;
        CommandNodeEditor nodeEditor = GetNodeEditor(node);

        nodeEditor.SetActivityState();
      }
    }
    //----------------------------------------------------------------------------------
    public void SetVisibilityState(CRTreeNodeList listTreeNode)
    {
      foreach (CRTreeNode treeNode in listTreeNode )
      {
        CommandNode node = treeNode as CommandNode;
        CommandNodeEditor nodeEditor = GetNodeEditor(node);

        nodeEditor.SetVisibilityState();
      }
    }
    //----------------------------------------------------------------------------------
    public void SetExcludedState(CRTreeNodeList listTreeNode)
    {
      foreach (CRTreeNode treeNode in listTreeNode )
      {
        CommandNode node = treeNode as CommandNode;
        CommandNodeEditor nodeEditor = GetNodeEditor(node);

        nodeEditor.SetExcludedState();
      }
    }
    //----------------------------------------------------------------------------------
    public void SetEnableStateSelection(bool enabled)
    {
      List<CommandNode> listSelectedNodeFull = new List<CommandNode>();
      List<CommandNode> listSelectedNode = Selection;
      CreateFullNodeList( rootNode_, listSelectedNode, listSelectedNodeFull, false );

      foreach (CommandNode node in listSelectedNode)
      {
        node.IsNodeEnabled = enabled;
      }

      SceneView.RepaintAll();
    }
    //----------------------------------------------------------------------------------
    public void SetVisibilityStateSelection(bool visible)
    {
      List<CommandNode> listSelectedNode = Selection;

      foreach (CommandNode node in listSelectedNode)
      {
        node.IsNodeVisible = visible;
      }

      foreach (CommandNode node in listSelectedNode)
      {
        CommandNodeEditor nodeEditor = GetNodeEditor(node);
        nodeEditor.SetVisibilityState();
      }

      SceneView.RepaintAll();
    }
    //----------------------------------------------------------------------------------
    public void SetExcludeStateSelection(bool excluded)
    {
      List<CommandNode> listSelectedNodeFull = new List<CommandNode>();
      List<CommandNode> listSelectedNode = Selection;

      CreateFullNodeList( rootNode_, listSelectedNode, listSelectedNodeFull, false );
      foreach (CommandNode node in listSelectedNode)
      {
        node.IsNodeExcluded = excluded;
        EditorUtility.SetDirty(node);
      }

      List<CommandNode> listBodyNode            = new List<CommandNode>();
      List<CommandNode> listNotBodyNotGroupNode = new List<CommandNode>();
      List<CommandNode> listGroupNode           = new List<CommandNode>();


      // Groups and not processed to avoid duplicated SetExcludedState due to recursion
      foreach(CommandNode node in listSelectedNodeFull)
      {
        if ( node is CNBody )
        {
          listBodyNode.Add( node );
        }
        else if ( node is  CNGroup )
        {
          listGroupNode.Add( node );   
        }
        else
        {
          listNotBodyNotGroupNode.Add( node );
        }
      }

      foreach (CommandNode node in listBodyNode)
      {
        CommandNodeEditor nodeEditor = GetNodeEditor(node);
        nodeEditor.SetExcludedState();
      }

      foreach (CommandNode node in listGroupNode)
      {
        CommandNodeEditor nodeEditor = GetNodeEditor(node);
        nodeEditor.ResetState();
      }

      foreach (CommandNode node in listNotBodyNotGroupNode)
      {
        CommandNodeEditor nodeEditor = GetNodeEditor(node);
        nodeEditor.SetExcludedState();
      }

      managerEditor_.RepaintSubscribers();
      SceneView.RepaintAll();
    }
    //----------------------------------------------------------------------------------
    private void SetSelectionGameObjectsVisibility(bool active)
    {
      List<CommandNode> listSelectedNodeFull = new List<CommandNode>();
      List<CommandNode> listSelectedNode = Selection;

      CreateFullNodeList( rootNode_, listSelectedNode, listSelectedNodeFull, false );

      List<CommandNode> listBodyNode    = new List<CommandNode>();

      foreach(CommandNode node in listSelectedNodeFull)
      {
        if ( node is CNBody )
        {
          listBodyNode.Add( node );
        }
      }

      
      Undo.RecordObject( fxData_, "Change enable state of GameObjects " + fxData_.name );
      foreach (CommandNode node in listBodyNode)
      {
        CNBodyEditor nodeEditor = (CNBodyEditor)GetNodeEditor(node);
        List<GameObject> listBodyNodeGameObjects = nodeEditor.GetGameObjects();

        foreach(GameObject go in listBodyNodeGameObjects)
        { 
          Undo.RecordObject(go, "Change enable state " + go.name );
          go.SetActive(active);
          EditorUtility.SetDirty(go);
        }
      }

      Undo.CollapseUndoOperations( Undo.GetCurrentGroup() );
    }
    //----------------------------------------------------------------------------------
    private void SelectAllBodyAndFracturerGameObjects()
    {
      List<CommandNode> listSelectedNodeFull = new List<CommandNode>();
      List<CommandNode> listSelectedNode = SelectionWithChildren;

      CreateFullNodeList( rootNode_, listSelectedNode, listSelectedNodeFull, false );

      List<CommandNode> listMonoField = new List<CommandNode>();

      foreach(CommandNode node in listSelectedNodeFull)
      {
        if ( node is CNBody || node is CNFracture )
        {
          listMonoField.Add( node );
        }
      }

      List<GameObject> listWholeObjects = new List<GameObject>();

      foreach (CommandNode node in listMonoField)
      {
        CNMonoFieldEditor nodeEditor = (CNMonoFieldEditor)GetNodeEditor(node);
        listWholeObjects.AddRange( nodeEditor.GetGameObjectsTopMost() );
      }

      UnityEditor.Selection.objects = listWholeObjects.ToArray();
    }
    //----------------------------------------------------------------------------------
    private void AddDraggedObjects(CommandNode nodeTo, UnityEngine.Object[] arrGameObject)
    {
      if (nodeTo == null)
      {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create rigidbodies from selection"), false, () =>
        {
          CNRigidbody node = CreateNodeUnique<CNRigidbody>("RigidBodies");
          CommandNodeEditor cnEditor = dictCommandNodeEditor_[node];
          nodeEditorFactory_.ApplyDragAction(node, cnEditor, arrGameObject);
        });

        menu.AddItem(new GUIContent("Create irresponsives from selection"), false, () =>
        {
          CNRigidbody node = CreateIrresponsiveBodiesNode();
          CommandNodeEditor cnEditor = dictCommandNodeEditor_[node];
          nodeEditorFactory_.ApplyDragAction(node, cnEditor, arrGameObject);
        });

        menu.AddItem(new GUIContent("Create animated from selection"), false, () =>
        {
          CNAnimatedbody node = CreateNodeUnique<CNAnimatedbody>("AnimatedBodies");
          CommandNodeEditor cnEditor = dictCommandNodeEditor_[node];
          nodeEditorFactory_.ApplyDragAction(node, cnEditor, arrGameObject);
        });


        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Create softbodies from selection"), false, () =>
        {
          CNSoftbody node = CreateNodeUnique<CNSoftbody>("SoftBodies");
          CommandNodeEditor cnEditor = dictCommandNodeEditor_[node];
          nodeEditorFactory_.ApplyDragAction(node, cnEditor, arrGameObject);
        });

        menu.AddItem(new GUIContent("Create cloths from selection"), false, () =>
        {
          CNCloth node = CreateClothBodiesNode();
          CommandNodeEditor cnEditor = dictCommandNodeEditor_[node];
          nodeEditorFactory_.ApplyDragAction(node, cnEditor, arrGameObject);
        });

        menu.AddItem(new GUIContent("Create ropes from selection"), false, () =>
        {
          CNRope node = CreateRopeBodiesNode();
          CommandNodeEditor cnEditor = dictCommandNodeEditor_[node];
          nodeEditorFactory_.ApplyDragAction(node, cnEditor, arrGameObject);
        });

        menu.ShowAsContext();
      }
      else
      {
        CommandNodeEditor cnEditor = dictCommandNodeEditor_[nodeTo];
        nodeEditorFactory_.ApplyDragAction(nodeTo, cnEditor, arrGameObject);
      }
    }
    //----------------------------------------------------------------------------------
    public List<CommandNodeEditor> GetEditorsFromHierarchy<T>(CommandNode node)
    {
      listTreeNodeAux_.Clear();
      listCommandNodeEditorAux_.Clear();

      node.GetHierarchyPlainList(listTreeNodeAux_);

      foreach( CRTreeNode treeNode in listTreeNodeAux_ )
      {
        CommandNode cNode = (CommandNode)treeNode;

        CommandNodeEditor cnEditor = GetNodeEditor(cNode);
        if (cnEditor is T)
        {
          listCommandNodeEditorAux_.Add(cnEditor);
        }
      }

      return listCommandNodeEditorAux_;
    }
    //----------------------------------------------------------------------------------
    private void RemoveNullNodes()
    {
      hashSetAlreadyVisitedNodesAux_.Clear();
      RemoveNullNodes(rootNode_);
    }
    //----------------------------------------------------------------------------------
    private void RemoveNullNodes(CommandNode node)
    {
      CNGroup groupNode = node as CNGroup;
      if (groupNode != null)
      {
        if ( hashSetAlreadyVisitedNodesAux_.Contains(node) )
        {
          return;
        }
        hashSetAlreadyVisitedNodesAux_.Add(node);

        CRTreeNodeList listChild = groupNode.Children;
        int lastChildNodeIdx = listChild.Count - 1;

        for (int i = lastChildNodeIdx; i >= 0; i--)
        {
          CommandNode childNode = (CommandNode)listChild[i];
          if (childNode == null)
          {
            listChild.RemoveAt(i);
          }
          else
          {
            RemoveNullNodes(childNode);
          }
        }
      }
    }
    //----------------------------------------------------------------------------------
    private bool CheckForDestroyedNodes(List<CommandNodeEditor> listCommandNodeEditorToDestroy)
    {
      bool anyDestroyed = false;
      
      Dictionary<CommandNode, CommandNodeEditor> auxDict = new Dictionary<CommandNode,CommandNodeEditor>();
 
      foreach (var nodeEditorPair in dictCommandNodeEditor_)
      {
        CommandNode node             = nodeEditorPair.Key;
        CommandNodeEditor nodeEditor = nodeEditorPair.Value;

        if (node == null)
        {
          anyDestroyed = true;
          listCommandNodeEditorToDestroy.Add(nodeEditor);
        }
        else
        {
          auxDict[node] = nodeEditor;
        }
      }

      if (anyDestroyed)
      {
        dictCommandNodeEditor_ = auxDict;
      }

      return anyDestroyed;
    }
    //----------------------------------------------------------------------------------
    private bool CheckForNodesWithoutEditor(List<CommandNode> listCommandNodeToCreate)
    {
      bool isAnyNodeWithoutEditor = false;
      CheckForNodesWithoutEditor(rootNode_, ref isAnyNodeWithoutEditor, listCommandNodeToCreate);

      return isAnyNodeWithoutEditor;
    }
    //----------------------------------------------------------------------------------
    private void CheckForNodesWithoutEditor(CommandNode node, ref bool isAnyNodeWithoutEditor, List<CommandNode> listCommandNodeToCreate)
    {   
      if (!dictCommandNodeEditor_.ContainsKey(node) )
      {
        isAnyNodeWithoutEditor |= true;
        listCommandNodeToCreate.Add(node);
      }

      CNGroup groupNode = node as CNGroup;
      if (groupNode != null)
      {
        CRTreeNodeList listChild = groupNode.Children;
        int lastChildNodeIdx = listChild.Count - 1;

        for (int i = lastChildNodeIdx; i >= 0; i--)
        {
          CommandNode childNode = (CommandNode)listChild[i];
          CheckForNodesWithoutEditor(childNode, ref isAnyNodeWithoutEditor, listCommandNodeToCreate);
        }
      }
    }
    //----------------------------------------------------------------------------------
    public void CreateFromUndo(List<CommandNodeEditor> listCommandNodeEditor)
    {
      List<CNBodyEditor>        listBodyEditor        = new List<CNBodyEditor>();
      List<CNJointGroupsEditor> listJointGroupsEditor = new List<CNJointGroupsEditor>();
      List<CNServosEditor>      listServosEditor      = new List<CNServosEditor>();
      List<CNEntityEditor>      listEntityEditor      = new List<CNEntityEditor>();

      foreach (CommandNodeEditor nodeEditor in listCommandNodeEditor)
      {
        CNBodyEditor bodyEditor = nodeEditor as CNBodyEditor;
        if (bodyEditor != null)
        {
          listBodyEditor.Add(bodyEditor);
        }

        CNJointGroupsEditor jgEditor = nodeEditor as CNJointGroupsEditor;
        if (jgEditor != null)
        {
          listJointGroupsEditor.Add(jgEditor);
        }

        CNServosEditor svEditor = nodeEditor as CNServosEditor;
        if (svEditor != null)
        {
          listServosEditor.Add(svEditor);
        }

        CNEntityEditor entityEditor = nodeEditor as CNEntityEditor;
        if (entityEditor != null)
        {
          listEntityEditor.Add(entityEditor);
        }
      }

      foreach( CNBodyEditor bodyEditor in listBodyEditor )
      {
        bodyEditor.CreateBodies();
      }

      foreach( CNJointGroupsEditor jgEditor in listJointGroupsEditor )
      {
        jgEditor.CreateEntities();
      }

      foreach( CNServosEditor svEditor in listServosEditor )
      {
        svEditor.CreateEntities();
      }

      foreach( CNEntityEditor entityEditor in listEntityEditor )
      {
        entityEditor.CreateEntity();
      }
    }
    //----------------------------------------------------------------------------------
    public void CheckHierarchyUndoRedo()
    {
      if (fxData_ != null)
      {
        RemoveNullNodes();

        bool anyCreated = false;
        bool anyDestroyed = false;

        List<CommandNode>       listCommmandNodeToCreate      = new List<CommandNode>();
        List<CommandNodeEditor> listCommanNodeEditorToDestroy = new List<CommandNodeEditor>();

        anyCreated   = CheckForNodesWithoutEditor(listCommmandNodeToCreate);
        anyDestroyed = CheckForDestroyedNodes(listCommanNodeEditorToDestroy);

        if (anyCreated)
        {
          List<CommandNodeEditor> listCommandNodeEditor = new List<CommandNodeEditor>();
          foreach( CommandNode node in listCommmandNodeToCreate )
          {
            CommandNodeEditor nodeEditor = CreateNodeEditor(node);
            dictCommandNodeEditor_[node] = nodeEditor;

            listCommandNodeEditor.Add(nodeEditor);
          }

          CreateFromUndo(listCommandNodeEditor);
        }
        else if (anyDestroyed)
        {
          foreach( CommandNodeEditor nodeEditor in listCommanNodeEditorToDestroy )
          {
            nodeEditor.FreeResources();
          }
        }

        SetNodeScopes();
        RebuildLists();
        ValidateEditors();
        SceneSelection();

        foreach (CommandNodeEditor cnEditor in listCommandNodeEditor_)
        {
          cnEditor.LoadInfo();
        }

        if (CNFieldWindow.IsOpen)
        {
          CNFieldWindow.Update();
        }

        manager_.CustomHierarchyChange();
      }
    }

    public void ValidateEditors()
    {
      List<CommandNodeEditor> listCommandNodeEditor = ListCommandNodeEditor;
      foreach (CommandNodeEditor cnEditor in listCommandNodeEditor)
      {
        cnEditor.ValidateState();
      }
    }

  } //CNHierarchy...

} //CaronteFX...

