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
using CaronteSharp;
using System.Collections;

namespace CaronteFX
{
  public class CRCreationData
  {
    private GameObject gameObject_;

    private bool isRope_;
    private bool positionRotationCanBeUpdated_;

    private Caronte_Fx_Body.EColliderType colliderType_;
    private bool hasValidCollider_;

    private byte[] renderFingerprint_;
    private byte[] colliderFingerprint_;

    private CRBalltreeAsset btAsset_;

    private Vector3    position_;
    private Quaternion rotation_;
    private Vector3    lossyScale_;

    private Vector3    positionCollider_;
    private Quaternion rotationCollider_;
    private Vector3    lossyScaleCollider_;

    private static byte[] fingerprintAux_ = new byte[256];

    public CRCreationData(GameObject go, Mesh renderMesh, Mesh colliderMesh, CRBalltreeAsset btAsset, bool isRope)
    {
      gameObject_ = go;
      isRope_ = isRope;

      positionRotationCanBeUpdated_ = true;

      renderFingerprint_   = null;
      colliderFingerprint_ = null;

      btAsset_ = btAsset;

      if (renderMesh != null)
      {
        renderFingerprint_ = new byte[256];
        CRGeometryUtils.CalculateFingerprint( renderMesh, renderFingerprint_ );
      }

      if (colliderMesh != null)
      {
        colliderFingerprint_ = new byte[256];
        CRGeometryUtils.CalculateFingerprint( colliderMesh, colliderFingerprint_ );
      }

      position_      = go.transform.position;
      rotation_      = go.transform.rotation;
      lossyScale_    = go.transform.lossyScale;

      positionCollider_   = Vector3.zero;
      rotationCollider_   = Quaternion.identity;
      lossyScaleCollider_ = Vector3.one;

      Caronte_Fx_Body fxBody = go.GetComponent<Caronte_Fx_Body>();
      if (fxBody != null)
      {
        colliderType_ = fxBody.ColliderType;
        hasValidCollider_ = fxBody.HasValidCollider(); 

        if (colliderType_ == Caronte_Fx_Body.EColliderType.CustomGameObject)
        {
          GameObject goCollider =  fxBody.GetColliderGameObject();

          if (goCollider != null)
          {
            positionCollider_   = goCollider.transform.position;
            rotationCollider_   = goCollider.transform.rotation;
            lossyScaleCollider_ = goCollider.transform.lossyScale;
          }
        }
      }
    }

    public bool IsValidScale()
    {
      if (gameObject_ == null)
      {
        return false;
      }

      if (gameObject_.transform.lossyScale != lossyScale_ )
      {
        return false;
      }

      if (colliderType_ == Caronte_Fx_Body.EColliderType.CustomGameObject)
      {
        Caronte_Fx_Body bodyComponent = gameObject_.GetComponent<Caronte_Fx_Body>();

        if (bodyComponent == null)
        {
          return false;
        }

        GameObject colliderGameObject = bodyComponent.GetColliderGameObject();

        if (colliderGameObject == null)
        {
          return false;
        }

        if (colliderGameObject.transform.lossyScale != lossyScaleCollider_ )
        {
          return false;
        }
      }   

      return true;
    }

    public bool IsValidPositionRotation()
    {
      if (gameObject_ == null)
      {
        positionRotationCanBeUpdated_ = false;
        return false;
      }
       
      if (gameObject_.transform.position != position_ ||
          gameObject_.transform.rotation != rotation_ )
      {
        return false;
      }

      if (colliderType_ == Caronte_Fx_Body.EColliderType.CustomGameObject)
      {
        positionRotationCanBeUpdated_ = false;

        Caronte_Fx_Body bodyComponent = gameObject_.GetComponent<Caronte_Fx_Body>();

        if (bodyComponent == null)
        {
          return false;
        }
        GameObject colliderGameObject = bodyComponent.GetColliderGameObject();

        if (colliderGameObject == null)
        {
          return false;
        }

        if (colliderGameObject.transform.position != positionCollider_ ||
            colliderGameObject.transform.rotation != rotationCollider_ )
        {
          return false;
        }
      }   
      return true;
    }

    public bool CanPositionRotationBeUpdated()
    {      
      return positionRotationCanBeUpdated_;
    }

