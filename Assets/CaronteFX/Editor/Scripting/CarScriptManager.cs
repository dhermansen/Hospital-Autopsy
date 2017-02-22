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
using CaronteFX.Scripting;



namespace CaronteFX
{
  public static class CRScriptManager
  {  
    static List<CarSimulationScript> listSimulationScript_ = new List<CarSimulationScript>(); 
    static bool skipDueToExcepction_ = false;

    public static void Init(List<CNScriptPlayerEditor> listSpEditor)
    {
      skipDueToExcepction_ = false;
      listSimulationScript_.Clear();

      foreach( CNScriptPlayerEditor spEditor in listSpEditor )
      {
        if (spEditor.IsEnabledInHierarchy && !spEditor.IsExcludedInHierarchy)
        {
          spEditor.InitSimulationScriptObject();
        
          CarSimulationScript simScript = spEditor.GetSimulationScript();
          if (simScript != null)
          {
            listSimulationScript_.Add(simScript);
          }
        }
      }

      if (listSpEditor.Count > 0)
      {
        SimulationManager.SetScriptingCallbacks(SimulationStart, SimulationUpdate);
      }
    }

    public static void SimulationStart()
    {
      if ( listSimulationScript_.Count > 0 && !skipDueToExcepction_ )
      {
        try
        {
          foreach(CarSimulationScript simulationScript in listSimulationScript_)
          {
            simulationScript.SimulationStart();
          }
        }
        catch (Exception e)
        {
          Debug.LogError("Exception happened in simulation start. Check simulation scripts. Details: ");
          Debug.LogError(e.Message);
          Debug.LogError(e.StackTrace);
          skipDueToExcepction_ = true;
          return;
        }
      }
    }

    public static void SimulationUpdate()
    {
      if ( listSimulationScript_.Count > 0 && !skipDueToExcepction_ )
      {
        try
        {
          foreach(CarSimulationScript simulationScript in listSimulationScript_)
          {
            simulationScript.SimulationUpdate();
          }
        }
        catch (Exception e)
        {
          Debug.LogError("Exception happened in simulation update. Check simulation scripts. Details: ");
          Debug.LogError(e.Message);
          Debug.LogError(e.StackTrace);
          skipDueToExcepction_ = true;
          return;
        }
      }
    }

  }
}

