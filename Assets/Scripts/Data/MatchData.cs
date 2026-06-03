using Fulbo.DB;
using Fulbo.Game;
using Fulbo.Game.Powerups;
using Fulbo.Stadiums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Fulbo.DB.DBUserData;
using static Fulbo.Game.CharactersPositions;
using Fulbo.Onboarding;
using System;

namespace Fulbo
{
    public class MatchData : MonoBehaviour
    {
        public LevelData levelData;
        MatchRewards matchRewards;

        public int secs;
        public float time;

        public int[] players;

        public Vector2 score;
        public int penaltyGoalKeeperTeamID;
        public int totalPlayers;
        public int lastGoalBy;
        public int totalCharacters_team1;
        public int totalCharacters_team2;
        public int[] charactersPositions;

        public List<int> team1;
        public List<int> team2;

        public List<int> team1_goals;
        public List<int> team2_goals;

        public bool team1Controlled;
        public bool team2Controlled;

        bool timeoutStarted;
        public bool matchPlayed;

        public int totalFigusWon;

        public int totalTime = 120;

        int maxExtraTimeCount = 2;

        public bool cameFromMatch = false;

        List<int> actualCharacterLevels;    // only to compare with new levels:
        List<int> actualCharacterLevels_xp; // only to compare with new xp:

        public Powerup.types team2_powerup;

        public MatchRewards Rewards() { return matchRewards; }

        public DataOnInit dataOnInit;
        [Serializable]
        public class DataOnInit
        {
            public int hard_on_init_match;
            public int soft_on_init_match;
            public int shards_on_init_match;
            public int myTeamPower;

            public bool hasWatchVideoForLife;

