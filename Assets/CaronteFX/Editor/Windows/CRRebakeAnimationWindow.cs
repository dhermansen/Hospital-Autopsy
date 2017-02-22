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
using System.IO;
using System.Collections.Generic;
using CaronteFX.AnimationFlags;
using CaronteSharp;


namespace CaronteFX
{
  using TGOHeaderData   = Tuple4<string, int, int, List<string>>;
  using TVisibilityData = Tuple2<Transform, Vector2>;
  using TGOFrameData    = Tuple2<Transform, CRGOKeyframe>;

  public class CRRebakeAnimationWindow : CRWindow<CRRebakeAnimationWindow>
  {
    CRAnimation crAnimation_;

    int trimFrameStart_ = 0;
    int trimFrameEnd_   = 0;
    int trimFrameLast_  = 0;

    bool vertexCompression_  = true;
    bool saveTangents_       = false;
    bool alignData_          = true;

    int binaryVersion_ = 9;

    private string[] arrVertexCompressionModes_;
    private int vertexCompressionIdx_ = 0;

    public static CRRebakeAnimationWindow ShowWindow(CRAnimation crAnimation)
    {
      if (Instance == null)
      {
        Instance = (CRRebakeAnimationWindow)EditorWindow.GetWindow(typeof(CRRebakeAnimationWindow), true, "CaronteFX - Rebake animation");
        Instance.crAnimation_ = crAnimation;
      }

      float width  = 320f;
      float height = 180f;

      Instance.minSize = new Vector2(width, height);
      Instance.maxSize = new Vector2(width, height);

      crAnimation.LoadAnimation(true);

      Instance.trimFrameStart_    = 0;
      Instance.trimFrameEnd_      = crAnimation.LastFrame;
      Instance.trimFrameLast_     = crAnimation.LastFrame;

      Instance.Focus();
      return Instance;
    }

    void OnEnable()
    {
      if (CRVersionChecker.IsAdvanceCompressionVersion())
      {
        arrVertexCompressionModes_ = new string[2] { "Box (medium compression)", "Fiber (high compression)" };
      }
      else
      {
        arrVertexCompressionModes_ = new string[1] { "Box (medium compression)" };
      }
    }

    void OnLostFocus()
    {
      Close();
    }

    private void SetStartEndFrames(int startFrame, int endFrame)
    {
      trimFrameStart_ = Mathf.Clamp( startFrame, 0, trimFrameLast_);
      trimFrameEnd_   = Mathf.Clamp( endFrame, 0, trimFrameLast_);
    }

    public void DrawRebake()
    {
      float start = EditorGUILayout.IntField("Frame Start : ", trimFrameStart_ );
      float end   = EditorGUILayout.IntField("Frame End   : ", trimFrameEnd_ );

      EditorGUILayout.MinMaxSlider( new GUIContent("Frames:"), ref start, ref end, 0, trimFrameLast_ );
      SetStartEndFrames( (int)start, (int)end );

      EditorGUILayout.Space();

      vertexCompression_ = EditorGUILayout.Toggle("Vertex compression", vertexCompression_);

      EditorGUI.BeginDisabledGroup(!vertexCompression_);
      vertexCompressionIdx_ = EditorGUILayout.Popup( "Compression mode", vertexCompressionIdx_, arrVertexCompressionModes_);
      EditorGUI.EndDisabledGroup();

      bool isFiberCompression = vertexCompression_ && vertexCompressionIdx_ == 1;

      EditorGUI.BeginDisabledGroup(isFiberCompression);
      saveTangents_ = EditorGUILayout.Toggle("Save tangents", saveTangents_);
      EditorGUI.EndDisabledGroup();

      alignData_ = EditorGUILayout.Toggle("Align data", alignData_);

      EditorGUILayout.Space();
      CRGUIUtils.Splitter();

      if ( GUILayout.Button("Rebake animation") )
      {
        if (trimFrameEnd_ <= trimFrameStart_)
        {
          EditorUtility.DisplayDialog("CaronteFX - Invalid frames", "Frame End must be above Frame Start", "Ok");
          return;
        }
        bool isTextAsset = crAnimation_.animationFileType == CRAnimation.AnimationFileType.TextAsset;
        RebakeClip(isTextAsset);
      }
    }

