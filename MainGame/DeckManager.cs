using System;
using System.Collections.Generic;
using Game.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainGame
{
    public class DeckManager : MonoBehaviour
    {
        public GameManager gameManager;
        public List<Deck> cardList;
        public List<DeckType> cardTypeList;
        public List<DeckType> waitingList;

        public float moving = 30f;
        public float upDownSpeed = 0.2f;
        
        
        public int cardDeal = 2;
        public float showWait = 1f;

        // public List<Vector3> posList;

        private void Awake()
        {
            if (gameManager == null) gameManager = FindObjectOfType<GameManager>();


            // foreach (var card in cardList)
            // {
            //     posList.Add(card.transform.position);
            // }
        }

        private void Start()
        {
            // Invoke(nameof(InitShow), showWait);
        }

        public void InitShow()
        {
            foreach (var card in cardList)
            {
                card.gameObject.SetActive(true);
                card.ShowCard();
            }
        }
        
        public void AllReset()
        {
            foreach (var card in cardList)
            {
                if (card.cardOn == true)
                {
                    card.OffCard();
                }
            }
        }
        
        public void ResetCard()
        {
            foreach (var card in cardList)
            {
                card.gameObject.SetActive(false);
                card.deckType = DeckType.Non;
                
                foreach (var image in card.images)
                {
                    image.gameObject.SetActive(false);
                }
            }
        }
    }
}
