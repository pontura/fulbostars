using System;
using System.Collections.Generic;
using Fulbo.AssetsBundle;
using Fulbo.DB;
using UnityEngine;

namespace Fulbo.Tournamets
{
    public class TournamentsData : MonoBehaviour
    {
        public TData data;
        public TextAsset fileAsset;

        public bool isTournament = true;
        public int myTeamID; // 1 o 2 (left o right)

        public int goles1; // total goles del torneo, no se resetea cada partido
        public int goles2; // total goles del torneo, no se resetea cada partido

        public int lastMatchGoles1; // total goles del partido, se resetea cada partido
        public int lastMatchGoles2; // total goles del partido, se resetea cada partido

        public int gamesPlayed;
        public bool played;


        void Start()
        {
            myTeamID = 0;
            DBManager.Instance.tournamentsManager.GetResults("torneo1", OnLoadedResults);
        }
         [Serializable]
        public class TData
        {
            public TsData[] tournaments;
        }
         [Serializable]
         public class TsData
        {
            public string id;
            public string name;
            public string[] team1_win;
            public string[] team2_win;
        }
       
        System.Action OnDone;
        public void Init(System.Action OnDone)
        {
            this.OnDone = OnDone;
            if (Data.Instance.loadType == Data.loadTypes.LOCAL || Data.Instance.loadType == Data.loadTypes.DATABASE)
            {
                AllLoaded(fileAsset.text);
            }
            else
            {
                print("tournaments_texts_" + Data.Instance.langsManager.GetLang() + ".json");
                AssetsBundleLoader abl = AssetsBundleManager.Instance.assetsBundleLoader;
                AllLoaded(abl.GetJsonText("tournaments_texts_" + Data.Instance.langsManager.GetLang() + ".json"));
            }
        }
         private void AllLoaded(string text)
        {
            if(Data.Instance.settings.mainSettings.isArcade)
                isTournament = false;
            data = JsonUtility.FromJson<TData>(text);
            OnDone();
        }
        System.Action OnRefreshed;
        public void Refresh(System.Action OnRefreshed)
        {
            this.OnRefreshed = OnRefreshed;
            DBManager.Instance.tournamentsManager.GetResults("torneo1", OnLoadedResults);
        }
        private void OnLoadedResults(bool success, int goles1, int goles2, int gamesPlayed)
        {
            this.goles1 = goles1;
            this.goles2 = goles2;
            this.gamesPlayed = gamesPlayed;
            if(OnRefreshed != null)
                OnRefreshed.Invoke();
        }

        public bool IsTournament()
        {
            return isTournament;
        }
        public void SetTeam(int teamID)
        {
            if(teamID>0)
            {
                isTournament = true;
                this.myTeamID = teamID;
            } else
                isTournament = false;
#if !UNITY_STANDALONE
            if(teamID ==2) // team invertido, el team1 es el derecho y el team2 el izquierdo
                GetComponent<MatchData>().players[0] = 1;
            else
                GetComponent<MatchData>().players[0] = 1;
#endif
        }
        public void SetTournament(bool isTournament)
        {
            this.isTournament = isTournament;
        }

        public string[] GetRandomFrases(int torneoID, int teamID)
        {
            switch(teamID)
            {
                case 1: return data.tournaments[torneoID].team1_win;
                default: return data.tournaments[torneoID].team2_win;
            }
        }
        public void Played()
        {
            played = true;
        }
    }
}