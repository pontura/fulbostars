using Fulbo;
using Fulbo.DB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class CupsSignal : MonoBehaviour
    {
        [SerializeField] Text field;
        [SerializeField] Text cupName;
        [SerializeField] Text field2;
        [SerializeField] GameObject panel;
        [SerializeField] Transform container;

        [SerializeField] ButtonCustom claimLoseButton;
        [SerializeField] GameObject winUI;
        [SerializeField] GameObject loseUI;
        [SerializeField] Text loseField;

        System.Action OnReady;
        [SerializeField] ButtonCustom button;
        int state = 0;
        bool win;
        int timesWon;

        private void Start()
        {
            Close();
            Events.ShowCupWinSignal += ShowCupWinSignal;
            button.Init(0, Clicked);
        }
        private void OnDestroy()
        {
            Events.ShowCupWinSignal -= ShowCupWinSignal;
        }
       
        public void ShowCupWinSignal(bool win, System.Action OnReady)
        {
            state = 0;
            panel.SetActive(true);

            Animator anim = panel.GetComponent<Animator>();
            if (anim != null)
                anim.Play("idle", 0, 0);

            int cupID = Data.Instance.matchData.levelData.cupID;
            int tier = Data.Instance.matchData.levelData.tier;

            DB.DBCupsData cups = DB.DBManager.Instance.DbUserData.data.gameData.cups;
            CupsData.CupData d = CupsData.Instance.GetCup(cupID);

            Utils.RemoveAllChildsIn(container);

            GameObject go = d.GetAssetCup();
            if (go != null)
            {
                GameObject cupNew = Instantiate(go, container);
                CupsData.Instance.AddTier(cupNew, tier);
            }

            loseUI.SetActive(false);
            winUI.SetActive(false);

            this.win = win;
            if (win)
            {
                winUI.SetActive(true);
                Events.OnVoiceSay("cupwon", null);
                AudioManager.Instance.PlaySoundOneShot("ui", "ui/cups/ui_cup_win");
                timesWon = cups.GetTimesWon(cupID, tier);
                print("Times won " + timesWon);
                if (timesWon <= 1)
                {
                    field.text = Data.Instance.texts.Get("won_cup_first_time");
                }
                else
                {
                    field.text = Data.Instance.texts.Get("won_cup");
                }
                cupName.text = Data.Instance.texts.CheckAndReplaceVarsIn("[name cup]");
                Invoke("Animate", 0.1f);
                field2.text = Data.Instance.texts.Get("skipTextMobile");
            }
            else
            {
                button.gameObject.SetActive(false);
                claimLoseButton.gameObject.SetActive(true);
                claimLoseButton.Init(0, OnLoseClaimed, Data.Instance.texts.Get("claim"));
                loseField.text = "";
                Events.OnVoiceSay("cuplose", null);
                loseUI.SetActive(true);
            }

            this.OnReady = OnReady;

            Dictionary<string, object> param = new Dictionary<string, object>();

            param["cup"] = d.id;
            param["tier"] = d.tier;
            param["counter"] = cups.GetCup(d.id, d.tier).timesWon;

            if (win)
                Events.OnTrack("CupWon", param);
            else
                Events.OnTrack("CupLost", param);

           

        }
        void OnLoseClaimed(int a)
        {
            button.gameObject.SetActive(true);
            claimLoseButton.gameObject.SetActive(false);
            loseField.text = Data.Instance.texts.Get("lose_cup");
            Invoke("PlayLoseSfxDelayed", 1.08f);
            field2.text = Data.Instance.texts.Get("skipTextMobile");
            Animate();
        }
        void Animate()
        {
            Animator anim = panel.GetComponent<Animator>();
            if (anim == null) return;
            if (win)
                anim.Play("win", 0, 0);            
            else
                anim.Play("lose", 0, 0);
        }

        void PlayLoseSfxDelayed() {
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/cups/ui_cup_lose");
        }

        void Clicked(int id)
        {
            if (!win || timesWon > 1) // si perdiste o ya la ganaste antes no hay cofre 
                OnAllDone();
            else if (state == 0)
            {
                Data.Instance.ui.OnCupWon();
                int cupID = Data.Instance.matchData.levelData.cupID;
                int tier = Data.Instance.matchData.levelData.tier;
                int chestID = CupsData.Instance.GetCupData(cupID, tier).chest;
                MatchData.ResponseFromServer.ChestDataFromDB chestData = Data.Instance.matchData.response.chestData;
                chestData.shard += Data.Instance.matchData.response.shardsWon;

                chestData.hard_from = Data.Instance.matchData.dataOnInit.hard_on_init_match;
                chestData.energy_from = DBManager.Instance.DbUserData.data.gameData.energyData.available;
                chestData.shard_from = Data.Instance.matchData.dataOnInit.shards_on_init_match;

                Events.OpenChest(chestID, OnAllDone, chestData);
                state++;
                panel.SetActive(false);
            }
        }
        public void OnAllDone()
        {
#if UNITY_ANDROID || UNITY_IOS
            if(win) Data.Instance.reviewRequest.Check();
#endif
            Close();
            OnReady();
        }
        public void Close()
        {
            panel.SetActive(false);
        }
    }
}
