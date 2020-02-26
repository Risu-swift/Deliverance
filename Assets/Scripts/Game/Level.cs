using UnityEngine;

namespace GFG
{
    public class Level : MonoBehaviour
    {
        public Canvas Canvas;

        private void Awake()
        {
            Canvas.worldCamera = Camera.main;
        }
    }
}
