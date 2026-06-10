using Fulbo.Stadiums;
using Fulbo.Tournamets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class PlayersTeamSelector : MonoBehaviour
    {
        public TournamentSelector tournamentSelector;
        public PlayerTeamSignalUI[] all;
        public Text field;

        void Start()
        {
            all = GetComponentsInChildren<PlayerTeamSignalUI>();
            int id = 1;
            foreach (PlayerTeamSignalUI p in all)
            {
                p.Init(id);
                id++;
            }
            Events.OnRight += OnRight;
            Events.OnSkipOn(Go, "skip");

            int team1 = (int)Data.Instance.settings.selectedTeams[0];
            int team2 = (int)Data.Instance.settings.selectedTeams[1];
        }
        private void OnDestroy()
        {
            Events.OnRight -= OnRight;
        }
        void OnRight(int playerID, bool isRight)
        {
            print("Right playerID: " + playerID + " isRight: " + isRight);
            int id = playerID - 1;
            
            if(all[id].teamID == 0 && isRight && ignoreRight) return;
            if(all[id].teamID == 0 && !isRight && ignoreLeft) return;

            all[id].MoveRight(isRight);
            SetTotalPlayers();
            if(tournamentSelector == null) return;
            if(isRight)
                tournamentSelector.SetTeamActive(1);
            else 
                tournamentSelector.SetTeamActive(2);
        }
        void Go()
        {
            Events.OnSkipOff();
            if (AnyActive())
            {
                if(Data.Instance.tournamentsData.IsTournament())
                    Data.Instance.LoadLevel("GameIntro");
                else
                    Data.Instance.LoadLevel("TeamSelector");
            }
            else
            {
                Events.OnSkipOn(Go, "continue");
            }
        }
        bool AnyActive()
        {
            bool oneActive = false;
            int id = 0;
            foreach (PlayerTeamSignalUI p in all)
            {
                if (p.teamID != 0)
                    oneActive = true;
                int teamID = p.teamID;
                if (teamID == 1) teamID = 2; else if (teamID == 2) teamID = 1;
                Data.Instance.matchData.AddPlayer(id + 1, teamID);
                id++;
            }
            if (oneActive)
                return true;
            return false;
        }
        void SetTotalPlayers()
        {
            int total = 0;
            foreach (PlayerTeamSignalUI p in all)
            {
                if (p.teamID != 0)
                    total++;
            }
            field.text = total + " JUGADOR/ES";
        }
        bool ignoreRight;
        bool ignoreLeft;
        public void IgnoreSize(bool team1, bool team2)
        {
            if(team2)ignoreRight = true;
            if(team1)ignoreLeft = true;
        }
    }
}
