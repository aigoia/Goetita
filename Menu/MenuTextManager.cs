using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Game.Menu
{
    public class MenuTextManager : MonoBehaviour
    {
        public TextMeshProUGUI sans;
        public TextMeshProUGUI sansJp;
        public TextMeshProUGUI sansTc;
        public TextMeshProUGUI sansSc;
        public TextMeshProUGUI tai;
        public TextMeshProUGUI robot;
        public TextMeshProUGUI alphabet;
        
        public String currentLanguage = "English";
        public MeshText meshText;
        public TMP_Dropdown dropdownData;
        public VerticalLayoutGroup layoutGroup;
        public HorizontalLayoutGroup continueGroup;
        public HorizontalLayoutGroup newGameGroup;
        
        // home
        public TextMeshProUGUI playNormal;
        public TextMeshProUGUI playHigh;
        public TextMeshProUGUI settingNormal;
        public TextMeshProUGUI settingHigh;
        public TextMeshProUGUI exitNormal;
        public TextMeshProUGUI exitHigh;

        // settings
        public TextMeshProUGUI settingsHead;
        public TextMeshProUGUI fullScreen;

        // play
        public TextMeshProUGUI playHead;
        public TextMeshProUGUI continueNormal;
        public TextMeshProUGUI continueHigh;
        public TextMeshProUGUI newGameNormal;
        public TextMeshProUGUI newGameHigh;
        public GameStart gameStart;
        public TextMeshProUGUI textOne;
        public TextMeshProUGUI textTwo;
        public TextMeshProUGUI textThree;
        public TextMeshProUGUI old;


        private void Start()
        {
            LoadLanguage();
            MakeLanguage();
        }

        [ContextMenu("Layout")]
        void LayOut()
        {
            layoutGroup.enabled = false;
            layoutGroup.enabled = true;
            continueGroup.enabled = false;
            continueGroup.enabled = true;
            newGameGroup.enabled = false;
            newGameGroup.enabled = true;
        }

        private void Update()
        {
            LayOut();
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

        public void SetLanguage()
        {
            currentLanguage = dropdownData.options[dropdownData.value].text;
            SaveLanguage();
            MakeLanguage();
        }
        
        private void ChangeFont(TextMeshProUGUI change)
        {
            playNormal.font = change.font;
            playHigh.font = change.font;
            settingNormal.font = change.font;
            settingHigh.font = change.font;
            exitNormal.font = change.font;
            exitHigh.font = change.font;
            settingsHead.font = change.font;
            fullScreen.font = change.font;
            playHead.font = change.font;
            continueNormal.font = change.font;
            continueHigh.font = change.font;
            newGameNormal.font = change.font;
            newGameHigh.font = change.font;
            textOne.font = change.font;
            textTwo.font = change.font;
            textThree.font = change.font;
            old.font = change.font;
        }
        
        // private void ChangeFontSans()
        // {
        //     playNormal.font = sans.font;
        //     playHigh.font = sans.font;
        //     settingNormal.font = sans.font;
        //     settingHigh.font = sans.font;
        //     exitNormal.font = sans.font;
        //     exitHigh.font = sans.font;
        //     settingsHead.font = sans.font;
        //     fullScreen.font = sans.font;
        //     playHead.font = sans.font;
        //     continueNormal.font = sans.font;
        //     continueHigh.font = sans.font;
        //     newGameNormal.font = sans.font;
        //     newGameHigh.font = sans.font;
        //     textOne.font = sans.font;
        //     textTwo.font = sans.font;
        //     textThree.font = sans.font;
        //     old.font = sans.font;
        // }
        //
        // private void ChangeFontRobot()
        // {
        //     playNormal.font = robot.font;
        //     playHigh.font = robot.font;
        //     settingNormal.font = robot.font;
        //     settingHigh.font = robot.font;
        //     exitNormal.font = robot.font;
        //     exitHigh.font = robot.font;
        //     settingsHead.font = robot.font;
        //     fullScreen.font = robot.font;
        //     playHead.font = robot.font;
        //     continueNormal.font = robot.font;
        //     continueHigh.font = robot.font;
        //     newGameNormal.font = robot.font;
        //     newGameHigh.font = robot.font;
        //     textOne.font = robot.font;
        //     textTwo.font = robot.font;
        //     textThree.font = robot.font;
        //     old.font = robot.font;
        // }

        void PutText()
        {
            playNormal.text = meshText.play;
            playHigh.text = meshText.play;
            settingNormal.text = meshText.setting;
            settingHigh.text = meshText.setting;
            exitNormal.text = meshText.exit;
            exitHigh.text = meshText.exit;
            settingsHead.text = meshText.setting;
            fullScreen.text = meshText.fullScreen;
            playHead.text = meshText.play;
            continueNormal.text = meshText.continueGame;
            continueHigh.text = meshText.continueGame;
            newGameNormal.text = meshText.newGame;
            newGameHigh.text = meshText.newGame;
            gameStart.hpPlus = meshText.HP;
            gameStart.armorPlus = meshText.armor;
            gameStart.deckPlus = meshText.skill;
            gameStart.randomDealPlus = meshText.randomATK;
            gameStart.baseDealPlus = meshText.ATK;
            old.text = meshText.old;
        }

        [ContextMenu("LoadLanguage")]
        void LoadLanguage()
        {
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/Language.Json");
            string jsonData = File.ReadAllText(path);
            var newString = JsonUtility.FromJson<CurrentLanguage>(jsonData);
            currentLanguage = newString.language;
            dropdownData.value = newString.value;
            dropdownData.RefreshShownValue();
        }
        
        [ContextMenu("SaveLanguage")]
        void SaveLanguage()
        {
            var newString = new CurrentLanguage {language = currentLanguage, value =  dropdownData.value};
            string jsonData = JsonUtility.ToJson(newString, true);
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/Language.Json");
            File.WriteAllText(path, jsonData);
        }

        [ContextMenu("SaveMeshText")]
        void SaveTextMenu()
        {
            var newString = new MeshText();
            string jsonData = JsonUtility.ToJson(newString, true);
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/MeshText.Json");
            File.WriteAllText(path, jsonData);
        }
        
        [ContextMenu("LoadMeshText")]
        void LoadMeshText()
        {
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/MeshText.Json");
            string jsonData = File.ReadAllText(path);
            var newString = JsonUtility.FromJson<MeshText>(jsonData);
            meshText = newString;
        }
        
        [ContextMenu("LoadText")]
        void LoadText()
        {
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/Text" + currentLanguage + ".Json");
            string jsonData = File.ReadAllText(path);
            var newString = JsonUtility.FromJson<MeshText>(jsonData);
            meshText = newString;
        }

        // [ContextMenu("LoadKorean")]
        // void LoadKorean()
        // {
        //     string path = Path.Combine(Application.dataPath + "/StreamingAssets/TextKorean.Json");
        //     string jsonData = File.ReadAllText(path);
        //     var newString = JsonUtility.FromJson<MeshText>(jsonData);
        //     meshText = newString;
        // }
        //
        // [ContextMenu("LoadEnglish")]
        // void LoadEnglish()
        // {
        //     string path = Path.Combine(Application.dataPath + "/StreamingAssets/TextEnglish.Json");
        //     string jsonData = File.ReadAllText(path);
        //     var newString = JsonUtility.FromJson<MeshText>(jsonData);
        //     meshText = newString;
        // }
        //
        // [ContextMenu("LoadUkraine")]
        // void LoadUkraine()
        // {
        //     string path = Path.Combine(Application.dataPath + "/StreamingAssets/TextUkraine.Json");
        //     string jsonData = File.ReadAllText(path);
        //     var newString = JsonUtility.FromJson<MeshText>(jsonData);
        //     meshText = newString;
        // }
        //
        // [ContextMenu("LoadVietnam")]
        // void LoadVietnam()
        // {
        //     string path = Path.Combine(Application.dataPath + "/StreamingAssets/TextVietnam.Json");
        //     string jsonData = File.ReadAllText(path);
        //     var newString = JsonUtility.FromJson<MeshText>(jsonData);
        //     meshText = newString;
        // }
        //
        // [ContextMenu("LoadTurkish")]
        // void LoadTurkish()
        // {
        //     string path = Path.Combine(Application.dataPath + "/StreamingAssets/TextTurkish.Json");
        //     string jsonData = File.ReadAllText(path);
        //     var newString = JsonUtility.FromJson<MeshText>(jsonData);
        //     meshText = newString;
        // }
    }

    [Serializable]
    public class CurrentLanguage
    {
        public String language;
        public int value;
    }
    
    [Serializable]
    public class MeshText
    {
        public string play;
        public string setting;
        public string exit;
        public string fullScreen;
        public string continueGame;
        public string newGame;
        public string HP;
        public string skill;
        public string armor;
        public string randomATK;
        public string ATK;
        public string old;
        public string retreat;
        public string mainMenu;

        public string eventMission;
        public string market;
        public string inventory;
        public string hospital;
        public string rearm;
        public string levelUp;

        public string buy;
        public string sell;
        public string current;
        
        public string profile;
        public string stock;
        public string stats;
        public string baseText;
        public string trait;

    }
}
