using UnityEngine;

namespace Fulbo.Game.AIs
{
    public class AiGotoBall : AIState
    {
        Vector3 dest;
        float timerToCalculate;
        float timerToCalculateDefending;
        float timeFreezedDefending;
        float timeUnfreezedDefending;
        float timerToWait;
        Ball ball;
        float timerBehindBall;
        bool dash;

        public override void Init(AI ai)
        {
            type = types.GOTOBALL;
            base.Init(ai);
            color = Color.black;
            timerToCalculateDefending = ai.character.stats.delayToGotoToBall;
            timerToCalculate = timerToCalculateDefending / 2;
            timeFreezedDefending = ai.character.stats.timeFreezedDefending;
            timeUnfreezedDefending = ai.character.stats.timeUnfreezedDefending;
            ball = ai.ball;
        }
        public override void SetActive()
        {
            timerToWait = 0;
            timerBehindBall = 0;
            Character characterWithBall = ball.character;
            SetDest(characterWithBall);
        }
        public override AIState UpdatedByAI()
        {
            timerToWait += Time.deltaTime;
            timer += Time.deltaTime;
            Character characterWithBall = ball.character;

            if (characterWithBall != null && characterWithBall == ai.character)
            {
                SetState(ai.aiHasBall);
                return State();
            }

            if (ai.character.states.currentState.type != CharacterStates.types.DASH)
            {
                dash = false;
                if (characterWithBall == null)
                {
                    if (ball.kickType == CharacterStates.kickTypes.CENTRO)
                    {
                        dest = ball.GetProyectedPositionInGround();
                        ai.character.SuperRun();
                    } else
                        SetDest(characterWithBall);
                }
                else if (IsBehind(characterWithBall))
                {
                    if (timer < timeFreezedDefending)
                        dest = ai.character.transform.position; // FREEZED
                    else if (timer > timeFreezedDefending && timer < timeUnfreezedDefending)
                        SetDest(characterWithBall);// UNFREEZED -> attacking
                    else
                        timer = 0;
                }
                else if (timer > timerToCalculateDefending/2) // si el defensor te corre de atras:
                {
                    timer = 0;
                    SetDest(characterWithBall);
                }
            }
            Move(dest, false);
            return State();
        }
        bool IsBehind(Character characterWithBall)
        {
            if (characterWithBall != null)
            {
                if (ai.character.teamID == 1 && ai.character.transform.position.x > characterWithBall.transform.position.x)
                    return true;
                else if (ai.character.teamID == 2 && ai.character.transform.position.x < characterWithBall.transform.position.x)
                    return true;
            }
            return false;
        }
        public override void OnCatchBall()
        {
            SetState(ai.aiHasBall);
        }
        public override void OnCharacterCatchBall(Character character)
        {
            if (character.teamID == ai.character.teamID)
                SetState(ai.aiPositionAttacking);
            else
                SetState(ai.aiIdle);
        }
        void SetDest(Character characterWithBall)
        {
            Vector3 ballPos = ball.transform.position;
            dest = ballPos;
            float distToBall = Vector3.Distance(ai.transform.position, dest);

            if (characterWithBall == null)
            {
                dest = ball.GetProyectedPositionInGround();
                ai.character.SuperRun();
            }
            else SetDestDefending(characterWithBall, ball, distToBall);

            if (distToBall > 8)
                ai.character.SuperRun();
        }
        void SetDestDefending(Character characterWithBall, Ball ball, float distToBall)
        {
            if (CheckForWait(characterWithBall))
            {
                SetState(ai.aiIdle);
                return;
            }
            if (characterWithBall == ai.character)
            {
                SetState(ai.aiHasBall);
                return;
            }
            if (characterWithBall.teamID == ai.character.teamID)
            {
                SetState(ai.aiPositionAttacking);
                return;
            }
            if (ai.character.states.currentState.type != CharacterStates.types.DASH)
            {
                bool canDash = ai.character.stats.CanDash();
                if (distToBall < stats.distance_to_dash_ai && canDash)
                {
                    CheckToDash();

                    if (characterWithBall.states.currentState.type == CharacterStates.types.RUN)
                        dest = ball.GetForwardPosition(1.01f);
                    else
                        dest = ball.transform.position;
                }
            }
        }
        void CheckToDash()
        {
            if (ball.character != null)
            {
                CharacterStates.types t = ball.character.states.currentState.type;
                if (t == CharacterStates.types.GAMBETA || t == CharacterStates.types.LUJITO)
                {
                    if (!ai.character.duelChecker.CanDash(ball.character)) return;
                }
            }
            dash = true;
            ai.character.Dash();
        }
        bool CheckForWait(Character other)
        {
            if (ai.character.type == Character.types.DEF) return false;
            if (timerToWait < (1.5f - timerToCalculate)) return false;
            if (
                Mathf.Abs(ai.transform.position.z - other.transform.position.z) < 1
                &&
                IsBehind(other)
            )
            {
                if (Random.Range(0, 10) < 5)
                    return true;
                return false;
            }
            return false;
        }
    }
}