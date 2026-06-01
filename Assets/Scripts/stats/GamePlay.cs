using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.Game;

namespace Fulbo
{
    [Serializable]
    public class GamePlay
    {
        public Character.types characterType;
        public float speed;
        public float speedRun;
        public float speedRunWithBall;
        public float freeze_by_kick;
        public float freeze_by_loseBall;
        public float freeze_by_dashBall;
        public float freeze_by_hit;
        public float freeze_dash;
        public float distance_to_dash_ai;   // distancia para hacer un dash
        public float dash_percent;          //probabilidades 0 a 100 para hacer un dash
        public float random_jump_a_dash;    //probabilidades de 0 a 10 las probabilidades de saltar un dash
        public float height_to_dominate_ball;
        public float speedWithBall;
        public float speedDash;
        public float speedRunFade;
        public float defenseDelay; //tiempo que espera para defender
        public float attackDelay;
        public float idleDelay;
        public float kickHard;
        public float kickHardAngle;
        public float kickSoft;
        public float kickSoftAngle;
        public float kickBaloon;
        public float kickBaloonAngle;
        public float kickHead;
        public float kickHeadAngle;
        public float kickChilena;
        public float kickChilenaAngle;
        public float kickCentro;
        public float kickCentroAngle;
        public float aim_to_poster; // Random: 1 = patea al lado del palo, 0 al centro
        public float duration_dash;
        public float lujito_multiplier;
        public float collider_radius; // radio de collision para agarrar la pelota
        public float collider_height; // radio de collision para agarrar la pelota en altura
        public float collider_radius_air; // radio de collision para agarrar la pelota en altura
        public float forceToDominateBallOnAir; // 6: es la fuerza horizontal bajo la cual domina la pelota
        public float collider_radius_dash_multiplier;
        public float probabilityToCrossBall = 40; // probabilidad de patear al palo opuesto.
        public int gk_catch_on_air;
        public float gkSpeed_speed_flying_multiply;
        public float cooldown_lujito;
        public float cooldown_dash;

        //hardcoded:
        public float delayToGotoToBall = 1f;
        public float timeFreezedDefending = 0.25f;
        public float timeUnfreezedDefending = 0.5f;
        public float delayToKickToGoal = 0.4f; // estando perfilado para pegarle al arco, se detiene este delay a cargar potencia
        public float powerup_SuperGoalKick_multiplier;


        
        //PLAYERS:
        public bool CanDash()
        {
            if (Utils.GetRandomFloatBetween(0, 100) < dash_percent)
                return true;
            return false;
        }
        public bool CheckToJumpDash()
        {
            if (Utils.GetRandomFloatBetween(0, 100) < random_jump_a_dash)
                return true;
            return false;
        }        
        public Vector3 AimGoal(float goalX, Character character, CharacterStates.kickTypes kickType)
        {
            Vector3 characterPos = character.transform.position;
            float arco_width = 5; //de -5 as 5
            float min_aim_z = aim_to_poster * 5;
            Vector3 dest = new Vector3(goalX, 0, Utils.GetRandomFloatBetween(min_aim_z - 1, arco_width - 0.6f));

            Character gk_opponent;
            if (character.teamID == 1)
                gk_opponent = Fulbo.Game.GameManager.Instance.charactersManager.GetCharacterByType(Character.types.GOALKEEPER, 2);
            else
                gk_opponent = Fulbo.Game.GameManager.Instance.charactersManager.GetCharacterByType(Character.types.GOALKEEPER, 1);

           
            if (gk_opponent.transform.position.z > 0) {
                float _probabilityToCrossBall = probabilityToCrossBall;
                if (
                kickType == CharacterStates.kickTypes.CHILENA || 
                kickType == CharacterStates.kickTypes.VOLEA || 
                kickType == CharacterStates.kickTypes.HEAD
                )
                    _probabilityToCrossBall *= 1.5f;
                if (Utils.GetRandomFloatBetween(0, 100) < _probabilityToCrossBall)
                    dest.z *= -1; // elije el palo opuesto:
            }   
            return dest;
        }
        public bool CanControllTheBallOnAir(float force)// fuerza para dominar o no la ball de los Players!
        {
            if (force < forceToDominateBallOnAir)
                return true;
            return false;
        }
        public void ChangeSpeedByResults(float diffGoalsValue) 
        {
            speed -= diffGoalsValue;
        }
        public void ChangeKickForceByResults(float diffGoalsValue) 
        {
            kickHard -= diffGoalsValue;
        }
        public void SetStatsByResults(int goalDiff) // 0=empate. si positivo va ganando
        {
            gk_catch_on_air += (goalDiff * -1 * 3);
        }

