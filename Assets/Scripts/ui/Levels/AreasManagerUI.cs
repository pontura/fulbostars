using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class AreasManagerUI : UIMainScreen
    {
        //[SerializeField] Transform container;
        //[SerializeField] Text stadiumField;
        //[SerializeField] Text difficultField;
        //[SerializeField] Text statsField;

        //[SerializeField] StoryModeLevelsUI storyModeLevelsUI;

        //[SerializeField] ButtonCustom nexButton;
        //[SerializeField] ButtonCustom prevButton;

        //[SerializeField] AreaButtonUI buttonToAdd;

        //List<AreaButtonUI> buttons;

        //int id = 1;
        //public void Init()
        //{
        //    int _id = 1;
        //    buttons = new List<AreaButtonUI>();
        //    Utils.RemoveAllChildsIn(container);
        //    if(Data.Instance.matchData.levelData.isCup)
        //    {
        //       // GetComponent<CupsScreen>().Init();
        //        return;
        //    }
        //    for (int a= 0; a<3; a++)
        //    {
        //        //AreaButtonUI button = Instantiate(buttonToAdd, container);
        //        //buttons.Add(button);
        //        //StoryModeData.StadiumData sd = StoryModeData.Instance.storyModeData[_id];
        //        //button.Init(this, sd, _id);
        //        //if (sd.stadium_id == Data.Instance.matchData.levelData.stadium_id)
        //        //{
        //        //    id = _id;
        //        //    AreaClicked(button);
        //        //}
        //        //_id++;
        //    }

        //    nexButton.Init(0, Next, Data.Instance.texts.Get("next"));
        //    prevButton.Init(1, Prev, Data.Instance.texts.Get("prev"));
        //}
        //void SetStats()
        //{
        //    Vector2 stats = GetComponent<LevelsUI>().GetMinMAxStats();
        //    statsField.text = "(" + stats.x + "-" + stats.y + ")";
        //}
        //public void AreaClicked(AreaButtonUI button)
        //{
        //    SetAreaButtonSelected(button);           
        //}
        //void SetAreaButtonSelected(AreaButtonUI button)
        //{
        //    storyModeLevelsUI.StadiumClicked(button.data);
        //    stadiumField.text = button.data.stadium_name;
        //    int difficultID = Stadiums.StadiumsData.Instance.active.difficulty_level;
        //    difficultField.text = Data.Instance.texts.Get("level_difficulty_" + difficultID);
        //    statsField.text = "";
        //    Invoke("SetStats", 0.1f);

        //    foreach (AreaButtonUI ab in buttons)
        //        ab.OnSelected(false);
        //    button.OnSelected(true);
        //}
        //void Next(int a)
        //{
        //    id++;
        //    if (id > buttons.Count)
        //        id = 1;
        //    SetAreaButtonSelected(buttons[id - 1]);
        //}
        //void Prev(int a)
        //{
        //    id--;
        //    if (id < 1)
        //        id = buttons.Count;
        //    SetAreaButtonSelected(buttons[id - 1]);

        //}
    }
}