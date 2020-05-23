using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainGame
{
	public class DialogueManager : MonoBehaviour {

		public Text dialogueText;
		MainCanvas _mainCanvas;
		public GameObject dialogueBox;

		Queue<string> _sentences;

		void Awake()
		{
			if (_mainCanvas == null) _mainCanvas = FindObjectOfType<MainCanvas>();
		}

		void Start()
		{
			_sentences = new Queue<string>();
		}

		public void StartDialogue(Dialogue dialog)
		{
			dialogueBox.SetActive(true);
			_sentences.Clear();
			if (dialog.Sentences == null) return;

			foreach (var sentence in dialog.Sentences)
			{
				_sentences.Enqueue(sentence);
			}

			DisplaceNextSentence();
		}

		public void DisplaceNextSentence()
		{
			if (_sentences.Count == 0)
			{
				EndDialogue();
				return;
			}

			string sentence = _sentences.Dequeue();
			StopAllCoroutines();
			StartCoroutine(TypeSentence (sentence));
		}

		IEnumerator TypeSentence(string sentence)
		{
			dialogueText.text = "";

			foreach (var letter in sentence.ToCharArray())
			{
				dialogueText.text += letter;
				yield return null;
			}
		}

		void EndDialogue()
		{
			dialogueBox.SetActive (false);
			print("End of conversation");
		}
	}
}