            public void Reset()
            {
                hasWatchVideoForLife = false;
            }
        }
        public void ShufflePlayersInTeam(int teamID)
        {
            List<int> t = GetTeam(teamID);
            int goalkeeper = t[0];
            t.RemoveAt(0);
            Utils.Shuffle(t);
            t.Insert(0, goalkeeper);
            if(teamID == 1)
                team1 = t;
            else
                team2 = t;
        }
        public void SetPowerup(int teamID)
        {
            int rand = UnityEngine.Random.Range(0, 100);
            if (rand < 33)
                team2_powerup = Powerup.types.SUPERKICK;
            else if (rand < 66)
                team2_powerup = Powerup.types.SPEED;
            else
                team2_powerup = Powerup.types.BOMB;
        }
        public int GetTotalPlayersInMatch(int teamID)
        {
            if (teamID == 1)
                return team1.Count;
            return team2.Count;
        }
        public List<int> GetTeam(int teamID)
        {
            if (teamID == 1)
                return team1;
            return team2;
        }
        void Start()
        {
            dataOnInit = new DataOnInit();
            matchRewards = new MatchRewards();
            secs = totalTime;
            time = 0;
            Events.GameInit += GameInit;
            Events.OnGoal += OnGoal;
            Events.TotalJoysticks += TotalJoysticks;
        }
        private void OnDestroy()
        {
            Events.GameInit -= GameInit;
            Events.OnGoal -= OnGoal;
            Events.TotalJoysticks -= TotalJoysticks;
        }
        void TotalJoysticks(int qty)
        {
            print("TotalJoysticks: " + qty);
            if(Data.Instance.tournamentsData.IsTournament())
            {
                for(int a = 0; a<qty; a++)
                {
                    if(team1Controlled)
                        players[a] = 1;
                    else  
                        players[a] = 2;
                }
            }
        }
        void SetJoysticksToLeftTeam()
        {
            print("SetJoysticksToLeftTeam");
            for (int a = 0; a < players.Length; a++)
            {
                if(players[a] > 0)
                {
                    if(team1Controlled)
                        players[a] = 1;
                    else  
                        players[a] = 2;
                }
            }
        }
        public CharactersPositions.PositionsData GetPositionsForTeam(int teamID)
        {
            CharactersPositions.PositionsData positionsData;
            int charactersPositions_id;
            if (Data.Instance.mode == Data.modes.PARTYMODE)
            {
                charactersPositions_id = charactersPositions[teamID-1];
                positionsData = Data.Instance.charactersPositions.GetPositionsData(teamID, charactersPositions_id);
            }else
            if (teamID == 1 && Data.Instance.mode == Data.modes.PVP)
            {
                List<DBUserData.DBCharacterData> charactersData = Data.Instance.pvpData.GetCharactersData();
                positionsData = Data.Instance.charactersPositions.GetPvpOpponentPositionData().ToPositions();
            }
            else
            {
                positionsData = Data.Instance.charactersPositions.GetPlayerPositionData().ToPositions();
            }

            return positionsData;
        }
        public void SetActualStadium(int stadium_id)
        {
         //   Debug.Log("SetActualStadium " + stadium_id);
            this.levelData.stadium_id = stadium_id;
        }
        public void InitLevel(LevelData levelData)
        {
            matchRewards.Reset();
            Reset();
            this.levelData = levelData;

         //   print("Init Level " + levelData.cupID + " levelID:" + levelData.id);

            StadiumsData.Instance.SetActiveStadium(levelData.stadium_id, levelData.size);

            Data.Instance.matchData.totalCharacters_team2 = levelData.myTeamQty;
            Data.Instance.matchData.totalCharacters_team1 = levelData.oponents.Count;
            secs = levelData.duration;
            totalTime = levelData.duration;
            time = 0;
            Data.Instance.matchData.charactersPositions[1] = levelData.charactersPositions;
            Data.Instance.charactersPositions.LoadPositions();
            CharactersData.Instance.SetReferi(levelData.referiID);

            Data.Instance.matchData.team1.Clear();
            foreach (int a in levelData.oponents)
                Data.Instance.matchData.team1.Add(a);

            Data.Instance.matchData.AddRealTeam(levelData.oponents.Count);
            CheckToSwapColors();

            if(Data.Instance.settings.SettingExist("maxExtraTimeCount"))
                maxExtraTimeCount = (int)Data.Instance.settings.GetSetting("maxExtraTimeCount");
        }
        public void SetTotalPlayers(int myTeamQty, int oponentsQty)
        {
            if(Data.Instance.tournamentsData.IsTournament())
            {
                SetJoysticksToLeftTeam();
            }
            totalPlayers = myTeamQty; // for guestMode only:
            if (myTeamQty > team2.Count && Data.Instance.mode != Data.modes.PARTYMODE) myTeamQty = team2.Count;

            totalCharacters_team2 = myTeamQty;
            Data.Instance.matchData.totalCharacters_team1 = oponentsQty;
        }
        public void AddRealTeam(int maxPlayers)
        {
            team2.Clear();
            foreach (int id in Data.Instance.myTeam.GetBestTeamPlayersID(maxPlayers))
                team2.Add(id);
            //ajusta los totales al total de characters posibles
            if (totalCharacters_team2 > team2.Count) totalCharacters_team2 = team2.Count;
        }
        public void AddOponentTutorialTeam()
        {
            totalCharacters_team1 = 4;
            totalCharacters_team2 = 6;
            team1.Clear();
            team1.Add(1);
            for (int a = 0; a < 4; a++)
            {
                if(a%2==0)
                    team1.Add(117);
                else
                    team1.Add(118);
            }
        }
        public void AddOponentTeam(List<int> oponents)
        {
            team1.Clear();
            foreach (int id in oponents)
                team1.Add(id);
        }
        public void AddOponentTeam(List<DBCharacterData> oponents)
        {
            team1.Clear();
            foreach (DBCharacterData d in oponents)
                team1.Add(d.player_id);
        }
        public void AddCharacterToTeam(int teamID, int id)
        {
            if (teamID == 1)
            {
                team1.Add(id);
                CharactersData.Instance.availablesTeam1.Remove(id);
            }
            else
            {
                team2.Add(id);
                CharactersData.Instance.availablesTeam2.Remove(id);
            }
        }
        private void Awake()
        {
            AddPlayer(1, 2);
        }
        public void AddPlayer(int id, int teamID)
        {
            players[id - 1] = teamID;
            totalPlayers++;
            if (teamID == 1) team1Controlled = true;
            if (teamID == 2) team2Controlled = true;
        }
        public void RemoveTeam(int id, int teamID)
        {
            players[id - 1] = 0;
            if (teamID == 1) team1Controlled = false;
            if (teamID == 2) team2Controlled = false;
        }
        public bool IsTeamBeingControlled(int teamID)
        {
            if (teamID == 1 && team1Controlled) return true;
            if (teamID == 2 && team2Controlled) return true;
            else return false;
        }
        public bool IsTutorial()
        {
            if (SceneManager.GetActiveScene().name == "Tutorial") return true;
            if (Data.Instance.matchData.levelData.stadium_id == -1) return true;
            return false;
        }
        void GameInit()
        {
            // agrega los levels de los jugadores activos:
            actualCharacterLevels = new List<int>();
            actualCharacterLevels_xp = new List<int>();
            print("GameInit istutorial: " + Fulbo.Game.GameManager.Instance.isTutorial);

            if (Fulbo.Game.GameManager.Instance.isTutorial)
                return;

            dataOnInit.Reset();
            dataOnInit.hard_on_init_match = DBManager.Instance.DbUserData.data.hard_currency;
            dataOnInit.soft_on_init_match = DBManager.Instance.DbUserData.data.score;
            dataOnInit.shards_on_init_match = DBManager.Instance.DbUserData.data.shards;

            timeoutStarted = false;
            CancelInvoke();
            time = 0;

            if (Data.Instance.mode == Data.modes.PARTYMODE)
            {
                secs = 120;
                totalTime = 120;
            }
            
            if (Data.Instance.mode == Data.modes.STORYMODE || Data.Instance.mode == Data.modes.PVP)
                SetCharactersLevelData();
        }
        void SetCharactersLevelData()
        {
            PlayerPositionData savedFormation = DB.DBManager.Instance.DbGameData.GetFormation(team2.Count);
            foreach (CharacterPositionData pData in savedFormation.posData)
            {
                int playerID = pData.uniqueId;
                DB.DBUserData.DBCharacterData cData = DB.DBManager.Instance.DbUserData.data.GetPlayerByID(playerID);
                actualCharacterLevels.Add(cData.level);
                actualCharacterLevels_xp.Add(cData.current_level_xp);
            }
        }
        // compara los levels de los jugadores activos:
        public bool CheckForUpgradedLevel(int arrID, int newLevel)
        {
            if (Data.Instance.mode == Data.modes.PARTYMODE) return false;
            if (actualCharacterLevels == null) return false;
            if (actualCharacterLevels.Count>=arrID)
            {
                int lastLevel = actualCharacterLevels[arrID];
                return lastLevel < newLevel;
            }
            return false;
        }
        // compara los levels de los jugadores activos:
        public int GetLastXP(int arrID)
        {
            if (Data.Instance.mode == Data.modes.PARTYMODE) return 0;
            if (actualCharacterLevels_xp.Count >= arrID)
                return  actualCharacterLevels_xp[arrID];
            return 0;
        }
        public void ResetAll()
        {
            Reset();
            team1.Clear();
            team2.Clear();
            Data.Instance.GetComponent<MatchStats>().Reset();
            charactersPositions[0] = 0;
            charactersPositions[1] = 0;
        }
        public void Reset()
        {
            matchPlayed = false;
            team1_goals.Clear();
            team2_goals.Clear();
            score = Vector2.zero;
            lastGoalBy = 0;
            penaltyGoalKeeperTeamID = 0;
            totalFigusWon = 0;
            if (Data.Instance.mode == Data.modes.PARTYMODE)
            {
                secs = 120;
                totalTime = 120;
            }
        }
        void OnGoal(int _teamID, Character character)
        {
            // totalFigusWon++;
            if (_teamID == 1)
            {
                score.x++;
                team1_goals.Add(character.data.id);
            }
            else
            {
                score.y++;
                team2_goals.Add(character.data.id);
            }
            Events.SetNewScores((int)score.x, (int)score.y);
            lastGoalBy = _teamID;

            //if (GameManager.Instance.isTutorialMatch())
            //    GameOver();
        }
        private void Update()
        {
            if (GameManager.Instance == null) return;
            if(!GameManager.Instance.isTutorial && GameManager.Instance.state == GameManager.states.PLAYING && secs > 0)
            {
                time += Time.deltaTime;
                if (totalTime - time < secs)
                    Loop();
            }
        }
        void Loop()
        {
            if (Fulbo.Game.GameManager.Instance != null && Fulbo.Game.GameManager.Instance.state == Fulbo.Game.GameManager.states.PLAYING)
                secs--;
            if (secs < 10 && !timeoutStarted)
            {
                timeoutStarted = true;
                Events.OnInitTimeout(true);
            }           
            if (secs <= 0)
            {
               if(ShowTimeExtraLife())
                    TimeOutExtraLife();
                else if (score.x < score.y || // you win
                    watchVideoWasDisplayed >= maxExtraTimeCount ||
                    Data.Instance.mode != Data.modes.STORYMODE)
                    GameOver();
                else
                    TimeOut();
            }
        }
        bool watchVideoExtraLife;
        void TimeOutExtraLife()
        {
            print("TimeOutExtraLife");
            Time.timeScale = 0;
            watchVideoExtraLife = true;
            Events.AdsCheckForExtraLife(AcceptExtraLife);
        }
        void AcceptExtraLife(bool ok)
        {
            Time.timeScale = 1;
            print("AcceptExtraLife " + ok);
            if (ok)
            {
                dataOnInit.hasWatchVideoForLife = true;
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["adType"] = "EXTRA_LIFE";
                Events.OnTrack("AdsReward", param);

                Events.ResetLifesTo(0);
                Vector2 centerOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
                float from = 0;
                Events.OnFlyingParticles(1, UI.FlyingParticlesUI.types.LIFE, centerOfScreen, from, 1);
                DBManager.Instance.DbUserData.data.gameData.cups.AddNewLives(1);
            }
            GameOver();
        }
        bool ShowTimeExtraLife()
        {
            bool isPlayingLastLife = DBManager.Instance.DbUserData.data.gameData.cups.IsPlayingLastLife();
            print("ShowTimeExtraLife isPlayingLastLife: " + isPlayingLastLife + " watchVideoWasDisplayed:" + watchVideoWasDisplayed + "  watchVideoExtraLife:" + watchVideoExtraLife);
            if (isPlayingLastLife  && score.x >= score.y && watchVideoWasDisplayed>=maxExtraTimeCount && !watchVideoExtraLife)
                return true;
            return false;
        }
        int watchVideoWasDisplayed;
        public void TimeOut()
        {
            Time.timeScale = 0;            
            watchVideoWasDisplayed++;
            Events.AdsCheckForExtraTime(AcceptExtraTime);
        }
        void AcceptExtraTime(bool ok)
        {
            if (ok) {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["adType"] = "EXTEND_MATCH";
                Events.OnTrack("AdsReward", param);
                int addTime = 30;
                secs += addTime;
                time -= addTime;
                timeoutStarted = false;
                Events.OnInitTimeout(false);
                Time.timeScale = 0;
                StartCoroutine(TimeScaleResetGradualy(0.075f, 1f));
            } else if (ShowTimeExtraLife()) {
                Time.timeScale = 0;
                TimeOutExtraLife();
            } else {
                Time.timeScale = 1;
                GameOver();
            }
        }

