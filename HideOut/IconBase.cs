using UnityEngine;
using UnityEngine.UI;

namespace Game.HideOut
{
    public class IconBase : MonoBehaviour
    {
        public Transform BaseTransform;
        public CityArea baseCityArea;
        // public GameObject marketIcon;
        // public GameObject eventIcon;
        public Image marketBoundary;
        public Image EventBoundary;

        private void Start()
        {
           if (baseCityArea == null) baseCityArea = BaseTransform.GetComponent<CityArea>();
           // if (marketIcon == null) marketIcon = GameObject.Find("MarketIcon");
           // if (eventIcon == null) eventIcon = GameObject.Find("EventIcon");
        }
    }
}
