using Fulbo.Stadiums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.AIs
{

    public class AI : MonoBehaviour
    {
        [HideInInspector] public Character character;
        [HideInInspector] public Character characterWithBall;
        [HideInInspector] public Ball ball;

        [HideInInspector] public AIPositionDefending aiPositionDefending;
        [HideInInspector] public AIPositionAttacking aiPositionAttacking;
        [HideInInspector] public AiGotoBall aiGotoBall;
        [HideInInspector] public AiHasBall aiHasBall;
        [HideInInspector] public AiHasBallTryCentro aiHasBallTryCentro;
        [HideInInspector] public AIIdle aiIdle;
        [HideInInspector] public AiGotoForcePosition aiGotoForcePosition;

        [HideInInspector] public AiIdleGK aiIdleGK;
        [HideInInspector] public AiPositionGK aiPositionGK;
        [HideInInspector] public AIFlyingGK aiFlyingGK;
        [HideInInspector] public AiAlertGK aiAlertGK;
        [HideInInspector] public AIHasBallGK aiHasBallGK;
        [HideInInspector] public AIIFreeze aiIFreeze;

        public scoreStates scoreState;
        public enum scoreStates
        {
            DRAW,
            WIN,
            LOSE
        }
        public int diffGoals;

        public AIState currentState;
        public Vector3 originalPosition;

        public MeshRenderer debugAsset;
        public Fulbo.Stadiums.StadiumsData.StadiumAsset stadiumAsset;
        public float stadiumDataSizeX;
        GameManager _game;
        bool debugState;
        public float areaEnding_x;
        [SerializeField] bool isOn;

        public void SetOn(bool isOn)
        {
            this.isOn = isOn;

            if (!isOn)
                OnDestroy();
            else
            {
                Events.OnGoal += OnGoal;
                Events.CharacterCatchBall += CharacterCatchBall;
                Events.OnBallKicked += OnBallKicked;
                Events.SetCharacterNewDefender += SetCharacterNewDefender;
                Events.SetNewScores += SetNewScores;
                Events.OnBallHitCharacter += OnBallHitCharacter;
            }
        }
        public virtual void Init()
        {
            _game = Fulbo.Game.GameManager.Instance;
            if (_game.recordType == GameManager.recordTypes.PLAYING) this.enabled = false;

            debugState = Data.Instance.settings.mainSettings.debug;

            debugAsset.gameObject.SetActive(debugState);

            stadiumAsset = StadiumsData.Instance.active.GetAssetBySelectedSize();
            stadiumDataSizeX = stadiumAsset.size_x;

            originalPosition = transform.position;

            ball = _game.ball;
            character = GetComponent<Character>();

            SetOn(true);

            if (character.type == Character.types.GOALKEEPER)
            {
                if (Fulbo.Game.GameManager.Instance != null)
                    SetGoalkeeperValues();

                aiIdleGK = new AiIdleGK();
                aiPositionGK = new AiPositionGK();

                aiFlyingGK = new AIFlyingGK();
                aiAlertGK = new AiAlertGK();
                aiHasBallGK = new AIHasBallGK();
                aiIFreeze = new AIIFreeze();

                aiIdleGK.Init(this);
                aiPositionGK.Init(this);
                aiFlyingGK.Init(this);
                aiAlertGK.Init(this);
                aiHasBallGK.Init(this);
                aiIFreeze.Init(this);

                currentState = aiIdleGK;
            }
            else
            {
                aiIdle = new AIIdle();
                aiPositionDefending = new AIPositionDefending();
                aiPositionAttacking = new AIPositionAttacking();
                aiGotoBall = new AiGotoBall();
                aiHasBall = new AiHasBall();
                aiHasBallTryCentro = new AiHasBallTryCentro();
                aiGotoForcePosition = new AiGotoForcePosition();

                aiIdle.Init(this);
                aiPositionDefending.Init(this);
                aiPositionAttacking.Init(this);
                aiGotoBall.Init(this);
                aiHasBall.Init(this);
                aiHasBallTryCentro.Init(this);
                aiGotoForcePosition.Init(this);

                currentState = aiIdle;
            }
            ResetAll();
        }
        private void OnDestroy()
        {
            Events.OnGoal -= OnGoal;
            Events.CharacterCatchBall -= CharacterCatchBall;
            Events.OnBallKicked -= OnBallKicked;
            Events.SetCharacterNewDefender -= SetCharacterNewDefender;
            Events.SetNewScores -= SetNewScores;
            Events.OnBallHitCharacter -= OnBallHitCharacter;
        }
        public void OnBallHitCharacter(Character _character)
        {
            if(this.character == _character)
                currentState.OnBallHitCharacter();
        }
        public void SetGoalkeeperValues()
        {
            originalPosition.x = (stadiumDataSizeX / 2) - 1;
            if (character.teamID == 2)
                originalPosition.x *= -1;

            areaEnding_x = Mathf.Abs(originalPosition.x) - Data.Instance.settings.GetSetting("gkSpeed_sale_x");
            if (character.teamID == 2)
                areaEnding_x *= -1;

        }
        public void OnBallNearOnAir()
        {
            if (!isOn) return;

            if (_game.state != Fulbo.Game.GameManager.states.PLAYING) return;
            if (character.type == Character.types.GOALKEEPER)
            {
                currentState.OnBallNearOnAir();
                return;
            }
            if (character.isBeingControlled) return;
            if (
                character.states.currentState.type != CharacterStates.types.IDLE &&
                character.states.currentState.type != CharacterStates.types.RUN
               )
                return;
            currentState.OnBallNearOnAir();
            ResetAll();
            character.Jump();
        }
        public void OnBallNear()
        {
            if (!isOn) return;
            if (ball.character != null && ball.character.teamID == character.teamID && ball.character.type == Character.types.GOALKEEPER)
                return;

            //la tiene uno del otro equipo y no es el arquero:
            Character characterNearest = character.charactersManager.GetNearest(character.teamID, false, ball.transform.position, true);
            if (characterNearest == character)
            {
                ResetGoingToBall();
                currentState.GotoBall();
            }
            if(ball.characterThatKicked != null && character.type != Character.types.GOALKEEPER && ball.character == null && ball.characterThatKicked.teamID != character.teamID && ball.kickType == CharacterStates.kickTypes.SOFT)
            {
                if (!character.duelChecker.CanInterceptPassFrom(ball.characterThatKicked))
                {
                    character.characterColliders.SetCollidersOff(0.5f);
                    character.Jump();
                }
            }
        }
        void ResetGoingToBall()
        {
            foreach (Character ch in character.charactersManager.GetCharactersByTeam(character.teamID))
                if (ch != character && ch.ai.currentState is AiGotoBall)
                    ch.ai.ResetAll(); // si alguien lo seguía chau:
        }
        public void UpdatedByCharacter()
        {
            if (!isOn) return;
            if (_game.state != Fulbo.Game.GameManager.states.PLAYING) return;
            if (_game.recordType == GameManager.recordTypes.PLAYING) return;
            
            if (character.type != Character.types.GOALKEEPER && !character.states.CanMove()) return;
            if (character.isBeingControlled)
            {
                return;
                //if (currentState is AiHasBall)
                //{
                //    currentState.SetState(aiIdle);
                //    return;
                //}
            }
            if(currentState == null)
            {
                if (character.type != Character.types.GOALKEEPER)
                   currentState = aiIdle;
                else
                    currentState = aiIdleGK;
            }
            currentState.UpdatedByAI();
        }
        public void SetNewDebugColor(Color color)
        {
            if (debugState) debugAsset.material.color = color;
        }
        void OnGoal(int teamID, Character _character)
        {
            if (teamID != character.teamID && character.type == Character.types.GOALKEEPER)
            {
                //nada:
            }
            else
                ResetAll();
        }
        void SetNewScores(int team1, int team2)
        {
            scoreState = scoreStates.DRAW;
            if (character.teamID == 1)
            {
                if (team1 > team2)
                    scoreState = scoreStates.WIN;
                else if (team1 < team2)
                    scoreState = scoreStates.LOSE;
            }
            else
            {
                if (team1 < team2)
                    scoreState = scoreStates.WIN;
                else if (team1 > team2)
                    scoreState = scoreStates.LOSE;
            }
            diffGoals = Mathf.Abs(team1 - team2);
        }
        void OnBallKicked(CharacterStates.kickTypes kickType, float forceForce, Character _character)
        {
            if (!isOn) return;
            if (_character == null) //fue un cabezaso o algo insignificante...
                return;
            if (!character.states.CanMove()) return;
            float _z = _character.transform.position.z;
            if (currentState.type == AIState.types.GOTO_FORCE_POSITION)
                currentState.GotoBall();
            if (_character.type == Character.types.GOALKEEPER)
            {
                if (character.type == Character.types.MID)
                    currentState.GotoBall();
                else if (currentState is AiGotoForcePosition)
                    currentState.SetState(aiIdle);

                return;
            }
            switch (kickType)
            {
                case CharacterStates.kickTypes.CENTRO:
                    if (_character.teamID != character.teamID) // DEFIENDE!
                    {
                        if (character.type == Character.types.DEF)
                        {
                            if ((_z > 0 && character.fieldPosition == Character.fieldPositions.DOWN) || (_z < 0 && character.fieldPosition == Character.fieldPositions.UP))
                                currentState.GotoBall();
                        }
                    }
                    else // ATACA!
                    {
                        if (character.type == Character.types.FOR && character != _character)
                        {
                            currentState.GotoBall();
                        }
                    }
                    break;
            }
        }
        void SetCharacterNewDefender(Character _character)
        {
            if (!character.states.CanMove()) return;
            if (!isOn) return;
            if (character != _character) return;
            if (ball.character != null && ball.character.type == Character.types.GOALKEEPER) return;
            ResetGoingToBall();
            currentState.GotoBall();
        }
        public void CharacterCatchBall(Character characterWithBall)
        {
            if (!isOn) return;
            if (_game.state != Fulbo.Game.GameManager.states.PLAYING) return;
            if (characterWithBall == character)
            {
                currentState.OnCatchBall();
            } else
            if (characterWithBall.type == Character.types.GOALKEEPER)
            {
                if (character.type != Character.types.GOALKEEPER)
                    currentState.SetState(aiGotoForcePosition);
            }
            else
            {
                this.characterWithBall = characterWithBall;
                currentState.OnCharacterCatchBall(characterWithBall);
            }
        }
        public void ResetPosition()
        {
            currentState.ResetAll();
            transform.position = originalPosition;
        }
        public void SetInitialCharacterPosition()
        {            
            float _x = 0.9f;
            if (GameManager.Instance.isTutorial)
            {
                if (character.teamID == 2) _x *= -1.2f;
            }
            else
            {
                _x = 0.75f;
                if (character.teamID == 1)
                    _x *= -1;
            }
            transform.position = new Vector3(_x, originalPosition.y, 0);

            //if (!GameManager.Instance.isTutorial)
            //    character.states.LookAtBall();
        }
        public void ResetAll()
        {
            currentState.ResetAll();
        }

        
        public Vector2 MoveToBall(float min_x, float max_x, float min_z, float max_z)
        {
            Vector2 pos = new Vector2();
            Vector3 ballPos = ball.transform.position;
            float diff_z = Mathf.Abs(transform.position.z - ballPos.z);
            float diff_x = Mathf.Abs(transform.position.x - ballPos.x);

            float diff = 0.15f;

            if (diff_z > diff)
            {
                if (transform.position.z > ballPos.z && transform.position.z > min_z) pos.y = -1;
                else if (transform.position.z < ballPos.z && transform.position.z < max_z) pos.y = 1;
            }
            if (diff_x > diff)
            {
                if (transform.position.x > ballPos.x && transform.position.x > min_x) pos.x = -1;
                else if (transform.position.x < ballPos.x && transform.position.x < max_x) pos.x = 1;
            }
            return pos;
        }
        public bool IsBallInArea()
        {
            if (ball.transform.position.x > 0 && character.teamID == 2) return false;
            if (ball.transform.position.x < 0 && character.teamID == 1) return false;
            if (Mathf.Abs(ball.transform.position.x) > (originalPosition.x - 4)
               && Mathf.Abs(ball.transform.position.z) < 4.5f)
                return true;
            return false;
        }
        public float GetDistanceToBall()
        {
            return Vector3.Distance(transform.position, ball.transform.position);
        }
        public float GetDistanceToBallInX()
        {
            Vector3 pos = transform.position; pos.z = 0;
            Vector3 ballPos = ball.transform.position; ballPos.z = 0;
            return Vector3.Distance(pos, ballPos);
        }
        public void OnCharacterInFront(Character character) { currentState.OnCharacterInFront(character); }
    }
}