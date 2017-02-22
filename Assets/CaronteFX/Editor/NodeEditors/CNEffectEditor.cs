using UnityEngine;
using UnityEditor;
using System.Collections;


namespace CaronteFX
{
  public class CNEffectEditor : CNGroupEditor
  {
    protected Caronte_Fx   fxData_;
    protected CREffectData effectData_;

    protected static readonly GUIContent efScopeCt_       = new GUIContent( CRStringManager.GetString("EfScope"),               CRStringManager.GetString("EfScopeTooltip") );
    protected static readonly GUIContent efSelectScopeCt_ = new GUIContent( CRStringManager.GetString("EfSelectScope"),         CRStringManager.GetString("EfSelectScopeTooltip") );

    protected static readonly GUIContent[] arrScopeTypeCt_ = { new GUIContent(CRStringManager.GetString("EfScopeWholeScene")), new GUIContent(CRStringManager.GetString("EfScopeFxGameObjectParent")), new GUIContent(CRStringManager.GetString("EfScopeFxGameObject")) };
    protected int selectedScopeIdx_;
    //----------------------------------------------------------------------------------
    public CNEffectEditor( CNGroup data, CommandNodeEditorState state )
      : base ( data, state )
    {
      fxData_     = cnManager.FxData;
      effectData_ = fxData_.effect;

      selectedScopeIdx_ = (int)Data.CaronteFX_scope;
    }
    //----------------------------------------------------------------------------------
    public override void RenderGUI(Rect area, bool isEditable)
    {
      GUILayout.BeginArea(area);
      
      RenderTitle(isEditable);

      float originalLabelwidth = EditorGUIUtility.labelWidth;
      EditorGUIUtility.labelWidth = 100f;

      EditorGUI.BeginDisabledGroup( !isEditable );

      EditorGUI.BeginChangeCheck();
      selectedScopeIdx_ = EditorGUILayout.Popup(efScopeCt_, selectedScopeIdx_, arrScopeTypeCt_);
      if ( EditorGUI.EndChangeCheck() )
      {
        ChangeScope( (CNGroup.CARONTEFX_SCOPE)selectedScopeIdx_ );
      }

      EditorGUI.EndDisabledGroup();
      EditorGUILayout.Space();
      EditorGUILayout.Space();

      if (GUILayout.Button(efSelectScopeCt_, GUILayout.Height(30f)))
      {
        SceneSelection();
      }

      EditorGUIUtility.labelWidth = originalLabelwidth;

      CRGUIUtils.DrawSeparator();
      CRGUIUtils.Splitter();
      EditorGUILayout.Space();

      GUILayout.EndArea();
    }
    //----------------------------------------------------------------------------------
    protected void ChangeScope(CNGroup.CARONTEFX_SCOPE scope)
    {   
      Data.CaronteFX_scope = scope; 
      ApplyEffectScope();
      cnHierarchy.RecalculateFieldsDueToUserAction();
    }
    //----------------------------------------------------------------------------------
    public void ApplyEffectScope()
    {
      Transform effectTr       = Data.transform.parent;
      Transform effectTrParent = effectTr.parent;

      CNGroup.CARONTEFX_SCOPE scope = Data.CaronteFX_scope;

      ClearField();

      switch (scope)
      {
        case CNGroup.CARONTEFX_SCOPE.CARONTEFX_GAMEOBJECT:
          {
            GameObject[] arrGameObject = effectTr.gameObject.GetAllChildObjects(true);
            AddGameObjects(arrGameObject, false);
            break;
          }

        case CNGroup.CARONTEFX_SCOPE.CARONTEFX_GAMEOBJECT_PARENT:
          {
            if (effectTrParent != null)
            {
              GameObject[] arrGameObject = effectTrParent.gameObject.GetAllChildObjects(true);
              AddGameObjects(arrGameObject, false);
            }
            else
            {
              AddWildcard("*", false);
            }
            break;
          }

        case CNGroup.CARONTEFX_SCOPE.SCENE:
          {
            AddWildcard("*", false);
            break;
          }

        default:
          break;
      }

      EditorUtility.SetDirty(Data);
    }
  }
}

