using Fulbo.Stadiums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.UI
{
    public class Splash : MonoBehaviour
    {
        void Start()
        {
            print("SPLASK");
            PlayMusicIntro();
            Events.OnButtonClick += OnButtonClick;
        }
        void OnDestroy()
        {
            Events.OnButtonClick -= OnButtonClick;
        }
        public void PlayMusicIntro()
        {
            AudioManager.Instance.Play2Musics("music/intro", "music/music");
        }
        void OnButtonClick(int buttonID, int playerID)
        {
            print("OnButtonClick");
            GotoGame();
        }
        void GotoGame()
        {
            print("GotoGame");
            AudioManager.Instance.FadeVolume("music", 0.3f);
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_play_now");

            if (Data.Instance.mode == Data.modes.PARTYMODE)
            {
                if (Data.Instance.settings.mainSettings.isArcade) // multiplayer:
                    Data.Instance.LoadLevel("Controls");
                else
                    Data.Instance.LoadLevel("TeamSelector"); // Ruleta
            }
        }
    }
}
