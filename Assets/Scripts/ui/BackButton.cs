using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class BackButton : MonoBehaviour
    {
        types type;
        [SerializeField] Image image;
        [SerializeField] Sprite[] assets;

        public enum types
        {
            BACK,
            HOME
        }
        public void SetActive(bool isOn)
        {
            gameObject.SetActive(isOn);
        }
        public void SetType(types t)
        {
            type = t;
            switch(t )
            {
                case types.BACK:
                    image.sprite = assets[0]; break;
                case types.HOME:
                    image.sprite = assets[1]; break;
            }
        }
    }
}
