using UnityEngine;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour 
{
    static PathRequestManager instance;

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	PathRequest currentPathRequest;

    // pathfinding
    [Header("pathfinding")]
    public Pathfinding pathfinding;
	
    private bool isProcessingPath;

	void Awake ()
	{
		instance = this;
	}

	public static void RequestPath ( Vector3 _pathStart, Vector3 _pathEnd, Grid _grid, Action<Vector3[],Grid,bool> _callback )
	{
		PathRequest newRequest = new PathRequest(_pathStart,_pathEnd,_grid,_callback);
		instance.pathRequestQueue.Enqueue(newRequest);
		instance.TryProcessNext(_grid);
	}

	void TryProcessNext ( Grid _grid )
	{
		if ( !isProcessingPath && (pathRequestQueue.Count > 0) )
		{
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true;
			pathfinding.StartFindPath(currentPathRequest.pathStart,currentPathRequest.pathEnd,_grid);
		}
	}

	public void FinishedProcessingPath ( Vector3[] _path, Grid _grid, bool _success )
	{
		currentPathRequest.callback(_path,_grid,_success);
		isProcessingPath = false;
		TryProcessNext(_grid);
	}

	struct PathRequest 
	{
		public Vector3 pathStart;
		public Vector3 pathEnd;
        public Grid grid;
		public Action<Vector3[],Grid,bool> callback;

		public PathRequest ( Vector3 _start, Vector3 _end, Grid _grid, Action<Vector3[],Grid,bool> _callback )
		{
			pathStart = _start;
			pathEnd = _end;
            grid = _grid;
			callback = _callback;
		}
	}
}
