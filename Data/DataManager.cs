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

namespace Game.Data
{
	public enum LevelUpWait
	{
		Non, Wait, Click,
	}

	public enum LevelUpWhat
	{
		Non, Deal, Hp, Skill,
	}

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
		
		public BaseData(int initGold, int currentGold, int turnData, int itemCount)
		{
			this.initGold = initGold;
			this.currentGold = currentGold;
			this.turnDate = turnData;
			this.itemCount = itemCount;
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

	public class DataManager : MonoBehaviour
	{
		public Character testCharacter;
		public BaseData baseData;
		public WhereData whereData;

		public Vector3 initPosition = new Vector3(5, 0.5f, 15);

		// public int gold = 0;
		public TextMeshProUGUI goldText;

		// public int turnDate = 0;
		public TextMeshProUGUI turnDateText;

		public List<GameObject> imageList;
		public List<Character> currentCharacterList;
		public List<int> neededLevel = new List<int>();
		public List<int> levelStage = new List<int>();
		public List<Item> itemList = new List<Item>();

		public int testExp = 8;
		// public int setInitGold = 200;
		public List<Item> ownedItems;

		public InventoryManager inventoryManager;
		public GameObject mainCharacter;

		public UnityEvent levelUp;
		public LevelUpWait levelUpWait = LevelUpWait.Non;
		public LevelUpWhat levelUpWhat = LevelUpWhat.Non;

		public TextMeshProUGUI levelUpName;
		public TextMeshProUGUI levelUpLevel;

		private void Awake()
		{
			if (inventoryManager == null) inventoryManager = FindObjectOfType<InventoryManager>();

			currentCharacterList = LoadCharacter();
			
			foreach (var character in currentCharacterList)
			{
				print(character + " (" + character.characterId + ") : " +  character.currentHp);
				foreach (var item in character.itemList)
				{
					print(item.itemName);
				}
			}
		}

		public void Start()
		{
			currentCharacterList = LoadCharacter();
			LoadBaseData();
			RenewalTurnData();
			RenewalGold();
			LoadOwnedItems();
		}

		[ContextMenu("InitAll")]
		public void InitAll()
		{
			SetInitBase();
			SetInitCharacter();
			SetInitPosition();
			SetInitOwnedItems();
		}

		[ContextMenu("MakeItemList")]
		public void MakeItemList()
		{
			var newList = new List<Item>()
			{
				new Item(0, "Pie", 500, ItemType.Weapon),
				new Item(0, "Cookie", 1200, ItemType.Weapon),
				new Item(0, "Ice cream", 3100, ItemType.Weapon),
				new Item(0, "Cake", 7200, ItemType.Weapon),
				new Item(0, "Formal dress", 1000, ItemType.Armor),
				new Item(0, "Tactical dress", 1900, ItemType.Armor),
				new Item(0, "Party dress", 5500, ItemType.Armor),
				new Item(0, "Underwear", 2800, ItemType.Underwear),
				// new Item(0, "Blue screen", 2800, ItemType.BlueScreen),
				// new Item(0, "Steam pack", 3500, ItemType.SteamPack),
			};
			
			itemList = newList;
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
			var baseItemList = new ItemList {Items = ownedItems.ToArray()};
			string jsonData = JsonUtility.ToJson(baseItemList, true);
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/OwnedItemList.Json");
			File.WriteAllText(path, jsonData);
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
			var baseItemList = new ItemList {Items = itemList.ToArray()};
			string jsonData = JsonUtility.ToJson(baseItemList, true);
			string path = Path.Combine(Application.dataPath + "/StreamingAssets/BaseItemList.Json");
			File.WriteAllText(path, jsonData);
		}

		[ContextMenu("SetInitBase")]
		public void SetInitBase()
		{
			baseData.currentGold = baseData.initGold;
			baseData.turnDate = 0;
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
			var characterList = new CharacterList() { Characters = characters.ToArray()};
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
				new Character(1, "Dietrich", CharacterClass.Non, 0, 0, 0, 0, 0, new List<Item>()),
				new Character(2, "Deneve", CharacterClass.Non, 0, 0, 0, 0, 0, new List<Item>()),
				new Character(3, "Flora", CharacterClass.Non, 0, 0, 0, 0, 0, new List<Item>()),
				new Character(4, "Mirria", CharacterClass.Non, 0, 0, 0, 0, 0, new List<Item>()),
				new Character(5, "Clare", CharacterClass.Non, 0, 0, 0, 0, 0, new List<Item>()),
				new Character(6, "Jean", CharacterClass.Non, 0, 0, 0, 0, 0, new List<Item>()),
			};

			currentCharacterList = newList;
			SaveCharacter(currentCharacterList);
		}

		[ContextMenu("SetInitCharacter")]
		public void SetInitCharacter()
		{
			SetNullCharacter();
			
			var newList = new List<Character>()
			{
				// new Character(1, "Dietrich", CharacterClass.Claymore, 6, 6, 0 , 0, 0, new List<Item>()),    
				new Character(2, "Deneve", CharacterClass.Ranger, 5, 5, 0, 0, 0, new List<Item>()),
				// new Character(3, "Flora" , CharacterClass.Ranger, 4, 2, 0, 0, 0, new List<Item>()),         
				new Character(4, "Mirria", CharacterClass.Ranger, 5, 5, 0, 0, 0, new List<Item>()),
				new Character(5, "Clare", CharacterClass.Claymore, 8, 8, 1, 0, 0, new List<Item>()),
				// new Character(6, "Jean", CharacterClass.Ranger, 4, 4, 0, 0, 0, new List<Item>()),           
			};

			currentCharacterList = newList;
			SaveCharacter(currentCharacterList);
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

		public void RecruitSword()
		{
			var oldList = LoadCharacter();

			var newList = new List<Character>()
            {
	            new Character(1, "Dietrich", CharacterClass.Claymore, 8, 8, 1, 0, 0, new List<Item>()),
	            new Character(2, "Deneve", CharacterClass.Claymore, 8, 8, 1, 0, 0, new List<Item>()),
	            new Character(3, "Flora" , CharacterClass.Claymore, 8, 8, 1, 0, 0, new List<Item>()),
	            new Character(4, "Mirria", CharacterClass.Claymore, 8, 8, 1, 0, 0, new List<Item>()),
	            new Character(5, "Clare" ,CharacterClass.Claymore, 8, 8, 1, 0, 0, new List<Item>()),
	            new Character(6, "Jean", CharacterClass.Claymore, 8, 8, 1, 0, 0, new List<Item>()),
            };

			var waitingList  = new List<Character>(); 
			
			foreach (var character in newList)
			{
				if (!oldList.Exists(i => i.characterName == character.characterName))
				{
					if (!oldList.Exists(i => i.characterId == character.characterId))
					{
						waitingList.Add(character);
					}
				}
			}

			oldList.Add(waitingList[0]);

			SaveCharacter(oldList);
			RenewalCharacter();
		}

		public void RecruitGun()
		{
			var oldList = LoadCharacter();
            
            var newList = new List<Character>()
            {
	            new Character(1, "Dietrich", CharacterClass.Ranger, 5, 5, 0, 0, 0, new List<Item>()),
	            new Character(2, "Deneve", CharacterClass.Ranger, 5, 5, 0, 0 ,0, new List<Item>()),
	            new Character(3, "Flora" , CharacterClass.Ranger, 5, 5, 0, 0, 0, new List<Item>()),
	            new Character(4, "Mirria", CharacterClass.Ranger, 5, 5, 0,0, 0, new List<Item>()),
	            new Character(5, "Clare" , CharacterClass.Ranger, 5, 5, 0,0, 0, new List<Item>()),
	            new Character(6, "Jean",  CharacterClass.Ranger, 5, 5, 0, 0, 0, new List<Item>()),
            };

            var waitingList  = new List<Character>(); 
            
            foreach (var character in newList)
            {
            	if (!oldList.Exists(i => i.characterName == character.characterName))
            	{
            		if (!oldList.Exists(i => i.characterId == character.characterId))
                    {
	                    waitingList.Add(character);
                    }
            	}
            }

            oldList.Add(waitingList[0]);

            SaveCharacter(oldList);
            RenewalCharacter();
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
			turnDateText.text = TurnToText(baseData.turnDate);
			SaveBaseData();
		}

		public void RenewalGold()
		{
			goldText.text = GoldToText(baseData.currentGold);
			SaveBaseData();
		}

		public void RenewalCharacter()
		{
			LoadCharacter();
		}

		public void HealHpFull()
		{
			foreach (var character in currentCharacterList)
			{
				character.currentHp = character.baseHp;
			}
			
			foreach (var hpBar in inventoryManager.characterSelect.hpBarList)
			{
				hpBar.RenewalHp();
			}
			
			SaveCharacter(currentCharacterList);
		}

		[ContextMenu("Add Exp Test")]
		public void AddExpTest()
		{
			AddAllExp(testExp);
		}

		[ContextMenu("Level Check")]
		public void LevelCheck()
		{
			StartCoroutine(LevelUp());
            
			SaveCharacter(currentCharacterList);
		}

		void SetLevelUpPanel(string characterName, int level)
		{
			levelUpName.text = characterName;
			levelUpLevel.text = level.ToString();
		}
		
		void AddAllExp(int exp)
		{
			if (currentCharacterList == null)
			{
				print("CurrentList is null");
				return;
			}
			
			var each = (int) exp / currentCharacterList.Count;
            
			foreach (var character in currentCharacterList)
			{
				if (character.currentHp <= 0) continue;
				character.exp = character.exp + each;
                
				print(character.characterName + " : " + character.exp);
			}
            
			SaveCharacter(currentCharacterList);
		}
		
		void AddEqualExp(int exp)
		{
			var each = (int) exp / currentCharacterList.Count;
            
			foreach (var character in currentCharacterList)
			{
				if (character.currentHp <= 0) continue;
				character.exp = character.exp + each;
                
				print(character.characterName + " exp : " + character.exp);
			}
            
			SaveCharacter(currentCharacterList);
		}

		IEnumerator LevelUp()
		{
			foreach (var character in currentCharacterList)
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
						levelUp.Invoke();
						SetLevelUpPanel(character.characterName, toLevel);
						levelUpWait = LevelUpWait.Wait;

						while (levelUpWait != LevelUpWait.Click)
						{
							yield return null;
						}

						if (levelUpWhat == LevelUpWhat.Deal) 
						{
							character.level = toLevel;
							character.baseDeal = character.baseDeal + 1;
							print("selected deal up (" + character.level + ", " + character.baseDeal + ")");
							
							
						}
						else if (levelUpWhat == LevelUpWhat.Hp)
						{
							character.level = toLevel;
							character.baseHp = character.baseHp + 1;
							character.currentHp = character.currentHp + 1;

							print("selected hp up (" +  character.level + ", " + character.baseHp + ", " + character.currentHp + ")");
							
						}
						else if (levelUpWhat == LevelUpWhat.Skill)
						{
							character.level = toLevel;
							// print("selected skill up");
							print("not yet!");
						}

						levelUpWait = LevelUpWait.Non;
						levelUpWhat = LevelUpWhat.Non;
						
						SaveCharacter(currentCharacterList);

						print(character.characterName + " level up to " + toLevel);
					}
				}
			}
		}

		public void SaveAll()
		{
			SaveCharacter(currentCharacterList);
			SaveBaseData();
		}
		
		public void ClickLevelUp()
		{
			levelUpWait = LevelUpWait.Click;
		}

		public void SelectDeal()
		{
			levelUpWhat = LevelUpWhat.Deal;
		}

		public void SelectHp()
		{
			levelUpWhat = LevelUpWhat.Hp;
		}

		public void SelectSkill()
		{
			levelUpWhat = LevelUpWhat.Skill;
		}
	}

}