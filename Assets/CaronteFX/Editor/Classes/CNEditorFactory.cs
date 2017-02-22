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
  public class CNEditorFactory
  {
    private CNHierarchy hierarchy_;
       
    private delegate CommandNodeEditor CRCommandNodeEditorCreationDel(CommandNode node);
    private Dictionary<Type, CRCommandNodeEditorCreationDel> @switchNodeEditor;

    private delegate void CNCommandNodeDragAction(CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects);
    private Dictionary<Type, CNCommandNodeDragAction> @switchDragAction;

    public CNEditorFactory(CNHierarchy hierarchy)
    {
      hierarchy_ = hierarchy;
      InitDictionaries();
    }

    public bool CanCrateEditor(CommandNode node)
    {
      Type nodeType = node.GetType();
      return @switchNodeEditor.ContainsKey(nodeType);
    }

    public CommandNodeEditor CreateNodeEditor(CommandNode node)
    {
      Type nodeType = node.GetType();
      return @switchNodeEditor[nodeType](node);
    }

    public void ApplyDragAction(CommandNode node, CommandNodeEditor nodeEditor, UnityEngine.Object[] arrDraggedObjects)
    {
      Type nodeType = node.GetType();
      @switchDragAction[nodeType](nodeEditor, arrDraggedObjects);
    }

    private void InitDictionaries()
    {
      @switchNodeEditor = new Dictionary<Type, CRCommandNodeEditorCreationDel> {

              { typeof(CNRigidbody),        (CommandNode node) => 
                                            {
                                              CNRigidbody       rbNode   = (CNRigidbody) node;
                                              CNRigidbodyEditor rbEditor = new CNRigidbodyEditor(rbNode, new CNRigidbodyEditorState());
                                              rbEditor.Init();
                                              return rbEditor;
                                            } },

              { typeof(CNAnimatedbody),     (CommandNode node) =>
                                            {
                                              CNAnimatedbody animationNode = (CNAnimatedbody) node;
                                              CNAnimatedbodyEditor animatedEditor = new CNAnimatedbodyEditor(animationNode, new CNRigidbodyEditorState());
                                              animatedEditor.Init();
                                              return animatedEditor;
                                            } },

              { typeof(CNSoftbody),         (CommandNode node) => 
                                            {
                                              CNSoftbody sbNode = (CNSoftbody) node;
                                              CNSoftbodyEditor sbEditor = new CNSoftbodyEditor(sbNode, new CNSoftbodyEditorState());
                                              sbEditor.Init();
                                              return sbEditor;
                                            } },

              { typeof(CNCloth),            (CommandNode node) => 
                                            {
                                              CNCloth clNode = (CNCloth) node;
                                              CNClothEditor clEditor = new CNClothEditor(clNode, new CNClothEditorState());
                                              clEditor.Init();
                                              return clEditor;
                                            } },

              { typeof(CNRope),            (CommandNode node) => 
                                            {
                                              CNRope rpNode = (CNRope) node;
                                              CNRopeEditor rpEditor = new CNRopeEditor(rpNode, new CNRopeEditorState());
                                              rpEditor.Init();
                                              return rpEditor;
                                            } },

              { typeof(CNJointGroups),      (CommandNode node) => 
                                            {
                                              CNJointGroups jgNode = (CNJointGroups) node;
                                              if (jgNode.IsRigidGlue)
                                              {
                                                CNRigidGlueEditor rgEditor = new CNRigidGlueEditor(jgNode, new CommandNodeEditorState());
                                                rgEditor.Init();
                                                return rgEditor;
                                              }
                                              else
                                              {
                                                CNJointGroupsEditor jgEditor = new CNJointGroupsEditor(jgNode, new CommandNodeEditorState());
                                                jgEditor.Init();
                                                return jgEditor;
                                              }               
                                            } },

              { typeof(CNServos),           (CommandNode node) => 
                                            {
                                              CNServos svNode = (CNServos) node;
                                              CNServosEditor svEditor = new CNServosEditor(svNode, new CommandNodeEditorState());
                                              svEditor.Init();
                                              return svEditor;  
                                            } },

              { typeof(CNGroup),            (CommandNode node) => 
                                            {
                                              CNGroup groupNode = (CNGroup) node;
                                              if (groupNode.IsEffectRoot)
                                              {
                                                if ( hierarchy_.RootNode == groupNode )
                                                {
                                                  CNEffectExtendedEditor fxEditor = new CNEffectExtendedEditor(groupNode, new CommandNodeEditorState());
                                                  fxEditor.Init();
                                                  return fxEditor;
                                                }
                                                else
                                                {
                                                  CNEffectEditor fxEditor = new CNEffectEditor(groupNode, new CommandNodeEditorState());
                                                  fxEditor.Init();
                                                  return fxEditor;
                                                }
                                              }
                                              else
                                              {
                                                CNGroupEditor groupEditor = new CNGroupEditor( groupNode, new CommandNodeEditorState() );
                                                groupEditor.Init();
                                                return groupEditor;
                                              }
                                            } },

             { typeof(CNFracture),          (CommandNode node) => 
                                            {
                                              CNFracture frNode = (CNFracture) node;
                                              CNFractureEditor frEditor = new CNFractureEditor(frNode, new CommandNodeEditorState());
                                              frEditor.Init();
                                              return frEditor;
                                            } },

             { typeof(CNWelder),            (CommandNode node) => 
                                            {
                                              CNWelder wdNode = (CNWelder) node;
                                              CNWelderEditor wdEditor = new CNWelderEditor(wdNode, new CommandNodeEditorState());
                                              wdEditor.Init();
                                              return wdEditor;
                                            } },

             { typeof(CNSelector),          (CommandNode node) => 
                                            {
                                              CNSelector selNode = (CNSelector) node;
                                              CNSelectorEditor selEditor = new CNSelectorEditor(selNode, new CommandNodeEditorState());
                                              selEditor.Init();
                                              return selEditor;
                                            } },

             { typeof(CNTessellator),       (CommandNode node) => 
                                            {
                                              CNTessellator tssNode = (CNTessellator) node;
                                              CNTessellatorEditor tssEditor = new CNTessellatorEditor(tssNode, new CommandNodeEditorState());
                                              tssEditor.Init();
                                              return tssEditor;
                                            } },

             { typeof(CNHelperMesh),        (CommandNode node) => 
                                            {
                                              CNHelperMesh hmNode = (CNHelperMesh) node;
                                              CNHelperMeshEditor hmEditor = new CNHelperMeshEditor(hmNode, new CommandNodeEditorState());
                                              hmEditor.Init();
                                              return hmEditor;
                                            } },

             { typeof(CNBalltreeGenerator), (CommandNode node) => 
                                            {
                                              CNBalltreeGenerator btNode = (CNBalltreeGenerator) node;
                                              CNBalltreeGeneratorEditor btEditor = new CNBalltreeGeneratorEditor(btNode, new CommandNodeEditorState());
                                              btEditor.Init();
                                              return btEditor;
                                            } },

             { typeof(CNGravity),           (CommandNode node) => 
                                            {
                                              CNGravity gravityNode = (CNGravity) node;
                                              CNGravityEditor gravityEditor = new CNGravityEditor(gravityNode, new CommandNodeEditorState());
                                              gravityEditor.Init();
                                              return gravityEditor;
                                            } },

             { typeof(CNExplosion),         (CommandNode node) => 
                                            {
                                              CNExplosion explosionNode = (CNExplosion) node;
                                              CNExplosionEditor explosionEditor = new CNExplosionEditor(explosionNode, new CNExplosionEditorState());
                                              explosionEditor.Init();
                                              return explosionEditor;
                                            } },

             { typeof(CNWind),             (CommandNode node) => 
                                           {
                                             CNWind windNode = (CNWind) node;
                                             CNWindEditor windEditor = new CNWindEditor(windNode, new CommandNodeEditorState());
                                             windEditor.Init();
                                             return windEditor;
                                           } },

             { typeof(CNAimedForce),       (CommandNode node) => 
                                           {
                                               CNAimedForce afNode = (CNAimedForce) node;
                                               CNAimedForceEditor afEditor = new CNAimedForceEditor(afNode, new CommandNodeEditorState());
                                               afEditor.Init();
                                               return afEditor;
                                           } },

             { typeof(CNAimedFall),       (CommandNode node) => 
                                           {
                                               CNAimedFall afNode = (CNAimedFall) node;
                                               CNAimedFallEditor afEditor = new CNAimedFallEditor(afNode, new CommandNodeEditorState());
                                               afEditor.Init();
                                               return afEditor;
                                           } },

             { typeof(CNSpeedLimiter),     (CommandNode node) => 
                                           {
                                               CNSpeedLimiter slNode = (CNSpeedLimiter) node;
                                               CNSpeedLimiterEditor slEditor = new CNSpeedLimiterEditor(slNode, new CommandNodeEditorState());
                                               slEditor.Init();
                                               return slEditor;
                                           } },

             { typeof(CNAttractor),     (CommandNode node) => 
                                           {
                                               CNAttractor atNode = (CNAttractor) node;
                                               CNAttractorEditor atEditor = new CNAttractorEditor(atNode, new CommandNodeEditorState());
                                               atEditor.Init();
                                               return atEditor;
                                           } },

             { typeof(CNJet),              (CommandNode node) => 
                                           {
                                               CNJet jetNode = (CNJet) node;
                                               CNJetEditor jetEditor = new CNJetEditor(jetNode, new CommandNodeEditorState());
                                               jetEditor.Init();
                                               return jetEditor;
                                           } },

            { typeof(CNParameterModifier), (CommandNode node) =>
                                           {
                                             CNParameterModifier pmNode = (CNParameterModifier) node;
                                             CNParameterModifierEditor pmEditor = new CNParameterModifierEditor(pmNode, new CommandNodeEditorState());
                                             pmEditor.Init();
                                             return pmEditor;
                                           } },

            { typeof(CNTriggerByTime),     (CommandNode node) =>
                                           {
                                             CNTriggerByTime tbtNode = (CNTriggerByTime) node;
                                             CNTriggerByTimeEditor tbtEditor = new CNTriggerByTimeEditor(tbtNode, new CommandNodeEditorState());
                                             tbtEditor.Init();
                                             return tbtEditor;
                                           } },

            { typeof(CNTriggerByContact),  (CommandNode node) =>
                                           {
                                             CNTriggerByContact tbcNode = (CNTriggerByContact) node;
                                             CNTriggerByContactEditor tbcEditor = new CNTriggerByContactEditor(tbcNode, new CommandNodeEditorState());
                                             tbcEditor.Init();
                                             return tbcEditor;
                                           } },


            { typeof(CNTriggerByDetector),  (CommandNode node) =>
                                           {
                                             CNTriggerByDetector tbdNode = (CNTriggerByDetector) node;
                                             CNTriggerByDetectorEditor tbdEditor = new CNTriggerByDetectorEditor(tbdNode, new CommandNodeEditorState());
                                             tbdEditor.Init();
                                             return tbdEditor;
                                           } },

            { typeof(CNTriggerByForce),  (CommandNode node) =>
                                           {
                                             CNTriggerByForce tbfNode = (CNTriggerByForce) node;
                                             CNTriggerByForceEditor tbfEditor = new CNTriggerByForceEditor(tbfNode, new CommandNodeEditorState());
                                             tbfEditor.Init();
                                             return tbfEditor;
                                           } },

           { typeof(CNTriggerByExplosion), (CommandNode node) =>
                                           {
                                             CNTriggerByExplosion tbeNode = (CNTriggerByExplosion) node;
                                             CNTriggerByExplosionEditor tbeEditor = new CNTriggerByExplosionEditor(tbeNode, new CommandNodeEditorState());
                                             tbeEditor.Init();
                                             return tbeEditor;
                                           } },

          { typeof(CNSubstituter),        (CommandNode node) =>
                                          {
                                            CNSubstituter subNode = (CNSubstituter) node;
                                            CNSubstituterEditor subEditor = new CNSubstituterEditor(subNode, new CommandNodeEditorState());
                                            subEditor.Init();
                                            return subEditor;
                                          } },

          { typeof(CNContactEmitter),     (CommandNode node) =>
                                          {
                                            CNContactEmitter ceNode = (CNContactEmitter) node;
                                            CNContactEmitterEditor ceEditor = new CNContactEmitterEditor(ceNode, new CommandNodeEditorState());
                                            ceEditor.Init();
                                            return ceEditor;
                                          } },

          { typeof(CNScriptPlayer),     (CommandNode node) =>
                                          {
                                            CNScriptPlayer spNode = (CNScriptPlayer) node;
                                            CNScriptPlayerEditor spEditor = new CNScriptPlayerEditor(spNode, new CommandNodeEditorState());
                                            spEditor.Init();
                                            return spEditor;
                                          } }
            
        };

        @switchDragAction  = new Dictionary<Type, CNCommandNodeDragAction> {

            { typeof(CNRigidbody),        (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                            mfEditor.AddGameObjects( draggedObjects, true );
                                          } },

            { typeof(CNSoftbody),         (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                            mfEditor.AddGameObjects( draggedObjects, true );
                                          } },

            { typeof(CNCloth),            (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                            mfEditor.AddGameObjects( draggedObjects, true );
                                          } },

            { typeof(CNRope),             (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNRopeEditor rpEditor = (CNRopeEditor)cnEditor;
                                            rpEditor.AddGameObjects( draggedObjects, true );
                                          } },

            { typeof(CNAnimatedbody),     (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                            mfEditor.AddGameObjects( draggedObjects, true );
                                          } },

            { typeof(CNJointGroups),      (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNJointGroupsEditor jgEditor = (CNJointGroupsEditor)cnEditor;

                                            GenericMenu menuJoints = new GenericMenu();
                                            menuJoints.AddItem(new GUIContent("Add to ObjectsA"), false, () =>
                                            {
                                              jgEditor.AddGameObjectsToA( draggedObjects, true );
                                            } );
                                            menuJoints.AddItem(new GUIContent("Add to ObjectsB"), false, () =>
                                            {
                                              jgEditor.AddGameObjectsToB( draggedObjects, true );
                                            });
                                            menuJoints.AddItem(new GUIContent("Add to LocatorsC"), false, () =>
                                            {
                                              jgEditor.AddGameObjectsToC( draggedObjects, true );
                                            });
                                            menuJoints.ShowAsContext();
                                          } },

            { typeof(CNServos),           (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNServosEditor svEditor = (CNServosEditor)cnEditor;

                                            GenericMenu menuServos = new GenericMenu();
                                            menuServos.AddItem(new GUIContent("Add to ObjectsA"), false, () =>
                                            {
                                              svEditor.AddGameObjectsToA( draggedObjects, true );
                                            } );
                                            menuServos.AddItem(new GUIContent("Add to ObjectsB"), false, () =>
                                            {
                                              svEditor.AddGameObjectsToB( draggedObjects, true );
                                            });
                                            menuServos.ShowAsContext();
                                          } },

            { typeof(CNGroup),            (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) =>
                                          {
                                            CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                            mfEditor.AddGameObjects( draggedObjects, true );
                                          } },

            { typeof(CNGravity),          (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) =>
                                          {
                                            CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                            mfEditor.AddGameObjects( draggedObjects, true );
                                          } },

             { typeof(CNWind),          (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects) => 
                                            {
                                              CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                              mfEditor.AddGameObjects( draggedObjects, true );
                                            } },


            { typeof(CNAimedForce),       (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) =>
                                          {
                                            CNAimedForceEditor afEditor = (CNAimedForceEditor)cnEditor;
                                            
                                            GenericMenu menu = new GenericMenu();
                                            menu.AddItem(new GUIContent("Add to Bodies"), false, () =>
                                            {
                                              afEditor.AddGameObjectsToBodies( draggedObjects, true );
                                            } );
                                            menu.AddItem(new GUIContent("Add to Aim GameObjects"), false, () =>
                                            {
                                              afEditor.AddGameObjectsToAim( draggedObjects, true );
                                            });

                                            menu.ShowAsContext();
                                          } },

            { typeof(CNAimedFall),       (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) =>
                                          {
                                            CNAimedFallEditor afEditor = (CNAimedFallEditor)cnEditor;
                                            
                                            GenericMenu menu = new GenericMenu();
                                            menu.AddItem(new GUIContent("Add to Bodies"), false, () =>
                                            {
                                              afEditor.AddGameObjectsToBodies( draggedObjects, true );
                                            } );
                                            menu.AddItem(new GUIContent("Add to Aim GameObjects"), false, () =>
                                            {
                                              afEditor.AddGameObjectsToAim( draggedObjects, true );
                                            });

                                            menu.ShowAsContext();
                                          } },

             { typeof(CNSpeedLimiter),    (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects) => 
                                            {
                                              CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                              mfEditor.AddGameObjects( draggedObjects, true );
                                            } },

             { typeof(CNAttractor),    (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects) => 
                                            {
                                              CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                              mfEditor.AddGameObjects( draggedObjects, true );
                                            } },

             { typeof(CNJet),             (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects) => 
                                            {
                                              CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                              mfEditor.AddGameObjects( draggedObjects, true );
                                            } },

            { typeof(CNWelder),           (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                            mfEditor.AddGameObjects( draggedObjects, true );
                                          } },

            { typeof(CNTessellator),      (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                            mfEditor.AddGameObjects( draggedObjects, true );
                                          } },

            { typeof(CNSelector),         (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                            mfEditor.AddGameObjects( draggedObjects, true );
                                          } },

            { typeof(CNFracture),         (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                            mfEditor.AddGameObjects( draggedObjects, true );
                                          } },

            { typeof(CNBalltreeGenerator),(CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                            mfEditor.AddGameObjects( draggedObjects, true );
                                          } },

            { typeof(CNParameterModifier), (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                            mfEditor.AddGameObjects( draggedObjects, true );
                                          } },

            { typeof(CNTriggerByContact), (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNTriggerByContactEditor tbcEditor = (CNTriggerByContactEditor)cnEditor;
                                            
                                            GenericMenu menu = new GenericMenu();
                                            menu.AddItem(new GUIContent("Add to ObjectsA"), false, () =>
                                            {
                                              tbcEditor.AddGameObjectsToA( draggedObjects, true );
                                            } );
                                            menu.AddItem(new GUIContent("Add to ObjectsB"), false, () =>
                                            {
                                              tbcEditor.AddGameObjectsToB( draggedObjects, true );
                                            });

                                            menu.ShowAsContext();
                                          } },

            { typeof(CNTriggerByDetector), (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNTriggerByDetectorEditor tbdEditor = (CNTriggerByDetectorEditor)cnEditor;
                                            
                                            GenericMenu menu = new GenericMenu();
                                            menu.AddItem(new GUIContent("Add to Detectors"), false, () =>
                                            {
                                              tbdEditor.AddGameObjectsToA( draggedObjects, true );
                                            } );
                                            menu.AddItem(new GUIContent("Add to Bodies"), false, () =>
                                            {
                                              tbdEditor.AddGameObjectsToB( draggedObjects, true );
                                            });

                                            menu.ShowAsContext();
                                          } },

          { typeof(CNTriggerByExplosion), (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNTriggerByExplosionEditor tbeEditor = (CNTriggerByExplosionEditor)cnEditor;
                                            
                                            tbeEditor.AddGameObjectsToBodies( draggedObjects, true );

                                          } },

          { typeof(CNTriggerByForce), (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNTriggerByForceEditor tbfEditor = (CNTriggerByForceEditor)cnEditor;
                                            
                                            GenericMenu menu = new GenericMenu();
                                            menu.AddItem(new GUIContent("Add to Objects"), false, () =>
                                            {
                                              tbfEditor.AddGameObjectsToA( draggedObjects, true );
                                            } );

                                            menu.ShowAsContext();
                                          } },

            { typeof(CNSubstituter),      (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNSubstituterEditor subsEditor = (CNSubstituterEditor)cnEditor;
                                            
                                            GenericMenu menu = new GenericMenu();
                                            menu.AddItem(new GUIContent("Add to ObjectsA"), false, () =>
                                            {
                                              subsEditor.AddGameObjectsToA( draggedObjects, true );
                                            } );
                                            menu.AddItem(new GUIContent("Add to ObjectsB"), false, () =>
                                            {
                                              subsEditor.AddGameObjectsToB( draggedObjects, true );
                                            });

                                            menu.ShowAsContext();
                                          } },

            { typeof(CNContactEmitter), (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNContactEmitterEditor ceEditor = (CNContactEmitterEditor)cnEditor;
                                            
                                            GenericMenu menu = new GenericMenu();
                                            menu.AddItem(new GUIContent("Add to ObjectsA"), false, () =>
                                            {
                                              ceEditor.AddGameObjectsToA( draggedObjects, true );
                                            } );
                                            menu.AddItem(new GUIContent("Add to ObjectsB"), false, () =>
                                            {
                                              ceEditor.AddGameObjectsToB( draggedObjects, true );
                                            });

                                            menu.ShowAsContext();
                                          } },

            { typeof(CNScriptPlayer),   (CommandNodeEditor cnEditor, UnityEngine.Object[] draggedObjects ) => 
                                          {
                                            CNMonoFieldEditor mfEditor = (CNMonoFieldEditor)cnEditor;
                                            mfEditor.AddGameObjects( draggedObjects, true );
                                          } },

        };
    }

  }
}