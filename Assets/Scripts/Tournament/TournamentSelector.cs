using Fulbo.Game;
using Fulbo.Stadiums;
using Fulbo.UI;
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

        int teamID = 0;

        void Start()
        {
            StadiumsData.Instance.SetRandomStadium();
            Data.Instance.matchData.SetTotalPlayers(8, 8);
            List<LevelData> teams = CupsData.Instance.levels.GetByState("torneo");

            print("teamsCount " + teams.Count);
            
            CupsData.Instance.levels.InitTournament();


            Data.Instance.partyModeData.SetTeamID(1, teams[0].id);
            Data.Instance.partyModeData.SetTeamID(2, teams[1].id);

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
        public void OnDestroyed()
        {
            Events.OnRight -= OnRight;
        }
        void OnRight(int playerID, bool isRight)
        {
            if (isRight)
                SelectTeam(1);
            else if(!isRight)
                SelectTeam(2);
        }
        void SelectTeam(int teamID)
        {
            this.teamID = teamID;
            tournamentButtons[0].SetOn(false);
            tournamentButtons[1].SetOn(false);
            tournamentButtons[teamID-1].SetOn(true);
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
