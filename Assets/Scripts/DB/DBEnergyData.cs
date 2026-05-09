using UnityEngine;
using System.Collections;
using System;

namespace Fulbo.DB
{
    [Serializable]
    public class DBEnergyData
    {
        public int totalEnergy;
        public int available;
        public int numberOfBoughtsToday; // compras hoy
        public string today;
        public int videosSeen;
        public int nextHourRefill;

        public void ResetEnergyTo(int value)
        {
            totalEnergy = value;
            
            numberOfBoughtsToday = 0;
            videosSeen = 0;
            nextHourRefill = -1;
        }

        public void RefillEnergyByHour() {
            Debug.Log("#RefillEnergyByHour");
            if (available < totalEnergy && nextHourRefill!=-1) {
                DateTime now = DB.DBManager.Instance.Now();
                int nowHour = IsANewDay()?24 + now.Hour:now.Hour;
                available = System.Math.Min(totalEnergy, available + 1 + (nowHour - nextHourRefill));
                Debug.Log("#Available: "+ totalEnergy+", "+(1 + (nowHour - nextHourRefill)));
                nextHourRefill = available < totalEnergy ? (now.Hour + 1)%24 : -1;
                today = Utils.Today(DB.DBManager.Instance.versionMode == DB.DBManager.versionModes.PROD);
                DB.DBManager.Instance.DbGameData.Put(null);
                Events.EnergyUpdated();
            }
        }

        public void SetAvailable(int value)
        {
            available = value;
        }
        public void Add(int value)
        {
            available += value;
            today = Utils.Today(DB.DBManager.Instance.versionMode == DB.DBManager.versionModes.PROD);

            DateTime now = DB.DBManager.Instance.Now();

            if (value > 0)
                numberOfBoughtsToday++;
            else if (available < 0) available = 0;

            if(available<totalEnergy && nextHourRefill==-1)
                nextHourRefill = (now.Hour + 1) % 24;
        }
        public bool IsANewDay()
        {
            return Utils.Today(DB.DBManager.Instance.versionMode == DB.DBManager.versionModes.PROD) != today;
        }

        public bool IsANewHour() {
            DateTime now = DB.DBManager.Instance.Now();
            return now.Hour>=nextHourRefill;
        }


    }
    
}
