using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.UI
{
    public class FigusAsset : MonoBehaviour
    {
        public int id;
        FigusScreen manager;
        public GameObject back;
        bool clicked;

        public void Init(FigusScreen manager, int id)
        {
            this.id = id;
            this.manager = manager;
        }

        public void EnterEnvelope() {
            GetComponentInChildren<Animation>().Play("enterSobre");
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_pack");
        }

        public void OnClicked()
        {
            if (clicked) return;
            clicked = true;
            GetComponent<Animation>().Play();
            StartCoroutine(CallEnter());
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_prize_pack");
            //Invoke("Goto", 0.6f);
        }
        public void OpenFigus() // lo llama el cartel de win en el dashboard:
        {
            Data.Instance.LoadLevel("Figus");
        }
        public void SetBack(bool isOn)
        {
            if (back != null)
                back.SetActive(isOn);
        }
        public IEnumerator CallEnter() {
            yield return new WaitForSeconds(1f);
            manager.OnClicked(this);
            yield return null;
        }
        void Goto()
        {
            manager.OnClicked(this);
        }

    }
}