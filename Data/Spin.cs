using UnityEngine;

namespace Game.Data
{
    public class Spin : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<Animation>().PlayQueued("Spin");
        }
    }
}
