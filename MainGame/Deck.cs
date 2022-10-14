using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainGame
{
    public enum DeckType
    {
        Non, ShadowWalk, PowerFull, 
    }
    
    public class Deck : MonoBehaviour
    {
        public DeckManager deckManager;
        public int index;
        public bool cardOn = false;
        
        private void Awake() {
            if (deckManager == null) deckManager = FindObjectOfType<DeckManager>();
        }

        public DeckType deckType = DeckType.Non;
        public List<Image> images;

        void NoOverlap()
        {
            foreach (var card in deckManager.cardList)
            {
                if (card.name == this.name) continue;
                
                if (card.deckType == deckType)
                {
                    if (card.cardOn == true)
                    {
                        return;
                    }
                }
            }
        }
        
        public void OnCard()
        {
            if (deckManager.gameManager.somethingOn) return;
            
            if (cardOn == false)
            {
                
                this.GetComponent<Animation>().PlayQueued(name + "Up");
                // iTween.MoveBy(gameObject, Vector3.up * cardManager.moving, cardManager.upDownSpeed);
                cardOn = true;
            }
            else if (cardOn == true)
            {
                
                this.GetComponent<Animation>().PlayQueued(name + "Down");
                // iTween.MoveBy(gameObject, Vector3.down * cardManager.moving, cardManager.upDownSpeed);
                cardOn = false;
                
                // OffCard();
            }
        }

        public void ShowCard()
        {
            this.GetComponent<Animation>().PlayQueued(name + "Show");
        }

        public void OffCard()
        {
            GameObject o = gameObject;

            foreach (var image in images)
            {
                image.gameObject.SetActive(false);
            }
            cardOn = false;

            if (deckManager.waitingList.Count == 0)
            {
                o.SetActive(false);
            }
            else
            {
                var newWaitingList = new List<DeckType>();
                var waitingCard = deckManager.waitingList[0];
                // cards[count].gameObject.SetActive(true);
                deckType = waitingCard;
                var image = images.Find(i => i.name == waitingCard.ToString());
                // print(image);
                image.gameObject.SetActive(true);

                int count = 0;
                if (deckManager.waitingList.Count != 0)
                {
                    foreach (var type in deckManager.waitingList)
                    {
                        if (count == 0)
                        {
                            count += 1;
                            continue;
                        }

                        newWaitingList.Add(type);
                        count += 1;
                    }
                }

                deckManager.waitingList = newWaitingList;
            }

            this.GetComponent<Animation>().PlayQueued(name + "Show");
         
        }
    }
}
