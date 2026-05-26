using Fulbo.Game.AIs;
using Fulbo.Game.Powerups;
using Fulbo.Stadiums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class Character : MonoBehaviour
    {
        public bool track_DEBUG;
        public string avatarName;
        
        [HideInInspector] public CharactersData.CharacterData dataSources;
        public int control_id; //for player Input;
        public float speed;
        float speed_multiplied_byPowerup = 1;
        public Transform characterContainer;
        public GamePlay stats;
        public types type;
        public CharacterStats characterStats;
        StatsUpgraderByResults statsUpgraderByResults;
        public DebufSystem debufSystem;
        public DuelChecker duelChecker;
        public CharacterMovement characterMovement;

        public enum types
        {
            DEF,
            MID,
            FOR,
            GOALKEEPER,
            REFERI
        }
        public fieldPositions fieldPosition;
        public enum fieldPositions
        {
            UP,
            CENTER,
            DOWN
        }
        [HideInInspector] public CharactersManager charactersManager;
        public int teamID;
        public int orderID; // the id of the order in the list:
        public TextsData.CharacterData data;
        // [HideInInspector] public CharacterActions actions;
        [HideInInspector] public CharacterSignal characterSignal;
        [HideInInspector] public BallCatcher ballCatcher;
        public bool isBeingControlled;
        [HideInInspector] public AI ai;
        [HideInInspector] public CharacterStates states;
        [HideInInspector] public float scaleFactor;
        [HideInInspector] public CharacterPowerupsManager powerupsManager;
        public Character oponent;
        public Vector2 limits_y;
        public Vector2 limits_x;
        CharacterFloorSignal floorSignal;
        StadiumsData.StadiumData stadiumData;
        Transform _cameraTransform;
        bool isGame;
        float _distanceToForceCentro;
        float _distanceToChilena;
        public CharacterColliders characterColliders;
        public CharacterFXManager characterFXManager;
        public CharactersPositions.CharacterPositionData characterPositionData;
        DB.DBUserData.DBCharacterData dbData;
        float happinessMultiplier = 1;

        void Awake()
        {
            powerupsManager = GetComponent<CharacterPowerupsManager>();
            characterColliders = GetComponent<CharacterColliders>();
            ballCatcher = GetComponent<BallCatcher>();
            floorSignal = GetComponent<CharacterFloorSignal>();
            characterFXManager = GetComponent<CharacterFXManager>();
            ai = GetComponent<AI>();
            states = GetComponent<CharacterStates>();
            characterMovement = GetComponent<CharacterMovement>();

            if (Fulbo.Game.GameManager.Instance != null)
                isGame = true;
        }
        void Start()
        {
           
            _cameraTransform = Fulbo.Game.GameManager.Instance.cameraInGame.transform;
            OnStart();
        }
        public void SetCharacterPositionData(CharactersPositions.CharacterPositionData characterPositionData)
        {
            this.characterPositionData = characterPositionData;
        }
        public virtual void OnStart() { }
        public void SetOponent()
        {
            if (oponent == null)
                oponent = charactersManager.GetOponentFor(this);
        }
        public void SetOrder(int orderID)
        {
            this.orderID = orderID;
        }
        public void Init(int _temaID, int characterID, CharactersManager charactersManager, GameObject asset_to_instantiate, DB.DBUserData.DBCharacterData dbData)
        {
            this.dbData = dbData;
            if (dbData.IsGoalkeeper())
                type = types.GOALKEEPER;
            else
                type = (types)dbData.position;
            Init(_temaID, characterID, charactersManager, asset_to_instantiate);
        }
        public void Init(int _temaID, int characterID, CharactersManager charactersManager, GameObject asset_to_instantiate)
        {
            
            stadiumData = StadiumsData.Instance.active;
            GamePlay gameplaySettings = Data.Instance.settings.GetStats(type, _temaID);
            gameplaySettings.InitStatsFromSettings();
            _distanceToForceCentro = Data.Instance.settings.GetSetting("distanceToForceCentro");
            _distanceToChilena = Data.Instance.settings.GetSetting("distanceToChilena");

            scaleFactor = Data.Instance.settings.GetSetting("scaleFactor");
            this.charactersManager = charactersManager;
            this.teamID = _temaID;

            if (floorSignal != null)
            {
                ClubData team2Data = Data.Instance.clubsData.GetData(2);
                if (teamID == 1)
                    floorSignal.Init(Data.Instance.clubsData.GetData(teamID).GetAnotherColor(team2Data.clubColor1));
                else
                    floorSignal.Init(team2Data.GetColor(1));
            }

            data = Data.Instance.textsData.GetCharactersData(characterID, type == Character.types.GOALKEEPER);
            dataSources = CharactersData.Instance.GetCharacterData(data.id, type == Character.types.GOALKEEPER);
            avatarName = CharactersData.Instance.GetCharacterData(characterID, type == Character.types.GOALKEEPER).avatarName;

            if (asset_to_instantiate == null)
                return;

            GameObject asset = Instantiate(asset_to_instantiate);
            asset.transform.SetParent(characterContainer);
            asset.transform.localEulerAngles = asset.transform.localPosition = Vector3.zero;
            asset.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

            states = GetComponent<CharacterStates>();
            states.Init(asset);
            if (Fulbo.Game.GameManager.Instance != null)
            {
                SetLimits();

                if (type == types.GOALKEEPER)
                {
                    SetLimitsY (6);
                    float goalkeeperOffset = 5f;
                    if (teamID == 1)
                    {
                        states.LookDirection(-1);
                        limits_x = new Vector2(limits_x.y - goalkeeperOffset, limits_x.y);
                    }
                    else
                        limits_x = new Vector2(limits_x.x, limits_x.x + goalkeeperOffset);
                }
            }

            if(Data.Instance.mode == Data.modes.PARTYMODE)
                SetPartymodeStats();
            else
            {
                SetStorymodeStats(characterID);
                floorSignal.SetActive(teamID == 2);
            }

            stats = StatsManager.SetIngameStats(gameplaySettings, dataSources, characterStats, happinessMultiplier, teamID == 2);

            statsUpgraderByResults = new StatsUpgraderByResults();
            statsUpgraderByResults.Init(stats, teamID, type);
            OverrideStats();
            ai.Init();
            if (characterColliders != null) characterColliders.Init(this);
            speed = stats.speed;

            duelChecker = new DuelChecker(); duelChecker.Init(this);
            if (type == types.GOALKEEPER)
            {
                debufSystem = new DebufSystem();
                debufSystem.Init(this);
                debufSystem.InitGk();
            }
        }
        void SetPartymodeStats()
        {
            characterStats.ForceStats((int)(Data.Instance.settings.GetSetting("stats_for_guestmode_opponents")*100));
           // if (!Data.Instance.settings.mainSettings.isArcade && teamID == 2)
           //     characterStats.ForceStats((int)(Data.Instance.settings.GetSetting("stats_for_guestmode_myteam") * 100));            
        }
        void SetStorymodeStats(int characterID)
        {
            if (dbData != null)
            {
                characterStats.ForceStats(dbData.accuracy, dbData.stamina, dbData.speed, dbData.dexterity, dbData.trickery);
            }
            else
            {
                if(Data.Instance.mode == Data.modes.PARTYMODE)
                {
                    int totalStatsForStoryMode = 15;
                    characterStats.ForceStats(totalStatsForStoryMode, totalStatsForStoryMode, totalStatsForStoryMode, totalStatsForStoryMode, totalStatsForStoryMode);
                    return;
                }
                LevelData levelData = CupsData.Instance.GetActualLevel();
                //LevelData levelData = CupsData.Instance.GetLevelData(10, 1, 10);

                if (levelData != null)
                {
                    CharacterStats defaultStatsByLevel = levelData.GetTotalStats(type);
                    characterStats = dataSources.GetStatsFromStoryMode(defaultStatsByLevel);
                }
                else
                {
                    int totalStatsForStoryMode = 15;
                    characterStats.ForceStats(totalStatsForStoryMode, totalStatsForStoryMode, totalStatsForStoryMode, totalStatsForStoryMode, totalStatsForStoryMode);
                }
            }

            if (teamID == 2)
            {
                int originalTypeIDByPosition = Data.Instance.myTeam.GetCharacterType(characterID);
                int happinessID = characterPositionData.GetHappiness(originalTypeIDByPosition, characterPositionData.type);
                happinessMultiplier = characterStats.GetHappinessMultiplier(happinessID);
            }
        }
        public void SetLimits()  {
            float _x = StadiumsData.Instance.active.GetAssetBySelectedSize().size_x / 2;
            float _y = StadiumsData.Instance.active.GetAssetBySelectedSize().size_y / 2;
            SetLimitsX(-_x, _x);
            SetLimitsY(_y);
        }
        public void SetLimitsY(float _limits_y)  {
            limits_y = new Vector2(_limits_y, -_limits_y);
        }
        public void SetLimitsX(float _limits_MIN, float _limits_MAX)   {
            limits_x = new Vector2(_limits_MIN, _limits_MAX);
        }

        void OverrideStats()
        {
            if (type == types.GOALKEEPER)
            {
                speed = Data.Instance.settings.GetSetting("gk_speed") * 10;
                stats.speed = speed;
            }
        }
        
        public bool IsPositionOutsideLimitsZ(float pos)
        {
            if (pos < limits_y.y || pos > limits_y.y)
                return true;
            return false;
        }
        void ResetSpeed(float _speed)
        {
            if (track_DEBUG) print("RESET SPEED" + _speed + " statsSpeed: " + stats.speed);
            if (runCoroutine != null) StopCoroutine(runCoroutine);
            runCoroutine = null;
            speed_multiplied_byPowerup = 1;
            speed = _speed;
        }
        public void OnCatch(Ball _ball, Character lastCharacterWithBall)
        {
            if (lastCharacterWithBall != null)
            {
                if (states.currentState.type == CharacterStates.types.DASH)
                {
                    lastCharacterWithBall.states.Hitted();
                    Events.OnCatchBallWithDash(this);
                }
                else
                {
                    int rand = Random.Range(0, 3);
                    if(rand >0)
                        AudioManager.Instance.PlaySound("shouts", "ingame/voices/game_vox_confused" + rand, false);
                    lastCharacterWithBall.states.Cry();
                }
            }
            Events.CharacterCatchBallFrom(this, lastCharacterWithBall);

            ResetSpeed(stats.speedWithBall);

            ballCatcher.Catch(ai.ball);

            //  if (!isBeingControlled)
            charactersManager.CharacterCatchBall(this);
            if (type == types.GOALKEEPER)
            {
                AudioManager.Instance.PlayBallSound(StadiumsData.Instance.active.ball_gk_catch);
                AudioManager.Instance.PlayCrowd(Fulbo.Game.GameManager.Instance.stadiumData.active.crowd_good);
            }
            else
                AudioManager.Instance.PlayBallSound(StadiumsData.Instance.active.ball_catch);

            Events.CharacterCatchBall(this);
        }

        public Vector3 bounceDirection;
        public void Bounce(Vector3 _bounceDirection)
        {
            if (ai != null && ai.ball.character == this)
            {
                ai.ball.ForceLoseBall(GetForward() * 1.5f);
                ballCatcher.LoseBall();
            }
            this.bounceDirection = _bounceDirection;
            states.Bounce();
        }
        void PenaltyDelayed()
        {
            Events.OnPenalty(this);
        }
        public virtual void SetPosition(float _x, float _y)
        {
            MoveTo(_x, _y);
        }
        public void StartKicking(int buttonID)
        {
            if (!states.CanMove()) return;
            if (states.currentState.type == CharacterStates.types.DASH 
                || states.currentState.type == CharacterStates.types.SPECIAL_ACTION
                || states.currentState.type == CharacterStates.types.LUJITO)
                return;

            if (states.currentState.type == CharacterStates.types.LUJITO)
            {
                if (buttonID == 1)
                    ai.ball.Kick(CharacterStates.kickTypes.VOLEA, 1.1f);
                else if (buttonID == 2)
                    ai.ball.Kick(CharacterStates.kickTypes.BALOON);
                return;
            }
            if (buttonID == 1)//patea al arco
            {
                LookAtGoal();
                states.AimingKick(true);
            }
            else if (buttonID == 2) // pase
                states.AimingKick(false);
            ballCatcher.InitKick(buttonID);
        }
        public void LookAtGoal()
        {
            float arco = (stadiumData.GetAssetBySelectedSize().size_x / 2);
            if (teamID == 1)
                arco *= -1;
            ballCatcher.LookAt(new Vector3(arco, 0, 0));
        }
        public void Kick(CharacterStates.kickTypes kickType, float forceForce = 0)
        {
            if ( ai.ball.GetCharacter() == this)
            {
                ai.ball.Kick(kickType, forceForce);
                ballCatcher.LoseBall();
                ResetSpeed(stats.speed);
                SetCollidersOff(0.5f);
                states.Kick(kickType);
            }
        }

        float lastTimeDash;
        public bool Dash()
        {
            if (!states.CanMove())  return false;

            bool block = lastTimeDash > 0 && (lastTimeDash + stats.cooldown_dash > Time.time);

            if (block) return false;

            lastTimeDash = Time.time;
            states.Dash();
            characterFXManager.OnDash(1);
            return true;
        }
        public void Hit() { if (!states.CanMove()) return; states.Hit(); }
        public void Jueguito() { if (!states.CanMove()) return; states.Jueguito(); }

        float lastTimeLujito = 0;
        public bool Lujito()
        {
            if (!states.CanMove()) return false;
            if (IsOnLujitoCooldown()) return false;
            ForceLujito();
            ballCatcher.ForceResetSignal();
            return true;
        }
        public bool IsOnLujitoCooldown()
        {
            return lastTimeLujito > 0 && (lastTimeLujito + stats.cooldown_lujito > Time.time);
        }
        public void ForceLujito()
        {
            lastTimeLujito = Time.time;
            states.Lujito();
        }
        float lastTimeGambeta;
        public bool Gambeta()
        {
            if (!states.CanMove()) return false;
            if (lastTimeGambeta > 0 && (lastTimeGambeta + stats.cooldown_lujito) > Time.time) return false;
            lastTimeGambeta = Time.time;
            states.Gambeta();
            ballCatcher.ForceResetSignal();
            return true;
        }
        public void ChangeSpeedTo(float value)
        {
            if (ai.ball.character == this)
                speed = stats.speedWithBall + value;
            else
                speed = stats.speed + value;
        }
        public void Freeze()
        {
            speed = 0;
            ChangeSpeedTo(0);
        }
        public bool IsMoving()
        {
            if (direction == Vector2.zero)
                return false;
            return true;
        }
        Vector2 direction = new Vector2();
        public Vector2 GetDirection()
        {
            return direction;
        }
        public virtual Vector3 MoveTo(float _x, float _y)
        {
          
            if (!states.CanMove())
            {
                characterColliders.ResetRigidBody();
                return Vector2.zero;
            }

            if (_x == 0 && _y == 0)
            {
                characterColliders.ResetRigidBody();
                states.Stopped();
                direction = Vector2.zero;
                ballCatcher.Idle();
                return Vector2.zero;
            }
            else
            {
                float minSpeed;
                if (ai.ball.character == this)
                    minSpeed = stats.speedWithBall;
                else
                    minSpeed = stats.speed;

                if(speed <= minSpeed)
                    states.Move(1);
                else
                    states.Move(2);

                int dir = 0;
                if (_x < 0)
                    dir = -1;
                else if (_x > 0)
                    dir = 1;
                states.LookTo(dir);
            }
            Vector3 aimVector = (Vector3.right * _x * speed * Time.deltaTime) + (Vector3.forward * _y * speed * Time.deltaTime);
            if (states.aiming)
            {
                _x = direction.x;
                _y = direction.y;
            }
            else
            {
                direction = new Vector2(_x, _y);
            }
            Vector3 pos = transform.position;

            float xtraSpeedX = 1;
            float xtraSpeedZ = 1;

            if (type == types.GOALKEEPER && ai.currentState == ai.aiFlyingGK)
            {
                xtraSpeedX = 0.7f;
                xtraSpeedZ = 2f;
            }

            Vector3 speedX = Vector3.right * _x * speed * xtraSpeedX;
            Vector3 speedZ = Vector3.forward * _y * speed * xtraSpeedZ;

            Vector3 forwardVector = (speedX + speedZ) * speed_multiplied_byPowerup * Time.deltaTime;

            if (forwardVector != Vector3.zero && states.currentState.type == CharacterStates.types.LUJITO)
                forwardVector *= stats.lujito_multiplier;
            if (states.aiming)
                aimVector = aimVector / (Time.deltaTime * 5000);

            if (ballCatcher != null && ballCatcher.isOn)
                ballCatcher.RotateTo(aimVector);


            if ((pos.x > limits_x.y && forwardVector.x > 0) || (pos.x < limits_x.x && forwardVector.x < 0))
                forwardVector.x = 0;
            if ((pos.z > limits_y.x && forwardVector.z > 0) || (pos.z < limits_y.y && forwardVector.z < 0))
                forwardVector.z = 0;

            if (forwardVector != Vector3.zero)
                transform.Translate(forwardVector);

            return forwardVector;
        }
        public void SetSignal(CharacterSignal signal)
        {
            characterSignal = signal;

            if (signal == null)
            {
                Debug.LogError("no hay signal");
                return;
            }

            signal.transform.localScale = Vector3.one;
            signal.MoveTo(transform);
        }
        public void SetControlled(bool _isBeingControlled)
        {            
            ballCatcher.Show(_isBeingControlled);
            this.isBeingControlled = _isBeingControlled;
            floorSignal.SetOn(_isBeingControlled);

            if (isBeingControlled)
            {
                if (charactersManager.ball.character == this)
                    ResetSpeed(stats.speedWithBall);
                else if (states.currentState.type != CharacterStates.types.DASH)
                    ResetSpeed(stats.speed);
            }
        }
        public void OnGoal(bool win)
        {
            ballCatcher.OnGoal();
            statsUpgraderByResults.OnGoal();

            if(win)
            {
                if(Random.Range(0,10)<6)
                    Events.SetDialogue(this, Data.Instance.textsData.GetRandomDialogue("goal", this.data.id, this.type == Character.types.GOALKEEPER));
            }

            if (ai.enabled)
                ai.ResetAll();
        }
        public void SetCollidersOff(float delay)
        {
            if (characterColliders != null)
                characterColliders.SetCollidersOff(delay);
        }
        public void ResetRigidBody()
        {
            characterColliders.ResetRigidBody();
        }
        public bool GetColliderState()
        {
            return characterColliders.GetColliderState();
        }
        public void MoveCollidersTo(Vector3 pos)
        {
            if (characterColliders != null)
                characterColliders.MoveCollidersTo(pos);
        }


        public void Jump()
        {
            if (type != types.GOALKEEPER)
                states.Jump();
        }
        Coroutine runCoroutine;
        public void SuperRun()
        {
            if (runCoroutine != null) return;
            characterFXManager.OnSuperRun();
            states.SuperRun();
            if(gameObject.activeSelf)
                runCoroutine = StartCoroutine(RunSpeedDesacelerate());
        }
        public IEnumerator RunSpeedDesacelerate()
        {
            float minSpeed;
            float to;
            if (ai.ball.character == this)
            {
                to = stats.speedRunWithBall;
                minSpeed = stats.speedWithBall;
            }
            else
            {
                to = stats.speedRun;
                minSpeed = stats.speed;
            }
            float fadeInDuration = 0.3f;
            float fadeOutDuration = 0.33f;

            if (isBeingControlled)
                Events.OnRun(fadeInDuration + fadeOutDuration + 0.2f);

            // sube en fade
            float duration = fadeInDuration;
            float d = 0;
            while (d < duration)
            {
                d += Time.deltaTime;
                speed = Mathf.Lerp(speed, to, 0.15f);
                yield return new WaitForEndOfFrame();
            }
            speed = to;

            // se mantiene arriba 1 seg:
            duration = fadeOutDuration;
            d = 0;
            while (d < duration)
            {
                d += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            //baja
            while (speed > minSpeed)
            {
                speed -= stats.speedRunFade * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            if (Fulbo.Game.GameManager.Instance.state == Fulbo.Game.GameManager.states.PLAYING)
            {
                if (type != types.GOALKEEPER) ballCatcher.ResetFastRun();
               // states.Move(1);
            }
            speed = minSpeed;
            ChangeSpeedTo(0);
            runCoroutine = null;
            yield break;
        }
        public enum PositionsInGame
        {
            COMMON,
            IN_AREA_ATTACKING,
            CENTRO,
            IN_AREA_DEFENDING
        }
        public PositionsInGame GetPosition()
        {
            float _x = transform.position.x;

            float distanceToForceCentro = (stadiumData.GetAssetBySelectedSize().size_x / 2) * _distanceToForceCentro;
            float distanceToChilena = (stadiumData.GetAssetBySelectedSize().size_x / 2) * _distanceToChilena;

            if (teamID == 1 && _x < -distanceToForceCentro ||
                teamID == 2 && _x > distanceToForceCentro)
            {
                if (Mathf.Abs(transform.position.z) > 7.5f)
                    return PositionsInGame.CENTRO;
                else
                    return PositionsInGame.IN_AREA_ATTACKING;
            }
            else if (teamID == 1 && _x < -distanceToChilena ||
                     teamID == 2 && _x > distanceToChilena)
            {
                return PositionsInGame.IN_AREA_ATTACKING;
            }
            if (
               (teamID == 1 && _x > distanceToForceCentro
               ||
               teamID == 2 && _x < -distanceToForceCentro))
            {
                return PositionsInGame.IN_AREA_DEFENDING;
            }
            return PositionsInGame.COMMON;
        }
        float fr_update;
        private void Update()
        {
            if (isGame)
            {
                states.UpdatedByCharacter();
                ai.UpdatedByCharacter();
                fr_update += Time.deltaTime;
                if (fr_update > 0.2f)
                {
                    characterContainer.forward = _cameraTransform.forward;
                    transform.localEulerAngles = Vector3.zero;
                    fr_update = 0;
                }
            }
            else
            {
                Vector3 dest = _cameraTransform.localEulerAngles;
                if (transform.localScale.x < 0)
                {
                    dest.y *= -1;
                    dest.z *= -1;
                }
                characterContainer.localEulerAngles = dest;
            }
        }
        public void SetSpeedMultiplier(float speed_multiplied_byPowerup, float timer)
        {
            this.speed_multiplied_byPowerup = speed_multiplied_byPowerup;
        }
        public Vector3 GetForward()
        {
            return ballCatcher.GetForward();
        }
        public bool GoingForward(float new_x)
        {
            if (teamID == 1 && new_x < 0)
                return true;
            if (teamID == 2 && new_x > 0)
                return true;
            return false;
        }
        public float GetNormalizedDistanceToOGoal()
        {
            float w = stadiumData.GetAssetBySelectedSize().size_x;
            float stadiumWidth = w/2;
            float myPos = transform.position.x;
            float dist = 0;
            if (teamID == 1) dist = (stadiumWidth) + transform.position.x;
            else dist = (stadiumWidth) + (transform.position.x*-1);
            return dist / w;
        }
    }

}