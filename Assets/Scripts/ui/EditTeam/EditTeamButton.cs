using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.EditTeam
{
    public class EditTeamButton : ButtonCustom
    {
        public typeButton typeBtn;

        public enum typeButton
        {
            PATTERNS,
            COLOR1,
            COLOR2,
            COLOR3,
            COLOR4,
            SHAPES,
            LOGO
        }
        public ClubData clubData;

        public void OnInit(EditTeamScreen screen, typeButton type, ClubData clubData, ClubShield clubShield_to_add)
        {
            this.typeBtn = type;
            this.clubData = clubData;
            if (type == typeButton.COLOR3)
            {
                GameObject asset = Instantiate(screen.asset_for_shorts, transform);
                Color color = clubData.GetColor(3);
                SetColor(asset, color);
            }
            else if (type == typeButton.COLOR4)
            {
                GameObject asset = Instantiate(screen.asset_for_shoes, transform);
                asset.transform.localScale = new Vector2(0.7f, 0.7f);
                Color color = clubData.GetColor(4);
                SetColor(asset, color);
            }
            else
            {
                ClubShield clubShield = Instantiate(clubShield_to_add, transform);
                clubShield.Init(clubData);
                clubShield.transform.localScale = new Vector2(0.5f, 0.5f);
            }

        }
        //public void OnClicked()
        //{
        //    screen.OnSelect(clubData);
        //    AudioManager.Instance.PlaySoundOneShot("ui", "_new/ui/click3");
        //}
        public void SetColor(GameObject asset, Color color)
        {
            Image[] all = transform.GetComponentsInChildren<Image>();
            all[all.Length-1].color = color;
        }
    }
}
