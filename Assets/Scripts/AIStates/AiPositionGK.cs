using UnityEngine;
namespace Fulbo.Game.AIs
{
    public class AiPositionGK : AIState
    {
        float gkSpeed_sale_z;
        float gk_distance_to_alert;
        float offset = 0.25f;

        public override void Init(AI ai)
        {
            type = types.POSITION;
            base.Init(ai);
            color = Color.green;
            _ballTransform = ai.ball.transform;
            gkSpeed_sale_z = Data.Instance.settings.GetSetting("gkSpeed_sale_z");
            gk_distance_to_alert = Data.Instance.settings.GetSetting("gk_distance_to_alert");            
        }
        public override void SetActive()
        {
            timer = 0;
            base.SetActive();
            ai.character.states.Stopped();

            float duelStats = CupsData.Instance.GetActualLevel().duelStatsGK;
            float limits_x = duelStats / 100;
            if (limits_x > offset) limits_x = offset;
            SetLimits(limits_x, gkSpeed_sale_z);
        }

        public override AIState UpdatedByAI()
        {
            timer += Time.deltaTime;
            if (timer < 0.1f) return State();            

            if (gameManager.state == Fulbo.Game.GameManager.states.GOAL) return State();
            if (ai.ball.character != null && ai.ball.character.teamID != ai.character.teamID && ai.IsBallInArea())
            {
                SetState(ai.aiAlertGK);
                return State();
            }
            if (ai.ball.character == null)
            {
                if (ai.ball.IsComingToGoal(ai.character.teamID, ai.GetDistanceToBallInX(), 3) || BallIsOnAirAndNear())
                {
                    SetState(ai.aiFlyingGK);
                    return State();
                }
            }
                       
            float distanceToBall = Vector3.Distance(_ballTransform.position, _transform.position);
            if (distanceToBall < 7)
            {
                if (ai.ball.character == null)
                {
                    if (ai.ball.IsComingToGoal(ai.character.teamID, ai.GetDistanceToBallInX(), 3) || BallIsOnAirAndNear())
                    {
                        SetState(ai.aiFlyingGK);
                        return State();
                    }
                }
            } 
            if (ai.ball.character != null && ai.ball.character == ai.character)
                SetState(ai.aiHasBallGK);
            else if ( CheckToAlert(_ballTransform.position, distanceToBall) )
                SetState(ai.aiAlertGK);
            else
            {
                Vector2 dest = CalculatePositionWithBall(offset);
               
                if ((dest.y < 0 && ai.character.transform.position.z - offset < ai.character.limits_y.y)
                    ||
                    (dest.y > 0 && ai.character.transform.position.z + offset > ai.character.limits_y.x))
                    ai.character.MoveTo(0,0);
                else if (dest.x != 0 || dest.y != 0)
                    ai.character.MoveTo(dest.x/1.5f, dest.y / 1.5f);
                else
                    SetState(ai.aiIdleGK);
            }

            return State();
        }
        bool CheckToAlert(Vector3 ballPos, float distanceToBall)
        {
            if (ai.ball.character != null && ai.ball.character.teamID == ai.character.teamID) return false;
            if (Mathf.Abs(ballPos.z) > gkSpeed_sale_z + 0.5f) return false;
            if (distanceToBall < gk_distance_to_alert)
                return true;
            return false;
        }
        public override void OnCatchBall()
        {
            SetState(ai.aiHasBallGK);
        }
        public override void OnCharacterCatchBall(Character character)
        {
            if (character.teamID == ai.character.teamID)
            {
                if(character == ai.character)
                    SetState(ai.aiHasBallGK);
                else
                    SetState(ai.aiIdleGK);
            }
        }
        Vector2 CalculatePositionWithBall(float offset)
        {
            Vector2 newPos = Vector2.zero;
            float to_z  = ai.ball.GetRaycastPos().z;
            float limits_x = ai.character.limits_y.x;
            if (Mathf.Abs(_transform.position.x) < Mathf.Abs(ai.originalPosition.x) - offset) // está fuera del limite de salida
            { if (ai.character.teamID == 1) newPos.x = 1; else newPos.x = -1; }

            float dist_x = Mathf.Abs(_ballTransform.position.x - _transform.position.x);

            if (to_z > limits_x) to_z = limits_x; else if (to_z < ai.character.limits_y.y) to_z = ai.character.limits_y.y;// si se pasa

            if (dist_x > 2)
                to_z /= 1 + (dist_x / 25); // offset para tender al centro si la pelota está lejos.

            if (Mathf.Abs(_transform.position.z - to_z) > 0.25f)
            {
                if (_transform.position.z > to_z) newPos.y = -1; else newPos.y = 1;
            }

            return newPos;
        }
    }

}