using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Powerups
{
    public class PowerupSpeed : Powerup
    {
        float multiplierSpeed = 2; 
        float duration = 4;
        public override void OnInstanced()
        {
            AudioManager.Instance.PlaySound("common", "ingame/powerups/game_superspeed_loop", true);
            multiplierSpeed = Data.Instance.settings.GetSetting("powerup_SuperSpeed_multiplier");
            duration = Data.Instance.settings.GetSetting("powerup_SuperSpeed_duration");
            manager.Activate(OnResetPowerup, duration);
            character.SetSpeedMultiplier(multiplierSpeed, duration);
            character.characterFXManager.OnPowerupSuperRun(duration);
        }
        void OnResetPowerup()
        {
            AudioManager.Instance.PlaySound("common", "ingame/powerups/game_superspeed_out", false);
            character.characterFXManager.Reset();
            character.SetSpeedMultiplier(1, 0);
            manager.DestroyPowerup(this);
        }
    }
}