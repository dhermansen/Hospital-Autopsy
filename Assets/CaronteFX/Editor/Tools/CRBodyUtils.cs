using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace CaronteFX
{
  public static class CRBodyUtils
  {

    public static Caronte_Fx_Body AddBodyComponentIfHasMesh(GameObject go)
    {
      Caronte_Fx_Body bodyComponent = go.GetComponent<Caronte_Fx_Body>();
   
      if (bodyComponent == null && go.HasMesh() )
      {
        bodyComponent = go.AddComponent<Caronte_Fx_Body>();
      }

      return bodyComponent;
    }
    //-----------------------------------------------------------------------------------
    public static bool AddBodyComponentIfHasMeshReturnHasValidCollider(GameObject go)
    {
      Caronte_Fx_Body bodyComponent = AddBodyComponentIfHasMesh(go);

      return (bodyComponent != null && bodyComponent.HasValidCollider() );
    }
    //-----------------------------------------------------------------------------------
    public static bool AddBodyComponentIfHasMeshReturnHasValidColliderOrBalltree(GameObject go)
    {
      Caronte_Fx_Body bodyComponent = AddBodyComponentIfHasMesh(go);

      return (bodyComponent != null && (bodyComponent.HasValidCollider() || bodyComponent.IsUsingBalltree()) );
    }

    //-----------------------------------------------------------------------------------
    public static bool AddBodyComponentIfHasMeshReturnHasValidRenderMesh(GameObject go)
    {
      Caronte_Fx_Body bodyComponent = AddBodyComponentIfHasMesh(go);

      return (bodyComponent != null && go.HasMesh() );
    }
    //-----------------------------------------------------------------------------------
    public static bool HasValidRenderMesh(GameObject go)
    {
      return go.HasMesh();
    }
    //-----------------------------------------------------------------------------------
    public static bool HasValidColliderMesh(GameObject go)
    {
      Caronte_Fx_Body bodyComponent = go.GetComponent<Caronte_Fx_Body>();
   
      if (bodyComponent != null )
      {
        return bodyComponent.HasValidCollider();
      }
      return false;
    }
    //-----------------------------------------------------------------------------------
    public static void GetRenderMeshData( GameObject go, ref Mesh meshRender, out Matrix4x4 m_Render_MODEL_to_WORLD, ref bool isBakedRenderMesh )
    {
      Caronte_Fx_Body bodyComponent = go.GetComponent<Caronte_Fx_Body>();
      isBakedRenderMesh = bodyComponent.GetRenderMesh(out meshRender, out m_Render_MODEL_to_WORLD);
    }
    //-----------------------------------------------------------------------------------
    public static void GetColliderMeshData( GameObject go, ref Mesh meshCollider, out Matrix4x4 m_Collider_MODEL_to_WORLD, ref bool isBakedColliderMesh )
    {     
      Caronte_Fx_Body bodyComponent = go.GetComponent<Caronte_Fx_Body>();
      isBakedColliderMesh = bodyComponent.GetColliderMesh(out meshCollider, out m_Collider_MODEL_to_WORLD);
    }
    //-----------------------------------------------------------------------------------
    public static void GetDefinitionAndTileMeshes( GameObject go, ref Mesh meshDefinition, out Matrix4x4 m_Definition_MODEL_to_WORLD, ref bool isBakedDefinitionMesh, ref Mesh meshTile)
    {     
      Caronte_Fx_Body bodyComponent = go.GetComponent<Caronte_Fx_Body>();
      isBakedDefinitionMesh = bodyComponent.GetRenderMesh(out meshDefinition, out m_Definition_MODEL_to_WORLD);
      meshTile = bodyComponent.GetTileMesh();     
    }
    //-----------------------------------------------------------------------------------
    public static void GetBalltreeAsset( GameObject go, ref CRBalltreeAsset btAsset)
    {     
      Caronte_Fx_Body bodyComponent = go.GetComponent<Caronte_Fx_Body>();
      btAsset =  bodyComponent.GetBalltreeAsset();   
    }
    //-----------------------------------------------------------------------------------

  }
}
