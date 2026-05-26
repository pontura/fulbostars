using UnityEngine;

namespace Fulbo.UI
{
    public class Splash : MonoBehaviour
    {
        [SerializeField] Animator[] buttons;

        int var = 1;
        void Start()
        {
            print("SPLASK");
            PlayMusicIntro();
            Events.OnButtonClick += OnButtonClick;
            Events.OnUp += OnUp; 
            Select(1);
        }
        void OnDestroy()
        {
            Events.OnButtonClick -= OnButtonClick;
            Events.OnUp -= OnUp; 
        } 
        void OnUp(int playerID, bool a)
        {
            if (a)
                Select(1);
            else 
                Select(2);
        }
        void Select(int _var)
        {
            var = _var;
            if(buttons.Length > 0)
            {
                buttons[0].SetBool("isOn", var == 1);
                buttons[1].SetBool("isOn", var == 2);
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

            if(var == 1)//tournamemt:
            {
                Data.Instance.InitTournament();
            } else//friendly
            {                
                Data.Instance.tournamentsData.SetTournament(false);
                Data.Instance.LoadLevel("TeamSelector");
            }
        }
    }
}
