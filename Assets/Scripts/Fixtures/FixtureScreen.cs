using System;
using System.Collections.Generic;
using Fulbo.UI;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace Fulbo.Fixture
{
    public class FixtureScreen : MonoBehaviour
    {
        List<FixtureTeamButton> all;

        [SerializeField] List<Button> menu;
        [SerializeField] FixtureTeamButton fixtureTeamButton;
        [SerializeField] Transform container;
        [SerializeField] Scrollbar scrollBar;
        [SerializeField] Text field;
        [SerializeField] Text teamList;
        [SerializeField] int totalPlayers;
        string allTeams;

        [SerializeField] int id = 0;
        [SerializeField] int menuID = 0;
        [SerializeField] bool leftMenu;

        void Start()
        {
            Events.OnButtonClick += OnButtonClick;
            Events.OnUp += OnUp;
            Events.OnRight += OnRight;
            Init();
            teamList.supportRichText = true;
        }
        void OnDestroy()
        {
              Events.OnUp -= OnUp;
              Events.OnRight -= OnRight;
              Events.OnButtonClick -= OnButtonClick;
        }

        private void OnButtonClick(int arg1, int arg2)
        {
              if(!leftMenu)
              {
                
                if(!all[id].isOn && totalPlayers>7) return;

                all[id].OnClicked();

                if(all[id].isOn)
                    totalPlayers++;
                else
                    totalPlayers--;

                SetTotal();
              }
            else
            {
                switch(menuID)
                {
                    case 0:
                        StartFixture(); break;
                    default:
                        Data.Instance.Back(); break;
                }
            }
        }

        private void StartFixture()
        {
            List<LevelData> selectedTeams= new List<LevelData>();
            foreach(FixtureTeamButton b in all)
            {
                if(b.isOn)
                {
                    selectedTeams.Add(b.levelData); 
                }             
            }
            Data.Instance.StartFixture(selectedTeams);
        }

        void SetTotal()
        {
            field.text = totalPlayers.ToString();
            allTeams = "";
            int num = 1;

            foreach(FixtureTeamButton b in all)
            {
                if(b.isOn)
                {
                    allTeams += num + "- " + b.levelData.name +"\n";
                    num++;   
                }             
            }

            teamList.text = allTeams;
        }
        void Init()
        {
            all = new List<FixtureTeamButton>();
            Utils.RemoveAllChildsIn(container);
            foreach(LevelData lData in CupsData.Instance.levels.GetByState("on"))
            {
                CupsData.Instance.levels.SetOponents(lData);
                AddButton(lData);
            }
            all[id].RollOver();
        }
        void AddButton(LevelData lData)
        {
            FixtureTeamButton b = Instantiate(fixtureTeamButton, container);
            b.Init(lData);
            b.SetOn(false);
            all.Add(b);
        }
        private void OnRight(int a, bool right)
        {
            leftMenu = !leftMenu;
            if(leftMenu)
            {
                foreach(FixtureTeamButton b in all)
                    b.RollOut();
                menuID = 0;
                SetMenuButton();
            }
            else
            {
               foreach(Button b in menu)
                {
                    b.GetComponent<Animator>().SetBool("isOn", false);
                }
                all[id].RollOver();
            }
        }
        private void OnUp(int a, bool up)
        {
            if(!leftMenu){
                print("up" + up);
                if(!up)   id++; else id--;
                if(id<0) id = 0;
                else if(id>all.Count-1) id = all.Count-1;
                SetSelected();
            }
            else
            {
                 if(!up)   menuID++; else menuID--;
                 if(menuID<0)menuID = 1; else if(menuID>menu.Count-1) menuID = 0;
                SetMenuButton();
            }
        }
        float gotoNum = 1;
        void SetSelected()
        {
            foreach(FixtureTeamButton b in all)
                b.RollOut();
            all[id].RollOver();

            gotoNum = 1-(((float)(id-1))/(float)(all.Count-3));
            if(id <3) gotoNum = 1;
            if(id > all.Count-2) gotoNum = 0;

        }
        void Update()
        {
            scrollBar.value = Mathf.Lerp(scrollBar.value , gotoNum, 0.05f);
        }
        void SetMenuButton()
        {
            int a = 0;
            foreach(Button b in menu)
            {
                b.GetComponent<Animator>().SetBool("isOn", a == menuID);
                a++;
            }
        }
    }
}
