using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Fulbo.Game
{
    public class Momentums : MonoBehaviour
    {
        Camera cam;
        float cam_size;
        void Start()
        {
            cam = Fulbo.Game.GameManager.Instance.cameraInGame.cam;
            Events.OnBallKicked += OnBallKicked;
        }
        private void OnDestroy()
        {
            Events.OnBallKicked -= OnBallKicked;
        }
        void OnBallKicked(CharacterStates.kickTypes kickType, float value, Character character)
        {
            StopAllCoroutines();
            cam.orthographicSize = Fulbo.Game.GameManager.Instance.cameraInGame.GetOriginalSize();

            cam_size = cam.orthographicSize;
            if (kickType == CharacterStates.kickTypes.CHILENA)
                StartCoroutine(Ralenta());
        }
        IEnumerator Ralenta()
        {
            yield return new WaitForEndOfFrame();
        }
    }

}