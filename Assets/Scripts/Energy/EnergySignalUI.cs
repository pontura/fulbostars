using Fulbo.UI;
using Fulbo.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Fulbo.Energy.UI
{
    public class EnergySignalUI : MonoBehaviour
    {
        [SerializeField] Text field;
        [SerializeField] Text clockField;
        [SerializeField] ButtonCustom buyBtn;
        //[SerializeField] BuyEnergyPopupUI buyEnergyPopupUI;
        [SerializeField] UIParticleSystem uiParticleSystem;
        [SerializeField] Animator anim;
        [SerializeField] GameObject infinity;
        [SerializeField] Image energyRefillBar;

        int eneryAvailable;

        states state;
        enum states
        {
            IDLE,
            UPDATING // particles flying            
        }
        void Start()
        {
            Events.EnergyUpdated += EnergyUpdated;
            Events.OnEnergyUseUpdate += OnEnergyUseUpdate;
            Events.OnFlyingParticles += OnFlyingParticles;
            Events.OnFlyingPArrives += OnFlyingPArrives;
        }
        void OnDestroy()
        {
            Events.EnergyUpdated -= EnergyUpdated;
            Events.OnEnergyUseUpdate -= OnEnergyUseUpdate;
            Events.OnFlyingParticles -= OnFlyingParticles;
            Events.OnFlyingPArrives -= OnFlyingPArrives;

            anim = GetComponent<Animator>();
        }

        private void OnFlyingParticles(int arg1, FlyingParticlesUI.types type, Vector2 arg3, float b, float arg4)
        {
            if (type != FlyingParticlesUI.types.ENERGY) return;
            state = states.UPDATING;
        }
        private void OnFlyingPArrives(FlyingParticlesUI.types type, float percent, float init, float final)
        {
            if (type != FlyingParticlesUI.types.ENERGY) return;

            print("SET Energy init:" + init + "  final: " + final);
            int _eneryAvailable = (int)final;
            if (percent == 1)
            {
                state = states.IDLE;
                SetParticles();
                AudioManager.Instance.PlaySound("fx", "ui/ui_energy", false);
                int plus = (int)(final - init);
                
                // Invoke("Delayed", 0.25f);
            }
            else
            {
                _eneryAvailable = (int)(init + (float)(final - init) * percent);                
            }
            DB.DBManager.Instance.DbUserData.data.gameData.energyData.SetAvailable(_eneryAvailable);
            SetEnergyField(_eneryAvailable);
            if (percent == 1)
                Data.Instance.energySystem.EnergyChestupdate();
        }
        //void Delayed()
        //{
        //    buyEnergyPopupUI.OpenEnergyPopup();
        //}
        public void Init()
        {
            EnergyUpdated();
            buyBtn.Init(0, BuyClicked);
        }

        void OnEnergyUseUpdate() {
            anim.Play("scoreDown", -1, 0f);
            AudioManager.Instance.PlaySound("fx", "ui/ui_energy_loss", false);
            EnergyUpdated();
        }

        void EnergyUpdated()
        {           
            eneryAvailable = Data.Instance.energySystem.GetEnergyAvailable();
            if (eneryAvailable < DB.DBManager.Instance.DbUserData.data.gameData.energyData.totalEnergy)
                SetClock();
            if(eneryAvailable > 0)
                SetEnergyField(eneryAvailable);

            CheckInfintyDay();
        }
        void SetParticles()
        {
            if (uiParticleSystem != null)
                uiParticleSystem.Play();
        }
        void SetClock() {
            CancelInvoke();
            string countdown = "";
            float progress = 0;
            bool hasAvailable = DB.DBManager.Instance.DbUserData.data.gameData.energyData.available > 0;
            clockField.gameObject.SetActive(!hasAvailable);
            field.gameObject.SetActive(hasAvailable);       
            if (DB.DBManager.Instance.DbUserData.data.gameData.energyData.available > 0)
                progress = 1f - Utils.GetNextHourProgress(DB.DBManager.Instance.Now());
            else
                countdown = Utils.GetNextHourCountdown(DB.DBManager.Instance.Now());
            if (DB.DBManager.Instance.DbUserData.data.gameData.energyData.IsANewHour()) {
                energyRefillBar.fillAmount = 0;
                EnergyUpdated();
            } else {
                clockField.text = countdown;
                energyRefillBar.fillAmount = progress;
                Invoke("SetClock", 1);
            }
        }
        void SetEnergyField(float eneryAvailable)
        {
            print("SET Energy " + eneryAvailable);
            if (eneryAvailable >= DB.DBManager.Instance.DbUserData.data.gameData.energyData.totalEnergy)
                CancelInvoke();
            clockField.gameObject.SetActive(false);
            field.gameObject.SetActive(true);
            field.text = eneryAvailable.ToString();
        }
        public void BuyClicked(int id)
        {
            Events.BuyEnergyPopup(true);
        }
        
        void CheckInfintyDay() {
            DateTime now = DB.DBManager.Instance.Now();
            buyBtn.gameObject.SetActive(now.DayOfWeek != DayOfWeek.Sunday);
            infinity.SetActive(now.DayOfWeek == DayOfWeek.Sunday);      
            if(now.DayOfWeek == DayOfWeek.Sunday)
                Invoke("SetInfinityClock", 1);
        }

        void SetInfinityClock() {
            CancelInvoke();
            clockField.gameObject.SetActive(true);
            field.gameObject.SetActive(false);
            DateTime now = DB.DBManager.Instance.Now();
            int hours = (int)(23 - now.Hour);
            string countdown = "+" + hours + "hs";
            if (DB.DBManager.Instance.DbUserData.data.gameData.energyData.IsANewDay()&& now.DayOfWeek != DayOfWeek.Sunday)
                EnergyUpdated();
            else {
                clockField.text = countdown;
                Invoke("SetInfinityClock", 1);
            }
        }
    }
}