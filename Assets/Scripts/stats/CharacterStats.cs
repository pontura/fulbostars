using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Fulbo
{
    [Serializable]
    public class CharacterStats
    {
        public int accuracy = 0;
        public int stamina = 0;
        public int speed = 0;
        public int dexterity = 0;
        public int awareness = 0;

        public int happiness = 0; //0=Ok, 1=Bad. 2=veryBad. //segun la posicion en la cancha

        public int GetStatByName(int id)
        {
            switch (id)
            {
                case 0: return accuracy;
                case 1: return stamina;
                case 2: return speed;
                case 3: return dexterity;
                default: return awareness;
            }
        }
        public string GetStatName(int id)
        {
            switch (id)
            {
                case 0: return Data.Instance.texts.Get("stat_accuracy");
                case 1: return Data.Instance.texts.Get("stat_stamina");
                case 2: return Data.Instance.texts.Get("stat_speed");
                case 3: return Data.Instance.texts.Get("stat_dexterity");
                default: return Data.Instance.texts.Get("stat_awareness");
            }
        }
        public static string GetStatHelp(int id) {
            switch (id) {
                case 0: return Data.Instance.texts.Get("stat_accuracy_help");
                case 1: return Data.Instance.texts.Get("stat_stamina_help");
                case 2: return Data.Instance.texts.Get("stat_speed_help");
                case 3: return Data.Instance.texts.Get("stat_dexterity_help");
                default: return Data.Instance.texts.Get("stat_awareness_help");
            }
        }

        public int GetAverage() {
            return GetTotal(false) / 5;
        }

        public int GetTotal(bool considerPosition)
        {
            int total = accuracy + stamina + speed + dexterity + awareness;
            if(considerPosition)
                total = (int)((float)total * GetHappinessMultiplier(happiness));
            return total;
        }
        public float GetHappinessMultiplier(int happiness)
        {
            if (happiness == 0) return 1;
            if (happiness == 1) return 0.75f;
            else return 0.5f;
        }
        public void SetHappiness(int value)
        {
            this.happiness = value;
        }
        public void ForceStats(DB.DBUserData.DBCharacterData dbData)
        {
            ForceStats(dbData.accuracy, dbData.stamina, dbData.speed, dbData.dexterity, dbData.trickery);
        }
        public void ForceStats(int allStats)
        {
            ForceStats(allStats, allStats, allStats, allStats, allStats);
        }
        public void ForceStats(int accuracy, int stamina, int speed, int dexterity, int awareness)
        {
            this.accuracy = accuracy;
            this.stamina = stamina;
            this.speed = speed;
            this.dexterity = dexterity;
            this.awareness = awareness;
        }
    }
}