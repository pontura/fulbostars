using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class PriceAsset : MonoBehaviour
    {
        [SerializeField] Image icon;
        [SerializeField] Sprite iconOk;
        [SerializeField] Sprite iconOkHard;
        [SerializeField] Text field;

        [SerializeField] Image button = null;
        [SerializeField] Color[] color;

        public void Init(int price, bool canBeLocked = true, string currency = "soft")
        {
            if (price > DB.DBManager.Instance.DbUserData.data.score && canBeLocked)
            {
                if (button != null)
                    button.color = color[0];
            }
            else
            {
                if (button != null)
                    button.color = color[1];
            }
            if(currency == "soft")
                icon.sprite = iconOk;
            else
                icon.sprite = iconOkHard;

            field.text = Utils.FormatNumbers(price, false);
        }
        public void SetColorForText(Color color)
        {
            field.color = Color.white;
        }
    }
}
