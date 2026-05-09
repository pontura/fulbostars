using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class CharacterStatLine : MonoBehaviour
    {
        [SerializeField] Text field;
        [SerializeField] Text valueField;
        [SerializeField] ButtonCustom upgradeBtn;
        [SerializeField] Text upgradeField;
        int id;
        [SerializeField] bool isSmall;
        [SerializeField] Image icon;
        [SerializeField] Image imageBar;
        [SerializeField] GameObject progressBar;
        [SerializeField] string colorName;

        [SerializeField] Image fillImage;
        [SerializeField] float max_x = -290;

        [SerializeField] GameObject SecondProgressBar;
        [SerializeField] GameObject SecondBar;
        [SerializeField] Button button;
        int value;

        public void Init(string text, int value, int upgrades, int id, Settings.Stats stat)
        {
            this.value = value;
            if(SecondBar != null) SecondBar.SetActive(false);
            int totalStatValue = 100;
            if (value > totalStatValue) value = totalStatValue;
            this.id = id;
            value += upgrades;
            valueField.text = value.ToString();
            icon.sprite = stat.icon;
            if (!isSmall)
            {
                field.text = text.ToUpper();
                if (upgrades > 0)
                    field.text += " (" + upgrades + ")";
            }
            else
                field.text = "";

            Color color = Data.Instance.settings.GetStat(stat.stat).color;
            if (fillImage != null)
            {
                fillImage.color = color;
                fillImage.fillAmount = (float)value / (float)totalStatValue;
            }
            else if (progressBar != null && imageBar != null)
            {
                float _x = Mathf.Lerp(max_x, 0, (float)value / totalStatValue);
                imageBar.color = color;
                progressBar.transform.localPosition = new Vector2(_x, 0);
            }
            if(button != null)
                button.onClick.AddListener(() => Events.OnPopup(CharacterStats.GetStatHelp(id), null));
        }
        public void SetUpgradeButton(int id, DB.DBUserData.DBCharacterData characterData, System.Action<int> OnUpgrade)
        {
            if (upgradeBtn == null) return;
            if (value >= 100)
            {
                upgradeBtn.SetInteraction(false);
                return;
            }
            //characterData.price_per_stat = Data.Instance.pricesData.GetUpgradeStatPrice(characterData);
            //upgradeBtn.Init(id, (idVal)=> {
            //    if (DB.DBManager.Instance.DbUserData.data.Score() >= characterData.price_per_stat) {
            //        value++;
            //        valueField.text = "" + value;                    
            //        OnUpgrade(idVal);
            //    } else Events.OnPopup(Data.Instance.texts.Get("not_enough_money")+" "+ characterData.price_per_stat, null);
            //}, "$" + characterData.price_per_stat);
            //if (characterData.available_stats > 0) {
            //    UpdateUpgradableState(true);
            //} else
            //    UpdateUpgradableState(false);            
        }

        public void UpdateUpgradableState(bool enable) {
            upgradeBtn.SetInteraction(enable);
            if (upgradeField.transform.parent.gameObject.activeSelf)
                foreach (Transform child in upgradeField.transform.parent)
                    child.gameObject.SetActive(enable);            
        }



        public void InitSecondBar(float value)
        {
            valueField.text = "";
            int totalStatValue = 100;
            if (value > totalStatValue) value = totalStatValue;
            SecondBar.SetActive(true);
            float _x = Mathf.Lerp(max_x, 0, (float)value / totalStatValue);
            SecondProgressBar.transform.localPosition = new Vector2(_x, 0);
        }

    }
}