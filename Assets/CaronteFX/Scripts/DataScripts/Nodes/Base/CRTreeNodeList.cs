// ***********************************************************
//	Copyright 2016 Next Limit Technologies, http://www.nextlimit.com
//	All rights reserved.
//
//	THIS SOFTWARE IS PROVIDED 'AS IS' AND WITHOUT ANY EXPRESS OR
//	IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
//	WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
//
// ***********************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaronteFX
{
  /// <summary>
  /// Each node holds a tree node list 
  /// so it can have references to child nodes.
  /// </summary>
  [Serializable]
  public class CRTreeNodeList : IEnumerable
  {
    [SerializeField]
    private CRTreeNode Parent;

    [SerializeField]
    private List<CRTreeNode>  listTreeNode_;
 
    public CRTreeNodeList(CRTreeNode parent)
    {
      this.Parent = parent;
      listTreeNode_ = new List<CRTreeNode>();
    }
 
    public void Add(CRTreeNode node)
    {
      listTreeNode_.Add(node);
      node.Parent = Parent;
    }

    public void AddAfter(CRTreeNode node, CRTreeNode previousNode)
    {
      int indexOfPrevious = listTreeNode_.IndexOf( previousNode );
      if (indexOfPrevious == -1)
      {
        listTreeNode_.Insert(0, node);
      }
      else
      {
        listTreeNode_.Insert(indexOfPrevious+1, node);
      }   
      node.Parent = Parent;
    }

    public bool Remove(CRTreeNode node)
    {
      return (listTreeNode_.Remove(node));
    }

    public bool Contains(CRTreeNode node)
    {
      return (listTreeNode_.Contains(node));
    }

    public void RemoveAt(int childIdx)
    {
      listTreeNode_.RemoveAt(childIdx);
    }

    public int Count
    {
      get
      {
        return listTreeNode_.Count;
      }
    }

    public CRTreeNode this[int i]
    {
      get { return listTreeNode_[i]; }
      set { listTreeNode_[i] = value; }
    }

    public override string ToString()
    {
        return "Count=" + listTreeNode_.Count.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
       return (IEnumerator) GetEnumerator();
    }

    public CRTreeNodeEnum GetEnumerator()
    {
        return new CRTreeNodeEnum(listTreeNode_);
    }

  } //CRTreeNodeList...

  public class CRTreeNodeEnum : IEnumerator
  {
    public List<CRTreeNode> listTreeNode_;
    int position = -1;

    public CRTreeNodeEnum(List<CRTreeNode> list)
    {
      listTreeNode_ = list;
    }

    public bool MoveNext()
    {
      position++;
      return (position < listTreeNode_.Count);
    }

    public void Reset()
    {
      position = -1;
    }

    object IEnumerator.Current
    {
      get
      {
        return Current;
      }
    }

    public CRTreeNode Current
    {
      get
      {
        try
        {
          return listTreeNode_[position];
        }
        catch (IndexOutOfRangeException)
        {
          throw new InvalidOperationException();
        }
      }
    }

  } //class CRTreeNodeEnum...

} //namespace CaronteFX...

