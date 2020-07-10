using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.MainGame
{
	public class DialogueTrigger : MonoBehaviour {


		Dialogue _dialogue;
		DialogueManager _dialogManager;

		MainCanvas _mainCanvas;

		public string fileName;
		string _path;
		string _jsonString;

		Queue<string> _sentences;

		void Awake()
		{
			if (_mainCanvas == null) _mainCanvas = FindObjectOfType<MainCanvas>();
			if (_dialogManager == null) _dialogManager = FindObjectOfType<DialogueManager>();
		}

		public void TriggerDialogue()
		{
			if (_dialogManager == null) return;

			// json import
			_dialogue = new Dialogue();
			_path = Application.streamingAssetsPath + "/" + fileName + ".json";
			_jsonString = File.ReadAllText (_path);
			_dialogue = JsonUtility.FromJson<Dialogue>(_jsonString);

			_dialogManager.StartDialogue(_dialogue);
			_mainCanvas.dialogueBox.gameObject.SetActive(true);
		}

	}
}
