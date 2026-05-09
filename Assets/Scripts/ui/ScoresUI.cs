using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Fulbo.UI
{
    public class ScoresUI : MonoBehaviour
    {
        public ScoreIngameUI scoreUI;

        void Start()
        {
            //if(Data.Instance.isMobile)
            Events.OnGoldScoreWin += OnGoldScoreWin;
        }
        void OnDestroy()
        {
            Events.OnGoldScoreWin -= OnGoldScoreWin;
        }
        void OnGoldScoreWin(int value, Vector3 pos)
        {
            if (pos == Vector3.zero)
                return;
            Vector3 posCanvas = Fulbo.Game.GameManager.Instance.cameraInGame.cam.WorldToScreenPoint(pos);

            ScoreIngameUI s = Instantiate(scoreUI, transform);
            s.transform.position = posCanvas;
            s.Init(value);
        }
    }

}