using UnityEngine;

namespace Fulbo.Game.AIs
{
    public class AIIFreeze : AIState
    {
        AIState aiNextState;
        float delayTimer;

        public override void Init(AI ai)
        {
            type = types.FREEZE;
            base.Init(ai);
            color = Color.magenta;           

            if (gameManager.state == GameManager.states.GOAL)
                ai.character.states.Cry();
        }
        public override void SetActive()
        {
            aiNextState = null;
            base.SetActive();
            timer = 0;

            if (ai.character.type == Character.types.GOALKEEPER)
            {
                if (ai.ball.character != null && ai.ball.character == ai.character)
                    delayTimer = ai.character.debufSystem.GetDelayOnFloorIfCatchBall();
                else
                    delayTimer = ai.character.debufSystem.GetDelayOnFloor();
            }
        }
        public override AIState UpdatedByAI()
        {
            timer += Time.deltaTime;
            if(timer > delayTimer)
            {
                if (ai.character.type == Character.types.GOALKEEPER)
                    SetState(ai.aiIdleGK);
            }
            return State();
        }

    }

}