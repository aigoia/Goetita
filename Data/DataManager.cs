using System;
using System.Collections;
using System.Collections.Generic;
using Game.MainGame;
using Game.Window;
// using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine.UI;
using DeckManager = Game.MainGame.DeckManager;
using GameManager = Game.Window.GameManager;

public enum Trait
{
	Non, FaintResist, PhysicsResist, MagicResist, PoisonResist, MagicAttack, PhysicsAttack,  
}

public enum UnitClass
{
	Zero ,One, Two, Three,
}

public enum ItemGrade
{
	Zero, One, Two, Three, Four
}

namespace Game.Data
{
	public enum LevelUpWait
	{
		Non, Wait, Click,
	}

	// public enum LevelUpWhat
	// {
	// 	Non, Deal, Hp, Skill,
	// }

	public class CharacterList
	{
		public Character[] Characters;
	}
	
	public class ItemList
	{
		public Item[] Items;
	}
	
	[System.Serializable]
	public class BaseData
	{
		public int initGold;
		public int currentGold = 0;
		public int turnDate = 0;
		public int itemCount = 0;
		// public int initCount = 0;
		public bool initClear = false;
		
		public BaseData(int initGold, int currentGold, int turnData, int itemCount, bool initClear)
		{
			this.initGold = initGold;
			this.currentGold = currentGold;
			this.turnDate = turnData;
			this.itemCount = itemCount;
			this.initClear = initClear;
			// this.initCount = initCount;
		}
	}

	[System.Serializable]
	public class WhereData
	{
		public Vector3 initPosition = new Vector3();
		public Vector3 currentPosition = new Vector3();
		
		public WhereData(Vector3 initPosition, Vector3 currentPosition)
		{
			this.initPosition = initPosition;
			this.currentPosition = currentPosition;
		}
	}

	public enum LevelUpTrait
	{
		Non, Test,
	}

	public class DataManager : MonoBehaviour
	{
		public GameManager gameManager;
		public AccidentManager accidentManager;
		public MarketManager marketManager;
		public WindowTextManager windowTextManager;
		public GameObject recruitPanel;
		public GameObject recruitX;
		public CharacterSelect characterSelect;
		public Button inventoryButton;
		public GameObject basePanel;

		public Character testCharacter;
		public BaseData baseData;
		public WhereData whereData;
		public CurrentData currentData;

		public string claymoreClass = "Claymore";
		public string rangerClass = "Ranger";
		public string defaultClass = "Warrior";
		
		public Vector3 initPosition = new Vector3(0, 0.5f, 10);

		// public int gold = 0;
		public TextMeshProUGUI goldText;

		// public int turnDate = 0;
		public TextMeshProUGUI turnDateText;

		public List<GameObject> profileImageList;
		public List<Transform> itemImageList;

		public List<Character> CurrentCharacterList => LoadCharacter();

		public List<int> neededLevel = new List<int>();
		public List<int> levelStage = new List<int>();
		
		// public List<Item> items = new List<Item>();
		public List<Item> baseItemList = new List<Item>();
		public int testExp = 8;
		public int recruitSword = 1000;
		public int recruitGun = 1200;
		// public int setInitGold = 200;
		public List<Item> ownedItems;
		public bool noItems = false;

		public InventoryManager inventoryManager;
		public GameObject mainCharacter;

		public List<Frequency> frequencyList;
		public List<int> frequencyTiming;

		public UnityEvent levelUp;
		public LevelUpWait levelUpWait = LevelUpWait.Non;
		public OptionStyle currentOptionStyle = OptionStyle.Non;

		public TextMeshProUGUI levelUpName;
		public TextMeshProUGUI levelUpLevel;
		public TextMeshProUGUI currentDamageText;
		public TextMeshProUGUI currentHpText;
		public TextMeshProUGUI className;
		public DeckManager specialDeckManager;
		public GameObject background;
		public GameObject fullMembers;
		public GameObject noMoney;
		public List<Option> optionList;
		public List<Option> currentOptionList;
		
		public GameObject optionOneObject;
		public TextMeshProUGUI optionOneName;
		public TextMeshProUGUI optionOneCurrentData;
		public TextMeshProUGUI optionOneReward;
		
		public GameObject optionTwoObject;
		public TextMeshProUGUI optionTwoName;
		public TextMeshProUGUI optionTwoCurrentData;
		public TextMeshProUGUI optionTwoReward;
		
		public GameObject optionThreeObject;
		public TextMeshProUGUI optionThreeName;
		public TextMeshProUGUI optionThreeCurrentData;
		public TextMeshProUGUI optionThreeReward;
		
		public int recruitLimit = 5;
		
