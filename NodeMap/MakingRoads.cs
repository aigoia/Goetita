using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeMap
{
	public class MakingRoads : MonoBehaviour
	{
		[SerializeField] private GameObject roadObject;
		[SerializeField] private Transform roadManager;

		[ContextMenu("Set Road on the Nodes")]
		private void SetRoadOnNodes()
		{
			if ((roadObject == null) || (roadManager == null)) return;
			
			for (int i = 0; i < transform.childCount; i++)
			{
				var road = transform.GetChild(i);
				var position = road.position;
				Instantiate(roadObject, position ,Quaternion.identity, roadManager);
				Instantiate(roadObject, position ,Quaternion.Euler(0, 90, 0), roadManager);
			}
		}

		[ContextMenu("Collect All Rodes")]
		private void CollectAllRoads()
		{
			var nodeList = transform.GetComponent<MakingNodes>().nodeList;
			
			foreach (var node in nodeList)
			{
				node.CollectConnectedRoad();
			}
		}
		
		[ContextMenu("Collect All Connected Nodes")]
		private void CollectAllConnectedNodes()
		{
			var nodeList = transform.GetComponent<MakingNodes>().nodeList;
			
			foreach (var node in nodeList)
			{
				node.CollectConnectedNode();
			}
		}
	}
}