    public void OnGUI()
    {
      EditorGUILayout.Space();
      DrawRebake();
      EditorGUILayout.Space();
    }

    private void RebakeClip(bool isTextAsset)
    {
      bool wasInterpolating = crAnimation_.interpolate;
      crAnimation_.interpolate = false;

      int   nFrames;
      int   originalFps;
      float deltaTimeFrame;
      float animationLength;

      crAnimation_.InitAnimationBaking(out nFrames, out originalFps, out deltaTimeFrame, out animationLength);

      List<TGOHeaderData> listGOHeaderData = new List<TGOHeaderData>();
      crAnimation_.GetGOHeaderData(listGOHeaderData);

      List<TVisibilityData> listGOVisibilityData = new List<TVisibilityData>();
      crAnimation_.GetVisibilityData(listGOVisibilityData);

      MemoryStream msW = new MemoryStream();

      if ( msW != null )
      {
        BinaryWriter bw = new BinaryWriter(msW);

        if (bw != null)
        {
          //version
          int version = binaryVersion_;
          bw.Write( version );
          //vertexCompression
          bw.Write( vertexCompression_ );
          //vertexSaveTangents
          bw.Write( saveTangents_ );
          //frameCount
          int newFrameCount = trimFrameEnd_ - trimFrameStart_ + 1;
          bw.Write( newFrameCount );
          //fps
          bw.Write( originalFps );
          //nGameObjects
          int nGameObjects = listGOHeaderData.Count;
          bw.Write( nGameObjects );

          EFileHeaderFlags fileheaderFlags = EFileHeaderFlags.NONE;
          if (alignData_)
          {
            fileheaderFlags |= EFileHeaderFlags.ALIGNEDDATA;
          }
       
          if (vertexCompression_)
          {
            if (vertexCompressionIdx_ == 0)
            {
              fileheaderFlags |= EFileHeaderFlags.BOXCOMPRESSION;
            }
            else if ( vertexCompressionIdx_ == 1)
            {
              fileheaderFlags |= EFileHeaderFlags.FIBERCOMPRESSION;
              fileheaderFlags |= EFileHeaderFlags.VERTEXLOCALSYSTEMS;
            }
          }
          bool isFiberCompression = fileheaderFlags.FlagCheck(EFileHeaderFlags.FIBERCOMPRESSION);

          bw.Write((uint)fileheaderFlags);

          int updateFramesDelta = Mathf.Min( Mathf.Max(newFrameCount / 5, 1), 5);
          int frame = 0;

          List<TGOFrameData> listGOFrameData = new List<TGOFrameData>();
          crAnimation_.GetFrameGOData(0, listGOFrameData);

          VerticesAnimationCompressor[] arrVerticesAnimationCompressor = new VerticesAnimationCompressor[nGameObjects];
          if (isFiberCompression)
          {
            for (int i = 0; i < listGOFrameData.Count; i++)
            {
              TGOFrameData goFrameData = listGOFrameData[i];
              Transform tr = goFrameData.First;
              CRGOKeyframe goKeyframe = goFrameData.Second;
              CRFrameWriterUtils.InitVerticesAnimationCompressor(tr, goKeyframe, ref arrVerticesAnimationCompressor[i]);
            }
            
            EditorUtility.DisplayProgressBar("CaronteFX - Rebake", "Rebake first pass. ", 0f);
            for (int i = trimFrameStart_; i <= trimFrameEnd_; i++)
            {
              if( (frame % updateFramesDelta) == 0 )
              {        
                EditorUtility.DisplayProgressBar("CaronteFX - Rebake", "Rebake first pass. Frame " + frame + ".", (float)frame/(float)newFrameCount );
              }
  
              crAnimation_.GetFrameGOData(i, listGOFrameData);

              for (int j = 0; j < listGOFrameData.Count; j++)
              {
                TGOFrameData goFrameData = listGOFrameData[j];
                CRGOKeyframe goKeyframe = goFrameData.Second;
                VerticesAnimationCompressor vac = arrVerticesAnimationCompressor[j];
                CRFrameWriterUtils.VerticesAnimationCompressorFirstPass(goKeyframe, vac);
              }

              frame++;
            }
          }

          crAnimation_.GetFrameGOData(0, listGOFrameData);

          VertexNormalsFastUpdater vnfu = null;
          if (CRVersionChecker.IsAdvanceCompressionVersion())
          {
            vnfu = new VertexNormalsFastUpdater();
          }
  
          for (int i = 0; i < nGameObjects; i++)
          {
            TGOHeaderData goHeaderData = listGOHeaderData[i];

            if (goHeaderData == null)
            {
              continue;
            }

            string relativePath = goHeaderData.First;
            bw.Write( relativePath );
          
            int vertexCount = goHeaderData.Second;;
            bw.Write( vertexCount );

            int boneCount = goHeaderData.Third;
            bw.Write( boneCount );
            
            TVisibilityData visibilityData = listGOVisibilityData[i];
            Vector2 visibilityInterval = visibilityData.Second;
            float visibilityStart = visibilityInterval.x;
            float visibilityEnd   = visibilityInterval.y;

            float visibilityShift = trimFrameStart_ * deltaTimeFrame;
            float rangeMin = 0f;
            float rangeMax = (newFrameCount - 1) * deltaTimeFrame;

            float newStart = Mathf.Clamp(visibilityStart - visibilityShift, rangeMin, rangeMax);
            float newEnd   = Mathf.Clamp(visibilityEnd - visibilityShift,   rangeMin, rangeMax);

            bw.Write( newStart );
            bw.Write( newEnd );

            VerticesAnimationCompressor vac = arrVerticesAnimationCompressor[i];
            if ( vac != null)
            {       
              byte[] definitionBytes = vac.GetDefinitionAsBytes();
              bw.Write(definitionBytes);
            }

            if (isFiberCompression && vertexCount > 0)
            {
              TGOFrameData goFrameData = listGOFrameData[i];
              Transform tr = goFrameData.First;
              CRGOKeyframe goKeyframe = goFrameData.Second;
              GameObject go = tr.gameObject;

              Mesh mesh = go.GetMesh();
              if (mesh != null)
              {
                List<Vector3> listPosition = goKeyframe.GetVertexesPosInFrame();
                List<Vector3> listNormal = goKeyframe.GetVertexesNorInFrame();

                vnfu.Calculate(listPosition.ToArray(), listNormal.ToArray(), mesh.triangles);
                byte[] vertexDataBytes = vnfu.VertexDataAsBytes();
                bw.Write(vertexDataBytes);
              }
            }

            List<string> listBoneRelativePath = goHeaderData.Fourth; 
            for (int j = 0; j < boneCount; j++)
            {
              string boneRelativePath =  listBoneRelativePath[j];
              bw.Write( boneRelativePath );
            }
          }

          if (vnfu != null)
          {
           vnfu.Dispose();
          }

          //TODO: rebake contact event info
          int nEmitters = 0;
          bw.Write( nEmitters );

          long[] fOffsets = new long[newFrameCount];
          long fOffsetsPosition = msW.Position;
          for (int i = 0; i < newFrameCount; i++)
          {
            long val = 0;
            bw.Write(val);
          }

          frame = 0;
          for (int i = trimFrameStart_; i <= trimFrameEnd_; i++)
          {
            if( (frame % updateFramesDelta) == 0 )
            {        
              EditorUtility.DisplayProgressBar( "CaronteFX - Rebake", "Rebaking. Frame " + frame + ".", (float)frame/(float)newFrameCount );
            }

            fOffsets[frame] = msW.Position;
            crAnimation_.GetFrameGOData(i, listGOFrameData);

            for (int j = 0; j < listGOFrameData.Count; j++)
            {
              CRGOKeyframe goKeyframe = listGOFrameData[j].Second;
              VerticesAnimationCompressor vac = arrVerticesAnimationCompressor[j];
              CRFrameWriterUtils.WriteGOKeyframe(goKeyframe, vac, vertexCompression_, saveTangents_, fileheaderFlags, msW, bw);
            }

            //TODO: bakeFrameEvents
            int nFrameEvents = 0;
            bw.Write(nFrameEvents);

            frame++;
          }

          EditorUtility.ClearProgressBar();

          foreach(VerticesAnimationCompressor vac in arrVerticesAnimationCompressor)
          {
            if (vac != null)
            {
              vac.Dispose();
            }
          }

          msW.Position = fOffsetsPosition;
          for (int i = 0; i < newFrameCount; i++)
          {
            bw.Write(fOffsets[i]);
          }

          CreateAssetAndAnimationComponent(msW, isTextAsset);

          bw.Close();   
          msW.Close();
        }
      }

      crAnimation_.FinishAnimationBaking();
      crAnimation_.interpolate = wasInterpolating;
    }

