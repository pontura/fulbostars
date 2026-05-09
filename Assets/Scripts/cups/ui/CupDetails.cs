using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Fulbo.UI.Progress;

namespace Fulbo.UI
{
    public class CupDetails : MonoBehaviour
    {
        [SerializeField] Transform cupContainer;
        [SerializeField] Text cupNameField;
        [SerializeField] Text matchesField;
        [SerializeField]  float totalWidth;
        [SerializeField] PorgressBarPieces progressBar;

        private void Start()
        {
            Init(Data.Instance.matchData.levelData);
            if (Data.Instance.mode == Data.modes.PARTYMODE)
                gameObject.SetActive(false);
        }

        public void Init(LevelData data) {
            Init(data.cupID, data.tier);
        }

        public void Init(int cupID, int tier, bool isPreMatch = true)
        {
            if (cupID != 0) {
                DB.DBCupsData cups = DB.DBManager.Instance.DbUserData.data.gameData.cups;
                DB.DBCupsData.DBCupData d = cups.GetCup(cupID, tier);

                int played = 1;
                if (d != null)
                {
                    foreach (DB.DBCupsData.DBCupLevelData l in d.levels)
                    {
                        if (l.score > l.opp_score)
                            played++;
                    }
                }

                if (!isPreMatch)played--;

                int totalLevels = CupsData.Instance.GetAllLevelsFromCup(cupID, tier).Count;
                CupsData.CupData cupData = CupsData.Instance.GetCupData(cupID, tier);
                cupNameField.text = cupData.cup_name.ToUpper();


               // matchesTitleField.text = Data.Instance.texts.Get("matchTitle").ToUpper();
                if (played == totalLevels)
                    matchesField.text = Data.Instance.texts.Get("last_match").ToUpper();
                else
                    matchesField.text = "Match " + played + " of " + totalLevels;

                SetBar(played , totalLevels);

                if (cupContainer != null) {
                    Utils.RemoveAllChildsIn(cupContainer);

                    GameObject go = Instantiate(cupData.GetAssetCup(), cupContainer);
                    CupsData.Instance.AddTier(go, cupData.tier);
                    go.transform.localPosition = Vector2.zero;
                    go.transform.localScale = Vector2.one;
                }
            }
        }
        public void SetBar(int selectedID, int total) // force initial value
        {
            progressBar.Init(selectedID, total);
        }
    }
}
