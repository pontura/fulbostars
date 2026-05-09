using UnityEngine;
using System.Collections;

namespace Fulbo.Game
{
    public class DuelChecker
    {
        Character character;
        int duelStats = 0;
        float powDuelStatsValue;
        bool IsFirstTimeInFirstCup;
        bool debug;
        int additionalValue = 0;
        bool isArcade;

        public void Init(Character character) {
             isArcade = Data.Instance.mode == Data.modes.PARTYMODE;

            LevelData ld = CupsData.Instance.GetActualLevel();
            if (ld.cupID == 1 && ld.tier == 1 && DB.DBManager.Instance.DbUserData.data.gameData.cups.GetCup(ld.cupID, ld.tier).timesWon == 0)
                additionalValue = 50;

            debug = DB.DBManager.Instance.versionMode == DB.DBManager.versionModes.DEV;
            this.character = character;

            if (isArcade)
                debug = false;
            else
            {
                IsFirstTimeInFirstCup = DB.DBManager.Instance.DbUserData.data.gameData.cups.IsStillPlayingFirstCup();
                powDuelStatsValue = Data.Instance.settings.GetSetting("PowDuelStatsValue");
                
                if (character.teamID == 1)
                {
                    if (character.type == Character.types.GOALKEEPER)
                        duelStats = CupsData.Instance.GetActualLevel().duelStatsGK;
                    else
                        duelStats = CupsData.Instance.GetActualLevel().duelStatsPlayer;
                }
                else
                    duelStats = 0;
            }
        }
        float GetAdditionValue(Character ch)
        {
            if (ch.teamID == 2) return additionalValue;
            return 0;
        }
        float SetPow(float v) {
            float n =  Mathf.Pow(v, powDuelStatsValue);
            if (float.IsNaN(n))
                return 0;
            return Mathf.Round(n);
            //return v *= v; 
        }
        string GetResultTypeString(bool myTeamWon, bool result)
        {
            string s = "";

            if (myTeamWon && result) s = "<color=green>";
            else  s = "<color=red>";

            s += result.ToString();
            s += "</color>";
            return s;
        }
        string GetCharacterTypeString(Character ch)
        {
            string s = SetColor(ch);
            s += ch.type.ToString();
            s += "</color>";
            return s;
        }
        string SetColor(Character ch)
        {
            if (ch.teamID == 1) return "<color=red>";
            return "<color=green>";
        }


        //DUELS: Goalkeeper:
        public bool GKFlyBetter()
        {
            Character characterThatKicked = GameManager.Instance.ball.characterThatKicked;
            if (characterThatKicked == null) return false;

            float total1 = characterThatKicked.characterStats.accuracy + characterThatKicked.duelChecker.duelStats;
            float total2 = character.characterStats.GetAverage() + duelStats;
            string t1 = "ACC";
            string t2 = "AV";
            if (GameManager.Instance.ball.kickType == CharacterStates.kickTypes.KICK_POWERUP)  total1 *= total1;

            total1 = SetPow(total1); total2 = SetPow(total2);

            float characterChance = Random.Range(0, total1);
            float goalKeeperChance = Random.Range(0, total2);

            bool result = (characterChance < goalKeeperChance);
            if (debug)
            {
                string ch1 = GetCharacterTypeString(characterThatKicked);
                string ch2 = GetCharacterTypeString(character);
                string duel = "[GK Fly-Better]";
                string s_characters = ch1 + " kicks against " + ch2 + " ";
                string s_result = GetResultTypeString(characterThatKicked.teamID == 2, result);
                string s_r_1 = (int)characterChance + SetColor(characterThatKicked) + "(max " + total1 + " = [" + t1 + ": " + characterThatKicked.characterStats.accuracy + " + DuelValue " + characterThatKicked.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                string s_r_2 = (int)goalKeeperChance + SetColor(character) + "(max " + total2 + " = [" + t2 + ": " + character.characterStats.GetAverage() + " + DuelValue " + character.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                if (GameManager.Instance.ball.kickType == CharacterStates.kickTypes.KICK_POWERUP) s_r_2 += " *POWERUP ";
                Events.Log(s_characters + duel + s_result + " " + ch1 + " rolls " + s_r_1 + " / " + ch2 + " rolls " + s_r_2);
            }
            return result;
        }
       
