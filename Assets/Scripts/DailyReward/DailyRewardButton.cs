using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class DailyRewardButton : ButtonCustom
    {
        [SerializeField] Text desc;
        [SerializeField] Transform container;
        public int value;
        bool loaded;

        Animator iconAnim;

        public void SetPrize(int value)
        {
            this.value = value;
        }
        
        public void SetData(string _desc, GameObject go)
        {
            desc.text = _desc;
            if (!loaded)
            {
                GameObject newGO = Instantiate(go, container);
                newGO.transform.localScale = Vector3.one;
                iconAnim = newGO.GetComponent<Animator>();
                loaded = true;
            }
        }
        string lastAnim = "";
        public void SetState(string animName)
        {
            //Debug.Log(gameObject.name + " - " + lastAnim + " - " + animName);
            //if (lastAnim == "claim") return;
            lastAnim = animName;
            iconAnim.Play(animName);
            if (lastAnim == "claimed") 
                anim.Play("Claimed");
        }

        public void Reset() {
            if (loaded) {
                foreach (Transform child in container)
                    Destroy(child.gameObject);
                loaded = false;
                SetBool("claimed", false);
                SetInteraction(false);
                anim.Play("Disabled");
            }
        }
    }
}
