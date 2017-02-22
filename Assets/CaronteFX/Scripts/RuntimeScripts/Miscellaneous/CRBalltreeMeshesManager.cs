using UnityEngine;
using System.Collections.Generic;

namespace CaronteFX
{

  public class CRBalltreeMeshes
  {
    List<Mesh>         listMesh_    = new List<Mesh>();
    CRSpheresGenerator spGenerator_ = new CRSpheresGenerator();
    //-----------------------------------------------------------------------------------
    public CRBalltreeMeshes()
    {
    }
    //-----------------------------------------------------------------------------------
    public void AddSphere(Color color, Vector3 position, float radius)
    {
      if (!spGenerator_.CanAddSphere128Faces66Vertices())
      {
        listMesh_.Add(spGenerator_.GenerateMeshTmp());
      }

      spGenerator_.AddOrthoSphere128Faces66Vertices(position, radius, color);
    }
    //-----------------------------------------------------------------------------------
    public void FinishAddingSpheres()
    {
      if (spGenerator_.CanGenerateMesh())
      {
        listMesh_.Add(spGenerator_.GenerateMeshTmp());
      }
    }
    //-----------------------------------------------------------------------------------
    public void DrawMeshesSolid(Matrix4x4 m_Local_To_World, Material material)
    {
      int nMeshes = listMesh_.Count;

      for (int i = 0; i < nMeshes; i++)
      {
        Mesh mesh = listMesh_[i]; 
        material.SetPass(0);
        Graphics.DrawMeshNow(mesh, m_Local_To_World);
      }
    }
    //-----------------------------------------------------------------------------------
  }


  public class CRBalltreeMeshesManager
  {
    Dictionary<CRBalltreeAsset, CRBalltreeMeshes> dictionaryBalltreeMeshes = new Dictionary<CRBalltreeAsset, CRBalltreeMeshes>();
    Material material_;

    private static CRBalltreeMeshesManager instance_;
    public static CRBalltreeMeshesManager Instance
    {
      get
      {
        if ( instance_ == null )
        {
          instance_ = new CRBalltreeMeshesManager();
        }
        return instance_;
      }
    }

    private CRBalltreeMeshesManager()
    {

    }

    public bool HasBalltreeMeshes(CRBalltreeAsset btAsset)
    {
      return dictionaryBalltreeMeshes.ContainsKey(btAsset);
    }

    public CRBalltreeMeshes GetBalltreeMeshes(CRBalltreeAsset btAsset)
    {
      if (!dictionaryBalltreeMeshes.ContainsKey(btAsset))
      {
        CRBalltreeMeshes btMeshes = CreateBalltreeMeshes(btAsset);
        dictionaryBalltreeMeshes.Add(btAsset, btMeshes);
      }

      return dictionaryBalltreeMeshes[btAsset];
    }

    public Material GetBalltreeMaterial()
    {
      if ( material_ == null)
      {
        material_ = new Material(Shader.Find("CaronteFX/Vertex Colors"));
      }
      return material_;
    }

    private CRBalltreeMeshes CreateBalltreeMeshes(CRBalltreeAsset btAsset)
    {
      CRBalltreeMeshes btMeshes = new CRBalltreeMeshes();
      CRSphere[] arrSphere = btAsset.LeafSpheres;
      int arrSphere_size = arrSphere.Length;
      for (uint i = 0; i < arrSphere_size; i++)
      {
        CRSphere sphere = arrSphere[i];
        btMeshes.AddSphere(CRColor.ColorBasic42(i), sphere.center_, sphere.radius_);
      }

      btMeshes.FinishAddingSpheres();
      return btMeshes;
     }
  }
}
