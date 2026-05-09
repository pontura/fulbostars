using Fulbo.Stadiums;
using Fulbo.UI.Carrousel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Fulbo.Stadiums.StadiumsData;

namespace Fulbo.UI
{
    public class GalaxyMapUI : MonoBehaviour
    {
        [SerializeField] Animation anim;

        public GalaxyMapButtonUI button;
        public List<GalaxyMapButtonUI> buttons;
        [SerializeField] CarrouselManager carrousel;
        [SerializeField] Transform container;
        [SerializeField] GameObject scene3d;
        [SerializeField] GameObject scene3d_planets;
        [SerializeField] ButtonCustom buttonToEnter;

        public void Init()
        {
            //CancelInvoke();
            //buttonToEnter.Init(0, OnEnterClicked);
            //scene3d.SetActive(true);
            //anim.Play("in");
            //Invoke("BackDelayed", 0.75f);

            //if (buttons.Count > 0)
            //    return;     
            //List<StadiumsData.StadiumData> all = StadiumsData.Instance.GetAll(Data.Instance.matchData.levelData.isCup);
            //int id = 1;
            //foreach (StadiumsData.StadiumData stadiumData in all)
            //{
            //    if (stadiumData.id >= 0) // skip tutorial:
            //    {
            //        GalaxyMapButtonUI newButton = Instantiate(button, container);
            //        newButton.Init(this, stadiumData);
            //        buttons.Add(newButton);
            //        carrousel.AddItem(id, newButton.gameObject, all.Count - 1);
            //        newButton.transform.localEulerAngles = new Vector3(0, 180, 0);
            //        id++;
            //    }
            //}
            //carrousel.OnInit();
        }
        void BackDelayed()
        {
            Data.Instance.ui.SetBackButton(true, Back);
        }
        void OnEnterClicked(int id)
        {
            int num = carrousel.id;
            if (num == 0) num = buttons.Count;

            GalaxyMapButtonUI buttonClicked = buttons[num - 1];

            if (buttonClicked.stadiumData.unavailable)
            {
                Events.OnPopup(Data.Instance.texts.Get("unavailableStadium"), null);
                return;
            }

            int stadiumID = buttonClicked.stadiumData.id;
            AreaClicked(stadiumID);
        }
        public void Back()
        {
            CancelInvoke();
            anim.Play("out");
            Events.Back();
            Invoke("Reset", 0.5f);
            scene3d.GetComponent<Animation>().Play("planetsOff");
        }

        private void Reset()
        {
            scene3d.SetActive(false);
            gameObject.SetActive(false);
        }

        public void AreaClicked(int stadiumID)
        {
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_storymode");
            Data.Instance.mode = Data.modes.STORYMODE;
            Data.Instance.LoadLevel("Levels");

            //HACER QUE DEPENDIENDO DE buttonUI.stadiumID SE CARGUE EL MAPA CORRECTO
            Data.Instance.matchData.SetActualStadium(stadiumID);

            //GetComponent<MainMenu>().Reset();
        }
    }
}