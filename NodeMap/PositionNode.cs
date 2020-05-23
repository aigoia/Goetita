using System.Collections.Generic;
using UnityEngine;

namespace NodeMap
{
	public enum PositionStyle
	{
		Normal, Battle, Heal, Begin, End,
	}

	public class PositionNode : MonoBehaviour {

		public PositionStyle thisPositionStyle = PositionStyle.Normal;
		public List<Road> connectedRoadList;
		public List<PositionNode> connectedNodeList;
		
		public void CollectConnectedRoad()
		{
			var thisTransform = transform;
			var forward = thisTransform.forward;
			connectedRoadList = new List<Road>();
			
			var hitList = Physics.BoxCastAll(thisTransform.position, Vector3.one, forward, Quaternion.identity, 0, LayerMask.GetMask ("Road"));
			foreach (var hit in hitList)
			{
				var newHit = hit.transform.GetComponent<Road>();
				if (newHit == null) continue;
				connectedRoadList.Add(newHit);
			}
		}

		[ContextMenu("Collect Connected Nodes")]
		public void CollectConnectedNode()
		{
			if (connectedRoadList == null) return;
			
			connectedNodeList = new List<PositionNode>();

			var nodeList = transform.parent.GetComponent<MakingNodes>().nodeList;

			foreach (var newRoad in connectedRoadList)
			{
				foreach (var node in nodeList)
				{
					foreach (var oldRoad in node.connectedRoadList)
					{
						if (newRoad == oldRoad)
						{
							if (node == transform.GetComponent<PositionNode>()) continue;
							
							connectedNodeList.Add(node);
						}
						
					}
				}
			}
		}
	}
}