		private void Awake()
		{
			if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
			if (accidentManager == null) accidentManager = FindObjectOfType<AccidentManager>();
			if (currentData == null) currentData = FindObjectOfType<CurrentData>();
			if (inventoryManager == null) inventoryManager = FindObjectOfType<InventoryManager>();
			if (characterSelect == null) characterSelect = FindObjectOfType<CharacterSelect>();
			if (marketManager == null) marketManager = FindObjectOfType<MarketManager>();
			if (windowTextManager == null) windowTextManager = FindObjectOfType<WindowTextManager>();

			RenewalCharacter();
			
			foreach (var character in CurrentCharacterList)
			{
				print(character + " (" + character.characterName + ") : " +  character.currentHp);
				foreach (var item in character.itemList)
				{
					print(item.itemName);
				}
			}
		}

		public void Start()
		{
			SaveBaseItems();
		}

		[ContextMenu("SetItemImages")]
		public void SetItemImageList()
		{
			itemImageList = transform.Find("ItemImage").GetComponentsInChildren<Transform>().ToList();
		}

		public Image FindItemImage(string itemName)
		{
			if (itemName == null) return null;
			if (itemName == "Flak Jacket") itemName = "Core";
			if (itemName == "Armor") itemName = "Core";
			if (itemName == "Barrier") itemName = "Core";
			if (itemName == "Tanker") itemName = "Core";
			if (itemName == "Shield") itemName = "Core";
			
			return itemImageList.Find(i => i.name == itemName).GetComponent<Image>();
		}

		public void SettingData()
		{
			RenewalCharacter();
			LoadBaseData();
			RenewalTurnData();
			RenewalGold();
			LoadOwnedItems();
			LoadFrequency();
			LoadFrequencyTiming();
		}

		[ContextMenu("InitAll")]
		public void InitAll()
		{
			SetInitBase();
			SetInitCharacter();
			SetInitPosition();
			SaveBaseItems();
			SetInitOwnedItems();
		}
		
		void MakeItemList()
		{
			var newList = new List<Item>()
			{
				new Item(0, "Memory", 400, ItemType.Memory, CharacterClass.Non ,ItemConsumable.Consume, 8, 0, Trait.Non, ItemGrade.Zero),
				
				new Item(0, "Base Armor", 0, ItemType.Armor, CharacterClass.Non, ItemConsumable.Non, 0, 0, Trait.Non, ItemGrade.Zero),
				new Item(0, "Flak Jacket", 700, ItemType.Armor, CharacterClass.Non, ItemConsumable.Equip, 1, 0, Trait.Non, ItemGrade.One),
				new Item(0, "Armor", 1400, ItemType.Armor, CharacterClass.Non, ItemConsumable.Equip, 2, 0, Trait.Non, ItemGrade.Two),
				new Item(0, "Barrier", 2500, ItemType.Armor, CharacterClass.Non, ItemConsumable.Equip, 3, 0, Trait.Non, ItemGrade.Two),
				new Item(0, "Tanker", 3200, ItemType.Armor, CharacterClass.Non, ItemConsumable.Equip, 4, 0, Trait.Non, ItemGrade.Three),
				new Item(0, "Shield", 5000, ItemType.Armor, CharacterClass.Non, ItemConsumable.Equip, 5, 0, Trait.Non, ItemGrade.Four),
				
				new Item(0, "Base Sword", 0, ItemType.Weapon, CharacterClass.Claymore, ItemConsumable.Non, 1, 1, Trait.Non, ItemGrade.Zero),
				new Item(0, "Sword", 800, ItemType.Weapon, CharacterClass.Claymore, ItemConsumable.Equip, 0, 5, Trait.Non, ItemGrade.One),
				new Item(0, "Katana", 1700, ItemType.Weapon, CharacterClass.Claymore, ItemConsumable.Equip, 1, 6, Trait.Non, ItemGrade.Two),
				new Item(0, "Bic One", 900, ItemType.Weapon, CharacterClass.Claymore, ItemConsumable.Equip, 2, 1, Trait.ArmorPiercing, ItemGrade.One),
				new Item(0, "Hammer", 1900, ItemType.Weapon, CharacterClass.Claymore, ItemConsumable.Equip, 3, 2, Trait.ArmorPiercing, ItemGrade.Two),
				new Item(0, "Axe", 3500, ItemType.Weapon, CharacterClass.Claymore, ItemConsumable.Equip, 4, 4, Trait.ArmorPiercing, ItemGrade.Three),
				new Item(0, "Alane", 6100, ItemType.Weapon, CharacterClass.Claymore, ItemConsumable.Equip, 5, 5, Trait.ArmorPiercing, ItemGrade.Four),
				
				new Item(0, "Base Gun", 0, ItemType.Weapon, CharacterClass.Ranger, ItemConsumable.Non, 0, 2, Trait.Non, ItemGrade.Zero),
				new Item(0, "Aes Gun", 500, ItemType.Weapon, CharacterClass.Ranger, ItemConsumable.Equip, 0, 3, Trait.ArmorPiercing, ItemGrade.One),
				new Item(0, "Laser Gun", 1300, ItemType.Weapon, CharacterClass.Ranger, ItemConsumable.Equip, 1, 4, Trait.ArmorPiercing, ItemGrade.Two),
				new Item(0, "Pistol Gun", 600, ItemType.Weapon, CharacterClass.Ranger, ItemConsumable.Equip, 2, 2, Trait.Non, ItemGrade.One),
				new Item(0, "High Gun", 1200, ItemType.Weapon, CharacterClass.Ranger, ItemConsumable.Equip, 3, 3, Trait.Non, ItemGrade.Two),
				new Item(0, "Dubble Gun", 1100, ItemType.Weapon, CharacterClass.Ranger, ItemConsumable.Equip, 0, 8, Trait.Non, ItemGrade.Two),
				new Item(0, "Pulse Gun", 2000, ItemType.Weapon, CharacterClass.Ranger, ItemConsumable.Equip, 1, 12, Trait.Non, ItemGrade.Three),
				new Item(0, "Rail Gun", 4000, ItemType.Weapon, CharacterClass.Ranger, ItemConsumable.Equip, 2, 16, Trait.Non, ItemGrade.Four),
				new Item(0, "Plasma Gun", 2700, ItemType.Weapon, CharacterClass.Ranger, ItemConsumable.Equip, 3, 7, Trait.ArmorPiercing, ItemGrade.Three),
			};
			
			baseItemList = newList;
		}
		
