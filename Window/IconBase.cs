using UnityEngine;

namespace Game.Window
{
    public class IconBase : MonoBehaviour
    {
        public Transform baseTransform;
        public CityArea baseCityArea;

        private void Start()
        {
           if (baseCityArea == null) baseCityArea = baseTransform.GetComponent<CityArea>();
        }
    }
}
