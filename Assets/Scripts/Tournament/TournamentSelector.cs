using Fulbo.Game;
using Fulbo.Stadiums;
using Fulbo.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Tournamets
{
    public class TournamentSelector : MonoBehaviour
    {
        public MultipleCharactersScene multipleCharactersScene;
        public TournamentButton[] tournamentButtons;
        public CharacterCardInGame[] cards;
        public Text[] teams;
        public Text[] goles;
        public Text gamesPlayedField;

        int teamID = 0;

        void Start()
        {
            SetResults();
            Data.Instance.tournamentsData.Refresh(OnDataRefreshed);
            StadiumsData.Instance.SetRandomStadium();
            Data.Instance.matchData.SetTotalPlayers(8, 8);
            List<LevelData> teams = CupsData.Instance.levels.GetByState("torneo");

            print("teamsCount " + teams.Count);
            
            CupsData.Instance.levels.InitTournament();

            cards[0].ForceShow(CharactersData.Instance.GetCharacterData(Data.Instance.matchData.team1[1], false), 10000);
            cards[1].ForceShow(CharactersData.Instance.GetCharacterData(Data.Instance.matchData.team2[1], false), 10000);

            teams[0].name = teams[0].name;
            teams[1].name = teams[1].name;

            multipleCharactersScene.TounrnamentMode();

            Events.OnRight += OnRight; 

            tournamentButtons[0].SetOn(false);
            tournamentButtons[1].SetOn(false);
        }
        void SetResults()
        {
            goles[0].text = Data.Instance.tournamentsData.goles1.ToString();
            goles[1].text = Data.Instance.tournamentsData.goles2.ToString();
            gamesPlayedField.text = Data.Instance.tournamentsData.gamesPlayed.ToString();
        }
        private void OnDataRefreshed()
        {
            SetResults();
        }

        public void OnDestroyed()
        {
            Events.OnRight -= OnRight;
        }
        bool canSelect = false;
        void OnRight(int playerID, bool isRight)
        {
            if (!canSelect)
            {
                Events.OnSkipOn(Done, "skip");
                canSelect = true;
            }
            if (isRight)
                SelectTeam(2);
            else 
                SelectTeam(1);
        }
        void SelectTeam(int teamID)
        {
            this.teamID = teamID;
            tournamentButtons[0].SetOn(false);
            tournamentButtons[1].SetOn(false);
            tournamentButtons[teamID-1].SetOn(true);
            Data.Instance.tournamentsData.SetTeam(teamID);
            Data.Instance.matchData.team1Controlled = teamID == 2;
            Data.Instance.matchData.team2Controlled = teamID == 1;
            if(teamID == 1)
                Data.Instance.matchData.players[0] = 2;
            else
                Data.Instance.matchData.players[0] = 1;
        }
        void Done()
        {
            if (teamID == 0)
                return;
            Events.OnSkipOff();
            Data.Instance.LoadLevel("Controls");
            Data.Instance.matchData.SetTotalPlayers(8, 8);
        }
    }
}
