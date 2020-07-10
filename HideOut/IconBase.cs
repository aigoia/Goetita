using UnityEngine;

namespace Game.HideOut
{
    public class IconBase : MonoBehaviour
    {
        public Transform BaseTransform;
        public GameObject marketIcon;
        public GameObject eventIcon;

        private void Start()
        {
            marketIcon = GameObject.Find("MarketIcon");
            eventIcon = GameObject.Find("EventIcon");
        }
    }
}
