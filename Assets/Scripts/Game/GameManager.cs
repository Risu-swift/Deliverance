using Zenject;
using System;
using UnityEngine;

namespace GFG
{
    public class GameManager : IInitializable, IDisposable, IFixedTickable
    {
        Settings settings;
		GameView gameView;
        SignalBus signalBus;

        //int playerSpotted = 0;
        bool playerSpotted;
        float timeSpentVisible;


        [Inject]
        public void Construct(Settings _settings, SignalBus _signalBus, GameView _gameView)
        {
            settings = _settings;
            signalBus = _signalBus;

			gameView = _gameView;

			StartUp();
        }

        public void Initialize()
        {
			//signalBus.Subscribe<StartGameSignal>(StartGame);	
		}

        public void Dispose()
        {
            //signalBus.Unsubscribe<StartGameSignal>(StartGame);
        }

        /*public void StartGame(StartGameSignal signal)
        {

            Debug.Log("New Game Started.");
        }*/

        public void StartGame()
		{
            playerSpotted = false;

            gameView.PlayIntroCinematic();
		}

        public void LoadLevel(int id = 0)
        {
            playerSpotted = false;
            gameView.LoadLevel(id);
        }

        public void ShowTitleScreen()
        {
            gameView.ShowTitleScreen();
        }

        public void ShowCaptureCinematic()
        {
            gameView.PlayCaptureCinematic();
        }

        public void ShowLossCinematic()
        {
            gameView.PlayLossCinematic();
        }

        public void ShowEndCinematic()
        {
            gameView.PlayEndCinematic();
        }

        public void FixedTick()
        {
            if(playerSpotted)
            {
                timeSpentVisible += Time.deltaTime;

                if(timeSpentVisible >= settings.VisibleTimeToGameOver)
                {
                    playerSpotted = false;
                    gameView.PlayerSpotted(false);

                    ShowLossCinematic();
                }
            }
        }

        public void PlayerSpotted(bool spotted = true)
        {
            if(spotted)
            {
                if(!playerSpotted)
                {
                    timeSpentVisible = 0;
                }
                playerSpotted = true;
           
                gameView.PlayerSpotted(true);
            }
            else{
                if(playerSpotted)
                {
                    playerSpotted = false;
                    gameView.PlayerSpotted(false);
                }
            }

        }

        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
        }

        public void SpeedUpGame()
        {
            //Time.timeScale = settings.SpeedUpRate;
        }

        public void ResetGameSpeed()
        {
            Time.timeScale = 1;
        }

        void StartUp()
        {
        }

        [Serializable]
        public class Settings
        {
            public float VisibleTimeToGameOver = 3;
        }
    }
}
