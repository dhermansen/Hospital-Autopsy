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
  public interface IListController
  {
    int    ItemIdxEditing { get; set; }
    string ItemIdxEditingName { get; set; }

    bool BlockEdition { get; }
    int  NumVisibleElements { get; }

    bool ItemIsNull(int itemIdx);
    bool ItemIsBold(int itemIdx);
    bool ItemIsSelectable(int itemIdx);
    bool ItemIsSelected(int itemIdx);
    int  ItemIndentLevel(int itemIdx);
    bool ItemIsGroup(int itemIdx);
    bool ItemIsExpanded(int itemIdx);
    void ItemSetExpanded(int itemIdx, bool open);
    bool ItemIsDraggable(int itemIdx);
    bool ItemHasContext(int itemIdx);
    bool ItemIsEditable(int itemIdx);
    bool ItemIsDisabled(int itemIdx);
    bool ItemIsExcluded(int itemIdx);

    bool ItemIsValidDragTarget(int itemIdx, string dragDropIdentifier);
    bool ListIsValidDragTarget();

    void MoveDraggedItem(int itemFromIdx, int itemToIdx, bool edgeDrag);
    void AddDraggedObjects(int itemToIdx, UnityEngine.Object[] objects);

    void RemoveSelected();
    void UnselectSelected();

    string     GetItemName(int itemIdx);
    void       SetItemName(int itemIdx, string name);
    string     GetItemListName(int itemIdx);
    Texture    GetItemIcon(int itemIdx);

    void OnClickItem(int itemIdx, bool ctrlPressed, bool shiftPressed, bool altPressed, bool isUpClick);
    void OnSelectItemPrev();
    void OnSelectItemNext();
    void OnContextClickItem(int itemIdx, bool ctrlPressed, bool shiftPressed, bool altPressed);
    void OnContextClickList();
    void OnDoubleClickItem(int itemIdx);
    void FinishRenderingItems();
    void GUIUpdateRequested();
  }
}


