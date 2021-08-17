using UnityEngine;

public class Node : IHeapItem<Node>
{
	[HideInInspector] public bool walkable;
	[HideInInspector] public Vector3 worldPosition;
    [HideInInspector] public Bounds bounds;

	[HideInInspector] public int gridX, gridY;
	[HideInInspector] public int gCost, hCost;

	[HideInInspector] public Node parent;
    int heapIndex;

    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY )
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }

	public int fCost 
	{
		get
		{
			return gCost + hCost;
		}
	}

	public int HeapIndex 
	{
		get 
		{
			return heapIndex;
		}
		set 
		{
			heapIndex = value;
		}
	}

	public int CompareTo ( Node _nodeToCompare )
	{
		int compare = fCost.CompareTo(_nodeToCompare.fCost);

		if ( compare == 0 )
		{
			compare = hCost.CompareTo(_nodeToCompare.hCost);
		}

		return -compare;
	}
}
