using UnityEngine;

namespace Fulbo.Game.States
{
    public class StateGambeta : StateCharacter
    {
        public override void Init(CharacterStates states)
        {
            base.Init(states);
            type = CharacterStates.types.GAMBETA;
        }
        public override void SetActive()
        {
            Debug.Log("______GAMBETA");
            base.SetActive();
            AudioManager.Instance.PlaySoundOneShot("shouts", "ingame/voices/game_vox_yea", false);
            animName = "gambeta";
            states.PlayAnim(animName, 0.8f, Reset, true);
            states.character.ballCatcher.Gambeta();
            states.AimingKick(false);
            Events.Lujito();
        }
        void Reset()
        {
            states.character.ballCatcher.Reset();
            states.Reset();
        }
        public override void Stopped() { }
        public override void Move(float d) { }

    }
}