        public float GetJumpHeight(Character character)
        {
            Character.PositionsInGame posInGame = character.GetPosition();
            if (posInGame == Character.PositionsInGame.IN_AREA_DEFENDING)
            {
                return 4.5f + (character.characterStats.dexterity / 100);
            }
            else
                return 6.7f + (character.characterStats.dexterity / 100);
        }
       


        ///GOALKEEPER
        public float TimeGKStayIdle(Character goalkeeper)// delay de reaccion en idle del arquero
        {
            return 0.35f - (goalkeeper.characterStats.awareness / 300);
        }
        public float TimeGKStayAlert(Character goalkeeper)// delay de reaccion en alert del arquero
        {
            return 0.15f - (goalkeeper.characterStats.awareness / 500);
        }
        public bool GK_CanGrabBallOnAir(Ball ball, Character character, Character characterThatKicked)
        {
            if (ball.kickType == CharacterStates.kickTypes.KICK_POWERUP) return false;
            float rand = Utils.GetRandomFloatBetween(0, 100);
            if (ball.transform.position.y < 1.35f && rand < 90)  return true;
           // Debug.Log("GK Ball speed: " + speed + " check catch rand:" + rand + "   stats (gk_catch_on_air):" + gk_catch_on_air + " ball.lastTimeKicked: " + ball.lastTimeKicked + " time: " + Time.time);
            if (ball.lastTimeKicked + 1.75f < Time.time) return true;
            if (rand < gk_catch_on_air) return true;
            if (character.duelChecker.GoalKeeperCatchBall()) return true;
            return false;
        }
        public void InitStatsFromSettings()
        {
            powerup_SuperGoalKick_multiplier = Data.Instance.settings.GetSetting("powerup_SuperGoalKick_multiplier");
            gk_catch_on_air = (int)(Data.Instance.settings.GetSetting("gk_catch_on_air") * 100);
            gkSpeed_speed_flying_multiply = Data.Instance.settings.GetSetting("gkSpeed_speed_flying_multiply");
            forceToDominateBallOnAir = Data.Instance.settings.GetSetting("forceToDominateBallOnAir");
        }
        public void DuplicateFrom(GamePlay original)
        {
            powerup_SuperGoalKick_multiplier = original.powerup_SuperGoalKick_multiplier;
            // from settings
            gk_catch_on_air = original.gk_catch_on_air;
            gkSpeed_speed_flying_multiply = original.gkSpeed_speed_flying_multiply;
            forceToDominateBallOnAir = original.forceToDominateBallOnAir;
            powerup_SuperGoalKick_multiplier = original.powerup_SuperGoalKick_multiplier;

            //from charactersStats
            characterType = original.characterType;
            speed = original.speed;
            speedRun = original.speedRun;
            speedRunWithBall = original.speedRunWithBall;
            freeze_by_kick = original.freeze_by_kick;
            freeze_by_loseBall = original.freeze_by_loseBall;
            freeze_by_dashBall = original.freeze_by_dashBall;
            freeze_by_hit = original.freeze_by_hit;
            freeze_dash = original.freeze_dash;
            distance_to_dash_ai = original.distance_to_dash_ai;
            dash_percent = original.dash_percent;
            random_jump_a_dash = original.random_jump_a_dash;
            height_to_dominate_ball = original.height_to_dominate_ball;
            speedWithBall = original.speedWithBall;
            speedDash = original.speedDash;
            speedRunFade = original.speedRunFade;
            defenseDelay = original.defenseDelay;
            attackDelay = original.attackDelay;
            kickHard = original.kickHard;
            kickHardAngle = original.kickHardAngle;
            kickSoft = original.kickSoft;
            kickSoftAngle = original.kickSoftAngle;
            kickBaloon = original.kickBaloon;
            kickBaloonAngle = original.kickBaloonAngle;
            kickHead = original.kickHead;
            kickHeadAngle = original.kickHeadAngle;
            kickChilena = original.kickChilena;
            kickChilenaAngle = original.kickChilenaAngle;
            kickCentro = original.kickCentro;
            kickCentroAngle = original.kickCentroAngle;
            aim_to_poster = original.aim_to_poster;
            duration_dash = original.duration_dash;
            lujito_multiplier = original.lujito_multiplier;
            collider_radius = original.collider_radius;
            collider_height = original.collider_height;
            collider_radius_air = original.collider_radius_air;
            collider_radius_dash_multiplier = original.collider_radius_dash_multiplier;
            cooldown_lujito = original.cooldown_lujito;
            cooldown_dash = original.cooldown_dash;
        }



