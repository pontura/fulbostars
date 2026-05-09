using UnityEngine;
namespace Fulbo.Game.AIs
{
    public class AiHasBall : AIState
    {
        Vector3 dest;
        int _z = 0;
        Vector3 limits;
        public Vector2 stadiumSize;
        float initTimer;
        float timerJueguito;
        int teamID;
        Character character;
        Transform _transform;
        actionStates actionState;
        float delayToKickGoal;

        enum actionStates
        {
            RUNING,
            JUEGUITO,
            KICK
        }

        public override void Init(AI ai)
        {
            delayToKickGoal = ai.character.stats.delayToKickToGoal;
            type = types.HASBALL;
            initTimer = Time.time;
            stadiumSize = new Vector2(ai.stadiumAsset.size_x, ai.stadiumAsset.size_y);
            limits = new Vector2(stadiumSize.x, stadiumSize.y);
            base.Init(ai);
            color = Color.blue;
            _transform = ai.transform;
            character = ai.character;
            teamID = character.teamID;
        }
        public override void SetActive()
        {
            if (ai.ball.character == null || ai.ball.character != character)
                SetState(ai.aiIdle);
            else
            {
                timerJueguito = 0;
                dest.x = stadiumSize.x / 2 - Utils.GetRandomFloatBetween(6, 13);
                if (character.teamID == 1) dest.x *= -1;
                dest.z = Utils.GetRandomFloatBetween(-stadiumSize.y/4, stadiumSize.y / 4);
                actionState = actionStates.RUNING;
                timer = 0;
            }
        }
        public override void OnCatchBall()
        {
            SetState(ai.aiIdle);
        }
        public override AIState UpdatedByAI()
        {
            timer += Time.deltaTime;
            
            if (timer > 1)
            {
                timer = 0;
                if (ai.ball.character == null || ai.ball.character != character)
                {
                    SetState(ai.aiIdle);
                    return State();
                }
                if (CheckPase(_transform.position, 3))
                {
                    SetState(ai.aiIdle);
                    return State();
                }
                if (TryGotoCentro())
                {
                    SetState(ai.aiHasBallTryCentro);
                    return State();
                }
            }
            if (actionState == actionStates.JUEGUITO)
            {
                if (timerJueguito < 1.5f)
                    timerJueguito += Time.deltaTime;
                else
                    KickToGoal();
            }
            else
            if (timer > delayToKickGoal)
            {
                if ((teamID == 1 && _transform.position.x - 0.15f < dest.x) ||
                        (teamID == 2 && _transform.position.x + 0.15f > dest.x))
                    CheckToKickBall();
            }

            if (actionState == actionStates.RUNING)
                RunToGoal();

            return State();
        }
        bool TryGotoCentro()
        {
            float corner_z = Mathf.Abs(stadiumSize.y / 3);
            if (
                (teamID == 2
                && _transform.position.x + 2f > dest.x
                && Mathf.Abs(_transform.position.z) > corner_z)
                 ||
                 (teamID == 1
                 && _transform.position.x - 2f < dest.x
                 && Mathf.Abs(_transform.position.z) > corner_z)
                 )
                return true;
            return false;
        }
        int _x;
        void RunToGoal()
        {
            if (timer == 0)
            {
                if (!gameManager.isTutorial)
                {
                    character.SuperRun();
                    if (Mathf.Abs(_transform.position.z - dest.z) < 1) _z = 0;
                    else if (Random.Range(0, 10) < 5) _z = 1;
                    else _z = -1;
                }
            }
            if (Mathf.Abs(_transform.position.x - dest.x) < 0.2f)
            {
                if (timer > delayToKickGoal)
                {
                    CheckToKickBall();
                    return;
                }
                _x = 0;
            }
            else if (_transform.position.x < dest.x)
                _x = 1;
            else
                _x = -1;

            character.MoveTo(_x, _z);
        }
        void CheckToKickBall()
        {           
            if (ai.scoreState == AI.scoreStates.WIN && Random.Range(0, 15) < (ai.diffGoals + 1) * 2)
            {
                timer = 0;
                character.Jueguito();
                actionState = actionStates.JUEGUITO;
                timerJueguito = 0;
            }
            else
            {
                KickToGoal();
            }
        }
        void KickToGoal()
        {
            actionState = actionStates.KICK;
            character.Kick(CharacterStates.kickTypes.KICK_TO_GOAL);
            SetState(ai.aiPositionAttacking);
        }
        bool CheckPase(Vector3 originalPosition, float forward)
        {
            if (Random.Range(0, 15) < 5) return false;
            //Vector3 pos = _transform.position + (_transform.forward * 3);
            Character characterToPass = gameManager.charactersManager.GetNearestTo(character, teamID, false);

            if (characterToPass == null) return false;
            Vector3 otherPos = characterToPass.transform.position;

            //fuera de posicion de pase:
            if (character.teamID == 2 && otherPos.x < _transform.position.x - 1
                || character.teamID == 1 && otherPos.x > _transform.position.x + 1
                ) return false;

            float offset = 3;
            if (character.teamID == 1)
                otherPos.x -= offset;
            else if (character.teamID == 2)
                otherPos.x += offset;

            character.ballCatcher.LookAt(otherPos);

            float corner_x = (stadiumSize.x / 2) * 0.7f;
            //tipos de pase
            if (character.teamID == 2 && otherPos.x > corner_x || character.teamID == 1 && otherPos.x < -corner_x)
                character.Kick(CharacterStates.kickTypes.CENTRO, Utils.GetRandomFloatBetween(0.5f, 2));
            else
            {
                if (ai.ball.character != ai.character)
                {
                    SetState(ai.aiIdle);
                }
                else
                {
                    character.Kick(CharacterStates.kickTypes.SOFT, Utils.GetRandomFloatBetween(0.5f, 2));
                    ai.ball.PaseTo(characterToPass);
                }
            }
            return true;
        }
        public override void GotoBall() // si te la sacan:
        {
            SetState(ai.aiIdle);
        }
        public override void OnCharacterInFront(Character other)
        {
            if (timer < 0.25f) return;
            if (other.teamID != character.teamID)
            {
                if (other.transform.position.z < ai.transform.position.z) _z = 1; else _z = -1;
                _x = 0;
                timer = 0;

                //Check to pass ball to the other side of opponent:
                Vector3 pos = _transform.position;
                pos.z += 2 * _z;
                if (CheckPase(pos, 4))
                {
                    SetState(ai.aiIdle);
                }
                ////////////////////////////////////////////////////
            }
        }
        public override void OnCharacterCatchBall(Character character)
        {
            if (character.teamID == ai.character.teamID)
                SetState(ai.aiPositionAttacking);
            else
                SetState(ai.aiIFreeze);
        }
    }
}