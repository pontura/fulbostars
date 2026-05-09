using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fulbo.Game.States;

namespace Fulbo.Game
{

    public class CharacterStates : MonoBehaviour
    {
        [HideInInspector] public StateIdle idle;
        [HideInInspector] public StateRun run;
        [HideInInspector] public StateKick kick;
        [HideInInspector] public StateDash dash;
        [HideInInspector] public StateJueguito jueguito;
        [HideInInspector] public StateFreeze freeze;
        [HideInInspector] public StateHit hit;
        [HideInInspector] public StateLujito lujito;
        [HideInInspector] public StateJump jump;
        [HideInInspector] public StateGoal goal;
        [HideInInspector] public StateHitted hitted;
        [HideInInspector] public StateBounce bounce;
        [HideInInspector] public StateCry cry;
        [HideInInspector] public StateThrow throwState;
        [HideInInspector] public StatePowerupCharging powerupCharging;
        [HideInInspector] public StateGambeta gambeta;

        public CharacterHitCollider hitCollider;
        public StateCharacter currentState;
        //public types currentStateType;
        public types lastState;
        public Character character;
        public Ball ball;
        [SerializeField] Animator anim;
        public kickTypes kickType;
        GameManager _game;
        bool GameScene;
        Data _data;
        bool recording;

        public enum types
        {
            IDLE,
            RUN,
            KICK,
            DASH,
            ACTION_DONE,
            SPECIAL_ACTION,
            GOAL,
            KICKED,
            FREEZE,
            JUMP,
            AIMING_KICK,
            LUJITO,
            JUEGUITO,
            JUMP_ATAJA,
            CRY,
            POWERUP,
            GAMBETA
        }
        public enum kickTypes
        {
            SOFT,
            HARD,
            BALOON,
            HEAD,
            CHILENA,
            KICK_TO_GOAL,
            DESPEJE_GOALKEEPER,
            CENTRO,
            VOLEA,
            KICK_POWERUP,
            BAD_KICK // lose the duel
        }
        private void Start()
        {
            if(GameRecorder.Manager.Instance().state == GameRecorder.Manager.states.RECORDING)
                recording = true;

            Events.CharacterCatchBall += CharacterCatchBall;
            Events.OnGoal += OnGoal;
            Events.OnGameStatusChanged += OnGameStatusChanged;
            Events.OnPowerupCharging += OnPowerupCharging;
        }
        private void OnDestroy()
        {
            Events.CharacterCatchBall -= CharacterCatchBall;
            Events.OnGoal -= OnGoal;
            Events.OnGameStatusChanged -= OnGameStatusChanged;
            Events.OnPowerupCharging -= OnPowerupCharging;
        }
        void OnPowerupCharging(bool isOn, Character character)
        {
            if(character == this.character)
            {
                currentState.OnPowerupCharge(isOn);
            }
        }
        void CharacterCatchBall(Character character)
        {
            if (GameManager.Instance.gameRecorder.state == GameRecorder.Manager.states.PLAYING) return;
            if (character == this.character)
            {
                if (currentState.type == types.DASH) return;

                //CancelInvoke();
                currentState.OnCatchBall();
            }
        }
        void OnGameStatusChanged(GameManager.states state)
        {
            if (state == GameManager.states.PLAYING)
            {
                Reset();
            }
        }
        public void Reset()
        {
            currentState.Unfreeze();
        }
        void OnGoal(int i, Character ch)
        {
            if (ch.type == Character.types.REFERI) return;
            if (character.teamID != ch.teamID)
            {
                if (character.type == Character.types.GOALKEEPER && currentState.type == types.JUMP_ATAJA) return;
                currentState.Cry();
            }
        }
        public void Init(GameObject asset)
        {
            _game = GameManager.Instance;
            _data = Data.Instance;
            character = GetComponent<Character>();
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (GameManager.Instance != null)
            {
                GameScene = true;
                ball = _game.ball;
            }
            else if (sceneName == "GameIntro" || sceneName == "GameOver")
            {
                CharactersManager cm = GetComponentInParent<CharactersManager>();
                ball = cm.ball;
            }
            if (asset != null)
            {
                anim = asset.GetComponent<Animator>();
                InitStates();
            }
            currentState = idle;
        }

