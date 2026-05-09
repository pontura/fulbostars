using UnityEngine;
using System;

namespace Fulbo.Game.States
{
    [Serializable]
    public class StateCharacter
    {
        protected string animName;
        public CharacterStates.types type;
        StateCharacter nextState;

        public CharacterStates states;
        public virtual void LoseBall() { }
        public virtual void SetActive() { }
        //public void OnReset() { nextState = null; }
        public virtual void Init(CharacterStates states) { this.states = states; }
        public virtual void Stopped() { }
        public virtual void OnCatchBall() { SetState(states.idle); }
        public virtual void OnPowerupCharge(bool isOn) {
            if (isOn)
                SetState(states.powerupCharging);
            else
                Unfreeze();
        }
        public virtual void Freeze() { SetState(states.freeze); }
        public virtual void Dash() { SetState(states.dash); }
        public virtual void Jueguito() { SetState(states.jueguito); }
        public virtual void Lujito() { SetState(states.lujito); }
        public virtual void Hitted() { SetState(states.hitted); }
        public virtual void Bounce() { SetState(states.bounce); }
        public virtual void Gambeta() { SetState(states.gambeta); }
        public virtual void Jump() { SetState(states.jump); }
        public virtual void Cry() { SetState(states.cry); }
        public virtual void ThrowSomething() { SetState(states.throwState); }
        public virtual void Goal() { SetState(states.goal); }
        public virtual void Hit() { SetState(states.hit); }
        public virtual void Unfreeze() { SetState(states.idle); }
        public virtual void Move(float speed) { }
        public virtual void Kick(CharacterStates.kickTypes kickType) { SetState(states.kick); }

        public StateCharacter State()
        {
                return this;
        }
        public void SetState(StateCharacter _newState)
        {
            if (states.currentState.type == _newState.type) return;
            states.currentState = _newState;
            states.currentState.SetActive();

            if (states.character.track_DEBUG)
                Debug.Log("________" + type + " -> " + _newState.type);
        }
        public virtual StateCharacter Updated()
        {
            return State();
        }

        public string GetAnimName()
        {
            return animName;
        }

    }

}