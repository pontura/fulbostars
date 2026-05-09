using Fulbo.Game;
using Fulbo.Game.Powerups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class PowerupUIScreen : MonoBehaviour
    {
        [SerializeField] Animator anim;
        [SerializeField] Image powerupImage;
        [SerializeField] Image powerupTitleImage;
        PowerupsManager manager;
        
        void Start()
        {
            manager = Fulbo.Game.GameManager.Instance.powerupsManager;
            anim.gameObject.SetActive(false);
        }
        System.Action OnDone;
        public void Init(Character character, System.Action OnDone)
        {
            this.OnDone = OnDone;
            Powerup.types type = character.powerupsManager.GetPowerupType();
            anim.gameObject.SetActive(true);
            anim.Play("powerupScreen");
            anim.playbackTime = 0;
            StartCoroutine(DoIt());
            powerupImage.sprite = manager.GetPowerupData(type).image;
            powerupTitleImage.sprite = manager.GetPowerupData(type).title;
            powerupTitleImage.SetNativeSize();
        }
        IEnumerator DoIt()
        {
            float timeScale = Time.timeScale;
            Time.timeScale = 0;
            Data.Instance.ui.SettingsButtonActive(false);
            while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
                yield return new WaitForSecondsRealtime(0.1f);  

            Time.timeScale = timeScale;
            if(OnDone != null) OnDone();
            yield return new WaitForSecondsRealtime(0.5f);
            anim.gameObject.SetActive(false);
            Data.Instance.ui.SettingsButtonActive(true);

        }
        public void Reset()
        {
            Time.timeScale = Data.Instance.settings.GetSetting("timeScale");
            StopAllCoroutines();
            OnDone = null;
            anim.StopPlayback();
            anim.gameObject.SetActive(false);
        }
    }
}
