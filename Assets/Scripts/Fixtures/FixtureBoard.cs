using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using Fulbo.UI;
using Microsoft.SqlServer.Server;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Fixture
{
    public class FixtureBoard : MonoBehaviour
    {
         List<LevelData> teams ;
        [SerializeField] List<Button> menu;

        [SerializeField] List<FixtureTeam> round1;
        [SerializeField] List<FixtureTeam> round2;
        [SerializeField] List<FixtureTeam> round3;
        [SerializeField] List<FixtureTeam> round4;

        [SerializeField] int rooundID = 0;
        [SerializeField] int gameID = 0;
        [SerializeField] bool finish;
        void Start()
        {
            Events.OnBackActive(false);
            Events.OnButtonClick += OnButtonClick;
            Events.OnUp += OnUp;
            Init();
            SetMenuButton();
            SetMenu();
        }
        void SetMenu()
        {
             menu[0].GetComponentInChildren<TMPro.TMP_Text>().text = "JUGAR!";
             menu[1].GetComponentInChildren<TMPro.TMP_Text>().text = "SALIR";
        }
        void OnDestroy()
        {
              Events.OnUp -= OnUp;
              Events.OnButtonClick -= OnButtonClick;
        }
        void OnSkip()
        {
             Events.OnSkipOff();
             Data.Instance.Back();
        }
        bool finishedClicked;
        private void OnButtonClick(int arg1, int arg2)
        {           
            if(finish)
            {
                if(!finishedClicked)
                    Events.OnSkipOn(OnSkip, "VOLVER");
                finishedClicked = true;
                OnDestroy();
                return;
            }
            switch(menuID)
            {
                case 0:
                    StartPlaying(); break;
                default:
                    Events.OnConfirmPanel("TERMINAR TORNEO", "SEGURO QUE QUERÉS CERRAR EL TORNEO?", OnConfirmExit, "TERMINAR", "NO");
                    break;
            }
        }

        private void OnConfirmExit(bool ok)
        {
            if(ok)Data.Instance.Back(); 
        }

        private void StartPlaying()
        {
            Data.Instance.PlayNextFixture();
        }
        void Init()
        {
            SetState(round1,FixtureTeam.states.on);
            SetState(round2,FixtureTeam.states.off);
            SetState(round3,FixtureTeam.states.off);
            SetState(round4,FixtureTeam.states.off);
            teams = Data.Instance.fixtureManager.teams;
            int a = 0;
            foreach(LevelData l in teams)
            {
                round1[a].Init(l);
                a++;
            }
            a = 0; 
            int roundID = 0;
            int scoresID = 0;
            foreach(LevelData l in Data.Instance.fixtureManager.won)
            {
                
                print("a " + a + " round: " + roundID);

                Vector2 scores = Data.Instance.fixtureManager.scores[scoresID];
                scoresID++;
                List<FixtureTeam> round;

                

                if(a == 8 || a == 12)
                    roundID = 0;
                if(a<8)
                    round= round1;
                else if(a<12)
                    round= round2;
                else
                    round= round3;
                
                
                round[roundID].SetScore((int)scores[0]);
                round[roundID+1].SetScore((int)scores[1]);

                FixtureTeam win;
                FixtureTeam lose;

                if(l.team_tag == round[roundID].levelData.team_tag)
                {
                    win = round[roundID];
                    lose = round[roundID+1];
                }
                else
                {
                    win = round[roundID+1];
                    lose = round[roundID];                
                }
                win.SetState(FixtureTeam.states.win);   
                lose.SetState(FixtureTeam.states.lose);  
                if(a<8)
                {
                    int num = (int)(a/2);
                    print("win2____________" + num);
                    round2[num].Init(win.levelData); 
                    round2[num].SetState(FixtureTeam.states.on);
                } else if(a<12)
                { 
                    int num = (int)(a-8)/2;
                    print("win3____________" + num);
                    round3[num].Init(win.levelData); 
                    round3[num].SetState(FixtureTeam.states.on);
                }
                if(a == 12) // FINAL
                {
                    finish = true;
                    round4[0].Init(Data.Instance.fixtureManager.won[Data.Instance.fixtureManager.won.Count-1]); 
                    round4[0].SetState(FixtureTeam.states.win);
                }

                roundID += 2;        
                a += 2;
                
            }
            SetPlayers();
        }
        void SetPlayers()
        {
            int totalPlayed = Data.Instance.fixtureManager.won.Count*2;
            if(totalPlayed<8)
            {
                round1[totalPlayed].SetState(FixtureTeam.states.playing);                
                round1[totalPlayed+1].SetState(FixtureTeam.states.playing);
            } else if(totalPlayed<12)
            {
                round2[totalPlayed-8].SetState(FixtureTeam.states.playing);                
                round2[totalPlayed-8+1].SetState(FixtureTeam.states.playing);
            }else if(totalPlayed<14)
            {
                round3[0].SetState(FixtureTeam.states.playing);                
                round3[1].SetState(FixtureTeam.states.playing);
            }
        }
        void SetState(List<FixtureTeam> all, FixtureTeam.states state)
        {
            foreach(FixtureTeam f in all)
                f.SetState(state);
        }
        int menuID = 0;
        private void OnUp(int a, bool up)
        {
                if(!up)   menuID++; else menuID--;
                if(menuID<0)menuID = 1; else if(menuID>menu.Count-1) menuID = 0;
            SetMenuButton();
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
