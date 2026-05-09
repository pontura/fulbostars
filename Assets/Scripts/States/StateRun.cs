using UnityEngine;

namespace Fulbo.Game.States
{
    public class StateRun : StateCharacter
    {
        float speed;
        public override void Init(CharacterStates states)
        {
            base.Init(states);
            type = CharacterStates.types.RUN;
        }
        public override void SetActive()
        {
            lastSpeed = 0;
            base.SetActive();
            if (states.character.type == Character.types.REFERI)
            {
                states.PlayAnim("run");
                return;
            }            
            animName = "runInit";
            states.PlayAnim(animName);
            if (states.character != null && states.character.ballCatcher != null && states.character.type != Character.types.GOALKEEPER)
                states.character.ballCatcher.SetState(BallCatcher.states.RUN);
        }
        float lastSpeed;
        public override void Move(float _speed)
        {
            if (lastSpeed == _speed) return;
            lastSpeed = _speed;
            if (states.character.ballCatcher.HasBall())
            {
                if (_speed == 1)
                    animName = "runWithBall";
                else
                    animName = "runBoostWithBall";

               // Debug.Log("Move " + animName);
            }
            else
            {
                if (_speed == 1)
                    animName = "run";
                else
                    animName = "runBoost";
            }
            states.PlayAnim(animName);
            if (states.character.type != Character.types.GOALKEEPER)
                states.character.ballCatcher.SetState(BallCatcher.states.RUN_FAST);
        }
        public override void Stopped()
        {
            SetState(states.idle);
        }
    }
}