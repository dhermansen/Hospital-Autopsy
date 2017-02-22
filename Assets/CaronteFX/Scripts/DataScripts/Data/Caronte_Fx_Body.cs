// ***********************************************************
//	Copyright 2016 Next Limit Technologies, http://www.nextlimit.com
//	All rights reserved.
//
//	This source code is free for all non-commercial uses.
//
//	THIS SOFTWARE IS PROVIDED 'AS IS' AND WITHOUT ANY EXPRESS OR
//	IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
//	WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
//
// ***********************************************************

using UnityEngine;

namespace CaronteFX
{
  /// <summary>
  /// This script is auto-added to any GameObject that is defined as body in a 
  /// CaronteFX scene. It allows to define different collider and collider rendering options.
  /// Its usage is Editor only, so it will contain no data or register unity callbacks in a runtime build.
  /// </summary>
  [AddComponentMenu("CaronteFX/CaronteFX Body")]
  [DisallowMultipleComponent]
  public class Caronte_Fx_Body : MonoBehaviour
  {
#if UNITY_EDITOR

    public enum EColliderType
    {
      MeshFilter,
      MeshFilterConvexHull,
      CustomMesh,
      CustomGameObject,
      BalltreeAsset
    }

    public enum EColliderRenderMode
    {
      Wireframe,
      Solid
    }

    [SerializeField, HideInInspector]
    private EColliderType      colliderType_        = EColliderType.MeshFilter;
    [SerializeField, HideInInspector]
    private Mesh               colliderMesh_        = null;
    [SerializeField, HideInInspector]
    private GameObject         colliderGameObject_  = null;
    [SerializeField, HideInInspector]
    private Color              colliderColor_       = Color.red;
    [SerializeField, HideInInspector]
    private EColliderRenderMode colliderRenderMode_ = EColliderRenderMode.Solid;
    [SerializeField, HideInInspector]
    private Mesh               tileMesh_            = null;
    [SerializeField, HideInInspector]
    private CRBalltreeAsset    btAsset_             = null;
    [SerializeField, HideInInspector]
    private bool renderCollider_ = true;

    private Mesh auxDisplayMesh_;

    private bool inFocusByCaronteFX_ = false;
    public bool InFocusByCaronteFX
    {
      get { return inFocusByCaronteFX_; }
      set { inFocusByCaronteFX_ = value; }
    }
   
    public EColliderType ColliderType
    {
      get { return colliderType_; }
    }

    public Mesh ColliderMesh
    {
      get { return colliderMesh_; }
      set { colliderMesh_ = value; }
    }

    public bool RenderCollider
    {
      get { return renderCollider_; }
      set { renderCollider_ = value; }
    }

    public Color ColliderColor
    {
      get { return colliderColor_; }
    }

    public EColliderRenderMode ColliderRenderMode
    {
      get { return colliderRenderMode_; }
    }

    public Mesh TileMesh
    {
      get { return tileMesh_; }
    }

    public bool IsUsingCustomCollider()
    {
      if ( IsUsingCustomColliderMesh() ||
           IsUsingCustomColliderGameObject() ||
           IsUsingBalltree())
      {
        return true;
      }

      return false;
    }


    public bool HasValidCollider()
    {
      return (IsUsingMeshFilterOrMeshFilterConvex() || IsUsingCustomColliderMesh() || IsUsingCustomColliderGameObject() );
    }

    public bool IsUsingBalltree()
    {
      return (colliderType_ == EColliderType.BalltreeAsset && btAsset_ != null);
    }

    private bool IsUsingMeshFilterOrMeshFilterConvex()
    {
      return ( (colliderType_ == EColliderType.MeshFilter || colliderType_ == EColliderType.MeshFilterConvexHull) && gameObject.HasMesh() );
    }

    private bool IsUsingCustomColliderMesh()
    {
      return (colliderType_ == EColliderType.CustomMesh && colliderMesh_ != null);
    }


    private bool IsUsingCustomColliderGameObject()
    {
      return (colliderType_ == EColliderType.CustomGameObject && colliderGameObject_ != null && colliderGameObject_.HasMesh());
    }

    public void SetCustomColliderMesh(Mesh colliderMesh)
    {
      colliderType_ = EColliderType.CustomMesh;
      colliderMesh_ = colliderMesh;
    }

    public void SetBalltreeAsset(CRBalltreeAsset btAsset)
    {
      colliderType_ = EColliderType.BalltreeAsset;
      btAsset_ = btAsset;
    }

