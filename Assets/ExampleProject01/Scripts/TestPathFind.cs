using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPathFind : MonoBehaviour
{
    public World world;
    public Transform target;
    
    protected List<Grid> path = new List<Grid>();
    	
	// Update is called once per frame
	void Update ()
    {
		if (target.hasChanged)
        {
            world.FindPath(transform.position, target.position, path);
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        if (path != null)
        {
            Gizmos.color = Color.black;
            for (int i = 0; i < path.Count; i++)
            {
                Gizmos.DrawCube(path[i].worldPosition, Vector3.one * (world.gridRadius * 2 - 0.1f));
            }
        }
        
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(world.GridFromWorldPosition(transform.position).worldPosition, Vector3.one * 10.0f);
        Gizmos.DrawCube(world.GridFromWorldPosition(target.position).worldPosition, Vector3.one * 10.0f);

        // show grid
        /*
        Grid head = findedGrid;
        Gizmos.color = Color.black;
        while (head != null)
        {
            Gizmos.DrawCube(head.worldPosition, Vector3.one * (world.gridRadius * 2 - 0.1f));
            head = head.prev;
        }*/
    }
}