        //DUELS: Goalkeeper:
        public bool GKCanFly()
        {
            if (isArcade) return true;
            Character characterThatKicked = GameManager.Instance.ball.characterThatKicked;
            if (characterThatKicked == null) return false;

            float total1 = characterThatKicked.characterStats.accuracy + characterThatKicked.duelChecker.duelStats;
            float total2 = character.characterStats.GetAverage() + duelStats;

            string t1 = "ACC";
            string t2 = "AV";

            if (GameManager.Instance.ball.kickType == CharacterStates.kickTypes.KICK_POWERUP)  total1 *= total1;

            total1 = SetPow(total1); total2 = SetPow(total2);

            float characterChance = Random.Range(0, total1);
            float goalKeeperChance = Random.Range(0, total2);

            bool result = (characterChance < goalKeeperChance);
            if (debug)
            {
                string ch1 = GetCharacterTypeString(characterThatKicked);
                string ch2 = GetCharacterTypeString(character);
                string duel = "[GK Can-Fly]";
                string s_characters = ch1 + " kicks against " + ch2 + " ";
                string s_result = GetResultTypeString(characterThatKicked.teamID == 2, result);
                string s_r_1 = (int)characterChance + SetColor(characterThatKicked) + "(max " + total1 + " = [" + t1 + ": " + characterThatKicked.characterStats.accuracy + " + DuelValue " + characterThatKicked.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                string s_r_2 = (int)goalKeeperChance + SetColor(character) + "(max " + total2 + " = [" + t2 + ": " + character.characterStats.GetAverage() + " + DuelValue " + character.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                if (GameManager.Instance.ball.kickType == CharacterStates.kickTypes.KICK_POWERUP) s_r_2 += " *POWERUP ";
                Events.Log(s_characters + duel + s_result + " " + ch1 + " rolls " + s_r_1 + " / " + ch2 + " rolls " + s_r_2);
            }
            return result;
        }
        public bool GoalKeeperCatchBall()  // agarra o da rebote:
        {
            if (GameManager.Instance.ball.kickType == CharacterStates.kickTypes.KICK_POWERUP)
            {
                Events.Log("[GK-Catch-Ball] false by Powerup");
                return false;
            }
            Character characterThatKicked = GameManager.Instance.ball.characterThatKicked;
            if (characterThatKicked == null) return false;

            float total1 = characterThatKicked.characterStats.accuracy + characterThatKicked.duelChecker.duelStats;
            float total2 = character.characterStats.GetAverage() + duelStats;
            string t1 = "ACC";
            string t2 = "AV";

            total1 = SetPow(total1); total2 = SetPow(total2);

            float characterChance = Random.Range(0, total1);
            float goalKeeperChance = Random.Range(0, total2);

            bool result = (characterChance < goalKeeperChance);
            if (debug)
            {
                string ch1 = GetCharacterTypeString(characterThatKicked);
                string ch2 = GetCharacterTypeString(character);
                string duel = "[GK-Catch-Ball]";
                string s_characters = ch1 + " kicks against " + ch2 + " ";
                string s_result = GetResultTypeString(characterThatKicked.teamID == 2, result);
                string s_r_1 = (int)characterChance + SetColor(characterThatKicked) + "(max " + total1 + " = [" + t1 + ": " + characterThatKicked.characterStats.accuracy + " + DuelValue " + characterThatKicked.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                string s_r_2 = (int)goalKeeperChance + SetColor(character) + "(max " + total2 + " = [" + t2 + ": " + character.characterStats.GetAverage() + " + DuelValue " + character.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                Events.Log(s_characters + duel + s_result + " " + ch1 + " rolls " + s_r_1 + " / " + ch2 + " rolls " + s_r_2);
            }
            return result;
        }