        void InitStates()
        {
            idle = new StateIdle(); idle.Init(this);
            run = new StateRun(); run.Init(this);
            kick = new StateKick(); kick.Init(this);
            dash = new StateDash(); dash.Init(this);
            jueguito = new StateJueguito(); jueguito.Init(this);
            freeze = new StateFreeze(); freeze.Init(this);
            hit = new StateHit(); hit.Init(this);
            lujito = new StateLujito(); lujito.Init(this);
            jump = new StateJump(); jump.Init(this);
            goal = new StateGoal(); goal.Init(this);
            hitted = new StateHitted(); hitted.Init(this);
            bounce = new StateBounce(); bounce.Init(this);
            gambeta = new StateGambeta(); gambeta.Init(this);
            cry = new StateCry(); cry.Init(this);
            throwState = new StateThrow(); throwState.Init(this);
            powerupCharging = new StatePowerupCharging(); powerupCharging.Init(this);
        }
        public void UpdatedByCharacter()
        {
            if (GameScene && _game.state != GameManager.states.PLAYING && _game.state != GameManager.states.GOAL) return;
            if (currentState == null) return;
            currentState.Updated();
            //currentState = currentState.Updated();
            //StateCharacter sc = currentState.Updated();
            //if (sc != currentState)
            //{
            //    currentState = sc;
            //    currentState.Updated();
            //}
        }
        public void Stopped()
        {
            if (currentState.type == types.IDLE) return;
            if (CanMove())
                currentState.Stopped();
        }
        public void Move(float speed) // 1 es normal 2 es Boost
        {
            if (CanMove())
                currentState.Move(speed);
        }
        public void Dash() { currentState.Dash(); }
        public void Jueguito() { currentState.Jueguito(); }
        public void Lujito() { currentState.Lujito(); }
        public void Hitted()
        {
            currentState.Hitted();
            if(character.ballCatcher != null)
                character.ballCatcher.ShowLineSignal(false);
            AimingKick(false);
        }
        public void Bounce()
        {
            if (character.ballCatcher != null)
            {
                character.ballCatcher.ShowLineSignal(false);
                AimingKick(false);
                currentState.Bounce();
            }
        }
        public void Gambeta()
        {
            if (character.ballCatcher != null)
            {
                character.ballCatcher.ShowLineSignal(false);
                AimingKick(false);
                currentState.Gambeta();
            }
        }
        public void AimingKick(bool isOn) { aiming = isOn; }
        public void Jump() { currentState.Jump(); }
        public void Cry() {
            if (character.type != Character.types.REFERI)
                currentState.Cry();
        }
        public void Pita(int id) {
            switch (id)
            {
                case 0:
                    PlayAnim("start");
                    AudioManager.Instance.PlaySound("common", "ingame/referee/game_referee_start", false);
                    break;
                case 1:
                    PlayAnim("enter");
                    AudioManager.Instance.PlaySound("common", "ingame/referee/game_referee_opening", false);
                    break;
                case 2:
                    PlayAnim("start");
                    AudioManager.Instance.PlaySound("common", "ingame/referee/game_referee_end", false);
                    break;
            }
        }
        public void Goal() { currentState.Goal(); }
        public void EnterCancha() { currentState.Move(1); }
        public bool aiming;

        public void SuperRun()
        {
            if(CanMove() && currentState.type == types.RUN)
                currentState.Move(2);
        }

        public void Kick(kickTypes _kickType)
        {
            if (!CanMove()) return;
            kickType = _kickType;
            currentState.Kick(kickType);
            AimingKick(false);
            if(_kickType == kickTypes.HARD)
                if (character.teamID == 2) LookTo(1); else LookTo(-1);
        }

