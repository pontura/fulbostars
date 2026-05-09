using UnityEngine;
using UnityEditor;

namespace Fulbo.Game.Tutorial
{
    public class Tutorial_Run : TutorialStep
    {
        int hits;

        public override void Setup(CharactersManagerTutorial charactersManagerTutorial, GameObject asset)
        {
            SetProgress(0);

            charactersManagerTutorial.character.transform.localPosition = new Vector3(-0.9f, 0.55f, 0);
            Fulbo.Game.GameManager.Instance.cameraInGame.OnSetTarget(charactersManagerTutorial.character.transform);
            


            SetInitCharacter(charactersManagerTutorial.character);

            SetLimitsTo(new Vector2(-1,17), new Vector2(3, -12), charactersManagerTutorial.character);
        }
        public override void OnInit() {
            hits = 0;
            Events.OnBallHitObstacle += OnBallHitObstacle;
            Events.OnCharacterHitTrigger += OnCharacterHitTrigger;
        }
        public override void OnReset()
        {
            Events.OnBallHitObstacle -= OnBallHitObstacle;
            Events.OnCharacterHitTrigger -= OnCharacterHitTrigger;
        }
        void OnBallHitObstacle(GameObject go)
        {
            hits++;
        }
        void OnCharacterHitTrigger(GameObject go)
        {
            if(go.name == "Zone")
            {
                go.GetComponentInChildren<Animator>().Play("end");
                if(hits>0)
                    Lose();
                else
                    Done();
            }
        }
    }
}