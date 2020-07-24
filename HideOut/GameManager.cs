using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace Game.HideOut
{
	public class GameManager : MonoBehaviour
	{
		public List<GameObject> mainWindowList = new List<GameObject>();
		public List<Transform> marketButtonList = new List<Transform>();

		public TextMeshPro turnDate;
		
		[Header("Team Panel")]
		public GameObject leftPanel;
		public GameObject middlePanel;
		public GameObject rightPanel;

		public MarketManager marketManager;

		public void TeamPanelActive(bool button)
		{
			leftPanel.SetActive(button);
			middlePanel.SetActive(button);
			rightPanel.SetActive(button);
		}
		
		
	}
}