using Fulbo.Stadiums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class GameOverPartymode : MonoBehaviour
    {
        [SerializeField] Text team1Result;
        [SerializeField] Text team2Result;

        void Start()
        {
            Events.OnOutroSound();
            AudioManager.Instance.PlaySound("music", "music/music_summary", true);
            AudioManager.Instance.ChangeVolume("music", 1);

            team1Result.text = Data.Instance.matchData.score[0].ToString();
            team2Result.text = Data.Instance.matchData.score[1].ToString();
        }
      
    }
}
