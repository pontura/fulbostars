using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class ArcadeUI : MonoBehaviour
    {
        public GameObject volPanel;
        public Image volumenProgress;
        Coroutine volC;
        public int menuOpenedClicks;

        void Start()
        {
            if (Data.Instance.mode != Data.modes.PARTYMODE)
                Destroy(gameObject);
            else
            {
                volPanel.SetActive(false);
            }
            Events.SetArcadeVolUp += VolUp;
            Events.SetArcadeVolDown += VolDown;
        }
        private void OnDestroy()
        {
            Events.SetArcadeVolUp -= VolDown;
            Events.SetArcadeVolDown += VolUp;
        }
        void VolDown()
        {
            SetVolume(-.05f);
        }
        void VolUp()
        {
            SetVolume(.05f);
        }
        //void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha9)) SetVolume(-.05f);
        //    else if (Input.GetKeyDown(KeyCode.Alpha0)) SetVolume(.05f);

        //    if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3))
        //    {
        //        menuOpenedClicks++;
        //        if (menuOpenedClicks > 1)
        //        {
        //            menuOpenedClicks = 0;
        //            Events.OpenMainMenu();
        //        }
        //    }
        //    if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Alpha3))
        //    {
        //        menuOpenedClicks = 0;
        //    }
        //}
        void SetVolume(float sum)
        {
            print("Set vol_: " + sum + " to: " + AudioListener.volume);
            volPanel.GetComponent<Animation>().Play("on");
            if (volC != null) StopCoroutine(volC);
            volPanel.SetActive(true);
            float fillAmount = volumenProgress.fillAmount;
            fillAmount += sum;
            if (fillAmount > 1) fillAmount = 1;
            else if (fillAmount < 0) fillAmount = 0;

            volumenProgress.fillAmount = fillAmount;

            AudioListener.volume = fillAmount;

            AudioManager.Instance.PlaySound("common", "ui/click", false);

            volC = StartCoroutine(ResetVolPanel());
        }
        IEnumerator ResetVolPanel()
        {
            yield return new WaitForSeconds(1);
            volPanel.GetComponent<Animation>().Play("off");
            yield return new WaitForSeconds(0.5f);
            volPanel.SetActive(false);
        }
    }
}