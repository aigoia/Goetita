using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainGame
{
	public class MainCanvas : MonoBehaviour
	{
		public GameObject enemyTurnText;
		public Transform dialogueBox;
		public List<GameObject> nonPlayerIcon;
		public bool menuOn = false;
		public GameObject gameMenu;
		public Image profileImage;
		public GameObject criticalImage;
		public Vector3 criticalImageMove = new Vector3(-100, 0, 0);
		public Dictionary<string, string> codeNameList;
		public TextMeshProUGUI codeNameText;
		
		void Awake()
		{
			if (dialogueBox == null) dialogueBox = transform.Find("DialogueBox");
			if (enemyTurnText == null) enemyTurnText = transform.Find("EnemyTurnText").gameObject;

			codeNameList = new Dictionary<string, string>()
			{
				{"Tina", "Ketchup"},
				{"Den", "Balsamic"},
				{"Flora", "Thousand island"},
				{"Mia", "Horseradish"},
				{"Clare", "Hot sauce"},
				{"Sue", "Mustard"},
			};
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (menuOn == false)
				{
					gameMenu.SetActive(true);
					menuOn = true;
				}
				else
				{
					gameMenu.SetActive(false);
					menuOn = false;
				}    
			}
		}

		public void CanvasOff()
		{
			foreach (var icon in nonPlayerIcon)
			{
				icon.GetComponent<IconManager>().highlighted.SetActive(false);
			}
			gameObject.SetActive(false);
		}

		public void CanvasOn()
		{
			gameObject.SetActive(true);
			foreach (var icon in nonPlayerIcon)
			{
				icon.GetComponent<IconManager>().SettingIcon();
			}
		}

		public void CriticalAnimation()
		{
			
		}
	}
}
