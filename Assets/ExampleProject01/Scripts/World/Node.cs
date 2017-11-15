using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public Vector3 worldPosition;

    public int xCoord;
    public int yCoord;

    #region for path finding
    public Grid next;
    public Grid prev;

    public bool walkable;

    public int gCost;
    public int hCost;
    
    public int fCost { get { return gCost + hCost; } }
    #endregion

    public Grid(bool walkable_, Vector3 worldPosition_)
    {
        walkable = walkable_;
        worldPosition = worldPosition_;
    }
}