		[ContextMenu("SetMarketItems")]
		public void SetMarketItems()
		{
			marketManager.defaultBuyList = new List<Item>
			{
				FindBaseItem("Memory"),
				
				FindBaseItem("Sword"),
				FindBaseItem("Katana"),
				FindBaseItem("Bic One"),
				FindBaseItem("Hammer"),
				FindBaseItem("Axe"),
				FindBaseItem("Alane"),
				
				FindBaseItem("Flak Jacket"),
				FindBaseItem("Armor"),
				FindBaseItem("Barrier"),
				FindBaseItem("Tanker"),
				FindBaseItem("Shield"),
				
				FindBaseItem("Aes Gun"),
				FindBaseItem("Laser Gun"),
				FindBaseItem("Pistol Gun"),
				FindBaseItem("High Gun"),
				FindBaseItem("Dubble Gun"),
				FindBaseItem("Pulse Gun"),
				FindBaseItem("Rail Gun"),
				FindBaseItem("Plasma Gun"),
			};
		}

		Item FindBaseItem(string itemName)
		{
			return baseItemList.Find(i => i.itemName == itemName);
		}
		
		[ContextMenu("LoadOwnedItems")]
		public void LoadOwnedItems()
		{
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/OwnedItemList.Json");
			string jsonData = File.ReadAllText(path);
			var newItems = JsonUtility.FromJson<ItemList>(jsonData);
			ownedItems = newItems.Items.ToList();
		}

		[ContextMenu("SaveOwnedItems")]
		public void SaveOwnedItems()
		{
			var itemList = new ItemList {Items = ownedItems.ToArray()};
			string jsonData = JsonUtility.ToJson(itemList, true);
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/OwnedItemList.Json");
			File.WriteAllText(path, jsonData);
		}

		[ContextMenu("SetInitTestItems")]
		public void SetInitTestItems()
		{
			ownedItems = new List<Item>();
			for (int i = 0; i < 9; i++) 
			{
				ownedItems.Add(FindBaseItem("Memory"));	
			}
			SaveOwnedItems();
		}
		
		[ContextMenu("SetInitOwnedItems")]
		public void SetInitOwnedItems()
		{
			ownedItems = new List<Item>();
			SaveOwnedItems();
		}
		
		[ContextMenu("LoadBaseItems")]
		public List<Item> LoadBaseItems()
		{
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/BaseItemList.Json");
			string jsonData = File.ReadAllText(path);
			var newItems = JsonUtility.FromJson<ItemList>(jsonData);
			return newItems.Items.ToList();
		}

		[ContextMenu("SaveBaseItems")]
		public void SaveBaseItems()
		{
			MakeItemList();
			var itemList = new ItemList {Items = this.baseItemList.ToArray()};
			string jsonData = JsonUtility.ToJson(itemList, true);
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/BaseItemList.Json");
			File.WriteAllText(path, jsonData);
		}

		[ContextMenu("SetInitBase")]
		public void SetInitBase()
		{
			baseData.currentGold = baseData.initGold;
			baseData.turnDate = 0;
			baseData.itemCount = 1;
			baseData.initClear = false;
			SaveBaseData();
		}

