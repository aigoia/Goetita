using UnityEngine;

namespace Game.Window
{
    public class CharacterManager : MonoBehaviour
    {
        public float censorExtents = 1f;

        [ContextMenu("WhereIam")]
        public CityArea WhereIam()
        {
            var origins = Physics.BoxCastAll(transform.position, Vector3.one * censorExtents, Vector3.forward, Quaternion.identity, 0, LayerMask.GetMask("CityArea"));
            if (origins.Length == 0) return null;
        
            var originArea = origins[0].transform.GetComponent<CityArea>();
            if (originArea == null) return null;
            return originArea;
        }
        
        public CityArea WhereForm(Vector3 where)
        {
            var origins = Physics.BoxCastAll(where, Vector3.one * censorExtents, Vector3.forward, Quaternion.identity, 0, LayerMask.GetMask("CityArea"));
            if (origins.Length == 0) return null;
        
            var originArea = origins[0].transform.GetComponent<CityArea>();
            if (originArea == null) return null;

            return originArea;
        }
    }
}