using Fulbo.DB;
using Fulbo.Game.AIs;
using Fulbo.Game.Powerups;
using Fulbo.Stadiums;
using Fulbo.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Fulbo.Game
{
    public class CharactersManager : MonoBehaviour
    {

        public int totalUsedPlayers_team1;
        public int totalUsedPlayers_team2;

        public List<int> team1_playingCharacters;
        public List<int> team2_playingCharacters;

        [HideInInspector] public int teamID_1;
        [HideInInspector] public int teamID_2;

        public float rotationOffset = 10;
        public Referi referi;
        Vector2 limits;
        public Ball ball;

        public GameObject containerTeam1, containerTeam2;
        public List<Character> team1;
        public List<Character> team2;

        public List<Character> playingCharacters;

        int team_1_id = 0;
        int team_2_id = 0;
        public CharactersSignals signals;
        GamePlay gameplaySettings;
        List<Character> charactersForTheStart;
        MatchData matchData;
        GameManager game;
        public CharactersStrategy charactersStrategy;
        public CharacterViewZoneController characterViewZoneController;
        CharactersAiManager charactersAiManager;
        float loopTime;
        PowerupsManager powerupsManager;
        bool singlePlayer; // one player only in team 2.

        private void Awake()
        {
            charactersStrategy = new CharactersStrategy();
            charactersAiManager = new CharactersAiManager();
        }
        private void Start()
        {
            totalUsedPlayers_team1 = Data.Instance.matchData.GetTotalUsedPlayersInTeam(1);
            totalUsedPlayers_team2 = Data.Instance.matchData.GetTotalUsedPlayersInTeam(2);  

            if(Data.Instance.mode == Data.modes.STORYMODE || Data.Instance.mode == Data.modes.PVP)
                singlePlayer = true;
            powerupsManager = GameManager.Instance.powerupsManager;
            matchData = Data.Instance.matchData;
            game = Fulbo.Game.GameManager.Instance;

            Events.KickToGoal += KickToGoal;
           
        }
        private void OnDestroy()
        {
            Events.KickToGoal -= KickToGoal;
        }
       
        public virtual void Init(bool isBillboard = false)
        {
            loopTime = Data.Instance.settings.GetSetting("timeToSwapCharactersAutomatically");
            CharactersConstructor cc = GetComponent<CharactersConstructor>();
            if (cc != null) cc.AddCharacters();

            gameplaySettings = Data.Instance.settings.gameplay;

            ball = game.ball;
            charactersStrategy.Init(this);
            charactersAiManager.Init(this);
            Loop();

            //AddCharacter(2);
            referi.InitReferi(this, CharactersData.Instance.GetReferi().asset);
            limits = new Vector2(StadiumsData.Instance.active.GetAssetBySelectedSize().size_x / 2, StadiumsData.Instance.active.GetAssetBySelectedSize().size_y / 2);
            limits.x -= 1;

            if (Data.Instance.mode == Data.modes.PVP)
            {
                SetCharactersToTeam(1, true, isBillboard);
                SetCharactersToTeam(2, true, isBillboard);
            }
            else
            {
                SetCharactersToTeam(1, false, isBillboard); // opponents
                if (Data.Instance.mode == Data.modes.PARTYMODE)
                    SetCharactersToTeam(2, false, isBillboard); // myteam not in DB:
                else
                    SetCharactersToTeam(2, true, isBillboard); // myteam in DB:
            }


            if (isBillboard) return;

            for (int a = 0; a < matchData.players.Length; a++)
            {
                int teamID = matchData.players[a];
                if (teamID != 0) AddCharacter(a + 1, teamID);
            }
            ResetAll();

#if UNITY_EDITOR
            if (Data.Instance.settings.mainSettings.turn_on_only_other_goalkeeper)
            {
                foreach (Character ch in team1)
                    if (ch.type != Character.types.GOALKEEPER)
                        ch.gameObject.SetActive(false);
            }
            else if (Data.Instance.settings.mainSettings.turn_off_team2)
                containerTeam1.SetActive(false);
#endif

            foreach (Character ch in team1)
                ch.SetOponent();
            foreach (Character ch in team2)
                ch.SetOponent();
        }
        void Loop()
        {
            Invoke("Loop", loopTime);
            if (GameManager.Instance.state == Fulbo.Game.GameManager.states.PLAYING)
                charactersAiManager.Loop();
        }
        public void SetCharactersToTeam(int teamID, bool userDBTeam, bool isBillboard = false, bool isGameOver = false)
        {
           // print("SetCharacters teamID: " + teamID + "     userDBTeam: " + userDBTeam + "   is billboard:"+  isBillboard + "  reverse: " + reverseTeams);
            List <Character> arr;
            GameObject container;
            if (teamID == 1) {
                arr = team1;
                container = containerTeam1;
            } else {
                arr = team2;
                container = containerTeam2;
            } 
            if (isGameOver)  container = containerTeam1;

            int id = 0;
            GameObject asset = null;
            CharactersData.CharacterData characterData;
            List<DBUserData.DBCharacterData> allPlayersData = new List<DBUserData.DBCharacterData>();

            if (userDBTeam)
            {
                int totalPlayers = container.GetComponentsInChildren<Character>().Length;
                if(teamID == 2)
                    allPlayersData = Data.Instance.myTeam.GetSavedTeamFor(totalPlayers);
                else
                    allPlayersData = Data.Instance.pvpData.GetCharactersData();
            }
            foreach (Character character in container.GetComponentsInChildren<Character>())
            {
                arr.Add(character);
                characterData = CharactersData.Instance.GetCharacterDataByTeam(teamID, id, character.type == Character.types.GOALKEEPER);
                if (!isBillboard) asset = characterData.asset;
                character.SetOrder(id);
                if (userDBTeam)
                {
                    DB.DBUserData.DBCharacterData dbData = allPlayersData[id];
                    character.Init(teamID, characterData.id, this, asset, dbData);
                }
                else
                {
                    character.Init(teamID, characterData.id, this, asset);
                }
                id++;
            }
        }
        public virtual Character GetSacaCharacter(List<Character> team, int who) // delantero o segundo delantero
        {
            List<Character> arr =  team.OrderBy(x => Vector3.Distance(x.transform.position, Vector3.zero)).ToList();
            return arr[who-1];
        }
        public void RemovePlayer()
        {
            foreach (Character ch in team1)
            {
                if (ch.isBeingControlled)
                    ch.SetControlled(false);
            }
            Data.Instance.matchData.RemoveTeam(1, 1);
            Data.Instance.matchData.RemoveTeam(1, 2);
            signals.Remove();
        }
        public virtual void ResetAll()
        {
            referi.OnRestartGame();

            ResetTeam(team1);
            ResetTeam(team2);

            if (Fulbo.Game.GameManager.Instance != null)
            {
                if (matchData.lastGoalBy < 2) Saca(team2); else Saca(team1);
            }
            if (!GameManager.Instance.isTutorial)
                charactersForTheStart[0].characterColliders.SetCollidersOff(1000);
        }
        void ResetTeam(List<Character> team)
        {
            foreach (Character c in team)
            {
                c.ai.ResetPosition();
                c.states.LookAtBall();
                c.states.Reset();
            }
        }
        public virtual void Saca(List<Character> team)
        {
            charactersForTheStart = new List<Character>();
            charactersForTheStart.Add(GetSacaCharacter(team, 1));
            charactersForTheStart.Add(GetSacaCharacter(team, 2));
            if (charactersForTheStart[0] != null)
            {
                AIs.AI ai = charactersForTheStart[0].ai;
                if (ai != null)
                    ai.SetInitialCharacterPosition();

                charactersForTheStart[0].states.LookAtBall();
            }
            if(team[0].teamID == 1)
                RepositionateOtherTeam(team2);
            else
                RepositionateOtherTeam(team1);
        }
        void RepositionateOtherTeam(List<Character> otherTeam)
        {
            float max = 6f;
            int teamID = otherTeam[0].teamID;
            foreach (Character ch in otherTeam)
            {
                if (ch.type != Character.types.GOALKEEPER)
                {
                    ch.states.LookAtBall();
                    float distanceToCenter = Vector3.Distance(ch.transform.position, Vector3.zero);
                    if (distanceToCenter < max)
                    {
                        Vector3 pos = ch.transform.position;
                        if (teamID == 2) pos.x -= max / 1.5f; else pos.x += max / 2;
                        pos.z *= 1.2f;
                        ch.transform.position = pos;
                    }
                }
            }
        }
        public void OnRestartGame()
        {
            SwapIfNeededAtStart(charactersForTheStart[1].teamID);
            charactersForTheStart[0].states.Kick(CharacterStates.kickTypes.SOFT);
            charactersForTheStart[1].characterColliders.Reset();
            ball.PaseTo(charactersForTheStart[1]);
            charactersForTheStart.Clear();

            if (!GameManager.Instance.isTutorial)
                Invoke("ResetAllPlayersOnKickOff", 0.25f);
        }
        void ResetAllPlayersOnKickOff()
        {
            if (GameManager.Instance.state == GameManager.states.PLAYING)
            {
                foreach (Character ch in team1)
                    ch.characterColliders.Reset();
                foreach (Character ch in team2)
                    ch.characterColliders.Reset();
            }
        }
        void SwapIfNeededAtStart(int teamID) // busca si hay otro cerca de la pelota y le da control
        {

            Character character = GetCharacterControlledFarest(teamID, ball.transform.position);
            Character newCharacter = GetNearest(teamID, false, ball.transform.position, false, true, true);

            if (character == null || newCharacter == null) return;
            if (newCharacter.data.id != character.data.id)
                SwapTo(character, newCharacter);
        }
       
        public void CharacterCatchBall(Character newCharacterWithBall)
        {
            if (newCharacterWithBall.isBeingControlled || (newCharacterWithBall.teamID == 1 && singlePlayer)) return;
            Character from = GetCharacterControlledFarest(newCharacterWithBall.teamID, newCharacterWithBall.transform.position);
            SwapTo(from, newCharacterWithBall);
        }


        public void AddCharacter(int id, int teamID)
        {
            Character character = GetNextCharacterByTeam(teamID);
            character.control_id = id;

            if(teamID == 1) 
                team1_playingCharacters.Add(character.data.id); 
            else 
                team2_playingCharacters.Add(character.data.id);  
            
            playingCharacters.Add(character);
            signals.Add(character);
            character.SetControlled(true);
        }
        int GetTeamByPlayer(int id)
        {
            return matchData.players[id - 1];
        }
        //hasControl = if true solo busca entre los activos.
        public Character GetNearestOppositeGoal(Character characterWithBall)
        {
            List<Character> team;
            if (characterWithBall.teamID == 1)  team = team1;  else team = team2;
            float distanceMin = 1000;
            Character character = null;

            Vector3 pos = new Vector3(StadiumsData.Instance.active.GetAssetBySelectedSize().size_x / 2, 0, 0);
            if (characterWithBall.teamID == 1) pos.x *= -1;

            int myCharacterID = characterWithBall.data.id;
            foreach (Character c in team)
            {
                if (c.data.id != myCharacterID)
                {
                    float distance = Vector3.Distance(c.transform.position, pos);
                    if (distanceMin > distance && c.type != Character.types.GOALKEEPER)
                    {
                        character = c;
                        distanceMin = distance;
                    }
                }
            }
            return character;
        }

        //hasControl = if true solo busca entre los activos.
        public Character GetNearestTo(Character myCharacter, int teamID, bool checkLookAt)
        {
            List<Character> team;
            if (teamID == 1)
                team = team1;
            else team = team2;
            float distanceMin = 1000;
            Character character = null;
            Vector3 myCharacterPos = myCharacter.transform.position;
            int myCharacterID = myCharacter.data.id;
            foreach (Character c in team)
            {
                if (c.data.id != myCharacterID)
                {
                    if (checkLookAt) {
                        if ((myCharacter.transform.localScale.x > 0 && c.transform.position.x < myCharacterPos.x) || (myCharacter.transform.localScale.x < 0 && c.transform.position.x > myCharacterPos.x))
                            continue;
                    }
                    float distance = Vector3.Distance(c.transform.position, myCharacterPos);
                    if (distanceMin > distance && c.type != Character.types.GOALKEEPER && c.gameObject.activeSelf)
                    {
                        character = c;
                        distanceMin = distance;
                    }
                }
            }
            return character;
        }
        public Character GetCharacterControlledFarest(int teamID, Vector3 pos)
        {
            List<Character> team;
            if (teamID == 1)
                team = team1;
            else team = team2;
            float distanceNearest = 0;
            Character character = null;
            foreach (Character c in team)
            {
                float dist = Vector3.Distance(c.transform.position, pos);
                if (c.isBeingControlled && dist > distanceNearest)
                {
                    character = c;
                    distanceNearest = dist;
                }
            }
            return character;
        }

        public Character GetNearest(int teamID, bool hasControl, Vector3 pos, bool ifHasControlGetSecond = false, bool DontGetGoalKeeper = false, bool ignoreControlls = false)
        {
            List<Character> team;
            if (teamID == 1)
                team = team1;
            else team = team2;
            float distanceMin = 1000;
            Character character = null;
            foreach (Character c in team)
            {
                float distance = Vector3.Distance(c.transform.position, pos);

                if (!c.GetColliderState()) { }
                else if (DontGetGoalKeeper && c.type == Character.types.GOALKEEPER) { }
                else if (distanceMin > distance)
                {
                    if (ifHasControlGetSecond)
                    {
                        if (!c.isBeingControlled)
                        {
                            character = c;
                            distanceMin = distance;
                        }
                    }
                    else if (ignoreControlls)
                    {
                        character = c;
                        distanceMin = distance;
                    }
                    if (hasControl && c.isBeingControlled)
                    {
                        character = c;
                        distanceMin = distance;
                    }
                    else if (!hasControl && !c.isBeingControlled)
                    {
                        character = c;
                        distanceMin = distance;
                    }
                }
            }
            return character;
        }
        Character GetNextCharacterByTeam(int teamID)
        {
            switch (teamID)
            {
                case 1:
                    team_1_id++;
                    if (team_1_id > matchData.totalCharacters_team1) team_1_id = 1;
                    return team1[team_1_id - 1];
                case 2:
                    team_2_id++;
                    if (team_2_id > matchData.totalCharacters_team2) team_2_id = 1;
                    return team2[team_2_id - 1];
            }
            return null;
        }
        public void SetPosition(int control_id, float _x, float _y)
        {
            Character character = GetPlayer(control_id);
            if (character == null || !character.isBeingControlled)
                return;

            Vector3 ballPos = ball.transform.position;
            Vector3 characterPos = character.transform.position;
            // si la agarra el arquero contrario, mi jugador se vuelve forzado para su zona:
            if (ball.character != null && ball.character.type == Character.types.GOALKEEPER)
            {
                if (ball.character.teamID != character.teamID)
                    if (character.teamID == 1) _x = 1; else _x = -1;
            } else
            // DASH TELEDIRIGIDO
            if (character.states.currentState.type == CharacterStates.types.DASH)
            {
                
                if ( Vector3.Distance(ball.transform.position, character.transform.position)<7)
                {
                    if (Mathf.Abs(ballPos.x - characterPos.x) < 0.15f) _x = 0; else if (ballPos.x < characterPos.x) _x = -1; else _x = 1;
                    if (Mathf.Abs(ballPos.z - characterPos.z) < 0.15f) _y = 0; else if (ballPos.z < characterPos.z) _y = -1; else _y = 1;
                }
            }
            if (characterPos.x >= limits.x && _x > 0 || characterPos.x <= -limits.x && _x < 0) _x = 0;
            if (characterPos.z >= limits.y && _y > 0 || characterPos.z <= -limits.y && _y < 0) _y = 0;

            Vector2 dir = GetDirectionToBall(character, ballPos, _x, _y);
            character.SetPosition(dir.x, dir.y);
            //Vector3 rot = character.characterContainer.transform.localEulerAngles;
            //rot.y += transform.position.x * rotationOffset;
            //character.characterContainer.transform.localEulerAngles = rot;
        }
        Vector2 GetDirectionToBall(Character character, Vector3 ballPos, float _x, float _z)
        {
            if (
                (_x == 0 && _z == 0)
               || (ball.character != null && ball.character.teamID == character.teamID)
               )
                return new Vector2(_x, _z);

            Vector2 char_Pos = new Vector2(character.transform.position.x, character.transform.position.z);
            Vector2 ball_Pos = new Vector2(ballPos.x, ballPos.z);

            Vector2 dir = (ball_Pos - char_Pos).normalized;
            if(Mathf.Sign(dir.x) == Mathf.Sign(_x) && Mathf.Sign(dir.y) == Mathf.Sign(_z))
            {
                dir *= 1.25f;
                if (dir.x > 1) dir.x = 1; else if (dir.x < -1) dir.x = -1;
                if (dir.y > 1) dir.y = 1; else if (dir.y < -1) dir.y = -1;
                return dir;
            }
            else
                return new Vector2(_x, _z);
        }
        public void Lujito(int control_id)
        {
            Character character = GetPlayer(control_id);
            if (character == null) return;
            if (powerupsManager != null && powerupsManager.IsCharging(character.teamID)) return;
            if (ball.character == character)
            character.Lujito();
        }
        public void ButtonPressed(int buttonID, int control_id)
        {
            cancelNextRelease = false;
            if (Fulbo.Game.GameManager.Instance.state == GameManager.states.GOAL) return;
            if (Fulbo.Game.GameManager.Instance.isTutorial && buttonID == 4) return;
            Character character = GetPlayer(control_id);

            if (powerupsManager != null && powerupsManager.CheckForPowerUp(buttonID, character, ball))
                return;

            if (character == null) return;
            if (buttonID == 3) return;
            if (buttonID == 4) return;

            if (ball.character == null || ball.character != character)
            {
                Character characterReceiver = charactersStrategy.GetReceiver();
                switch (buttonID)
                {
                    case 1: // ARCO           
                        if (characterReceiver == character)
                            charactersStrategy.SetReceiverState(CharactersStrategy.receiverStates.VOLEA);
                        else if (character.type == Character.types.GOALKEEPER)
                            character.states.Jump();
                        else if(character.states.currentState.type == CharacterStates.types.RUN)
                            character.Dash();
                        break;
                    case 2: // PASE o SWAP
                        if (character.states.currentState.type == CharacterStates.types.LUJITO) return;
                        if (Fulbo.Game.GameManager.Instance.isTutorial) return; 
                        else if (character.type == Character.types.GOALKEEPER)
                            Swap(control_id);
                        else if (characterReceiver == character)
                        {
                            print("_______________");
                            charactersStrategy.SetReceiverState(CharactersStrategy.receiverStates.PARED);
                        }
                        else //if(character.IsMoving()) 
                            Swap(control_id);
                        //else
                        //    character.Hit();
                        break;
                }
            }
            else if (ball.character == character)
            {
                if (ball.character == character && character.states.currentState.type == CharacterStates.types.LUJITO)
                {
                    switch (buttonID)
                    {
                        case 1: // ARCO    
                            ball.Kick(CharacterStates.kickTypes.VOLEA, 1.1f); return;
                        case 2: // ARCO 
                            ball.Kick(CharacterStates.kickTypes.BALOON, 1.6f); return;
                    }
                    return;
                }
                Events.PlayerProgressBarSetState(true);
                character.StartKicking(buttonID);
            }
        }
        bool cancelNextRelease;
        public void CancelNextRelease() {  cancelNextRelease = true; }
       
        public void ButtonUp(int buttonID, int id)
        {
            if (Fulbo.Game.GameManager.Instance.state == GameManager.states.GOAL) return;
            if (cancelNextRelease)
            { cancelNextRelease = false; return; }
            if (Fulbo.Game.GameManager.Instance.isTutorial && buttonID == 4) return;

            Character character = GetPlayer(id);
            if (character == null || character.states.currentState.type == CharacterStates.types.LUJITO)
            {
                Debug.Log("no hay usuario id: " + id);
                return;
            }
            if (powerupsManager != null && powerupsManager.IsCharging(character.teamID))
            {
                powerupsManager.ResetPowerBar(character.teamID);
                return;
            }
            if (buttonID == 3)
            {
                if (character.states.currentState.type != CharacterStates.types.IDLE)
                    character.SuperRun();
            }

            if (ball.character != null && ball.character.teamID == character.teamID)
            {
                Events.PlayerProgressBarSetState(false);
                switch (buttonID)
                {
                    //arco
                    case 1:
                       
                        if (character.states.currentState.type == CharacterStates.types.DASH) return;
                        float timeCatched = ball.GetDurationOfBeingCatch();
                        character.Kick(CharacterStates.kickTypes.HARD);
                        break;
                    //pasa:
                    case 2:
                        if (ball.character != character) return;
                       
                        float uiForceValue = character.ballCatcher.GetForce();
                        CheckPassBall(character, uiForceValue);
                        //if (!Pasar(character, uiForceValue))
                        //{
                        //    if (character.GetPosition() == Character.PositionsInGame.CENTRO 
                        //        && !character.ballCatcher.HasCharacterInPase())
                        //    {
                        //        Vector3 centroPos = character.transform.position;
                        //        centroPos.x *= 0.95f;
                        //        centroPos.z *= -0.86f;
                        //        character.ballCatcher.LookAt(centroPos);
                        //        character.Kick(CharacterStates.kickTypes.CENTRO);
                        //    }
                        //    else // pase sin dirección:
                        //    {
                        //        if (uiForceValue > 0.6f)
                        //            character.Kick(CharacterStates.kickTypes.BALOON);
                        //        else
                        //            character.Kick(CharacterStates.kickTypes.SOFT);
                        //    }
                        //}
                        break;
                }
            }
        }
        bool CheckPassBall(Character character, float uiForceValue)
        {
            Character characterNear;

            CharacterStates.kickTypes kickType;

            if (uiForceValue > 0.5f)
                kickType = CharacterStates.kickTypes.BALOON;
            else
                kickType = CharacterStates.kickTypes.SOFT;

            if (character.GetPosition() == Character.PositionsInGame.CENTRO 
                && (uiForceValue>0.5f || !character.ballCatcher.HasCharacterInPase())
                )
            {
                kickType = CharacterStates.kickTypes.CENTRO;
                characterNear = GetNearestOppositeGoal(character);
            }
            else
                characterNear = charactersStrategy.GetCharacterEnPase(character);            

            if (characterNear == null) characterNear = GetNearestTo(character, character.teamID, true);
            if (characterNear == null) characterNear = GetNearestTo(character, character.teamID, false);
            if (characterNear != null)// && characterNear.type != Character.types.GOALKEEPER)
            {
                character.ballCatcher.LookAt(characterNear.transform.position);
                Character characterToPassInTeam = GetCharacterToPassInTeam(character);

                if (character.teamID == 1 && totalUsedPlayers_team1>1 || character.teamID == 2 && totalUsedPlayers_team2>1)   
                    SwapTo(characterToPassInTeam, characterNear);
                else
                    SwapTo(character, characterNear);

                float dist = Vector3.Distance(character.transform.position, characterNear.transform.position);

                character.Kick(kickType, 1 + (dist / 10));

                ball.PaseTo(characterNear);
                return true;
            }
            return false;
        }
        Character GetCharacterToPassInTeam(Character character)
        {
            if(character.teamID == 1)
            {
                foreach (Character ch in team1)
                    if (ch.control_id != character.control_id)
                        return ch;
            }
            else
            {
                foreach (Character ch in team2)
                    if (ch.control_id != character.control_id)
                        return ch;
            }
            return character;
        }
        public void Swap(int control_id)
        {
            Character character = GetPlayer(control_id);
            Character newCharacter = GetNearest(character.teamID, false, ball.transform.position, true, true, true);

            if (newCharacter != character)
            {
                AudioManager.Instance.PlaySound("ui", "ui/swapPlayer", false);
                SwapTo(character, newCharacter);
            }
        }

        public void SwapTo(Character from, Character to)
        {   
             print("SwapTo");
            if (to == null || to.isBeingControlled) return;
            if (from == null) return;
            if (from.teamID != to.teamID) return;
             print("SwapTo  " + from.control_id  + "to: " + to.control_id);
            if (from.control_id == 0 && to.control_id == 0) return;

            if (powerupsManager != null) powerupsManager.ResetPowerBar(from.teamID);

            print("SwapTo from: " + from.avatarName + " to: " + to.avatarName);

            int teamID = from.teamID;
            to.control_id = from.control_id;
            signals.ChangeSignal(from, to);

            from.control_id = 0;
            playingCharacters.Remove(from);
            playingCharacters.Add(to);

            from.SetControlled(false);
            to.SetControlled(true);
            Events.SwapCharacter(to);
        }
        Character GetPlayer(int control_id)
        {
            if (control_id == 0) return null;

            foreach (Character character in playingCharacters)
                if (character.control_id == control_id)
                    return character;

            return null;
        }
        void KickToGoal()
        {
            if (ball.character != null)
                ball.character.Kick(CharacterStates.kickTypes.KICK_TO_GOAL);
        }
        public List<Character> GetCharactersByTeam(int teamID)
        {
            if (teamID == 1) return team1; else return team2;
        }
        public Character GetGoalkeeper(int teamID)
        {
            if (teamID == 1) return team1[0]; else return team2[0];
        }
        public Character GetCharactersByTeam(int teamID, int orderID)
        {
            return GetCharactersByTeam(teamID)[orderID];
        }
        public Character GetCharacterByType(Character.types type, int teamID)
        {
            List<Character> team;
            if (teamID == 1) team = team1; else team = team2;
            foreach (Character character in team)
            {
                if (character.type == type)
                    return character;
            }
            return null;
        }

        int rep = 0;
        public Character GetOponentFor(Character ch)
        {
            rep = 0;
            Character oponent = GetOponentForType(ch, ch.type);
            // Debug.Log("ch " + ch.teamID + " myType: " + ch.type + " fieldPosition: " + ch.fieldPosition + " oponent: " + oponent.teamID + " type: " + oponent.type + " fieldPosition: " + oponent.fieldPosition);
            return oponent;
        }
        Character GetOponentForType(Character ch, Character.types myType)
        {
            rep++;
            List<Character> all;

            if (ch.teamID == 1) all = team2; else all = team1;
            foreach (Character c in all)
            {
                bool thisOponentIsTaken = ThisOponentIsTaken(ch.teamID, c);
                if (c.type == Character.types.DEF && myType == Character.types.FOR && (!thisOponentIsTaken || rep > 5))
                    return c;
                else if (c.type == Character.types.FOR && myType == Character.types.DEF && (!thisOponentIsTaken || rep > 5))
                    return c;
                else if (c.type == Character.types.MID && myType == Character.types.MID && (!thisOponentIsTaken || rep > 5))
                    return c;
                else if (c.type == Character.types.GOALKEEPER && myType == Character.types.GOALKEEPER)
                    return c;
                else if (rep > 9)
                    return c;
            }

            if (myType == Character.types.FOR) myType = Character.types.MID;
            else if (myType == Character.types.DEF) myType = Character.types.MID;
            else if (Random.Range(0, 10) < 5)
                myType = Character.types.DEF;
            else
                myType = Character.types.FOR;

            return GetOponentForType(ch, myType);

        }
        bool ThisOponentIsTaken(int teamID, Character oponent)
        {
            List<Character> all;
            if (teamID == 1) all = team1; else all = team2;
            foreach (Character ch in all)
                if (ch.oponent == oponent)
                    return true;
            return false;
        }


       
    }
}