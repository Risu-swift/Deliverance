using UnityEngine;
using Zenject;

namespace GFG
{
    public class LossTrigger : MonoBehaviour
    {
        GameManager gameManager;
        bool triggeredOnce;

        [Inject]
        public void Construct(GameManager _gameManager)
        {
            gameManager = _gameManager;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(!triggeredOnce && collision.gameObject.CompareTag("Player"))
            {
                triggeredOnce = true;
                gameManager.ShowLossCinematic();
            }
        }
    }
}
