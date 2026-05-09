using UnityEngine;
namespace Fulbo.Game.AIs
{
    public class AiHasBallTryCentro : AiHasBall
    {
        Vector3 dest;
        float center_goto_goal_x = 12;
        int _z = 0;
        Vector3 limits;
        float timerDestination;
        float initTimer;

        public override void Init(AI ai)
        {
            type = types.HASBALL_TRY_CENTRO;
            base.Init(ai);
        }
        public override void SetActive()
        {
            timer = 0;
            SetDestination();
        }
        void SetDestination()
        {
            dest.x = (stadiumSize.x / 2) - Utils.GetRandomFloatBetween(2.5f, 4.2f);

            float lateral = stadiumSize.y / 2;
            dest.z = Utils.GetRandomFloatBetween(lateral - 0.85f, lateral - 3.5f);

            if (ai.transform.position.z < 0)
                dest.z *= -1;
            if (ai.character.teamID == 1)
                dest.x *= -1;

        }
        public override AIState UpdatedByAI()
        {
            timer += Time.deltaTime;
            Run();
            return State();
        }
        void Centro()
        {
            Vector3 centroPos = ai.transform.position;
            centroPos.x *= 0.85f;
            centroPos.z *= -0.85f;
            ai.character.ballCatcher.LookAt(centroPos);
            float power = Utils.GetRandomFloatBetween(0.9f, 1.6f);
            ai.character.Kick(CharacterStates.kickTypes.CENTRO, power);
        }
        void Run()
        {
            if (timer > 0.7f)
            {
                ai.character.SuperRun();
                timer = 0;
            }

            int _x;
            if (Mathf.Abs(ai.transform.position.x - dest.x) < 0.15f)
                _x = 0;
            else if (ai.transform.position.x < dest.x)
                _x = 1;
            else
                _x = -1;

            int _z;
            if (Mathf.Abs(ai.transform.position.z - dest.z) < 0.15f)
                _z = 0;
            else if (ai.transform.position.z < dest.z)
                _z = 1;
            else
                _z = -1;

            if (_x == 0 && _z == 0)
            {
                Centro();
                SetState(ai.aiIdle);
            }
            else
                ai.character.MoveTo(_x, _z);
        }
    }
}