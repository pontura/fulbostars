using UnityEngine;

namespace Fulbo.Game.States
{
    public class StateIdle : StateCharacter
    {
        bool isGoalKeeper;

        public override void Init(CharacterStates states)
        {
            isGoalKeeper = states.character.type == Character.types.GOALKEEPER;
            base.Init(states);
            type = CharacterStates.types.IDLE;
        }
        public override void SetActive()
        {
            base.SetActive();

            animName = "idle";

            if (states.character.ballCatcher != null)
                states.character.ballCatcher.Idle();

            if (states.character.type != Character.types.REFERI)
            {
                if (states.ball.character != null && states.ball.character.teamID != states.character.teamID)
                    animName = "alert";
                else if (states.character.type == Character.types.GOALKEEPER)
                    animName = "idle";
                if (states.character != null
                    && animName == "idle" && states.ball.character != null
                    && states.ball.character == states.character)
                    animName = "idle_ball";
            }
            states.PlayAnim(animName);
        }
        public override void Move(float speed)
        {
            SetState(states.run);
        }
    }

}