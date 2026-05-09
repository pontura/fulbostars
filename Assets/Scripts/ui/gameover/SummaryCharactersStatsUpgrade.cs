using Fulbo.DB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class SummaryCharactersStatsUpgrade : CascadeList
    {
        [SerializeField] SummaryCharacterXP summaryCharacterXP;
        [SerializeField] Transform container;
        [SerializeField] GameObject panel;
        [SerializeField] Text field;

        private void Awake()
        {
            Hide();
        }
        private void Start()
        {
            string title = Data.Instance.texts.Get("stats_upgrade_title");
            SetTitle(title);
        }
        public void Hide()
        {
            panel.SetActive(false);
        }
        public void Init()
        {
            Utils.RemoveAllChildsIn(container);
            if (Data.Instance.mode == Data.modes.PARTYMODE) return;
            
            panel.SetActive(true);
            InitCascade();
            int totalPlayers = Data.Instance.matchData.GetTotalPlayersInMatch(2);
            DBGameData.DBFormationSave formation = DBManager.Instance.DbUserData.data.gameData.GetFormation(totalPlayers);
            int id = 0;
            print("SummaryCharactersStatsUpgrade Init");
            print(formation.formation);
            foreach (DBGameData.DBFormationSave.DBFormationChar player in formation.formation)
            {
                DBMatches.PlayerData playerData = new DBMatches.PlayerData();
                playerData.id = player.uniqueID;
                SummaryCharacterXP newSummaryCharacterXP = Instantiate(summaryCharacterXP, container);
                DB.DBUserData.DBCharacterData db = DB.DBManager.Instance.DbUserData.data.GetPlayerByID(player.uniqueID);
                bool levelUpgraded = Data.Instance.matchData.CheckForUpgradedLevel(id, db.level);
                int lastXP = Data.Instance.matchData.GetLastXP(id);

                print("SummaryCharactersStatsUpgrade Init current_level_xp: " + db.current_level_xp + "    lastXP: " + lastXP);
                newSummaryCharacterXP.OnInit(db, levelUpgraded, lastXP);
                AddToCascade(newSummaryCharacterXP);
                id++;
            }
            StartCascade();
        }
        public void SetTitle(string text)
        {
            field.text = text;
        }
    }
}
