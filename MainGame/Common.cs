using System;
using System.Collections;
using UnityEngine;

namespace Game.MainGame
{
	public class Common : MonoBehaviour
	{
		[SerializeField] GameManager gameManager;

		private void Awake()
		{
			if (gameManager == null) GetComponent<GameManager>();
		}
	}
}