        IEnumerator TimeScaleResetGradualy(float initial, float target) { // Va aumentando exponencialmente el timescale hasta llegar a 1
            yield return new WaitForSecondsRealtime(0.1f);
            Time.timeScale = Time.timeScale == 0 ? initial : Time.timeScale;
            Time.timeScale *= 1f + Time.timeScale;
            if (Time.timeScale < target)
                StartCoroutine(TimeScaleResetGradualy(initial, target));
            else
                Time.timeScale = target;
        }

        public void GameOver()
        {
           
            watchVideoWasDisplayed = 0;
            matchPlayed = true;

            Game.GameManager.Instance.GameOver();
            if (Data.Instance.mode != Data.modes.PARTYMODE)
            {
                Data.Instance.onBoardingManager.CheckOnboardingGamesDone();
                Rewards().Calculate();
            }            
        }
        void OnResultSaved(bool ok)
        {
            print("OnResultSaved " + ok);
        }
        public void SetMyPosition(int myPos)
        {
            charactersPositions[0] = myPos;
        }
        public void SetCharactersPositions(int team1, int team2)
        {
            charactersPositions[0] = team1;
            charactersPositions[1] = team2;
        }
        public int GetDiffGoles()
        {
            return (int)Mathf.Abs(score.x - score.y);
        }
        public CharactersData.CharacterData GetWinnerCharacter()
        {
            if (team1.Count < 1) return null;
            int characterID;
            if (score.x > score.y)
                characterID = team1[UnityEngine.Random.Range(1, team1.Count - 1)];
            else
                characterID = team2[UnityEngine.Random.Range(1, team2.Count - 1)];
            return CharactersData.Instance.GetCharacterData(characterID, false);
        }
        public int GetWinner()
        {
            if(Data.Instance.matchData.score.x == Data.Instance.matchData.score.y)
                return 0;
             if(Data.Instance.matchData.score.x<Data.Instance.matchData.score.y)
                return 2;
            else 
                return 1;
        }
        public void AddMonitoToMyTeam(int characterID)
        {
            team2.Add(characterID);
        }
        //public void SetTeams(List<int> new_team1, List<int> new_team2)
        //{
        //    team1.Clear();
        //    team2.Clear();
        //    foreach (int id in new_team1)
        //        team1.Add(id);
        //    foreach (int id in new_team2)
        //        team2.Add(id);

