using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Xtras
{
    public class RecordedEvents : RecordedItem
    {
        string lastEvent = "";
        public override void Updated(float now)
        {
            if (timeline.events.Count <= keyframeID - 1) return;
            GameRecorder.KeyFrame.KeyFrameEventData data = timeline.events[keyframeID];
            if (data != null && data.time < now)
            {
                keyframeID++;
                lastEvent = data.key;
                switch (data.key)
                {
                    case "ballCatched": CatchBall(data.characterID, data.teamID); break;
                    case "ballFree": FreeBall(); break;
                    case "goal":
                        Character character = charactersManager.GetCharactersByTeam(data.teamID, data.characterID);
                        GameManager.Instance.Goal(data.teamID, character);
                        ball.SetPhysics(true);
                        break;
                }
            }
        }
        void CatchBall(int characterID, int teamID)
        {
            Character character = charactersManager.GetCharactersByTeam(teamID, characterID);
            ball.CharacterCatchBall(character);
            character.ballCatcher.SetState(BallCatcher.states.RUN);
        }
        void FreeBall()
        {
            GameRecorder.Manager.Instance().automaticPlayByRecorder.StopAllTweens();
            ball.ForceLoseBall(Vector3.zero);
        }
    }
}
