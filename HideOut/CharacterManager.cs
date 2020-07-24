using System.Collections.Generic;
using UnityEngine;

namespace Game.HideOut
{
    public class CharacterManager : MonoBehaviour
    {
        public List<Character> activeCharacters = new List<Character>();
        public List<CharacterButton> allCharacterButtons = new List<CharacterButton>();
        public List<CharacterButton> activeCharacterButtons = new List<CharacterButton>();
        public float censorExtents = 1f;

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

        public CityArea WhereIam()
        {
            var origins = Physics.BoxCastAll(transform.position, Vector3.one * censorExtents, Vector3.forward, Quaternion.identity, 0, LayerMask.GetMask("CityArea"));
            if (origins.Length == 0) return null;
        
            var originArea = origins[0].transform.GetComponent<CityArea>();
            if (originArea == null) return null;

            return originArea;
        }
    }
    
    
}
