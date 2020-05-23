using System.Collections.Generic;
using UnityEngine;

namespace Game.MainGame
{
	public class MainCanvas : MonoBehaviour
	{
		public GameObject enemyTurnText;
		public Transform dialogueBox;
		public List<GameObject> nonPlayerIcon;
		
		void Awake()
		{
			if (dialogueBox == null) dialogueBox = transform.Find("DialogueBox");
			if (enemyTurnText == null) enemyTurnText = transform.Find("EnemyTurnText").gameObject;
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

	}
}
