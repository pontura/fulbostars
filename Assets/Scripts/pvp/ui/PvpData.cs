using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Fulbo.DB;
using static Fulbo.DB.DBMundial;
using static Fulbo.DB.DBGameData.DBFormationSave;
using System.Collections.Generic;

namespace Fulbo.UI.Pvp
{
    public class PvpData : MonoBehaviour
    {
        public ClubData clubData;
        public DBUserData.DataFromServer data;
        public LevelData levelData;
        public List<DBUserData.DBCharacterData> formation;

        public LevelData GetLevelData()
        {
            //TO-DO:
           // Data.Instance.matchData.SetActualStadium(1);
           // Data.Instance.matchData.SetActualLevel(1);
            levelData = CupsData.Instance.GetActualLevel();
            levelData.name = data.user.shortName;
            return levelData;
        }
        public List<DBUserData.DBCharacterData> GetCharactersData()
        {
            formation = new List<DBUserData.DBCharacterData>();
            int id = 0;
            foreach (DBFormationChar data in data.user.gameData.formation5.formation)
            {
                DBUserData.DBCharacterData dbCharacterData;
                if (id == 0) dbCharacterData = GetPlayer(data.uniqueID, true);
                else dbCharacterData = GetPlayer(data.uniqueID, false);
                formation.Add(dbCharacterData);
                id++;
            }
            return formation;
        }
        public DBUserData.DBCharacterData GetPlayer (int id, bool isGoalkeeper)
        {
            List<DBUserData.DBCharacterData> all;
            if (isGoalkeeper) all = data.goalkeepers;
            else all = data.players;
            foreach (DBUserData.DBCharacterData d in all)
                if (d.id == id)
                    return d;
            Debug.LogError("No opponent for PVP Team id: " + id + " isGoalkeeper : " + isGoalkeeper);
            return all[0];
    }
        public void Load(System.Action OnSuccess, string email)
        {
            StartCoroutine(LoadC(OnSuccess, DBManager.Instance.UrlRegister + email));
        }
        IEnumerator LoadC(System.Action OnSuccess, string url)
        {
            WWWForm form = new WWWForm();
            UnityWebRequest www = UnityWebRequest.Get(url);

            print("[GET] " + url);

            yield return www.SendWebRequest();

            if (www.isNetworkError)
                Debug.LogError(string.Format("{0}: {1}", www.url, www.error));
            else
            {
                string s = www.downloadHandler.text;
                Debug.Log(s);
                DBUserData.DataFromServer d = JsonUtility.FromJson<DBUserData.DataFromServer>(s);
                data = d;
                clubData.SetDataFromString(data.user.style);
                ParseContent(d.user.game_data);
                OnSuccess();
            }
        }
        void ParseContent(string game_data)
        {
            if (game_data == null || game_data.Length < 2) return;
            Debug.Log("DBGameData PARSE: " + game_data);
            try
            {
                data.user.gameData = JsonUtility.FromJson<DBGameData.Content>(game_data);
            }
            catch
            {
                Debug.Log("Not json");
            }
        }
    }
}
