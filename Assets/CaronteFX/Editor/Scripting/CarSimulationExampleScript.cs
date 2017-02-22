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

namespace CaronteFX.Scripting
{
  public class CarSimulationExampleScript : CarSimulationScript 
  {
    CarRigidBody carBodyA_;
    CarRigidBody carBodyB_;

    CarServo carServo_;

    public override void SimulationStart()
    {
      carBodyA_ = (CarRigidBody) CarGlobals.GetBodyByName("Cube");
      carBodyB_ = (CarRigidBody) CarGlobals.GetBodyByName("Cube (1)");

      carServo_ = CarGlobals.GetServoByBodyPair(carBodyA_, carBodyB_, true);
    }

    public override void SimulationUpdate()
    {
      float time = CarGlobals.GetTimeSimulated();

      if (time > 1.0f)
      {
        carServo_.SetTarget( new Vector3(0f, 5f, 0f) );
      }
    }

  }
}


