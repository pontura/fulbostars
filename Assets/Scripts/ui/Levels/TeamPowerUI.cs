using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class TeamPowerUI : MonoBehaviour
    {
        [SerializeField] Text field;
        public enum types
        {
            WIN,
            LOSE,
            DRAW
        }
        public void Init(int totalStats, types TYPE)
        {
            field.text = Utils.FormatNumbers(totalStats, false);
        }

        public int GetTotalStats() {
            return int.Parse(field.text);
        }
    }
}
