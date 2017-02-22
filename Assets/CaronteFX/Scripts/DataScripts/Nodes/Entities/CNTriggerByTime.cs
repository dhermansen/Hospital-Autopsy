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

namespace CaronteFX
{
  [AddComponentMenu("CaronteFX/Nodes-Do-Not-Use-Directly/CNTriggerByTime")]
  public class CNTriggerByTime : CNTrigger
  {
    public override CommandNode DeepClone(GameObject dataHolder)
    {
      CNTriggerByTime clone = CommandNode.CreateInstance<CNTriggerByTime>(dataHolder);
      CloneData(clone);  
      return clone;
    }

  }
}
