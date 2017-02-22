#if UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9
#define UNITY_5_3_OR_NEWER
#endif

using UnityEngine;
using System.Collections;



namespace CaronteFX
{
  public class CRDustEventsReceiver : CRExampleEventsReceiver
  {
    public ParticleSystem particleSystem_;
    public float emitfactor_ = 0.01f;

    [System.Serializable]
    public enum EMagnitude
    {
      RelativeVelocityT,
      RelativeVeloctiyN,
      RelativeMomentumN,
      RelativeMomentumT
    }

    public EMagnitude magnitude_ = EMagnitude.RelativeMomentumN;

#if UNITY_5_3_OR_NEWER
    ParticleSystem.EmitParams emitParams_ = new ParticleSystem.EmitParams();
#endif

    void Start()
    {
      particleSystem_.Stop();
#if UNITY_5_3_OR_NEWER
      emitParams_.applyShapeToPosition = true;
#endif
    }

    public override void ProcessAnimationEvent(CRAnimationEvData aeData)
    {
      if (enabled)
      {
        CRContactEvData ceData = (CRContactEvData) aeData;

#if UNITY_5_3_OR_NEWER
        emitParams_.position = ceData.position_;
#endif

        float numParticles = 0;
        //emit particles based on the selected magnitude of the contact   
        switch (magnitude_)
        {
          case EMagnitude.RelativeVeloctiyN:
            numParticles = (int)(ceData.relativeSpeed_N_ * emitfactor_);
            break;
          case EMagnitude.RelativeVelocityT:
            numParticles = (int)(ceData.relativeSpeed_T_ * emitfactor_);
            break;
          case EMagnitude.RelativeMomentumN:
            numParticles = (int)(ceData.relativeP_N_ * emitfactor_);
            break;
          case EMagnitude.RelativeMomentumT:
            numParticles = (int)(ceData.relativeP_T_ * emitfactor_);
            break;     
        }

#if UNITY_5_3_OR_NEWER
        particleSystem_.Emit(emitParams_, (int)numParticles);
#else
        particleSystem_.transform.localPosition = ceData.position_;
        particleSystem_.Emit((int)numParticles);
#endif 

      }
    }
  }
}
