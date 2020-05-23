using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeMap
{
	public class MakingNodes : MonoBehaviour {
		[SerializeField] GameObject cube;
		[SerializeField] private int nodeNumber = 50;
		[SerializeField] private int halfSize = 20;
		
		[SerializeField] private GameObject battlePosition;
		[SerializeField] private GameObject normalPosition;
		[SerializeField] private GameObject beginPosition;
		[SerializeField] private GameObject endPosition;
		[SerializeField] private GameObject healPosition;
		
		public List<PositionNode> nodeList;

		[ContextMenu("Make Random Nodes")]
		public void MakeRandomNodes()
		{
			if (cube == null) return;
			
			var randomVector3List = new List<Vector3>();
			
			for (int i = 0; i < nodeNumber; i++)
			{
				var pos = new Vector3(Random.Range(-halfSize, halfSize), 0f, Random.Range(-halfSize, halfSize));
				Instantiate(cube, pos, Quaternion.identity);
			}
		}

		[ContextMenu("Collect Nodes")]
		public void CollectNodes()
		{
			nodeList = new List<PositionNode>();
			
			for (int i = 0; i < transform.childCount; i++)
			{
				var newNode = transform.GetChild(i).GetComponent<PositionNode>();
				newNode.name = "PositionNode " + "("+ i + ")";
				nodeList.Add(newNode);
			}
		}

		[ContextMenu("Set Node Style")]
		public void SetNodeStyle()
		{
			foreach (var node in nodeList)
			{
				if (node.thisPositionStyle == PositionStyle.Battle)
				{
					Instantiate(battlePosition, node.transform);
				}
				else if (node.thisPositionStyle == PositionStyle.Begin)
				{
					Instantiate(beginPosition, node.transform);
				}
				else if (node.thisPositionStyle == PositionStyle.End)
				{
					Instantiate(endPosition, node.transform);
				}
				else if (node.thisPositionStyle == PositionStyle.Heal)
				{
					Instantiate(healPosition, node.transform);
				}
				else if (node.thisPositionStyle == PositionStyle.Normal)
				{
					Instantiate(normalPosition, node.transform);
				}
			}
		}

		[ContextMenu(("Delete Node Style"))]
		public void DeleteNodeStyle() {
			foreach (var node in nodeList)
			{
				DestroyImmediate(node.transform.GetChild(0).gameObject);
			}
			
		}
	}
}

