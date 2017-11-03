using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class SwipePageControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public delegate void OnSwipiePageChangedEvent(int newPage, int oldPage);
    public OnSwipiePageChangedEvent OnSwipiePageChanged;

    // define which item's position is the center;
    public int centerItem = 0;

    internal List<float> itemPositions;
    internal ScrollRect scrollRect;
    internal Vector2 orientation;
    internal int curItem;
    internal int targetItem;
    //internal Vector2 scrollSpeedVector;
    internal Vector2 basePosition;

    #region privates
    private bool bScrollDirty;
    private bool bChildDirty;

    private bool isDraging = false;
    private Vector2 positionDeltaDrag = Vector2.zero;
    private float timeElapsed = 0.0f;
    private float itemBaseLength = 0.0f;   // can either be height or width;
    private float scrollLength = 0.0f;
    private float scrollTimeLeft = 0.2f;
    private float scrollSpeed = 0.0f;
    #endregion

    #region Interfaces
    /// <summary>
    /// Touch screen to start swiping
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDraging = true;
        positionDeltaDrag = scrollRect.content.anchoredPosition;
        timeElapsed = Time.time;
    }

    /// <summary>
    /// While dragging do
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDraging)
        {
            isDraging = false;
            timeElapsed = Time.time - timeElapsed;
            positionDeltaDrag = scrollRect.content.anchoredPosition - positionDeltaDrag;

            Debug.Log("-------------OnEndDrag Item " + positionDeltaDrag[GetOrientation()] + ", " + itemBaseLength);

            // check if is scroll dirty:
            bool shouldScrollToNext = Mathf.Abs(positionDeltaDrag[GetOrientation()]) > itemBaseLength * 0.2f || (timeElapsed <= 0.25f && Mathf.Abs(positionDeltaDrag[GetOrientation()]) > itemBaseLength * 0.05f);
            if (shouldScrollToNext)
            {
                if (positionDeltaDrag[GetOrientation()] > 0)
                {
                    SetTargetItem(curItem + (GetOrientation() == 0 ? -1 : 1), true);
                }
                else
                {
                    SetTargetItem(curItem + (GetOrientation() == 0 ? 1 : -1), true);
                }
            }
            else
            {
                if (curItem == 0)
                {
                    if (positionDeltaDrag[GetOrientation()] < 0)
                        SetTargetItem(curItem, true);
                }
                else if (curItem == itemPositions.Count - 1)
                {
                    if (positionDeltaDrag[GetOrientation()] > 0)
                        SetTargetItem(curItem, true);
                }
                else
                {
                    SetTargetItem(curItem, true);
                }
            }
        }
    }
    #endregion

    void Awake()
    {
        itemPositions = new List<float>();
        scrollRect = GetComponent<ScrollRect>();
        orientation = new Vector2(scrollRect.horizontal ? 1 : 0, scrollRect.horizontal ? 0 : 1);
        //scrollSpeedVector = orientation * scrollSpeed;
        
        basePosition = Vector2.zero;
        basePosition.x = scrollRect.horizontal ? 0 : scrollRect.content.anchoredPosition.x;
        basePosition.y = scrollRect.horizontal ? scrollRect.content.anchoredPosition.y : 0;

        FetchChildItems();
    }

	// Update is called once per frame
	void Update ()
    {
        if (bChildDirty)
        {
            FetchChildItems();
        }
        
        if (bScrollDirty && !isDraging)
        {
            UpdateScroll();
        }
	}

    void UpdateScroll()
    {
        scrollTimeLeft -= Time.deltaTime;
        if (scrollTimeLeft > 0)
        {
            float moveVec = scrollSpeed * Time.deltaTime;
            Vector2 newPos = scrollRect.content.anchoredPosition;
            float basePosition = itemPositions[centerItem] - itemPositions[targetItem];
            if (Mathf.Abs(moveVec) <= Mathf.Abs(basePosition - newPos[GetOrientation()]))
            {
                newPos[GetOrientation()] += moveVec;
                scrollRect.content.anchoredPosition = newPos;
            }
        }
        else// if (bScrollDirty)
        {
            CenterItem(targetItem);
            bScrollDirty = false;
        }
    }

    void CenterItem(int item)
    {
        if (OnSwipiePageChanged != null)
        {
            OnSwipiePageChanged(item, curItem);
        }

        curItem = item;

        basePosition[GetOrientation()] = itemPositions[centerItem] - itemPositions[curItem];
        Debug.Log("-------------CenterItem Item  :" + curItem + " / " + basePosition);
        scrollRect.content.anchoredPosition = basePosition;
    }

    void FetchChildItems()
    {
        ClearChildItems();
        int totalItems = 0;
        itemBaseLength = 0.0f;
        foreach (Transform child in scrollRect.content.transform)
        {
            if (child.gameObject.activeSelf)
            {
                totalItems++;
                itemBaseLength += GetOrientation() == 0 ? (child as RectTransform).rect.width : (child as RectTransform).rect.height;
                itemPositions.Add((child as RectTransform).anchoredPosition[GetOrientation()]);
            }
        }
        itemBaseLength /= totalItems;
        bChildDirty = false;

        if (itemPositions.Count > 0)
        {
            SetTargetItem(targetItem);
        }
    }

    int GetOrientation()
    {
        return (orientation.x == 0) ? 1 : 0;
    }

    #region public functions
    public void ClearChildItems() { itemPositions.Clear(); }

    public void SetChildDirty()
    {
        bChildDirty = true; targetItem = 0;
    }

    public void SetTargetItem(int itemIndex, bool scrollToItem = false)
    {
        targetItem = itemIndex;

        if (itemIndex < 0 || itemIndex >= itemPositions.Count) return;
        if (scrollToItem)
        {
            bScrollDirty = true;
            scrollLength = itemPositions[centerItem] - itemPositions[targetItem] - scrollRect.content.anchoredPosition[GetOrientation()];
            scrollTimeLeft = 0.2f;
            scrollSpeed = scrollLength / scrollTimeLeft;
        }
        else if (!bChildDirty && curItem != targetItem)
        {
            CenterItem(targetItem);
        }
    }
    #endregion
}
