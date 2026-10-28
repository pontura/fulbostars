using UnityEngine;
namespace Fulbo.Game.AIs
{
    public class AiIdleGK : AIState
    {
        float gk_distance_to_alert;
        float gkSpeed_sale_z;
        public override void Init(AI ai)
        {
            gkSpeed_sale_z = Data.Instance.settings.GetSetting("gkSpeed_sale_z");
            timeToReact = ai.character.stats.TimeGKStayIdle(ai.character);
            type = types.IDLE;
            timer = 0;
            base.Init(ai);
            color = Color.yellow;
            gk_distance_to_alert = Data.Instance.settings.GetSetting("gk_distance_to_alert");
        }
        public override void SetActive()
        {
            timer = 0;
            base.SetActive();
            ai.character.states.Stopped();
            ai.character.states.LookDirection(ai.GetGKDefaultLookDirection()); // por si quedó flipeado al tirarse
        }
        public override AIState UpdatedByAI()
        {
            timer += Time.deltaTime;

            if (gameManager.state == Fulbo.Game.GameManager.states.GOAL) return State();
            float distToBallX = ai.GetDistanceToBallInX();
            if (distToBallX > 20)
            {
                CheckTarget();
                return State();
            }

            if (timer > 0.1f)
            {
                if (ai.ball.character == null)
                {
                    if (ai.ball.IsDeadAndInArea(ai.character.teamID))
                    {
                        SetState(ai.aiAlertGK);
                        return State();
                    }
                }
            }
            if (timer > timeToReact)
            {
                Vector3 ballPos = ai.ball.transform.position;
                float distToBall = ai.GetDistanceToBall();

                if (ai.ball.character != null && ai.ball.character == ai.character)
                    SetState(ai.aiHasBallGK);
                else if (CheckAlert(distToBallX, distToBall, ai.ball))
                    SetState(ai.aiAlertGK);
                else if (timer > 0.5f)
                {
                    timer = 0;
                    SetState(ai.aiPositionGK);
                }
                else
                    CheckTarget();
            }
            return State();
        }
        // si el punto donde entraría la pelota se movió, vuelve a acomodarse sin esperar:
        void CheckTarget()
        {
            if (timer < timeToReact) return;
            if (Mathf.Abs(ai.GetGKTargetZ(gkSpeed_sale_z) - _transform.position.z) > 0.5f)
            {
                timer = 0;
                SetState(ai.aiPositionGK);
            }
        }
        bool CheckAlert(float distToBallX, float distToBall, Ball ball)
        {
            if (ai.ball.character == null && ai.ball.IsComingToGoal(ai.character.teamID, distToBall, 5)) return true;
            if (ball.character != null && ball.character.teamID == ai.character.teamID) return false;
            if(!ai.IsBallInArea()) return false;
            if (Mathf.Abs(ai.ball.transform.position.x) > (ai.stadiumDataSizeX/2) - gk_distance_to_alert
                && Mathf.Abs(ai.ball.transform.position.z) < 4.5f)
                return true;
            return false;
        }
        public override void OnCatchBall()
        {
            SetState(ai.aiHasBallGK);
        }
        public override void OnBallNearOnAir()
        {
            //if (ai.character.duelChecker.GKCanFly())
            if (BallIsFast())
                ai.character.states.Jump();
        }
        public override void GotoBall()
        {
            SetState(ai.aiGotoBall);
        }

    }

}