using System;
using System.Collections;
using Game.MainGame;
using TMPro;
using UnityEngine;

namespace Game.Data
{
    public class CheatCode : MonoBehaviour
    {
        public GameManager gameManager;
        public TMP_InputField cheat;
        public GameObject inputPanel;
        public GameObject test;

        private void Awake()
        {
            if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                inputPanel.SetActive(true);
                StartCoroutine(Cheat());
            }
        }

        private IEnumerator Cheat()
        {
            var waitEnter = true;
        
            while (waitEnter)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (cheat.text == "win")
                    {
                        waitEnter = false;
                        
                        gameManager.victory.Invoke();
                    }

                    if (cheat.text == "lose")
                    {
                        waitEnter = false;
                        gameManager.gameOver.Invoke();
                    }

                    inputPanel.SetActive(false);
                }

                yield return null;
            }
        }
    }
}
