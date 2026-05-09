using UnityEngine;

namespace Fulbo.Game.States
{
    public class StateBounce : StateCharacter
    {
        float powerups_bomb_freeze_time;
        Vector3 dest;
        float timer = 0;

        public override void Init(CharacterStates states)
        {
            base.Init(states);
            type = CharacterStates.types.KICKED;
            powerups_bomb_freeze_time = Data.Instance.settings.GetSetting("powerups_bomb_freeze_time");
        }
        public override void SetActive()
        {
            dest = states.transform.position + states.character.bounceDirection;
            timer = 0;
            base.SetActive();
            animName = "kicked";
            float delay = powerups_bomb_freeze_time; // Data.Instance.settings.gameplay.freeze_by_hit;

            if (delay == 0)  delay = 0.5f;
            states.PlayAnim(animName, delay, OnReady);
        }
        void OnReady()
        {
            timer = 0;
            SetState(states.idle);
        }
        public override void Stopped() { }
        public override void Move(float speed) { }
    }
}