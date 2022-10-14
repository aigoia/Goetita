using System.Collections.Generic;
using Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Window
{
    [SerializeField]
    public class HealManager : MonoBehaviour
    {
        public DataManager dataManager;
        public List<GameObject> activeList = new List<GameObject>();
        public List<GameObject> buttonList = new List<GameObject>();
        public List<HealButton> activeButtonList = new List<HealButton>();
        public List<HpBar> hpBarList = new List<HpBar>();
        
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI characterClass;
        public TextMeshProUGUI level;

        public TextMeshProUGUI bill;

        private List<Character> _selectedCharacterList;
        private int _billGold = 0;

        public int baseCost = 40;
        public int plusCost = 0;
        public int deathDoor = 2;
        
        public Button defaultButton;
        
        private void Awake()
        {
            if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
        }
        
        private void Start()
        {
            // dataManager.RenewalCharacter();
            _selectedCharacterList = null;
            _billGold = 0;

            for (int i = 0; i < dataManager.CurrentCharacterList.Count; i++)
            {
                if (i <= buttonList.Count)
                {
                    buttonList[i].SetActive(true);
                    string loadCharacterName = dataManager.CurrentCharacterList[i].characterName;
                    buttonList[i].GetComponent<HealButton>().character.characterName = dataManager.CurrentCharacterList[i].characterName;
                    hpBarList[i].characterName = dataManager.CurrentCharacterList[i].characterName;
                    activeButtonList.Add(buttonList[i].GetComponent<HealButton>());
                    activeButtonList[i].character = dataManager.CurrentCharacterList[i];
                    
                    var profileButtonImage = dataManager.profileImageList.Find(image => image.name == loadCharacterName).transform
                        .Find("100x100").GetComponent<Image>().sprite;

                    buttonList[i].transform.Find("ProfileImage").GetComponent<Image>().sprite =
                        profileButtonImage;
                }
            }
            
            FillHp();
        }

        public void AllBill()
        {
            _selectedCharacterList = null;
            _billGold = 0;
            
            // make data
            var selectedButtons = new List<HealButton>();
            
            foreach (var healButton in activeButtonList)
            {
                if (healButton.isSelected)
                {
                    selectedButtons.Add(healButton);
                }
            }

            var selectedCharacters = new List<Character>();
            _selectedCharacterList = selectedCharacters;

            foreach (var button in selectedButtons)
            {
                selectedCharacters.Add(dataManager.CurrentCharacterList.Find(i => i.characterName == button.character.characterName));
            }
            
            // make bill
            int allBill = 0;

            foreach (var character in _selectedCharacterList)
            { 
                if (character.baseHp <= character.currentHp) continue;
                
                var newBill = baseCost + plusCost * character.level;

                if (character.currentHp <= 0) newBill = newBill * deathDoor;

                allBill = allBill + newBill;

            }
            
            // change icon
            bill.text = allBill.ToString();
            _billGold = allBill;
        }

        public void Pay()
        {
            var newCharacters = dataManager.CurrentCharacterList;
            
            if (_selectedCharacterList == null) return;
            
            // pay gold
            if (dataManager.baseData.currentGold < _billGold) return;
            dataManager.baseData.currentGold = dataManager.baseData.currentGold - _billGold;
            dataManager.RenewalGold();
            
            // heal character
            foreach (var currentCharacter in newCharacters)
            {
                if (_selectedCharacterList.Exists(i => i.characterName == currentCharacter.characterName))
                {
                    currentCharacter.currentHp = currentCharacter.baseHp;
                }
            }
            
            dataManager.SaveCharacter(newCharacters);
            
            // dataManager.RenewalCharacter();
            FillHp();
            
            _selectedCharacterList = null;
            _billGold = 0;
            bill.text = 0.ToString();
            ActiveOff();
        }

        void AllOff()
        {
            activeList.ForEach(i => i.SetActive(false));
        }

        public void FillHp()
        {
            foreach (var hpBar in hpBarList)
            {
                hpBar.MakeHp();
            }
        }
        
        public void SetInformation(string characterFindName)
        {
            var selectedCharacter = dataManager.CurrentCharacterList.Find(i => i.characterName == characterFindName);
            if (selectedCharacter == null) return;

            characterName.text = selectedCharacter.characterName;
            level.text = selectedCharacter.level.ToString();
        
            if (selectedCharacter.classType == CharacterClass.Ranger)
            {
                characterClass.text = "Ranger";
            }
            if (selectedCharacter.classType == CharacterClass.Claymore)
            {
                characterClass.text = "Claymore";
            }
        }
        
        public void DefaultOn()
        {
            if (defaultButton == null) return;
			
            defaultButton.animator.Play("Hover to Pressed");
        }

        public void ReSetting()
        {
            activeList.ForEach(i => i.SetActive(false));
        }

        public void ActiveOff()
        {
            foreach (var active in activeList)
            {
                active.SetActive(false);
            }
        }
    }
}