    public Matrix4x4 GetCurrentMatrixLocalToWorld()
    {
      if (colliderType_ == Caronte_Fx_Body.EColliderType.CustomGameObject)
      {
        Caronte_Fx_Body bodyComponent = gameObject_.GetComponent<Caronte_Fx_Body>();
        GameObject colliderGameObject = bodyComponent.GetColliderGameObject();

        return colliderGameObject.transform.localToWorldMatrix;
      }
      else
      {
        return gameObject_.transform.localToWorldMatrix;
      }
    }

    public void UpdatePositionRotation()
    {
      position_ = gameObject_.transform.position;
      rotation_ = gameObject_.transform.rotation;

      if (colliderType_ == Caronte_Fx_Body.EColliderType.CustomGameObject)
      {
        Caronte_Fx_Body bodyComponent = gameObject_.GetComponent<Caronte_Fx_Body>();

        if (bodyComponent != null)
        {
          GameObject colliderGameObject = bodyComponent.GetColliderGameObject();

          if (colliderGameObject != null)
          {
            positionCollider_ = colliderGameObject.transform.position;
            rotationCollider_ = colliderGameObject.transform.rotation;
          }
        }

      }
    }

    public bool AreFingerprintsValid()
    {
      Caronte_Fx_Body fxBody = gameObject_.GetComponent<Caronte_Fx_Body>();
      if (fxBody == null)
      {
        return false;
      }

      return AreMeshChecksValid(fxBody) && AreMeshFingerprintsValid(fxBody);     
    }


    private bool AreMeshChecksValid(Caronte_Fx_Body fxBody)
    {
      if ( colliderType_ != fxBody.ColliderType )
      {
        return false;
      }

      if ( hasValidCollider_ != fxBody.HasValidCollider() )
      {
        return false;
      }

      if (btAsset_ != fxBody.GetBalltreeAsset())
      {
        return false;
      }
      else
      {
        Mesh colliderMesh;
        bool isBakedMesh = false;
        if (isRope_)
        {
          colliderMesh = fxBody.GetTileMesh();
        }      
        else
        {
          isBakedMesh = fxBody.GetColliderMesh(out colliderMesh);
        }
        if (colliderFingerprint_ == null && colliderMesh != null ||
            colliderFingerprint_ != null && colliderMesh == null )
        {
           if (isBakedMesh)
          {
            Object.DestroyImmediate(colliderMesh);
          }   
          return false;
        }

        if (isBakedMesh)
        {
          Object.DestroyImmediate(colliderMesh);
        }
      }

      {
        Mesh renderMesh;
        bool isBakedMesh = fxBody.GetRenderMesh(out renderMesh);

        if (renderFingerprint_ == null && renderMesh != null ||
            renderFingerprint_ != null && renderMesh == null )
        {
          if (isBakedMesh)
          {
            Object.DestroyImmediate(renderMesh);
          }
          return false;
        }

        if (isBakedMesh)
        {
          Object.DestroyImmediate(renderMesh);
        }
      }

      return true;
    }


    private bool AreMeshFingerprintsValid(Caronte_Fx_Body fxBody)
    {
      bool isValid = true;

      if (renderFingerprint_ != null)
      {
        Mesh renderMesh;
        bool isBakedMesh = fxBody.GetRenderMesh(out renderMesh);

        if (renderMesh == null)
        {
          return false;
        }
        
        CRGeometryUtils.CalculateFingerprint(renderMesh, fingerprintAux_);
        isValid &= CRGeometryUtils.AreFingerprintsEqual(fingerprintAux_, renderFingerprint_);

        if (isBakedMesh)
        {
          Object.DestroyImmediate(renderMesh);
        }
      }

      if (colliderFingerprint_ != null)
      {
        Mesh meshToCheck;
        bool isBakedMesh = false;
        if (isRope_)
        {
          meshToCheck = fxBody.GetTileMesh();
        }      
        else
        {
          isBakedMesh = fxBody.GetColliderMesh(out meshToCheck);
        }

        if (meshToCheck == null)
        {
          return false;
        }

        CRGeometryUtils.CalculateFingerprint(meshToCheck, fingerprintAux_);
        isValid &= CRGeometryUtils.AreFingerprintsEqual(fingerprintAux_, colliderFingerprint_ );

        if (isBakedMesh)
        {
          Object.DestroyImmediate(meshToCheck);
        }
      }

      return isValid;
    }

  }
}
