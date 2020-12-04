using System.Collections.Generic;
using Game.Data;
using UnityEngine;

namespace Game.Window
{
    public class CharacterSelect : MonoBehaviour {

        public List<GameObject> activeList = new List<GameObject>();
        public List<GameObject> buttonList = new List<GameObject>();
        public List<CharacterButton> activeButtonList = new List<CharacterButton>();
        public List<HpBar> hpBarList = new List<HpBar>();
        public List<Character> characters;

        public DataManager dataManager;

        private void Awake()
        {
            if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
        }

        private void Start()
        {
            for (int i = 0; i < dataManager.currentCharacterList.Count; i++)
            {
                if (i <= buttonList.Count)
                {
                    buttonList[i].SetActive(true);
                    buttonList[i].GetComponent<CharacterButton>().characterId = dataManager.currentCharacterList[i].CharacterId;
                    activeButtonList.Add(buttonList[i].GetComponent<CharacterButton>());
                    activeButtonList[i].character = dataManager.currentCharacterList[i];
                }
            }
        }

        public void FillHp()
        {
            foreach (var hpBar in hpBarList)
            {
                hpBar.MakeHp();
            }
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
