using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Xtras
{
    public class RecordedCharacters: RecordedItem
    {

        public override void Updated(float now)
        {
            if (GameManager.Instance.state != GameManager.states.PLAYING) return;

            if (timeline.characters.Count <= keyframeID) return;
            for (int a = keyframeID; a < timeline.characters.Count; a++)
            {
                GameRecorder.KeyFrame.KeyFrameCharacterData data = timeline.characters[a];
                if (data.time < now)
                {
                    keyframeID = a + 1;
                    Character character = charactersManager.GetCharactersByTeam(data.teamID, data.characterID);

                    if(data.x>character.transform.localPosition.x)
                        character.states.LookDirection(1);
                    else
                        character.states.LookDirection(-1);

                    character.transform.localPosition = new Vector3(data.x, character.transform.localPosition.y, data.z);
                    GameRecorder.KeyFrame.KeyFrameCharacterData nextData = GetNextFrameForCharacterID(data.characterID, data.teamID);

                    if (data.action != "")
                    {
                        character.states.PlayAnim(data.action);
                        if (data.action == "run")
                            character.ballCatcher.SetState(BallCatcher.states.RUN);
                        else if (data.action == "runBoost")
                            character.ballCatcher.SetState(BallCatcher.states.RUN_FAST);
                        else if (data.action == "idle")
                            character.ballCatcher.SetState(BallCatcher.states.IDLE);
                        else if (data.action == "jueguito")
                            character.ballCatcher.SetState(BallCatcher.states.JUEGUITO);
                    }

                    if (nextData != null)
                    {
                        float duration = nextData.time - data.time;
                        character.characterMovement.SetDestination(new Vector3(nextData.x, character.transform.localPosition.y, nextData.z), duration);
                    }
                }
                else
                    return;
            }
        }
        GameRecorder.KeyFrame.KeyFrameCharacterData GetNextFrameForCharacterID(int characterID, int teamID)
        {
            for (int a = keyframeID; a < timeline.characters.Count; a++)
            {
                GameRecorder.KeyFrame.KeyFrameCharacterData data = timeline.characters[a];
                if (data.characterID == characterID && data.teamID == teamID)
                    return data;
            }
            return null;
        }
    }
}
