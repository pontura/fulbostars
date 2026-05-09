using UnityEngine;

namespace Fulbo.Game
{
    public class GameState : MonoBehaviour
    {
        GoalMoment goalState;

        private void Start()
        {
            Events.OnGoalDone += OnGoalDone;
        }
        private void OnDestroy()
        {
            Events.OnGoalDone -= OnGoalDone;
        }
        public void OnGoalDone()
        {
            if (goalState != null)
            {
                goalState.OnDone();
                goalState = null;
            }
        }
        public void InitGoalMoment(int teamId, Character character)
        {
            StopAllCoroutines();
            goalState = new GoalMoment();
            StartCoroutine(goalState.Init(this, teamId, character));
            StartCoroutine(goalState.UpdateC());
        }
    }
}
