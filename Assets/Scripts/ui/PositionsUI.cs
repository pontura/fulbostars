using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class PositionsUI : MonoBehaviour
    {
        public PositionsUIManager team1_manager;
        public PositionsUIManager team2_manager;
        public int positionID_team1;
        public int positionID_team2;
        public Image team1Escudo;
        public Image team2Escudo;

        void Start()
        {
            Events.OnRight += OnRight;
            // Events.OnButtonClick += OnButtonClick;

            int team1 = (int)Data.Instance.settings.selectedTeams[0];
            int team2 = (int)Data.Instance.settings.selectedTeams[1];

            //team1Escudo.sprite = Data.Instance.settings.teamSettings[team1].escudo;
            //team2Escudo.sprite = Data.Instance.settings.teamSettings[team2].escudo;

            Init(1);
            Init(2);

            Events.OnSkipOn(Go, "skip");
        }
        private void OnDestroy()
        {
            Events.OnRight -= OnRight;
            // Events.OnButtonClick -= OnButtonClick;
        }
        //void OnButtonClick(int a, int b)
        //{
        //    Go();
        //}
        void Init(int teamID)
        {
            if (teamID == 1)
                team1_manager.Init(Data.Instance.charactersPositions.team1.all[positionID_team1], Data.Instance.matchData.GetTeam(1), 1);
            else
                team2_manager.Init(Data.Instance.charactersPositions.team2.all[positionID_team2], Data.Instance.matchData.GetTeam(2), 2);
        }
        void OnRight(int characterID, bool isRight)
        {
            int teamID = Data.Instance.matchData.players[characterID - 1];
            if (teamID == 0)
                return;
            if (teamID == 1)
            {
                if (isRight)
                    positionID_team1++;
                else
                    positionID_team1--;
                if (positionID_team1 > Data.Instance.charactersPositions.team1.all.Length - 1)
                    positionID_team1 = 0;
                else if (positionID_team1 < 0)
                    positionID_team1 = Data.Instance.charactersPositions.team2.all.Length - 1;
                Init(1);
            }
            else if (teamID == 2)
            {
                if (isRight)
                    positionID_team2++;
                else
                    positionID_team2--;
                if (positionID_team2 > Data.Instance.charactersPositions.team1.all.Length - 1)
                    positionID_team2 = 0;
                else if (positionID_team2 < 0)
                    positionID_team2 = Data.Instance.charactersPositions.team2.all.Length - 1;
                Init(2);
            }
        }
        void Go()
        {
           // Events.OnSkipOff();
           // Data.Instance.matchData.SetCharactersPositions(positionID_team1, positionID_team2);
           // Data.Instance.LoadLevel("GameIntro");
        }
    }

}