# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

FulboStars is a 3D football game built in **Unity 2022.3.20f1**, targeting **WebGL** as the primary platform (with Android support). The game features story mode progression through cups/levels, party mode (local/guest play), a player card collection system (Figus), and PvP rankings.

## Setup

This is a Unity project — all building, running, and testing is done through the **Unity Editor**, not the CLI. Required packages (install via Unity Package Manager):

- Input System
- Unity UI (UGUI)

## C# namespace convention

All game code lives under the `Fulbo` namespace and sub-namespaces:

- `Fulbo` — top-level data singletons (`Data`, `Events`)
- `Fulbo.Game` — in-game runtime (`GameManager`, `Character`, `Ball`, `CharactersManager`)
- `Fulbo.Game.AIs` — AI state machine
- `Fulbo.Game.CharacterStates` — character animation states
- `Fulbo.DB` — server persistence layer (`DBManager` and all `DB*` classes)
- `Fulbo.UI` — all UI components
- `Fulbo.Stadiums` — stadium data and assets
- `Fulbo.Input` — input management

## Architecture

### Singleton pattern

The game uses MonoBehaviour singletons extensively. The four you'll reference most often:

| Singleton | Scene | Purpose |
|-----------|-------|---------|
| `Data.Instance` | `0_Init` (DontDestroyOnLoad) | Runtime data hub — holds references to all loaded data objects |
| `DBManager.Instance` | `0_Init` (DontDestroyOnLoad) | Server comms, auth tokens, user persistence |
| `GameManager.Instance` | `Game` scene | In-match state machine and coordinator |
| `AudioManager.Instance` | everywhere | Sound/music playback |

### Global event bus

`Events` (static class, `Assets/Scripts/Utils/Events.cs`) is the primary decoupling mechanism. All cross-system communication goes through it. Subscribe in `Awake`/`SetOn`, always unsubscribe in `OnDestroy`/`SetOff` to avoid memory leaks:

```csharp
void Awake()   { Events.OnGoal += OnGoal; }
void OnDestroy() { Events.OnGoal -= OnGoal; }
```

Adding a new event: add a `static System.Action<...>` field initialized to `delegate {}` in `Events.cs`.

### Data flow

```
DBManager (server/PlayerPrefs)
    └── Data (DontDestroyOnLoad runtime hub)
            ├── MatchData, MyTeam, ClubsData, TextsData, ...
            ├── DataLoaderManager → loads from SERVER (WebGL/Android) or LOCAL (Standalone)
            └── GameManager → CharactersManager → Character[]
```

`Data.Instance.AllLoaded()` returns true once `DataLoaderManager` finishes. `GameManager.OnInit()` is called either by `Data.LoadReady()` (normal flow) or by the game scene itself if data was already loaded.

### Scene loading

Always use `Data.Instance.LoadLevel("SceneName")` — it plays a fade transition before loading. Direct `SceneManager.LoadScene` skips the fade and breaks the expected flow.

### Game modes

`Data.modes` enum controls two main modes:

- `STORYMODE` — authenticated user, cup progression, real server data
- `PARTYMODE` — guest/local play, simplified stats, no server writes

Guest users (`DBUserData.types.GUEST`) are automatically forced into `PARTYMODE`.

### Server environments

`DBManager.versionMode` switches between `PROD` and `DEV` API endpoints. In the Unity Editor, select the test user via `DBManager.user` (enum: `PONTURA`, `BRENDA`, `DARIO`, etc.) — this sets the email to `USERNAME@GooglePlayGames` automatically.

### Team IDs

Team 1 = the player's team (attacks left → right, goal on negative X side).  
Team 2 = the opponent (attacks right → left). This convention is used everywhere: `character.teamID`, `AI.scoreState`, ball ownership checks, limit calculations.

### AI system

Each non-player `Character` has an `AI` component that delegates to an `AIState` subclass (`currentState`). The AI updates via `AI.UpdatedByCharacter()` called from `Character.Update()`. State transitions are triggered by `Events` subscriptions inside `AI.SetOn()`.

Field players use: `AIIdle` → `AiGotoBall` / `AIPositionAttacking` / `AIPositionDefending` / `AiHasBall`  
Goalkeepers use a separate set: `AiIdleGK` → `AiPositionGK` / `AIFlyingGK` / `AiAlertGK` / `AIHasBallGK`

To add a new AI behavior: extend `AIState`, add a field to `AI`, instantiate and `Init()` it inside `AI.Init()` (in the appropriate goalkeeper/field-player branch).

### Character state machine

`CharacterStates` (on each `Character` GameObject) manages animation states via `StateCharacter` subclasses in `Assets/Scripts/States/`. States are entered by calling methods like `states.Run()`, `states.Kick()`, `states.Dash()`, etc. on `Character`. The state's `CanMove()` method gates most actions.

### Object pooling

Use `Data.Instance.pool` (`PoolObjects`) for any frequently spawned/despawned GameObjects (particles, FX, etc.) rather than `Instantiate`/`Destroy`.
