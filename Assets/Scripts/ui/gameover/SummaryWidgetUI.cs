using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class SummaryWidgetUI : MonoBehaviour
    {
        [SerializeField] Text title;
        [SerializeField] Text scoreField;

        public void Init(string titleText, int score)
        {
            title.text = titleText;
            scoreField.text = Utils.FormatNumbers(score, false);
            SetOn(true);
        }
        public void InitStrings(string titleText, string subtitle)
        {
            title.text = titleText;
            scoreField.text = subtitle;
            SetOn(true);
        }
        public void SetOn(bool isOn)
        {
            gameObject.SetActive(isOn);
        }
    }
}
