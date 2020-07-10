using UnityEngine;

namespace NodeMap
{
	public class RoadManager : MonoBehaviour {

		[ContextMenu("Renaming Rodes")]
		public void CollectNodes()
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				var newNode = transform.GetChild(i);
				newNode.name = "Road " + "("+ i + ")";
			}
		}
	}
}
