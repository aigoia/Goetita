using System;
using System.Collections.Generic;
using System.IO;
using Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Menu
{
    public class Options : MonoBehaviour
    {
        private FullScreenMode _screenMode;

        public Toggle fullScreenButton;
        // public Dropdown resolutionDropdown;
        public TMP_Dropdown resolutionDropdown;
    
        private List<Resolution> _resolutions = new List<Resolution>();
        public Resolution currentResolution;

        public int resolutionNumber;

        private int _optionNumber = 0;
        // Start is called before the first frame update

        public Toggle enemyMoveButton;

        public AudioManager audioManager;
        public Option optionData;

        void Start()
        {
            // InitUi();
            if (audioManager == null) audioManager = FindObjectOfType<AudioManager>();
            // Set();
        }

        public void InitSetting()
        { 
            LoadOptions();
            print(optionData.width + "x" + optionData.height + " " + optionData.fullScreenMode);
            Screen.SetResolution(optionData.width, optionData.height, optionData.fullScreenMode);
        }

        [ContextMenu("InitUI")]
        public void InitUi()
        {
            // _resolutions.AddRange(Screen.resolutions)

            foreach (var item in Screen.resolutions)
            {
                if (item.refreshRate == 60)
                {
                    _resolutions.Add(item);
                }
            }
            resolutionDropdown.options.Clear();
            _resolutions.Reverse();
        
            foreach (var resolution in _resolutions)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
                {
                    text = resolution.width + " x " + resolution.height + " " + resolution.refreshRate + "hz"
                };
                resolutionDropdown.options.Add(option);
            

                if (resolution.width == Screen.width && resolution.height == Screen.height)
                {
                    resolutionDropdown.value = _optionNumber;
                }

                _optionNumber++;
            
            }
            resolutionDropdown.RefreshShownValue();
            
            // fullScreenButton.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
        }

        public void Set()
        {
            resolutionDropdown.value = optionData.dropValue;
        }

        public void DropdownOptionChange(int x)
        {
            resolutionNumber = x;
        }

        public void FullScreenButton(bool isFull)
        {
            _screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        }

        public void OkClickFullScreen()
        {
            optionData.dropValue = resolutionDropdown.value;
            optionData.width = _resolutions[optionData.dropValue].width;
            optionData.height = _resolutions[optionData.dropValue].height;

            optionData.fullScreenMode = fullScreenButton.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            SaveOptions();
            print(optionData.width + "x" + optionData.height + " " + optionData.fullScreenMode);
            Screen.SetResolution(_resolutions[optionData.dropValue].width, _resolutions[optionData.dropValue].height, optionData.fullScreenMode);
        }

        public void OkClickEnemyMove()
        {
            if (audioManager == null) audioManager = FindObjectOfType<AudioManager>();
            if (audioManager == null) return;

            optionData.enemyMove = enemyMoveButton.isOn;
            SaveOptions();
            audioManager.enemyMove = enemyMoveButton.isOn;
        }
        
        [ContextMenu("SetInitOptions")]
        public void SetInitCharacter()
        {
            optionData = new Option(true, FullScreenMode.FullScreenWindow, 0, 1920, 1080);
            SaveOptions();
        }
        
        [ContextMenu("LoadOptions")]
        public void LoadOptions()
        {
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/OptionData.Json");
            string jsonData = File.ReadAllText(path);
            optionData = JsonUtility.FromJson<Option>(jsonData);
        }

        [ContextMenu("SaveOptions")]
        public void SaveOptions()
        {
            string jsonData = JsonUtility.ToJson(optionData, true);
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/OptionData.Json");
            File.WriteAllText(path, jsonData);
        }
    }

    public class Option
    {
        public bool enemyMove;
        public FullScreenMode fullScreenMode;
        public int dropValue;
        public int width;
        public int height;

        public Option(bool enemyMove, FullScreenMode fullScreenMode, int dropValue, int width, int height)
        {
            this.enemyMove = enemyMove;
            this.fullScreenMode = fullScreenMode;
            this.dropValue = dropValue;
            this.width = width;
            this.height = height;
        }
    }
}
