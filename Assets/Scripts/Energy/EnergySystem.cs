using Fulbo.DB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fulbo.Onboarding;
using System;

namespace Fulbo.Energy
{
    public class EnergySystem
    {       
        public void GameInit()
        {
            isAFreeGame = GetEnergyAvailable() <= 0;
            if (Data.Instance.mode == Data.modes.PARTYMODE) return;

            DateTime now = DB.DBManager.Instance.Now();
            if (!Data.Instance.matchData.IsTutorial() && now.DayOfWeek!=DayOfWeek.Sunday)
                EnergyUsed(1);

            if (now.DayOfWeek == DayOfWeek.Sunday)
                Events.EnergyUpdated();
        }
        bool isAFreeGame;
        public bool IsAFreeGame()
        {
            Debug.Log("___Is Free game: " + isAFreeGame);
            return isAFreeGame;
        }
        public int GetEnergyAvailable()
        {
            DBEnergyData dbEnergyData = GetEnergy();
            return dbEnergyData.available;
        }
        public void EnergyUsed(int qty)
        {
            // if tutorial dont lose energy
            if (!Data.Instance.onBoardingManager.IsBoardingStepDone(OnBoardingManager.BoardingStepStates.FIRST_MATCH_PLAYED))return;

            Debug.Log("EnergyUsed " + qty);
            DB.DBManager.Instance.DbUserData.data.gameData.OnEnergyChanged(-qty, null);
            Events.OnEnergyUseUpdate();

            if (GetEnergyAvailable() == 0)
            {
                //Analytics
                Dictionary<string, object> param = new Dictionary<string, object>();
                Events.OnTrack("EnergyUsed", param);
            }
        }

        public void EnergyChestupdate() {
            DB.DBManager.Instance.DbGameData.Put(null);
        }       
        
        public void EnergyCheat()
        {
            int totalEnergy = 10;
            DBEnergyData energy = GetEnergy();
            int energyChange = totalEnergy - energy.totalEnergy;
            energy.ResetEnergyTo(totalEnergy);
            DB.DBManager.Instance.DbUserData.data.gameData.OnEnergyChanged(energyChange, null);
            Events.EnergyUpdated();
        }
        DBEnergyData GetEnergy()
        {
            int defaultEnergyInitial = (int)(Data.Instance.settings.GetSetting("energyInitial"));
            return DB.DBManager.Instance.DbUserData.data.gameData.GetEnergyData(defaultEnergyInitial);
        }
    }
}
