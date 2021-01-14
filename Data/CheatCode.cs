using System.Collections;
using TMPro;
using UnityEngine;

namespace Game.Data
{
    public class CheatCode : MonoBehaviour
    {
        public TMP_InputField cheat;
        public GameObject inputPanel;
        public GameObject test;
        public GameObject win;
    

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
                        win.SetActive(true);
                    }

                    inputPanel.SetActive(false);
                }

                yield return null;
            }
        }
    }
}
