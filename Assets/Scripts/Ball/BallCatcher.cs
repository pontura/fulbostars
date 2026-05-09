using Fulbo.Stadiums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Fulbo.Game
{
    public class BallCatcher : MonoBehaviour
    {
        [SerializeField] Animation anim;
        [SerializeField] Transform container;
        public Transform pivot;
        [SerializeField] GameObject signal;
        states state;
        Character character;
        [SerializeField] GameObject arrow;
        public SpriteMask forceMask;
        float maxMaskWidth = 1.57f; // el width total en y de la barra
        float forceMaskValue;
        float speed;
        float value;
        AimState aimState;
        public bool isOn;
        int kickID;
        [SerializeField] LineSignal lineSignal;

        public enum AimState
        {
            STANDARD,
            CENTRO
        }
        public enum states
        {
            IDLE,
            GOT_IT,
            JUEGUITO,
            RUN,
            RUN_FAST,
            KICKING,
            GAMBETA
        }
        private void Awake()
        {
            character = GetComponent<Character>();
        }
        private void Start()
        {
            speed = Data.Instance.settings.GetSetting("forceBarSpeed");

            SetForce(0);

            Show(false);
        }
        public Vector3 GetForward() { return container.transform.forward; }
        void SetForce(float value )
        {
            if (forceMask == null)
                return;
            Vector3 s = forceMask.transform.localScale;
            forceMaskValue = value;
            s.y = maxMaskWidth * forceMaskValue;
            forceMask.transform.localScale = s;
        }
        public void OnGoal()
        {
            if (character.isBeingControlled)
                Show(false);
        }
        public Vector3 GetLookAt()
        {
            return pivot.transform.localEulerAngles;
        }
        Coroutine c;
        public void Catch(Ball _ball)
        {
            if (state == states.GOT_IT) return;
            pivot.gameObject.SetActive(true);
            pivot.transform.localEulerAngles = new Vector3(0, 90, 0);
            Reset();
            _ball.Catched(container);
            state = states.GOT_IT;
            if (character.type == Character.types.GOALKEEPER &&
                ((Data.Instance.matchData.team1Controlled && character.teamID == 1)
                || (Data.Instance.matchData.team2Controlled && character.teamID == 2)
                )
                )
            {
                if(c != null)
                    StopCoroutine(c);
                c = StartCoroutine(AutoKick());
            }
            if (!character.isBeingControlled) return;
            if (lineSignal != null)
            {
                ShowLineSignal(true);
                Show(true);
            }

        }
        public bool HasCharacterInPase()
        {
            return lineSignal.HasCharacterInLine();
        }
        public void LoseBall()
        {
            character.states.AimingKick(false);

            character.states.LoseBall();
            //if (character.states.currentState.type == CharacterStates.types.JUEGUITO) character.states.LoseBall();
            //if (character.states.currentState.type == CharacterStates.types.POWERUP) character.states.LoseBall();

            ShowLineSignal(false);
           // SetColors(Color.white);
            Reset();
        }
        //void SetColors(Color color)
        //{
        //    //if (colaArrow.color == color) return;
        //   // print("SetColors " + color);
        //    colaArrow.color = color;
        //    arrow.color = color;
        //}
        public void Reset()
        {
            signal.SetActive(false);
            state = states.IDLE;
            kickID = 0;
            if (character.type != Character.types.GOALKEEPER) PlayAnim("ball_idle");
            else  PlayAnim("ball_idle_gk");
            SetForce(0);
        }
        public void LookAt(Vector3 targetPos)
        {
            targetPos.y = pivot.transform.position.y;
            pivot.transform.LookAt(targetPos);
        }
        float maxRot = 3f;
        float timer = 0;

        public void RotateTo(Vector3 targetPos)
        {
            //evita los goles en contra:
            if (character.type == Character.types.GOALKEEPER)
            {
                if (character.teamID == 1 && targetPos.x > 0)
                    targetPos.x *= -1;
                else if (character.teamID == 2 && targetPos.x < 0)
                    targetPos.x *= -1;
            }

            timer += Time.deltaTime;
            if (kickID == 1)
            {
                Vector3 to = new Vector3();
                to.y = container.transform.position.y;
                to.z = targetPos.z * 10000;

                if (to.z > maxRot)
                    to.z = maxRot;
                else if (to.z < -maxRot)
                    to.z = -maxRot;

                to.x = (Fulbo.Game.GameManager.Instance.stadiumData.active.GetAssetBySelectedSize().size_x / 2) + 2;
                if (character.teamID == 1) to.x *= -1;

                LookAt(to);
                return;
            }
            else
            {
                if (timer > 0.5f)
                {
                    timer = 0;
                }
            }



            if (targetPos == Vector3.zero) return;

            pivot.transform.forward = targetPos;
        }
        public void Show(bool isBeingControlled)
        {
            state = states.IDLE;
            this.isOn = isBeingControlled;
            if (arrow != null)
                arrow.SetActive(isOn);
            pivot.gameObject.SetActive(isOn);
            signal.SetActive(false);

            if (!isBeingControlled)
                ShowLineSignal(false);
            else
                ShowLineSignal(true);

        }
        public void ForceResetSignal() // si haces un lujito por ejemplo:
        {
            state = states.GOT_IT;
            SetForce(0);
        }
        public void ShowLineSignal(bool isOn)
        {
            if (lineSignal != null)
                lineSignal.SetOn(isOn);
        }
        public void Jump()
        {
            PlayAnim("ball_Jump");
        }
        public void Gambeta()
        {
            state = states.GAMBETA;
            PlayAnim("ball_gambeta");
        }
        public void Jueguito()
        {
            state = states.JUEGUITO;
            PlayAnim("ball_jueguito");
        }
        public void ResetFastRun()
        {
            if (state != states.RUN_FAST) return;
            PlayAnim("ball_run");
            state = states.RUN;
        }
        void AudioKickSoft(float volume)
        {
            AudioManager.Instance.PlayBallSound(StadiumsData.Instance.active.ball_carry, volume);
        }
        public void SetState(states newState)
        {
            if (state == states.KICKING || !HasBall()) return;

            if (state == newState || state == states.RUN_FAST && newState == states.RUN)
                return;

            state = newState;
            if (state == states.IDLE)
                Idle();
            else
            {
                if (state == states.RUN_FAST)
                {
                    PlayAnim("ball_run_fast");
                    AudioKickSoft(1);
                    PushingBall(true, 0.7f);
                }
                else if (state == states.RUN)
                {
                    PlayAnim("ball_run");
                    PushingBall(true);
                }
            }
        }
        void PushingBall(bool isCarrining, float delayed = 0)
        {
            if (runningWithBall != null)
                 StopCoroutine(runningWithBall);
            if(isCarrining)
                runningWithBall = StartCoroutine(Kicking(delayed));
        }
        Coroutine runningWithBall;
        IEnumerator Kicking(float delayed)
        {
            yield return new WaitForSeconds(delayed);
            while (state == states.RUN || state == states.RUN_FAST)
            {
                AudioKickSoft(0.4f);
                yield return new WaitForSeconds(0.5f);
            }
        }
        void PlayAnim(string animName)
        {
            if (HasBall())
            {
                anim.Play(animName);
            }
        }
        public void Idle()
        {
            PushingBall(false);
            if (state == states.JUEGUITO || state == states.GOT_IT || state == states.KICKING) return;
            if (HasBall())
            {
                Reset();
                LookLeftOrRight();
                state = states.GOT_IT;
            }
        }
        void LookLeftOrRight()
        {
            Vector3 destLook = transform.position;
            destLook.x += 10 * transform.localScale.x;
            LookAt(destLook);
        }
        public bool HasBall()
        {
            if (GameManager.Instance.ball != null && GameManager.Instance.ball.character == this.character)
                return true;
            return false;
        }
        public void ResetJueguito()
        {
            Idle();
        }
        public void InitKick(int kickID)
        {
            signal.SetActive(true);
            this.kickID = kickID;
            value = 0;
            Color arrowColor = Color.yellow;
            switch (kickID)
            {
                case 1:
                    arrowColor = Color.red;
                    break;
            }
            //SetColors(arrowColor);
            state = states.KICKING;
        }
        void Update()
        {
            if (!isOn) return;
            if (state == states.KICKING)
            {
                value += speed * Time.deltaTime;
                SetForce(value);
            }
            if (ForceResetLine())
                ShowLineSignal(false);
        }
        bool ForceResetLine()
        {
            if (!lineSignal.gameObject.activeSelf) return false;
            if (!HasBall()) return true;
            return false;
        }
        public float GetForce()
        {
            return forceMaskValue/1.5f;
        }
        IEnumerator AutoKick()
        {
            yield return new WaitForSeconds(3.5f);
            if (HasBall())
            {
                GameManager.Instance.ball.Kick(CharacterStates.kickTypes.CENTRO);
                LoseBall();
            }
            Reset();
        }
    }
}