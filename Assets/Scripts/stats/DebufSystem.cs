using UnityEngine;
using System.Collections;
using System;

namespace Fulbo.Game
{
    public class DebufSystem 
    {
        [SerializeField] float debufTime;
        [SerializeField] int debuf;

        float gk_timeOnFloor_min;
        float gk_timeOnFloor_max;
        float gk_fatigue;
        float gk_fatigue_timtToReset;

        Character character;

        #region INIT
        float speed;
        float collider_radius;
        float collider_height;
        float collider_radius_air;
        float gkSpeed_speed_flying_multiply;
        float gk_catch_on_air;

        int _stamina;
        int _awareness;
        int _speed;
        int _dexterity;
        int _accuracy;

        public void SetInititalValues()
        {
            _stamina = character.characterStats.stamina;
            _accuracy = character.characterStats.accuracy;
            _speed = character.characterStats.speed;
            _dexterity = character.characterStats.dexterity;
            _awareness = character.characterStats.awareness;

            speed = character.stats.speed;
            collider_radius = character.stats.collider_radius;
            collider_height = character.stats.collider_height;
            collider_radius_air = character.stats.collider_radius_air;
            gkSpeed_speed_flying_multiply = character.stats.gkSpeed_speed_flying_multiply;
            gk_catch_on_air = character.stats.gk_catch_on_air;
        }
        public void Init(Character character) { this.character = character; }
        public void InitGk()
        {
            gk_timeOnFloor_min = Data.Instance.settings.GetSetting("gk_timeOnFloor_min");
            gk_timeOnFloor_max = Data.Instance.settings.GetSetting("gk_timeOnFloor_max");
            gk_fatigue = Data.Instance.settings.GetSetting("gk_fatigue");
            gk_fatigue_timtToReset = Data.Instance.settings.GetSetting("gk_fatigue_timtToReset");
            SetInititalValues();
        }
        #endregion

        float lastTimeActive = 0;
        public void DebufFatigue()
        {
            if (debuf >= 5) return;
            if (lastTimeActive + 0.5f > Time.time) return;
          //  Debug.Log("Fatigue: " + debuf);
            lastTimeActive = Time.time;
            debuf++;
            Data.Instance.StartCoroutine(ResetFatigue());
            ChangeStatsByDebuf(-gk_fatigue);// RESTA POWER
        }
        IEnumerator ResetFatigue()
        {
            yield return new WaitForSeconds(gk_fatigue_timtToReset);
            if (debuf > 0)
            {
                debuf--;
                ChangeStatsByDebuf(gk_fatigue);// RESETEA POWER }
            }
        }
        public float GetDelayOnFloor()
        {
            int stamina = character.characterStats.stamina;
            float normalizedValue = ((float)stamina - ((float)stamina * gk_fatigue / 100)/100);
            float delay = Mathf.Lerp(gk_timeOnFloor_min, gk_timeOnFloor_max, normalizedValue);
           // Debug.Log("GetDelayOnFloor delay: " + delay  + " stamina: " + stamina + " normalizedValue: " + normalizedValue);
            return delay;
        }
        public float GetDelayOnFloorIfCatchBall()
        {
           // Debug.Log("GetDelayOnFloorIfCatchBall");
            return 0.5f;
        }       
        public void ChangeStatsByDebuf(float percent)
        {
            character.characterStats.stamina += (int)((float)_stamina * percent / 100);
            character.characterStats.accuracy += (int)((float)_accuracy * percent / 100);
            character.characterStats.speed += (int)((float)_speed * percent / 100);
            character.characterStats.dexterity += (int)((float)_dexterity * percent / 100);
            character.characterStats.awareness += (int)((float)_awareness * percent / 100);

            character.stats.speed += (speed * percent / 100);
            character.stats.collider_radius += (collider_radius * percent / 100);
            character.stats.collider_height += (collider_height * percent / 100);
            character.stats.collider_radius_air += (collider_radius_air * percent / 100);
            character.stats.gkSpeed_speed_flying_multiply += (gkSpeed_speed_flying_multiply * percent / 100);
            character.stats.gk_catch_on_air += (int)(gk_catch_on_air * percent / 100);
        }
    }
}
