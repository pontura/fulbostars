using Fulbo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class CupButton : ButtonCustom
    {
        public GameObject[] stars;
        public bool active;
        Animator anim;
        public CupsData.CupData data;
        [SerializeField] Transform container;
        public int tier;

        bool tierWon;

        public void Init(CupsData.CupData data, int tier, bool showOnlyTier = false)
        {
            this.tier = tier;
            anim = GetComponent<Animator>();
            this.data = data;
            if(field != null)
            {
                field.text = CupsData.Instance.GetCupData(data.id, tier).cup_name;
            }
            if (data != null)
            {
                GameObject asset;

                if (showOnlyTier)
                    asset = CupsData.Instance.tiersAssets[tier-1];
                else
                    asset = data.GetAssetCup();

                GameObject go = Instantiate(asset, container);
                CupsData.Instance.AddTier(go, tier);
                go.transform.localPosition = Vector2.zero;
                go.transform.localScale = Vector2.one;
            }
            
        }

        private void OnEnable() {
            if(anim==null)
                anim = GetComponent<Animator>();
            SetState(active);
        }

        public void ShowUnlocked() {
            anim.Play("Disabled", 0, 0);
            Invoke("SetUnlocked", 1f);
        }

        void SetUnlocked() {
            AudioManager.Instance.PlaySound("common", "ui/ui_unlock", false);
            anim.Play("unlocked", 0, 0);
            DB.DBManager.Instance.DbGameData.Put("unlockedCup", "false", null);
            Invoke("SetActive", 2f);
        }

        public void SetTierWon() {
            tierWon = true;
            anim.SetBool("tierWon", true);
        }

        void SetActive() {
            SetState(true);
        }

        bool selected;
        public void SetSelected(bool isSelected)
        {
            selected = isSelected;
        }
        public void SetState(bool active)
        {
            int starNum = tier;
            this.active = active;
            switch (active)
            {
                case true:
                    if (tierWon)
                        anim.Play("TierActiveWon", 0, 0);
                    else
                        anim.Play("Normal", 0, 0);
                    break;
                case false:
                    if (tierWon)
                        anim.Play("TierWon", 0, 0);
                    else
                        anim.Play("Disabled", 0, 0);
                    tier = 0;
                    break;
            }
            SetInteraction(active);
            if (selected)
            {
                SetInteraction(false);
                anim.SetBool("active", true);
            }
            else
                anim.SetBool("active", false);            

            if (stars != null && stars.Length > 1)
            {
                
               foreach(GameObject star in stars)
                    star.GetComponent<Animation>().Play("locked");
                for (int a = 0; a < starNum; a++)
                {
                    if (a < tier)
                    {
                        stars[a].SetActive(true);
                        DB.DBCupsData.DBCupData c  = DB.DBManager.Instance.DbUserData.data.gameData.cups.GetCup(data.id, a+1);
                        print(data.id + " tier: " + a + 1 + " cupID: " +  c.cupID + " timesWon: " + c.timesWon);
                        if (c != null && c.timesWon>0)
                            stars[a].GetComponent<Animation>().Play("won");
                        else
                            stars[a].GetComponent<Animation>().Play("active");
                    }
                }
            }
        }
    }
}
