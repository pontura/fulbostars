using System;
using UnityEngine;
using Fulbo.Onboarding;

namespace Fulbo
{
    public static class StatsManager
    {

        public static GamePlay SetIngameStats(GamePlay ingameStats, CharactersData.CharacterData characterData, CharacterStats characterStats, float happinessMultiplier, bool isCotrolledTeam)
        {
            float idleDelayToAdd = 0;

            if (!isCotrolledTeam)
            {
                idleDelayToAdd += CupsData.Instance.GetActualLevel().idleDelay;
            }

            float accuracy = (float)characterStats.accuracy * happinessMultiplier;
            float stamina = (float)characterStats.stamina * happinessMultiplier;
            float speed = (float)characterStats.speed * happinessMultiplier;
            float dexterity = (float)characterStats.dexterity * happinessMultiplier;
            float awareness = (float)characterStats.awareness * happinessMultiplier;

            GamePlay newIngameStats = new GamePlay();
            newIngameStats.DuplicateFrom(ingameStats);

            //SPEED:
            newIngameStats.speed = (ingameStats.speed + (speed / 70));
            newIngameStats.speedRun = (ingameStats.speedRun + (speed / 80));
            newIngameStats.speedRunWithBall = (ingameStats.speedRunWithBall + (speed / 80));
            newIngameStats.speedWithBall = (ingameStats.speedWithBall + (speed / 80));
            newIngameStats.speedDash = (ingameStats.speedDash + (speed / 60));
            newIngameStats.gkSpeed_speed_flying_multiply = (ingameStats.gkSpeed_speed_flying_multiply + (speed / 20));
            //newIngameStats.gk_speed = ingameStats.gk_speed + (speed / 70); esta en settings, por eso no funciona creo.

            //trickery
            newIngameStats.random_jump_a_dash = (ingameStats.random_jump_a_dash + (awareness / 20));
            newIngameStats.idleDelay = (ingameStats.idleDelay - (awareness / 250)) + idleDelayToAdd;
            newIngameStats.distance_to_dash_ai = (ingameStats.distance_to_dash_ai + (awareness / 100));
            newIngameStats.duration_dash = (ingameStats.duration_dash + (awareness / 90));
            newIngameStats.delayToGotoToBall = (newIngameStats.delayToGotoToBall - (awareness / 130)); // cuanto tarda en recalcular 0.5f= tontin 0.1 inteligente;

            //stamina:
            newIngameStats.freeze_by_dashBall = (ingameStats.freeze_by_dashBall - (stamina / 80));
            newIngameStats.freeze_by_hit = (ingameStats.freeze_by_hit - (stamina / 80));
            newIngameStats.freeze_by_kick = (ingameStats.freeze_by_kick - (stamina / 250));
            newIngameStats.freeze_by_loseBall = (ingameStats.freeze_by_loseBall - (stamina / 150));

            //accuracy:
            newIngameStats.aim_to_poster = (accuracy / 95);
            newIngameStats.forceToDominateBallOnAir = (ingameStats.forceToDominateBallOnAir + (accuracy / 30));
            newIngameStats.probabilityToCrossBall = (ingameStats.probabilityToCrossBall + (accuracy / 2));
            newIngameStats.kickCentro = (ingameStats.kickCentro + (accuracy / 60));
            newIngameStats.kickHard = (ingameStats.kickHard + (accuracy * 10));
            newIngameStats.kickSoft = (ingameStats.kickSoft + (accuracy * 2));


            //Dexterity
            newIngameStats.kickHardAngle = (ingameStats.kickHardAngle + dexterity);
            newIngameStats.kickBaloonAngle = (ingameStats.kickBaloonAngle + dexterity);
            newIngameStats.kickHeadAngle = (ingameStats.kickHeadAngle + dexterity);
            newIngameStats.kickChilenaAngle = (ingameStats.kickChilenaAngle + dexterity);

            newIngameStats.gk_catch_on_air = (int)(ingameStats.gk_catch_on_air + (dexterity / 4));
            newIngameStats.lujito_multiplier = (ingameStats.lujito_multiplier + (dexterity / 250));
            newIngameStats.freeze_dash = (ingameStats.freeze_dash - (dexterity / 180));
            newIngameStats.height_to_dominate_ball = (ingameStats.height_to_dominate_ball + (dexterity / 100));
            newIngameStats.collider_radius_dash_multiplier = (ingameStats.collider_radius_dash_multiplier + (dexterity / 100));

            //Tutorial lo hace muuuy fácil
            if (Data.Instance.mode == Data.modes.STORYMODE)
            {
                if (
                    (accuracy == 1 && stamina == 1 && speed == 1 && dexterity == 1 && awareness == 1)
                    ||
                    (!Data.Instance.onBoardingManager.IsBoardingStepDone(OnBoardingManager.BoardingStepStates.SECOND_MATCH_PLAYED) && !isCotrolledTeam)
                )
                {
                    newIngameStats.speed /= 1.8f;
                    newIngameStats.speedRun /= 1.8f;
                    newIngameStats.speedDash /= 1.8f;
                    newIngameStats.speedRunWithBall /= 1.8f;
                    newIngameStats.gkSpeed_speed_flying_multiply /= 1.8f;
                }
            }
            return newIngameStats;

        }
    }

}