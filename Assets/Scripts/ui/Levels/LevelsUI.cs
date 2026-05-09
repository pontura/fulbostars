using Fulbo.Stadiums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Fulbo.UI
{
    public class LevelsUI : CascadeList
    {
        [SerializeField] Transform stadiumContainer;
        [SerializeField] Transform container;
        [SerializeField] LevelsButton button;
       // [SerializeField] Image image;
        [SerializeField] Scrollbar scrollBar;
        StadiumsData stadiumsData;
        List<LevelsButton> buttons;

        //private void Start()
        //{
        //    Events.OnBoardingPanelAction += OnBoardingPanelAction;
        //}
        //private void OnDestroy()
        //{
        //    Events.OnBoardingPanelAction -= OnBoardingPanelAction;
        //}
        //void OnBoardingPanelAction(string actionName, int id)
        //{
        //    if (actionName == "levels" && id == 1)// opens the level popup
        //    {
        //        GetComponent<MyTeamSelector>().Go();
        //    }
        //}
        public void Init()
        {
            buttons = new List<LevelsButton>();
            //image.enabled = false;
            Utils.RemoveAllChildsIn(stadiumContainer);

            stadiumsData = StadiumsData.Instance;
            GameObject asset = stadiumsData.active.GetAssetBySelectedSize().asset;
            Instantiate(asset, stadiumContainer);
            asset.transform.localPosition = Vector3.zero;

            OnLevelSelected(stadiumsData.active.id);
        }
        void OnLevelSelected(int stadiumID)
        {
            buttons.Clear();
            scrollBar.value = 1;
            int id = 1;
            Utils.RemoveAllChildsIn(container);
            InitCascade();
            StartCascade();
        }
        public void Clicked(int id, LevelsButton levelsButton)
        {
            GetComponent<PopupLevelUI>().Init(id, levelsButton);
        }
        public Vector2 GetMinMAxStats()
        {
            if (buttons.Count < 1) return Vector2.zero;
            LevelsButton b1 = buttons[0];
            LevelsButton b2 = buttons[buttons.Count - 1];
            return new Vector2(b1.stats, b2.stats);
        }
    }
}
