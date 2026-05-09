using UnityEngine;

namespace Fulbo.Game.States
{
    public class StateLujito : StateCharacter
    {
        public override void Init(CharacterStates states)
        {
            base.Init(states);
            type = CharacterStates.types.LUJITO;
        }
        public override void SetActive()
        {
            base.SetActive();
            AudioManager.Instance.PlaySoundOneShot("shouts", "ingame/voices/game_vox_yea", false);
            animName = "dashWithBall";
            states.PlayAnim(animName, 0.6f, Reset, true);
            states.character.ballCatcher.Jump();
            states.AimingKick(false);
            Events.Lujito();
        }
        void Reset()
        {
          //  Debug.Log("Lujito Reset");
            states.character.ballCatcher.Reset();
            states.Reset();
        }
        public override void LoseBall() { SetState(states.hitted); }
        public override void Stopped() { }
        public override void Move(float d) { }

    }
}