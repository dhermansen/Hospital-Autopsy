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

namespace CaronteFX
{

  [AddComponentMenu("CaronteFX/Miscellaneous/CRCameraCapturer")]
  [DisallowMultipleComponent]
  public class CRCameraCapturer : MonoBehaviour 
  {
      public CRAnimation cranimation;

      public string folder;
      public int supersize;
 
      public int frameRate;
      public int frameCount;

      private int currentFrame;

      void Start() 
      {
        if (cranimation != null)
        {
          cranimation.LoadAnimation(false);
          cranimation.SetFrame(0f);

          frameRate  = (int)cranimation.Fps;
          frameCount = (int)(cranimation.FrameCount / cranimation.speed); 

          Time.captureFramerate = frameRate;
          currentFrame = 0;
        }
      }

      [ExecuteInEditMode]
      void Update() 
      {
        if (cranimation == null)
        {
          return; 
        }

        if ( currentFrame >= (frameCount - 2) )
        {
          return;
        }

        // Append filename to folder name (format is '0005 shot.png"')
        string name = string.Format("{0}/{1:D04} shot.png", folder, currentFrame);

        Application.CaptureScreenshot(name, supersize);
        currentFrame++;
      }
  }
}

