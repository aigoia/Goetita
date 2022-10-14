using System;
using System.Collections;
// using Game.MainGame;
using TMPro;
using UnityEngine;

namespace Game.Data
{
    public enum SceneType
    {
        Non, Main, Window, Menu,
    }
    
    public class CheatCode : MonoBehaviour
    {
        public MainGame.GameManager mainGameManager;
        public Window.GameManager windowGameManager;
        public DataManager dataManager;
        public TMP_InputField cheat;
        public GameObject inputPanel;
        public GameObject test;
        public SceneType sceneType;
        public int addGold = 10000;
        public int addExp = 64;

        private void Awake()
        {
            // if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
        }

        private void Start()
        {
            if (sceneType == SceneType.Main)
            {
                if (mainGameManager == null) mainGameManager = FindObjectOfType<MainGame.GameManager>();
            }
            else if (sceneType == SceneType.Window)
            {
                if (windowGameManager == null) windowGameManager = FindObjectOfType<Window.GameManager>();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                inputPanel.SetActive(true);
                StartCoroutine(Cheat());
            }

            // if (Input.GetKeyDown(KeyCode.Escape))
            // {
            //     gameObject.SetActive(false);
            // }
        }

        private IEnumerator Cheat()
        {
            var waitEnter = true;

            if (sceneType == SceneType.Main)
            {
                while (waitEnter)
                {
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        if (cheat.text == "win")
                        {
                            waitEnter = false;
                        
                            print("Cheat win");
                            mainGameManager.victory.Invoke();
                        }

                        if (cheat.text == "lose")
                        {
                            waitEnter = false;
                            
                            print("Cheat lose");
                            mainGameManager.gameOver.Invoke();
                        }

                        inputPanel.SetActive(false);
                    }

                    yield return null;
                }
            }
            else if (sceneType == SceneType.Window)
            {
                while (waitEnter)
                {
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        if (cheat.text == "money")
                        {
                            waitEnter = false;
                            dataManager.AddGold(addGold);
                        }
                        if (cheat.text == "exp")
                        {
                            waitEnter = false;
                            dataManager.AddAllExp(addExp);
                            dataManager.LevelCheck();
                        }

                        inputPanel.SetActive(false);
                    }

                    yield return null;
                }    
            }
            
            
        }
    }
}
