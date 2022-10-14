using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.MainGame
{
	public enum PathFindingStyle
	{
		In, Out,
	}

	public class PathFinding : MonoBehaviour {

		// debug
		[SerializeField] GameObject openObject;
		[SerializeField] GameObject closedObject;
		[SerializeField] GameObject wayObject;

		public bool debugMod = false;

		Transform _debugPath;
		int pathFindingCount = 32;

		void Awake()
		{
			if (_debugPath == null) _debugPath = transform.Find("Path");
		}

		void DebugPath(GameObject objectType, Vector2 pos) 
		{
			Instantiate(objectType, new Vector3(pos.x, 0, pos.y) * 2, Quaternion.identity, _debugPath);
		}

		readonly Vector2[] _nears = 
		{
			new Vector2(1, 0), new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, -1),
			new Vector2(-1, 1), new Vector2(0, 1), new Vector2(1, 1), new Vector2(-1, 0),
		};

		List<TileNode> GetNears (TileNode currentNode, TileNode endNode,List<TileNode> map, PathFindingStyle pathFindingStyle) 
		{
			var neighbours = new List<TileNode>();

			foreach (var direction in _nears) {

				Vector2 foundNear = currentNode.Coordinate + direction;

				if (endNode.Coordinate == foundNear)
				{
					neighbours.Clear();
					neighbours.Add(endNode);
				}

				foreach (var tile in map)
				{
					if (pathFindingStyle == PathFindingStyle.In)
					{
						if (tile.Coordinate == foundNear && (tile.tileStyle == TileStyle.OneArea || tile.tileStyle == TileStyle.TwoArea))
						{
							neighbours.Add(tile);
						}
					}
					else if (pathFindingStyle == PathFindingStyle.Out)
					{
						if (tile.Coordinate == foundNear && (tile.tileStyle == TileStyle.OneArea || tile.tileStyle == TileStyle.TwoArea || tile.tileStyle == TileStyle.Normal))
						{
							neighbours.Add(tile);
						}
					}
				}
			}
			return neighbours;
		}

		TileNode GetMin(TileNode startNode, TileNode endNode, List<TileNode> openList) 
		{
			foreach (var viaNode in openList) 
			{
				viaNode.farFormTarget = Vector2.Distance(viaNode.Coordinate, endNode.Coordinate); 
			}

			var valueList = new List<float>();
			foreach (var viaNode in openList)
			{
				valueList.Add(viaNode.farFormTarget);
			}
			return openList.Find(i => i.farFormTarget == valueList.Min());
		}

		List<TileNode> Retrace(TileNode startNode, TileNode endNode, List<TileNode> closedMap, PathFindingStyle pathFindingStyle) 
		{
			var openList = new List<TileNode>();
			var wayList = new List<TileNode>();

			openList.Add(startNode);
			var limit = pathFindingCount;
		
			while (limit > 0 && openList.Count > 0) 
			{	
				var current = GetMin (startNode, endNode, openList);

				openList.Remove(current);
				wayList.Add(current);

				foreach (var node in GetNears(current, endNode, closedMap, pathFindingStyle))
				{
					if (node == endNode)
					{
						return wayList;
					}
					if (openList.Contains(node) || wayList.Contains(node)) continue;
					openList.Add(node);
				}
				limit--;
			}
			
			return wayList;
		}

		public List<TileNode> GreedPathFinding(TileNode startNode, TileNode endNode, List<TileNode> map, PathFindingStyle pathFindingStyle = PathFindingStyle.In) 
		{
			// GreedPathFinding is needed to consider level design risk

			var openList = new List<TileNode>();
			var closedList = new List<TileNode>();
			var wayList = new List<TileNode>();

			openList.Add(startNode);
			var limit = pathFindingCount;

			while (limit > 0 && openList.Count > 0) 
			{
				var current = GetMin(startNode, endNode, openList);
				openList.Remove(current);
				closedList.Add(current);

				foreach (var node in GetNears(current, endNode, map, pathFindingStyle))
				{
					if (node == endNode)
					{
						wayList.AddRange(Retrace(endNode, startNode, closedList, pathFindingStyle));

						if (pathFindingStyle == PathFindingStyle.In)
						{
							return wayList;
						}
						else if (pathFindingStyle == PathFindingStyle.Out)
						{
							var newWayList = new List<TileNode>();
		
							foreach (var tileNode in wayList)
							{
								if (tileNode.tileStyle == TileStyle.OneArea || tileNode.tileStyle == TileStyle.TwoArea)
								{
									newWayList.Add(tileNode);
								}
							}
							return newWayList;
						}
					}

					if (openList.Contains (node) || closedList.Contains (node)) continue;
					openList.Add(node);
				}

				if (debugMod == true)
				{
					openList.ForEach(i => DebugPath(openObject, i.Coordinate));
					closedList.ForEach(i => DebugPath(closedObject, i.Coordinate));
				}
				limit--;
			}

			print(gameObject.name + "There is no way");
			return null;
		}
	}
}