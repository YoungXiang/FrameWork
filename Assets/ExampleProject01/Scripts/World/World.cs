using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A world is defined as a group of tiles or grids, and extra info on each grid.
/// </summary>
public class World : MonoBehaviour
{
    public LayerMask groundMask;
    public LayerMask unwalkableMask;
    public Vector2 worldSize;
    public float gridRadius;

#if GROUND_HEIGHT
    public float highest = 100.0f;
    public float lowest = -100.0f;
#endif

    protected AStarPathFinding pathFinding;

    Grid[,] grids;
    // radius * 2
    float gridDiameter;
    int gridNumX, gridNumY;
    public int MaxLength { get { return gridNumX * gridNumY; } }

    Vector3 worldBottomLeft;
    
    #region Private Methods
    void Start()
    {
        gridDiameter = gridRadius * 2;
        gridNumX = Mathf.RoundToInt(worldSize.x / gridDiameter);
        gridNumY = Mathf.RoundToInt(worldSize.y / gridDiameter);
        CreateGrids();
        pathFinding = new AStarPathFinding(this);
    }

    void CreateGrids()
    {
        grids = new Grid[gridNumX, gridNumY];

        worldBottomLeft = transform.position - Vector3.right * worldSize.x / 2 - Vector3.forward * worldSize.y / 2;

        for (int x = 0; x < gridNumX; x++)
        {
            for (int y = 0; y < gridNumY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * gridDiameter + gridRadius) + Vector3.forward * (y * gridDiameter + gridRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, gridRadius, unwalkableMask));
                grids[x, y] = new Grid(walkable, worldPoint);
                grids[x, y].xCoord = x;
                grids[x, y].yCoord = y;

#if GROUND_HEIGHT
                // ground height                
                worldPoint.y = highest;
                RaycastHit hitInfo;
                if (Physics.Raycast(worldPoint, Vector3.down, out hitInfo, highest - lowest + 1.0f, groundMask))
                {
                    grids[x, y].groundHeight = hitInfo.point.y;
                }
#endif
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
                Gizmos.DrawCube(grid.worldPosition, Vector3.one * (gridDiameter - 1f));
            }
        }
    }
#endregion

    public Grid GridFromWorldPosition(Vector3 worldPosition_)
    {
        int percentX = Mathf.Clamp(Mathf.RoundToInt((worldPosition_.x - worldBottomLeft.x - gridRadius) / gridDiameter), 0, gridNumX - 1);
        int percentY = Mathf.Clamp(Mathf.RoundToInt((worldPosition_.z - worldBottomLeft.z - gridRadius) / gridDiameter), 0, gridNumY - 1);
        
        return grids[percentX, percentY];
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

    public void FindPath(Vector3 startPos_, Vector3 targetPos_, List<Grid> outPath_)
    {
        pathFinding.FindPath(startPos_, targetPos_, outPath_);
    }
}