		public void SetInitBase(int initGold)
		{
			baseData.currentGold = initGold;
			baseData.turnDate = 0;
			baseData.itemCount = 1;
			baseData.initClear = false;
			SaveBaseData();
		}

		[ContextMenu("MakeLevelStage")]
		public void MakeLevelStage()
		{
			var whileCount = 0;
			var levelCount = neededLevel.Count;
			var expToLevelList = new List<int> {0};

			while (whileCount < levelCount)
			{
				var levelExp = expToLevelList[whileCount] + neededLevel[whileCount];
				expToLevelList.Add(levelExp);
				whileCount++;
			}

			levelStage = expToLevelList;
		}

		CharacterList GetCharacter()
		{
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/CharacterList.Json");
			string jsonData = File.ReadAllText(path);
			return JsonUtility.FromJson<CharacterList>(jsonData);
		}

		void SetCharacter(List<Character> characters)
		{
			var characterList = new CharacterList() {Characters = characters.ToArray()};
			string jsonData = JsonUtility.ToJson(characterList, true);
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/CharacterList.Json");
			File.WriteAllText(path, jsonData);
		}

		[ContextMenu("LoadCharacter")]
		public List<Character> LoadCharacter()
		{
			return GetCharacter().Characters.ToList();
		}
		
		[ContextMenu("SaveCharacter")]
		public void SaveCharacter(List<Character> characters)
		{
			SetCharacter(characters);
		}

		[ContextMenu("SetNullCharacter")]
		public void SetNullCharacter()
		{
			var newList = new List<Character>()
			{
				new Character( "Tina", CharacterClass.Non, 0, 0, 0, 0, 0, 0, 0, new List<Item>()),
				new Character( "Den", CharacterClass.Non, 0, 0, 0, 0, 0, 0, 0, new List<Item>()),
				new Character( "Flora", CharacterClass.Non, 0, 0, 0, 0, 0,0, 0, new List<Item>()),
				new Character( "Mia", CharacterClass.Non, 0, 0, 0, 0, 0, 0, 0, new List<Item>()),
				new Character( "Clare", CharacterClass.Non, 0, 0, 0, 0, 0, 0, 0, new List<Item>()),
				// new Character( "Sue", CharacterClass.Non, 0, 0, 0, 0, 0, 0, 0, new List<Item>()),
			};
			
			SaveCharacter(newList);
			RenewalCharacter();
		}

		[ContextMenu("SetInitCharacter")]
		public void SetInitCharacter()
		{
			SetNullCharacter();
			
			var newList = new List<Character>()
			{
				new Character("Tina", CharacterClass.Claymore, 6, 6, 3, 1, 1, 0, 0, new List<Item>()),
				new Character("Den", CharacterClass.Ranger, 4, 4, 2+1, 2, 0 , 0,0, new List<Item>()),
				new Character("Flora" , CharacterClass.Ranger, 4+1, 4+1, 2, 2, 0, 0, 0, new List<Item>()),
				// new Character("Mia", CharacterClass.Claymore, 7, 7, 3, 0, 1, 0, 0, new List<Item>()),
				// new Character("Clare" ,CharacterClass.Claymore, 7, 7, 3, 0, 1, 0, 0, new List<Item>()),
				// new Character("Sue", CharacterClass.Claymore, 7, 7, 3, 0, 1, 0, 0, new List<Item>()),
			};
			
			SaveCharacter(newList);
			RenewalCharacter();
		}
		
		public void SetNewCharacter(List<Character> newCharacter)
		{
			SetNullCharacter();

			var newList = newCharacter;
			SaveCharacter(newList);
			RenewalCharacter();
		}
		
		[ContextMenu("LoadBaseData")]
		public void LoadBaseData()
		{
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/BaseData.Json");
			string jsonData = File.ReadAllText(path);
			baseData = JsonUtility.FromJson<BaseData>(jsonData);
		}

		[ContextMenu("SaveBaseData")]
		public void SaveBaseData()
		{
			string jsonData = JsonUtility.ToJson(baseData, true);
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/BaseData.Json");
			File.WriteAllText(path, jsonData);
		}
		
		[ContextMenu("LoadFrequency")]
		public void LoadFrequency()
		{
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/Frequency.Json");
			string jsonData = File.ReadAllText(path);
			var data = JsonUtility.FromJson<Serialization<Frequency>>(jsonData);
			frequencyList = data.ToList();
		}
		
		[ContextMenu("SaveFrequency")]
		public void SaveFrequency()
		{
			string jsonData = JsonUtility.ToJson(new Serialization<Frequency>(frequencyList), true);
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/Frequency.Json");
			File.WriteAllText(path, jsonData);
		}
		
