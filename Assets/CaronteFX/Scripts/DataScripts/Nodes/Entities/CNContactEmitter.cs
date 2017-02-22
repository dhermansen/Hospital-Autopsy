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


namespace CaronteFX
{
  /// <summary>
  /// Contacts the data of a contact emitter node. (Entity which emit contact events)
  /// </summary>
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNContactEmitter")] 
  public class CNContactEmitter : CNEntity 
  {

    [SerializeField]
           CNField fieldA_;
    public CNField FieldA
    {
      get
      {
        if (fieldA_ == null)
        {
          CNField.AllowedTypes allowedTypes =   CNField.AllowedTypes.Geometry
                                              | CNField.AllowedTypes.RigidBodyNode;
                      
          fieldA_ = new CNField( false, allowedTypes, false );
        }
        return fieldA_;
      }
    }

    [SerializeField]
           CNField fieldB_;
    public CNField FieldB
    {
      get
      {
        if (fieldB_ == null)
        {
          CNField.AllowedTypes allowedTypes =   CNField.AllowedTypes.Geometry
                                              | CNField.AllowedTypes.RigidBodyNode;
                      
          fieldB_ = new CNField( false, allowedTypes, false );
        }
        return fieldB_;
      }
    }


    public enum EmitModeOption
    {
      OnlyFirst,
      All
    }

    [SerializeField]
    private EmitModeOption emitMode_ = EmitModeOption.All;
    public EmitModeOption EmitMode
    {
      get { return emitMode_; }
      set { emitMode_ = value; }
    }

    [SerializeField]
    private int maxEventsPerSecond_ = 100;
    public int MaxEventsPerSecond
    {
      get { return maxEventsPerSecond_; }
      set { maxEventsPerSecond_ = value; }
    }

    [SerializeField]
    private float relativeSpeedMin_N_ = 0.1f;
    public float RelativeSpeedMin_N
    {
      get { return relativeSpeedMin_N_; }
      set { relativeSpeedMin_N_ = value; }
    }

    [SerializeField]
    private float relativeSpeedMin_T_ = 0.0f;
    public float RelativeSpeedMin_T
    {
      get { return relativeSpeedMin_T_; }
      set { relativeSpeedMin_T_ = value; }
    }

    [SerializeField]
    private float relativeMomentum_N_ = 0f;
    public float RelativeMomentum_N
    {
      get { return relativeMomentum_N_; }
      set { relativeMomentum_N_ = value;}
    }

    [SerializeField]
    private float relativeMomentum_T_ = 0f;
    public float RelativeMomentum_T
    {
      get { return relativeMomentum_T_; }
      set { relativeMomentum_T_ = value; }
    }

    [SerializeField]
    private float lifeTimMinInSecs_ = 0.1f;
    public float LifeTimeMin
    {
      get { return lifeTimMinInSecs_; }
      set { lifeTimMinInSecs_ = value; }
    }

    [SerializeField]
    private float collapseRadius_ = 0f;
    public float CollapseRadius
    {
      get { return collapseRadius_; }
      set { collapseRadius_ = value; }
    }
    //----------------------------------------------------------------------------------
    protected override void CloneData(CommandNode node)
    {
      base.CloneData(node);
      CNContactEmitter clone = (CNContactEmitter)node;

      clone.fieldA_ = fieldA_.DeepClone();
      clone.fieldB_ = fieldB_.DeepClone();

      clone.emitMode_           = emitMode_;
      clone.maxEventsPerSecond_ = maxEventsPerSecond_;
      clone.relativeSpeedMin_N_ = relativeSpeedMin_N_;
      clone.relativeSpeedMin_T_ = relativeSpeedMin_T_;
      clone.relativeMomentum_N_ = relativeMomentum_N_;
      clone.relativeMomentum_T_ = relativeMomentum_T_;
      clone.lifeTimMinInSecs_   = lifeTimMinInSecs_;
      clone.collapseRadius_     = collapseRadius_;
    }
    //----------------------------------------------------------------------------------
    public override CommandNode DeepClone(GameObject dataHolder)
    {
      CNContactEmitter clone = CommandNode.CreateInstance<CNContactEmitter>(dataHolder);   
      CloneData(clone);
      return clone;
    }
    //----------------------------------------------------------------------------------
    public override bool UpdateNodeReferences(Dictionary<CommandNode, CommandNode> dictNodeToClonedNode)
    {
      bool wasAnyUpdatedA = fieldA_.UpdateNodeReferences(dictNodeToClonedNode);
      bool wasAnyUpdatedB = fieldB_.UpdateNodeReferences(dictNodeToClonedNode);

      return (wasAnyUpdatedA || wasAnyUpdatedB);
    }
    //----------------------------------------------------------------------------------
  }
}
