using UnityEngine;
using System.Collections.Generic;

namespace Game.HideOut
{
	public class GameManager : MonoBehaviour
	{
		public List<GameObject> mainWindowList = new List<GameObject>();
		public List<Transform> marketButtonList = new List<Transform>();

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

		private string GoldToText(int money)
		{
			if (money > 999999)
			{
				return "999999";
			}
			else
			{
				return money.ToString();
			}
		}
	}
}