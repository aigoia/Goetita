using UnityEngine;

namespace Game.MainGame
{
    public class TileManager : MonoBehaviour
    {
        public int x = 14;
        public int y = 14;
        public int initX = 2;
        public int initY = 2;
        public GameObject tile;
        public int interval = 4;
    
        [ContextMenu("Set Tile")]
        public void SetTile()
        {
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    Instantiate(tile,
                        new Vector3(i * interval + initX, 0, j  * interval + initY),
                        Quaternion.identity, this.transform);
                }
            }
        }
    }
}
