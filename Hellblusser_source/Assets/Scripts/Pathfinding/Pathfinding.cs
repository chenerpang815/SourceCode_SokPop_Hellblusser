using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour 
{
    // requestManager
    [Header("requestManager")]
	public PathRequestManager requestManager;

	public void StartFindPath ( Vector3 _startPos, Vector3 _targetPos, Grid _grid )
	{
        StartCoroutine(FindPath(_startPos,_targetPos,_grid));
        //FindPath(_startPos,_targetPos,_grid);
	}

    Vector3[] waypointsFind = new Vector3[0];
	IEnumerator FindPath ( Vector3 _startPos, Vector3 _targetPos, Grid _grid )
	{
        //Vector3[] waypoints = new Vector3[0];
        Array.Clear(waypointsFind,0,0);
		bool pathSuccess = false;

		Node startNode = _grid.NodeFromWorldPoint(_startPos);
		Node targetNode = _grid.NodeFromWorldPoint(_targetPos);

		if ( /*startNode.walkable &&*/ targetNode.walkable )
		{
			Heap<Node> openSet = new Heap<Node>(_grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();

			openSet.Add(startNode);
			while ( openSet.Count > 0 )
			{
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);

				if ( currentNode == targetNode )
				{
					pathSuccess = true;
					break;
				}

				foreach ( Node neighbour in _grid.GetNeighbours(currentNode) )
				{
					if ( !neighbour.walkable || closedSet.Contains(neighbour) )
					{
						continue;
					}

					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode,neighbour);
					if ( (newMovementCostToNeighbour < neighbour.gCost) || !openSet.Contains(neighbour) )
					{
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour,targetNode);
						neighbour.parent = currentNode;

						if ( !openSet.Contains(neighbour) )
						{
							openSet.Add(neighbour);
						}
					}
					else
					{
						openSet.UpdateItem(neighbour);
					}
				}
			}
		}

		yield return null;

		if ( pathSuccess )
		{
            waypointsFind = RetracePath(startNode,targetNode);
		}
		requestManager.FinishedProcessingPath(waypointsFind, _grid,pathSuccess);
	}

    List<Node> path = new List<Node>();
    //Vector3[] waypointsArray = new Vector3[0];
    Vector3[] RetracePath ( Node _startNode, Node _endNode )
	{
        //List<Node> path = new List<Node>();
        path.Clear();
		Node currentNode = _endNode;

		while ( currentNode != _startNode )
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}

        Vector3[] waypointsArray = SimplifyPath(path);
		Array.Reverse(waypointsArray);

		return waypointsArray;
	}

    List<Vector3> waypoints = new List<Vector3>();
    Vector2 directionNew = Vector2.zero;
    Vector3[] SimplifyPath ( List<Node> _path )
	{
        waypoints.Clear();
		//List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;

		bool simplify = false;
		if ( simplify )
		{
			for ( int i = 1; i < _path.Count; i ++ )
			{
                //Vector2 directionNew = new Vector2(_path[i - 1].gridX - _path[i].gridX,_path[i - 1].gridY - _path[i].gridY);
                directionNew.x = _path[i - 1].gridX - _path[i].gridX;
                directionNew.y = _path[i - 1].gridY - _path[i].gridY;

                if ( directionNew != directionOld )
				{
					waypoints.Add(_path[i].worldPosition);
				}
				directionOld = directionNew;
			}
		}
		else
		{
			for ( int i = 0; i < _path.Count; i ++ )
			{
				waypoints.Add(_path[i].worldPosition);
			}
		}

		return waypoints.ToArray();
	}

	int GetDistance ( Node _nodeA, Node _nodeB )
	{
		int distX = Mathf.Abs(_nodeA.gridX - _nodeB.gridX);
		int distY = Mathf.Abs(_nodeA.gridY - _nodeB.gridY);

		if ( distX > distY )
		{
			return 14 * distY + 10 * (distX - distY);
		}
		else
		{
			return 14 * distX + 10 * (distY - distX);
		}
	}
}
