using Fulbo.DB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class SummaryCharacterXP : ButtonCascade
    {
        [SerializeField] ProgressBarCustom progressBarCustom;
        [SerializeField] ButtonCustom upgradeBtn;
        [SerializeField] CharacterButton characterButton;
        [SerializeField] Text xpProgressField;
        [SerializeField] Text levelField;
        [SerializeField] Text XPlevelField;
        [SerializeField] Text levelUpField;

        float levelProgressValue;
        int playerID;

        public void OnInit(DBUserData.DBCharacterData uData, bool levelUpgraded, int lastXP)
        {
            progressBarCustom = GetComponent<ProgressBarCustom>();
            this.playerID = uData.id;
            upgradeBtn.gameObject.SetActive(uData.available_stats > 0);

            if (uData.available_stats>0)
            {
                if (levelUpgraded)  levelUpField.text = Data.Instance.texts.Get("level_up");
                else  levelUpField.text = "";

                upgradeBtn.Init(0, GotoUpgrade, Data.Instance.texts.Get("upgrade"));
            }

            characterButton.OnInit(uData, uData.IsGoalkeeper());

            if (uData == null || uData.role == "") return;

            levelField.text = uData.GetTotalStats().ToString();
            XPlevelField.text = Data.Instance.myTeam.GetCharacterLevelAsString(uData);

            float pvalue = GetCharacterLevelProgressValueWithLastXP(uData, lastXP);
            if (lastXP > uData.current_level_xp + uData.xp_to_next_level)
                pvalue = 0;

            progressBarCustom.Init(pvalue);

            //if (levelUpgraded)
            //{
            //    levelProgressValue = 1; // Complete the bar 
            //    xpProgressField.text = "";
            //}
            //else
            //{
                Debug.Log("_________ uData.current_level_xp" + uData.current_level_xp + "    xp_to_next_level:" + uData.xp_to_next_level);
                xpProgressField.text = Data.Instance.myTeam.GetCharacterLevelProgress(uData);
                levelProgressValue = Data.Instance.myTeam.GetCharacterLevelProgressValue(uData);

                if(pvalue > levelProgressValue)
                    progressBarCustom.Init(0);

                if (uData.xp == uData.currentTierMaxXP && uData.level == uData.maxLevelFromTier)
                {
                    xpProgressField.text = "FULL";
                    levelProgressValue = 1;
                }
           // }
            Invoke("Animate", 0.5f);
        }
        void Animate()
        {
            AudioManager.Instance.PlaySound("common2", "ui/ui_xp", false);
            progressBarCustom.Animate(levelProgressValue, 2, null);
        }
        public float GetCharacterLevelProgressValueWithLastXP(DBUserData.DBCharacterData uData, int lastXP)
        {
            float f = (float)lastXP / (float)(uData.current_level_xp + uData.xp_to_next_level);
          //  print(f + "% de: _______OnInit lastXP: " + lastXP + "  current_level_xp: " + uData.current_level_xp + "    xp_to_next_level: " + uData.xp_to_next_level);
            return f;
        }
        public void GotoUpgrade(int id)
        {
            Data.Instance.shortcut_upgrade_playerID = playerID;
            AudioManager.Instance.PlaySound("shouts", "", false);
            Events.OnSkipOff();
            Data.Instance.matchData.ResetAll();
            
            Data.Instance.LoadLevel("MainMenu");
        }
    }
}
