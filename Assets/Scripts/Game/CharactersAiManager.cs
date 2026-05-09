using UnityEngine;
using System.Collections;
using Fulbo.Game.AIs;

namespace Fulbo.Game
{
    public class CharactersAiManager
    {
        CharactersManager charactersManager;
        Ball ball;

        public void Init(CharactersManager charactersManager)
        {
            this.charactersManager = charactersManager;
        }
        public void Loop()
        {
            if(ball == null)
                ball = charactersManager.ball;
            if (ball == null)
                return;
            
            CheckNewDefenderByTeam(1);
            CheckNewDefenderByTeam(2);
        }
        void CheckNewDefenderByTeam(int teamID)
        {
            if (ball.character == null || (ball.character != null && ball.character.teamID != teamID))
                CheckForNewDefender(teamID);
        }

        void CheckForNewDefender(int teamID)
        {
            float offset = 2;
            Vector3 ballPos = ball.transform.position;
            if (teamID == 1) ballPos.x += offset; else ballPos.x -= offset;

            Character nearestToDefend = charactersManager.GetNearest(teamID, false, ballPos, false, true, true);
            if (nearestToDefend == null) return;
            if (nearestToDefend.ai.currentState is AiGotoBall)
                return;
            if (ball.character != null && ball.character.type == Character.types.GOALKEEPER)
                return;

            Events.SetCharacterNewDefender(nearestToDefend);
        }
    }
}
