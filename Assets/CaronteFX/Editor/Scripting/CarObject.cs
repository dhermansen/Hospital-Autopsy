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
using CaronteSharp;

namespace CaronteFX.Scripting
{
  public enum ECarObjectType
  {
    None        = 0,
    Body        = 1 << 1,
    RigidBody   = 1 << 2,
    BodyMesh    = 1 << 3,
    SoftBody    = 1 << 4,
    Rope        = 1 << 5,
    Cloth       = 1 << 6,
    Joint       = 1 << 7,
    Servo       = 1 << 8,
    Entity      = 1 << 9
  }             

  public abstract class CarObject 
  {
    public uint Id { get; private set; }
    public ECarObjectType Type { get; protected set; }
    public string Name { get; private set; }

    protected bool HasTypeFlag(ECarObjectType typeFlag)
    {
      return ( (Type & typeFlag) == typeFlag );
    }

    public bool IsBody       { get { return HasTypeFlag( ECarObjectType.Body ); } }
    public bool IsRigidBody  { get { return HasTypeFlag( ECarObjectType.RigidBody ); } }
    public bool IsBodyMesh   { get { return HasTypeFlag( ECarObjectType.BodyMesh ); } }
    public bool IsSoftbody   { get { return HasTypeFlag( ECarObjectType.SoftBody ); } }
    public bool IsRope       { get { return HasTypeFlag( ECarObjectType.Rope ); } }
    public bool IsCloth      { get { return HasTypeFlag( ECarObjectType.Cloth ); } }
    public bool IsJoint      { get { return HasTypeFlag( ECarObjectType.Joint ); } }
    public bool IsServo      { get { return HasTypeFlag( ECarObjectType.Servo ); } }
    public bool IsEntity     { get { return HasTypeFlag( ECarObjectType.Entity ); } }

    public CarObject(uint id, ECarObjectType type, string name)
    {
      Id   = id;
      Type = type;
      Name = name;
    }
    
    public override bool Equals(System.Object obj)
    {
        // If parameter cannot be cast to ThreeDPoint return false:
        CarObject p = obj as CarObject;
        if ((object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return base.Equals(obj) && (Id == p.Id && Type == p.Type);
    }

    public bool Equals(CarObject p)
    {
        // Return true if the fields match:
        return base.Equals((CarObject)p) && (Id == p.Id && Type == p.Type);
    }


    public static bool operator ==(CarObject a, CarObject b)
    {
      if ( System.Object.ReferenceEquals(a, b) )
      {
        return true;
      }

      if ( (a == null) || (b == null) )
      {
        return false;
      }

      return ( (a.Id == b.Id) && (a.Type == b.Type) );
    }

    public static bool operator !=(CarObject a, CarObject b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
      const int prime = 397;
		  return ( prime * Id.GetHashCode() ) ^ Type.GetHashCode();
	  }
  }

  public abstract class CarBody : CarObject
  {
    public CarBody(uint id, ECarObjectType type, string name)
      : base( id, type, name )
    {

    }

    public void Connect()
    {
      BodyManager.ConnectBody(Id);
    }

    public void Disconnect()
    {
      BodyManager.DisconnectBody(Id, false);
    }
  }

  public class CarRigidBody : CarBody
  {
    public CarRigidBody(uint id, string name )
      : base( id, ECarObjectType.Body | ECarObjectType.RigidBody, name )
    {
    }

    public Vector3 GetPosition()
    {
      return CaronteSharp.RigidbodyManager.Rg_getPosition_WORLD(Id);
    }

    public Quaternion GetRotation()
    {
      return CaronteSharp.RigidbodyManager.Rg_getRotation_WORLD(Id);
    }

    public Vector3 GetEulerAngles()
    {
      return Vector3.zero;
    }
  }

  public class CarBodyMesh : CarBody
  {
    public CarBodyMesh(uint id, string name )
      : base( id, ECarObjectType.Body | ECarObjectType.BodyMesh, name )
    {
    }
  }

  public class CarSoftBody : CarBody
  {
    public CarSoftBody(uint id, string name )
      : base( id, ECarObjectType.Body | ECarObjectType.SoftBody, name )
    {
    }
  }

  public class CarRope : CarSoftBody
  {
    public CarRope(uint id, string name )
      : base( id, name )
    {
      Type |= ECarObjectType.Rope;
    }
  }

  public class CarCloth : CarBody
  {
    public CarCloth(uint id, string name )
      : base( id, ECarObjectType.Cloth, name )
    {
    }
  }

  public class CarJoint : CarObject
  {
    public GameObject GameObject { get; protected set; }

    public CarJoint(uint id, string name)
      : base( id, ECarObjectType.Joint, name)
    {
    }
  }

  public class CarServo : CarObject
  {
    public GameObject GameObject { get; protected set; }

    public CarServo(uint id, string name)
      : base( id, ECarObjectType.Servo, name)
    {
    }

    public void SetTarget(Vector3 target)
    {
      ServosManager.Servo_setTarget(Id, target);
    }

    public void SetTargetDelta(Vector3 targetDelta)
    {
      ServosManager.Servo_setTargetDelta(Id, targetDelta);
    }
  }

  public class CarEntity : CarObject
  {
    public GameObject GameObject { get; protected set; }

    public CarEntity(uint id, string name)
      : base( id, ECarObjectType.Entity, name)
    {
    }
  }

}

