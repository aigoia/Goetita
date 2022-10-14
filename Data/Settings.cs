using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.MainGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Data
{
    public class Settings : MonoBehaviour
    {
        public List<Transform> profileImageList;
        public List<Transform> itemImageList; 
        public BaseData baseData;
        public Vector3 initPosition = new Vector3(5, 0.5f, 15);
        public PlayerData playerData;
        public List<Item> ownedItems;
        public CurrentData currentData;
        public List<Item> baseItemList = new List<Item>();
        
        public TextMeshProUGUI mapName;
        public TextMeshProUGUI mapExp;
        public TextMeshProUGUI mapGold;
        public TextMeshProUGUI memoryExp;
        public List<GameObject> rewardGameObjectList;
        public List<TextMeshProUGUI> rewardTextList;
        public List<Image> rewardImageList;

        public int getItemPercent = 35;

        private void Awake()
        {
            if (playerData == null) playerData = FindObjectOfType<PlayerData>();
            if (currentData == null) currentData = FindObjectOfType<CurrentData>();
        }

        private void Start()
        {
            LoadBaseData();
            baseItemList = LoadBaseItems();
            LoadOwnedItems();
        }
        
        Image FindItemImage(string itemName, ItemType itemType)
        {
            var newItemName = itemName;
            if (itemName == null) return null;
            if (itemName == "Flak Jacket") newItemName = "Core";
            if (itemName == "Armor") newItemName = "Core";
            if (itemName == "Barrier") newItemName = "Core";
            if (itemName == "Tanker") newItemName = "Core";
            if (itemName == "Shield") newItemName = "Core";
            if (itemType == ItemType.Armor) newItemName = "Core";

            return itemImageList.Find(i => i.name == newItemName).GetComponent<Image>();
        }

        public void GiveItem()
        {
            foreach (var reward in rewardGameObjectList)
            {
                reward.SetActive(false);
            }
            
            // spin roulette
            var getItem = false;
            int itemNumber = Random.Range(0, 100);
            if (itemNumber <= getItemPercent)
            {
                getItem = true;
            }

            if (getItem == false)
            {
                print("No item");
                return;
            }

            print("ItemCount : " + baseData.itemCount);

            // item index
            while (ownedItems.Exists(i => i.itemId == baseData.itemCount))
            {
                baseData.itemCount = baseData.itemCount + 1;    
            }
            SaveBaseData();

            rewardGameObjectList[0].SetActive(true);
            var difficultItemList = new List<Item>();

            if (currentData == null)
            {
                foreach (var item in baseItemList)
                {
                    if (item.itemGrade == ItemGrade.Three)
                    {
                        difficultItemList.Add(item);
                    }
                }

                if (difficultItemList.Count == 0)
                {
                    print("There is no difficultItemList");
                    return;
                }
                
                var whatItem = Random.Range(0, difficultItemList.Count);
                var giveItem = difficultItemList[whatItem];
                giveItem.itemId = baseData.itemCount;
                print("We get '" + giveItem.itemName +"(" + giveItem.itemId + ")'");
                ownedItems.Add(giveItem);
                SaveOwnedItems();
                
                rewardGameObjectList[0].SetActive(true);
                rewardTextList[0].text = giveItem.itemName;
                rewardImageList[0].sprite = FindItemImage(giveItem.itemName, giveItem.itemType).sprite;
            }
            
            if (currentData.currentMission.difficultLevel == DifficultLevel.Easy)
            {
                foreach (var item in baseItemList)
                {
                    if (item.itemGrade == ItemGrade.One)
                    {
                        difficultItemList.Add(item);
                    }
                }

                var whatItem = Random.Range(0, difficultItemList.Count);
                var giveItem = difficultItemList[whatItem];
                giveItem.itemId = baseData.itemCount;
                print("We get '" + giveItem.itemName +"(" + giveItem.itemId + ")'");
                ownedItems.Add(giveItem);
                SaveOwnedItems();
                
                rewardGameObjectList[0].SetActive(true);
                rewardTextList[0].text = giveItem.itemName;
                rewardImageList[0].sprite = FindItemImage(giveItem.itemName, giveItem.itemType).sprite;
            }
            else if (currentData.currentMission.difficultLevel == DifficultLevel.Normal)
            {
                foreach (var item in baseItemList)
                {
                    if (item.itemGrade == ItemGrade.Two)
                    {
                        difficultItemList.Add(item);
                    }
                }
                
                var whatItem = Random.Range(0, difficultItemList.Count);
                var giveItem = difficultItemList[whatItem];
                giveItem.itemId = baseData.itemCount;
                print("We get '" + giveItem.itemName +"(" + giveItem.itemId + ")'");
                ownedItems.Add(giveItem);
                SaveOwnedItems();
                
                rewardGameObjectList[0].SetActive(true);
                rewardTextList[0].text = giveItem.itemName;
                rewardImageList[0].sprite = FindItemImage(giveItem.itemName, giveItem.itemType).sprite;
            }
            else if (currentData.currentMission.difficultLevel == DifficultLevel.Hard)
            {
                foreach (var item in baseItemList)
                {
                    if (item.itemGrade == ItemGrade.Three)
                    {
                        difficultItemList.Add(item);
                    }
                }
                
                var whatItem = Random.Range(0, difficultItemList.Count);
                var giveItem = difficultItemList[whatItem];
                giveItem.itemId = baseData.itemCount;
                print("We get '" + giveItem.itemName +"(" + giveItem.itemId + ")'");
                ownedItems.Add(giveItem);
                SaveOwnedItems();
                
                rewardGameObjectList[0].SetActive(true);
                rewardTextList[0].text = giveItem.itemName;
                rewardImageList[0].sprite = FindItemImage(giveItem.itemName, giveItem.itemType).sprite;
            }
        }

        public Character GetCharacter(int characterId)
        {
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/Character" + characterId + ".Json");
            string jsonData = File.ReadAllText(path);
            return JsonUtility.FromJson<Character>(jsonData);
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
        
        [ContextMenu("LoadBaseItems")]
        public List<Item> LoadBaseItems()
        {
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/BaseItemList.Json");
            string jsonData = File.ReadAllText(path);
            var newItems = JsonUtility.FromJson<ItemList>(jsonData);
            return newItems.Items.ToList();
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
        
        [ContextMenu("SetInitBase")]
        public void SetInitBase()
        {
            baseData.currentGold = baseData.initGold;
            baseData.turnDate = 0;
            SaveBaseData();
        }
        
        [ContextMenu("SetInitPosition")]
        public void SetInitPosition()
        {
            var whereData = new WhereData(initPosition, initPosition)
            {
                initPosition = initPosition, currentPosition = initPosition
            };

            string jsonData = JsonUtility.ToJson(whereData, true);                                            
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/WhereData.Json");             
            File.WriteAllText(path, jsonData);
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

        [ContextMenu("SetInitOwnedItems")]
        public void SetInitOwnedItems()
        {
            ownedItems = new List<Item>();
            SaveOwnedItems();
        }
        
        public Sprite SetProfileImage(Player player, string what)
        {
            var sprite = profileImageList.Find(image => image.name == player.characterName).transform
                .Find(what).GetComponent<Image>().sprite;
            print(sprite.name + " : " + what);
            return sprite;
        }
    }
}
