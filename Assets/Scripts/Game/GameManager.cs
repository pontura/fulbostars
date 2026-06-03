using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fulbo.Stadiums;
using Fulbo.UI;
using Fulbo.Input;
using Fulbo.Game.Powerups;
using Fulbo.DB;

namespace Fulbo.Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] Ball ball_to_instantiate;
        BallTrajectory ballTrajectory;
        public Transform shadows;
        public InputManagerGame inputManagerGame;
        static GameManager mInstance = null;
        public CharactersManager charactersManager;
        public CameraInGame cameraInGame;
        public Ball ball;
        GameState gameState;
        public StadiumsData stadiumData;
        public bool isTutorial;
        public bool isOnboardingGame;
        public PowerupsManager powerupsManager;

        public states state;
        public GameRecorder.Manager gameRecorder;

        public enum states
        {
            WAITING,
            PLAYING,
            GOAL,
            PENALTY,
            GAMEOVER,
            WAITING_FOR_TUTORIAL
        }
        public recordTypes recordType;
        public enum recordTypes
        {
            NONE,
            PLAYING,
            RECORDING
        }

        public static GameManager Instance
        {
            get
            {
                return mInstance;
            }
        }
        void Awake()
        {
            if (!mInstance)
                mInstance = this;         

            Events.OnGameStatusChanged += OnGameStatusChanged;
            Events.OnPenalty += OnPenalty;
            Events.EndGame += EndGame;
        }
        public void InitPowerups(GameObject powerupsSpawners)
        {
            if(powerupsSpawners != null && powerupsManager != null)
                powerupsManager.Init(powerupsSpawners);
        }
        private void Start()
        {
            Events.OnBackActive(Data.Instance.tournamentsData.played);
            gameRecorder = GameRecorder.Manager.Instance();
            if (gameRecorder.state == GameRecorder.Manager.states.PLAYING) recordType = recordTypes.PLAYING;
            else if (gameRecorder.state == GameRecorder.Manager.states.RECORDING) recordType = recordTypes.RECORDING;

            AudioManager.Instance.FadeVolume("music2", 0, 0.15f);
            AudioManager.Instance.PlaySpecificSound(Fulbo.Stadiums.StadiumsData.Instance.active.ambience_loop, "ambience", true);
            AudioManager.Instance.FadeVolume("ambience", 1, 0.2f);
            Data.Instance.ui.SetBackButton(false);
            if (Data.Instance.AllLoaded())
                OnInit();
        }
        public void OnInit()
        {
            UIMain.Instance.Init();
            this.ball = Instantiate(ball_to_instantiate, transform);
           // Events.AddPinball(CupsData.Instance.GetActualLevel().pinballID);
            stadiumData = StadiumsData.Instance;
           // if(Data.Instance.mode != Data.modes.PARTYMODE)
                Time.timeScale = Data.Instance.settings.GetSetting("timeScale");
            inputManagerGame = GetComponent<InputManagerGame>();
            gameState = GetComponent<GameState>();
            //if (inputManagerGame != null)
            //    inputManagerGame.Init();

            GetComponent<StadiumsManager>().Init();
            cameraInGame.Init();
            ball.Init(stadiumData.active.ball);
            ball.transform.SetParent(transform);

            ballTrajectory = GetComponent<BallTrajectory>();
            if(ballTrajectory != null) ballTrajectory.Init();

            DialoguesManager d = GetComponent<DialoguesManager>();
            if (d != null) d.Init();

            cameraInGame.LookAtBall();
            cameraInGame.Restart();

            charactersManager.Init();
            if (charactersManager is CharactersManagerTutorial)
                StartCoroutine(OnWaitToStartTutorial());
            else if (state != states.PENALTY)
                StartCoroutine(OnWaitToStart());

            Events.GameInit();
            #region ANALYTICS
            Dictionary<string, object> param = new Dictionary<string, object>();
            param["stadium"] = Data.Instance.matchData.levelData.stadium_id;
            param["level"] = Data.Instance.matchData.levelData.id;
            param["cup"] = Data.Instance.matchData.levelData.cupID;
            param["tier"] = Data.Instance.matchData.levelData.tier;
            param["teamPower"] = Data.Instance.matchData.dataOnInit.myTeamPower;

            if (DBManager.Instance.DbUserData.type == DBUserData.types.GUEST)
                param["userMail"] = "GuestMode";
            else
                param["userMail"] = DBManager.Instance.Email;

            Events.OnTrack("MatchStarted", param);

            #endregion

            if (gameRecorder.state == GameRecorder.Manager.states.PLAYING)
            {
               // inputManagerGame.ResetPlayer();
                gameRecorder.automaticPlayByRecorder.Init();
            }
            else if (gameRecorder.state == GameRecorder.Manager.states.RECORDING)
            {
                gameRecorder.InitRecording();
                gameRecorder.KeyframeRecorder.SaveSettings();
            }
        }

        private void OnDestroy()
        {
            Events.OnGameStatusChanged -= OnGameStatusChanged;
            Events.OnPenalty -= OnPenalty;
            Events.EndGame -= EndGame;
        }
        public IEnumerator OnWaitToStart()
        {
            cameraInGame.StopAllCoroutines();
            cameraInGame.Restart();
            cameraInGame.LookAtBall();
            ball.Reset();
            state = states.WAITING;
            yield return new WaitForSeconds(0.1f);
            Events.SayResults();
            yield return new WaitForSeconds(2.2f);
            state = states.PLAYING;
            Events.OnGameStatusChanged(GameManager.states.PLAYING);
            charactersManager.referi.states.Pita(0);            
            charactersManager.OnRestartGame();
            yield return new WaitForSeconds(0.35f);
            cameraInGame.Reset();
        }
        public IEnumerator OnWaitToStartTutorial()
        {
            cameraInGame.StopAllCoroutines();
            cameraInGame.LookAtBall();
            cameraInGame.Reset();
            yield return new WaitForSeconds(0.1f);
            InitTutorialDone();
        }
        void InitTutorialDone()
        {
            GetComponent<CharactersManagerTutorial>().InitTutorial(InitTutorial);
        }
        public void InitTutorial()
        {
            cameraInGame.Reset();
            state = states.PLAYING;
            Events.OnGameStatusChanged(GameManager.states.PLAYING);
        }
        public void Goal(int teamID, Character character)
        {
            state = states.GOAL;
            Events.OnGameStatusChanged(state);

            if (isTutorial)
            {
                Events.OnGoal(teamID, character);
            }
            else
            {
                AudioManager.Instance.ChangeVolume("music2", 1);
                if (teamID == 1)
                    AudioManager.Instance.PlaySound("music2", "music/music_goal3", false);
                else
                    AudioManager.Instance.PlaySound("music2", "music/music_goal1", false);
                gameState.InitGoalMoment(teamID, character);
            }
        }
        void OnGameStatusChanged(states state)
        {
            this.state = state;
        }
        void OnPenalty(Character ch)
        {
            Data.Instance.matchData.penaltyGoalKeeperTeamID = ch.teamID;
            state = states.PENALTY;
            Time.timeScale = 0;
        }
        public void BackFromGoal()
        {
            if (isTutorial)
            {
                StartCoroutine(OnWaitToStartTutorial());
                return;
            }
            StartCoroutine(OnWaitToStart());
            UIMain.Instance.OnShow();
        }
        public void GameOver()
        {
            state = states.GAMEOVER;
            Events.GameOver();
           // Events.TurnPersonalCamera(Game.GameManager.Instance.charactersManager.referi.gameObject, true);
        }
        public BallTrajectory BallTrajectory
        {
            get { return ballTrajectory; }
        }
        void EndGame()
        {
    #if UNITY_STANDALONE
             GameOver();
             return;
    #endif
            if(!Data.Instance.tournamentsData.played) return;
            GameOver();
        }
    }
}