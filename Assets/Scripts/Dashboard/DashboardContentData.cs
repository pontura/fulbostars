using System;
using UnityEngine;

namespace Fulbo.Dashoard
{
    [Serializable]
    public class DashboardContentData
    {
        public types type;
        public enum types
        {
            hero,
            caja_1_tercio,
            caja_2_tercios,
            two_characters,
            stats,
            caja_1_medio,
            header,
            win
        }
        public string title;
        public string copete;
        public string text;
        public string param;
        public Color color;
        public int characterID;
        public int characterID2;

        public bool OnlyIfPlayed()
        {
            if (param != null && param.Contains("played")) return true;
            return false;
        }
    }
}
