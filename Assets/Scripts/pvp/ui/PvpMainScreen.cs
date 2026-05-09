using UnityEngine;
using System.Collections;
using Fulbo.Game;
using Fulbo.Stadiums;
using UnityEngine.UI;

namespace Fulbo.UI.Pvp
{
    public class PvpMainScreen : MonoBehaviour
    {
        [SerializeField] Animation anim;
        [SerializeField] ButtonCustom randomPlayBtn;
        [SerializeField] InputField emailInputDebug;

        private void Start()
        {
            randomPlayBtn.Init(0, Clicked, Data.Instance.texts.Get("randomPlayBtn"));
            emailInputDebug.text = "brenburgoa@gmail.com";
        }
        void Clicked(int id)
        {
            switch(id)
            {
                case 0:
                    Data.Instance.pvpData.Load(OnDataLoaded, emailInputDebug.text);
                    break;
            }
        }
        void OnDataLoaded()
        {
            if (Data.Instance.pvpData.data == null
                || Data.Instance.pvpData.data.user == null)
                return;

            //Data.Instance.mode = Data.modes.PVP;
            //Data.Instance.LoadLevel("Levels");
            ////TO-DO:
            //StadiumsData.Instance.SetActiveStadium(0, "small");
            //Data.Instance.matchData.SetActualLevel(1);
            //
        }
        public void Init()
        {
            anim.Play("in");
            Invoke("BackDelayed", 0.75f);
        }
        public void Back()
        {
            CancelInvoke();
            anim.Play("out");
            Events.Back();
            Invoke("Reset", 0.5f);
        }
        void BackDelayed()
        {
            Data.Instance.ui.SetBackButton(true, Back);
        }
        private void Reset()
        {
            gameObject.SetActive(false);
        }
    }
}
