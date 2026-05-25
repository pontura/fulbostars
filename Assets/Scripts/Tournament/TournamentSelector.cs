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
            Events.OnSkipOn(Done, "skip");

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
        void OnRight(int playerID, bool isRight)
        {
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
        }
        void Done()
        {
            if (teamID == 0)
                return;
            Events.OnSkipOff();
            Data.Instance.LoadLevel("TeamSelector");
        }
    }
}
