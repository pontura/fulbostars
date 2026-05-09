using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class SummaryItem : MonoBehaviour
    {
        [SerializeField] Text field;
        [SerializeField] Text field_value1;
        [SerializeField] Text field_value2;
        [SerializeField] ScoreUI scoreUI;
        [SerializeField] GameObject bg;
        public string key;
        public float winScore;

        public enum types
        {
            NUM,
            PERCENT
        }

        public void Init(string _key, int value1, int value2, types type, float winScore)
        {
            this.winScore = winScore;
            scoreUI.gameObject.SetActive(false);
            this.key = _key;
            field_value1.text = field_value2.text = "";

            field.text = Data.Instance.texts.Get(key);

            if(type == types.PERCENT)
            {
                float total = (float)(value1 + value2);
                value1 = (int)Mathf.Round(((float)(value1) * 100) / total);
                value2 = (int)Mathf.Round(((float)(value2) * 100) / total);
            }

            field_value2.text = value1.ToString();
            field_value1.text = value2.ToString();


            if(type == types.PERCENT)
            {
                field_value1.text += "%";
                field_value2.text += "%";
            }
        }
        public void SetBg()
        {
           // bg.SetActive(false);
        }
        public void SetWin()
        {
            if (winScore != 0)
            {
                scoreUI.gameObject.SetActive(true);
                scoreUI.ForceScore(winScore, "scoreUp");
            }
        }
    }
}
