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
using System.Collections;
using System.Collections.Generic;
using CaronteSharp;

namespace CaronteFX
{
  public class CRAnimationData
  {
    public float timeStart_;
    public float timeLenght_;
    public float timeAnimated_;

    public bool overrideAnimatorController_;
    public AnimationClip clip_un_;

    public GameObject[] arrRootGameObjects_;

    public CNAnimatedbody.EAnimationType animationType_;

    public List<CarAnimatorInfo>    listCarAnimatorInfo_;
    public List<CarCRAnimationInfo> listCarCRAnimationInfo_;


    public CRAnimationData(CNAnimatedbodyEditor animNodeEditor)
    {
      timeStart_ = animNodeEditor.Data.TimeStart;
      timeLenght_ = animNodeEditor.Data.TimeLength;
      timeAnimated_ = 0;

      animationType_ = animNodeEditor.getAnimationType();

      if (animationType_ == CNAnimatedbody.EAnimationType.Animator)
      {
        overrideAnimatorController_ = animNodeEditor.Data.OverrideAnimationController;
        clip_un_ = animNodeEditor.Data.UN_AnimationClip;

        arrRootGameObjects_ = animNodeEditor.GetAnimationGameObjects<Animator>();
        listCarAnimatorInfo_ = new List<CarAnimatorInfo>();
        BuildCRAnimatorInfo();
      }
      else if (animationType_ == CNAnimatedbody.EAnimationType.CaronteFX)
      {
        arrRootGameObjects_ = animNodeEditor.GetAnimationGameObjects<CRAnimation>();
        listCarCRAnimationInfo_ = new List<CarCRAnimationInfo>();
        BuildCRAnimationInfo();
      }
    }

    private void BuildCRAnimatorInfo()
    {
      listCarAnimatorInfo_.Clear();
      foreach (GameObject rootGameObject in arrRootGameObjects_)
      {
        listCarAnimatorInfo_.Add(new CarAnimatorInfo(rootGameObject));
      }
    }

    private void BuildCRAnimationInfo()
    {
      listCarCRAnimationInfo_.Clear();
      foreach (GameObject rootGameObject in arrRootGameObjects_)
      {
        listCarCRAnimationInfo_.Add(new CarCRAnimationInfo(rootGameObject));
      }
    }

    public void UpdateInfo()
    {
      if (animationType_ == CNAnimatedbody.EAnimationType.Animator)
      {
        for (int i = 0; i < listCarAnimatorInfo_.Count; i++)
        {
          CarAnimatorInfo animatorInfo = listCarAnimatorInfo_[i];
          GameObject rootGameObject = arrRootGameObjects_[i];

          animatorInfo.AssignTmpAnimatorController(rootGameObject, clip_un_, overrideAnimatorController_);
        }
      }
      else if (animationType_ == CNAnimatedbody.EAnimationType.CaronteFX)
      {
        for (int i = 0; i < listCarCRAnimationInfo_.Count; i++)
        {
          CarCRAnimationInfo crAnimationInfo = listCarCRAnimationInfo_[i];
          GameObject rootGameObject = arrRootGameObjects_[i];

          crAnimationInfo.AssignTmpAnimationController(rootGameObject);
        }
      }
    }


    public void UpdateSimulating(UnityEngine.Mesh animBakingMesh, float eventTime, float deltaTime)
    {
      if (animationType_ == CNAnimatedbody.EAnimationType.Animator)
      {
        foreach (CarAnimatorInfo animatorInfo in listCarAnimatorInfo_)
        {
          animatorInfo.UpdateSimulating(this, animBakingMesh, eventTime, deltaTime);
        }
      }
      else if (animationType_ == CNAnimatedbody.EAnimationType.CaronteFX)
      {
        foreach (CarCRAnimationInfo crAnimationInfo in listCarCRAnimationInfo_)
        {
          crAnimationInfo.UpdateSimulating(this, animBakingMesh, eventTime, deltaTime);
        }
      }
    }

    public void Reset()
    {
      if (animationType_ == CNAnimatedbody.EAnimationType.Animator)
      {
        foreach (CarAnimatorInfo animatorInfo in listCarAnimatorInfo_)
        {
          animatorInfo.Reset();
        }
      }
    }

    public void SetModeAnimation(bool active)
    {
      if (animationType_ == CNAnimatedbody.EAnimationType.Animator)
      {
        foreach (CarAnimatorInfo animatorInfo in listCarAnimatorInfo_)
        {
          animatorInfo.SetModeAnimation(active);
        }
      }
      else if (animationType_ == CNAnimatedbody.EAnimationType.CaronteFX)
      {
        foreach (CarCRAnimationInfo crAnimationInfo in listCarCRAnimationInfo_)
        {
          crAnimationInfo.SetModeAnimation(active);
        }
      }
    }
  }
}
