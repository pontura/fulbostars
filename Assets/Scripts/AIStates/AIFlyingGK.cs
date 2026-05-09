using UnityEngine;
namespace Fulbo.Game.AIs
{
    public class AIFlyingGK : AIState
    {
        float flyingDuration = 0.7f;
        float timeToChange = 0.5f;
        int totalStats;

        float gkSpeed_speed_flying_multiply;
        bool cancelJumpDirection;
        float sale_z;
        bool wonDuel;
        bool superFly;

        public override void Init(AI ai)
        {
            type = types.FLYING;
            sale_z = Data.Instance.settings.GetSetting("gkSpeed_sale_z")*2f;

            gkSpeed_speed_flying_multiply = ai.character.stats.gkSpeed_speed_flying_multiply;
            totalStats = ai.character.characterStats.GetTotal(false);
            base.Init(ai);
            color = Color.white;
        }
        public override void SetActive()
        {
            cancelJumpDirection = false;
            timer = 0;
            ai.character.states.Jump();

            float duelStats = CupsData.Instance.GetActualLevel().duelStatsGK;
            float limits_x = duelStats / 20;
            if (limits_x > 5) limits_x = 5;
            SetLimits(limits_x, sale_z);

            wonDuel = ai.character.duelChecker.GKCanFly();
            if (wonDuel)
            { 
                superFly = ai.character.duelChecker.GKFlyBetter();
                SetSuperFly(superFly);
            } else
                SetSuperFly(false);
        }
        void SetSuperFly(bool isOn)
        {
            ai.character.characterColliders.ChangeRadius(2, 0.7f);
            superFly = isOn;
        }
        public override AIState UpdatedByAI()
        {
            timer += Time.deltaTime;
            float _flyingDuration = flyingDuration;
            if (superFly)  _flyingDuration *= 1.5f;

            if (timer > timeToChange)
            {
                timer = 0;
                SetState(ai.aiIFreeze);
            }
            else if (timer < _flyingDuration && !cancelJumpDirection)
            {
                float _x = 0; float _z = 0;
                Vector3 dest = new Vector3(_ballTransform.position.x, 0, ai.ball.GetRaycastPos().z);
                dest = GetPositionInsideArea(dest);
                
                if (Vector2.Distance(new Vector2(_transform.position.x, _transform.position.z), new Vector2(dest.x, dest.z)) > 0.7f)
                {
                    if (ai.character.teamID == 1 && ai.ball.rb.velocity.x < 0) cancelJumpDirection = true;
                    else if (ai.character.teamID == 2 && ai.ball.rb.velocity.x > 0) cancelJumpDirection = true;

                    if (Mathf.Abs(_transform.position.x - dest.x) < 0.3f) _x = 0;
                    else if (_transform.position.x > dest.x) _x = -1;
                    else if (_transform.position.x < dest.x) _x = 1;

                    if (Mathf.Abs(_transform.position.z - dest.z) < 0.3f) _x = 0;
                    else if (_transform.position.z > dest.z) _z = -1;
                    else if (_transform.position.z < dest.z) _z = 1;

                    if (_x != 0) _x /= 6; //para que no avance muy rápido:

                    if (superFly)
                        ai.character.MoveTo(_x, _z);
                    else
                    {
                        if (!wonDuel)
                            _z =  _z * (gkSpeed_speed_flying_multiply / 6);
                        else
                            _z = _z * gkSpeed_speed_flying_multiply;
                        if (_z > 1) _z = 1;

                        ai.character.MoveTo(_x / 5, _z);
                    }
                }
            }
            return State();
        }
        public override void OnBallHitCharacter()
        {
            cancelJumpDirection = true;
        }
        public override void OnCatchBall()
        {
            SetState(ai.aiHasBallGK);
        }
        Vector3 GetPositionInsideArea(Vector3 to)
        {
            Vector3 dest = to;

            if (ai.character.teamID == 1 && ai.areaEnding_x > to.x)
                dest.x = ai.areaEnding_x;
            else if (ai.character.teamID == 2 && ai.areaEnding_x < to.x)
                dest.x = ai.areaEnding_x;

            if (dest.z > sale_z) dest.z = sale_z;
            else if (dest.z < -sale_z) dest.z = -sale_z;

            return dest;
        }
    }

}