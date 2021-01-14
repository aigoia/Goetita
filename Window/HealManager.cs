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
         
            dataManager.currentCharacterList = dataManager.LoadCharacter();
            		
            
            _selectedCharacterList = null;
            _billGold = 0;

            for (int i = 0; i < dataManager.currentCharacterList.Count; i++)
            {
                if (i <= buttonList.Count)
                {
                    buttonList[i].SetActive(true);
                    buttonList[i].GetComponent<HealButton>().characterId = dataManager.currentCharacterList[i].characterId;
                    hpBarList[i].characterId = dataManager.currentCharacterList[i].characterId;
                    activeButtonList.Add(buttonList[i].GetComponent<HealButton>());
                    activeButtonList[i].character = dataManager.currentCharacterList[i];
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
                selectedCharacters.Add(dataManager.currentCharacterList.Find(i => i.characterId == button.characterId));
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
            if (_selectedCharacterList == null) return;
            
            // pay gold
            if (dataManager.baseData.currentGold < _billGold) return;
            dataManager.baseData.currentGold = dataManager.baseData.currentGold - _billGold;
            dataManager.RenewalGold();
            
            // heal character
            foreach (var currentCharacter in dataManager.currentCharacterList)
            {
                if (_selectedCharacterList.Exists(i => i.characterId == currentCharacter.characterId))
                {
                    currentCharacter.currentHp = currentCharacter.baseHp;
                }
            }
            
            dataManager.SaveCharacter(dataManager.currentCharacterList);
            
            dataManager.RenewalCharacter();
            FillHp();
            
            _selectedCharacterList = null;
            _billGold = 0;
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
        
        public void SetInformation(int characterId)
        {
            var selectedCharacter = dataManager.currentCharacterList.Find(i => i.characterId == characterId);
            if (selectedCharacter == null) return;

            characterName.text = selectedCharacter.characterName;
            level.text = selectedCharacter.level.ToString();
        
            if (selectedCharacter.type == CharacterClass.Ranger)
            {
                characterClass.text = "Ranger";
            }
            if (selectedCharacter.type == CharacterClass.Claymore)
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
