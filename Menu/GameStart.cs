using System;
using System.Collections.Generic;
using System.IO;
using Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Menu
{
    public class GameStart : MonoBehaviour
    {
        // public List<GameObject> profileImages;
        public DataManager dataManager;

        public int initGold = 3000;
        private int _newGold = 0;
        public TextMeshProUGUI goldText;
        public int costSword = 700;
        public int costGun = 800;
        public string swordText = "Claymore";
        public string gunText = "Gun";
        public GameObject minObject;

        public string nameOne = "Tina";
        public string nameTwo = "Den";
        public string nameThree = "Flora";
        
        public List<GameObject> imageListOne;
        public List<GameObject> imageListTwo;
        public List<GameObject> imageListThree;

        public Toggle toggleOne;
        public Toggle toggleTwo;
        public Toggle toggleThree;

        public TMP_Dropdown dropdownOne;
        public TMP_Dropdown dropdownTwo;
        public TMP_Dropdown dropdownThree;

        public List<TMP_Dropdown.OptionData> options;
        public DefaultData defaultData;

        public GameObject characterOne;
        public GameObject characterTwo;
        public GameObject characterThree;

        public TextMeshProUGUI textOne;
        public TextMeshProUGUI textTwo;
        public TextMeshProUGUI textThree;

        public String deckPlus = "Deck +1";
        public String baseDealPlus = "Deal +1";
        public String hpPlus = "HP +1";
        public String armorPlus = "Armor +1";
        public String randomDealPlus = "Random deal + 2"; 
        
        private void Awake()
        {
            if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
        }

        private void Start()
        {
            LoadDefaultData();   
            CalculateCost();
            DefaultStart();
        }
        
        [ContextMenu("LoadDefaultData")]
        public void LoadDefaultData()
        {
        	string path = Path.Combine(Application.dataPath + "/StreamingAssets/DefaultData.Json");
        	string jsonData = File.ReadAllText(path);
        	defaultData = JsonUtility.FromJson<DefaultData>(jsonData);
        }

        [ContextMenu("SaveDefaultData")]
        public void SaveDefaultData()
        {
            string jsonData = JsonUtility.ToJson(defaultData, true);
            print(jsonData);
        	string path = Path.Combine(Application.dataPath + "/StreamingAssets/DefaultData.Json");
        	File.WriteAllText(path, jsonData);
        }

        void DefaultStart()
        {
            DefaultOneOn();
            DefaultTwoOn();
            DefaultThreeOn();
        }

        void CharacterPlus(GameObject character, TextMeshProUGUI text)
        {
            if (GetName(character) == "Tina")
            {
                text.text = deckPlus;
            }
            else if (GetName(character) == "Den")
            {
                text.text = baseDealPlus;
            }
            else if (GetName(character) == "Flora")
            {
                text.text = hpPlus;
            }
            else if (GetName(character) == "Mia")
            {
                text.text = armorPlus;
            }
            else if (GetName(character) == "Clare")
            {
                text.text = randomDealPlus;
            }
        }

        public void NextProfileOne()
        {
            var currentIndex = imageListOne.FindIndex(i => i.name == GetName(characterOne));
            var anotherIndex = imageListTwo.FindIndex(i => i.name == GetName(characterTwo));
            var otherIndex = imageListThree.FindIndex(i => i.name == GetName(characterThree));
            
            print("One " + currentIndex + " " + GetName(characterOne));
            print("Two " + anotherIndex + " " + GetName(characterTwo));
            print("Three " + otherIndex + " " + GetName(characterThree));
            //
            foreach (var button in imageListOne)
            {
                button.SetActive(false);
            }
            
            var newIndex = currentIndex + 1;
            newIndex = newIndex % imageListOne.Count;
            
            if (newIndex == anotherIndex)
            {
                newIndex = newIndex + 1;
                // print(newIndex);
                
                if (newIndex == otherIndex)
                {
                    newIndex = newIndex + 1;
                    // print(newIndex);
                }
            }
            if (newIndex == otherIndex)
            {
                newIndex = newIndex + 1;
                // print(newIndex);
                
                if (newIndex == anotherIndex)
                {
                    newIndex = newIndex + 1;
                    // print(newIndex);
                }
            }
            newIndex = newIndex % imageListOne.Count;

            if (newIndex == 0)
            {
                if (currentIndex == 0 || otherIndex == 0 || anotherIndex == 0)
                {
                    newIndex = 1;
                }
            }
            imageListOne[newIndex].SetActive(toggleOne.isOn);
            
            CharacterPlus(characterOne, textOne);
        }

        public void NextProfileTwo()
        {
            var currentIndex = imageListTwo.FindIndex(i => i.name == GetName(characterTwo));
            var anotherIndex = imageListThree.FindIndex(i => i.name == GetName(characterOne));
            var otherIndex = imageListOne.FindIndex(i => i.name == GetName(characterThree));
            
            // print("One " + currentIndex + " " + GetName(characterOne));
            // print("Two " + anotherIndex + " " + GetName(characterTwo));
            // print("Three " + otherIndex + " " + GetName(characterThree));
            //
            foreach (var button in imageListTwo)
            {
                button.SetActive(false);
            }
            
            var newIndex = currentIndex + 1;
            newIndex = newIndex % imageListTwo.Count;
            
            if (newIndex == anotherIndex)
            {
                newIndex = newIndex + 1;
                // print(newIndex);
                
                if (newIndex == otherIndex)
                {
                    newIndex = newIndex + 1;
                    // print(newIndex);
                }
            }
            if (newIndex == otherIndex)
            {
                newIndex = newIndex + 1;
                // print(newIndex);
                
                if (newIndex == anotherIndex)
                {
                    newIndex = newIndex + 1;
                    // print(newIndex);
                }
            }
            newIndex = newIndex % imageListTwo.Count;

            if (newIndex == 0)
            {
                if (currentIndex == 0 || otherIndex == 0 || anotherIndex == 0)
                {
                    newIndex = 1;
                }
            }

            imageListTwo[newIndex].SetActive(toggleTwo.isOn);
            CharacterPlus(characterTwo, textTwo);
        }

        public void NextProfileThree()
        {
            var currentIndex = imageListThree.FindIndex(i => i.name == GetName(characterThree));
            var anotherIndex = imageListOne.FindIndex(i => i.name == GetName(characterTwo));
            var otherIndex = imageListTwo.FindIndex(i => i.name == GetName(characterOne));
            
            // print("One " + currentIndex + " " + GetName(characterOne));
            // print("Two " + anotherIndex + " " + GetName(characterTwo));
            // print("Three " + otherIndex + " " + GetName(characterThree));
            //
            foreach (var button in imageListThree)
            {
                button.SetActive(false);
            }

            var newIndex = currentIndex + 1;
            newIndex = newIndex % imageListThree.Count;
            
            if (newIndex == anotherIndex)
            {
                newIndex = newIndex + 1;
                // print(newIndex);
                
                if (newIndex == otherIndex)
                {
                    newIndex = newIndex + 1;
                    // print(newIndex);
                }
            }
            if (newIndex == otherIndex)
            {
                newIndex = newIndex + 1;
                // print(newIndex);
                
                if (newIndex == anotherIndex)
                {
                    newIndex = newIndex + 1;
                    // print(newIndex);
                }
            }
            newIndex = newIndex % imageListThree.Count;
            
            if (newIndex == 0)
            {
                if (currentIndex == 0 || otherIndex == 0 || anotherIndex == 0)
                {
                    newIndex = 1;
                }
            }
            imageListThree[newIndex].SetActive(toggleThree.isOn);
            CharacterPlus(characterThree, textThree);
        }

        void DefaultOneOn()
        {
            if (defaultData.defaultStart)
            {
                foreach (var button in imageListOne)
                {
                    button.GetComponent<Button>().enabled = false;
                    button.SetActive(false);
                }
                var imageOne = imageListOne.Find(i => i.name == nameOne);
                imageOne.SetActive(toggleOne.isOn);
                OnOffOne();
            }
            else
            {
                foreach (var button in imageListOne)
                {
                    button.SetActive(false);
                }
                
                var imageOne = imageListOne.Find(i => i.name == nameOne);
                imageOne.SetActive(toggleOne.isOn);
                NextProfileOne();
                OnOffOne();
            }
            
            CharacterPlus(characterOne, textOne);
        }

        void DefaultTwoOn()
        {
            if (defaultData.defaultStart)
            {
                foreach (var button in imageListTwo)
                {
                    button.GetComponent<Button>().enabled = false;
                    button.SetActive(false);
                }
                        
                var imageTwo = imageListTwo.Find(i => i.name == nameTwo);
                imageTwo.SetActive(toggleTwo.isOn);
                OnOffTwo();
            }
            else
            {
                foreach (var button in imageListTwo)
                {
                    button.SetActive(false);
                }
                        
                var imageTwo = imageListTwo.Find(i => i.name == nameTwo);
                imageTwo.SetActive(toggleTwo.isOn);
                NextProfileTwo();
                OnOffTwo();
            }
            
            CharacterPlus(characterTwo, textTwo);
        }

        void DefaultThreeOn()
        {
            if (defaultData.defaultStart)
            {
                foreach (var button in imageListThree)
                { 
                    button.GetComponent<Button>().enabled = false;
                    button.SetActive(false);
                }
                            
                var imageThree = imageListThree.Find(i => i.name == nameThree);
                imageThree.SetActive(toggleThree.isOn);
                OnOffThree();
            }
            else
            {
                foreach (var button in imageListThree)
                {
                    button.SetActive(false);
                }
                                            
                var imageThree = imageListThree.Find(i => i.name == nameThree);
                imageThree.SetActive(toggleThree.isOn);
                NextProfileThree();
                OnOffThree();
            }
            
            CharacterPlus(characterThree, textThree);
        }

        string GetName(GameObject newObject)
        {
            var newString = "";
            foreach (var button in newObject.transform.GetComponentsInChildren<Transform>())
            {
                if (button.gameObject.activeSelf == true)
                {
                    newString = button.name;
                }
            }

            return newString;
        }

        public void GotoWindow()
        {
            dataManager.InitAll();
            // var boolList = new List<bool> {toggleOne.isOn, toggleTwo.isOn, toggleThree.isOn};
            
            if (toggleOne.isOn == false && toggleTwo.isOn == false && toggleThree.isOn == false)
            {
                minObject.SetActive(true);
                return;
            }
            
            var newList = new List<Character>();
            
            string one = GetName(characterOne);
            string two = GetName(characterTwo);
            string three = GetName(characterThree);

            var swordList = dataManager.SwordCharacters();
            var gunList = dataManager.GunCharacters();

            if (toggleOne.isOn)
            {
                if (dropdownOne.value == 0)
                {
                    // newList.Add(new Character( one, CharacterClass.Claymore, 7, 7, 2, 1, 1, 0, 0, new List<Item>()));
                    newList.Add(swordList.Find(i => i.characterName == one));
                }
                else if (dropdownOne.value == 1)
                {
                    // newList.Add(new Character( one, CharacterClass.Ranger, 5, 5, 2, 2, 0, 0, 0, new List<Item>()));
                    newList.Add(gunList.Find(i => i.characterName == one));
                }
            }
            if (toggleTwo.isOn)
            {
                if (dropdownTwo.value == 0)
                {
                    // newList.Add(new Character( two, CharacterClass.Claymore, 7, 7, 2, 1, 1, 0, 0, new List<Item>()));
                    newList.Add(swordList.Find(i => i.characterName == two));
                }
                else if (dropdownTwo.value == 1)
                {
                    // newList.Add(new Character( two, CharacterClass.Ranger, 5, 5, 2, 2, 0, 0, 0, new List<Item>()));
                    newList.Add(gunList.Find(i => i.characterName == two));
                }
            }
            if (toggleThree.isOn)
            {
                if (dropdownThree.value == 0)
                {
                    // newList.Add(new Character( three, CharacterClass.Claymore, 7, 7, 2, 1, 1, 0, 0, new List<Item>()));
                    newList.Add(swordList.Find(i => i.characterName == three));
                }
                else if (dropdownThree.value == 1)
                {
                    // newList.Add(new Character( three, CharacterClass.Ranger, 5, 5, 2, 2, 0, 0, 0, new List<Item>()));
                    newList.Add(gunList.Find(i => i.characterName == three));
                }
            }
            

            dataManager.SetNewCharacter(newList);
            dataManager.SetInitBase(_newGold);

            SceneManager.LoadScene("Game/Window");
        }

        public void CalculateCost()
        {
            var newCost = initGold;
            
            if (toggleOne.isOn)
            {
                if (dropdownOne.value == 0)
                {
                    newCost = newCost - costSword;
                }
                else if (dropdownOne.value == 1)
                {
                    newCost = newCost - costGun;
                }
            }
            if (toggleTwo.isOn)
            {
                if (dropdownTwo.value == 0)
                {
                    newCost = newCost - costSword;
                }
                else if (dropdownTwo.value == 1)
                {
                    newCost = newCost - costGun;
                }
            }
            if (toggleThree.isOn)
            {
                if (dropdownThree.value == 0)
                {
                    // print(costSword);
                    newCost = newCost - costSword;
                }
                else if (dropdownThree.value == 1)
                {
                    // print(costGun);
                    newCost = newCost - costGun;
                }
            }

            _newGold = newCost;
            goldText.text = newCost.ToString();
        }

        void OnOffOne()
        {
            if (toggleOne.isOn)
            {
                dropdownOne.AddOptions(options);
            }
            else
            {
                dropdownOne.ClearOptions();
            }
        }
        void OnOffTwo()
        {
            if (toggleTwo.isOn)
            {
                dropdownTwo.AddOptions(options);   
            }
            else
            {
                dropdownTwo.ClearOptions();
            }
        }
        void OnOffThree()
        {
            if (toggleThree.isOn)
            {
                dropdownThree.AddOptions(options);
            }
            else
            {
                dropdownThree.ClearOptions();
            }

        }
    }

    [System.Serializable]
    public class DefaultData
    {
        public bool defaultStart = true;
    }
}
