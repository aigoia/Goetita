using Game.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Window
{
    [SerializeField]
    public class HealButton : MonoBehaviour
    {
        public HealManager healManager;
        public bool isSelected = false;
        public GameObject active;
        public Character character;
        public Image image;
        

        private void Awake()
        {
            if (healManager == null) FindObjectOfType<HealManager>();
            if (active == null) transform.Find("Active");
            if (image == null) transform.Find("ProfileImage");
        }
        
        public void OnClick()
        {
            isSelected = !isSelected;
            active.SetActive(isSelected);
            
            healManager.SetInformation(character.characterName);
        }
    }
}
