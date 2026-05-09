using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Fulbo.UI.Shop
{
    public class ChestPiece : MonoBehaviour
    {
        [SerializeField] GameObject hard;
        [SerializeField] GameObject energy;
        [SerializeField] GameObject shards;
        [SerializeField] Text field;

        public void Init(string particleType, int qty)
        {
            hard.SetActive(false);
            energy.SetActive(false);
            shards.SetActive(false);
            switch(particleType)
            {
                case "hard": hard.SetActive(true); break;
                case "energy": energy.SetActive(true); break;
                case "shards": shards.SetActive(true); break;
            }
            field.text = "x" + qty;
        }
    }
}
