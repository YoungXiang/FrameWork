using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A world is defined as a group of tiles or grids, and extra info on each grid.
/// </summary>
public class World : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 worldSize;
    public float gridRadius;

    Grid[,] grids;
    // radius * 2
    float gridDiameter;
    int gridNumX, gridNumY;

    #region Private Methods
    void Start()
    {
        gridDiameter = gridRadius * 2;
        gridNumX = Mathf.RoundToInt(worldSize.x / gridDiameter);
        gridNumY = Mathf.RoundToInt(worldSize.y / gridDiameter);
        CreateGrids();
    }

    void CreateGrids()
    {
        grids = new Grid[gridNumX, gridNumY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * worldSize.x / 2 - Vector3.forward * worldSize.y / 2;

        for (int x = 0; x < gridNumX; x++)
        {
            for (int y = 0; y < gridNumY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * gridDiameter + gridRadius) + Vector3.forward * (y * gridDiameter + gridRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, gridRadius, unwalkableMask));
                grids[x, y] = new Grid(walkable, worldPoint);
                grids[x, y].xCoord = x;
                grids[x, y].yCoord = y;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(worldSize.x, 1, worldSize.y));

        if (grids != null)
        {
            foreach (Grid grid in grids)
            {
                Gizmos.color = grid.walkable ? Color.green : Color.red;
                Gizmos.DrawCube(grid.worldPosition, Vector3.one * (gridDiameter - 0.1f));
            }
        }
    }
    #endregion

    public Grid GridFromWorldPosition(Vector3 worldPosition_)
    {
        float percentX = Mathf.Clamp01((worldPosition_.x + worldSize.x * 0.5f) / worldSize.x);
        float percentY = Mathf.Clamp01((worldPosition_.y + worldSize.y * 0.5f) / worldSize.y);

        int x = Mathf.RoundToInt((gridNumX - 1) * percentX);
        int y = Mathf.RoundToInt((gridNumY - 1) * percentY);
        return grids[x, y];
    }

    public void GetNeighbours(Grid grid_, List<Grid> outNeightbours_)
    {
        outNeightbours_.Clear();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = grid_.xCoord + x;
                int checkY = grid_.yCoord + y;

                if (checkX >= 0 && checkX < gridNumX &&
                    checkY >= 0 && checkY < gridNumY)
                {
                    outNeightbours_.Add(grids[checkX, checkY]);
                }
                
            }
        }
    }
}
