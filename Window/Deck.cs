using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Window
{
    public enum DeckType
    {
        Non, ShadowWalk, PowerFull, 
    }
        
    public class Deck : MonoBehaviour
    {
        public List<Image> deckImages;
    }
}
