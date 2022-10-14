using System.Collections.Generic;
using Game.Data;
using Game.MainGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Window
{
	public class InventoryManager : MonoBehaviour
	{
		public MarketManager marketManager;
		public List<Transform> inventoryList;
		public List<Item> equipItem = new List<Item>();
		public List<TextMeshProUGUI> traitTextList = new List<TextMeshProUGUI>();
		public List<Transform> slotList;
		public Transform itemStock;
		public int itemFull = 24;
		public Image bicProfile;
		public DataManager dataManager;
		public ButtonManager buttonManager;
		public CharacterSelect characterSelect;
		public Data.Character selectedCharacter;
		public int limitSlot = 2;
		public Button defaultCharacter;
		public Button defaultButton;
		// public Button statsButton;
	
		public TextMeshProUGUI characterName;
		public TextMeshProUGUI characterClass;
		public TextMeshProUGUI level;
		public Scrollbar expBar;

		public TextMeshProUGUI hp;
		public TextMeshProUGUI damage;
		public TextMeshProUGUI armor;
		public List<TextMeshProUGUI> skillList;
		public List<TextMeshProUGUI> traitList;

		public List<Character> newCharacters;
		public Image profileImage;

		public DeckManager deckManager;
		public TraitManager traitManager;

		private void Awake()
		{
			if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
			if (buttonManager == null) buttonManager = FindObjectOfType<ButtonManager>();
			if (dataManager == null) deckManager = FindObjectOfType<DeckManager>();
		}

		private void Start()
		{
			characterSelect.ReSetHp();
			ShowProfileImage();
		}

		public void ChangeProfileImage() 
		{
			var profileZoomImage = dataManager.profileImageList.Find(image => image.name == selectedCharacter.characterName).transform
				.Find("Zoom").GetComponent<Image>().sprite;
			
			profileImage.sprite = profileZoomImage;
		}
		
		public void DefaultButtonOnClick()
		{
			defaultButton.onClick.Invoke();
		}
		
		public void DefaultCharacterOnClick()
		{
			defaultCharacter.onClick.Invoke();
		}

		public void ReSlot()
		{
			foreach (var slot in slotList)
			{
				slot.gameObject.SetActive(false);
			}
		}
		
		public void TraitOn(string message)
		{
			traitTextList.Find(i => i.text.ToString() == message).gameObject.SetActive(true);
		}

		public void AllTraitOff()
		{
			foreach (var trait in traitTextList)
			{
				trait.gameObject.SetActive(false);
			}
		}
		
		public void ReShowAll()
		{ 
			SetInformation();
			InitLevelExp();
			ShowStats();
		}

		public void ShowStats()
		{
			// check item
			var baseHp = selectedCharacter.baseHp;
			var baseDeal = selectedCharacter.baseDeal;
			var plusDeal = selectedCharacter.plusDeal;
			var baseArmor = selectedCharacter.baseArmor;

			if (selectedCharacter.itemList.Exists(i => i.itemType == ItemType.Weapon))
			{
				
			}
			else
			{
				if (selectedCharacter.classType == CharacterClass.Claymore)
				{
					var baseSword = dataManager.baseItemList.Find(i => i.itemName == "Base Sword");
					
					if (baseSword != null)
					{
						baseDeal = baseDeal + baseSword.baseInt;
						plusDeal = plusDeal + baseSword.plusInt;
					}
				}
				else if (selectedCharacter.classType == CharacterClass.Ranger)
				{
					var baseGun = dataManager.baseItemList.Find(i => i.itemName == "Base Gun");
					
					if (baseGun != null)
					{
						baseDeal = baseDeal + baseGun.baseInt;
						plusDeal = plusDeal + baseGun.plusInt;
					}
				}
			}
			if (selectedCharacter.itemList.Exists(i => i.itemType == ItemType.Armor))
			{
				
			}
			else
			{
				if (selectedCharacter.classType == CharacterClass.Claymore)
				{
					var loadBaseArmor = dataManager.baseItemList.Find(i => i.itemName == "Base Armor");
					
					if (loadBaseArmor != null)
					{
						baseArmor = baseArmor + loadBaseArmor.baseInt;	
					}
				}
				if (selectedCharacter.classType == CharacterClass.Ranger)
				{
					var loadBaseArmor = dataManager.baseItemList.Find(i => i.itemName == "Base Armor");
					
					if (loadBaseArmor != null)
					{
						baseArmor = baseArmor + loadBaseArmor.baseInt;	
					}
				}
			}
			
			// AllTraitOff();
			traitManager.TraitOffAll();
			
			foreach (var item in selectedCharacter.itemList)
			{
				if (item.itemType == ItemType.Weapon)
				{
					baseDeal = baseDeal + item.baseInt;
					plusDeal = plusDeal + item.plusInt;

					if (item.trait == Data.Trait.ArmorPiercing)
					{
						traitManager.TraitOn("ArmorPiercing", 0);
					}
					
				}
				else if (item.itemType == ItemType.Armor)
				{
					baseArmor = baseArmor + item.baseInt;
					baseHp = baseHp + item.plusInt;
				}
			}
			
			if (selectedCharacter.classType == CharacterClass.Claymore)
			{
				traitManager.TraitOn("ShadowWalk", 1);
			}
			
			// print data
			level.text = selectedCharacter.level.ToString();
			hp.text = dataManager.windowTextManager.hpPlus + " " + baseHp;
			damage.text = dataManager.windowTextManager.baseDealPlus + " " + baseDeal + " + " + plusDeal;
			armor.text = dataManager.windowTextManager.armorPlus + " " + baseArmor;
		}

		public void InitLevelExp()
		{
			var levelStageCurrent = dataManager.levelStage[selectedCharacter.level];
			var levelStageNext = dataManager.neededLevel[selectedCharacter.level];
			float currentExp = selectedCharacter.exp - levelStageCurrent;
			expBar.size = currentExp / levelStageNext;
			level.text = (selectedCharacter.level).ToString();
		}

		void EnrollItems()
		{
			var newCharacterList = dataManager.CurrentCharacterList;
			foreach (var character in newCharacterList)
			{
				foreach (var item in character.itemList)
				{
					print("Enroll item : " + item.itemName);
					foreach (var slot in slotList)
					{
						if (slot.gameObject.activeSelf == false)
						{
							var newItem = new Item(dataManager.ownedItems.Find(i => i.itemId == item.itemId));
							equipItem.Add(newItem);
							
							slot.gameObject.SetActive(true);
							var slotButton = slot.GetComponent<ItemButtonManager>();
							Insert(slotButton, newItem);
							dataManager.SaveCharacter(newCharacterList);
							break;
						}
					}
				}
			}
		}

		[ContextMenu("MakeInventoryList")]
		public void MakeInventoryList()
		{
			inventoryList = new List<Transform>();
			for (int i = 0; i < itemStock.childCount; i++)
			{
				inventoryList.Add(itemStock.GetChild(i));
			}
		}

		public void DefaultOn()
		{
			if (defaultButton == null) return;
			
			defaultButton.animator.Play("Hover to Pressed");

			selectedCharacter = dataManager.CurrentCharacterList[0];
			SetInformation();
		}
		
		public void SetProfile()
		{
			// SetBicProfile(0);
			buttonManager.ResetAllSlot();
			characterSelect.ReSetting();

			// for (int i = 0; i < dataManager.currentCharacterList.Count; i++)
			// {
			// 	
			//
			// 	// var profileImage = dataManager.profileImageList[characterId - 1].transform.Find("100x100")
			// 	// 		.GetComponent<Image>().sprite;
			// }
			
			var index = 0;
			foreach (var character in dataManager.LoadCharacter())
			{
				print(character.characterName);
				characterSelect.buttonList[index].SetActive(true);
				var profileButtonImage = dataManager.profileImageList.Find(image => image.name == character.characterName).transform
					.Find("100x100").GetComponent<Image>().sprite;

				characterSelect.buttonList[index].transform.Find("ProfileImage").GetComponent<Image>().sprite =
					profileButtonImage;

				index += 1;
			}

			selectedCharacter = dataManager.CurrentCharacterList[0];
			characterSelect.activeList[0].SetActive(true);
			ChangeSlot(selectedCharacter.characterName);
		}

		public void ShowProfileImage()
		{
			string spriteName = selectedCharacter.characterName + "Zoom";
			if (profileImage.sprite.name == spriteName)
			{
				var profileZoomImage = dataManager.profileImageList.Find(image => image.name == selectedCharacter.characterName).transform
					.Find("Zoom").GetComponent<Image>().sprite;
			
				profileImage.sprite = profileZoomImage;	
			}
			else 
			{
				var profileCloseImage = dataManager.profileImageList.Find(image => image.name == selectedCharacter.characterName).transform
					.Find("Close").GetComponent<Image>().sprite;
			
				profileImage.sprite = profileCloseImage;
			}
		}

		public void SetBicProfile()
		{
			if (dataManager.gameObject.activeSelf == false) dataManager.gameObject.SetActive(true);
		}

		void ChangeSlot(string characterFindName)
		{
			foreach (var slot in slotList)
			{
				slot.gameObject.SetActive(false);
			}
			
			var n = 0;
			
			foreach (var item in selectedCharacter.itemList)
			{
				slotList[n].gameObject.SetActive(true);
				var slotButton = slotList[n].GetComponent<ItemButtonManager>();
				Insert(slotButton, item);
				n = n + 1;
			}
		}
		
		private void Insert(ItemButtonManager itemButton, Item item)
		{
			dataManager.gameManager.Insert(itemButton, item);

			if (itemButton.itemConsumable == ItemConsumable.Consume)
			{
				itemButton.actTextHighlighted.text = "Consume";
				itemButton.actTextNormal.text = "Consume";
			}
			else if (itemButton.itemConsumable == ItemConsumable.Equip)
			{
				if (itemButton.unEquip) return;
				
				itemButton.actTextHighlighted.text = "Equip";
				itemButton.actTextNormal.text = "Equip";
			}
		}
		
		public void ShowOwnedItem()
		{
			// list is limited
			var stockCount = inventoryList.Count;
			var newList = new List<Item>();
			var consumableCount = 0;
			foreach (var item in dataManager.ownedItems)
			{
				if (item.itemConsumable == ItemConsumable.Consume)
				{
					consumableCount = consumableCount + 1;
				}
			}
			var ownedItemCount = dataManager.ownedItems.Count - consumableCount + 1;
			if (ownedItemCount > stockCount)
			{
				for (int l = 0; l < stockCount; l++)
				{
					newList.Add(dataManager.ownedItems[l]);
				}
			}
			else
			{
				newList = dataManager.ownedItems;
			}
			
			ShowItem(newList);
		}

		void ShowItem(List<Item> list)
		{
			// set base icon
			foreach (var item in inventoryList)
			{
				item.gameObject.SetActive(false);
				var itemButton = item.GetComponent<ItemButtonManager>();
				itemButton.icon.GetComponent<Image>().sprite = dataManager.FindItemImage("Base").sprite;
			}

			// start
			int i = 0;
			int n = 0;
			
			var consumeList = new List<Item>();
			var stockItemList = new List<ItemButtonManager>();
			
			foreach (var item in list)
			{
				if (item.itemConsumable == ItemConsumable.Equip)
				{
					var equip = false;
					foreach (var character in dataManager.CurrentCharacterList)
					{
						if (character.itemList.Exists(equipped => equipped.itemId == item.itemId))
						{ 
							equip = true;
						}
					}
					if (equip)
					{
						print(item.itemName + " is equipped");
						i = i + 1;
					}
					else
					{
						print(item.itemName + " show");
						inventoryList[n].gameObject.SetActive(true);
						var itemButton = inventoryList[n].GetComponent<ItemButtonManager>();
						itemButton.quantity.gameObject.SetActive(false);
						GetMarketText(n).text = list[i].itemName;
						GetPriceText(n).text = list[i].itemPrice.ToString();
						SetIconImage(list[i].itemName, itemButton);
						
						Insert(itemButton, list[i]);	
						stockItemList.Add(itemButton);
						i = i + 1;
						n = n + 1;	
					}
				}
				else if (item.itemConsumable == ItemConsumable.Consume)
				{
					if (consumeList.Exists(consumeItem => item.itemName == consumeItem.itemName))
					{
						print(item.itemName + " is stacked");
						i = i + 1;
					}
					else
					{
						print(item.itemName + " show");
						inventoryList[n].gameObject.SetActive(true);
						var itemButton = inventoryList[n].GetComponent<ItemButtonManager>();
						itemButton.quantity.gameObject.SetActive(true);
						GetMarketText(n).text = list[i].itemName;
						GetPriceText(n).text = list[i].itemPrice.ToString();
						SetIconImage(list[i].itemName, itemButton);
						
						Insert(itemButton, list[i]);
						itemButton.quantity.text = GameUtility.CountItem(list, item).ToString();
						consumeList.Add(item);
						stockItemList.Add(itemButton);
						i = i + 1;
						n = n + 1;
					}
				}
				else
				{
					print(item.itemName + " show");
					inventoryList[n].gameObject.SetActive(true);
					var itemButton = inventoryList[n].GetComponent<ItemButtonManager>();
					inventoryList[n].GetComponent<ItemButtonManager>().quantity.gameObject.SetActive(false);
					GetMarketText(n).text = list[i].itemName;
					GetPriceText(n).text = list[i].itemPrice.ToString();
					SetIconImage(list[i].itemName, itemButton);
					
					Insert(itemButton, list[i]);
					i = i + 1;
					n = n + 1;
					stockItemList.Add(itemButton);
				}
			}
		}

		void SetIconImage(string itemName, ItemButtonManager itemButton)
		{
			var iconImage = dataManager.FindItemImage(itemName);
			if (iconImage == null) return;
			
			itemButton.icon.GetComponent<Image>().sprite = iconImage.sprite;
		}

		TextMeshProUGUI GetPriceText(int index)
		{
			return inventoryList[index].GetComponent<ItemButtonManager>().priceText;
		}

		TextMeshProUGUI GetMarketText(int index)
		{
			return inventoryList[index].GetComponent<ItemButtonManager>().itemNameText;
		}
		
		
		void SetInformation()
		{
			if (selectedCharacter == null) return;
			
			characterName.text = selectedCharacter.characterName;
			level.text = selectedCharacter.level.ToString();

			if (selectedCharacter.classType == CharacterClass.Ranger)
			{
				characterClass.text = dataManager.rangerClass;
			}
			else if (selectedCharacter.classType == CharacterClass.Claymore)
			{
				characterClass.text = dataManager.claymoreClass;
			}
			else
			{
				characterClass.text = dataManager.defaultClass;
			}
		}

		public void ChangeCharacter()
		{
			
			// selectedCharacter = dataManager.CurrentCharacterList.Find(i => i.characterName == characterFindName);
			ChangeSlot(selectedCharacter.characterName);
			SetInformation();                                                                             
			InitLevelExp();                                                                               
			ShowStats();
			deckManager.MakeDeck();
		}
		
	}
}