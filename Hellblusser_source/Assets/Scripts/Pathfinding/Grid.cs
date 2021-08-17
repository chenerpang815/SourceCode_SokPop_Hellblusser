using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour 
{
    public static Grid instance;

    [HideInInspector] public Transform myTransform;

    Vector3 worldBottomLeft;
	[HideInInspector] public Vector2 gridWorldSize;
	public float nodeRadius;
	[HideInInspector] public Node[,] grid;

    [HideInInspector] public Bounds gridBounds;
    [HideInInspector] public Vector3 gridBoundsSize;

    [HideInInspector] public float nodeDiameter;
    [HideInInspector] public int gridSizeX, gridSizeY;
    [HideInInspector] public float gridOffX, gridOffY;

	public bool displayGridGizmos;

	[HideInInspector] public LayerMask gridLayerMask;

    //[Header("platform attached to")]
    //public LevelGenerator.Scripts.LevelGenerator platformAttachedTo;

    // layerMasks
    [Header("layerMasks")]
    public LayerMask walkableLayerMask;
    public LayerMask unwalkableMask;

	public int MaxSize 
	{
		get
		{
			return gridSizeX * gridSizeY;
		}
	}

    Bounds GetRenderBounds(GameObject objeto)
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        Renderer render = objeto.GetComponent<Renderer>();
        if (render != null)
        {
            return render.bounds;
        }
        return bounds;
    }

    Bounds GetBounds(GameObject o)
    {
        Bounds bounds;
        Renderer childRender;
        bounds = GetRenderBounds(o);
        if (bounds.extents.x == 0f)
        {
            bounds = new Bounds(o.transform.position, Vector3.zero);
            foreach (Transform child in o.transform)
            {
                childRender = child.GetComponent<Renderer>();
                if (childRender)
                {
                    bounds.Encapsulate(childRender.bounds);
                }
                else
                {
                    bounds.Encapsulate(GetBounds(child.gameObject));
                }
            }
        }
        return bounds;
    }

    void Awake ()
    {
        instance = this;
    }

    /*
    void Start()
    {
        Init();
        //Invoke("Init",.5f);
	}
    */

    public void Init ()
    {
        // get base components
        myTransform = this.transform;

        // get grid bounds
        gridBounds = GetBounds(LevelGeneratorManager.instance.activeLevelGenerator.boundsContainerGameObject);
        gridBoundsSize = gridBounds.size;
        gridBoundsSize.x += 4f;

        // define grid dimensions
        nodeDiameter = nodeRadius * 2f;
        gridWorldSize.x = gridBounds.size.x;
        gridWorldSize.y = gridBounds.size.z;
        gridSizeX = Mathf.CeilToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.CeilToInt(gridWorldSize.y / nodeDiameter);

        // create grid
        CreateGrid();
    }

    public int ClampCoords ( int _cur, int _min, int _max)
    {
        int ret = _cur;

        if ( ret < _min )
        {
            ret = _min;
        }
        if ( ret > _max )
        {
            ret = _max;
        }

        return ret;
    }

    void CreateGrid ()
	{
        grid = new Node[gridSizeX, gridSizeY];

        Vector3 gridBoundsCenterGet = gridBounds.center;

        worldBottomLeft = gridBoundsCenterGet;
        worldBottomLeft.y = 0f;
        worldBottomLeft -= (Vector3.right * Mathf.Floor(gridWorldSize.x * .5f));
        worldBottomLeft -= (Vector3.forward * Mathf.Floor(gridWorldSize.y * .5f));

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + (Vector3.right * ((x * nodeDiameter) + nodeRadius)) + (Vector3.forward * ((y * nodeDiameter) + nodeRadius));

                bool walkable;

                float cDst = 4f;
                Vector3 c0 = worldPoint;
                c0 += Vector3.up * cDst;
                Vector3 c1 = worldPoint;
                c1 += Vector3.up * -cDst;
                RaycastHit cHit;
                if (Physics.Linecast(c0, c1, out cHit, walkableLayerMask))
                {
                    walkable = true;
                    worldPoint.y = cHit.point.y;
                }
                else
                {
                    walkable = false;
                }

                // not walkable if we are hitting an unwalkable collider
                if (Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask))
                {
                    walkable = false;
                }

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }

        /*
		grid = new Node[gridSizeX,gridSizeY];

        worldBottomLeft = Vector3.zero;
        worldBottomLeft.y = .5f;
        worldBottomLeft += (Vector3.right * gridOffX);
        worldBottomLeft += (Vector3.forward * gridOffY);

		for ( int x = 0; x < gridSizeX; x ++ )
		{
			for ( int y = 0; y < gridSizeY; y ++ )
			{
				Vector3 worldPoint = worldBottomLeft + (Vector3.right * ((x * nodeDiameter) + nodeRadius)) + (Vector3.forward * ((y * nodeDiameter) + nodeRadius));

                bool walkable = true;

                float cDst = 4f;
                Vector3 c0 = worldPoint;
                c0 += Vector3.up * cDst;
                Vector3 c1 = worldPoint;
                c1 += Vector3.up * -cDst;
                RaycastHit cHit;
                if ( Physics.Linecast(c0,c1,out cHit,walkableLayerMask) )
                {
                    walkable = true;
                    worldPoint.y = cHit.point.y;
                }
                else
                {
                    walkable = false;
                }

                // not walkable if we are hitting an unwalkable collider
                if (Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask) )
                {
                    walkable = false;
                }

                Node nNode = new Node(walkable, worldPoint, x, y);
                grid[x, y] = nNode;
			}
		}
        */
    }

	public List<Node> GetNeighbours ( Node _node )
	{
		List<Node> neighbours = new List<Node>();

		for ( int x = -1; x <= 1; x ++ )
		{
			for ( int y = -1; y <= 1; y ++ )
			{
				if ( x == 0 && y == 0)
					continue;

				int checkX = _node.gridX + x;
				int checkY = _node.gridY + y;

				if ( (checkX >= 0 && checkX < gridSizeX) && (checkY >= 0 && checkY < gridSizeY) )
				{
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}

    public Node PickRandomNeighbour ( Node _node )
    {
        List<Node> neighbours = new List<Node>();
        Node rNeighbour = null;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = _node.gridX + x;
                int checkY = _node.gridY + y;

                if ((checkX >= 0 && checkX < gridSizeX) && (checkY >= 0 && checkY < gridSizeY))
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        if (neighbours != null && neighbours.Count > 0)
        {
            int rIndex = Mathf.RoundToInt(TommieRandom.instance.RandomRange(0f, neighbours.Count - 1));
            Node rNode = neighbours[rIndex];
            if (rNode != _node)
            {
                rNeighbour = neighbours[rIndex];
            }
        }

        return rNeighbour;
    }

    public Node NodeFromWorldPoint ( Vector3 _worldPosition )
	{
        float percentX = (-worldBottomLeft.x + _worldPosition.x) / gridWorldSize.x;
        float percentY = (-worldBottomLeft.z + _worldPosition.z) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

		return grid[x,y];
	}

    public Node NodeFromScreenPoint (Vector3 _screenPosition, int _curX, int _curZ )
    {
        int nodeX = _curX;
        int nodeZ = _curZ;
        for ( int x = 0; x < gridSizeX; x ++ )
        {
            for ( int z = 0; z < gridSizeY; z ++ )
            {
                Vector3 p0 = ScreenPointFromNode(x, z);
                Vector3 p1 = _screenPosition;
                p0.z = 10f;
                p1.z = 10f;
                float dstToNode = Vector3.Distance(p0,p1);
                if ( dstToNode <= 25f )
                {
                    nodeX = x;
                    nodeZ = z;
                }
            }
        }

        return grid[nodeX, nodeZ];
    }

    public Vector3 WorldPointFromNode ( int _x, int _z )
    {
        return grid[_x,_z].worldPosition;
    }

    public Vector3 ScreenPointFromNode(int _x, int _z)
    {
        Vector3 worldPoint = WorldPointFromNode(_x,_z);

        Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPoint);

        return screenPoint;
    }

    public Node GetNearestNode ( Vector3 _checkPos )
	{
		Node nearestNode = null;

		float minDist = Mathf.Infinity;
		foreach ( Node n in grid )
		{
			float dist = Vector3.Distance(n.worldPosition,_checkPos);
			if ( dist < minDist )
			{
				nearestNode = n;
				minDist = dist;
			}
		}

		return nearestNode;
	}

    void OnDrawGizmos ()
	{
        // draw nodes
		if ( grid != null && displayGridGizmos )
		{
			foreach ( Node n in grid )
            {
                Gizmos.color = ( n.walkable ) ? Color.black : Color.red;

                if ( n.gridX == 0 && n.gridY == 0 )
                {
                    Gizmos.color = Color.cyan; 
                }
                if ( n.gridX == gridSizeX - 1 && n.gridY == gridSizeY - 1 )
                {
                    Gizmos.color = Color.green;
                }

                Gizmos.DrawCube(n.worldPosition,Vector3.one * (nodeDiameter * .1f));
            }
		}
	}
}
