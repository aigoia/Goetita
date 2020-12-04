using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Data
{
    public class Settings : MonoBehaviour
    {
        public List<GameObject> imageList;
    
        [ContextMenu("Make Init Character")]
        public void MakeInitCharacterList()
        {
            var newList = new List<Character>()
            {
                // new Character(1, "Dietrich", imageList[0], CharacterClass.SwordMaster, 8, 8, 0, 0),
                new Character(2, "Deneve", CharacterClass.Ranger, 4, 4, 0, 0 ,0),
                // new Character(3, "Flora" , imageList[2], CharacterClass.Ranger, 6, 6, 0, 0),
                new Character(4, "Mirria", CharacterClass.Ranger, 4, 4, 0,0, 0),
                new Character(5, "Clare" , CharacterClass.Claymore, 8, 8, 0,0, 0),
                // new Character(6, "Jean", imageList[5], CharacterClass.SwordMaster, 8, 8, 0, 0),
            };
            

            ES3.Save<List<Character>>("Characters", newList, "Game");

            // test code
            // var saveCheck = ES3.Load<List<Character>>("Characters", "Game");
            // foreach (var character in saveCheck)
            // {
            // 	print(character);
            // }
        }
    
        [ContextMenu("Make Init All Sword")]
        public void MakeInitAllSword()
        {
            var newList = new List<Character>()
            {
                new Character(1, "Dietrich", CharacterClass.Claymore, 6, 4, 0, 0,0),
                new Character(2, "Deneve", CharacterClass.Claymore, 6, 4, 0, 0, 0),
                new Character(3, "Flora" , CharacterClass.Claymore, 6, 4, 0, 0, 0),
                new Character(4, "Mirria", CharacterClass.Claymore, 6, 4, 0, 0, 0),
                new Character(5, "Clare" , CharacterClass.Claymore, 6, 4, 0, 0, 0),
                new Character(6, "Jean", CharacterClass.Claymore, 6, 4, 0, 0, 0),

            };
			
            ES3.Save<List<Character>>("Characters", newList, "Game");

            // test code
            var saveCheck = ES3.Load<List<Character>>("Characters", "Game");
            foreach (var character in saveCheck)
            {
                // print(character.CharacterId);
            }
        }
    
        [ContextMenu("Make Init All Range")]
        public void MakeInitAllRange()
        {
            var newList = new List<Character>()
            {
                new Character(1, "Dietrich", CharacterClass.Ranger, 4, 4, 0, 0, 0),
                new Character(2, "Deneve", CharacterClass.Ranger, 4, 4, 0, 0, 0),
                new Character(3, "Flora" , CharacterClass.Ranger, 4, 4, 0, 0, 0),
                new Character(4, "Mirria", CharacterClass.Ranger, 4, 4, 0, 0, 0),
                new Character(5, "Clare" , CharacterClass.Ranger, 4, 4, 0, 0, 0),
                new Character(6, "Jean", CharacterClass.Ranger, 4, 4, 0, 0, 0),

            };
			
            ES3.Save<List<Character>>("Characters", newList, "Game");

            // test code
            var saveCheck = ES3.Load<List<Character>>("Characters", "Game");
            foreach (var character in saveCheck)
            {
                // print(character.CharacterId);
            }
        }
        
        [ContextMenu("Init Gold")]
		public void InitGold()
		{
			var gold = ES3.Load<int>("InitGold", "Game");
			ES3.Save<int>("Gold", gold, "Game");
		}

		[ContextMenu("Init Turn")]
		public void InitTurn()
		{
			var turnDate = 0;
			ES3.Save<int>("TurnDate", turnDate, "Game");
		}
        
        public void ResetAll()
        {
            MakeInitCharacterList();
            InitGold();
            InitTurn();
        }
    }
}
