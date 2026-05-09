using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Fulbo.DB;
using static Fulbo.DB.DBUserData;
using static Fulbo.DB.DBGameData.DBFormationSave;

namespace Fulbo.Game
{
    public class CharactersPositions : DataLoader
    {
        [SerializeField]  PlayerPositionData playerSave = null;
        public TextAsset[] files;

        //  public bool reLoad = true;
        public All team1;
        public All team2;
        
        [Serializable]
        public class All
        {
            public PositionsData[] all;
        }
        [Serializable]
        public class PositionsData
        {
            public int id;
            public string name;
            public CharacterPositionData[] posData;
        }

        [Serializable]
        public class PlayerPositionData
        {
            public List<CharacterPositionData> posData = new List<CharacterPositionData>();

            public PlayerPositionData() { }

            public PlayerPositionData(List<CharacterPositionData> posData, bool loadSelected = false)
            {
                this.posData = posData;

                if (!loadSelected)
                    return;

                print("PlayerPositionData " + posData.Count);
                // List<DBUserData.DBCharacterData> ids = DBManager.Instance.DbUserData.data.GetSelectedTeam();
                List<DBUserData.DBCharacterData> ids = Data.Instance.myTeam.GetBestTeamDataPlayersID(posData.Count);

                for (int i = 0; i < posData.Count; i++)
                {
                    if(i<ids.Count)
                        posData[i].uniqueId = ids[i].id;
                }
            }

            public PositionsData ToPositions()
            {
                PositionsData data = new PositionsData();
                data.posData = this.posData.ToArray();
                data.posData[0].type = Character.types.GOALKEEPER; // TO-DO
                return data;
            }

            public List<DBCharacterData> ToCharData(CharactersPositions.PlayerPositionData playerPositionData)
            {
                List<DBCharacterData> data = new List<DBCharacterData>();
                int arr_id = 0;
                foreach (CharacterPositionData id in playerPositionData.posData)
                {
                    DBCharacterData dbCharacterData = DBManager.Instance.DbUserData.data.GetPlayerByID(id.uniqueId);
                    if(dbCharacterData == null)
                    {
                        Debug.LogError("Ya no hay jugador id: " + id.uniqueId);
                        dbCharacterData = GetNewCharacterForReplacement(playerPositionData.posData, arr_id == 0);
                    }
                    arr_id++;
                    if (dbCharacterData != null)
                        data.Add(dbCharacterData);
                }
                return data;
            }
            DBCharacterData GetNewCharacterForReplacement(List<CharacterPositionData> team, bool isGoalkeeper)
            {
                List<DBCharacterData> arr;
                if(isGoalkeeper)
                    arr =  DB.DBManager.Instance.DbUserData.data.players_goalkeepers;
                else
                    arr = DB.DBManager.Instance.DbUserData.data.players_characters;
                int id = 0;
                foreach (DBCharacterData dbCharacterData in arr)
                {
                    if (!IsOnTeam(dbCharacterData.id, team))
                    {
                        team[id].uniqueId = dbCharacterData.id;
                        return dbCharacterData;
                    }
                    id++;
                }
                return null;
            }
            bool IsOnTeam(int uniqueID, List<CharacterPositionData> team)
            {
                foreach (CharacterPositionData characterPositionData in team)
                {
                    if (uniqueID == characterPositionData.uniqueId)
                        return true;
                }
                return false;
            }
            public DB.DBGameData.DBFormationSave ToDBSave()
            {
                DB.DBGameData.DBFormationSave save = new DB.DBGameData.DBFormationSave();

                DBFormationChar[] characters = new DBFormationChar[posData.Count];

                for(int i = 0; i < characters.Length; i++)
                {
                    characters[i] = new DBFormationChar();
                    characters[i].pos = posData[i].pos;
                    characters[i].uniqueID = posData[i].uniqueId;
                }

                save.formation = characters;
                return save;
            }
        }

        [Serializable]
        public class CharacterPositionData
        {
            public Character.types type;
            public float[] pos;
            public int uniqueId;

            public int GetHappiness(int originalTypeIDByPosition, Character.types characterPositionData)
            {
                int happinesID = 0;
                if (originalTypeIDByPosition == 0)
                {
                    if (characterPositionData == Character.types.MID) happinesID = 1;
                    else if (characterPositionData == Character.types.FOR) happinesID = 2;
                }
                else if (originalTypeIDByPosition == 1)
                {
                    if (characterPositionData == Character.types.DEF || characterPositionData == Character.types.FOR) happinesID = 1;
                }
                else if (originalTypeIDByPosition == 2)
                {
                    if (characterPositionData == Character.types.DEF) happinesID = 2;
                    else if (characterPositionData == Character.types.MID) happinesID = 1;
                }
                return happinesID;
            }
        }
        public override void LoadData(Action _OnReady)
        {
            LoadPositions();
            if (_OnReady != null)
                _OnReady();
        }
        public void LoadPositions()
        {
            team1 = JsonUtility.FromJson<All>(files[Data.Instance.matchData.totalCharacters_team1].text);
            team2 = JsonUtility.FromJson<All>(files[Data.Instance.matchData.totalCharacters_team2].text);            
        }

