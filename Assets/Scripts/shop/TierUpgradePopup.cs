using Fulbo.DB;
using Fulbo.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Fulbo.PricesData;

namespace Fulbo.UI.Shop
{
    public class TierUpgradePopup : MonoBehaviour
    {
        [SerializeField] GameObject panel;

        [SerializeField] Text title;
        [SerializeField] Text field;

        [SerializeField] Text shardsField;
        [SerializeField] Text priceField;
        [SerializeField] Text levelField;

        [SerializeField] GameObject shardsOff;
        [SerializeField] GameObject hardOff;
        [SerializeField] GameObject levelOff;

        [SerializeField] ButtonCustom confirm;
        [SerializeField] ButtonCustom cancel;

        [SerializeField] Button shardBtn;
        [SerializeField] Button priceBtn;
        [SerializeField] Button levelBtn;

        System.Action<bool> OnDone;

        void Start() {
            Events.ConfirmTierUpgrade += ConfirmTierUpgrade;
            Close();
            confirm.Init(0, Clicked);
            cancel.Init(1, Clicked);

            shardBtn.onClick.AddListener(() => Events.OnPopup(GetHelp("shard"), null));
            priceBtn.onClick.AddListener(() => Events.OnPopup(GetHelp("galaxium"), null));
            levelBtn.onClick.AddListener(() => Events.OnPopup(GetHelp("level"), null));

        }

        void OnDestroy()
        {
            Events.ConfirmTierUpgrade -= ConfirmTierUpgrade;
        }
        void ConfirmTierUpgrade(DBUserData.DBCharacterData uData, System.Action<bool> OnDone)
        {
            this.OnDone = OnDone;
            panel.SetActive(true);

            confirm.SetText(Data.Instance.texts.Get("upgrade_tier_confirm"));
            cancel.SetText(Data.Instance.texts.Get("upgrade_tier_cancel"));

            field.text = Data.Instance.texts.Get("Upgrade_tier_desc");
            title.text = Data.Instance.texts.Get("upgrade_tier_title");

            int shards = uData.tierPriceShards;
            shardsField.text = shards.ToString();

            int hard = uData.tierPriceHard;
            priceField.text = hard.ToString();

            shardsOff.SetActive(false);
            hardOff.SetActive(false);
            levelOff.SetActive(false);            

            if (shards > DB.DBManager.Instance.DbUserData.data.shards)
            {
                confirm.SetInteraction(false);
                field.text += Data.Instance.texts.Get("tier_no_shards");
                shardsOff.SetActive(true);
            }
            if (hard > DB.DBManager.Instance.DbUserData.data.hard_currency)
            {
                confirm.SetInteraction(false);
                hardOff.SetActive(true);
                field.text += Data.Instance.texts.Get("tier_no_hard");
            }
            if (uData.tier < 5) {
                int levelRequired = uData.tier * 10;
                levelField.text = "Level "+ levelRequired;
                if (uData.level < levelRequired) {
                    confirm.SetInteraction(false);
                    levelOff.SetActive(true);
                    field.text += Data.Instance.texts.Get("tier_no_level");
                }
            }else
                levelField.text = "Level 40";
        }
        void Close()
        {
            panel.SetActive(false);
        }
        void Clicked(int id) {
            if (id == 0)
                OnDone(true);
            else
                OnDone(false);
            Close();
        }

        string GetHelp(string key) {
                switch (key) {
                    case "shard": return Data.Instance.texts.Get("shards_help");
                    case "galaxium": return Data.Instance.texts.Get("galaxiums_help");
                    case "level": return Data.Instance.texts.Get("tier_levels_help");
                default: return "";
                }
        }
    }
}