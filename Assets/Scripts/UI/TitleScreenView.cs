using Zenject;
using UnityEngine;

namespace GFG
{
    public class TitleScreenView : MonoBehaviour
    {
        SignalBus signalBus;
        GameManager gameManager;

        [Inject]
        public void Construct(SignalBus _signalBus, GameManager _gameManager)
        {
            signalBus = _signalBus;
            gameManager = _gameManager;
        }

        public void StartGame()
        {
            gameManager.StartGame();
        }

        public void LoadLevel1()
		{
			gameManager.LoadLevel();
		}

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
