using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class LevelsButton : ButtonCascade
    {
        public int id;

        LevelsUI ui;

        [SerializeField] Text numField;
        [SerializeField] Text resultField;

        [SerializeField] Sprite winSprite;
        [SerializeField] Sprite drawSprite;
        [SerializeField] Sprite loseSprite;
        [SerializeField] Sprite playSprite;

        [SerializeField] Image image;
        [SerializeField] GameObject lockIcon;
        [SerializeField] GameObject rewardIcon;
        public int stats;

        public bool Locked;

        public states state;
        public enum states
        {
            LOCKED,
            WIN,
            DRAW,
            LOSE,
            UNLOCKED            
        }
        public LevelData levelData;
        public void Init(LevelsUI ui, LevelData levelData, int id)
        {
            rewardIcon.SetActive(false);
            lockIcon.gameObject.SetActive(false);
            resultField.text = "";
            this.levelData = levelData;
            numField.text = id.ToString();
            this.id = id;
            this.ui = ui;
            AddData(levelData);
        }
        public void AddData(LevelData levelData)
        {
            stats = levelData.GetPercentStats();
            FeedbackText("", "");
            field.text = Utils.FormatNumbers(levelData.GetScoreWin(), false);
            SetLevelData(DB.DBManager.Instance.DbMatches.GetBestMatchResults(levelData.stadium_id, id));
        }
        public override void OnClick()
        {
            base.OnClick();
            
            //if (Locked)
            //{
            //    Events.OnPopup(levelData.conditionText, null);
            //}
            //else
            //{
                if (ui != null)
                    ui.Clicked(id, this);
            //}
        }
        void SetLevelData(DB.DBMatches.MatchData d)
        {
            if (d == null)
            {
                if (
                    (levelData.CheckForUnlockCondition())
                    ||
                    (!levelData.locked || levelData.conditions.Count > 0 && levelData.conditions[0].type == LevelData.conditionType.UNLOCKED)
                    )
                {
                    SetLock(false);
                    state = states.UNLOCKED;
                    image.sprite = playSprite;
                    image.SetNativeSize();
                    FeedbackText("", "");
                    OnSelected(true);
                    return;
                }
                else
                {
                    SetLock(true);
                    lockIcon.gameObject.SetActive(true);
                }
                return;
            }

            SetLock(false);
            string title = "";//Data.Instance.texts.Get("your_hiscore");
            string result = "";
           
            if (d.score_team2 > d.score_team1)
            {
                rewardIcon.SetActive(true);
                // feedbackIcons[0].SetActive(true);
                state = states.WIN;
                result = Data.Instance.texts.Get("you_win") + " ";
                image.sprite = winSprite;
                resultField.text = "WIN";
            }
            else if (d.score_team2 < d.score_team1)
            {
                state = states.LOSE;
                result = Data.Instance.texts.Get("you_lose") + " ";
                image.sprite = loseSprite;
                resultField.text = "LOST";
            } 
            else if (d.score_team2 == d.score_team1)
            {
                state = states.DRAW;
                image.sprite = drawSprite;
                resultField.text = "TIED";
            }
           
            result += d.score_team2 + "-" + d.score_team1;
            FeedbackText(title, result);
        }
        void FeedbackText(string title, string results)
        {
            return;
        }
        public bool Won()
        {
            if (state == states.WIN)
                return true;
            return false;
        }
        void SetLock(bool isLocked)
        {
            Locked = isLocked;
            if(isLocked)
            {
                image.enabled = false;
                Color c = field.color;
                c.a = 0.5f;
                field.color = c;
                numField.color = c;
            }
        }
    }

}