        ///BALL
        ///
        Vector3 GetNewForceByDistance(Character character, Vector3 dir)
        {
            float distanceToGoalNormalized = character.GetNormalizedDistanceToOGoal();
            float limit = 0.4f;
            if (distanceToGoalNormalized > limit)
            {
                dir.x /= 1.25f + (distanceToGoalNormalized - limit);
            }
            return dir;
        }
        public Vector3 GetBallKickDirection(CharacterStates.kickTypes kickType, Character character, float forceForce)
        {
            //Debug.Log("GetBallKickDirection " + kickType + forceForce);
            character.SetCollidersOff(freeze_by_kick);

            float force;
            if (forceForce != 0) force = forceForce; else force = character.ballCatcher.GetForce() + 1;

            if (kickType == CharacterStates.kickTypes.KICK_TO_GOAL && !character.isBeingControlled)
                force = Utils.GetRandomFloatBetween(1.2f, 2);
            bool duelLost = false;
            // Debug.Log("Kick CharacterStates.kickTypes " + kickType + ", force " + force);
            Ball ball = Fulbo.Game.GameManager.Instance.ball;
            Vector3 dir = ball.transform.forward;
            Vector3 velDistortion = Vector3.zero;
            if (kickType == CharacterStates.kickTypes.HARD || 
                kickType == CharacterStates.kickTypes.KICK_TO_GOAL || 
                kickType == CharacterStates.kickTypes.VOLEA ||
                kickType == CharacterStates.kickTypes.CHILENA)
            {
                duelLost = true;
                // if (character.isBeingControlled) {
                Character gk_opponent;
                int gk_team = 1;
                if (character.teamID == 1) gk_team = 2;
                gk_opponent = Fulbo.Game.GameManager.Instance.charactersManager.GetCharacterByType(Character.types.GOALKEEPER, gk_team);

                float dist = Vector3.Distance(ball.transform.position, gk_opponent.transform.position);
                Vector3 destDirection = ball.GetForcedRaycastPos();
                float distToGK = Mathf.Abs(destDirection.z - gk_opponent.transform.position.z);

                if (character.type != Character.types.GOALKEEPER && character.duelChecker.KickToGoalKeeperFails(gk_opponent, kickType, distToGK))
                {
                    ball.DuelLost(true);
                    //if lose A Duel then kicks to GK
                    if (dist < UnityEngine.Random.Range(2, 8))
                    {
                        character.ballCatcher.LookAt(gk_opponent.transform.position);
                        dir = character.ballCatcher.pivot.transform.forward;
                        Debug.Log("Iman al GK");
                        Fulbo.Game.GameManager.Instance.ball.KickBallTo(gk_opponent);
                    }
                    else
                    {
                        int rand = UnityEngine.Random.Range(0, 100);
                        if(rand < 60)
                        {
                            character.ballCatcher.LookAt(gk_opponent.transform.position);
                            dir = character.ballCatcher.pivot.transform.forward;
                            Debug.Log("Tira a las manos " + gk_opponent.transform.position + " dir: " + dir);
                        }
                        //else if(rand <80) // ARRIBA:
                        //{
                        //    velDistortion = Vector3.up * UnityEngine.Random.Range(180, 240);
                        //    Debug.Log("Tira ARRIBA " + velDistortion);
                        //}
                        else // PALOS:
                        {
                            Vector3 d = gk_opponent.transform.position;
                            d.z = UnityEngine.Random.Range(5.1f, 5.3f);
                            if (character.transform.position.z > 0)  d.z *= -1;
                            character.ballCatcher.LookAt(d);
                            dir = character.ballCatcher.pivot.transform.forward;
                            Debug.Log("Tira PALOS");
                        }
                    }
                }
            }
           // }            
            switch (kickType)
            {
                case CharacterStates.kickTypes.VOLEA:
                    dir *= kickHard * force * 2.5f;
                    dir = GetNewForceByDistance(character, dir);
                    Vector3 _up = Vector3.up * kickHardAngle / 2.5F;
                    if (_up.y > 320) _up.y = 320; // fuerza la altura
                    dir += _up;
                    break;
                case CharacterStates.kickTypes.HARD:
                    float _kickHard = kickHard;
                    if (
                        (character.states.currentState.type == CharacterStates.types.JUEGUITO)
                        ||
                         (character.states.currentState.type == CharacterStates.types.LUJITO)
                        )
                    {
                        _kickHard *= 1.5f;
                    }
                    dir *= _kickHard * force;
                    dir = GetNewForceByDistance(character, dir);

                    Vector3 u = Vector3.up * (kickHardAngle + Utils.GetRandomFloatBetween(-80, 20));// * (force/10f);
                    float maxHeight = 440;
                    if (u.y > maxHeight) u.y = maxHeight;

                    if (velDistortion != Vector3.zero) {
                        dir += velDistortion;
                    }
                    dir += u;
                    break;
                case CharacterStates.kickTypes.SOFT:
                    CharactersManager cm = Fulbo.Game.GameManager.Instance.charactersManager;
                    dir *= kickSoft * force;
                    dir += Vector3.up * kickSoftAngle * force;
                    break;
                case CharacterStates.kickTypes.BALOON:
                    dir *= kickBaloon * force;
                    dir += Vector3.up * kickBaloonAngle * force;
                    if (character != null && character.type == Character.types.GOALKEEPER)
                        dir += Vector3.up * 1.2f;
                    break;
                case CharacterStates.kickTypes.HEAD:
                    dir *= kickHead;
                    dir += Vector3.up * kickHeadAngle;// * force;
                    break;
                case CharacterStates.kickTypes.CHILENA:
                    dir *= kickChilena * Utils.GetRandomFloatBetween(1.2f, 2f);
                    //dir.up = 0;
                    break;
                case CharacterStates.kickTypes.KICK_TO_GOAL:
                    dir *= kickHard * Utils.GetRandomFloatBetween(0.8f, 1.2f);
                    dir += (Vector3.up * (kickHardAngle + Utils.GetRandomFloatBetween(-80, 20)));// * (force / 1.35f);
                    break;
                case CharacterStates.kickTypes.CENTRO:
                    dir *= kickCentro * (Fulbo.Stadiums.StadiumsData.Instance.active.GetAssetBySelectedSize().size_y * 1.3f);
                    dir += Vector3.up * kickCentroAngle * force;
                    break;
                case CharacterStates.kickTypes.KICK_POWERUP:
                    dir *= kickHard * Utils.GetRandomFloatBetween(2f, 2.5f) * powerup_SuperGoalKick_multiplier;
                    //if (character != null && character.type == Character.types.GOALKEEPER)
                    //    dir += Vector3.up * kickHardAngle * 2;
                    //else
                    dir += (Vector3.up * (kickHardAngle + Utils.GetRandomFloatBetween(-30, 20)));// * (force / 1.35f);
                    break;
            }
            return dir;
        }
    }
}