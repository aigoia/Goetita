using System.Collections;
using System.Collections.Generic;
using Game.MainGame;
using Game.Window;
// using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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

	public class DataManager : MonoBehaviour
	{
		public int gold = 0;
		public TextMeshProUGUI goldText;
		public int turnDate = 0;
		public TextMeshProUGUI turnDateText;
		
		public List<GameObject> imageList;
		public List<Character> currentCharacterList = new List<Character>();
		public List<int> neededLevel = new List<int>();
		public int testExp = 8;
		public int setInitGold = 200;

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
			currentCharacterList = ES3.Load<List<Character>>("Characters", "Game");

			foreach (var character in currentCharacterList)
			{
				print(character + " (" + character.CharacterId + ") : HP " + character.CurrentHp);
			}
		}
		
		public void Start()
		{
			turnDate = ES3.Load<int>("TurnDate", "Game");
			RenewalTurnData();

			gold = ES3.Load<int>("Gold", "Game");
			RenewalGold();
			
		}

		[ContextMenu("Set InitGold")]
		public void SetInitGold()
		{
			ES3.Save<int>("InitGold", setInitGold ,"Game");
		}
		
		[ContextMenu("Init All")]
		public void InitAll()
		{
			InitGold();
			InitTurn();
			MakeInitCharacterList();
		}

		[ContextMenu("Reset All")]
		public void ResetAll()
		{
			MakeInitCharacterList();
			
			var initGold = ES3.Load<int>("InitGold", "Game");
			ES3.Save<int>("Gold", initGold, "Game");
			var resetDate = 0;
			ES3.Save<int>("TurnDate", resetDate, "Game");
		}

		[ContextMenu("Init Gold")]
		public void InitGold()
		{
			var initGold = ES3.Load<int>("InitGold", "Game");
			gold = initGold;
			ES3.Save<int>("Gold", gold, "Game");
		}

		[ContextMenu("Init Turn")]
		public void InitTurn()
		{
			turnDate = 0;
			ES3.Save<int>("TurnDate", turnDate, "Game");
		}

		[ContextMenu("Make Init Character")]
		public void MakeInitCharacterList()
		{
			var newList = new List<Character>()
			{
				new Character(1, "Dietrich", CharacterClass.Claymore, 6, 6, 0 , 0, 0),
				new Character(2, "Deneve", CharacterClass.Ranger, 4, 4, 0 , 0, 0),
				new Character(3, "Flora" , CharacterClass.Ranger, 4, 4, 0, 0, 0),
				// new Character(4, "Mirria", imageList[3], CharacterClass.SwordMaster, 6, 4, 0 , 0, 0),
				// new Character(5, "Clare" , imageList[4], CharacterClass.SwordMaster, 6, 4, 0 , 0, 0),
				// new Character(6, "Jean", imageList[5], CharacterClass.ranger, 6, 4, 0, 0, 0),
			};
			
			ES3.Save<List<Character>>("Characters", newList, "Game");

			// test code
			// var saveCheck = ES3.Load<List<Character>>("Characters", "Game");
			// foreach (var character in saveCheck)
			// {
			// 	// print(character.CharacterId);
			// }
		}

		public void RecruitSword()
		{
			var oldList = ES3.Load<List<Character>>("Characters","Game");

			var newList = new List<Character>()
            {
	            new Character(1, "Dietrich", CharacterClass.Claymore, 6, 4, 0, 0, 0),
	            new Character(2, "Deneve", CharacterClass.Claymore, 6, 4, 0, 0, 0),
	            new Character(3, "Flora" , CharacterClass.Claymore, 6, 4, 0, 0, 0),
	            new Character(4, "Mirria", CharacterClass.Claymore, 6, 4, 0, 0, 0),
	            new Character(5, "Clare" ,CharacterClass.Claymore, 6, 4, 0, 0, 0),
	            new Character(6, "Jean", CharacterClass.Claymore, 6, 4, 0, 0, 0),
            };

			var waitingList  = new List<Character>(); 
			
			foreach (var character in newList)
			{
				if (!oldList.Exists(i => i.CharacterName == character.CharacterName))
				{
					if (!oldList.Exists(i => i.CharacterId == character.CharacterId))
					{
						waitingList.Add(character);
					}
				}
			}

			if (waitingList == null)
			{
				print("The member list is full");
				return;
			}

			oldList.Add(waitingList[0]);

			ES3.Save<List<Character>>("Characters", oldList, "Game");
			RenewalCharacter();
		}

		public void RecruitGun()
		{
			var oldList = ES3.Load<List<Character>>("Characters","Game");
            
            var newList = new List<Character>()
            {
	            new Character(1, "Dietrich", CharacterClass.Ranger, 4, 4, 0, 0, 0),
	            new Character(2, "Deneve", CharacterClass.Ranger, 4, 4, 0, 0, 0),
	            new Character(3, "Flora" , CharacterClass.Ranger, 4, 4, 0, 0, 0),
	            new Character(4, "Mirria", CharacterClass.Ranger, 4, 4, 0, 0, 0),
	            new Character(5, "Clare" , CharacterClass.Ranger, 4, 4, 0, 0, 0),
	            new Character(6, "Jean",CharacterClass.Ranger, 4, 4, 0, 0, 0),
            };

            var waitingList  = new List<Character>(); 
            
            foreach (var character in newList)
            {
            	if (!oldList.Exists(i => i.CharacterName == character.CharacterName))
            	{
            		if (!oldList.Exists(i => i.CharacterId == character.CharacterId))
                    {
	                    waitingList.Add(character);
                    }
            	}
            }

            if (waitingList == null)
            {
            	print("The member list is full");
            	return;
            }

            oldList.Add(waitingList[0]);

            ES3.Save<List<Character>>("Characters", oldList, "Game");
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
			ES3.Save<int>("TurnDate", turnDate, "Game");
			turnDateText.text = TurnToText(turnDate);
		}

		public void RenewalGold()
		{
			ES3.Save<int>("Gold", gold, "Game");
			goldText.text = GoldToText(gold);
		}

		public void RenewalCharacter()
		{
			currentCharacterList = ES3.Load<List<Character>>("Characters", "Game");
		}

		public void HealHpFull()
		{
			foreach (var character in currentCharacterList)
			{
				character.CurrentHp = character.BaseHp;
			}
			
			foreach (var hpBar in inventoryManager.characterSelect.hpBarList)
			{
				hpBar.RenewalHp();
			}
			
			ES3.Save<List<Character>>("Characters", currentCharacterList, "Game");
		}
		
		[ContextMenu("Level Stage")]
        public void LevelStage()
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
        	
        	ES3.Save<List<int>>("LevelStage", expToLevelList, "Game");
        	
        	//test code

        	var test = ES3.Load<List<int>>("LevelStage", "Game");
        	GameUtility.PrintList(test);
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
            
			ES3.Save<List<Character>>("Characters", currentCharacterList, "Game");
		}

		void SetLevelUpPanel(string name, int level)
		{
			levelUpName.text = name;
			levelUpLevel.text = level.ToString();
		}
		
		void AddAllExp(int exp)
		{
			var currentList = ES3.Load<List<Character>>("Characters", "Game");
			if (currentList == null)
			{
				print("CurrentList is null");
				return;
			}
			
			var each = (int) exp / currentList.Count;
            
			foreach (var character in currentList)
			{
				if (character.CurrentHp <= 0) continue;
				character.Exp = character.Exp + each;
                
				print(character.CharacterName + " : " + character.Exp);
			}
            
			ES3.Save<List<Character>>("Characters", currentList, "Game");
		}
		
		void AddEqualExp(int exp)
		{
			var each = (int) exp / currentCharacterList.Count;
            
			foreach (var character in currentCharacterList)
			{
				if (character.CurrentHp <= 0) continue;
				character.Exp = character.Exp + each;
                
				print(character.CharacterName + " exp : " + character.Exp);
			}
            
			ES3.Save<List<Character>>("Characters", currentCharacterList, "Game");
		}

		IEnumerator LevelUp()
		{
			var currentList = ES3.Load<List<Character>>("Characters", "Game");
			var levelStage = ES3.Load<List<int>>("LevelStage", "Game");
            
			foreach (var character in currentList)
			{
				var currentLevel = character.Level;

				// print(character.CharacterName + " current level : " + character.Level);
				
				for (int i = 1; i < neededLevel.Count + 1; i++)
				{
					var toLevel = character.Level + 1;
					if (toLevel > neededLevel.Count) break;
					var toLevelStage = levelStage[toLevel];

					if (character.Exp >= toLevelStage)
					{
						levelUp.Invoke();
						SetLevelUpPanel(character.CharacterName, toLevel);
						levelUpWait = LevelUpWait.Wait;

						while (levelUpWait != LevelUpWait.Click)
						{
							yield return null;
						}

						if (levelUpWhat == LevelUpWhat.Deal) 
						{
							character.Level = toLevel;
							character.BaseDeal = character.BaseDeal + 1;
							print("selected deal up (" + character.Level + ", " + character.BaseDeal + ")");
							
							
						}
						else if (levelUpWhat == LevelUpWhat.Hp)
						{
							character.Level = toLevel;
							character.BaseHp = character.BaseHp + 1;
							character.CurrentHp = character.CurrentHp + 1;

							print("selected hp up (" +  character.Level + ", " + character.BaseHp + ", " + character.CurrentHp + ")");
							
						}
						else if (levelUpWhat == LevelUpWhat.Skill)
						{
							character.Level = toLevel;
							// print("selected skill up");
							print("not yet!");
						}

						levelUpWait = LevelUpWait.Non;
						levelUpWhat = LevelUpWhat.Non;
						
						ES3.Save<List<Character>>("Characters", currentList, "Game");
						RenewalCharacter();
						
						print(character.CharacterName + " level up to " + toLevel);
					}
				}
			}
		}

		public void SaveAll()
		{
			ES3.Save<List<Character>>("Characters", currentCharacterList, "Game");
			ES3.Save<int>("TurnDate", turnDate, "Game");
			ES3.Save<int>("Gold", gold, "Game");
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