using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
