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
using System.Collections;
using System.Collections.Generic;

namespace CaronteFX.Scripting
{
  public abstract class CarSimulationScript : ScriptableObject
  {
    protected List<CarObject> listCarObject_ = new List<CarObject>();

    public abstract void SimulationStart();
	  public abstract void SimulationUpdate();

    public void Init(List<CarObject> listCarObject)
    {
      listCarObject_.Clear();
      listCarObject_.AddRange(listCarObject);
    }

  }
}