		[ContextMenu("LoadFrequencyTiming")]
		public void LoadFrequencyTiming()
		{
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/FrequencyTiming.Json");
			string jsonData = File.ReadAllText(path);
			var data = JsonUtility.FromJson<Serialization<int>>(jsonData);
			frequencyTiming = data.ToList();
		}
		
		[ContextMenu("SaveFrequencyTiming")]
		public void SaveFrequencyTiming()
		{
			string jsonData = JsonUtility.ToJson(new Serialization<int>(frequencyTiming), true);
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/FrequencyTiming.Json");
			File.WriteAllText(path, jsonData);
		}

		[ContextMenu("SaveInitPosition")]
		public void SetInitPosition()
		{
			whereData.initPosition = initPosition;
			whereData.currentPosition = initPosition;
			
			string jsonData = JsonUtility.ToJson(whereData, true);                                            
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/WhereData.Json");             
			File.WriteAllText(path, jsonData);
		}
		
		[ContextMenu("SavePosition")]                                                                               
		public void SavePosition(Vector3 position)
		{
			whereData.currentPosition = position;			
			string jsonData = JsonUtility.ToJson(whereData, true);                                                      
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/WhereData.Json");                       
			File.WriteAllText(path, jsonData);                                                                          
		}

		[ContextMenu("LoadPosition")]
		public void LoadPosition()
		{
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/WhereData.Json");                  
			string jsonData = File.ReadAllText(path);                                                             
			whereData = JsonUtility.FromJson<WhereData>(jsonData);                                                  
		}
		
		public Vector3 GetPosition()
		{
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/WhereData.Json");                  
			string jsonData = File.ReadAllText(path);                                                             
			var where = JsonUtility.FromJson<WhereData>(jsonData);
			return where.currentPosition;
		}

		public List<Character> SwordCharacters()
		{
			var newList = new List<Character>()
			{
				new Character("Tina", CharacterClass.Claymore, 6, 6, 3, 1, 1, 0, 0, new List<Item>()),
				new Character("Den", CharacterClass.Claymore, 6, 6, 3+1, 1,1, 0, 0, new List<Item>()),
				new Character("Flora" , CharacterClass.Claymore, 6+1, 6+1, 3, 1, 1, 0, 0, new List<Item>()),
				new Character("Mia", CharacterClass.Claymore, 6, 6, 3, 1, 1+1, 0, 0, new List<Item>()),
				new Character("Clare" ,CharacterClass.Claymore, 6, 6, 3, 1+2, 1, 0, 0, new List<Item>()),
				// new Character("Sue", CharacterClass.Claymore, 7, 7, 3, 0, 1, 0, 0, new List<Item>()),
			};

			return newList;
		}
		
		public void RecruitSword()
		{
			if (baseData.currentGold < recruitSword)
			{	
				noMoney.SetActive(true);
				return;
			}

			var oldList = LoadCharacter();
			if (oldList.Count >= recruitLimit)
			{
				fullMembers.SetActive(true);
				return;
			}

			var newList = SwordCharacters();

			foreach (var character in CurrentCharacterList)
			{
				newList.RemoveAll(i => i.characterName == character.characterName);
			}

			// print("WaitingList");
			// GameUtility.PrintList(newList);

			GameUtility.ShuffleList(newList);
			print("Add " + newList[0].characterName);
			oldList.Add(newList[0]);

			SaveCharacter(oldList);
			RenewalCharacter();
			
			// spend gold
			baseData.currentGold = baseData.currentGold - recruitSword;
			RenewalGold();
			
			accidentManager.SpendAccident();
			recruitPanel.SetActive(false);
			recruitX.SetActive(false);
			
			// characterSelect.hpBarList.Find(i => i.characterName == waitingList[0].characterName).RemakeHp();;
			inventoryButton.onClick.Invoke();
			characterSelect.ReSetHp();
		}

		public List<Character> GunCharacters()
		{
			var newList = new List<Character>()
			{
				new Character("Tina", CharacterClass.Ranger, 4, 4, 2, 2, 0, 0, 0, new List<Item>()),
				new Character("Den", CharacterClass.Ranger, 4, 4, 2+1, 2, 0 , 0,0, new List<Item>()),
				new Character("Flora" , CharacterClass.Ranger, 4+1, 4+1, 2, 2, 0, 0, 0, new List<Item>()),
				new Character("Mia", CharacterClass.Ranger, 4, 4, 2,2, 0+1, 0,  0, new List<Item>()),
				new Character("Clare" , CharacterClass.Ranger, 4, 4, 2, 2+2, 0, 0, 0, new List<Item>()),
				// new Character("Sue",  CharacterClass.Ranger, 4, 4, 3, 0, 0, 0, 0, new List<Item>()),
			};

			return newList;
		}

