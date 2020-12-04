using System;
using System.Collections.Generic;
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
			var tapList = new List<Player>();

			foreach (var player in gameManager.activePlayerList)
			{
				if (player.currentVigor > 0)
				{
					tapList.Add(player);
	            }
			}
			
			if (gameManager.currentPlayer == null)
			{
				
				gameManager.SelectPlayer(tapList[0]);
				print(gameManager.currentPlayer);
				return;
				
			}

			var i = 0;
			var nextNumber = 1;

			foreach (var player in gameManager.activePlayerList)
			{
				player.tapId = 0;
			}
			

			foreach (var player in tapList)
			{
				player.tapId = i;
				i++;
				
				if (gameManager.currentPlayer == player)
				{
					nextNumber = i;
				}
			}

			gameManager.SelectPlayer(nextNumber < tapList.Count
				? tapList[nextNumber]
				: tapList[0]);
		}
	}
}
