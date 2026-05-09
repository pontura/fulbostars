using System.Collections;
using UnityEngine;
namespace Fulbo.Game.States
{
    public class StateDash : StateCharacter
    {
        float dashDuration = 0.5f;
        public override void Init(CharacterStates states)
        {
            base.Init(states);
            type = CharacterStates.types.DASH;
            dashDuration = Data.Instance.settings.gameplay.duration_dash;
        }
        public override void SetActive()
        {
            base.SetActive();
            animName = "dash";
            AudioManager.Instance.PlaySoundOneShot("ingame", "ingame/game_dash");
            states.PlayAnim(animName, dashDuration, OnEndDash);
            states.character.ChangeSpeedTo(states.character.stats.speedDash);
            states.character.characterColliders.ChangeRadius(states.character.stats.collider_radius_dash_multiplier, dashDuration);
        }
        public override void Dash() { }
        public override void Jueguito() { }
        public override void Lujito() { }
      //  public override void Hitted() { }
        public override void Jump() { }
        public override void Hit() { }

        void OnEndDash()
        {
            states.character.ChangeSpeedTo(0);
            if (!states.HasBall())
                SetState(states.freeze);
            else
                SetState(states.idle);
        }
    }
}