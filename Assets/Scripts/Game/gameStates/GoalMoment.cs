using Fulbo.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class GoalMoment
    {
        public states state;
        public enum states
        {
            IDLE,
            GOING_TO_TARGET,
            ON_GOAL
        }
        bool isDone;
        Character character_made_goal;
        List<Character> winners;
        GameState gameState;

        public IEnumerator Init(GameState gameState, int teamID, Character character)
        {
            Character goalkeeper;
            if (Fulbo.Game.GameManager.Instance.ball.transform.position.x < 0)
                goalkeeper = Fulbo.Game.GameManager.Instance.charactersManager.team2[0];
            else goalkeeper = Fulbo.Game.GameManager.Instance.charactersManager.team1[0];

            GameManager.Instance.cameraInGame.SetTargetTo(goalkeeper.transform);

            this.gameState = gameState;
            CharactersManager charactersManager = Fulbo.Game.GameManager.Instance.charactersManager;
            List<Character> winner;
            List<Character> loser;
            if (teamID == 1)
            {
                winner = charactersManager.team1;
                loser = charactersManager.team2;
            }
            else
            {
                winner = charactersManager.team2;
                loser = charactersManager.team1;
            }
            if (teamID == 1 && Data.Instance.isMobile)
                Events.OnGoldScoreWin(10, Vector3.zero);

            /* ALL TEAMMATES CHEER
            foreach (Character ch in winner)
                Events.SetDialogue(ch, Data.Instance.textsData.GetRandomDialogue("goal", ch.data.id, ch.type == Character.types.GOALKEEPER));
            */

            character_made_goal = character;

            // HACER QUE SHUFFLEE ENTRE LOS PERSONAJETAS
            Events.SetDialogue(character_made_goal, Data.Instance.textsData.GetRandomDialogue("goal", character_made_goal.data.id, character_made_goal.type == Character.types.GOALKEEPER));

            if (teamID == 1)
                winners = charactersManager.team1;
            else
                winners = charactersManager.team2;

            AudioManager.Instance.ChangeVolume("crowd", 1);
            Events.OnGoal(teamID, character);
            Fulbo.Game.GameManager.Instance.cameraInGame.OnGoal(character);
            //AudioManager.Instance.PlaySound("crowd", "_new/ambience/crowd_gol", false);
            AudioManager.Instance.PlayCrowd(Fulbo.Game.GameManager.Instance.stadiumData.active.crowd_gol);

            yield return new WaitForSeconds(0.5f);
            state = states.GOING_TO_TARGET;

            yield return new WaitForSeconds(0.3f);

           

            foreach (Character ch in winner)
                ch.OnGoal(true);
            foreach (Character ch in loser)
                ch.OnGoal(false);

            Events.OnSkipOn(gameState.OnGoalDone, "skip");

            yield return new WaitForSeconds(2f);
            if (Data.Instance.matchData.IsTutorial())
                OnDone();

            yield return new WaitForSeconds(1.7f);
            state = states.ON_GOAL;
            yield return new WaitForSeconds(1);
            character_made_goal.states.Goal();

            yield return new WaitForSeconds(2);

        }
        public void OnDone()
        {
            if (Data.Instance.matchData.IsTutorial())
            {
                Data.Instance.matchData.GameOver();
                return;
            }

            gameState.StopAllCoroutines();

            AudioManager.Instance.ChangeVolume("crowd", 0.5f);
            state = states.IDLE;

            Events.OnSkipOff();
            Fulbo.Game.GameManager.Instance.ball.Reset();
            Fulbo.Game.GameManager.Instance.charactersManager.ResetAll();
            state = states.IDLE;
            Fulbo.Game.GameManager.Instance.StartCoroutine(Fulbo.Game.GameManager.Instance.OnWaitToStart());
            winners = null;
            isDone = false;
            UIMain.Instance.OnShow();
        }
        public IEnumerator UpdateC()
        {
            while (!isDone)
            {
                Updated();
                yield return new WaitForEndOfFrame();
            }
        }
        void Updated()
        {
            if (state == states.IDLE) return;
            int id = 0;
            foreach (Character character in winners)
            {
                id++;
                Vector3 targetPos;
                if (character.data.id == character_made_goal.data.id)
                    targetPos = new Vector3(character.transform.position.x, 0, 10);
                else
                {
                    Vector3 offset = new Vector3(0, 0, 0);
                    if(id %2 == 0)
                        offset = new Vector3(-id/2, 0, Random.Range(0.5f,3));
                    else
                        offset = new Vector3(Random.Range(-0.5f, -2), 0, -id / 2);

                    targetPos = character_made_goal.transform.position + offset;
                }

                if (Vector3.Distance(character.transform.position, targetPos) > 3)
                {
                    int _x = 0;
                    int _z = 0;
                    Vector3 pos = character.transform.position;
                    if (Mathf.Abs(pos.x - targetPos.x) > 0.25f)
                    {
                        if (pos.x < targetPos.x) _x = 1;
                        else if (pos.x > targetPos.x) _x = -1;
                    }
                    if (pos.z < targetPos.z) _z = 1;
                    else if (pos.z > targetPos.z) _z = -1;
                    if (_x == 0 && _z == 0)
                        character.states.Goal();
                    else
                        character.MoveTo(_x, _z);
                }
                else
                {
                    if(character.states.currentState.type != CharacterStates.types.GOAL)
                        character.states.Goal();
                }
            }
        }
    }
}
