using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace GameRecorder
{    
    [Serializable]
    public class TimeLine
    {
        public List<KeyFrame.KeyFrameCharacterData> characters;
        public List<KeyFrame.KeyFrameBallData> ball;
        public List<KeyFrame.KeyFrameKeyValue> voices;
        public List<KeyFrame.KeyFrameEventData> events;
        public MatchSettings settings;
        public ParsedMatchSettings parsedSettings;

        public void NewTimeline()
        {
            characters = new List<KeyFrame.KeyFrameCharacterData>();
            ball = new List<KeyFrame.KeyFrameBallData>();
            voices = new List<KeyFrame.KeyFrameKeyValue>();
            events = new List<KeyFrame.KeyFrameEventData>();
        }
        public void AddCharacterData(KeyFrame.KeyFrameCharacterData keyFrameData)
        {
            characters.Add(keyFrameData);
        }
        public void AddBallData(KeyFrame.KeyFrameBallData keyFrameData)
        {
            ball.Add(keyFrameData);
        }
        public void AddVoiceData(KeyFrame.KeyFrameKeyValue keyFrameData)
        {
            voices.Add(keyFrameData);
        }
        public void AddEventData(KeyFrame.KeyFrameEventData keyFrameData)
        {
            events.Add(keyFrameData);
        }
        public void SetSettings(MatchSettings matchSettings)
        {
            this.settings = matchSettings;
            parsedSettings = new ParsedMatchSettings();
            parsedSettings.duration = settings.duration;
            parsedSettings.stadiumID = settings.stadiumID;
            parsedSettings.levelID = settings.levelID;
            parsedSettings.referee = settings.referee;
            parsedSettings.team1 = ParseTeam(settings.team1);
            parsedSettings.team2 = ParseTeam(settings.team2);
            parsedSettings.referee_position = ParseRefereePosition(settings.referee_position);
            parsedSettings.team1_positions = ParseTeamPosition(settings.team1_positions);
            parsedSettings.team2_positions = ParseTeamPosition(settings.team2_positions);
        }
        List<int> ParseTeam(string teamString)
        {
            string[] arr = teamString.Split(","[0]);
            List<int> result = new List<int>();
            foreach (string s in arr)
            {
                if(s != null && s.Length>0)
                    result.Add(int.Parse(s));
            }
            return result;
        }
        Vector3 ParseRefereePosition(string teamString)
        {
            string[] arr = teamString.Split(":"[0]);
            if(arr != null && arr.Length>1)
                return new Vector3(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));
            return Vector3.zero;
        }
        List<Vector3> ParseTeamPosition(string teamString)
        {
            List<Vector3> result = new List<Vector3>();
            string[] arr = teamString.Split(":"[0]);
            foreach (string s in arr)
            {
                if (s.Length > 2)
                {
                    string[] arr2 = s.Split("_"[0]);
                    result.Add(new Vector3(float.Parse(arr2[0]), float.Parse(arr2[1]), float.Parse(arr2[2])));
                }
            }
            return result;
        }
    }


    [Serializable]
    public class MatchSettings
    {
        public int stadiumID;
        public int levelID;
        public int duration;
        public int referee;
        public string team1;
        public string team2;
        public string referee_position;
        public string team1_positions;
        public string team2_positions;
    }
    [Serializable]
    public class ParsedMatchSettings
    {
        public int stadiumID;
        public int levelID;
        public int duration;
        public int referee;
        public Vector3 referee_position;
        public List<int> team1;
        public List<int> team2;
        public List<Vector3> team1_positions;
        public List<Vector3> team2_positions;
    }
}
