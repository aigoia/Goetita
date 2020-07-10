using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainGame
{
	public class PlayerUi : MonoBehaviour {

		GameManager _gameManager;
		Transform _profile;
		[SerializeField] Image profileImage;

		[SerializeField] Sprite red;
		[SerializeField] Sprite blue;
		[SerializeField] Sprite pink;
		[SerializeField] Sprite green;
		[SerializeField] Sprite white;
		[SerializeField] Sprite purple;

		public List<GameObject> iconList;
		public List<GameObject> activeUiList;
		
		void Awake()
		{
			if (_profile == null) _profile = this.transform.Find("Profile");
			if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
			if (profileImage == null) profileImage = _profile.Find("ProfileImage").GetComponent<Image>();
		}

		void OnEnable()
		{
			if (_gameManager.currentPlayer == null) return;
	
			if (_gameManager.currentPlayer.name == "Galateia")profileImage.sprite = red;
			else if (_gameManager.currentPlayer.name == "Jean")profileImage.sprite = blue;
			else if (_gameManager.currentPlayer.name == "Flora")profileImage.sprite = pink;
			else if (_gameManager.currentPlayer.name == "Miria")profileImage.sprite = green;
			else if (_gameManager.currentPlayer.name == "Clare")profileImage.sprite = white;
			else if (_gameManager.currentPlayer.name == "Deneve")profileImage.sprite = purple;
		}

		public void UiSetActive(bool on)
		{
			foreach (var ui in activeUiList)
			{
				ui.SetActive(on);
			}
		}
		
	}
}
