using UnityEngine;
using System;

namespace Fulbo.Game.AIs
{
    [Serializable]
    public class AIState
    {
        protected GameManager gameManager;
        protected float timeToReact;
        [HideInInspector] public AI ai;
        protected Transform _ballTransform;
        protected Transform _transform;
        protected float timer;
        protected Color color;
        AIState state;
        public GamePlay stats;
        public types type;
        public enum types
        {
            IDLE,
            ATTACKING,
            DEFENDING,
            POSITION,
            HASBALL,
            HASBALL_TRY_CENTRO,
            GOTOBALL,
            ALERT,
            FLYING,
            GOTO_FORCE_POSITION,
            FREEZE
        }
        public virtual void Init(AI ai)
        {
            gameManager = Fulbo.Game.GameManager.Instance;
            _transform = ai.transform;
            _ballTransform = ai.ball.transform;
            stats = ai.character.stats;
            this.ai = ai;
            state = this;
        }
        public virtual void GotoBall() { }
        public virtual void SetActive() { }
        public virtual void OnReset() { state = this; }
        public virtual AIState UpdatedByAI() { return this; }
        public virtual void OnCharacterCatchBall(Character character) { }
        public virtual void OnCatchBall() { }
        public virtual void OnBallNearOnAir() { }
        public virtual void OnBallHitCharacter() { }
        public virtual void OnCharacterInFront(Character character) { }

        public void ResetAll()
        {
            if (ai.character.type == Character.types.GOALKEEPER)
                SetState(ai.aiIdleGK);
            else
                SetState(ai.aiIdle);
        }
      //  void Reset() { state = this; OnReset(); }

        public void SetState(AIState _newState)
        {
          //  if (state == _newState) return;
            if (ai.character.track_DEBUG)
                Debug.Log("[ AI ____Cambió de:" + state.type + " --> " + _newState.type + "]");

            if (_newState == null) return;
            ai.currentState = _newState;
            ai.currentState.SetActive();
            ai.SetNewDebugColor(ai.currentState.color);
        }
        public AIState State()
        {
            //if (state == null)
            //    state = this;
            //if (state != this)
            //{
            //    AIState newState = state;
            //    newState.SetActive();
            //    state = this;
            //    ai.SetNewDebugColor(newState.color);
            //  //  Reset();
            //    return newState;
            //}
            return this;
        }
        public Vector2 CheckMoveTo(Character character, float _x, float _z, Vector3 dest)
        {
            //if(Vector2.Distance(new Vector2(_transform.position.x, _transform.position.z), new Vector2(dest.x, dest.z))<0.4)
            //    return Vector2.zero;

            float offset = 0.25f;
            float dist_x = Mathf.Abs(dest.x - _transform.position.x);
            float dist_z = Mathf.Abs(dest.z - _transform.position.z);

            if (dist_x < offset) _x = 0;
            if (dist_z < offset) _z = 0;

            if (_x != 0)
            {
                if (_transform.position.x - offset < character.limits_x.x && _x < 0 ||
                    _transform.position.x + offset > character.limits_x.y && _x > 0)
                    _x = 0;
            }
            if (_z != 0)
            {
                if (_transform.position.z - offset < character.limits_y.y && _z < 0 ||
                    _transform.position.z + offset > character.limits_y.x && _z > 0)
                    _z = 0;
            }
            return new Vector2(_x, _z);
        }
        public void Move(Vector3 gotoPosition, bool elseIdle, float speed = 1)
        {
            Vector3 pos = ai.transform.position;
            float _h = 0;
            float _v = 0;
            if (Vector3.Distance(pos, gotoPosition) > 0.5f)
            {
                if (Mathf.Abs(pos.x - gotoPosition.x) < 0.25f) _h = 0;
                else if (pos.x < gotoPosition.x) _h = speed;
                else _h = -speed;

                if (Mathf.Abs(pos.z - gotoPosition.z) < 0.25f) _v = 0;
                else if (pos.z < gotoPosition.z) _v = speed;
                else _v = -speed;

                ai.character.SetPosition(_h, _v);
            }
            else if(elseIdle)
            {
                SetState(ai.aiIdle);
            }

        }
        public bool BallIsOnAirAndNear() // si está cerca vuela pase lo que pase:
        {
            Rigidbody rb = ai.ball.rb;
            Vector3 myPos = ai.transform.position;
            Vector3 ballPos = ai.ball.transform.position;
            int teamID = ai.character.teamID;
            if (ai.ball.character != null) return false;

            if ((teamID == 1 && rb.velocity.x <= 0) || (teamID == 2 && rb.velocity.x >= 0)) return false;
            if (Mathf.Abs(ballPos.x + 4f) >= Mathf.Abs(myPos.x) && Mathf.Abs(ballPos.z) < 4.5f) // si está cerca vuela pase lo que pase:
            {
                return true;
            }
            return false;
        }       
        public float GetDirectionFiltered(float min, float max, float pos, float direction)
        {
            if (direction > 0 && pos >= max) return 0;
            if (direction < 0 && pos <= min) return 0;
            return direction;
        }
        public void SetLimits(float sale_x, float sale_z)
        {
            Vector2 limits_x;
            if (ai.character.teamID == 1)
            {
                limits_x.y = ai.originalPosition.x;
                limits_x.x = ai.originalPosition.x - sale_x;
            }
            else
            {
                limits_x.x = ai.originalPosition.x;
                limits_x.y = ai.originalPosition.x + sale_x;
            }
            ai.character.SetLimitsY(sale_z);
            ai.character.SetLimitsX(limits_x.x, limits_x.y);
        }
    }
}