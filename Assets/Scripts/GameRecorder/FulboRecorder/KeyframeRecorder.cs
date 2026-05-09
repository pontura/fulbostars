using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Xtras
{
    public class KeyframeRecorder : MonoBehaviour
    {
        Ball ball;
        public float ballFrameRate = 0.1f;
        public float charactersFrameRate = 1f;

        void Start()
        {
            Events.OnGameStatusChanged += OnGameStatusChanged;
            Events.CharacterCatchBall += CharacterCatchBall;
        }
        void OnDestroy()
        {
            Events.OnGameStatusChanged -= OnGameStatusChanged;
            Events.CharacterCatchBall -= CharacterCatchBall;
        }
        void CharacterCatchBall(Character ch)
        {
            if (ball == null) return;
            if (GameManager.Instance.state != GameManager.states.PLAYING) return;
            RecordEvent("ballCatched", "", ch);
        }
        void OnGameStatusChanged(Fulbo.Game.GameManager.states state)
        {
            if (GameManager.Instance == null) return;
            if (GameManager.Instance.recordType != GameManager.recordTypes.RECORDING) return;

            if (state == GameManager.states.PLAYING)
            {
                CancelInvoke();
                ball = GameManager.Instance.ball;
                LoopToRecordBall();
                LoopToRecordCharacters();
            }
        }
        void LoopToRecordBall()
        {
            if (ball == null) return;
            Invoke("LoopToRecordBall", ballFrameRate);

            if (GameManager.Instance.state != GameManager.states.PLAYING) return;
            if (ball.character != null) return;

            GameRecorder.KeyFrame.KeyFrameBallData data = new GameRecorder.KeyFrame.KeyFrameBallData();
            data.time = Data.Instance.matchData.time;
            data.x = ball.transform.position.x;
            data.z = ball.transform.position.z;
            data.y = ball.transform.position.y;
            GameRecorder.Manager.Instance().timeLine.AddBallData(data);
        }
        void LoopToRecordCharacters()
        {
            if (GameManager.Instance == null) return;
            Invoke("LoopToRecordCharacters", charactersFrameRate);

            if (GameManager.Instance.state != GameManager.states.PLAYING) return;
            foreach (Character ch in GameManager.Instance.charactersManager.team1)
                RecordCharacter(ch, "");
            foreach (Character ch in GameManager.Instance.charactersManager.team2)
                RecordCharacter(ch, "");
        }

        public void RecordEvent(string key, string value, Character character)
        {
            GameRecorder.KeyFrame.KeyFrameEventData data = new GameRecorder.KeyFrame.KeyFrameEventData();
            data.time = Data.Instance.matchData.time;
            data.key = key;
            data.value = value;
            if (character != null)
            {
                data.characterID = character.orderID;
                data.teamID = character.teamID;
            }
            GameRecorder.Manager.Instance().timeLine.AddEventData(data);
        }
        public void RecordVoice(string value)
        {
            //int playerID;
            //if (int.TryParse(value.Split("_"[0])[0], out playerID) == false) // si no dijo el nombre de un character:
            //{
                GameRecorder.KeyFrame.KeyFrameKeyValue data = new GameRecorder.KeyFrame.KeyFrameKeyValue();
                data.time = Data.Instance.matchData.time;
                data.value = value;
                GameRecorder.Manager.Instance().timeLine.AddVoiceData(data);
           // }
        }
        public void RecordCharacter(Character character, string actionName)
        {
            GameRecorder.KeyFrame.KeyFrameCharacterData data = new GameRecorder.KeyFrame.KeyFrameCharacterData();
            data.time = Data.Instance.matchData.time;
            data.x = character.transform.localPosition.x;
            data.z = character.transform.localPosition.z;
            data.action = actionName;
            data.characterID = character.orderID;
            data.teamID = character.teamID;
            GameRecorder.Manager.Instance().timeLine.AddCharacterData(data);
        }
        public void SaveSettings()
        {
            GameRecorder.MatchSettings matchSettings = new GameRecorder.MatchSettings();
            matchSettings.stadiumID = Data.Instance.matchData.levelData.stadium_id;
            matchSettings.levelID = Data.Instance.matchData.levelData.id;
            matchSettings.duration = Data.Instance.matchData.secs;
            matchSettings.referee = CharactersData.Instance.GetReferi().id;
            matchSettings.referee_position = ParseCharacterPosition(GameManager.Instance.charactersManager.referi);
            matchSettings.team1 = ParseTeam(GameManager.Instance.charactersManager.team1);
            matchSettings.team2 = ParseTeam(GameManager.Instance.charactersManager.team2);
            matchSettings.team1_positions = ParseTeamPosition(GameManager.Instance.charactersManager.team1);
            matchSettings.team2_positions = ParseTeamPosition(GameManager.Instance.charactersManager.team2);
            GameRecorder.Manager.Instance().timeLine.SetSettings(matchSettings);
        }
        string ParseTeam(List<Character> arr)
        {
            string result = "";
            foreach (Character character in arr)
                result += character.data.id + ",";
            return result;
        }
        string ParseTeamPosition(List<Character> arr)
        {
            string result = "";
            foreach (Character character in arr)
                result += ParseCharacterPosition(character) + ":";
            return result;
        }
        string ParseCharacterPosition(Character character)
        {
            string result = "";
            Vector3 pos = character.transform.localPosition;
            result += pos.x + "_" + pos.y + "_" + pos.z;
            return result;
        }
    }
}

