using Fulbo.Game;
using Fulbo.Game.Powerups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class PowerupUIButton : MonoBehaviour
    {
        [SerializeField] Image bar;
        [SerializeField] GameObject panel;
        [SerializeField] Image powerupImage;
        PowerupsManager manager;
        [SerializeField] PowerupUIScreen screen;
        public Character character;
        [SerializeField] int teamID;
        [SerializeField] bool isCharging;
        [SerializeField] PowerupUISignalPress powerupUISignalPress;

        float gameTimeScale;
        float powerups_bar_duration;
        float powerups_bar_duration_slow;
        float value;
        float speed = 10;
        float powerups_qty_to_activate;
        float powerups_slowMotion;

        void Start()
        {
            Events.OnPowerupIncrease += OnPowerupIncrease;
            Events.LoseBall += LoseBall;
            Events.OnBallKicked += OnBallKicked;
            Events.CharacterCatchBall += CharacterCatchBall;
            Events.OnPowerupCharging += OnPowerupCharging;
            Reset();
        }
        public void Init()
        {
            gameTimeScale = Data.Instance.settings.GetSetting("timeScale");
            manager = Fulbo.Game.GameManager.Instance.powerupsManager;
            powerups_qty_to_activate = Data.Instance.settings.GetSetting("powerups_qty_to_activate");
            powerups_bar_duration = Data.Instance.settings.GetSetting("powerups_bar_duration");
            powerups_bar_duration_slow = Data.Instance.settings.GetSetting("powerups_bar_duration_slow");
            powerups_slowMotion = Data.Instance.settings.GetSetting("powerups_slowMotion");
        }
        void OnDestroy()
        {
            Events.OnPowerupIncrease -= OnPowerupIncrease;
            Events.CharacterCatchBall -= CharacterCatchBall;
            Events.OnBallKicked -= OnBallKicked;
            Events.LoseBall -= LoseBall;
            Events.OnPowerupCharging -= OnPowerupCharging;
        }
        void OnPowerupIncrease(Character ch)
        {
            if (teamID != ch.teamID) return;

            gameObject.SetActive(true);
            this.character = ch;
            if(value == 0) powerupUISignalPress.OnPowerupActive(true, ch);

            value += 1f / powerups_qty_to_activate;
            StopAllCoroutines();
            StartCoroutine(Goto());
        }
        void SetBar(float v)
        {
            if(bar != null)
                bar.fillAmount = v;
        }
        void Reset()
        {
            panel.SetActive(false);
            value = 0;
            SetBar(0);
        }
        IEnumerator Goto()
        {
            float v = value;
            while (value>v)
            {
                v += (speed) * Time.deltaTime / powerups_slowMotion;
                if (v > 1) v = 1;
                SetBar(v);
                yield return new WaitForEndOfFrame();
            }
            SetBar(value);
            if (IsFilled())
                CharacterCatchBall(character);

        }
        void LoseBall(Character ch)
        {
            if (teamID != ch.teamID) return;
            if (ch == character)
                ResetPowerupImage();
        }
        void OnBallKicked(CharacterStates.kickTypes type, float a, Character ch)
        {
            if (teamID != ch.teamID) return;
            if (ch == character)
                ResetPowerupImage();
        }
        void CharacterCatchBall(Character ch)
        {
            if (teamID != ch.teamID) return;
            screen.Reset();
            character = ch;
            if (!IsFilled()) return;
            panel.SetActive(true);
            if (ch != null) powerupUISignalPress.OnPowerupActive(true, ch);
        }
        public bool IsCharging()
        {
            return isCharging;
        }
        public bool IsFilled()
        {
            if (value >= 0.95f)
                return true;
            return false;
        }
        public void InitCharging(Character character, Powerup.types type)
        {
            if (teamID != character.teamID) return;
            this.character = character;

            float duration = powerups_bar_duration;
            if(type == Powerup.types.SUPERKICK) duration = powerups_bar_duration_slow;

            character.characterSignal.powerupsProgressBar.Init(duration, OnChargingReady);
            Events.OnPowerupCharging(true, character);
        }
        void OnPowerupCharging(bool isOn, Character character)
        {
            if (character != null && teamID != character.teamID) return;
            if (isOn)
            {
                AudioManager.Instance.ChangeVolume("crowd", 0);
                AudioManager.Instance.ChangeVolume("ambience", 0);
                AudioManager.Instance.PlaySoundOneShot("ui", "ingame/powerups/game_powerup");
                Time.timeScale = powerups_slowMotion;
            }
            else
            {
                //AudioManager.Instance.PlaySoundOneShot("ui", "");
                AudioManager.Instance.ChangeVolume("crowd", 1);
                AudioManager.Instance.ChangeVolume("ambience", 1);
                if (character != null && character.characterSignal != null && character.characterSignal.powerupsProgressBar != null && isCharging)
                {
                    character.characterSignal.powerupsProgressBar.SetOff();
                    character = null;
                }
                Time.timeScale = gameTimeScale;
            }
            isCharging = isOn;
        }
       
        void OnChargingReady()// powerup bar filled!
        {
            GameManager.Instance.charactersManager.CancelNextRelease();
            if (character != null && isCharging)
            {
                Events.OnPowerupActive(false, character);
                powerupUISignalPress.OnPowerupActive(false, character);
                Events.OnPowerupActivated(character);
                screen.Init(character, OnPowerupUIScreenReady);
                Time.timeScale = 0;
                panel.SetActive(false);
            }
        }
        void OnPowerupUIScreenReady()
        {
            if(character != null)
                character.powerupsManager.Activate();
            Reset();
            Events.OnPowerupCharging(false, character);           
        }
        void ResetPowerupImage()
        {
            if (character != null && isCharging)
            {
                Events.OnPowerupCharging(false, character);
                character = null;
            }
            panel.SetActive(false);
        }
    }
}