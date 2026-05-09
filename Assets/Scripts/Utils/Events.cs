using UnityEngine;
using Fulbo.Game;
using Fulbo;
using System.Collections.Generic;
using static Fulbo.DB.DBUserData;
using Fulbo.DB;
using static Fulbo.UI.Shop.Shop;
using Fulbo.UI.Shop;
using static Fulbo.PricesData;

public static class Events
{
    public static System.Action EndGame = delegate { };
    public static System.Action<bool> OnShowDasboard = delegate { };
    public static System.Action EnergyUpdated = delegate { };
    public static System.Action OnEnergyUseUpdate = delegate { };
    public static System.Action<bool> OnPostProcessingFX = delegate { };
    public static System.Action OpenShareApp = delegate { };
    public static System.Action<bool> BuyEnergyPopup = delegate { };
    public static System.Action OpenOutOfEnergyPopup = delegate { };
    public static System.Action<string, string, System.Action<bool>, string, string> OnConfirmPanel = delegate { };
    public static System.Action<string, string, System.Action<int>, string, string, string> OnConfirmPanel3Buttons = delegate { };
    public static System.Action<string, string, System.Action<bool>> PopupText = delegate { };
    public static System.Action<Shop.sectionType> OnOpenInAppPurchases = delegate { };
    public static System.Action<bool, Character> OnPowerupActive = delegate { };
    public static System.Action<Character> OnPowerupActivated = delegate { };
    public static System.Action<bool> OnLoadingPanel = delegate { };
    public static System.Action<bool, Character> OnPowerupCharging = delegate { };
    public static System.Action ResetApp = delegate { };
    public static System.Action<System.Action, string> OnSkipOn = delegate { };
    public static System.Action OnSkipOff = delegate { };
    public static System.Action<bool> OpenSettings = delegate { };
    public static System.Action<string> OnSceneLoaded = delegate { };
    public static System.Action<bool> PlayerProgressBarSetState = delegate { };
    public static System.Action<Fulbo.Game.GameManager.states> OnGameStatusChanged = delegate { };
    public static System.Action<QtySoftHard, int, System.Action<bool>> ConfirmBuyPlayers = delegate { };
    public static System.Action<DBCharacterData, System.Action<bool>> ConfirmTierUpgrade = delegate { };

    public static System.Action<float> OnRun = delegate { };
    public static System.Action GameInit = delegate { };
    public static System.Action GameOver = delegate { };
    public static System.Action<string> GotoTo = delegate { };
    public static System.Action<string> GotoBackTo = delegate { };
    public static System.Action Back = delegate { };
   // public static System.Action<GameObject, bool> TurnPersonalCamera = delegate { };
    public static System.Action<bool, System.Action> OnFade = delegate { };
    public static System.Action<int, int> SetNewScores = delegate { };

    public static System.Action<int, int> OnButtonDown = delegate { };
    public static System.Action<int, int> OnButtonClick = delegate { };

    public static System.Action OnSkipButtonPress = delegate { };
    

    public static System.Action<int, bool> OnRight = delegate { };
    public static System.Action<int, bool> OnUp = delegate { };
    
    public static System.Action<Character> OnCatchBallWithDash = delegate { };
    public static System.Action<Character, Character> CharacterCatchBallFrom = delegate { };
    public static System.Action<Character> CharacterCatchBall = delegate { };
    public static System.Action<Character> SetCharacterNewDefender = delegate { };
    public static System.Action<Fulbo.Game.CharacterStates.kickTypes, float, Character> OnBallKicked = delegate { };
    public static System.Action<int, Character> OnGoal = delegate { };
    public static System.Action OnPenaltyGoal = delegate { };
    public static System.Action OnGoalDone = delegate { };
    public static System.Action<Character> SwapCharacter = delegate { };
    public static System.Action<int, Character> OnIntroSound = delegate { };
    public static System.Action OnOutroSound = delegate { };
    public static System.Action<Character> OnBallHitCharacter = delegate { };
    public static System.Action<string, System.Action> OnPopup = delegate { };
    public static System.Action OpenAccountSettings = delegate { };
    public static System.Action OnPopupForceSkip = delegate { };
    public static System.Action<Fulbo.Game.Tutorial.TipsManager.Types, System.Action> CheckTip = delegate { };
    public static System.Action<bool> OntutorialStepDone = delegate { };
    public static System.Action<int, string, System.Action, bool, bool> OnTutorialPopup = delegate { };
    public static System.Action<int, System.Action> OnTutorialProgress = delegate { };
    public static System.Action<int> OnTutorialProgressMenu = delegate { };
    public static System.Action<LevelData, System.Action<LevelData>> OpenUnlockLevelPopup = delegate { };
    public static System.Action<List<Fulbo.UI.ProgressMenu.ItemData>, bool> ShowTutorialProgressMenu = delegate { };
    public static System.Action<int> OnTutorialProgressAdd = delegate { };
    public static System.Action<bool> OnSkipTutorialShow = delegate { };
    public static System.Action OnSkipTutorial = delegate { };
    public static System.Action OnSkipAllTutorialSteps = delegate { };
    public static System.Action<Character, System.Action> OnGameOverVoiceHappy = delegate { };
    public static System.Action<string, System.Action> OnVoiceSay = delegate { };
    public static System.Action<bool> ToggleStats = delegate { };

    public static System.Action<Character, string> SetDialogue = delegate { };

    public static System.Action KickToGoal = delegate { };


