using UnityEngine;
namespace Fulbo.Game.AIs
{
    public class AIIdle : AIState
    {
        float initTimer;
        AIState aiNextState;
        float idleDelay;

        public override void Init(AI ai)
        {
            type = types.IDLE;
            base.Init(ai);
            color = Color.yellow;
            idleDelay = stats.idleDelay / 10;
        }
        public override void SetActive()
        {
            ai.character.states.Stopped();
            aiNextState = null;
            initTimer = Time.time;
            base.SetActive();
            timer = 0;
        }
        public override AIState UpdatedByAI()
        {
            initTimer += Time.deltaTime;
            timer += Time.deltaTime;
            if (timer > idleDelay)
            {
                if (ai.character == ai.ball.character)
                {
                    SetNextState(ai.aiHasBall);
                    return State();
                }
                ai.character.ResetRigidBody();
                if (initTimer > 0.5f && aiNextState != null)
                {
                    SetState(aiNextState);
                    return State();
                }
                timer = 0;
                if (ai.ball.character == null)
                {
                    Vector3 ballPos = _ballTransform.position;
                    if (Mathf.Sign(ballPos.x) != Mathf.Sign(ai.originalPosition.x))
                    {
                        if (ai.character.type == Character.types.FOR)
                        {
                            if (ballPos.z > 0 && ai.character.fieldPosition == Character.fieldPositions.UP)
                                SetNextState(ai.aiGotoBall);
                            else if (ballPos.z < 0 && ai.character.fieldPosition == Character.fieldPositions.DOWN)
                                SetNextState(ai.aiGotoBall);
                            else
                                SetNextState(ai.aiPositionAttacking);
                        }
                    }
                    else
                    {
                        if (ai.character.type == Character.types.DEF)
                        {
                            if (ballPos.z > 0 && ai.character.fieldPosition == Character.fieldPositions.UP)
                                SetNextState(ai.aiGotoBall);
                            else if (ballPos.z < 0 && ai.character.fieldPosition == Character.fieldPositions.DOWN)
                                SetNextState(ai.aiGotoBall);
                        }
                    }
                }
                else
                {
                    if (ai.ball.character.teamID == ai.character.teamID)
                        SetNextState(ai.aiPositionAttacking);
                    else if (ai.ball.character == ai.character.oponent)
                        SetNextState(ai.aiGotoBall);
                    else
                        SetNextState(ai.aiPositionDefending);
                }
            }
            return State();
        }
        void SetNextState(AIState aistate)
        {
            if (initTimer < 0.5f)
                aiNextState = aistate;
            else
                SetState(aistate);
        }
        public override void GotoBall()
        {
            SetNextState(ai.aiGotoBall);
        }
        public override void OnCatchBall()
        {
            SetState(ai.aiHasBall);
        }
        public override void OnCharacterCatchBall(Character character)
        {
            if (character.teamID == ai.character.teamID)
                SetNextState(ai.aiPositionAttacking);
            else
                SetNextState(ai.aiPositionDefending);
        }
        //public override void OnBallNearOnAir()
        //{
        //    ai.character.states.Jump();
        //}

    }

}