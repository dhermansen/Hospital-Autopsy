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

namespace CaronteFX
{
  public class CRWindow<T> : EditorWindow 
    where T:EditorWindow
  {

    public static T Instance { get; protected set; }
    public static bool IsOpen
    {
      get
      {
        return ( Instance != null );
      }
    }

    public static bool IsFocused
    {
      get
      {
        return ( EditorWindow.focusedWindow == Instance );
      }
    }

    public static void CloseIfOpen()
    {
      if (Instance != null)
      {
        Instance.Close();
      }
    }

    public static void RepaintIfOpen()
    {
      if (Instance != null)
      {
        Instance.Repaint();
      }
    }


  }
}

