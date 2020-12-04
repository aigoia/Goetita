using System;
using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Window;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Image healthBar;
    Transform _block;
    float _thisWidth;
    float _oneBlockPos;
    int _neededBlock;
    public int bonus = 2;
    public GameObject blockObject;
    public CharacterSelect characterSelect;
    public CharacterButton characterButton;

    private void Awake()
    {
        if (characterSelect == null) characterSelect = FindObjectOfType<CharacterSelect>();
        if (_block == null) _block = transform.Find("Block");
        if (healthBar == null) healthBar = transform.Find("HP").GetComponent<Image>();
        if (characterButton == null) characterButton = transform.parent.GetComponent<CharacterButton>();
    }

    public void MakeHp()
    {
        var character = characterSelect.dataManager.currentCharacterList.Find(i => i.CharacterId == characterButton.characterId);
        
        if (character == null) return;

        float currentHp = character.CurrentHp;
        float baseHp = character.BaseHp;
        
        print( currentHp + " / " + baseHp + " = " + currentHp / baseHp);
        
        // ReSharper disable once PossibleLossOfFraction
        healthBar.fillAmount = (float) currentHp / baseHp;
        _thisWidth = GetComponent<RectTransform>().rect.width;
        _oneBlockPos = _thisWidth / baseHp;
        _neededBlock = (int) baseHp;
        MakeBlock(character);
    }

    public void RenewalHp()
    {
        var character = characterSelect.dataManager.currentCharacterList.Find(i => i.CharacterId == characterButton.characterId);
        
        if (character == null) return;

        float currentHp = character.CurrentHp;
        float baseHp = character.BaseHp;
        
        print( currentHp + " / " + baseHp + " = " + currentHp / baseHp);
        
        // ReSharper disable once PossibleLossOfFraction
        healthBar.fillAmount = (float) currentHp / baseHp;
    }
    
    void MakeBlock(Character thisPlayer)
    {
        // print(oneBlockPos);
        for (int i = 1; i < thisPlayer.BaseHp ; i++)
        {
            var newBlock = Instantiate(blockObject, _block.position, Quaternion.identity, _block);
            newBlock.GetComponent<RectTransform>().localPosition = new Vector3(_oneBlockPos * i, 0f, 0f);
        }
    }
}
