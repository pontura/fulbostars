using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Fulbo.UI
{
    public class StatUI : MonoBehaviour
    {
        [SerializeField] Sprite[] sprites;
        [SerializeField] Image icon;
        [SerializeField] Text field;

        public void Init(int statID, int value)
        {
            Init(statID, value.ToString());
        }
        public void Init(int statID, string value)
        {
            icon.sprite = sprites[statID];
            field.text = value.ToString();
        }
    }
}
