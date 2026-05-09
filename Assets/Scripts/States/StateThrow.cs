using UnityEngine;
using System.Collections;

namespace Fulbo.Game.States
{
    public class StateThrow : StateCharacter
    {
        public override void Init(CharacterStates states)
        {
            base.Init(states);
            type = CharacterStates.types.SPECIAL_ACTION;
        }
        public override void SetActive()
        {
            base.SetActive();
            AudioManager.Instance.PlaySound("shouts", "dash", false);
            animName = "throw";
            states.PlayAnim(animName, 0.5f, OnReady);
        }
        void OnReady()
        {
            SetState(states.idle);
        }
        public override void Stopped() { }
        public override void Move(float speed) { }
    }
}