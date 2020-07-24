using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Data
{
	public class DataManager : MonoBehaviour
	{
		public int gold = 0;
		public TextMeshProUGUI goldText;
		public int turnDate = 0;
		public TextMeshProUGUI turnDateText;
		
		public List<GameObject> imageList;
		public List<Character> currentCharacterList = new List<Character>();

		public GameObject mainCharacter;
		
		private void Awake()
		{
			currentCharacterList = new List<Character>()
			{
				new Character(0, "Red", imageList[0]),
				new Character(1, "Purple", imageList[1]),
			};
		}
		
		public void Start()
		{
			turnDate = 0;
			RenewalTurnData();
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
			turnDateText.text = TurnToText(turnDate);
		}

		public void RenewalGold()
		{
			goldText.text = GoldToText(gold);
		}
		
	}
}