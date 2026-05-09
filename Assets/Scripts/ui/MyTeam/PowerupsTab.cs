using Fulbo.Game.Powerups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class PowerupsTab : CascadeList
    {
        [SerializeField] Text powerupTitleField;
        [SerializeField] Text powerupField;
        [SerializeField] Text subtitleField;
        [SerializeField] Text subField;
        [SerializeField] ButtonCascade[] buttons;

        public void OnEnable()
        {
            print("StartCascade");
            int id = 0;
            InitCascade();
            foreach (ButtonCascade b in buttons)
            {
                b.Init(id, Clicked);
                id++;
                AddToCascade(b);
            }
            Clicked(0);
            StartCascade();
        }
        public void Clicked(int id)
        {
            print("Clicked " + id);
            foreach (ButtonCascade b in buttons)
                b.SetInteraction(true);

            buttons[id].SetInteraction(false);

            Powerup.types type = Powerup.types.BOMB;
            switch (id)
            {
                case 0: type = Powerup.types.BOMB; break;
                case 1: type = Powerup.types.SPEED; break;
                case 2: type = Powerup.types.SUPERKICK; break;
            }
            powerupTitleField.text = Data.Instance.texts.Get(type.ToString());
            powerupField.text = Data.Instance.texts.Get("hint_" + type.ToString());
            subtitleField.text = Data.Instance.texts.Get("tooltip_powerup_title");
            subField.text = Data.Instance.texts.Get("tooltip_powerup_text");
        }
    }
}
