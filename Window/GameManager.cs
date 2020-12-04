using System.Collections.Generic;
using Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Game.Window
{
	public class GameManager : MonoBehaviour
	{
		public List<GameObject> mainWindowList = new List<GameObject>();
		public List<Transform> marketButtonList = new List<Transform>();

		public TextMeshProUGUI turnDate;
		public GameObject mainCanvas;
		public MainCamera mainCamera;
		
		[Header("Team Panel")]
		public GameObject basePanel;
		public GameObject characterPanel;
		public GameObject stockPanel;

		public MarketManager marketManager;
		public UnityEvent gameOver;
		public UnityEvent loseHp;
		public DataManager dataManager;
		public InventoryManager inventoryManager;

		private void Awake()
		{
			if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
			// if (inventoryManager == null) inventoryManager = FindObjectOfType<InventoryManager>();
		}

		private void Start()
		{
			mainCamera.Hungry(false);
			dataManager.LevelCheck();
		}

		public void TeamPanelActive(bool button)
		{
			basePanel.SetActive(button);
			characterPanel.SetActive(button);
			stockPanel.SetActive(button);
		}

		public void GoToMainMenu()
		{
			SceneManager.LoadScene("Game/Menu");
		}
	}
}