        //DUELS: Players vs Goalkeeper:
        public bool KickToGoalKeeperFails(Character other, CharacterStates.kickTypes kickType, float ball_gk = 1)
        {
            if(isArcade)
                return false;

            float totalFailValue = 0;
            float distToFail = (character.ai.stadiumDataSizeX/2) - 8;
            float _x = character.transform.position.x;
            if (_x < 0 && character.teamID == 2) return true;
            if (_x > 0 && character.teamID == 1) return true;
            distToFail -= Mathf.Abs(_x);

            float total1 = character.characterStats.accuracy + duelStats + GetAdditionValue(character);
            float total2 = other.characterStats.GetAverage() + other.duelChecker.duelStats + GetAdditionValue(other);

            string xtra = " [dist: " + distToFail + "]";
            if (distToFail > 0)
            {
                totalFailValue = ((100 - character.characterStats.stamina) * distToFail) / 100;
                xtra = " [STA=" + character.characterStats.stamina + " -FailValue:" + totalFailValue + "->("+ (total1 - totalFailValue)+ ")] ";
            }
            total1 -= totalFailValue*1.5f;

            float directionValue = (ball_gk * character.characterStats.dexterity)/10;
            xtra += " [dist to GK: " + ball_gk + " da:" + directionValue + " (dex: " + character.characterStats.dexterity + ")] ";
            total1 += directionValue;

            Debug.Log("total1: " + total1 + " pow: " + SetPow(total1));

            string t1 = "ACC";
            string t2 = "AV";

            total1 = SetPow(total1); total2 = SetPow(total2);

            float characterChance = Random.Range(0, total1);
            float goalKeeperChance = Random.Range(0, total2);

            bool result = (characterChance < goalKeeperChance);
            if (debug)
            {
                string ch1 = GetCharacterTypeString(character);
                string ch2 = GetCharacterTypeString(other);
                string duel = "[Kick-Fails]";
                string s_characters = ch1 + " kicks against " + ch2 + " ";
                string s_result = GetResultTypeString(character.teamID == 2, result);
                string s_r_1 = (int)characterChance + SetColor(character) + "(max " + total1 + " = [" + t1 + ": " + character.characterStats.accuracy + xtra + " + DuelValue " + character.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                string s_r_2 = (int)goalKeeperChance + SetColor(other) + "(max " + total2 + " = [" + t2 + ": " + other.characterStats.GetAverage() + " + DuelValue " + other.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                Events.Log(s_characters + duel + s_result + " " + ch1 + " rolls " + s_r_1 + " / " + ch2 + " rolls " + s_r_2);
            }
            return result;
        }
        void WinLujitoCooldown(Character other)
        {
            string ch1 = GetCharacterTypeString(character);
            string ch2 = GetCharacterTypeString(other);
            Events.Log(ch1 + " gana por Lujito cooldown a " + ch2);
        }
        //DUELS: Players:
        public bool CanStealBallFromDashTo(Character other)
        {
            if (isArcade)
                return true;
            if (other.IsOnLujitoCooldown()) { if (debug) WinLujitoCooldown(other); return true; }
            float total1 = character.characterStats.awareness + duelStats + GetAdditionValue(character);
            float total2 = other.characterStats.awareness + other.duelChecker.duelStats + GetAdditionValue(other);
            string t1 = "AWA";
            string t2 = "AWA";

            total1 = SetPow(total1); total2 = SetPow(total2);

            float characterChance = Random.Range(0, total1);
            float otherChance = Random.Range(0, total2);

            bool result = (characterChance > otherChance);
            if (debug)
            {
                string ch1 = GetCharacterTypeString(character);
                string ch2 = GetCharacterTypeString(other);
                string duel = "[Can-DashSteal]";
                string s_characters = ch1 + " dash against " + ch2 + " ";
                string s_result = GetResultTypeString(character.teamID == 2, result);
                string s_r_1 = (int)characterChance + SetColor(character) + "(max " + total1 + " = [" + t1 + ": " + character.characterStats.awareness + " + DuelValue " + character.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                string s_r_2 = (int)otherChance + SetColor(other) + "(max " + total2 + " = [" + t2 + ": " + other.characterStats.awareness + " + DuelValue " + other.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                Events.Log(s_characters + duel + s_result + " " + ch1 + " rolls " + s_r_1 + " / " + ch2 + " rolls " + s_r_2);
            }
            return result;
        }
        public bool CanStealBallTo(Character other)
        {
            if (isArcade)
                return true;
            //  if (other.IsOnLujitoCooldown()) { if (debug) WinLujitoCooldown(other); return true; }
            float total1 = character.characterStats.dexterity + duelStats + GetAdditionValue(character);
            float total2 = other.characterStats.dexterity + other.duelChecker.duelStats + GetAdditionValue(other);

            total1 = SetPow(total1); total2 = SetPow(total2);

            float rand1 = Random.Range(1, total1);
            float rand2 = Random.Range(1, total2);

            string moreData = "";

            if (other.states.currentState.type == CharacterStates.types.IDLE) { rand2 /= 50f; moreData += " <color=red>(idle /= 50)</color>"; } ;
            if (character.states.currentState.type == CharacterStates.types.DASH) { rand1 *= 4; moreData += " <color=green>(dashing *= 4)</color>"; } ;

            string t1 = "DEX";
            string t2 = "DEX";


            float characterChance = Random.Range(0, total1);
            float otherChance = Random.Range(0, total2);

            bool result = (characterChance > otherChance);
            if (debug)
            {
                string ch1 = GetCharacterTypeString(character);
                string ch2 = GetCharacterTypeString(other);
                string duel = "[Can-Steal]";
                string s_characters = ch1 + " steal to " + ch2 + " ";
                string s_result = GetResultTypeString(character.teamID == 2, result);
                string s_r_1 = (int)characterChance + SetColor(character) + "(max " + total1 + " = [" + t1 + ": " + character.characterStats.dexterity + " + DuelValue " + character.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") "  +  "</color>";
                string s_r_2 = (int)otherChance + SetColor(other) + "(max " + total2 + " = [" + t2 + ": " + other.characterStats.dexterity + " + DuelValue " + other.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                Events.Log(s_characters + duel + s_result + " " + ch1 + " rolls " + s_r_1 + " / " + ch2 + " rolls " + s_r_2 + " " + moreData);
            }
            return result;
        }
        public bool CanDefendWithJumpAndHead(Character other)
        {
            if (isArcade)
            {
                if (Random.Range(0, 100) < 50) return true;
                return false;
            }
            if (other.teamID == 1 && other.transform.position.x < 0) return false;
            if (other.teamID == 2 && other.transform.position.x > 0) return false;
            if (other == null) return false;

            float total1 = character.characterStats.dexterity + duelStats + GetAdditionValue(character);
            float total2 = other.characterStats.dexterity + other.duelChecker.duelStats + GetAdditionValue(other);

            total1 = SetPow(total1); total2 = SetPow(total2);

            float rand1 = Random.Range(1, total1);
            float rand2 = Random.Range(1, total2);

            string moreData = "";

            string t1 = "DEX";
            string t2 = "DEX";

            float characterChance = Random.Range(0, total1);
            float otherChance = Random.Range(0, total2);

            bool result = (characterChance > otherChance);
            if (debug)
            {
                string ch1 = GetCharacterTypeString(character);
                string ch2 = GetCharacterTypeString(other);
                string duel = "[Can-Defend-Jump]";
                string s_characters = ch1 + " jumps to " + ch2 + " ";
                string s_result = GetResultTypeString(character.teamID == 2, result);
                string s_r_1 = (int)characterChance + SetColor(character) + "(max " + total1 + " = [" + t1 + ": " + character.characterStats.dexterity + " + DuelValue " + character.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                string s_r_2 = (int)otherChance + SetColor(other) + "(max " + total2 + " = [" + t2 + ": " + other.characterStats.dexterity + " + DuelValue " + other.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                Events.Log(s_characters + duel + s_result + " " + ch1 + " rolls " + s_r_1 + " / " + ch2 + " rolls " + s_r_2 + " " + moreData);
            }
            return result;
        }
       
        
        public bool CanInterceptPassFrom(Character other)
        {
            if (isArcade)
            {
                if (Random.Range(0, 100) < 70) return true;
                return false;
            }
            if (other.teamID == 1) return true;
            
            float total1 = character.characterStats.awareness + duelStats + GetAdditionValue(character);
            float total2 = other.characterStats.dexterity + other.duelChecker.duelStats + GetAdditionValue(other);

            total1 = SetPow(total1); total2 = SetPow(total2);

            float rand1 = Random.Range(1, total1);
            float rand2 = Random.Range(1, total2);

            string moreData = "";

            string t1 = "AWA";
            string t2 = "DEX";

            float characterChance = Random.Range(0, total1);
            float otherChance = Random.Range(0, total2);

            bool result = (characterChance > otherChance);
            if (debug)
            {
                string ch1 = GetCharacterTypeString(character);
                string ch2 = GetCharacterTypeString(other);
                string duel = "[Can-Intercept-Pass]";
                string s_characters = ch1 + " intercepts " + ch2 + " ";
                string s_result = GetResultTypeString(character.teamID == 2, result);
                string s_r_1 = (int)characterChance + SetColor(character) + "(max " + total1 + " = [" + t1 + ": " + character.characterStats.awareness + " + DuelValue " + character.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                string s_r_2 = (int)otherChance + SetColor(other) + "(max " + total2 + " = [" + t2 + ": " + other.characterStats.dexterity + " + DuelValue " + other.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                Events.Log(s_characters + duel + s_result + " " + ch1 + " rolls " + s_r_1 + " / " + ch2 + " rolls " + s_r_2 + " " + moreData);
            }
            return result;
        }



