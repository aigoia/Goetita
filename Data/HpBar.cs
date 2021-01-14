using System;
using Game.Window;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Data
{
    public class HpBar : MonoBehaviour
    {
        public Image healthBar;
        Transform _block;
        float _thisWidth;
        float _oneBlockPos;
        int _neededBlock;
        public int bonus = 2;
        public GameObject blockObject;
        public DataManager dataManager;
        public int characterId;

        private void Awake()
        {
            if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
            if (_block == null) _block = transform.Find("Block");
            if (healthBar == null) healthBar = transform.Find("HP").GetComponent<Image>();
        }
        
        public void MakeHp()
        {
            var character = dataManager.currentCharacterList.Find(i => i.characterId == characterId);

            if (character == null) return;

            float currentHp = character.currentHp;
            float baseHp = character.baseHp;
        
            // print( currentHp + " / " + baseHp + " = " + currentHp / baseHp);
        
            // ReSharper disable once PossibleLossOfFraction
            healthBar.fillAmount = (float) currentHp / baseHp;
            _thisWidth = GetComponent<RectTransform>().rect.width;
            _oneBlockPos = _thisWidth / baseHp;
            _neededBlock = (int) baseHp;
            MakeBlock(character);
        }

        public void RenewalHp()
        {
            var character = dataManager.currentCharacterList.Find(i => i.characterId == characterId);
            
            if (character == null) return;

            float currentHp = character.currentHp;
            float baseHp = character.baseHp;
        
            print( currentHp + " / " + baseHp + " = " + currentHp / baseHp);
        
            // ReSharper disable once PossibleLossOfFraction
            healthBar.fillAmount = (float) currentHp / baseHp;
        }
    
        void MakeBlock(Character thisPlayer)
        {
            // print(oneBlockPos);
            for (int i = 1; i < thisPlayer.baseHp ; i++)
            {
                var newBlock = Instantiate(blockObject, _block.position, Quaternion.identity, _block);
                newBlock.GetComponent<RectTransform>().localPosition = new Vector3(_oneBlockPos * i, 0f, 0f);
            }
        }
    }
}