        //    totalCharacters_team1 = team1.Count;
        //    totalCharacters_team2 = team2.Count;
        //}
        public void SetTeam(int teamID, List<int> newTeam)
        {
            if (teamID == 1)
            {
                team1.Clear();
                foreach (int id in newTeam)
                    team1.Add(id);
            }
            else
            {
                team2.Clear();
                foreach (int id in newTeam)
                    team2.Add(id);
            }
            totalCharacters_team1 = team1.Count;
            totalCharacters_team2 = team2.Count;
        }
        public bool ShowQuickIngameTutorial()
        {
            if (Data.Instance.onBoardingManager.IsBoardingStep(OnBoardingManager.BoardingStepStates.GOT_FIRST_PLAYER_CARDS))
                return true;
            return false;
        }
        void CheckToSwapColors()
        {
            levelData.clubData.clubColor1 = Data.Instance.settings.GetSecondaryColorIfAreSimilar(Data.Instance.myTeam.clubData.clubColor1, levelData.clubData.clubColor1);
        }
        // cancel match:
        public void SaveCupDataOnGameClosed(System.Action<bool, string> OnSaved)
        {
            MatchData matchData = Data.Instance.matchData;
            matchData.score.y = 0;
            matchData.score.x = 1;
            SaveCupDataOnGameOver(OnSaved);
        }
        public void SaveCupDataOnGameOver(System.Action<bool, string> OnSaved)
        {
            if (Data.Instance.mode == Data.modes.PARTYMODE)
            {
                OnSaved(true, "");
                return;
            }
            //CUPS SAVE RESULTS:
            int myscore = (int)score.y;
            int opponentScore = (int)score.x;
            DB.DBManager.Instance.DbUserData.data.gameData.cups.SaveResults(opponentScore, myscore);
            DB.DBManager.Instance.DbGameData.Put(OnSaved);
        }

