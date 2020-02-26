using Zenject;
using UnityEngine;
using System.Collections.Generic;
using DarkTonic.MasterAudio;

namespace GFG
{
    public class GameView : MonoBehaviour
    {
        public List<GameObject> LevelPrefabs;
		public Animator Animator;
        public GameObject GameContainer;
        public GameObject TitleScreen;
		public GameObject VisibleIndicator;
        public GameObject EndGameExitButton;
        public GameObject CinematicPanel;

        SignalBus signalBus;
		DiContainer diContainer;
        GameObject LevelInstance;
        

        [Inject]
		public void Construct(SignalBus _signalBus, DiContainer _diContainer)
        {
            signalBus = _signalBus;
			diContainer = _diContainer;
		}

        private void Awake()
        {
            MasterAudio.PlaySound("Ambience_lvl1");
        }

        public void LoadLevel(int id = 0)
        {
            if (LevelInstance != null)
            {
                Destroy(LevelInstance);
            }

            LevelInstance = diContainer.InstantiatePrefab(LevelPrefabs[id], GameContainer.transform);

            ShowTitleScreen(false);
			IndieMarc.Platformer.PlatformerCharacter.UnlockGameplay();

            if (id == 0 )
            {
                MasterAudio.FadeBusToVolume("Cine_BG", 0, 4);
                MasterAudio.PlaySound("Ambience_lvl1");
            }

            if (id == 1)
            {
                MasterAudio.StopBus("Lvl1");
                MasterAudio.PlaySound("Ambience_lvl2");
            }
        }

        public void IntroDoorThud()
        {
            MasterAudio.PlaySound("Sister_Doors");
        }

        public void EndAudio()
        {
            MasterAudio.PlaySound("EndAudio");
        }

        public void P4Foots()
        {
            MasterAudio.PlaySound("Sister_2nd_Foots");
        }

        public void Thud()
        {
            MasterAudio.PlaySound("Sister_Thud");
        }

        public void Cine_BG()
        {
            MasterAudio.StopEverything();

            MasterAudio.PlaySound("Cine_BG");
        }

        public void IntroWalks()
        {
            MasterAudio.PlaySound("Sister_Foots_Intro");
        }
        public void PlayIntroCinematic()
		{
            IndieMarc.Platformer.PlatformerCharacter.LockGameplay();
			Animator.SetBool("intro", true);
		}

        public void IntroCinematicComplete()
		{
			Animator.SetBool("intro", false);
			LoadLevel(0);
		}

		public void PlayCaptureCinematic()
		{
			IndieMarc.Platformer.PlatformerCharacter.LockGameplay();
			Animator.SetBool("capture", true);
		}

		public void CaptureCinematicComplete()
		{
			Animator.SetBool("capture", false);
			LoadLevel(1);
		}

		public void PlayLossCinematic()
		{
            if (LevelInstance != null)
            {
                Destroy(LevelInstance);
            }

            IndieMarc.Platformer.PlatformerCharacter.LockGameplay();
			Animator.SetBool("loss", true);
		}

		public void LossCinematicComplete()
		{
			Animator.SetBool("loss", false);
			ShowTitleScreen();
		}

		public void PlayEndCinematic()
		{
			IndieMarc.Platformer.PlatformerCharacter.LockGameplay();
			Animator.SetBool("end", true);
		}

        public void PlayerSpotted(bool visible)
		{
			VisibleIndicator.SetActive(visible);
		}

		public void EndCinematicComplete()
		{
            MasterAudio.FadeBusToVolume("End", 0, 3);
            EndGameExitButton.SetActive(false);
            CinematicPanel.SetActive(false);

            Animator.SetBool("end", false);
			ShowTitleScreen();

            Application.Quit();
        }

		public void ShowTitleScreen(bool show = true)
        {
            /*if (LevelInstance != null)
            {
                Destroy(LevelInstance);
            }*/

            MasterAudio.StopEverything();
            MasterAudio.PlaySound("Ambience_lvl1");

            TitleScreen.SetActive(show);
        }
    }
}
