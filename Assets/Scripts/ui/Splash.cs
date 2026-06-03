using System.Linq;
using UnityEngine;

namespace Fulbo.UI
{
    public class Splash : MonoBehaviour
    {
        [SerializeField] Animator[] buttons;

        int var = 0;
        void Start()
        {
            Events.OnBackActive(false);
            print("SPLASK");
            PlayMusicIntro();
            Events.OnButtonClick += OnButtonClick;
            Events.OnUp += OnUp; 
            Select();
        }
        void OnDestroy()
        {
            Events.OnButtonClick -= OnButtonClick;
            Events.OnUp -= OnUp; 
        } 
        void OnUp(int playerID, bool a)
        {
            if (a)
                var--;
            else var++;

            if (var < 0) var = buttons.Length-1;
            if (var > 2) var = 0;
            Select();
        }
        void Select()
        {
            if(buttons != null &&buttons.Length > 0)
            {
                buttons[0].SetBool("isOn", var == 0);
                buttons[1].SetBool("isOn", var == 1);
                buttons[2].SetBool("isOn", var == 2);
            }
        }
        public void PlayMusicIntro()
        {
            AudioManager.Instance.Play2Musics("music/intro", "music/music");
        }
        void OnButtonClick(int buttonID, int playerID)
        {
            Events.OnButtonClick -= OnButtonClick;
            GotoGame();
        }
        void GotoGame()
        {
           
            print("GotoGame");
            AudioManager.Instance.FadeVolume("music", 0.3f);
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_play_now");

             if(Data.Instance.settings.mainSettings.isArcade)
                Data.Instance.LoadLevel("PlayersTeamSelector");

            else if(var == 0)//tournamemt:
            {
                Data.Instance.InitTournament();
            } else  if(var == 1) //friendly
            {                
                Data.Instance.tournamentsData.SetTournament(false);
                #if UNITY_STANDALONE
                    Data.Instance.LoadLevel("PlayersTeamSelector");
                    return;
                    #endif
                if(Data.Instance.webGLGamepadFix.playersQty > 1) // si hay más de un joystick
                    Data.Instance.LoadLevel("PlayersTeamSelector");
                else
                    Data.Instance.LoadLevel("TeamSelector");
            } else
            {                
                Data.Instance.LoadLevel("Controls");
            }
        } 
    }
}