    private void WriteMatchingPadding(long[] framOffsetsOld, MemoryStream msW, BinaryWriter bw)
    {
      int readPadding = GetPadding4((int)framOffsetsOld[trimFrameStart_]);
      int writePadding = GetPadding4((int)msW.Position);

      byte junk = 0;
      while (writePadding != readPadding)
      {
        bw.Write(junk);
        writePadding++;
        writePadding = writePadding++ % 4;
      }
    }

    private int GetPadding4(int cursor)
    {
      return cursor % 4;   
    }

    private void CreateAssetAndAnimationComponent( MemoryStream ms, bool isTextAsset )
    {
      AssetDatabase.Refresh(); 

      if (!isTextAsset)
      {
        CRAnimationAsset animationAsset = CRAnimationAsset.CreateInstance<CRAnimationAsset>();
        animationAsset.Bytes = ms.ToArray();

        CRAnimationAsset animationAssetOld = crAnimation_.activeAnimation;

        string path = AssetDatabase.GetAssetPath(animationAssetOld);
    
        string name = animationAssetOld.name;
        int index = path.IndexOf(name + ".asset");
        string newFolder = path.Substring(0, index);

        string assetFilePath = AssetDatabase.GenerateUniqueAssetPath( newFolder + name + "_rebake.asset" );
        AssetDatabase.CreateAsset( animationAsset, assetFilePath );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(); 

        crAnimation_.AddAnimation(animationAsset);
        EditorGUIUtility.PingObject(animationAsset);
      }
      else
      {
        TextAsset textAsset = crAnimation_.activeAnimationText;

        string path = AssetDatabase.GetAssetPath(textAsset);
    
        string name = textAsset.name;
        int index = path.IndexOf(name + ".bytes");
        string newFolder = path.Substring(0, index);

        string assetFilePath = AssetDatabase.GenerateUniqueAssetPath( newFolder + name + "_rebake.bytes" );

        FileStream fs = new FileStream(assetFilePath, FileMode.Create);
        byte[] arrByte = ms.ToArray();
        fs.Write(arrByte, 0, arrByte.Length);
        fs.Close();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(); 

        TextAsset crAnimationText = (TextAsset)AssetDatabase.LoadAssetAtPath(assetFilePath, typeof(TextAsset));
        crAnimation_.AddAnimation(crAnimationText);
        EditorGUIUtility.PingObject(crAnimationText);
      }

      EditorUtility.SetDirty(crAnimation_);
    }
  }
}