		public void RecruitGun()
		{
			if (baseData.currentGold < recruitGun)
			{
				noMoney.SetActive(true);
				return;
			}

			var oldList = LoadCharacter();
			if (oldList.Count >= recruitLimit)
			{
				fullMembers.SetActive(true);
				return;
			}

			var newList = GunCharacters();
			
			var waitingList  = new List<Character>(); 
            
            foreach (var character in newList)
            {
            	if (!oldList.Exists(i => i.characterName == character.characterName))
            	{
            		if (!oldList.Exists(i => i.characterName == character.characterName))
                    {
	                    waitingList.Add(character);
                    }
            	}
            }
            
            oldList.Add(waitingList[0]);

            SaveCharacter(oldList);
            RenewalCharacter();
            
            // spend gold
            baseData.currentGold = baseData.currentGold - recruitGun;
            RenewalGold();
            
            accidentManager.SpendAccident();
            recruitPanel.SetActive(false);
            recruitX.SetActive(false);
            
            // characterSelect.hpBarList.Find(i => i.characterName == waitingList[0].characterName).RemakeHp();;
            inventoryButton.onClick.Invoke();
            characterSelect.ReSetHp();
		}

		string TurnToText(int turn)
		{
			if (turn > 999)
			{
				return "999";
			}
			else
			{
				return turn.ToString();
			}
		}

		string GoldToText(int money)
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
		
		public void RenewalTurnData()
		{
			if (turnDateText == null) return;
			
			turnDateText.text = TurnToText(baseData.turnDate);
			SaveBaseData();
		}

		public void AddGold(int gold)
		{
			baseData.currentGold = baseData.currentGold + gold;
			RenewalGold();
		}
		
		public void RenewalGold()
		{
			if (goldText == null) return;
			
			goldText.text = GoldToText(baseData.currentGold);
			SaveBaseData();
		}

		public void RenewalCharacter()
		{
			// CurrentCharacterList = LoadCharacter();
		}

		public void HealHpFull()
		{
			foreach (var character in CurrentCharacterList)
			{
				character.currentHp = character.baseHp;
			}
			
			foreach (var hpBar in inventoryManager.characterSelect.hpBarList)
			{
				hpBar.RenewalHp();
			}
			
			SaveCharacter(CurrentCharacterList);
		}

		[ContextMenu("Add Exp Test")]
		public void AddExpTest()
		{
			AddAllExp(testExp);
		}

		[ContextMenu("Level Check")]
		public void LevelCheck(bool memory = false)
		{
			StartCoroutine(LevelUp(memory));
			SaveCharacter(CurrentCharacterList);
		}

		void SetLevelUpPanel(string characterName, int level, int baseDeal, int plusDeal, int hp, int baseArmor, CharacterClass characterClass)
		{
			currentOptionList = null;
			levelUpName.text = characterName;
			levelUpLevel.text = level.ToString();
			
			List<int> numberList = new List<int>();
			
			for (int i = 0; i < optionList.Count; i++)
			{
				numberList.Add(i);
			}
		
			GameUtility.ShuffleList(numberList);

			List<Option> newOptionList = new List<Option> {optionList[numberList[0]], optionList[numberList[1]]};
			currentOptionList = newOptionList;
		
			optionOneObject.SetActive(true);
			// optionOneName.text = currentOptionList[0].optionName;
			if (currentOptionList[0].optionStyle == OptionStyle.BaseDeal)
			{
				optionOneName.text = windowTextManager.baseDealPlus;
				optionOneCurrentData.text = windowTextManager.current + " : " + baseDeal;
			}
			else if (currentOptionList[0].optionStyle == OptionStyle.PlusDeal)
			{
				optionOneName.text = windowTextManager.randomDealPlus;
				optionOneCurrentData.text = windowTextManager.current + " : " + plusDeal;
			}
			else if (currentOptionList[0].optionStyle == OptionStyle.Hp)
			{
				optionOneName.text = windowTextManager.hpPlus;
				optionOneCurrentData.text = windowTextManager.current + " : " + hp;
			}
			else if (currentOptionList[0].optionStyle == OptionStyle.Armor)
			{
				optionOneName.text = windowTextManager.armorPlus;
				optionOneCurrentData.text = windowTextManager.current + " : " + baseArmor;
			}
			
			optionOneReward.text = "+" + currentOptionList[0].optionValue.ToString();
		
			optionTwoObject.SetActive(true);
			// optionTwoName.text = currentOptionList[1].optionName;
			if (currentOptionList[1].optionStyle == OptionStyle.BaseDeal)
			{
				optionTwoName.text = windowTextManager.baseDealPlus;
				optionTwoCurrentData.text = windowTextManager.current + " : " + baseDeal;
			}
			else if (currentOptionList[1].optionStyle == OptionStyle.PlusDeal)
			{
				optionTwoName.text = windowTextManager.randomDealPlus;
				optionTwoCurrentData.text = windowTextManager.current + " : " + plusDeal;
			}
			else if (currentOptionList[1].optionStyle == OptionStyle.Hp)
			{
				optionTwoName.text = windowTextManager.hpPlus;
				optionTwoCurrentData.text = windowTextManager.current + " : " + hp;
			}
			else if (currentOptionList[1].optionStyle == OptionStyle.Armor)
			{
				optionTwoName.text = windowTextManager.armorPlus;
				optionTwoCurrentData.text = windowTextManager.current + " : " + baseArmor;
			}
			optionTwoReward.text = "+" + currentOptionList[1].optionValue.ToString();
			
			if (characterClass == CharacterClass.Claymore)
			{
				className.text = claymoreClass;
			}
			else if (characterClass == CharacterClass.Ranger)
			{
				className.text = rangerClass;
			}
			else
			{
				className.text = defaultClass;
			}
		}
		
