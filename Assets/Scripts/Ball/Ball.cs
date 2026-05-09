using Fulbo.Stadiums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] GameObject shadow;
        GameObject asset;
        Transform container;
        BallAI ballAI;
        public Rigidbody rb;
        public Character character;
        public Character characterThatKicked;
        //  Vector3 limits;
        float timeCatched;
        public float lastTimeKicked;
        CombaFX combaFX;
        Settings settings;
        StadiumsData stadiumData;

        float catchedOffsetY = -1f;
        float offsetForward;
        float smooth;
        float stadium_size_x;
        GameManager game;
        BallRaycast ballRaycast;
        bool isRecordedMatch;
        public CharacterStates.kickTypes kickType;


        public void Init(GameObject _ballAsset)
        {
            if (GameRecorder.Manager.Instance().state == GameRecorder.Manager.states.PLAYING)
                isRecordedMatch = true;

            game = GameManager.Instance;
            transform.localEulerAngles = Vector3.zero;
            GameObject ballAsset = Instantiate(_ballAsset, transform);
            ballAsset.transform.localScale = Vector3.one;
            ballAsset.transform.localPosition = Vector3.zero;
            ballAsset.transform.localEulerAngles = Vector3.zero;
            asset = ballAsset.GetComponentInChildren<MeshRenderer>().gameObject; // TODO: encuentra el asset por su Render
            settings = Data.Instance.settings;
            stadiumData = StadiumsData.Instance;
            offsetForward = settings.GetSetting("ball_offsetForward");
            smooth = settings.GetSetting("ball_smooth");
            ballAI = GetComponent<BallAI>();
            Events.OnRestartGame += OnRestartGame;
            stadium_size_x = stadiumData.active.GetAssetBySelectedSize().size_x;

            container = transform.parent;
            this.rb = GetComponent<Rigidbody>();
            combaFX = GetComponent<CombaFX>();
            ballRaycast = GetComponent<BallRaycast>();
            Reset();

            if (isRecordedMatch)
            {
                ballAI.enabled = false;
                combaFX.enabled = false;
                ballRaycast.enabled = false;
                SetPhysics(false);
            }
        }
        public void SetPhysics(bool isOn)
        {
            GetComponent<Collider>().enabled = isOn;
            rb.isKinematic = !isOn;
        }
        private void OnDestroy()
        {
            Events.OnRestartGame -= OnRestartGame;
        }
        void OnRestartGame()
        {
            Reset();
        }
        public void KickIfOnGoal()
        {
            if (character != null)
                Kick(CharacterStates.kickTypes.HARD);
        }
        public void Reset()
        {
            if (isRecordedMatch)
            {
                SetPhysics(false);
                return;
            }
            rb.velocity = Vector3.zero;
            transform.position = new Vector3(0, 3, 0);
            FreeBall();
        }
        GamePlay GetStats()
        {
            if (character != null) return character.stats; return settings.gameplay;
        }

        public Character GetCharacter()
        {
            return character;
        }
        public void CharacterCatchBall(Character newCharacter)
        {
            if (this.character != null)
            {
                if (this.character.teamID == newCharacter.teamID)
                    return;

                Events.LoseBall(character);
                this.character.ballCatcher.LoseBall();
            }

            shadow.SetActive(false);

            Events.CatchBall(newCharacter);
            timeCatched = Time.time;
            characterThatKicked = newCharacter;
            newCharacter.OnCatch(this, this.character);

            this.character = newCharacter;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            ballRaycast.SetOff();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (isRecordedMatch) return;
            if (Data.Instance.newScene == "Penalty" && other.gameObject.tag == "Goal")
                Events.OnPenaltyGoal();
            if (game == null || game.state != Fulbo.Game.GameManager.states.PLAYING)
                return;
            if (other.gameObject.tag == "Goal")
            {
                int teamID = 1;
                if (transform.position.x > 0) teamID = 2;

                if (character != null) KickIfOnGoal();

                game.Goal(teamID, characterThatKicked);

                if (GameManager.Instance.recordType == GameManager.recordTypes.RECORDING)
                    GameRecorder.Manager.Instance().KeyframeRecorder.RecordEvent("goal", teamID.ToString(), characterThatKicked);
            }
            else if (other.gameObject.name == "Zone")
                Events.OnCharacterHitTrigger(other.gameObject);
        }
        public void KickByStrategy(Character _character, CharactersStrategy.receiverStates receiverState)
        {
            this.character = _character;
            characterThatKicked = character;
            CharacterStates.kickTypes newKickType = CharacterStates.kickTypes.VOLEA;
            float force = 1;

            if (kickType == CharacterStates.kickTypes.SOFT)
            {
                switch (receiverState)
                {
                    case CharactersStrategy.receiverStates.VOLEA:
                        newKickType = CharacterStates.kickTypes.VOLEA; force = 1; break;
                    case CharactersStrategy.receiverStates.PARED:
                        newKickType = CharacterStates.kickTypes.BALOON; force = 2; break;
                }
            }
            else if(kickType == CharacterStates.kickTypes.BALOON || kickType == CharacterStates.kickTypes.CENTRO)
            {
                switch (receiverState)
                {
                    case CharactersStrategy.receiverStates.VOLEA:
                        newKickType = CharacterStates.kickTypes.CHILENA;
                            force = 1; 
                        break;
                    case CharactersStrategy.receiverStates.PARED:
                        newKickType = CharacterStates.kickTypes.HEAD; force = 2; break;
                }
            }
            character.states.Kick(newKickType);
            Vector3 dest = AimGoal(character, newKickType);
            dest.y = 3;
            transform.LookAt(dest);
            Kick(newKickType, force);
        }
        public void OnEnterTrigger(Character _character)
        {
            if (isRecordedMatch || GameManager.Instance.state != GameManager.states.PLAYING) return;
            if (transform.localPosition.y > 0.95f)
            {
                Character.PositionsInGame p = _character.GetPosition();
                if (_character.type == Character.types.GOALKEEPER)
                {
                    _character.states.Jump();
                } else if (p == Character.PositionsInGame.IN_AREA_ATTACKING || p == Character.PositionsInGame.CENTRO)
                {
                    this.character = _character;
                    characterThatKicked = character;
                    character.states.Kick(CharacterStates.kickTypes.CHILENA);
                    Vector3 dest = AimGoal(character, CharacterStates.kickTypes.CHILENA);
                    dest.y = 3;
                    transform.LookAt(dest);
                    Kick(CharacterStates.kickTypes.CHILENA);
                }  else if( _character.duelChecker.CanDefendWithJumpAndHead(characterThatKicked))
                {
                    HeadToDefend(_character);
                }
            } 
        }
        void HeadToDefend(Character character)
        {
            this.characterThatKicked = character;
            this.character = character;

            Vector3 dest = AimGoal(character, CharacterStates.kickTypes.CHILENA);
            dest.y = 3;
            transform.LookAt(dest);

            Kick(CharacterStates.kickTypes.HEAD);
            character.Jump();
           // print("HeadToDefend " + character.avatarName + "  transform.eulerAngles: " + transform.eulerAngles + " teamID: " + character.teamID);
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (isRecordedMatch || GameManager.Instance.state == GameManager.states.GOAL) return;
            if (combaFX != null)
                combaFX.Reset();
            if (collision.gameObject.tag == "Floor")
            {
                float _y = rb.velocity.y;
                if (_y > 4.5f)
                {
                    AudioManager.Instance.PlayBallSound( StadiumsData.Instance.active.pica,
                        AudioManager.Instance.GetVolumeFor(4f, 7, rb.velocity.y));
                }
                else
                {
                    AudioManager.Instance.PlayBallSound(
                        StadiumsData.Instance.active.picaSoft,
                        AudioManager.Instance.GetVolumeFor(1.7f, 3, rb.velocity.y)
                        );
                }                    
                return;
            }
            if (collision.gameObject.tag == "Obstacle")
            {
                Events.OnBallHitObstacle(collision.gameObject);
                return;
            }
            if (collision.gameObject.tag == "GoalPalo")
            {
                float force = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);
                AudioManager.Instance.PlayBallSound(
                    StadiumsData.Instance.active.palo,
                    AudioManager.Instance.GetVolumeFor(0.25f, 5, force)
                    );
                if (collision.gameObject.transform.position.y > 2.75f)
                    Events.SayPalo(0);
                else
                    Events.SayPalo(1);
            }
            if (game == null) return;

            if (game.state != Fulbo.Game.GameManager.states.PLAYING)
                return;

            
            else if (character == null && collision.gameObject.tag == "lateral" || collision.gameObject.tag == "Corner")
            {
                float forceZ = Mathf.Abs( rb.velocity.z) + Mathf.Abs(rb.velocity.x);
                //print("Force wall: " + forceZ);
                if(forceZ<11)
                {
                    AudioManager.Instance.PlayBallSound(stadiumData.active.wallSoft,
                            AudioManager.Instance.GetVolumeFor(0, 11, forceZ), 2);
                }
                else
                {
                    AudioManager.Instance.PlayBallSound(stadiumData.active.wall,
                            AudioManager.Instance.GetVolumeFor(2, 20, forceZ), 2);
                }
              //  if (character != null)
                //    Kick(CharacterStates.kickTypes.BALOON, 2);
                    //ForceLoseBall(transform.forward * -100);
            }
            else if (collision.gameObject.tag == "Referi")
            {
                AudioManager.Instance.PlaySoundOneShot("ingame", "ingame/ball_hitref_hard", false);
                ballAI.Reset();
                Events.OnHitReferi(characterThatKicked);
                Character character = collision.gameObject.GetComponent<Character>();
                character.states.Hitted();
                Events.OnFX(FX.FXManager.types.REFEREE_HIT, transform.position);
                GameManager.Instance.charactersManager.charactersStrategy.ResetStates();
            }
            else if (collision.gameObject.tag == "Player")
            {
                ballAI.Reset();
                Character characterThatCollide = collision.gameObject.GetComponent<Character>();
                if (character == characterThatCollide) return;

                if (characterThatKicked != null && characterThatKicked.teamID != characterThatCollide.teamID)
                    GameManager.Instance.charactersManager.charactersStrategy.ResetStates();

                if (character != null && character.type == Character.types.GOALKEEPER) return; // si la tiene el arquero chau
                // Strategy manda que hacer con la pelota:
                else if (characterThatCollide.type == Character.types.GOALKEEPER)
                {
                    if (characterThatCollide.teamID == characterThatKicked.teamID)
                    {
                        GKDespeja(characterThatCollide);
                    }
                    else
                    {
                        characterThatCollide.debufSystem.DebufFatigue(); // Hace que se canse:
                        if (characterThatCollide.states.currentState.type != CharacterStates.types.FREEZE && characterThatCollide.stats.GK_CanGrabBallOnAir(this, characterThatCollide, characterThatKicked)) //Catch_on_air
                            CharacterCatchBall(characterThatCollide);
                        else
                        {
                            Events.OnBallHitCharacter(characterThatCollide);
                            AudioManager.Instance.PlayBallSound(StadiumsData.Instance.active.ball_gk_saca);
                            AudioManager.Instance.PlayCrowd(Fulbo.Stadiums.StadiumsData.Instance.active.crowd_good);
                        }
                    }                    
                }
                else if (GameManager.Instance.charactersManager.charactersStrategy.CheckForStrategy(characterThatCollide)) return;
                else if (character != null)
                    CheckToGetBall(characterThatCollide);
                else if(transform.localPosition.y < characterThatCollide.stats.height_to_dominate_ball)
                {
                    CheckToGetBall(characterThatCollide);
                }              
                else// if (transform.localPosition.y < characterThatCollide.stats.height_to_dominate_ball+1)
                {
                    //media altura:
                    float force = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);
                    bool canControllBall = characterThatCollide.stats.CanControllTheBallOnAir(force);
                    
                    print("Ball Height: " + transform.localPosition.y + "  force: " + force + "  canControllBall: " + canControllBall);
                    if (force > 11)
                        AudioManager.Instance.PlayBallSound(StadiumsData.Instance.active.ball_hit_character_hard);
                    else
                        AudioManager.Instance.PlayBallSound(StadiumsData.Instance.active.ball_hit_character_soft);

                    Character.PositionsInGame p = characterThatKicked.GetPosition();
                    if (p == Character.PositionsInGame.IN_AREA_DEFENDING)
                    {
                        HeadToDefend(characterThatCollide);
                    } else if (canControllBall && character == null)
                    {
                        print("canControllBall");
                        CharacterCatchBall(characterThatCollide);
                        return;
                    }
                    else
                    {
                        HeadToDefend(characterThatCollide);
                    }
                    Events.OnBallHitCharacter(characterThatCollide);
                }
            }
        }
        void GKDespeja(Character _character)
        {
            this.character = _character;
            character.characterColliders.SetCollidersOff(0.25f);
            characterThatKicked = character;
            character.states.Kick(CharacterStates.kickTypes.BALOON);
            Vector3 dest = new  Vector3();
            dest.z = Random.Range(-10, 10);
            dest.y = Random.Range(5, 8);
            dest.x = 0;
            transform.LookAt(dest);
            Kick(CharacterStates.kickTypes.BALOON, Random.Range(2, 3));
        }
        void Risa()
        {
            int rand = Random.Range(0, 3);
            if(rand == 0)
                AudioManager.Instance.PlaySoundOneShot("ingame", "ingame/voices/game_vox_yea", false);
            else
                AudioManager.Instance.PlaySoundOneShot("ingame", "ingame/voices/game_vox_laugh" + rand, false);
        }
        void CheckToGetBall(Character characterThatCollide)
        {
            bool canCatch = false;
            CharacterStates.types characterThatCollideState = characterThatCollide.states.currentState.type;
            CharacterStates.types characterState = CharacterStates.types.IDLE;
            if (character != null)
                characterState = character.states.currentState.type;

            if (character == null) canCatch = true;
            else if (characterState == CharacterStates.types.GAMBETA) canCatch = false;
            else if (characterThatCollide.type == Character.types.GOALKEEPER) canCatch = true;
            else if (characterThatCollideState == CharacterStates.types.KICKED) canCatch = false;
            else if (characterThatCollideState == CharacterStates.types.FREEZE) canCatch = false;
            else if (characterThatCollideState == CharacterStates.types.CRY) canCatch = false;
            else if (characterThatCollide.powerupsManager != null && characterThatCollide.powerupsManager.isOn)
                canCatch = false;
            else if (characterThatCollideState == CharacterStates.types.DASH && 
                !character.isBeingControlled && 
                character.stats.CheckToJumpDash() && 
                character.Lujito())
            {
                canCatch = false;
                Risa();
            }
            
            else if (
                characterState == CharacterStates.types.LUJITO
                || characterThatCollideState == CharacterStates.types.LUJITO
                )
            {
                Risa();
                canCatch = false;
            }
            else if (characterState == CharacterStates.types.DASH) canCatch = true;
            else if (character != characterThatCollide)
            {
                if (characterState == CharacterStates.types.IDLE)
                    canCatch = true;
                else if (characterThatCollideState == CharacterStates.types.DASH)
                {
                    if (characterState == CharacterStates.types.LUJITO)
                        canCatch = true;
                    else if (characterThatCollide.duelChecker.CanStealBallFromDashTo(character))
                        canCatch = true;
                    else
                    {
                        Risa();
                        character.ForceLujito();
                    }
                }
                else
                {
                    if (characterThatCollide.duelChecker.CanStealBallTo(character))
                        canCatch = true;
                    else
                    {
                        if(character.Gambeta())
                        {
                            Risa();
                            characterThatCollide.states.Cry();
                        } else
                            canCatch = true;
                    }
                }
            }
            if (canCatch)
                CharacterCatchBall(characterThatCollide);
        }
        Vector3 AimGoal(Character character, CharacterStates.kickTypes kickType)
        {
            float goalX = (stadium_size_x / 2);
            if (character.teamID == 1) goalX *= -1;

            Vector3 goalPos = character.stats.AimGoal(goalX, character, kickType);

            if (kickType == CharacterStates.kickTypes.KICK_POWERUP)
            {
                Vector3 dest;
                if (character.teamID == 1)
                    dest = GameManager.Instance.charactersManager.GetGoalkeeper(2).transform.position;
                else
                    dest = GameManager.Instance.charactersManager.GetGoalkeeper(1).transform.position;

                dest.z = UnityEngine.Random.Range(-4, 4);
                return dest;
            }
            return goalPos;
        }
        public void FreeBall()
        {
            shadow.SetActive(true);
            Vector3 rot = transform.eulerAngles;
            if (customContainer != null)
            {
                transform.localEulerAngles = customContainer.transform.eulerAngles;
                customContainer = null;
            }
            rb.constraints = RigidbodyConstraints.None;
            transform.SetParent(container);
            if (GameManager.Instance.recordType == GameManager.recordTypes.RECORDING &&
             GameManager.Instance.state == GameManager.states.PLAYING)
                GameManager.Instance.gameRecorder.KeyframeRecorder.RecordEvent("ballFree", "", null);
        }
        public void ForceLoseBall(Vector3 forceDirection)
        {
            FreeBall();
            rb.velocity = Vector3.zero;
            rb.AddForce(forceDirection);
            character = null;
        }
        bool duelLost;
        public void DuelLost(bool duelLost)
        {
            this.duelLost = duelLost;
        }
        public void Kick(CharacterStates.kickTypes kickType, float forceForce = 0)
        {
            if (isRecordedMatch) return;
            this.kickType = kickType;
            duelLost = false;
            lastTimeKicked = Time.time;
            ballAI.Reset();
            if (kickType == CharacterStates.kickTypes.HARD && character.ballCatcher.GetForce() > 0.5f)
            {
                if (character != null && !character.isBeingControlled)
                    kickType = CharacterStates.kickTypes.KICK_TO_GOAL;
            }
            if ( kickType == CharacterStates.kickTypes.KICK_POWERUP 
                || (kickType == CharacterStates.kickTypes.KICK_TO_GOAL && !character.isBeingControlled)
            )
            {
                character.ballCatcher.LookAt(AimGoal(character, kickType));
            }
            FreeBall();

            //El arquero saca por abajo:
            if (character != null && (character.type == Character.types.GOALKEEPER && kickType == CharacterStates.kickTypes.SOFT))
                transform.localPosition = new Vector3(transform.localPosition.x, -0.17f, transform.localPosition.z);

            Vector3 dir = transform.forward;

            if (character != null)
            {
                dir = character.stats.GetBallKickDirection(kickType, character, forceForce);
            }

            rb.velocity = Vector3.zero;
            rb.AddForce(dir);

            KickBallSound(kickType, dir);

            if (duelLost)
            {
                kickType = CharacterStates.kickTypes.BAD_KICK;
                Events.OnFX(FX.FXManager.types.BAD_KICK, transform.position);
            }
            else
                Events.OnFX(FX.FXManager.types.KICK, transform.position);

            Events.OnBallKicked(kickType, forceForce, character);
            character = null;

            ballRaycast.SetOn();
        }
        int kick_id = 1;
        void KickBallSound(CharacterStates.kickTypes kickType, Vector3 dir)
        {
            AudioClip ac;
            float totalForce = Mathf.Abs(dir.x) + Mathf.Abs(dir.y);

            switch (kickType)
            {
                case CharacterStates.kickTypes.CHILENA:
                    AudioManager.Instance.PlayBallSound(StadiumsData.Instance.active.kick_chilena);
                    break;
                case CharacterStates.kickTypes.VOLEA:
                        AudioManager.Instance.PlayBallSound(StadiumsData.Instance.active.kick_hard);
                    break;
                case CharacterStates.kickTypes.HARD:
                    if(totalForce<850)
                        AudioManager.Instance.PlayBallSound(StadiumsData.Instance.active.kick_soft);
                     else
                        AudioManager.Instance.PlayBallSound(StadiumsData.Instance.active.kick_hard);
                    break;
                case CharacterStates.kickTypes.SOFT:
                    AudioManager.Instance.PlayBallSound(StadiumsData.Instance.active.kick_pass);
                    break;
                case CharacterStates.kickTypes.CENTRO:
                case CharacterStates.kickTypes.BALOON:
                    AudioManager.Instance.PlayBallSound(StadiumsData.Instance.active.kick_balloon);
                    break;
                case CharacterStates.kickTypes.HEAD:
                    AudioManager.Instance.PlayBallSound(StadiumsData.Instance.active.kick_head);
                    break;
            }
            //Events.PlaySpecificSoundInArray(stadiumData.active.kick);
            kick_id++;
            if (kick_id > 4) kick_id = 1;
        }
        public void OnSetApplyForce(Vector3 dir, Character character)
        {
            rb.AddForce(dir);
            Events.OnBallKicked(CharacterStates.kickTypes.DESPEJE_GOALKEEPER, 0, character);
        }
        public float GetDurationOfBeingCatch()
        {
            return Time.time - timeCatched;
        }
        public void PaseTo(Character character)
        {
            if (!character.GetColliderState()) return;
            Events.BallPassedTo(character);
            ballAI.Pase(character);
        }
        public void KickBallTo(Character character)
        {
            Invoke("DelayedShout", 0.1f);
            ballAI.Pase(character);
        }
        void DelayedShout(string n)
        {
            string shout = "game_vox_hit" + UnityEngine.Random.Range(1, 4);
            AudioManager.Instance.PlaySoundOneShot("shouts", "ingame/voices/" + n, false);
        }

        float rotateSpeed = 5;
        Transform customContainer;

        private void Update()
        {
            // si se fue de la cancha se reacomoda:
            if(GameManager.Instance.state == GameManager.states.PLAYING && Mathf.Abs(transform.position.x)-1.5f> stadium_size_x/2)
            {
                Vector3 rePos = transform.position;
                rePos.x = (stadium_size_x / 2) - 2f;
                if (transform.position.x < 0)  rePos.x *= -1;
                transform.position = rePos;
                return;
            }
            /////////////////////////////////
            ///
            if (customContainer == null || character == null)
                return;

            float totalVelocity = 0;
           
            if (character.type != Character.types.GOALKEEPER)
            {
                totalVelocity = Mathf.Abs(character.GetDirection().x) * 100;

                if (totalVelocity < 1)
                    totalVelocity = Mathf.Abs(character.GetDirection().y) * 100;

                if (totalVelocity < 1)
                    totalVelocity = 0;
            }

            Vector3 dest = customContainer.transform.position + (customContainer.forward * offsetForward);
            dest.y += catchedOffsetY;
            transform.localEulerAngles = customContainer.transform.eulerAngles;
            transform.localPosition = Vector3.Lerp(transform.localPosition, dest, Time.deltaTime * smooth);

            if (totalVelocity < 1)
                asset.transform.localEulerAngles = Vector3.zero;
            else
                asset.transform.Rotate(Vector3.right, totalVelocity * Time.deltaTime * rotateSpeed);
        }
        public void Catched(Transform customContainer)
        {
            this.customContainer = customContainer;
            ForcePosition(customContainer);
        }
        public void ForcePosition(Transform customContainer)
        {
            transform.localPosition = customContainer.transform.position + new Vector3(0, catchedOffsetY, 0);
            rb.velocity = Vector3.zero;
            transform.localEulerAngles = customContainer.transform.localEulerAngles;
        }
        public Vector3 GetForwardPosition(float value)
        {
            Vector3 to = transform.localPosition + (transform.forward * value);
            return to;
        }
        public Vector3 GetProyectedPositionInGround()
        {
            if (GameManager.Instance.BallTrajectory == null) return Vector3.zero;
            else return GameManager.Instance.BallTrajectory.GetPosition();
        }
        public void HitObstacle()
        {
            ballAI.Reset();
        }
        public bool IsComingToGoal(int teamID, float distanceToBall, float distanceToActive)
        {
            if (distanceToBall < distanceToActive)
            {
                float ballSpeed_x = rb.velocity.x;
                if (ballSpeed_x > 0 && teamID == 1)
                    return true;
                if (ballSpeed_x < 0 && teamID == 2)
                    return true;
            }
            return false;
        }
        public bool IsDeadAndInArea(int teamID)
        {
            float area_x = (stadium_size_x / 2) - 4;// hasta donde sale en x;
            float area_z = 5;// hasta donde sale en z;

            float _x = transform.position.x;

            if (teamID == 2 && _x > 0) return false;
            if (teamID == 1 && _x < 0) return false;
            if (Mathf.Abs(transform.position.x) < area_x) return false;
            if (Mathf.Abs(transform.position.z) > area_z) return false;
            float vel = rb.velocity.x + rb.velocity.z;
            if (Mathf.Abs(vel) > 6) return false;
            print(teamID + " + _______IsDeadAndInArea vel:" + vel);

            return true;
        }
        public Vector3 GetRaycastPos()
        {
            if (character == null) return transform.position;
            Vector3 destPos = ballRaycast.GetPosition();
            return destPos;
        }
        public Vector3 GetForcedRaycastPos()
        {
            Vector3 destPos = ballRaycast.GetRayCast();
            return destPos;
        }
        public void ResetKick()
        {
            kickType = CharacterStates.kickTypes.HARD;
        }
    }

}