using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;
using System;

public class Grid : IHeapItem<Grid>
{
    public Vector3 worldPosition;
    // the height calculated using Physics
    public float groundHeight;

    public int xCoord;
    public int yCoord;
    
    #region for path finding
    //public Grid next;
    public Grid prev;

    public bool walkable;

    public int gCost;
    public int hCost;
    
    public int fCost { get { return gCost + hCost; } }

    private int heapIndex;
    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }
    #endregion

    public Grid(bool walkable_, Vector3 worldPosition_)
    {
        walkable = walkable_;
        worldPosition = worldPosition_;
    }
    
    public int CompareTo(Grid other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
}