    public CRBalltreeAsset GetBalltreeAsset()
    {
      if (colliderType_ == EColliderType.BalltreeAsset)
      {
        return btAsset_;
      }
      return null;
    }

    public bool GetRenderMesh(out Mesh renderMesh)
    {
      return gameObject.GetMeshOrBakedMesh(out renderMesh);
    }

    public bool GetRenderMesh(out Mesh renderMesh, out Matrix4x4 m_Render_LocalToWorld)
    {
      m_Render_LocalToWorld = transform.localToWorldMatrix;
      return gameObject.GetMeshOrBakedMesh(out renderMesh);
    }

    public bool GetColliderMesh(out Mesh colliderMesh)
    {
      if ( colliderType_ == EColliderType.MeshFilter || 
           colliderType_ == EColliderType.MeshFilterConvexHull )
      {
        return gameObject.GetMeshOrBakedMesh(out colliderMesh);
      }
      else if ( colliderType_ == EColliderType.CustomMesh )
      {
        colliderMesh = colliderMesh_;
        return false;
      }
      else if ( colliderType_ == EColliderType.CustomGameObject )
      {
        return colliderGameObject_.GetMeshOrBakedMesh(out colliderMesh);
      }
      else
      {
        colliderMesh = null;
        return false;
      } 
    }

    public bool GetColliderMesh(out Mesh colliderMesh, out Matrix4x4 m_Collider_LocalToWorld)
    {
      if ( colliderType_ == EColliderType.MeshFilter || 
           colliderType_ == EColliderType.MeshFilterConvexHull )
      {
        m_Collider_LocalToWorld = transform.localToWorldMatrix;
        return gameObject.GetMeshOrBakedMesh(out colliderMesh);
      }
      else if ( colliderType_ == EColliderType.CustomMesh )
      {
        m_Collider_LocalToWorld = transform.localToWorldMatrix;
        colliderMesh = colliderMesh_;
        return false;
      }
      else if( colliderType_ == EColliderType.CustomGameObject )
      {
        m_Collider_LocalToWorld = colliderGameObject_.transform.localToWorldMatrix;
        return colliderGameObject_.GetMeshOrBakedMesh(out colliderMesh);
      }
      else
      {
        colliderMesh = null;
        m_Collider_LocalToWorld = transform.localToWorldMatrix;
        return false;
      }   
    }

    public bool GetDefinitionMesh(out Mesh definitionMesh, out Matrix4x4 m_Definition_LocalToWorld)
    {
      m_Definition_LocalToWorld = transform.localToWorldMatrix;
      return gameObject.GetMeshOrBakedMesh(out definitionMesh);
    }

    public Mesh GetTileMesh()
    {
      return tileMesh_;  
    }

    public GameObject GetColliderGameObject()
    {
      return colliderGameObject_;
    }

    public bool IsDrawRequested()
    {  
      return (gameObject.activeInHierarchy && InFocusByCaronteFX && RenderCollider && IsUsingCustomCollider());
    }

    void OnDrawGizmos()
    {
      if ( IsDrawRequested() )
      {
        Color current = Gizmos.color;
        Gizmos.color = colliderColor_;

        if (colliderRenderMode_ == EColliderRenderMode.Solid)
        {
          if (colliderType_ == EColliderType.CustomMesh)
          {
            Gizmos.DrawMesh( colliderMesh_, transform.position, transform.rotation, transform.lossyScale ); 
          }
          else if (colliderType_ == EColliderType.CustomGameObject)
          {
            bool wasBaked = colliderGameObject_.GetMeshOrBakedMesh(out auxDisplayMesh_);
            Gizmos.DrawMesh( auxDisplayMesh_, colliderGameObject_.transform.position, colliderGameObject_.transform.rotation, colliderGameObject_.transform.lossyScale );

            if (wasBaked)
            {
              Object.DestroyImmediate(auxDisplayMesh_);
            }
          }
        }
        else
        {
          if (colliderType_ == EColliderType.CustomMesh)
          {
            Gizmos.DrawWireMesh( colliderMesh_, transform.position, transform.rotation, transform.lossyScale );
          }
          else if (colliderType_ == EColliderType.CustomGameObject)
          {
            bool wasBaked = colliderGameObject_.GetMeshOrBakedMesh(out auxDisplayMesh_);
            Gizmos.DrawWireMesh( colliderMesh_, colliderGameObject_.transform.position, colliderGameObject_.transform.rotation, colliderGameObject_.transform.lossyScale );

            if (wasBaked)
            {
              Object.DestroyImmediate(auxDisplayMesh_);
            }
          }
        }   
        Gizmos.color = current;
      }
    }
#endif
  }
}

