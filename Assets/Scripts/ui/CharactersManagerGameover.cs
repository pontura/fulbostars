using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class CharactersManagerGameover : CharactersManager
    {
        public int scoreWinned;

        public override void Init(bool reverseTeams = false, bool isBillboard = false)
        {
            SetScore();

            CharactersConstructor cc = GetComponent<CharactersConstructor>();
            if (cc != null) cc.AddCharacters();


            ball = Fulbo.Game.GameManager.Instance.ball;
            referi.InitReferi(this, CharactersData.Instance.GetReferi().asset);

            if (reverseTeams)
            {
                winnerTeam = team2;
                if(Data.Instance.mode == Data.modes.PARTYMODE)
                    SetCharactersToTeam(2, false, true, true);
                else
                    SetCharactersToTeam(2, true, true, true);
            }
            else
            {
                winnerTeam = team1;
                SetCharactersToTeam(1, false, true, false);
            }

            AudioManager.Instance.PlaySpecificSound(Fulbo.Stadiums.StadiumsData.Instance.active.ambience_end_loop, "ambience", true);
        }
        void SetScore()
        {
            scoreWinned = 5;
            if (Data.Instance.matchData.score.x > Data.Instance.matchData.score.y)
            {
                scoreWinned += (int)Data.Instance.matchData.score.x - (int)Data.Instance.matchData.score.y;
                scoreWinned *= 10;
            }
            Events.OnGoldScoreWin(scoreWinned, Vector3.zero);
        }
    }

}