    public static System.Action SayResults = delegate { };
    public static System.Action<int, bool> SayCharacterName = delegate { };
    public static System.Action<int> SayPalo = delegate { };
    public static System.Action<AudioClip[]> PlaySpecificSoundInArray = delegate { };
    public static System.Action<AudioClip> PlaySpecificSound = delegate { };
    public static System.Action<AudioClip, Vector3> PlayCharacterSFX = delegate { };
    public static System.Action<string, string, bool> PlaySound = delegate { };
    public static System.Action Lujito = delegate { };
    public static System.Action SetArcadeVolUp = delegate { };
    public static System.Action SetArcadeVolDown = delegate { };
    public static System.Action<string, float> ChangeVolume = delegate { };
    public static System.Action<Character> OnPowerupIncrease = delegate { };
    public static System.Action<string> OnLoading = delegate { };
    public static System.Action<float> OnLoadingProgress = delegate { };
    public static System.Action<bool> OnInitTimeout = delegate { };
    public static System.Action<System.Action<string>> LoadAssetBundles = delegate { };
    public static System.Action OnRestartGame = delegate { };
    public static System.Action<int, Vector3> OnGoldScoreWin = delegate { };
    public static System.Action<Character> OnPenalty = delegate { };
    public static System.Action<Character, System.Action> OnPenaltyWaitingToKick = delegate { };
    public static System.Action<AudioClip> OnRelatorSay = delegate { };
    public static System.Action<string> OnRelatorSayRecorded = delegate { };
    public static System.Action<int> AddPinball = delegate { };

    public static System.Action OnTeamPowerRefresh = delegate { };
    public static System.Action<Fulbo.UI.ButtonCustom.types> OnButtonPressed = delegate { };

    public static System.Action<Character> OnWeapon = delegate { };

    public static System.Action<Character> BallPassedTo = delegate { };
    public static System.Action<Character> CatchBall = delegate { };
    public static System.Action<Character> LoseBall = delegate { };
    public static System.Action<Character> OnHitReferi = delegate { };
    public static System.Action<Grabbable> OnGrab = delegate { };
    public static System.Action OpenMainMenu = delegate { };
    //  public static System.Action<CharactersData.CharacterData, System.Action> OpenCharacterFullCard = delegate { };

    public static System.Action<GameObject> OnCharacterHitTrigger = delegate { };
    public static System.Action<GameObject> OnBallHitObstacle = delegate { };
    public static System.Action<Fulbo.FX.FXManager.types, Vector3> OnFX = delegate { };

    //character, position
    public static System.Action<int, int, float, System.Action<int>> InitBuyPosition = delegate { };
    public static System.Action<float, System.Action<bool>, string> ConfirmBuy = delegate { };
    public static System.Action OnBuyReady = delegate { };
    public static System.Action<DBUserData.DBCharacterData> CharacterUpdatedData = delegate { };
    public static System.Action<DBUserData.DBCharacterData> RefreshData = delegate { };

    public static System.Action<int> ShowNewCoinsPopup = delegate { };
    public static System.Action<bool> ScoreFreezed = delegate { };
    public static System.Action<int> AddScore = delegate { };
    public static System.Action<int> AddHardScore = delegate { };
    public static System.Action<int> RefreshScore = delegate { };
    public static System.Action<int> RefreshHardCurrency = delegate { };
    public static System.Action<Fulbo.Onboarding.OnboardingPanel.panels, int, System.Action<bool>> OnboardingCheckStep = delegate { };
    public static System.Action<string, int> OnBoardingPanelAction = delegate { }; // Shortcut for actions of the tutorial onboarding:

    //Tracking:
    public static System.Action<string, Dictionary<string, object>> OnTrack = delegate { };

    //cheats
    public static System.Action CheatThrowPowerups = delegate { };


    //admob
    public static System.Action<System.Action<bool>> AdsWatchVideo = delegate { };
    public static System.Action<System.Action<bool>> AdsWatchInterstitial = delegate { };
    public static System.Action<System.Action<bool>> AdsCheckForExtraTime = delegate { };
    public static System.Action<System.Action<bool>> AdsCheckForExtraLife = delegate { };
    public static System.Action<bool, System.Action> CheckForDailyRewards = delegate { };
    public static System.Action OnNoMoreAdsForToday = delegate { };
    public static System.Action<sectionType> OpenShop = delegate { };
    //score to:
    public static System.Action<int, Fulbo.UI.FlyingParticlesUI.types, Vector2, float, float> OnFlyingParticles = delegate { };
    public static System.Action<bool> ShowParticlesDark = delegate { };
    //type, percent, from score, to score
    public static System.Action<Fulbo.UI.FlyingParticlesUI.types, float, float, float> OnFlyingPArrives = delegate { };

    //cups
    public static System.Action<int, int> InitCup = delegate { };
    public static System.Action EndCup = delegate { };
    public static System.Action<float> ResetLifesTo = delegate { };
    public static System.Action<bool, System.Action> ShowCupWinSignal = delegate { };

    //chests
    public static System.Action<int, System.Action, MatchData.ResponseFromServer.ChestDataFromDB> OpenChest = delegate { };

    public static System.Action<string> Log = delegate { };
    public static System.Action OpenNotifications = delegate { };
    public static System.Action<string> CheckForImportantNotifications = delegate { };

    public static System.Action<Fulbo.UI.Shop.Shop.sectionType, bool> OnFreeStaffUpdate = delegate { };
}