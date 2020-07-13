using System.Collections.Generic;
using UnityEngine;

namespace Game.HideOut
{
    public class CharacterManager : MonoBehaviour
    {
        public List<Character> activeCharacters = new List<Character>();
        public List<CharacterButton> allCharacterButtons = new List<CharacterButton>();
        public List<CharacterButton> activeCharacterButtons = new List<CharacterButton>();
        

        private void Start()
        {
            if (ES3.KeyExists("defaultCharacters"))
            {
                activeCharacters = ES3.Load<List<Character>>("defaultCharacters");
            }
            

            for (int i = 0; i < activeCharacters.Count; i++)
            {
                allCharacterButtons[i].gameObject.SetActive(true);
                allCharacterButtons[i].id = activeCharacters[i].Id;
                activeCharacterButtons.Add(allCharacterButtons[i]);
            }
        
        }

        [ContextMenu("Setting Default")]
        public void SettingDefault()
        {
            var characters = new List<Character>()
            {
                new Character()
                {
                    Id = 0, Level = 0,
                },
                new Character()
                {
                    Id = 1, Level = 0,
                }
            };
        
            ES3.Save<List<Character>>("defaultCharacters", characters);
        
        }
    }
    
    
}
