using UnityEngine;

namespace Fulbo.Game.States
{
    public class StateHitted : StateCharacter
    {
        float powerups_bomb_freeze_time;

        public override void Init(CharacterStates states)
        {
            base.Init(states);
            type = CharacterStates.types.KICKED;
            powerups_bomb_freeze_time = Data.Instance.settings.GetSetting("powerups_bomb_freeze_time");
        }
        public override void SetActive()
        {
            int rand = Random.Range(1,6);

            AudioManager.Instance.PlaySound("shouts", "ingame/voices/game_vox_hit" + rand, false);

            AudioManager.Instance.PlayCrowd(Fulbo.Game.GameManager.Instance.stadiumData.active.crowd_foul);

            base.SetActive();

            float delay;

            if (GameManager.Instance.ball.kickType == CharacterStates.kickTypes.KICK_POWERUP)
                delay = powerups_bomb_freeze_time;
            else 
                delay = states.character.stats.freeze_by_dashBall;

            if (delay <= 0) delay = 0.5f;

            states.character.SetCollidersOff(delay);

            animName = "kicked";
            states.PlayAnim(animName, delay, OnReady, true);

            //AudioManager.Instance.PlaySoundOneShot("common", "ingame/game_fall", false);
            Events.OnFX(FX.FXManager.types.FUZZY, states.character.transform.position);
        }
        void OnReady()
        {
            SetState(states.idle);
        }
        public override void Stopped() { }
        public override void Move(float speed) { }
    }
}