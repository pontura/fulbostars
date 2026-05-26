using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Fulbo.Tournamets
{
    public class TournamentsManager : MonoBehaviour
    {        
        string firebaseURL = "https://us-central1-fulbo-stars.cloudfunctions.net/";

        
        [System.Serializable] public class TournamentResponse { public Team team1; public Team team2; public int partidos_jugados; }
        [System.Serializable] public class Team { public int goles; }

        public void GetResults(string torneoId, System.Action<bool, int, int, int> onDone)
        {
            print ("Get Torneo Results torneoId:" + torneoId);
            StartCoroutine(GetTournamentDataC(torneoId, onDone));
        }

        IEnumerator GetTournamentDataC(string torneoId, System.Action<bool, int, int, int> onDone)
        {
            string url = firebaseURL + "getTournamentData?torneoId=" + torneoId;

            using UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                print($"GetTournamentData ERROR {request.responseCode}: {request.error}");
                onDone?.Invoke(false, 0, 0, 0);
                yield break;
            }

            print("GetTournamentDataC " + url + " | " + request.downloadHandler.text);
            // {"torneoId":"torneo1","team1":{"goles":2},"team2":{"goles":1},"partidos_jugados":1}
            TournamentResponse data = JsonUtility.FromJson<TournamentResponse>(request.downloadHandler.text);
            print("GetTournamentDataC " +data);
            print("GetTournamentDataC " +onDone);
            onDone?.Invoke(true, data.team1.goles, data.team2.goles, data.partidos_jugados);
        }



       
       


         public void OnSendResults(System.Action<bool> OnResultSaved)
        {
            print("TournamentsData OnGameOver");
            string torneoId = "torneo1";
            int golesTeam1 = (int)Data.Instance.matchData.score[0];
            int golesTeam2= (int)Data.Instance.matchData.score[1];
            
            Data.Instance.tournamentsData.lastMatchGoles1 = golesTeam1;
            Data.Instance.tournamentsData.lastMatchGoles2 = golesTeam2;

            SubmitMatchResult(torneoId, golesTeam1, golesTeam2, OnResultSaved);
        }
        public void SubmitMatchResult(string torneoId, int golesTeam1, int golesTeam2, System.Action<bool> onDone)
        {
            print ("SubmitMatchResultC torneoId:" + torneoId + " golesTeam1: " + golesTeam1+ " golesTeam2: " +  golesTeam2+ " " +  onDone);
            StartCoroutine(SubmitMatchResultC(torneoId, golesTeam1, golesTeam2, onDone));
        }
        IEnumerator SubmitMatchResultC(string torneoId, int golesTeam1, int golesTeam2, System.Action<bool> onDone)
        {
            string url = firebaseURL + "submitMatchResult";
            string body = $"{{\"torneoId\":\"{torneoId}\",\"golesTeam1\":{golesTeam1},\"golesTeam2\":{golesTeam2}}}";

            print("url = " + url + " body = " + body);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);

            using UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            bool ok = request.result == UnityWebRequest.Result.Success;
            if (!ok)
                print($"SubmitMatchResult ERROR {request.responseCode}: {request.error} | {request.downloadHandler.text}");
            onDone?.Invoke(ok);
        }
    }
}
