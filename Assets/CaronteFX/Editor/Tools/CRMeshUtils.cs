using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CaronteFX
{
  public static class CRMeshUtils
  {
    public static void ModifyInternalUVs(CNFracture data)
    {
      if (data.ArrChoppedGameObject != null && data.ArrChoppedMesh == null)
      {
        int nGameObject = data.ArrChoppedGameObject.Length;
        data.ArrChoppedMesh = new Mesh[nGameObject];
        for (int i = 0; i < nGameObject; i++)
        {
          GameObject go = data.ArrChoppedGameObject[i];
          data.ArrChoppedMesh[i] = go.GetMesh();
        }
        EditorUtility.SetDirty(data);
      }

      if (data.ArrChoppedGameObject != null && data.ArrInteriorSubmeshIdx != null)
      {
        int nMesh = data.ArrChoppedMesh.Length;
        
        for (int i = 0; i < nMesh; i++)
        {
          GameObject go = data.ArrChoppedGameObject[i];
          Mesh originalMesh = data.ArrChoppedMesh[i];
          if ( go != null && originalMesh != null )
          {
            Mesh newMesh = Object.Instantiate(originalMesh);
            int submeshIdx = data.ArrInteriorSubmeshIdx[i];
            if (submeshIdx != -1)
            {
              Vector2[] arrUV = newMesh.uv;
              if (arrUV != null)
              {
                BitArray bitArray = new BitArray(arrUV.Length, false);   
                int[] arrInternalTriangles = newMesh.GetTriangles(submeshIdx);

                for (int j = 0; j < arrInternalTriangles.Length; j++)
                {
                  int index = arrInternalTriangles[j];
                  bitArray.Set(index, true);
                }

                for (int j = 0; j < arrUV.Length; j++)
                {
                  if (bitArray.Get(j))
                  {
                    Vector2 uv = arrUV[j];
                    uv.Scale(data.InteriorFacesTiling);
                    uv += data.InteriorFacesOffset;    
                    arrUV[j] = uv;
                  }
                }
                newMesh.uv = arrUV;
                go.SetMesh(newMesh);
              }
            }
          }
        }
      }
    }
  }
}

