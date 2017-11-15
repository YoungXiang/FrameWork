using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinding
{
    public World world;

    private List<Grid> neighbours = new List<Grid>(8);

    public void FindPath(Vector3 startPos_, Vector3 targetPos_)
    {
        Grid startGrid = world.GridFromWorldPosition(startPos_);
        Grid targetGrid = world.GridFromWorldPosition(targetPos_);

        List<Grid> openSet = new List<Grid>();
        HashSet<Grid> closedSet = new HashSet<Grid>();
        openSet.Add(startGrid);

        while(openSet.Count > 0)
        {
            Grid currentGrid = openSet[0];
            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentGrid.fCost || 
                    (openSet[i].fCost == currentGrid.fCost && openSet[i].hCost < currentGrid.hCost))
                {
                    currentGrid = openSet[i];
                }
            }

            openSet.Remove(currentGrid);
            closedSet.Add(currentGrid);

            if (currentGrid == targetGrid)
            {
                return;
            }

            world.GetNeighbours(currentGrid, neighbours);
            for (int i = 0; i < neighbours.Count; i++)
            {
                Grid checkGrid = neighbours[i];
                if (!checkGrid.walkable || closedSet.Contains(checkGrid))
                {
                    continue;
                }

                int newCost = currentGrid.gCost + CalculateCost(currentGrid, checkGrid);
                if (newCost < checkGrid.gCost || !openSet.Contains(checkGrid))
                {
                    checkGrid.gCost = newCost;
                    checkGrid.hCost = CalculateCost(checkGrid, targetGrid);
                    //currentGrid.next = checkGrid;
                    checkGrid.prev = currentGrid;

                    if (!openSet.Contains(checkGrid)) openSet.Add(checkGrid);
                }
            }
        }
    }

    int CalculateCost(Grid a, Grid b)
    {
        int distX = Mathf.Abs(a.xCoord - b.xCoord);
        int distY = Mathf.Abs(a.yCoord - b.yCoord);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }
}