		public void AddAllExp(int exp)
		{
			if (CurrentCharacterList == null)
			{
				print("CurrentList is null");
				return;
			}
			
			var each = (int) exp / CurrentCharacterList.Count;
            
			foreach (var character in CurrentCharacterList)
			{
				if (character.currentHp <= 0) continue;
				character.exp = character.exp + each;
                
				print(character.characterName + " : " + character.exp);
			}
            
			SaveCharacter(CurrentCharacterList);
		}

		void AddEqualExp(int exp)
		{
			var each = (int) exp / CurrentCharacterList.Count;
            
			foreach (var character in CurrentCharacterList)
			{
				if (character.currentHp <= 0) continue;
				character.exp = character.exp + each;
                
				print(character.characterName + " exp : " + character.exp);
			}
            
			SaveCharacter(CurrentCharacterList);
		}

		IEnumerator LevelUp(bool memory = false)
		{
			basePanel.SetActive(false);
			var newCharacterList = CurrentCharacterList;
			var selectedCharacter = inventoryManager.selectedCharacter;
			
			foreach (var character in newCharacterList)
			{
				var currentLevel = character.level;

				// print(character.CharacterName + " current level : " + character.Level);
				
				for (int i = 1; i < neededLevel.Count + 1; i++)
				{
					var toLevel = character.level + 1;
					if (toLevel > neededLevel.Count) break;
					var toLevelStage = levelStage[toLevel];

					if (character.exp >= toLevelStage)
					{
						if (memory) inventoryManager.gameObject.SetActive(false);
						
						levelUp.Invoke();
						print(character.characterName + " base deal : " + character.baseDeal);
						SetLevelUpPanel(character.characterName, toLevel, character.baseDeal, character.plusDeal, character.baseHp, character.baseArmor, character.classType);
						levelUpWait = LevelUpWait.Wait;

						while (levelUpWait != LevelUpWait.Click)
						{
							yield return null;
						}
						
						if (currentOptionStyle == OptionStyle.BaseDeal) 
						{
							character.level = toLevel;
							character.baseDeal = character.baseDeal + optionList.Find(option => option.optionStyle == OptionStyle.BaseDeal).optionValue;
							print("selected base deal up (" + character.level + ", " + character.baseDeal + " + " + character.plusDeal + ")") ;
						}
						if (currentOptionStyle == OptionStyle.PlusDeal) 
						{
							character.level = toLevel;
							character.plusDeal = character.plusDeal + optionList.Find(option => option.optionStyle == OptionStyle.PlusDeal).optionValue;
							print("selected plus deal up (" + character.level + ", " + character.baseDeal + " + " + character.plusDeal + ")") ;
						}
						else if (currentOptionStyle == OptionStyle.Hp)
						{
							character.level = toLevel;
							character.baseHp = character.baseHp + optionList.Find(option => option.optionStyle == OptionStyle.Hp).optionValue;
							character.currentHp = character.currentHp + optionList.Find(option => option.optionStyle == OptionStyle.Hp).optionValue;
							print("selected hp up (" +  character.level + ", " + character.baseHp + ", " + character.currentHp + ")");
						}
						else if (currentOptionStyle == OptionStyle.Armor)
						{
							character.level = toLevel;
							character.baseArmor = character.baseArmor + optionList.Find(option => option.optionStyle == OptionStyle.Armor).optionValue;
							print("selected hp up (" +  character.level + ", " + character.baseArmor + ", " + ")");
						}

						levelUpWait = LevelUpWait.Non;
						currentOptionStyle = OptionStyle.Non;
						
						print(character.characterName + " level up to " + toLevel);
					}
				}
			}
			
			SaveCharacter(newCharacterList);
			RenewalCharacter();
			basePanel.SetActive(true);
			if (memory)
			{
				if (inventoryManager.gameObject.activeSelf == false)
				{
					inventoryManager.gameObject.SetActive(true);
					inventoryManager.SetProfile();
					inventoryManager.InitLevelExp();
					inventoryManager.DefaultOn();
					inventoryManager.ShowOwnedItem();
					background.SetActive(true);
					characterSelect.ReSetHp();
					// characterSelect.hpBarList.Find(i => i.characterId == inventoryManager.selectedCharacter.characterId).RemakeHp();
					var characters = CurrentCharacterList;
					characterSelect.activeButtonList.Find(i => characters[i.index].characterName == selectedCharacter.characterName)
							.GetComponent<Button>().onClick.Invoke();
					inventoryManager.defaultButton.onClick.Invoke();
				}
			}
		}
		
