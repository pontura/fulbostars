using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.Stadiums;
using Fulbo.Game;

namespace Fulbo
{
    public class LevelBonusData : DataLoader
    {
        public enum parameters
        {
            Ball_Possession,
            Effective_Passes,
            Shoots,
            Referee_Hits,
            Coins_Grabbed,
            Center_Kicks,
            Goal_difference,
            Effective_Tackles,
            Saves,
            Goals,
            Goals_received
        }
        public List<LData> content;
        [Serializable]
        public class LData
        {
            public parameters parameter;
            public int max;
            public int paymentPercent;
        }
        public float GetScoreByGoalDiff(float goals, float goalsReceived, float total)
        {
            //Debug.Log("GetScoreByGoalDiff goals: " + goals + "  goalsReceived: " + goalsReceived + "  total: " + total);
            if (goals < goalsReceived) return 0;
            float goalsValue = GetScore(parameters.Goals, goals, (int)total);
            float goalsReceivedValue = GetScore(parameters.Goals_received, goalsReceived, (int)total);
            //Debug.Log("goalsValue  " + goalsValue);
            //Debug.Log("goalsReceivedValue  " + goalsReceivedValue);
            //Debug.Log("GetScoreByGoalDiff  " + (goalsValue - goalsReceivedValue));

            return goalsValue - goalsReceivedValue;
        }
        public int GetScore(parameters parameter, float value, int total)
        {
            //Debug.Log("________" + parameter.ToString() + " value: " + value + " total_ " + total);
            if (value == 0) return 0;
            foreach(LData data in content)
            {
                if (data.parameter == parameter)
                {
                    float maxPercent = 13; // solo te da como máximo el % del total del partido:
                    float totalPercent = ((float)total * (maxPercent / 100) * (float)data.paymentPercent / 100);
                    if (value > data.max) value = data.max;
                    float result = value * totalPercent / (float)data.max;
                   // Debug.Log("result: " + result);
                    return (int)Mathf.Round(result);
                }
            }
            return 0;
        }
        Action OnReady;
        public void Init(Action OnReady)
        {
            this.OnReady = OnReady;
            LoadData(OnReady);
        }
        public override void OnLoaded(List<List<string>> d)
        {
            OnDataLoaded(content, d);
            base.OnLoaded(d);
        }
        void OnDataLoaded(List<LData> content, List<List<string>> d)
        {
            int colID = 0;
            int rowID = 0;
            LData contentLine = null;
            foreach (List<string> line in d)
            {
                foreach (string value in line)
                {
                    if (rowID >= 1)
                    {
                        if (colID == 0)
                        {
                            if (value != "")
                            {
                                contentLine = new LData();
                                content.Add(contentLine);
                                switch (rowID)
                                {
                                    case 1: contentLine.parameter = parameters.Goals; break;
                                    case 2: contentLine.parameter = parameters.Goals_received; break;
                                    case 3: contentLine.parameter = parameters.Ball_Possession; break;
                                    case 4: contentLine.parameter = parameters.Shoots; break;
                                    case 5: contentLine.parameter = parameters.Effective_Tackles; break;
                                    case 6: contentLine.parameter = parameters.Effective_Passes; break;
                                    case 7: contentLine.parameter = parameters.Saves; break;
                                    case 8: contentLine.parameter = parameters.Referee_Hits; break;
                                    case 9: contentLine.parameter = parameters.Center_Kicks; break;
                                }
                            }
                            else
                                return;
                        }
                        else
                        {
                            if (colID == 1 && value != "")
                            {
                                contentLine.max = int.Parse(value);
                            }
                            else if(colID == 2 && value != "")
                            {
                                contentLine.paymentPercent = int.Parse(value);
                            } 
                        }
                    }
                    colID++;
                }
                colID = 0;
                rowID++;
            }
        }
    }
}