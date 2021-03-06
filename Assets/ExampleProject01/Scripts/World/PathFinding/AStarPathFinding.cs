﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;

public class AStarPathFinding
{
    protected World world;
    Heap<Grid> openSet;
    HashSet<Grid> closedSet = new HashSet<Grid>();

    public AStarPathFinding(World world_)
    {
        world = world_;
        openSet = new Heap<Grid>(world.MaxLength);
    }

    private List<Grid> neighbours = new List<Grid>(8);

    public void FindPath(Vector3 startPos_, Vector3 targetPos_, List<Grid> outPath)
    {
        Grid startGrid = world.GridFromWorldPosition(startPos_);
        Grid targetGrid = world.GridFromWorldPosition(targetPos_);
        if (!targetGrid.walkable) return;

        openSet.Clear();
        closedSet.Clear();
        
        // 1. Add startGrid into open list
        openSet.Add(startGrid);
        // 2. Enter while loop
        while(openSet.Count > 0)
        {
            // 3. Find the minimum cost in the open list
            Grid currentGrid = openSet[0];
            /*
            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentGrid.fCost || 
                    (openSet[i].fCost == currentGrid.fCost && openSet[i].hCost < currentGrid.hCost))
                {
                    currentGrid = openSet[i];
                }
            }
            openSet.Remove(currentGrid);
            */
            openSet.PopFront();
            // 4. Add into the closed list
            closedSet.Add(currentGrid);

            // 5. if current node is the target node, return.
            if (currentGrid == targetGrid)
            {
                RetracePath(startGrid, targetGrid, outPath);
                return;
            }

            // 6. else, check the neighbours around the current node.
            world.GetNeighbours(currentGrid, neighbours);
            for (int i = 0; i < neighbours.Count; i++)
            {
                Grid checkGrid = neighbours[i];
                // 7. if the neighbor is not walkable or is already in the closed list, continue.
                if (!checkGrid.walkable || closedSet.Contains(checkGrid))
                {
                    continue;
                }

                // 8. else if is not in open list, then add it into the open list.
                // 9. calculate the new cost of the neighbor
                bool isInOpenSet = openSet.Contains(checkGrid);
                int newCost = currentGrid.gCost + CalculateCost(currentGrid, checkGrid);
                if (newCost < checkGrid.gCost || !isInOpenSet)
                {
                    checkGrid.gCost = newCost;
                    checkGrid.hCost = CalculateCost(checkGrid, targetGrid);
                    //currentGrid.next = checkGrid;
                    checkGrid.prev = currentGrid;

                    if (!isInOpenSet) openSet.Add(checkGrid);
                }
            }
        }        
    }

    public void RetracePath(Grid startGrid_, Grid endGrid_, List<Grid> path)
    {
        path.Clear();
        Grid current = endGrid_;

        while(current != startGrid_)
        {
            path.Add(current);
            current = current.prev;
        }

        path.Reverse();
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
