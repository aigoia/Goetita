using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.MainGame;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Data
{
    public class Settings : MonoBehaviour
    {
        public List<GameObject> imageList;
        public BaseData baseData;
        public Vector3 initPosition = new Vector3(5, 0.5f, 15);
        public PlayerData playerData;
        public List<Item> ownedItems;

        private void Awake()
        {
            if (playerData == null) playerData = FindObjectOfType<PlayerData>();
        }

        private void Start()
        {
            LoadBaseData();
            LoadOwnedItems();
        }

        public Character GetCharacter(int characterId)
        {
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/Character" + characterId + ".Json");
            string jsonData = File.ReadAllText(path);
            return JsonUtility.FromJson<Character>(jsonData);
        }
        

 		public CharacterList GetCharacter()
 		{
 			string path = Path.Combine(Application.dataPath + "/StreamingAssets/CharacterList.Json");
 			string jsonData = File.ReadAllText(path);
 			return JsonUtility.FromJson<CharacterList>(jsonData);
 		}
 
 		public void SetCharacter(List<Character> characters)
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
                new Character(1, "Dietrich", CharacterClass.Non, 0, 0, 0, 0, 0, new List<Item>()),
                new Character(2, "Deneve", CharacterClass.Non, 0, 0, 0, 0, 0, new List<Item>()),
                new Character(3, "Flora", CharacterClass.Non, 0, 0, 0, 0, 0, new List<Item>()),
                new Character(4, "Mirria", CharacterClass.Non, 0, 0, 0, 0, 0, new List<Item>()),
                new Character(5, "Clare", CharacterClass.Non, 0, 0, 0, 0, 0, new List<Item>()),
                new Character(6, "Jean", CharacterClass.Non, 0, 0, 0, 0, 0, new List<Item>()),
            };

            playerData.currentCharacterList = newList;

            SaveCharacter(playerData.currentCharacterList);
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

            playerData.currentCharacterList = newList;
            SaveCharacter(playerData.currentCharacterList);
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
        
        public void ResetAll()
        {
            // ES3.Save<Transform>("WhereCityArea", null, "Game");
            SetInitCharacter();
            SetInitBase();
            SetInitPosition();
            SetInitOwnedItems();
        }
    }
}