        [Serializable] public  class ResponseFromServer
        {
            public string message;
            public bool referralBonus;
            public bool cupWon;
            public bool chestWon;
            public int shardsWon;
            public ScoreDataFromDB scoreData;
            public XPDataFromDB xpData;
            public ChestDataFromDB chestData;

            [Serializable]
            public class ScoreDataFromDB
            {
                public int total;
                public int _base;
                public int bonus;
                public int multiplier;
                public int bonusTotal;
            }
            [Serializable]
            public class XPDataFromDB
            {
                public int total;
                public int _base;
                public int bonus;
                public int multiplier;
                public int bonusTotal;
            }
            [Serializable]
            public class ChestDataFromDB
            {
                public int hard;
                public int energy;
                public int shard;

                public int hard_from;
                public int energy_from;
                public int shard_from;
            }
        }
        public ResponseFromServer response;


        public void SetResponse(SimpleJSON.JSONNode jsonNode)
        {
            response = new ResponseFromServer();
            Debug.Log("TRACK Response: " + jsonNode);

            response.message = jsonNode["message"];
            response.referralBonus = jsonNode["referralBonus"];
            response.cupWon = jsonNode["cupWon"];

            response.shardsWon = jsonNode["shardsWon"];
            response.chestWon = jsonNode["chestWon"];

            response.scoreData = new ResponseFromServer.ScoreDataFromDB();
            response.scoreData.total = jsonNode["score"]["total"];
            response.scoreData._base = jsonNode["score"]["base"];
            response.scoreData.bonus = jsonNode["score"]["bonus"];
            response.scoreData.multiplier = jsonNode["score"]["multiplier"];
            response.scoreData.bonusTotal = jsonNode["score"]["bonusTotal"];

            response.xpData = new ResponseFromServer.XPDataFromDB();
            response.scoreData.total = jsonNode["xp"]["total"];
            response.xpData._base = jsonNode["xp"]["base"];
            response.xpData.bonus = jsonNode["xp"]["bonus"];
            response.xpData.multiplier = jsonNode["xp"]["multiplier"];
            response.xpData.bonusTotal = jsonNode["xp"]["bonusTotal"];

            response.chestData = new ResponseFromServer.ChestDataFromDB();
            response.chestData.hard = jsonNode["chestWon"]["hard"];
            response.chestData.energy = jsonNode["chestWon"]["energy"];
            response.chestData.shard = jsonNode["chestWon"]["shard"];
        }
        public int GetTotalUsedPlayersInTeam(int teamID)
        {
            int total = 0;
            for(int a = 0; a<4; a++)
            {
                if(players[a] == teamID)
                {
                    if(teamID == 1 && players[a] == 1)
                    {
                        total++;
                    }
                    else if(teamID == 2 && players[a] == 2)
                    {
                        total++;
                    }
                }
            }
            return total;
        }
    }    
}