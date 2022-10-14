using System;
using System.IO;
using Game.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Window
{
    public class WindowTextManager : MonoBehaviour
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
        
        public TextMeshProUGUI continueNormal;
        public TextMeshProUGUI continueHigh;
        public TextMeshProUGUI menuNormal;
        public TextMeshProUGUI menuHigh;
        public TextMeshProUGUI settingNormal;
        public TextMeshProUGUI settingHigh;
        public TextMeshProUGUI exitNormal;
        public TextMeshProUGUI exitHigh;
    
        // settings
        public TextMeshProUGUI settingsHead;
        public TextMeshProUGUI fullScreen;
        
        // button
        public TextMeshProUGUI eventButton;
        public TextMeshProUGUI eventButtonHigh;
        public TextMeshProUGUI marketButton;
        public TextMeshProUGUI marketButtonHigh;
        public TextMeshProUGUI inventoryButton;
        public TextMeshProUGUI inventoryButtonHigh;
        public TextMeshProUGUI hospitalButton;
        public TextMeshProUGUI hospitalButtonHigh;
        public TextMeshProUGUI rearmButton;
        public TextMeshProUGUI rearmButtonHigh;
        
        // window
        public TextMeshProUGUI eventHead;
        public TextMeshProUGUI marketHead;
        public TextMeshProUGUI buyNormal;
        public TextMeshProUGUI buyHigh;
        public TextMeshProUGUI buyPress;
        public TextMeshProUGUI sellNormal;
        public TextMeshProUGUI sellHigh;
        public TextMeshProUGUI sellPress;
        public TextMeshProUGUI hospitalHead;
        public TextMeshProUGUI rearmHead;
        public TextMeshProUGUI levelUpHead;
        public TextMeshProUGUI textOne;
        public TextMeshProUGUI textOneCurrent;
        public TextMeshProUGUI textTwo;
        public TextMeshProUGUI textTwoCurrent;
        public string current;
        public string hpPlus;
        public string armorPlus;
        public string randomDealPlus;
        public string baseDealPlus;
        
        // inventory
        public TextMeshProUGUI inventoryHead;
        public TextMeshProUGUI profileNormal;
        public TextMeshProUGUI profileHigh;
        public TextMeshProUGUI profilePress;
        public TextMeshProUGUI stockNormal;
        public TextMeshProUGUI stockHigh;
        public TextMeshProUGUI stockPress;
        public TextMeshProUGUI statsNormal;
        public TextMeshProUGUI statsHigh;
        public TextMeshProUGUI statsPress;
        public TextMeshProUGUI damageText;
        public TextMeshProUGUI armorText;
        public TextMeshProUGUI baseText;
        public TextMeshProUGUI skillText;
        public TextMeshProUGUI traitText;
        
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
            menuNormal.font = change.font;
            menuHigh.font = change.font;
            
            eventButton.font = change.font;
            eventButtonHigh.font = change.font;
            marketButton.font = change.font;
            marketButtonHigh.font = change.font;
            inventoryButton.font = change.font;
            inventoryButtonHigh.font = change.font;
            hospitalButton.font = change.font;
            hospitalButtonHigh.font = change.font;
            rearmButton.font = change.font;
            rearmButtonHigh.font = change.font;
            
            eventHead.font = change.font;
            marketHead.font = change.font;
            buyNormal.font = change.font;
            buyHigh.font = change.font;
            buyPress.font = change.font;
            sellNormal.font = change.font;
            sellHigh.font = change.font;
            sellPress.font = change.font;
            hospitalHead.font = change.font;
            rearmHead.font = change.font;
            levelUpHead.font = change.font;
            textOne.font = change.font;
            textOneCurrent.font = change.font;
            textTwo.font = change.font;
            textTwoCurrent.font = change.font;
            
            inventoryHead.font = change.font;
            profileNormal.font = change.font;
            profileHigh.font = change.font;
            profilePress.font = change.font;
            stockNormal.font = change.font;
            stockHigh.font = change.font;
            stockPress.font = change.font;
            statsNormal.font = change.font;
            statsHigh.font = change.font;
            statsPress.font = change.font;
            damageText.font = change.font;
            armorText.font = change.font;
            baseText.font = change.font;
            skillText.font = change.font;
            traitText.font = change.font;
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
            menuNormal.text = meshText.mainMenu;
            menuHigh.text = meshText.mainMenu;

            eventButton.text = meshText.eventMission;
            eventButtonHigh.text = meshText.eventMission;
            marketButton.text = meshText.market;
            marketButtonHigh.text = meshText.market;
            inventoryButton.text = meshText.inventory;
            inventoryButtonHigh.text = meshText.inventory;
            hospitalButton.text = meshText.hospital;
            hospitalButtonHigh.text = meshText.hospital;
            rearmButton.text = meshText.rearm;
            rearmButtonHigh.text = meshText.rearm;

            eventHead.text = meshText.eventMission;
            marketHead.text = meshText.market;
            buyNormal.text = meshText.buy;
            buyHigh.text = meshText.buy;
            buyPress.text = meshText.buy;
            sellNormal.text = meshText.sell;
            sellHigh.text = meshText.sell;
            sellPress.text = meshText.sell;
            hospitalHead.text = meshText.hospital;
            rearmHead.text = meshText.rearm;
            levelUpHead.text = meshText.levelUp;
            current = meshText.current;

            hpPlus = meshText.HP;
            armorPlus = meshText.armor;
            baseDealPlus = meshText.ATK;
            randomDealPlus = meshText.randomATK;
            
            inventoryHead.text = meshText.inventory;
            profileNormal.text = meshText.profile;
            profileHigh.text = meshText.profile;
            profilePress.text = meshText.profile;
            stockNormal.text = meshText.stock; 
            stockHigh.text = meshText.stock;
            stockPress.text = meshText.stock;
            statsNormal.text = meshText.stats; 
            statsHigh.text = meshText.stats;
            statsPress.text = meshText.stats;
            baseText.text = meshText.baseText; 
            skillText.text = meshText.skill; 
            traitText.text = meshText.trait;
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
