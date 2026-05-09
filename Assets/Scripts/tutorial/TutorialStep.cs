using UnityEngine;
using System.Collections;
using Fulbo.Game;

namespace Fulbo.Game.Tutorial
{ 
    public class TutorialStep
    {
        protected float timer = 0;
        public TutorialStepsManager manager;
        Character character_to_start;

        public virtual Vector3 GetOriginalPos()  {    return Vector3.zero;    }

        public void Init(TutorialStepsManager manager)
        {
            timer = 0;
            this.manager = manager;
            Fulbo.Game.GameManager.Instance.ball.rb.velocity = Vector3.zero;
            OnInit();
        }
        public virtual void Setup(CharactersManagerTutorial charactersManagerTutorial, GameObject asset)  { }
        public void OnSetInit()
        {
            manager.OnSetInit();
        }
        public virtual void Done()
        {
            manager.OnSetDone(true);
            OnReset();
        }
        public void Lose(int falseID = 0)
        {
            manager.OnSetDone(false, falseID);
            OnReset();
        }
        public virtual void OnReset() {  }
        public virtual void OnInit()   { }
        public void SetInitCharacter(Character character)
        {
            character_to_start = character;
            if (character.isBeingControlled) return;
            if (character_to_start.teamID == 2)
            {
                Fulbo.Game.GameManager.Instance.charactersManager.Swap(1);
            }
        }
        public void PassBallToCharacter()
        {
            character_to_start.characterColliders.Reset();
            Fulbo.Game.GameManager.Instance.ball.PaseTo(character_to_start);
        }
        public virtual void OnUpdate()
        {
            timer += Time.deltaTime;
        }
        public void SetProgress(int totalSteps)
        {
            Events.OnTutorialProgress(totalSteps, Done);
        }
        public void SetLimitsTo(Vector2 _x, Vector2 _y, Character character)
        {
            character.limits_x = _x;
            character.limits_y = _y;
        }
    }

}