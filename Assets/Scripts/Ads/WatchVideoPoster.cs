using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Ads
{
    public class WatchVideoPoster : MonoBehaviour
    {
        [SerializeField] Text field;

        public void Init(string text)
        {
            field.text = text;
        }
    }
}
