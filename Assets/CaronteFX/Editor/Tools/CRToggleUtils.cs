using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace CaronteFX
{
  public static class CRToggleUtils
  {
    public static void DrawToggleMixedMonoBehaviours(string toggleString, List<MonoBehaviour> listMonoBehaviour, float width)
    {
      EditorGUI.BeginChangeCheck();  
      int nMonoBehaviours = listMonoBehaviour.Count;

      EditorGUI.showMixedValue = false;
      bool value = false;
      
      if (nMonoBehaviours > 0)
      {
        value = listMonoBehaviour[0].enabled;
        for (int i = 1; i < nMonoBehaviours; ++i)
        {
          MonoBehaviour mbh = listMonoBehaviour[i];
          if ( value != mbh.enabled )
          {
            EditorGUI.showMixedValue = true;
            break;
          }
        }
      }
      EditorGUI.BeginDisabledGroup( nMonoBehaviours == 0 );
#if UNITY_PRO_LICENSE
      value = EditorGUILayout.ToggleLeft(toggleString, value, GUILayout.MaxWidth(width) );
#else
      value = GUILayout.Toggle(value, toggleString, GUILayout.MaxWidth(width));
#endif
      EditorGUI.showMixedValue = false;
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObjects(listMonoBehaviour.ToArray(), "CaronteFX - Change " + toggleString);
        for (int i = 0; i < nMonoBehaviours; ++i)
        {
          MonoBehaviour mbh = listMonoBehaviour[i];
          mbh.enabled = value;
          EditorUtility.SetDirty(mbh);
        }
      }
      EditorGUI.EndDisabledGroup();
    }
    //-----------------------------------------------------------------------------------
    public static void DrawToggleMixedRenderers(string toggleString, List<Renderer> listRenderer, float width)
    {
      EditorGUI.BeginChangeCheck();  
      int nRenderer = listRenderer.Count;

      EditorGUI.showMixedValue = false;
      bool value = false;
      
      if (nRenderer > 0)
      {
        value = listRenderer[0].enabled;
        for (int i = 1; i < nRenderer; ++i)
        {
          Renderer mbh = listRenderer[i];
          if ( value != mbh.enabled )
          {
            EditorGUI.showMixedValue = true;
            break;
          }
        }
      }
      EditorGUI.BeginDisabledGroup( nRenderer == 0 );

#if UNITY_PRO_LICENSE
      value = EditorGUILayout.ToggleLeft(toggleString, value, GUILayout.MaxWidth(width) );
#else
      value = GUILayout.Toggle(value, toggleString, GUILayout.MaxWidth(width));
#endif

      EditorGUI.showMixedValue = false;
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObjects(listRenderer.ToArray(), "CaronteFX - Change " + toggleString);
        for (int i = 0; i < nRenderer; ++i)
        {
          Renderer rn = listRenderer[i];
          rn.enabled = value;
          EditorUtility.SetDirty(rn);
        }
      }
      EditorGUI.EndDisabledGroup();
    }
    //-----------------------------------------------------------------------------------
    public static void DrawToggleMixedBodyComponents(string toggleString, List<Caronte_Fx_Body> listBodyComponent, float width)
    {
      EditorGUI.BeginChangeCheck();  
      int nMonoBehaviours = listBodyComponent.Count;

      EditorGUI.showMixedValue = false;
      bool value = false;
      
      if (nMonoBehaviours > 0)
      {
        value = listBodyComponent[0].RenderCollider;
        for (int i = 1; i < nMonoBehaviours; ++i)
        {
          Caronte_Fx_Body mbh = listBodyComponent[i];
          if ( value != mbh.RenderCollider )
          {
            EditorGUI.showMixedValue = true;
            break;
          }
        }
      }
      EditorGUI.BeginDisabledGroup( nMonoBehaviours == 0 );
#if UNITY_PRO_LICENSE
      value = EditorGUILayout.ToggleLeft(toggleString, value, GUILayout.MaxWidth(width) );
#else
      value = GUILayout.Toggle(value, toggleString, GUILayout.MaxWidth(width));
#endif
      EditorGUI.showMixedValue = false;
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObjects(listBodyComponent.ToArray(), "CaronteFX - Change " + toggleString);
        for (int i = 0; i < nMonoBehaviours; ++i)
        {
          Caronte_Fx_Body cfxBody = listBodyComponent[i];  
          cfxBody.RenderCollider = value;
          EditorUtility.SetDirty(cfxBody);
        }
      }
      EditorGUI.EndDisabledGroup();
    }
    //-----------------------------------------------------------------------------------
  }

}

