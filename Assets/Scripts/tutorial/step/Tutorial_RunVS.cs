using UnityEngine;
using UnityEditor;

namespace Fulbo.Game.Tutorial
{
    public class Tutorial_RunVS : TutorialStep
    {
        public override void Setup(CharactersManagerTutorial charactersManagerTutorial, GameObject asset)
        {
            SetProgress(0);

            charactersManagerTutorial.character.transform.localPosition = GetOriginalPos();
            Vector3 pos = charactersManagerTutorial.character.transform.position;
            pos.z = 1;
            Character otherCharacter = charactersManagerTutorial.AddCharacter(pos, 1, false);
            otherCharacter.ai.SetOn(false);
            PathInCharacter pathInCharacter = otherCharacter.gameObject.AddComponent<PathInCharacter>();
            TutorialPath tutorialPath = asset.GetComponent<TutorialPath>();
            pathInCharacter.SetOffset(new Vector3(0, 0, 1));
            pathInCharacter.Init(tutorialPath.GetPath(), false, manager.OnLose);
            pathInCharacter.SetCollidersOFF();
            pathInCharacter.SetSpeed(charactersManagerTutorial.character.speed);
            Fulbo.Game.GameManager.Instance.cameraInGame.OnSetTarget(charactersManagerTutorial.character.transform);

            SetInitCharacter(charactersManagerTutorial.character);

            SetLimitsTo(new Vector2(-1,22), new Vector2(3, -12), charactersManagerTutorial.character);

            charactersManagerTutorial.character.stats.speedRunWithBall = 6.2f;
        }
        public override Vector3 GetOriginalPos()
        {
            return new Vector3(-0.9f, 0.55f, -1);
        }
        public override void OnInit()
        {
            Events.OnCharacterHitTrigger += OnCharacterHitTrigger;
        }
        public override void OnReset()
        {
            Events.OnCharacterHitTrigger -= OnCharacterHitTrigger;
        }
        void OnCharacterHitTrigger(GameObject go)
        {
            if (go.name == "Zone")
            {
                go.GetComponentInChildren<Animator>().Play("end");
                Done();
            }
        }
        
    }
}