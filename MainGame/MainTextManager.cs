using System;
using System.IO;
using Game.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainGame
{
    public class MainTextManager : MonoBehaviour
    {
        public TextMeshProUGUI sans;
        public TextMeshProUGUI robot;
        public TextMeshProUGUI sansJp;
        public TextMeshProUGUI sansTc;
        public TextMeshProUGUI sansSc;
        public TextMeshProUGUI tai;
        public TextMeshProUGUI alphabet;
        public String currentLanguage = "English";
        public MeshText meshText;

        // home
        public TextMeshProUGUI continueNormal;
        public TextMeshProUGUI continueHigh;
        public TextMeshProUGUI retreatNormal;
        public TextMeshProUGUI retreatHigh;
        public TextMeshProUGUI menuNormal;
        public TextMeshProUGUI menuHigh;
        public TextMeshProUGUI settingNormal;
        public TextMeshProUGUI settingHigh;
        public TextMeshProUGUI exitNormal;
        public TextMeshProUGUI exitHigh;
    
        // settings
        public TextMeshProUGUI settingsHead;
        public TextMeshProUGUI fullScreen;

        private void Start()
        {
            LoadLanguage();
            MakeLanguage();
        }
        
        [ContextMenu("MakeLanguage")]
        public void MakeLanguage()
        {
            if (currentLanguage == "Korean")
            {
                ChangeFont(sans);
            }
            else if (currentLanguage == "Japan")
            {
                ChangeFont(sansJp);
            }
            else if (currentLanguage == "Thailand")
            {
                ChangeFont(tai);
            }
            else if (currentLanguage == "TraditionalChinese")
            {
                ChangeFont(sansTc);
            }
            else if (currentLanguage == "SimpleChinese")
            {
                ChangeFont(sansSc);
            }
            else if (currentLanguage == "Spain")
            {
                ChangeFont(robot);
            }
            else if (currentLanguage == "English")
            {
                ChangeFont(robot);
            }
            else 
            {
                ChangeFont(alphabet);
            }
            
            LoadText();
            PutText();
        }
        
        private void ChangeFont(TextMeshProUGUI change)
        {
            continueNormal.font = change.font;
            continueHigh.font = change.font;
            settingNormal.font = change.font;
            settingHigh.font = change.font;
            exitNormal.font = change.font;
            exitHigh.font = change.font;
            settingsHead.font = change.font;
            fullScreen.font = change.font;
            retreatNormal.font = change.font;
            retreatHigh.font = change.font;
            menuNormal.font = change.font;
            menuHigh.font = change.font;
        }

        void PutText()
        {
            continueNormal.text = meshText.continueGame;
            continueHigh.text = meshText.continueGame;
            settingNormal.text = meshText.setting;
            settingHigh.text = meshText.setting;
            exitNormal.text = meshText.exit;
            exitHigh.text = meshText.exit;
            settingsHead.text = meshText.setting;
            fullScreen.text = meshText.fullScreen;
            retreatNormal.text = meshText.retreat;
            retreatHigh.text = meshText.retreat;
            menuNormal.text = meshText.mainMenu;
            menuHigh.text = meshText.mainMenu;
        }
        
        [ContextMenu("LoadLanguage")]
        void LoadLanguage()
        {
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/Language.Json");
            string jsonData = File.ReadAllText(path);
            var newString = JsonUtility.FromJson<CurrentLanguage>(jsonData);
            currentLanguage = newString.language;
        }

        [ContextMenu("SaveTextMesh")]
        void SaveTextMenu()
        {
            var newString = new MeshText();
            string jsonData = JsonUtility.ToJson(newString, true);
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/MeshText.Json");
            File.WriteAllText(path, jsonData);
        }
        
        [ContextMenu("LoadText")]
        void LoadText()
        {
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/Text" + currentLanguage + ".Json");
            string jsonData = File.ReadAllText(path);
            var newString = JsonUtility.FromJson<MeshText>(jsonData);
            meshText = newString;
        }
    }
}
