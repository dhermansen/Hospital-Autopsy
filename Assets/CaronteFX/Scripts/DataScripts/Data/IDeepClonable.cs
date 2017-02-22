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
  /// General interface of clonable classes.
  /// </summary>
  public interface IDeepClonable<T>
  {
    T DeepClone();
  }

  /// <summary>
  /// General interface of clonable mono classes.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IMonoDeepClonable<T>
  {
    T DeepClone( GameObject fxData );
    bool UpdateNodeReferences( Dictionary<CommandNode, CommandNode> dictNodeToClonedNode );
  }

}//namesapce CaronteFX...