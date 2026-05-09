using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.Game;
using Fulbo.Game.States;

namespace Fulbo
{
    public class MatchStats : MonoBehaviour
    {
        public TeamStats[] teams;

        [Serializable]
        public class TeamStats
        {
            public int kicks_to_goal;
            public float ball_possesion;
            public int kicks_passes;
            public int balls_to_referi;
            public int centros;
            public int coins_grabbed;
            public int tackles;
            public int saves;
        }
        public int GetGoldWin(string key, int value)
        {
            switch (key)
            {
                case "kicks_to_goal": return value;
                case "ball_possesion": return value;
                case "kicks_passes": return value;
                case "balls_to_referi": return value;
                case "centros": return value;
                case "tackles": return value;
                default: return value;
            }
        }
        void Start()
        {
            Reset();
            Events.OnBallKicked += OnBallKicked;
            Events.OnHitReferi += OnHitReferi;
            Events.LoseBall += LoseBall;
            Events.CatchBall += CatchBall;
            Events.BallPassedTo += BallPassedTo;
            Events.OnGrab += OnGrab;
            Events.OnCatchBallWithDash += OnCatchBallWithDash;
            Events.OnBallHitCharacter += OnBallHitCharacter;
        }
        void OnDestroy()
        {
            Events.OnBallKicked -= OnBallKicked;
            Events.OnHitReferi -= OnHitReferi;
            Events.LoseBall -= LoseBall;
            Events.CatchBall -= CatchBall;
            Events.BallPassedTo -= BallPassedTo;
            Events.OnGrab -= OnGrab;
            Events.OnCatchBallWithDash -= OnCatchBallWithDash;
            Events.OnBallHitCharacter -= OnBallHitCharacter;
        }
        public void Reset()
        {
            getBallTimer = 0;
            foreach (TeamStats ts in teams)
            {
                ts.kicks_to_goal = 0;
                ts.ball_possesion = 0;
                ts.kicks_passes = 0;
                ts.balls_to_referi = 0;
                ts.centros = 0;
                ts.coins_grabbed = 0;
                ts.tackles = 0;
                ts.saves = 0;
            }
        }
        public TeamStats GetStats(int teamID)
        {
            if (teamID == 1)
                return teams[0];
            else if (teamID == 2) return teams[1];
            return null;
        }
        void OnGrab(Grabbable grabbable)
        {
            if (grabbable.type == Grabbable.types.GOLD)
            {
                TeamStats teamStates = GetStats(2);
                teamStates.coins_grabbed++;
            }
        }
        void BallPassedTo(Character character)
        {
            if (character == null || character.teamID == 0) return;
            TeamStats teamStates = GetStats(character.teamID);
            teamStates.kicks_passes++;
        }
        void OnBallKicked(CharacterStates.kickTypes type, float force, Character character)
        {
            if (character == null || character.teamID == 0) return;
            TeamStats teamStates = GetStats(character.teamID);
            switch (type)
            {
                case CharacterStates.kickTypes.CENTRO:
                    teamStates.centros++;
                    break;
                case CharacterStates.kickTypes.HARD:
                case CharacterStates.kickTypes.KICK_TO_GOAL:
                    if (character.teamID == 1 && character.transform.position.x < 0
                        ||
                       character.teamID == 2 && character.transform.position.x > 0)
                        teamStates.kicks_to_goal++;
                    break;
            }
            LoseBall(character); // RESETEA la tenencia de pelota:
        }
        void OnHitReferi(Character character)
        {
            if (character == null || character.teamID == 0) return;
            TeamStats teamStates = GetStats(character.teamID);
            teamStates.balls_to_referi++;
        }


        /// catch ball
        float getBallTimer = 0;
        void LoseBall(Character character)
        {
            if (character == null || character.teamID == 0) return;
            if (getBallTimer == 0) return;
            TeamStats teamStates = GetStats(character.teamID);
            float duration = Time.time - getBallTimer;
            teamStates.ball_possesion += duration;
            getBallTimer = 0;
        }
        void CatchBall(Character character)
        {
            getBallTimer = Time.time;
            CheckConsiderSave(character);
        }
        void OnBallHitCharacter(Character character)
        {
            CheckConsiderSave(character);
        }
        void CheckConsiderSave(Character character)
        {
            if (GameManager.Instance.ball.characterThatKicked == null) return;
            if (GameManager.Instance.ball.characterThatKicked.teamID == character.teamID) return;
            if (character.type == Character.types.GOALKEEPER)
                GetStats(character.teamID).saves++;
        }
        void OnCatchBallWithDash(Character character)
        {
            if(character.states.currentState.type == CharacterStates.types.DASH)
            {
                TeamStats teamStates = GetStats(character.teamID);
                teamStates.tackles++;
            }
        }
    }
}