        public PositionsData GetPositionsData(int teamID, int id)
        {

            // print("teamID: " + teamID + " id:" + id);

            All team;
            if (teamID == 1)
                team = team1;
            else
                team = team2;

            foreach (PositionsData p in team.all)
            {
                if (p.id == id)
                {
                    return p;
                }
            }
            Debug.LogError("No positions for  teamID: " + teamID + " charactersPositions_id: " + id);
            return null;
        }

        public PositionsData GetPositionData(int opponentsQty, int id)
        {
            TextAsset textAsset = files[opponentsQty];
            All team = JsonUtility.FromJson<All>(textAsset.text);

            foreach (PositionsData p in team.all)
            {
                if (p.id == id)
                {
                    return p;
                }
            }
            Debug.LogError("No positions for opponentsQty: " + opponentsQty + " id: " + id);
            return null;
        }
        public PlayerPositionData GetPvpOpponentPositionData()
        {
            DBGameData.DBFormationSave dSave = Data.Instance.pvpData.data.user.gameData.formation5;
            return dSave.ToPosition();
        }
        public PlayerPositionData GetPlayerPositionData()
        {

            int _numberOfPlayersInTeam;
            if (Data.Instance.mode == Data.modes.PARTYMODE)
                _numberOfPlayersInTeam = Data.Instance.matchData.GetTotalPlayersInMatch(1);
            else
                _numberOfPlayersInTeam = Mathf.Min(Data.Instance.matchData.GetTotalPlayersInMatch(1), DB.DBManager.Instance.DbUserData.data.players_characters.Count + 1);

            if (_numberOfPlayersInTeam <= 5) _numberOfPlayersInTeam = 5; // fixes the tutorial match:

            Debug.Log("Number Of Players In Team: " + _numberOfPlayersInTeam);
            PlayerPositionData savedFormation = DB.DBManager.Instance.DbGameData.GetFormation(_numberOfPlayersInTeam);
            if(savedFormation != null)
                return savedFormation; 
            else // si no existe la formacion:
            {
               // Debug.Log("Generate new Formation for " + _numberOfPlayersInTeam + " players");
                List<DBUserData.DBCharacterData> ids = Data.Instance.myTeam.GetBestTeamDataPlayersID(_numberOfPlayersInTeam);
                int totalPlayers = Mathf.Min(_numberOfPlayersInTeam, ids.Count);
                CharacterPositionData[] cData = Data.Instance.charactersPositions.GetPositionData(totalPlayers, 0).posData;

                int id = 0;
                foreach (CharacterPositionData d in cData)
                {
                    if (id < ids.Count)
                    {
                        d.uniqueId = ids[id].id;
                        id++;
                    }
                    else
                        break;
                }
                playerSave = new PlayerPositionData(cData.ToList(), true);
                DB.DBManager.Instance.DbGameData.PutFormation(playerSave.posData.Count, playerSave, null); // Force save to DB
                return playerSave;
            }           
        }


        public void SaveNewPositions()
        {
            if (playerSave != null && playerSave.posData.Count > 0)
                DB.DBManager.Instance.DbGameData.PutFormation(playerSave.posData.Count, playerSave, null);
            playerSave = null;
        }

        public void SavePlayerPositionData(Fulbo.UI.PositionsUIManager manager)
        {
            if (playerSave == null)
                playerSave = new PlayerPositionData();
            playerSave.posData = new List<CharacterPositionData>();
            int _numberOfPlayersInTeam = Mathf.Min(Data.Instance.matchData.GetTotalPlayersInMatch(1), DB.DBManager.Instance.DbUserData.data.players_characters.Count + 1);
            
            int id = 0;
            foreach (DBUserData.DBCharacterData dbData in Data.Instance.myTeam.GetBestTeamDataPlayersID(_numberOfPlayersInTeam))
            {
                UI.PositionsUIThumb thumb = GetP(manager, dbData.id);
                if (thumb != null)
                {
                    playerSave.posData.Add(thumb.characterPositionData);
                }
                id++;
            }
        }
        UI.PositionsUIThumb GetP(Fulbo.UI.PositionsUIManager manager, int uniqueID)
        {
            foreach (UI.PositionsUIThumb p in manager.all)
            {
                if (p.uniqueID == uniqueID)
                    return p;
            }
            return null;
        }

    }
}