        public bool CanDash(Character other)
        {
            if (isArcade)
                return true;
            //  if (other.IsOnLujitoCooldown()) { if (debug) WinLujitoCooldown(other); return true; }
            float total1 = character.characterStats.dexterity + duelStats + GetAdditionValue(character);
            float total2 = other.characterStats.dexterity + other.duelChecker.duelStats + GetAdditionValue(other);

            total1 = SetPow(total1); total2 = SetPow(total2);

            float rand1 = Random.Range(1, total1);
            float rand2 = Random.Range(1, total2);

            string moreData = "";

            if (other.states.currentState.type == CharacterStates.types.IDLE) { rand2 /= 50f; moreData += " <color=red>(idle /= 50)</color>"; };
            if (character.states.currentState.type == CharacterStates.types.DASH) { rand1 *= 4; moreData += " <color=green>(dashing *= 4)</color>"; };

            string t1 = "DEX";
            string t2 = "DEX";


            float characterChance = Random.Range(0, total1);
            float otherChance = Random.Range(0, total2);

            bool result = (characterChance > otherChance);
            if (debug)
            {
                string ch1 = GetCharacterTypeString(character);
                string ch2 = GetCharacterTypeString(other);
                string duel = "[Can-Dash]";
                string s_characters = ch1 + " steal to " + ch2 + " ";
                string s_result = GetResultTypeString(character.teamID == 2, result);
                string s_r_1 = (int)characterChance + SetColor(character) + "(max " + total1 + " = [" + t1 + ": " + character.characterStats.dexterity + " + DuelValue " + character.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                string s_r_2 = (int)otherChance + SetColor(other) + "(max " + total2 + " = [" + t2 + ": " + other.characterStats.dexterity + " + DuelValue " + other.duelChecker.duelStats + " ↑" + powDuelStatsValue + ") " + "</color>";
                Events.Log(s_characters + duel + s_result + " " + ch1 + " rolls " + s_r_1 + " / " + ch2 + " rolls " + s_r_2 + " " + moreData);
            }
            return result;
        }
    }
}
