using UnityEngine;
namespace Fulbo.Game.AIs
{
    public class AiAlertGK : AIState
    {
        float gk_distance_to_alert;
        float gkSpeed_speed_flying_multiply;
        int totalStats;

        public override void Init(AI ai)
        {
            timeToReact = ai.character.stats.TimeGKStayAlert(ai.character);
            gkSpeed_speed_flying_multiply = ai.character.stats.gkSpeed_speed_flying_multiply;
            type = types.ALERT;
            base.Init(ai);
            color = Color.red;
            gk_distance_to_alert = Data.Instance.settings.GetSetting("gk_distance_to_alert");
            totalStats = ai.character.characterStats.GetTotal(false);
        }
        public override void SetActive()
        {
            timer = 0;
            base.SetActive();
            ai.character.states.Stopped();

           // float duelStats = CupsData.Instance.GetActualLevel().duelStatsGK;
            float limits_x = 4;//duelStats / 20;
            if (limits_x > 4) limits_x = 4;
            SetLimits(limits_x, 4.5f);
        }
        public override AIState UpdatedByAI()
        {
            timer += Time.deltaTime;

            if (gameManager.state == Fulbo.Game.GameManager.states.GOAL) return State();
            
            if (ai.ball.character != null)
            {
                if (ai.ball.character.teamID == ai.character.teamID)
                {
                    SetState(ai.aiIdleGK);
                    return State();
                }
                else if (timer < timeToReact) return State();
            }
            if (!ai.IsBallInArea())
            {
                SetState(ai.aiIdleGK);
                return State();
            }

            float distanceToBallInX = ai.GetDistanceToBallInX();
            float _x = 0;
            float _z = 0;

            Vector3 ballPos = _ballTransform.position;
            ballPos.y = 0;

            if (ai.ball.character == null)
                ballPos.z = ai.GetGKTargetZ(4.5f);
     
            if (_transform.position.x > ballPos.x) _x = -1; else _x = 1;
            if (_transform.position.z > ballPos.z) _z = -1; else _z = 1;

            if(CheckBallFarInZ(_ballTransform.position))
                SetState(ai.aiPositionGK);
            if (distanceToBallInX > gk_distance_to_alert)
                SetState(ai.aiIdleGK);
            else
            {
                Vector2 _newPos = CheckMoveTo(ai.character, _x, _z, ballPos);
                if (_newPos != Vector2.zero)
                    ai.character.MoveTo(_newPos.x, _newPos.y);
                else
                    SetState(ai.aiIdleGK);
            }
            return State();
        }
        bool CheckBallFarInZ(Vector3 ballPos)
        {
            if ((Mathf.Abs(ballPos.z) > 7.5f)) return true;
            return false;
        }
    }

}