        int lookTo;
        public void LookTo(int v)
        {
            if (character.type == Character.types.GOALKEEPER)
                return;
            if (v != 0 && v != lookTo)
            {
                lookTo = v;
                LookDirection(v);
            }
        }
        public void LookDirection(int v)
        {
            Vector3 scale = transform.localScale;
            scale.x = v;
            transform.localScale = scale;
        }
        public void LookAtBall()
        {
            if (ball == null) return;
            if (ball.transform.position.x > transform.position.x)
                LookTo(1);
            else
                LookTo(-1);
        }
        public bool HasBall()
        {
            if (ball.character == character)
                return true;
            return false;
        }
        public bool CanMove()
        {
            if (GameManager.Instance.recordType == GameManager.recordTypes.PLAYING) return true;
            if (lastAnimPlayed == "enter") return false;
            if (currentState.type == types.FREEZE
                || currentState.type == types.GOAL
                || currentState.type == types.KICKED
                || currentState.type == types.KICK
                || currentState.type == types.SPECIAL_ACTION
                || currentState.type == types.JUMP
                || currentState.type == types.CRY 
                || currentState.type == types.POWERUP
                )
                return false;
            return true;
        }
        [SerializeField] string lastAnimPlayed = "";

        void PlayAnim(string animName)
        {
            if (lastAnimPlayed == animName) return;

            if (character.track_DEBUG)
                print("PlayAnim: " + animName);

            //if (recording && animName != "" && lastAnimPlayed != animName)
            //    GameManager.Instance.gameRecorder.KeyframeRecorder.RecordCharacter(character, animName);

            lastAnimPlayed = animName;
            if (anim == null) return;
            anim.Play(animName, 0, 0);
        }
        //public void ForceAnim(string animName)
        //{
        //    StopAllCoroutines();
        //    this.OnAnimDone = null;
        //    cantOverride = false;
        //    PlayAnim(animName);
        //}
        Coroutine ActionTillResetC;
        [SerializeField] bool cantOverride;
        System.Action OnAnimDone;
        public void PlayAnim(string animName, float duration = 0, System.Action _OnAnimDone = null, bool _cantOverride = false)
        {
            if (character.track_DEBUG)
                print("PLAY anim: " + animName + " can: " + (cantOverride && !_cantOverride) + " duration: " + duration + OnAnimDone + _cantOverride + cantOverride  + " currentSTATE:" + currentState.type);
            //if (cantOverride && !_cantOverride)  return;
            PlayAnim(animName);
          //  this.cantOverride = _cantOverride;
            
          //  if (duration > 0)
          //  {
                if (OnAnimDone != null) OnAnimDone();
                this.OnAnimDone = _OnAnimDone;
                if (ActionTillResetC != null)
                    StopCoroutine(ActionTillResetC);
                if (character.track_DEBUG)
                    print("START COROUTINE for:" + animName );
                if(character!=null && character.gameObject.activeSelf)
                    ActionTillResetC = StartCoroutine(ActionTillReset(duration));
           // }
        //    else cantOverride = false;
        }
        IEnumerator ActionTillReset(float duration)
        {
          //  if (character.data.id == 9) print("____StartCoroutine ActionTillReset" + duration);
            yield return new WaitForSeconds(duration);            
           // this.cantOverride = false;
            if (OnAnimDone != null) OnAnimDone();
            OnAnimDone = null;
            ActionTillResetC = null;
        }
        Coroutine WaitForCoroutine;
        public void Freeze(float duration)
        {
            if (duration <= 0) duration = 0.1f;
            if (WaitForCoroutine != null)
                StopCoroutine(WaitForCoroutine);
            WaitForCoroutine = StartCoroutine(WaitForDelayed(duration));
        }
        IEnumerator WaitForDelayed(float duration)
        {           
            yield return new WaitForSeconds(duration);
            PlayAnim("standup");
            yield return new WaitForSeconds(0.5f);
            currentState.Unfreeze();
            if (character.track_DEBUG) print("Unfreeze");
        }
        public void Hit()
        {
            if (hitCollider != null) hitCollider.Activate(true, 0.1f);
            currentState.Hit();
        }
        public void ThrowSomething()
        {
            currentState.ThrowSomething();
        }
        public void LoseBall()
        {
            // print("Lose ball " + currentState.type + " "  +character.data.avatarName);
            currentState.LoseBall();
        }
    }

}