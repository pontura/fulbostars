using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class FigusButtonUI : MonoBehaviour
    {
        public Text field;
        int total;

        private void Start()
        {
            Loop();
        }
        void Loop()
        {
            Invoke("Loop", 1);
            total = Data.Instance.myFigus.all.Count;
            if (total > 0)
                field.text = "SOBRES (" + total + ")";
            else
                field.text = "SIN SOBRES...";
        }
        public void OnClicked()
        {
            if (total == 0) return;
            Data.Instance.LoadLevel("Figus");
        }
    }
}