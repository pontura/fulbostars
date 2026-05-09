using Fulbo.UI.EditTeam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class PopupLevelUI : MonoBehaviour
    {
        [SerializeField] ButtonCustom playButton;
        [SerializeField] ButtonCustom closeButton;
        [SerializeField] GameObject rewardPanel;
        [SerializeField] Text warning_more_playersField;
        [SerializeField] Text rewardTitleField;
        [SerializeField] Text rewardScoreField;

        [SerializeField] GameObject energyAsset; // when has energy
        [SerializeField] ButtonCustom buyEnergyButton;// when No energy

        [SerializeField] GameObject unlockPanel;
        [SerializeField] Text unlockTitle;
        [SerializeField] Text unlockField;

        [SerializeField] GameObject panel;
        [SerializeField] Text textField;
        [SerializeField] TeamPoster teamPoster;
        [SerializeField] ClubShield clubShield;

        [SerializeField] Image backgroundData;
        [SerializeField] GameObject[] slots;

        [SerializeField] GameObject playerWarning;

        LevelsButton levelsButton;
        int levelID;

        void Start()
        {
            Close();
        }
        public void Init(int levelID, LevelsButton levelsButton)
        {
            buyEnergyButton.Init(2, Clicked, Data.Instance.texts.Get("buy_energy"));
            closeButton.Init(1, Clicked);
            this.levelsButton = levelsButton;
            this.levelID = levelID;
            LevelData levelData = CupsData.Instance.GetActualLevel();
            panel.SetActive(true);

            textField.text = levelData.name.ToUpper();
            teamPoster.AddData(levelData);

            DB.DBMatches.MatchData bestMatch = DB.DBManager.Instance.DbMatches.GetBestMatchResults(levelData.stadium_id, levelData.id, true);
            string bestMatchData = "";

            rewardPanel.SetActive(false);
            unlockPanel.SetActive(false);

            SetSlotData(slots[0], Data.Instance.texts.Get("levelpopup_reward"), levelData.GetScoreWin());
            SetSlotData(slots[1], Data.Instance.texts.Get("levelpopup_players"), levelData.oponents.Count);
            SetSlotData(slots[2], Data.Instance.texts.Get("levelpopup_team_power"), levelData.GetTotalStats());

            if (bestMatch != null)
            {
                slots[3].SetActive(true);
                backgroundData.GetComponent<RectTransform>().sizeDelta = new Vector2(580, 108);
                backgroundData.transform.localPosition = new Vector2(-126, -21);
                bestMatchData = bestMatch.score_team2 + "-" + bestMatch.score_team1;
                SetSlotData(slots[3], Data.Instance.texts.Get("levelpopup_best_match"), bestMatchData);
                //if(bestMatch.score_team2 <= bestMatch.score_team1)
                //    SetReward();
            }
            else
            {
                slots[3].SetActive(false);
                backgroundData.GetComponent<RectTransform>().sizeDelta = new Vector2(445, 108);
                backgroundData.transform.localPosition = new Vector2(-57, -21);
            }

            if (levelData.oponents.Count > Data.Instance.myTeam.GetCharacterIds(false).Count + 1)
            {
                playerWarning.SetActive(true);
                warning_more_playersField.text = Data.Instance.texts.Get("levelpopup_warning_players");
            }
            else
            {
                playerWarning.SetActive(false);
            }

            clubShield.Init(levelData.clubData);

            string playButtonText;
            if (Data.Instance.energySystem.GetEnergyAvailable()<=0)
            {
                energyAsset.SetActive(false);
                buyEnergyButton.gameObject.SetActive(true);
                playButtonText = Data.Instance.texts.Get("free_play");
                Events.OpenOutOfEnergyPopup();
            }
            else
            {
                energyAsset.SetActive(true);
                buyEnergyButton.gameObject.SetActive(false);
                playButtonText = Data.Instance.texts.Get("play_now");
            }
            playButton.Init(0, Clicked, playButtonText);

            if (levelsButton != null && levelsButton.state == LevelsButton.states.LOCKED)
            {
                buyEnergyButton.gameObject.SetActive(false);
                playButton.SetText(Data.Instance.texts.Get("locked"));
                playButton.SetInteraction(false);
                unlockPanel.SetActive(true);
                unlockField.text = levelsButton.levelData.conditionText;
                unlockTitle.text = Data.Instance.texts.Get("unlockConditionTitle");
               // SetReward();
            }
            else
            {
                playButton.SetInteraction(true);
            }
            //if (levelsButton != null && levelsButton.state == LevelsButton.states.UNLOCKED)
            //    SetReward();
        }
        public void Clicked(int id) //PLAY NOW
        {
            //if (id == 0)
            //{
            //    AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_play_now");

            //    Data.Instance.matchData.InitLevel(CupsData.Instance.GetActualLevel());
            //    GetComponent<MyTeamSelector>().InitMyTeamSelector();
            //  //  GetComponent<StoryModeLevelsUI>().Reset();
            //}
            //else if (id == 2) // buy energy:s
            //{
            //    Events.BuyEnergyPopup(true);
            //}
            Close();
        }
        //public int rewardScore;
        //void SetReward()
        //{
        //    rewardPanel.SetActive(true);
        //    rewardTitleField.text = Data.Instance.texts.Get("firstWinReward");

        //    float rewardScore = Data.Instance.settings.GetSetting("first_victory_level_multiply");
        //    print("rewardScore " + rewardScore);
        //    rewardScore = levelsButton.levelData.GetScoreWin() * (int)rewardScore;

        //    rewardScoreField.text = Utils.FormatNumbers((int)rewardScore, false);
        //}
        void Close()
        {
            panel.SetActive(false);
        }
        void SetSlotData(GameObject go, string title, int value)
        {
            SetSlotData(go, title, value.ToString());
        }
        void SetSlotData(GameObject go, string title, string value)
        {
            Text[] texts = go.GetComponentsInChildren<Text>();
            texts[0].text = title;
            texts[1].text = value;
        }
    }
}
