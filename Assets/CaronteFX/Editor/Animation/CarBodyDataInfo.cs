using UnityEngine;
using System.Collections.Generic;
using CaronteSharp;


namespace CaronteFX
{
  public abstract class CarBodyDataInfo
  {
    protected MeshRenderer[] arrNormalMeshRenderer_;
    protected SkinnedMeshRenderer[] arrSkinnedMeshRenderer_;

    protected uint[] arrIdBodyNormalGameObjects_;
    protected uint[] arrIdBodySkinnedGameObjects_;

    protected void AssignBodyIds()
    {
      CNManager cnManager = CNManager.Instance;
      CREntityManager entityManager = cnManager.EntityManager;

      GameObject[] normalObjects = getNormalObjects();

      arrIdBodyNormalGameObjects_ = new uint[normalObjects.Length];

      for (int i = 0; i < normalObjects.Length; i++)
      {
        GameObject go = normalObjects[i];
        if (entityManager.IsGameObjectAnimated(go))
        {
          uint idBody = entityManager.GetIdBodyFromGo(go);
          arrIdBodyNormalGameObjects_[i] = idBody;
        }
        else
        {
          arrIdBodyNormalGameObjects_[i] = uint.MaxValue;
        }
      }


      GameObject[] skinnedObjects = getSkinnedObjects();

      arrIdBodySkinnedGameObjects_ = new uint[skinnedObjects.Length];

      for (int i = 0; i < skinnedObjects.Length; i++)
      {
        GameObject go = skinnedObjects[i];
        if (entityManager.IsGameObjectAnimated(go))
        {
          uint idBody = entityManager.GetIdBodyFromGo(go);
          arrIdBodySkinnedGameObjects_[i] = idBody;
        }
        else
        {
          arrIdBodySkinnedGameObjects_[i] = uint.MaxValue;
        }
      }

    }

    public GameObject[] getNormalObjects()
    {
      List<GameObject> listGameObject = new List<GameObject>();
      foreach (MeshRenderer mr in arrNormalMeshRenderer_)
      {
        listGameObject.Add(mr.gameObject);
      }

      return listGameObject.ToArray();
    }

    public GameObject[] getSkinnedObjects()
    {
      List<GameObject> listGameObject = new List<GameObject>();
      foreach (SkinnedMeshRenderer smr in arrSkinnedMeshRenderer_)
      {
        listGameObject.Add(smr.gameObject);
      }

      return listGameObject.ToArray();
    }

    public void SetModeAnimation(bool active)
    {
      foreach (uint idBody in arrIdBodyNormalGameObjects_)
      {
        if (idBody != uint.MaxValue)
        {
          RigidbodyManager.Rg_setAnimatingMode(idBody, active);
        }

      }

      foreach (uint idBody in arrIdBodySkinnedGameObjects_)
      {
        if (idBody != uint.MaxValue)
        {
          RigidbodyManager.Rg_setAnimatingMode(idBody, active);
        }
      }
    }
  }
}


