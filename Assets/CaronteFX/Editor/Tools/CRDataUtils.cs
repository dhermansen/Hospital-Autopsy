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
  public static class CRDataUtils
  {
    
    public static void GetCaronteFxGameObjects(UnityEngine.Object[] arrObjectReference, out List<GameObject> listGameObject)
    {
      listGameObject = new List<GameObject>();
      int arrObjectReference_size = arrObjectReference.Length;

      for (int i = 0; i < arrObjectReference_size; i++)
      {
        GameObject go = arrObjectReference[i] as GameObject;
        if (go != null)
        {
          if (go.IsInScene())
          {
            if (go.GetComponent<Caronte_Fx>() != null)
            {
              listGameObject.Add(go);
            }
            GameObject[] arrGameObject = CREditorUtils.GetAllChildObjects(go, true);
            foreach (GameObject childGo in arrGameObject)
            {
              if (childGo.GetComponent<Caronte_Fx>() != null)
              {
                listGameObject.Add(childGo);
              }
            }
          }
        }
      }
    }

    public static void GetCaronteFxsRelations(Caronte_Fx caronteFx, out List<Tuple2<Caronte_Fx, int>> listCaronteFx )
    {
      listCaronteFx = new List<Tuple2<Caronte_Fx, int>>();
      GameObject go = caronteFx.gameObject;
      if ( go.IsInScene() )
      {     
        GameObject[] arrChild = CREditorUtils.GetAllGameObjectsInScene();
        AddRelations( go, arrChild, listCaronteFx );
      }
    }

    public static void AddRelations(GameObject parentFx, GameObject[] arrGameObject, List<Tuple2<Caronte_Fx, int>> listCaronteFx)
    {
      for (int i = 0; i < arrGameObject.Length; i++)
      {
        GameObject go = arrGameObject[i];
        Caronte_Fx fxChild = go.GetComponent<Caronte_Fx>();
        if (fxChild != null)
        {
          int depth = go.GetFxHierachyDepthFrom(parentFx);
          listCaronteFx.Add(Tuple2.New(fxChild, depth));
        }
      }
    }
  }
}
