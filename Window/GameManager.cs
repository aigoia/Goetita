using System;
using System.Collections.Generic;
using Game.Data;
using Game.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
		public UnityEvent init;
		public Color fullColor;
		public GameObject block;
		public UnityEvent start;
		
		public AudioSource click;
		public AudioSource hover;
		public AudioSource move;
		
		public AudioManager audioManager;
		public CharacterManager characterManager;
		
		private void Awake()
		{
			if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
			if (audioManager == null) audioManager = FindObjectOfType<AudioManager>();
		}

		private void Start()
		{
			mainCamera.Hungry(false);
			dataManager.LevelCheck();
			init.Invoke();
			start.Invoke();
			
			if (audioManager != null) audioManager.FadeInCaller();
			// Invoke(nameof(PlaySound), 1f);
		}

		void PlaySound()
		{
			if (audioManager != null) audioManager.PlaySound();
			if (audioManager != null) audioManager.VolumeUp();
		}

		public void TeamPanelActive(bool button)
		{
			basePanel.SetActive(button);
			characterPanel.SetActive(button);
			stockPanel.SetActive(button);
		}

		public void GoToMainMenu()
		{
			if (audioManager != null)
			{
				audioManager.FadeOutCaller(); 
			}

			// if (characterManager.WhereIam().assignedAccident.mission == null) return;
			
			SceneManager.LoadScene("Game/Menu");
		}
		
		public void Insert(ItemButtonManager itemButton, Item item)
		{
			itemButton.itemId = item.itemId;
			itemButton.itemNameText.text = item.itemName;
			itemButton.priceText.text = item.itemPrice.ToString();
			itemButton.priceInt = item.itemPrice;
			itemButton.itemType = item.itemType;
			itemButton.detailType = item.detailType;
			itemButton.itemConsumable = item.itemConsumable;
			itemButton.icon.GetComponent<Image>().sprite = dataManager.FindItemImage(item.itemName).sprite;
		}
		
		// ButtonManager has ResetAllSlot
		public void ReItem(ItemButtonManager itemButton)
		{
			itemButton.itemId = 0;
			itemButton.itemNameText.text = "Default";
			itemButton.priceInt = 0;
			itemButton.priceText.text = "0";
			itemButton.itemType = ItemType.Non;
			itemButton.detailType = CharacterClass.Non;
			itemButton.itemConsumable = ItemConsumable.Non;
			itemButton.icon.GetComponent<Image>().sprite = dataManager.FindItemImage("Base").sprite;
		}
	}
}