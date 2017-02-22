using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using CaronteSharp;

namespace CaronteFX.Scripting
{
  public static class CarGlobals
  {
    static List<CarBody>       listCarBody_        = new List<CarBody>();
    static List<CarRigidBody>  listCarRigidBody_   = new List<CarRigidBody>();
    static List<CarBodyMesh>   listCarBodyMesh_    = new List<CarBodyMesh>();
    static List<CarSoftBody>   listCarSoftbody_    = new List<CarSoftBody>();
    static List<CarRope>       listCarRope_        = new List<CarRope>();
    static List<CarCloth>      listCarCloth_       = new List<CarCloth>();
    static List<CarJoint>      listCarJoint_       = new List<CarJoint>();
    static List<CarServo>      listCarServo_       = new List<CarServo>();
    static List<CarEntity>     listCarEntity_      = new List<CarEntity>();

    public static void Init(List<CarObject> listCarObject)
    {
      listCarBody_        .Clear();     
      listCarRigidBody_   .Clear();
      listCarBodyMesh_    .Clear();
      listCarSoftbody_    .Clear();
      listCarCloth_       .Clear();
      listCarJoint_       .Clear();
      listCarServo_       .Clear();
      listCarEntity_      .Clear();

      foreach( CarObject carObject in listCarObject )
      {
        if ( carObject.IsBody )
        {
          listCarBody_.Add( (CarBody)carObject );
          if ( carObject.IsRigidBody )
          {
            listCarRigidBody_.Add( (CarRigidBody)carObject );
          }
          else if ( carObject.IsSoftbody )
          {
            listCarSoftbody_.Add( (CarSoftBody)carObject );
            if ( carObject.IsRope )
            {
              listCarRope_.Add( (CarRope)carObject );
            }
          }
          else if ( carObject.IsBodyMesh )
          {
            listCarBodyMesh_.Add( (CarBodyMesh)carObject );
          }
          else if ( carObject.IsCloth )
          {
            listCarCloth_.Add( (CarCloth)carObject );
          }
        }
        else if ( carObject.IsJoint )
        {
          listCarJoint_.Add( (CarJoint)carObject );
        }
        else if ( carObject.IsServo )
        {
          listCarServo_.Add( (CarServo)carObject );
        }
        else if ( carObject.IsEntity )
        {
          listCarEntity_.Add( (CarEntity)carObject );
        }
      }
    }

    public static float GetTimeSimulated()
    {
      return (float) CaronteSharp.SimulationManager.GetTimeSimulated();
    }

    public static float GetDeltaTimeSimulation()
    {
      return (float)CaronteSharp.SimulationManager.GetDeltaTimeSimulation();
    }

    public static float GetDeltaTimeFrame()
    {
      return (float)CaronteSharp.SimulationManager.GetDeltaTimeFrame();
    }

    public static List<CarBody> GetListBody()
    {
      return new List<CarBody>(listCarBody_);
    }

    public static List<CarRigidBody> GetListRigidBody()
    {
      return new List<CarRigidBody>(listCarRigidBody_);
    }

    public static List<CarSoftBody> GetListSoftBody()
    {
      return new List<CarSoftBody>(listCarSoftbody_);
    }

    public static List<CarRope> GetListRope()
    {
      return new List<CarRope>(listCarRope_);
    }

    public static List<CarCloth> GetListCloth()
    {
      return new List<CarCloth>(listCarCloth_);
    }

    public static List<CarJoint> GetListCarJoint()
    {
      return new List<CarJoint>(listCarJoint_);
    }

    public static List<CarServo> GetListCarServo()
    {
      return new List<CarServo>(listCarServo_);
    }

    public static List<CarEntity> GetListCarEntity()
    {
      return new List<CarEntity>(listCarEntity_);
    }

    public static CarBody GetBodyByName(string bodyName)
    {
      return ( listCarBody_.Find( x => (x.Name == bodyName) ) );  
    }

    public static CarServo GetServoByBodyPair( CarBody bodyA, CarBody bodyB, bool isLinearVsAngular )
    {
      uint idServo = ServosManager.GetServoBetweenBodiesById(bodyA.Id, bodyB.Id, isLinearVsAngular);

      if (idServo != uint.MaxValue)
      {
        return ( listCarServo_.Find( x => ( x.Id == idServo) ) );
      }

      return null;
    }

  }
}

