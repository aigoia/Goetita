using UnityEngine;
using UnityEngine.UI;

namespace Game.Data
{
   [System.Serializable]
    public class ItemImage : MonoBehaviour
    {
        public string itemName;
        public Image itemImage;
        public Vector3 position;
        public float width;
        public float height;

        ItemImage(string itemName, Image itemImage, Vector3 position, float width, float height)
        {
            this.itemName = itemName;
            this.itemImage = itemImage;
            this.position = position;
            this.width = width;
            this.height = height;
        }
    }
}
