using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Fulbo.Game
{
    public class SidePlayerSignalPlayer : MonoBehaviour
    {
        public Sprite[] sprites;
        public Image image;
        public int control_id;
        public Character character;
        public GameObject all;

        public void Init(int control_id, Color color)
        {
            this.control_id = control_id;
            image.sprite = sprites[control_id - 1];
            image.color = color;
        }
        public void SetOn(bool isOn)
        {
            gameObject.SetActive(isOn);
        }
        public void SetScale(int dir)
        {
            transform.localScale = new Vector3(dir, 1, 1);
            if (dir == -1)
                image.transform.localScale = new Vector3(-1, 1, 1);
            else
                image.transform.localScale = new Vector3(1, 1, 1);
        }
    }

}