		public void SaveAll()
		{
			SaveCharacter(CurrentCharacterList);
			SaveBaseData();
		}
		
		public void ClickLevelUp()
		{
			levelUpWait = LevelUpWait.Click;
		}

		public void SelectOptionOne()
		{
			if (currentOptionList[0].optionStyle == OptionStyle.Hp)
			{
				currentOptionStyle = OptionStyle.Hp;
				print(currentOptionStyle);
			}
			else if (currentOptionList[0].optionStyle == OptionStyle.BaseDeal)
			{
				currentOptionStyle = OptionStyle.BaseDeal;
				print(currentOptionStyle);
			}
			else if (currentOptionList[0].optionStyle == OptionStyle.PlusDeal)
			{
				currentOptionStyle = OptionStyle.PlusDeal;
				print(currentOptionStyle);
			}
			else if (currentOptionList[0].optionStyle == OptionStyle.Armor)
			{
				currentOptionStyle = OptionStyle.Armor;
				print(currentOptionStyle);
			}
			else if (currentOptionList[0].optionStyle == OptionStyle.Hp)
			{
				currentOptionStyle = OptionStyle.Hp;
				print(currentOptionStyle);
			}
		}
		
		public void SelectOptionTwo()
		{
			if (currentOptionList[1].optionStyle == OptionStyle.Hp)
			{
				currentOptionStyle = OptionStyle.Hp;
				print(currentOptionStyle);
			}
			else if (currentOptionList[1].optionStyle == OptionStyle.BaseDeal)
			{
				currentOptionStyle = OptionStyle.BaseDeal;
				print(currentOptionStyle);
			}
			else if (currentOptionList[1].optionStyle == OptionStyle.PlusDeal)
			{
				currentOptionStyle = OptionStyle.PlusDeal;
				print(currentOptionStyle);
			}
			else if (currentOptionList[1].optionStyle == OptionStyle.Armor)
			{
				currentOptionStyle = OptionStyle.Armor;
				print(currentOptionStyle);
			}
			else if (currentOptionList[1].optionStyle == OptionStyle.Hp)
			{
				currentOptionStyle = OptionStyle.Hp;
				print(currentOptionStyle);
			}
		}
		
		public void SelectOptionThree()
		{
			if (currentOptionList[2].optionStyle == OptionStyle.Hp)
			{
				currentOptionStyle = OptionStyle.Hp;
				print(currentOptionStyle);
			}
			else if (currentOptionList[2].optionStyle == OptionStyle.BaseDeal)
			{
				currentOptionStyle = OptionStyle.BaseDeal;
				print(currentOptionStyle);
			}
			else if (currentOptionList[2].optionStyle == OptionStyle.PlusDeal)
			{
				currentOptionStyle = OptionStyle.PlusDeal;
				print(currentOptionStyle);
			}
			else if (currentOptionList[2].optionStyle == OptionStyle.Armor)
			{
				currentOptionStyle = OptionStyle.Armor;
				print(currentOptionStyle);
			}
			else if (currentOptionList[2].optionStyle == OptionStyle.Hp)
			{
				currentOptionStyle = OptionStyle.Hp;
				print(currentOptionStyle);
			}
		}

		public Sprite SetProfileImage(Character player, string what)
		{
			var sprite = profileImageList.Find(image => image.name == player.characterName).transform
				.Find(what).GetComponent<Image>().sprite;
			return sprite;
		}
	}
	
	[Serializable]
	public class Serialization<T>
	{
		[SerializeField]
		List<T> target;
		public List<T> ToList() { return target; }

		public Serialization(List<T> target)
		{
			this.target = target;
		}
	}

	public enum OptionStyle
	{
		Non, BaseDeal, PlusDeal, Armor, Hp,
	}

	[System.Serializable]
	public class Option
	{
		public OptionStyle optionStyle = OptionStyle.Non;
		public string optionName;
		public int optionValue;
	}

	[System.Serializable]
	public class Frequency
	{
		public int easy;
		public int normal;
		public int hard;
	}
}