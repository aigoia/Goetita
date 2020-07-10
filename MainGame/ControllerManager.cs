using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.MainGame
{
	public class ControllerManager : MonoBehaviour
	{
		public UnityEvent menuOn;
		private ModalManager _modalManager;
		public GameManager gameManager;
		
		void Awake()
		{
			if (_modalManager == null) _modalManager = FindObjectOfType<ModalManager>();
			if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
		}

		void Update () 
		{
			OpenMenu();
			PressTap();
		}

		private void PressTap()
		{
			if (Input.GetButtonDown("Tap"))
			{
				TapController();
			}
		}

		void OpenMenu()
		{
			if (_modalManager.keyHelper.activeSelf) return;

			if (Input.GetButtonDown("Menu"))
			{
				menuOn.Invoke();
			}
		}

		void TapController()
		{
			if (gameManager.currentPlayer == null)
			{
				gameManager.SelectPlayer(gameManager.activePlayerList[0]);
				print(gameManager.currentPlayer);
				return;
			}

			var i = 0;
			var nextNumber = 1;
			
			foreach (var player in gameManager.activePlayerList)
			{
				player.activeNumber = i;
				i++;

				if (gameManager.currentPlayer == player)
				{
					nextNumber = i;
				}
			}

			gameManager.SelectPlayer(nextNumber < gameManager.activePlayerList.Count
				? gameManager.activePlayerList[nextNumber]
				: gameManager.activePlayerList[0]);
		}
	}
}
