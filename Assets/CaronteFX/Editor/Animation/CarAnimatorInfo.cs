using UnityEngine;
using System.Collections.Generic;
using CaronteSharp;

namespace CaronteFX
{
  public class CarAnimatorInfo : CarBodyDataInfo
  {
    Animator animator_;
    RuntimeAnimatorController runtimeAnimationController_;
    AnimatorOverrideController ovrrAnimationController_;

    public static RuntimeAnimatorController animatorSampler_;

    bool overrideAnimatorController_;

    public CarAnimatorInfo(GameObject rootGameObject)
    {
      CREditorUtils.GetRenderersFromRoot(rootGameObject, out arrNormalMeshRenderer_, out arrSkinnedMeshRenderer_);
      AssignBodyIds();
    }

    public void AssignTmpAnimatorController(GameObject rootGameObject, AnimationClip animationClip, bool overrideAnimatorController)
    {
      CREditorUtils.GetRenderersFromRoot(rootGameObject, out arrNormalMeshRenderer_, out arrSkinnedMeshRenderer_);
      animator_ = rootGameObject.GetComponent<Animator>();

      if (overrideAnimatorController)
      {
        animator_.runtimeAnimatorController = animatorSampler_;

        runtimeAnimationController_ = animator_.runtimeAnimatorController;
        ovrrAnimationController_ = new AnimatorOverrideController();

        ovrrAnimationController_.runtimeAnimatorController = runtimeAnimationController_;

        AnimationClip[] clips = ovrrAnimationController_.animationClips;
        foreach (AnimationClip animClip in clips)
        {
          ovrrAnimationController_[animClip] = animationClip;
        }
        animator_.runtimeAnimatorController = ovrrAnimationController_;
      }
    }

    public void UpdateSimulating(CRAnimationData animData, UnityEngine.Mesh animBakingMesh, float eventTime, float deltaTime)
    {
      if (animator_ == null)
      {
        return;
      }

      animator_.Update(deltaTime);

      double targetTime = eventTime + deltaTime;

      for (int i = 0; i < arrSkinnedMeshRenderer_.Length; ++i)
      {
        uint idBody = arrIdBodySkinnedGameObjects_[i];
        SkinnedMeshRenderer smRenderer = arrSkinnedMeshRenderer_[i];

        GameObject gameObject = smRenderer.gameObject;

        smRenderer.BakeMesh(animBakingMesh);

        if (idBody != uint.MaxValue)
        {
          Matrix4x4 m_MODEL_TO_WORLD = gameObject.transform.localToWorldMatrix;
          RigidbodyManager.Rg_addEventTargetArrPos_WORLD((double)eventTime, targetTime, idBody, ref m_MODEL_TO_WORLD, animBakingMesh.vertices);
        }
      }

      for (int i = 0; i < arrNormalMeshRenderer_.Length; ++i)
      {
        uint idBody = arrIdBodyNormalGameObjects_[i];
        MeshRenderer renderer = arrNormalMeshRenderer_[i];
        GameObject gameObject = renderer.gameObject;

        if (idBody != uint.MaxValue)
        {
          Matrix4x4 m_MODEL_TO_WORLD = gameObject.transform.localToWorldMatrix;
          RigidbodyManager.Rg_addEventTargetPos_WORLD((double)eventTime, targetTime, idBody, ref m_MODEL_TO_WORLD, 0.01);
        }
      }
      animData.timeAnimated_ += deltaTime;
    }

    public void Reset()
    {
      Object.DestroyImmediate(ovrrAnimationController_);
    }
  }

}

